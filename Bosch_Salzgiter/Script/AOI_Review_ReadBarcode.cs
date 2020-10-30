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
using System.Xml;
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
    public static void Review_ReadBarcode(KSMART.BRM.Script.AOIReviewReadBarcodeArgs args)
    {
    		args.Log.Write("Review_ReadBarcode");
    		if (args.AOI.Machine.Current.UseMES("AOI") != true)
    		{
    			args.SendOK();
    			return;
    		}
		args.CustomerProperty.SetValue("MachinePCHK", "ON");
		try
		{
			DataRow drPCB = args.AOI.GetDefaultInspectResultData(args.PCBGUID).Tables["PCB"].Rows[0];
			
			args.CustomerProperty.SetValue("eventId", (args.CustomerProperty.GetValue("eventId").ToInt() + 1).ToString());
			args.CustomerProperty.SetValue("BarcodeReview", args.PCBBarcode);
			args.CustomerProperty.SetValue("SideReview", drPCB["TB"].ToString());
			args.CustomerProperty.SetValue("LaneReview", args.Lane);
			
			string sSendMsg = args.Xsl.StringTransform(args.CustomerProperty.GetValue("partReceivedReview"));
			
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
				args.Log.Write("partReceivedReview : Body Length " + sSendMsg.Length, sSendMsg);
				nRcvMsgLength = 0;
			},
			(recv)=>
			{
				if(recv.Success)
				{
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
					switch(recv.Error)
					{
						case emCommunicationError.NotConnected:
							args.SendNG(args.CustomerProperty.GetValue("ConnFailMessage"));
							break;
						case emCommunicationError.Timeout:
							args.SendNG(args.CustomerProperty.GetValue("TimeoutMessage"));
							break;
						case emCommunicationError.Exception:
							args.SendNG("Please, Check MES Log.");
							break;
					}
				}
			},
			(rcv)=>
			{
				  //Receive
				sRcvMsg = System.Text.Encoding.UTF8.GetString(rcv.TotalByte);
				sRcvMsg = sRcvMsg.Substring(nHeadLength, nRcvMsgLength - nHeadLength);
				
				args.Log.Write("partReceivedReview_R : Body Length " + sRcvMsg.Length, sRcvMsg);
				
				RcvPartReceived(args, sRcvMsg);
			});
		}
		catch(Exception ex) 
		{		
			args.SendException(ex);
			args.CustomerProperty.SetValue("MachinePCHK", "OFF");
		}		
		finally
		{
			args.CustomerProperty.SetValue("MachinePCHK", "OFF");
		}

    }

    	private static void RcvPartReceived(KSMART.BRM.Script.AOIReviewReadBarcodeArgs args, string sRcvMsg)
    	{
	    	try
	    	{
			XmlDocument xDoc = new XmlDocument();
			
	    	      	if ( (sRcvMsg.Length > 4) && sRcvMsg.IsXmlDocumentFromString() )
	    	      		xDoc.LoadXml(sRcvMsg);

		      if(xDoc != null)
		      {
				XmlNode xNode = xDoc.SelectSingleNode("/root/event/result");
				
			      if ((xNode != null) && (xNode.Attributes["returnCode"].Value.ToString() == "0"))
			      {
	                        	xNode = xDoc.SelectSingleNode("/root/body/structs/workPart");
	
	                        	if(xNode != null)
	                        	{
                        	
		                        	bool partForStation = false;
		                        	bool changeOver = false;
		                        	string nextProcessNo = string.Empty;
		                        	string typeNo = string.Empty;
		                        	string typeVar = string.Empty;
		                        	string processNo = string.Empty;
		                        	string batch = string.Empty;
		                        	
	                        		partForStation = xNode.Attributes["partForStation"].Value.ToBoolean();
		                         	changeOver = xNode.Attributes["changeOver"].Value.ToBoolean();
		                         	nextProcessNo = xNode.Attributes["nextProcessNo"].Value.ToString();
		                         	typeNo =  xNode.Attributes["typeNo"].Value.ToString();
		                         	typeVar = xNode.Attributes["typeVar"].Value.ToString();	
		                         	batch = xNode.Attributes["typeVar"].Value.ToString();	
		                         	
						args.CustomerProperty.SetValue("TypeNoReview",typeNo);
						args.CustomerProperty.SetValue("TypeVarReview",typeVar);
						//for Test
						//args.WriteLog("FrontTypeNo : " + args.CustomerProperty["FrontTypeNo"] + ", RearTypeNo : " + args.CustomerProperty["RearTypeNo"]);
						//check processNo & nextProcessNo
						if(!partForStation)
						{
		                         		//NG
		                         		args.SendNG(args.CustomerProperty.GetValue("BoardBadmarkMessage"));
						}
						else 
						{
	                                	//workingCode
	                                	args.CustomerProperty.SetValue("workingCodeReview",  xNode.Attributes["workingCode"].Value.ToString());
	                                	args.SendOK();
						}
	                        	}
	                        	else
	                        	{
	                        		//NG
	                        		args.SendNG("Return : " + "No workPart telegram");
	                        	}
	                    
			      }
				else if (xNode != null)
				{
					//NG
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
		catch(Exception ex) 
		{
		
			args.SendException(ex);
		}		
    	}

}