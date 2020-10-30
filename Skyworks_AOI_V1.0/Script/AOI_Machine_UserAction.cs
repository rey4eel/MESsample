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
    public static void Machine_UserAction(KSMART.BRM.Script.AOIMachineUserActionArgs args)
    {
        if (args.Event == "MESIF")
        {
            if (args.GetValue("USER_ID").ToUpper().StartsWith("OP") == false)
            {
                args.CustomerProperty.Show(
                (load)=>
                {
                    load.UseSave = true;
                    load.SetVisible(false, "Definition");
                });
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
