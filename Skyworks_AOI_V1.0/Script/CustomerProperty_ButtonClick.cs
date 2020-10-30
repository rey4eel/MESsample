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
using KSMART.BRM.Core;
using KSMART.BRM.Script;
using System.IO;

public class Script
{
    public static void CustomerProperty_ButtonClick(KSMART.BRM.Script.PropertyButtonClickArgs args)
    {
        if (args.Name == "AutoClassificationButton")
        {
            var table = args.CustomerDataGrid.GetDataSource("AutoClassification");

            args.Log.WriteExecute("AutoClassification.Open");

            // Open auto classification menu
            args.CustomerDataGrid.ShowModal("AutoClassification",
                (load)=>
                {
                    load.Form.Text = "Auto Classification";
                    load.Grid.UseSave = true;
                    load.Grid.UseClear = true;
                    load.Grid.UseRefresh = true;
                    load.Grid.AllowEdit = true;
                    load.Grid.AllowDelete = true;
                    load.Grid.AllowNewItemRow = true;

                    load.Grid.OnSave += new KSMART.BRM.Core.CustomerDataGridControl.SaveEventHandler(OnSave);
                }
            );
        }
    }

    private static void OnSave(string gridName)
    {
        var args = new KSMART.BRM.Script.ScriptArgs();

        args.CustomerDataGrid.Refresh("AutoClassification");

        args.CustomerDataGrid.CloseModal("AutoClassification");

        args.Log.WriteSuccess("AutoClassification.Save");
    }
}
