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
    public static void Machine_BoardOut(KSMART.BRM.Script.AOIMachineBoardOutArgs args)
    {
    		if (args.AOI.Machine.Current.UseMES("AOI") != true)
    		{
    			return;
    		}
		args.Timer.GetObject("jamTimer").Stop();
		
		if(args.CustomerProperty.GetValue("jamStatus") == "true")
		{
			args.CustomerProperty.SetValue("jamStatus", "false");
			args.CustomerProperty.SetValue("partMissingStatus", "false");
    			SendTelegram(args, "plcJam");
		}
				
		args.Timer.GetObject("partMissingTimer").Start();
    }
    
    public static void SendTelegram(KSMART.BRM.Script.AOIMachineBoardOutArgs args, string strMsgName)
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
	}
	catch(Exception ex) 
	{		
		args.Log.WriteException(strMsgName , ex);
	}
    }
}
