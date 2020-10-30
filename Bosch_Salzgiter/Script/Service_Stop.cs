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

public class Script
{
    public static void Service_Stop(KSMART.BRM.Script.ScriptArgs args)
    {
		args.Timer.Remove("jamTimer");
		args.Timer.Remove("partMissingTimer");
		args.Timer.Remove("dataUploadTimer");
    }
   
}