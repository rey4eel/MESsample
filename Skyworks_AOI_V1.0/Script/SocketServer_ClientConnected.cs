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
    public static void SocketServer_ClientConnected(KSMART.BRM.Script.SocketServerClientConnectedArgs args)
    {
        if (args.Name == "AOI")
        {
            // Initialize configuration
            Machine_ResultDatabase(args);
            Machine_SECSGEM(args);
        }
    }

    private static void Machine_ResultDatabase(KSMART.BRM.Script.SocketServerClientConnectedArgs args)
    {
        // Set result database connection based on DB settings
        if (args.CustomerProperty.GetValue("AutoDBSetting").ToBoolean() == true)
        {
            var server = "127.0.0.1";
            var source = "KY_AOI";
            var user = "sa";
            var password = "koh1234";

            var ini = args.INI.Load(@"C:\KohYoung\AOI\AOIGUISetup.ini");

            if (ini.IsRead("RESULT", "DBServer") == true)
            {
                server = ini.Read("RESULT", "DBServer");
                user = ini.Read("RESULT", "DBID");
                password = ini.Read("RESULT", "DBPassword");
            }
            else
            {
                ini = args.INI.Load(@"C:\KohYoung\Review\Config.ini");

                if (ini.IsRead("DBMS", "ServerName") == true)
                {
                    server = ini.Read("DBMS", "ServerName");
                    user = ini.Read("DBMS", "UserID");
                    password = ini.Read("DBMS", "Password");
                }
            }

            args.AOI.Settings.InspectResult.Config.SetValue("Server", server);
            args.AOI.Settings.InspectResult.Config.SetValue("DataSource", source);
            args.AOI.Settings.InspectResult.Config.SetValue("UserID", user);
            args.AOI.Settings.InspectResult.Config.SetValue("Password", password);
        }
    }

    private static void Machine_SECSGEM(KSMART.BRM.Script.SocketServerClientConnectedArgs args)
    {
        if (args.CustomerProperty.GetValue("SECSGEMEnable").ToBoolean() == false)
        {
            args.Plugins.CIMConnect.Stop();
        }
    }
}
