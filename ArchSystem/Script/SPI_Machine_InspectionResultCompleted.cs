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
using KSMART.BRM;
using KSMART.BRM.Core;
using System.Net;

public class Script
{
    public static void Machine_InspectionResultCompleted(KSMART.BRM.Script.SPIMachineInspectResultCompletedArgs args)
    {
        if (args.SocketServer.IsConnected("REVIEW") == true)
        {
            // Send InspectEnd message
            WebService.Message.Send(args, args.Lane, WebService.Event.InspectEnd);
        }
        else
        {
            // Send ReviewEnd message
            WebService.Message.Send(args, args.Lane, WebService.Event.ReviewEnd);
        }

        args.SendOK();
    }
}
