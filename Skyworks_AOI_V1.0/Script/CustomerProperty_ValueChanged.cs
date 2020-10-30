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
using System.Diagnostics;
using KSMART.BRM;
using KSMART.BRM.Script;

public class Script
{
    public static void CustomerProperty_ValueChanged(KSMART.BRM.Script.PropertyValueChangedArgs args)
    {
        if (args.Name == "SECSGEMEnable")
        {
            if (args.Value.ToBoolean() == true)
            {
                if (args.Plugins.CIMConnect.IsStart == false)
                {
                    args.Plugins.CIMConnect.Start();
                }
            }
            else
            {
                args.Plugins.CIMConnect.Stop();
            }
        }
        else if ((args.Name == "AutoClassificationEnable") ||
            (args.Name == "AutoClassificationLock"))
        {
            if (args.Value.ToBoolean() == true)
            {
                args.KeyValue.SetValue("AutoClassificationStatus", "Active");
            }
            else
            {
                args.AOI.Review.RtCmdUnLock("REVIEW");
                args.AOI.Review.RtCmdUnLockJudgment("REVIEW");
            }
        }
    }
}
