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
using System.Management;
using KSMART.BRM;
using KSMART.BRM.Core;

public class Script
{
    public static void Service_Start(KSMART.BRM.Script.ScriptArgs args)
    {
		var ini = args.INI.Load(@"C:\KohYoung\KSMART Business Rules Management\bosch.ini");
		args.CustomerProperty.SetValue("ToolID.Current", ini.Read("MES", "ToolIDCurrent", "").ToString());
		args.CustomerProperty.SetValue("ToolOldID", ini.Read("MES", "ToolIDOld", "").ToString());
		
		args.CustomerProperty.SetValue("NowProcessing", "0");
		args.CustomerProperty.SetValue("FrontTypeNo", args.CustomerProperty.GetValue("DefaultTypeNo"));
		args.CustomerProperty.SetValue("FrontTypeVar", args.CustomerProperty.GetValue("DefaultTypeVar"));		
		args.CustomerProperty.SetValue("RearTypeNo", args.CustomerProperty.GetValue("DefaultTypeNo"));
		args.CustomerProperty.SetValue("RearTypeVar", args.CustomerProperty.GetValue("DefaultTypeVar"));
		args.CustomerProperty.SetValue("COFrontTypeNo", args.CustomerProperty.GetValue("DefaultTypeNo"));
		args.CustomerProperty.SetValue("COFrontTypeVar", args.CustomerProperty.GetValue("DefaultTypeVar"));		
		args.CustomerProperty.SetValue("CORearTypeNo", args.CustomerProperty.GetValue("DefaultTypeNo"));
		args.CustomerProperty.SetValue("CORearTypeVar", args.CustomerProperty.GetValue("DefaultTypeVar"));
		args.CustomerProperty.SetValue("jamStatus", "false");
		args.CustomerProperty.SetValue("partMissingStatus", "false");
		args.CustomerProperty.SetValue("errorNo", "0");
		args.CustomerProperty.SetValue("errorText", "");
		args.CustomerProperty.SetValue("errorType", "1");
		args.CustomerProperty.SetValue("errorState", "0");
		args.CustomerProperty.SetValue("StartFirst", "0");
		SetOsName(args);
		
		args.Log.Write("Bosch API - 1.91.kbr Apply");
		
    		args.Timer.Add("jamTimer", 180000, 
	    		()=>
	    		{
	    			if(args.CustomerProperty.GetValue("StartFirst").ToInt() < 2)
	    			{
	    				args.CustomerProperty.SetValue("StartFirst", (args.CustomerProperty.GetValue("StartFirst").ToInt() + 1).ToString());
	    				return;
	    			}
	    			
	    			//jam
	    			args.CustomerProperty.SetValue("jamStatus", "true");
	    			SendTelegram(args, "plcJamStarted");
	    		});

    		args.Timer.Add("partMissingTimer", 180000, 
	    		()=>
	    		{
	    			if(args.CustomerProperty.GetValue("StartFirst").ToInt() < 2)
	    			{
	    				args.CustomerProperty.SetValue("StartFirst", (args.CustomerProperty.GetValue("StartFirst").ToInt() + 1).ToString());
	    				return;
	    			}

	    			//partMissing
				args.CustomerProperty.SetValue("partMissingStatus", "true");
				SendTelegram(args, "plcPartsMissingStarted");
	    		});

		args.Timer.GetObject("jamTimer").Stop();
		args.Timer.GetObject("partMissingTimer").Stop();
	    	

    		args.Timer.Add("dataUploadTimer", 180000, 
	    		()=>
	    		{
	    			args.Log.Write("dataUploadTimer Start");
				SendTelegram(args, "dataUploadRequired");
	    		});


	    	args.Scheduler.AddDailyAtHourAndMinute("dataUploadDaily",1,0, (r)=>
	    	{
	    		SendTelegram(args, "dataUploadRequiredDaily");
	    	});

    }
    
	private static void SetOsName(KSMART.BRM.Script.ScriptArgs args)
	{
		string strOSVersion = "";

            OperatingSystem os = Environment.OSVersion;
            Version v = os.Version;
 
            if (5 == v.Major && v.Minor > 0)
            {
               strOSVersion = "Windows XP";
            }
            else if (6 == v.Major && v.Minor == 0)
            {
               strOSVersion = "Windows VISTA";
            }
            else if (6 == v.Major && v.Minor == 1)
            {
               strOSVersion = "Windows 7";
            }
            else if (6 == v.Major && v.Minor == 2)
            {
               strOSVersion = "Windows 10";
            }
            
            args.CustomerProperty.SetValue("machineSystemSoftware", strOSVersion);
            args.CustomerProperty.SetValue("softwareServicePack", os.ServicePack.ToString());
	}

    public static void SendTelegram(KSMART.BRM.Script.ScriptArgs args, string strMsgName)
    {
	try
	{
		args.CustomerProperty.SetValue("eventId", (args.CustomerProperty.GetValue("eventId").ToInt() + 1).ToString());
		string sSendMsg = args.Xsl.StringTransform(args.CustomerProperty.GetValue(strMsgName));
		
		int nHeadLength = 4;
		int nBodyLength = sSendMsg.Length;
		
		byte[] abHead = new byte[nHeadLength];
		abHead = BitConverter.GetBytes(nHeadLength + nBodyLength);

		byte[] abMsg = new byte[nHeadLength + nBodyLength];
		
		Array.Reverse(abHead);
	      Array.Copy(abHead, abMsg, nHeadLength);
		Array.Copy(System.Text.Encoding.UTF8.GetBytes(sSendMsg), 0, abMsg, nHeadLength, nBodyLength);

		args.SocketClient.SendByte("MES", abMsg);
	      args.Log.Write(strMsgName + " : Body Length " + sSendMsg.Length, sSendMsg);	
	}
	catch(Exception ex) 
	{		
		args.Log.WriteException(strMsgName , ex);
	}
    }

}


