//Add Reference
//css_reference System.Core;
//css_reference System.Data;
//css_reference System.Data.DataSetExtensions;
//css_reference System.Xml;
//css_reference System.Xml.Linq;

//Import

using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Threading;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Text;
using KSMART.BRM;
using KSMART.BRM.Core;
using System.IO;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Timers;

public class Script
{
    private const int MIN_WAIT_VALUE = 5000;

    private static System.Timers.Timer timer = new System.Timers.Timer(MIN_WAIT_VALUE);

    public static void Review_DefectSelected(KSMART.BRM.Script.AOIReviewDefectSelectedArgs args)
    {
        // Execute auto classification function
        Review_AutoClassification(args);
    }

    private static void Review_AutoClassification(KSMART.BRM.Script.AOIReviewDefectSelectedArgs args)
    {
        // Check if auto classification is enabled in customer property
        if ((args.CustomerProperty.GetValue("AutoClassificationEnable").ToBoolean() == true) &&
            (args.CustomerProperty.GetValue("AutoClassificationLock").ToBoolean() == true) &&
            (args.KeyValue.GetValue("AutoClassificationStatus") != "Inactive"))
        {
            try
            {
                var table = args.CustomerDataGrid.GetDataSource("AutoClassification");

                string inspType = args.AOI.Codes.GetInspectTypeName(args.LeadName.ToInt(), args.InspType);
                string packageType = AutoClassification_GetPackageType(args, args.ComponentGUID);

                // Get defect conditions
                var conditions = table.Select(
                    string.Format("Name IS NULL OR Name = '' " +
                    "OR (Type = 'Component' AND Name = '{0}') " +
                    "OR (Type = 'Part' AND Name = '{1}') " +
                    "OR (Type = 'Package' AND Name = '{2}') ",
                    args.ComponentName, args.PartNumber, packageType)
                );

                if (conditions.Length > 0)
                {
                    foreach (DataRow row in conditions)
                    {
                        var defects = row["Defect"].ToString();

                        if (defects.Contains(inspType) == true)
                        {
                            AutoClassification_Lock(args, args.ComponentName, inspType);

                            break;
                        }
                        else
                        {
                            AutoClassification_Unlock(args, args.ComponentName, inspType);
                        }
                    }
                }
                else
                {
                    AutoClassification_Unlock(args, args.ComponentName, inspType);
                }
            }
            catch (Exception ex)
            {
                args.Log.WriteError("AutoClassification.Lock", ex.Message, ()=>machine(args.Lane));
            }
        }
    }

    private static void AutoClassification_Lock(KSMART.BRM.Script.AOIReviewDefectSelectedArgs args, string ComponentName, string InspType)
    {
        args.Log.WriteExecute("AutoClassification.Lock", ComponentName, ()=>machine(args.Lane, args.DetailGUID, ComponentName, InspType));

        AutoClassification_Timer(args);

        args.AOI.Review.RtCmdMoveNextDefect("REVIEW");
        args.AOI.Review.RtCmdLock("REVIEW", "MES I/F", string.Format("Auto Classify ({0}: {1})", ComponentName, InspType));

        //AutoClassification_Update(args, ComponentName, InspType);
    }

    private static void AutoClassification_Timer(KSMART.BRM.Script.AOIReviewDefectSelectedArgs args)
    {
        timer.Stop();
        timer.Elapsed += AutoClassification_Disable;
        timer.Start();
    }

    private static void AutoClassification_Disable(object sender, ElapsedEventArgs e)
    {
        var args = new KSMART.BRM.Script.ScriptArgs();

        args.Log.WriteExecute("AutoClassification.Status", "Inactive");

        args.KeyValue.SetValue("AutoClassificationStatus", "Inactive");

        timer.Stop();

        args.AOI.Review.RtCmdMoveNextDefect("REVIEW");
        args.AOI.Review.RtCmdLockJudgment("REVIEW");
        args.AOI.Review.RtCmdUnLock("REVIEW");
    }

    private static void AutoClassification_Unlock(KSMART.BRM.Script.AOIReviewDefectSelectedArgs args, string ComponentName, string InspType)
    {
        if (timer.Enabled == true)
        {
            args.Log.WriteExecute("AutoClassification.Unlock", ComponentName, ()=>machine(args.Lane, args.DetailGUID, ComponentName, InspType));

            timer.Stop();

            args.AOI.Review.RtCmdUnLock("REVIEW");
        }
    }

    private static void AutoClassification_Update(KSMART.BRM.Script.AOIReviewDefectSelectedArgs args, string ComponentName, string InspType)
    {
        var connection = new SqlConnection(args.AOI.Settings.InspectResult.ConnString);
        var resultDBName = args.AOI.GetPCB(args.PCBGUID).Tables["PCB"].Rows[0]["ResultDBName"].ToString();
        var defectCode = AutoClassification_GetDefectCode(args, args.LeadName, args.InspType);

        try
        {
            connection.Open();

            string update = string.Format("UPDATE [{0}].[dbo].[TB_AOIDefectDetail] SET Defect = {1}, ReDefect = 30000000, DefectComment = 'AutoClassification' WHERE DetailGUID = '{2}'", resultDBName, defectCode, args.DetailGUID);

            args.Log.WriteExecute("AutoClassification.Update", ComponentName, ()=>machine(args.Lane, args.DetailGUID, ComponentName, InspType));

            // Update PCBRepair to clear Review
            var command = new SqlCommand(update, connection);
            command.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            args.Log.WriteError("AutoClassification.Error", ex.Message, ()=>machine(args.Lane, args.DetailGUID, ComponentName, InspType));

            args.AOI.Review.Alarm("REVIEW", args.Lane, "Failed to auto-classify defect");
        }
        finally
        {
            if (connection.IsNullEmpty() == false)
            {
                connection.Close();
            }
        }
    }

    private static string AutoClassification_GetPackageType(KSMART.BRM.Script.AOIReviewDefectSelectedArgs args, string ComponentGUID)
    {
        string package = string.Empty;

        var connection = new SqlConnection(args.AOI.Settings.InspectResult.ConnString);
        var resultDBName = args.AOI.GetPCB(args.PCBGUID).Tables["PCB"].Rows[0]["ResultDBName"].ToString();

        try
        {
            connection.Open();

            string select = string.Format("SELECT PackageType FROM [{0}].[dbo].[TB_AOIDefect] WHERE ComponentGUID = '{1}'", resultDBName, ComponentGUID);

            var command = new SqlCommand(select, connection);
            package = command.ExecuteScalar().ToString();
        }
        catch (Exception ex)
        {
            args.Log.WriteError("AutoClassification.Error", ex.Message, ()=>machine(args.Lane));

            args.AOI.Machine.RtCmdNotify("AOI", "MES I/F", "Failed to auto-classify defects");
        }
        finally
        {
            if (connection.IsNullEmpty() == false)
            {
                connection.Close();
            }
        }

        return package;
    }

    private static string AutoClassification_GetDefectCode(KSMART.BRM.Script.AOIReviewDefectSelectedArgs args, string Lead, string InpsTypeCode)
    {
        string review = string.Empty;

        var table = args.CustomerProperty.GetValue("AutoClassificationCodes").ToXDocument();
        var codes = table.XPathSelectElements("Codes/AutoClassification");

        var leadType = (Lead.ToInt() == 0) ? "Body" : "Lead";

        foreach (var code in codes)
        {
            if ((code.Attribute("Type").Value == leadType) &&
                (code.Attribute("MachineCode").Value == InpsTypeCode))
            {
                review = code.Attribute("ReviewCode").Value;
            }
        }

        return review;
    }

    private static string machine(string lane)
    {
        var parameter = new Dictionary<string, object>();

        parameter.Add("Lane", lane);

        return parameter.ToParametersEquals();
    }

    private static string machine(string lane, string detailguid, string component, string inpstype)
    {
        var parameter = new Dictionary<string, object>();

        parameter.Add("Lane", lane);
        parameter.Add("DetailGUID", detailguid);
        parameter.Add("Component", component);
        parameter.Add("InspType", inpstype);

        return parameter.ToParametersEquals();
    }
}
