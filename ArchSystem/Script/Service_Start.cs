//Add Reference
//css_reference System.Core;
//css_reference System.Data;
//css_reference System.Data.DataSetExtensions;
//css_reference System.Xml;
//css_reference System.Xml.Linq;

//Import
//css_import C:\KohYoung\KSMART Business Rules Management\Script\SPI_WebService.cs;

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

public class Script
{
    public static void Service_Start(KSMART.BRM.Script.ScriptArgs args)
    {
        // Initialize web service queue
        WebService.Queue.Init(args);
        
        // Initialize heatbeat message
        WebService.Heartbeat.Init(args);
    }
}
