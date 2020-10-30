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
using System.Text;
using KSMART.BRM;
using KSMART.BRM.Core;
using System.IO;
using System.Data.OleDb;

public class Script
{
    public static void Machine_UserAction(KSMART.BRM.Script.SPIMachineUserActionArgs args)
    {
        if (args.Event == "MESIF")
        {
            if (args.KeyValue.GetValue("UserLevel") != "0")
            {
                args.CustomerProperty.Show(
                    (load)=>
                    {
                        load.UseSave = true;
                        load.Form.Text = "Customer Property";
                        load.SetVisible(false, new [] {"Definition", "WebServiceTest"});
                    }
                );
            }
            else
            {
                args.MessageBox.Show(
                    "Please login as administrator and try again",
                    "MES I/F",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}
