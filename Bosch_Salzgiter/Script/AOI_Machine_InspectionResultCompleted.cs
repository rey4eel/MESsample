//css_reference System.Core;
//css_reference  System.Data.DataSetExtensions;
//css_reference  System.Xml;
//css_reference  System.Xml.Linq;

using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Threading;
using KSMART.BRM;

public class Script
{
    public static void Machine_InspectionResultCompleted(KSMART.BRM.Script.AOIMachineInspectResultCompletedArgs args)
    {
    	
    	try
    	{
    		if (args.CustomerProperty.GetValue("chkFileExport").ToBoolean() == true)
    		{
    			args.Log.Write("chkFileExport");
			var fileName = args.Xsl.StringTransform(args.ResultData,args.CustomerProperty.GetValue("XSLT_FileName"));
			var fileContens = args.Xsl.StringTransform(args.ResultData,args.CustomerProperty.GetValue("XSLT_Contents"));
			
			if(fileName.IsFileExists())
			{
				fileContens = System.Environment.NewLine +  fileContens.RemoveFirstLines(1);
			}
						
			fileName.WriteString(fileContens);	
			
			args.Log.WriteSuccess("FileExport", ()=>
			{
				var parameter = new Dictionary<string, object>();
				 
				 parameter.Add("FileName",fileName);
				 
				return parameter.ToParametersEquals();
				
			});
		}
		
    		if (args.AOI.Machine.Current.UseMES("AOI") != true)
    		{
    			args.SendOK();
    			return;
    		}
    		
    		args.CustomerProperty.SetValue("eventId", (args.CustomerProperty.GetValue("eventId").ToInt() + 1).ToString());
    	
    		args.CustomerProperty.SetValue("CallFrom", "GUI");
    	
		string sSendMsg = args.Xsl.StringTransform(args.ResultData, args.CustomerProperty.GetValue("partProcessed"));
		
		int nHeadLength = 4;
		int nBodyLength = sSendMsg.Length;
		
		byte[] abHead = new byte[nHeadLength];
		abHead = BitConverter.GetBytes(nHeadLength + nBodyLength);
		
		byte[] abMsg = new byte[nHeadLength + nBodyLength];
	
		Array.Reverse(abHead);
	      Array.Copy(abHead, abMsg, nHeadLength);
		Array.Copy(System.Text.Encoding.UTF8.GetBytes(sSendMsg), 0, abMsg, nHeadLength, nBodyLength);
	
		int nRcvMsgLength = 0;
		int nRcvLength = 0;
		string sRcvMsg = string.Empty;
		byte[] abRcvHead = new byte[nHeadLength];		

		args.SocketClient.SendReceiveByte("MES", abMsg,	args.CustomerProperty.GetValue("Timeout").ToInt()*1000,
		()=>
		{
			//Send
			args.Log.Write("partProcessed : Body Length " + sSendMsg.Length, sSendMsg);			
			nRcvMsgLength = 0;
		},
		(recv)=>
		{
			if(recv.Success)
			{
		        	//Receive	        	
				if((nRcvMsgLength == 0) && (recv.BytesTransferred >= 6))
				{
					Array.Copy(recv.Bytes, abRcvHead, nHeadLength);
					Array.Reverse(abRcvHead);
					nRcvMsgLength = BitConverter.ToInt32(abRcvHead, 0);
				}
	
				if((nRcvMsgLength > 0) && (recv.BytesTransferred >= nRcvMsgLength))
					recv.Complete();
			}
			else
			{
				//Exception
			      switch(recv.Error)
			      {
			      		case emCommunicationError.NotConnected:
						args.SendNG(args.CustomerProperty.GetValue("ConnFailMessage"));
			      			break;
					case emCommunicationError.Timeout:
						args.SendNG(args.CustomerProperty.GetValue("TimeoutMessage"));
						break;
					case  emCommunicationError.Exception:
						args.SendNG("Please, Check MES Log.");
						break;
			      }
			}
		},		
		(rcv)=>
		{
			sRcvMsg = System.Text.Encoding.UTF8.GetString(rcv.TotalByte);
			sRcvMsg = sRcvMsg.Substring(nHeadLength, nRcvMsgLength - nHeadLength);
			
			args.Log.Write("partProcessed_R : Body Length " + sRcvMsg.Length, sRcvMsg);			
			args.CustomerProperty.SetValue("NowProcessing", "0");

    	      		//if ( (sRcvMsg.Length > 4) && (sRcvMsg.IsXDocumentByString()) )
    	      		if ( (sRcvMsg.Length > 4) && sRcvMsg.IsXmlDocumentFromString() )
    	      		{
    	      			XmlDocument xDoc = new XmlDocument();	    	      			
    	      			
    	      			xDoc.LoadXml(sRcvMsg);
    	      		
	    	      	      if(xDoc != null)
		      		{
					XmlNode xNode = xDoc.SelectSingleNode("/root/event/result");
		
				      if ((xNode != null) && (xNode.Attributes["returnCode"].Value.ToString() == "0"))
				      {
						args.SendOK();
				      }
				      else if (xNode != null)
			        	{
						XmlNode xNode1 = xDoc.SelectSingleNode("/root/event/trace/trace");
						if (xNode1 != null)
						{
							args.SendNG("Return Code : " +xNode1.Attributes["text"].Value.ToString());
						}
						else
						{
							args.SendNG("Return Code : " + xNode.InnerText);
						}
			        	 }
				        else
				        {
				        	//NG
		                    	args.SendNG("Return Code : null");
				        }
			        }
			        else
			        {
			           //NG
			           args.SendNG("Return : null");		
			        }
			 }
			 else
			 {
			 	args.SendNG("MES Return Format is wrong!!!");	
			 }
		});
	
	}
	catch(Exception ex) 
	{
		args.Log.WriteException("Machine_InspectionResultCompleted" , ex);
	}		
    }
   
}