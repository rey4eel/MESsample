//css_reference System.Core;
//css_reference  System.Data.DataSetExtensions;
//css_reference  System.Xml;
//css_reference  System.Xml.Linq;

using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Threading;
using KSMART.BRM;
using System.IO;

public class Script
{
    public static void Machine_JobChanged(KSMART.BRM.Script.AOIMachineJobChangedArgs args)
    {
	try
	{
    		if (args.AOI.Machine.Current.UseMES("AOI") != true)
    		{
    			return;
    		}

		args.CustomerProperty.SetValue("changeFlagStatus", "false");
		//args.WriteLog("ChangeJobName : " + args.CustomerProperty["ChangeJobName"]);
		//args.WriteLog("JOB1 : " + Path.GetFileNameWithoutExtension(args.Member["JOB1"]));
		
		if (args.CustomerProperty.GetValue("machineSoftwareVersion") == string.Empty )
		{
		        args.CustomerProperty.SetValue("machineSoftwareVersion", args.AOI.Machine.Current.Version("AOI"));
		        args.CustomerProperty.SetValue("interfaceSoftwareVersionNo", args.History.BusinessRulesVersion);
	       }
	        
		if (args.GetValue("JOB1") == string.Empty && args.GetValue("JOB2") == string.Empty)
		{
		}
	      else if (string.Compare(args.CustomerProperty.GetValue("ChangeJobName"), Path.GetFileNameWithoutExtension(args.GetValue("JOB1")), true) != 0)
		{
			//Job변경지시가 아닌 경우, 초기화
			args.CustomerProperty.SetValue("ChangeJobName", string.Empty);
		      args.CustomerProperty.SetValue("FrontTypeNo", args.CustomerProperty.GetValue("DefaultTypeNo"));
			args.CustomerProperty.SetValue("RearTypeNo", args.CustomerProperty.GetValue("DefaultTypeNo"));		
		
		}
		else
		{
			//for Test
			args.Log.Write("ChangeJobName : " + args.CustomerProperty.GetValue("ChangeJobName"));
		
			//Send
			args.CustomerProperty.SetValue("eventId", (args.CustomerProperty.GetValue("eventId").ToInt() + 1).ToString());
			string sSendMsg = args.Xsl.StringTransform(args.CustomerProperty.GetValue("plcChangeOver"));
			
			
			int nHeadLength = 4;
			int nBodyLength = sSendMsg.Length;
			
			byte[] abHead = new byte[nHeadLength];
			abHead = BitConverter.GetBytes(nHeadLength + nBodyLength);
	
			byte[] abMsg = new byte[nHeadLength + nBodyLength];
			
			Array.Reverse(abHead);
		      Array.Copy(abHead, abMsg, nHeadLength);
			Array.Copy(System.Text.Encoding.UTF8.GetBytes(sSendMsg), 0, abMsg, nHeadLength, nBodyLength);
			
			args.SocketClient.SendByte("MES", abMsg);		
			
			//Send
			args.Log.Write("plcChangeOver : Body Length " + sSendMsg.Length , sSendMsg);
	           
	             //Job 변경이 완료되었으면, 초기화
		       args.CustomerProperty.SetValue("ChangeJobName", string.Empty);
	      }
	}
	catch(Exception ex) 
	{
		args.Log.WriteException("Machine_JobChanged" , ex);
	}      
    }
}