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

public class Script
{
    public static void Machine_StatusChanged(KSMART.BRM.Script.AOIMachineStatusChangedArgs args)
    {
	if (args.AOI.Machine.Current.UseMES("AOI") != true)
	{
		return;
	}
    
	if(args.CustomerProperty.GetValue("MachinePCHK").ToString() == "ON")
		return;
	
	try
	{
		 int nOldOperationMode = args.CustomerProperty.GetValue("operationMode").ToInt();
		 int nNewOperationMode = -1;
		 
		 //OperationMode
		 //1 (Auto), 2 (Step), 3 (Manual), 4 (Special)
	       switch(args.StatusCode)
		 {
		 	case "194":	//Disconnected
		 		break;
			case "202":	//Initialize
				//nNewOperationMode 
		 		break;
		 	case "203":	//Power off
		 		break;
		 	case "204":	//Homing
		 		nNewOperationMode = 3;
		 		break;
		 	case "205":	//Running
		 		nNewOperationMode = 1;
		 		break;
		 	case "206":	//Bypass
		 		nNewOperationMode = 4;
		 		break;
		 	case "207":	//Grap
		 		break;
		 	case "208":	//Idle
				if (args.CustomerProperty.GetValue("NowProcessing").ToInt() == 1)
				{
					SendTelegram(args, "partProcessingAborted");
					args.CustomerProperty.SetValue("NowProcessing", "0");
				}
		 		nNewOperationMode = 3;
		 		args.Timer.GetObject("jamTimer").Stop();
		 		args.Timer.GetObject("partMissingTimer").Stop();
		 		break;
		 	case "209":	//Stop
		 		nNewOperationMode = 3;
		 		args.Timer.GetObject("jamTimer").Stop();
		 		args.Timer.GetObject("partMissingTimer").Stop();
		 		break;
		 	case "210":	//Error
		 		nNewOperationMode = 3;
		 		break;
		 	case "211":	//Emergency
		 		nNewOperationMode = 3;
		 		break;
		 	case "220":	//Board Grab
		 		nNewOperationMode = 3;
		 		break;
		 	case "221":	//Initialize	 	
		 		break;
		 	case "300":	//Wait
		 		break;	 
		 }
		 
		if ((nNewOperationMode > 0) && (nNewOperationMode != nOldOperationMode))
		{
			args.CustomerProperty.SetValue("operationMode", nNewOperationMode.ToString());
			
			SendTelegram(args, "plcOperationModeChanged");
			
		}
	
		//plcError
		if (args.StatusCode.EqualsOrEx("210","211")) //Alarm Occur
		{
		    	  /*
			    Stop
			    Error
			    Emergency
			  */	
			
			if (args.CustomerProperty.GetValue("NowProcessing").ToInt() == 1)
			{
				SendTelegram(args, "partProcessingAborted");
				args.CustomerProperty.SetValue("NowProcessing", "0");
			}
			
			args.CustomerProperty.SetValue("errorNo", args.GetValue("ERR_CD"));
			args.CustomerProperty.SetValue("errorText", args.AOI.Codes.GetAlarmText(args.GetValue("ERR_CD")));
			args.CustomerProperty.SetValue("errorType", "1");
			args.CustomerProperty.SetValue("errorState", "0");
	
			SendTelegram(args, "plcError");

		} 
		else if (args.StatusCode.EqualsOrEx("208") && (args.CustomerProperty.GetValue("errorState") == "0")) //Alarm Clear
		{
		   /*
		   Idle
		   */	
		   	if(args.CustomerProperty.GetValue("errorNo") != "0")
		   	{
		   		args.Log.Write(args.CustomerProperty.GetValue("errorNo"));
				args.CustomerProperty.SetValue("errorNo", "0");
				args.CustomerProperty.SetValue("errorText", "acknowledge");
				args.CustomerProperty.SetValue("errorState", "1");
				
				SendTelegram(args, "plcError");
			}
			
		 }	

		if (nOldOperationMode != nNewOperationMode)
		{
		    args.CustomerProperty.SetValue("operationMode",  nNewOperationMode.ToString());
		    
		    SendTelegram(args, "dataUploadRequired");
		}
	
	}
	
	catch(Exception ex) 
	{
		args.Log.WriteException("Machine_StatusChanged" , ex);
	}	
    }


    public static void SendTelegram(KSMART.BRM.Script.AOIMachineStatusChangedArgs args, string strMsgName)
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