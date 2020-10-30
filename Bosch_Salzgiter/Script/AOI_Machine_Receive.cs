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
    static string strErrorCD = "0";
    static string strErrorMSG = "";
    static string strSide = "N";
    
    public static void Machine_Receive(KSMART.BRM.Script.AOIReceiveArgs args)
    {
    		if (args.AOI.Machine.Current.UseMES("AOI") != true)
    		{
    			return;
    		}
		if(args.Protocol.EventId == "PCODECHK")
		{
			ReadProductCode(args);
		}
		else if(args.Protocol.EventId == "CHTOOL_START")
		{
			ToolChangeStart(args);
		}
		else if(args.Protocol.EventId == "CHTOOL_END")
		{
			ToolChangeEnd(args);
		}
    }
    
    public static void ReadProductCode(KSMART.BRM.Script.AOIReceiveArgs args)
    {
	args.CustomerProperty.SetValue("MachinePCHK", "ON");
	try
	{
		args.CustomerProperty.SetValue("eventId", (args.CustomerProperty.GetValue("eventId").ToInt() + 1).ToString());
		args.CustomerProperty.SetValue("Barcode", args.Protocol.GetValue("BARCODE"));
		args.CustomerProperty.SetValue("Lane",  args.Protocol.GetValue("LANE"));
		
		string sSendMsg = args.Xsl.StringTransform(args.CustomerProperty.GetValue("partReceived"));
		
		int nHeadLength = 4;
		int nBodyLength = sSendMsg.Length;
		
		byte[] abHead = new byte[nHeadLength];
		abHead = BitConverter.GetBytes(nHeadLength + nBodyLength);
		
		byte[] abMsg = new byte[nHeadLength + nBodyLength];
		
		Array.Reverse(abHead);
		Array.Copy(abHead, abMsg, nHeadLength);
		Array.Copy(System.Text.Encoding.UTF8.GetBytes(sSendMsg), 0, abMsg, nHeadLength, nBodyLength);
		
		int nRcvMsgLength = 0;
		string sRcvMsg = string.Empty;
		byte[] abRcvHead = new byte[nHeadLength];
		
		args.SocketClient.SendReceiveByte("MES", abMsg,	args.CustomerProperty.GetValue("Timeout").ToInt()*1000,
		()=>
		{
			//Send
			args.Log.Write("partReceived : Body Length " + sSendMsg.Length, sSendMsg);
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
						strErrorMSG = args.CustomerProperty.GetValue("ConnFailMessage");
			      			break;
					case emCommunicationError.Timeout:
						strErrorMSG = args.CustomerProperty.GetValue("TimeoutMessage");
						break;
					case emCommunicationError.Exception:
						strErrorMSG = "Please, Check MES Log.";
						break;
				}
			}
		},
		(rcv)=>
		{
			  //Receive
			sRcvMsg = System.Text.Encoding.UTF8.GetString(rcv.TotalByte);
			sRcvMsg = sRcvMsg.Substring(nHeadLength, nRcvMsgLength - nHeadLength);
			
			args.Log.Write("partReceived_R : Body Length " + sRcvMsg.Length, sRcvMsg);
			
			if (RcvPartReceived(args, sRcvMsg) == false)
			{
				args.Log.Write(strErrorMSG);
			}
		});
			
	}
	catch(Exception ex) 
	{		
		strErrorMSG = ex.Message;
		args.CustomerProperty.SetValue("MachinePCHK", "OFF");
	}		
	finally
	{
		args.CustomerProperty.SetValue("MachinePCHK", "OFF");
	}
    }
   
    public static void ToolChangeStart(KSMART.BRM.Script.AOIReceiveArgs args)
    {
	try
	{
		args.CustomerProperty.SetValue("ToolOldID", args.CustomerProperty.GetValue("ToolID.Current"));
		args.CustomerProperty.SetValue("ToolID",args.Protocol.GetValue("TOOL_ID"));
		
		string sSendMsg = args.Xsl.StringTransform(args.CustomerProperty.GetValue("plcToolChangeStarted"));
		
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
		strErrorMSG = ex.Message;
	}		
    }

    public static void ToolChangeEnd(KSMART.BRM.Script.AOIReceiveArgs args)
    {
	try
	{
		args.CustomerProperty.SetValue("ToolOldID", args.CustomerProperty.GetValue("ToolID.Current"));
		args.CustomerProperty.SetValue("ToolID", args.Protocol.GetValue("TOOL_ID"));
		args.CustomerProperty.SetValue("ToolID.Current", args.Protocol.GetValue("TOOL_ID"));

		string sSendMsg = args.Xsl.StringTransform(args.CustomerProperty.GetValue("plcToolChanged"));
		
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
		strErrorMSG = ex.Message;
	}		
    }




    	private static bool RcvPartReceived(KSMART.BRM.Script.AOIReceiveArgs args, string sRcvMsg)
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
		                        	string typeNo = string.Empty;
		                        	string typeVar = string.Empty;
		                        	string batch = string.Empty;
		                        	
		                         	typeNo =  xNode.Attributes["typeNo"].Value.ToString();
		                         	typeVar = xNode.Attributes["typeVar"].Value.ToString();	
		                         	batch = xNode.Attributes["batch"].Value.ToString();	
	
						args.CustomerProperty.SetValue("FrontTypeNo", typeNo);
						args.CustomerProperty.SetValue("FrontTypeVar", typeVar);
						args.CustomerProperty.SetValue("RearTypeNo", typeNo);
						args.CustomerProperty.SetValue("RearTypeVar", typeVar);
						args.CustomerProperty.SetValue("batchStatus", batch);
						return DoChangeOver(args);
	                        	}
	                        	else
	                        	{
	                        		//NG
	                        		strErrorMSG = "No workPart telegram";
	                        		return false;
	                        	}
	                    
			      }
				else if (xNode != null)
				{
					//NG
                        		strErrorMSG = xNode.InnerText;
                        		return false;
				}
				else
				{
					//NG
                        		strErrorMSG = "Return Code is null";
                        		return false;
				}
		        }
		        else
		        {
		           //NG
                		strErrorMSG = "Return is null";
                		return false;
		        }
		}
		catch(Exception ex) 
		{
            		strErrorMSG = ex.Message;
            		return false;
		}		
    	}

	private static bool DoChangeOver(KSMART.BRM.Script.AOIReceiveArgs args)
	{
		try
		{
		     	args.CustomerProperty.SetValue("eventId", (args.CustomerProperty.GetValue("eventId").ToInt() + 1).ToString());
			string sSendMsg = args.Xsl.StringTransform(args.CustomerProperty.GetValue("dataDownloadRequired"));
			
			bool bReturn = true;
			int nHeadLength = 4;
			int nBodyLength = sSendMsg.Length;
			
			byte[] abHead = new byte[nHeadLength];
			abHead = BitConverter.GetBytes(nHeadLength + nBodyLength);
			
			byte[] abMsg = new byte[nHeadLength + nBodyLength];
		
			Array.Reverse(abHead);
		      Array.Copy(abHead, abMsg, nHeadLength);
			Array.Copy(System.Text.Encoding.UTF8.GetBytes(sSendMsg), 0, abMsg, nHeadLength, nBodyLength);
		
			int nRcvMsgLength = 0;
			string sRcvMsg = string.Empty;
			byte[] abRcvHead = new byte[nHeadLength];
			
			args.SocketClient.SendReceiveByte("MES", abMsg,	args.CustomerProperty.GetValue("Timeout").ToInt()*1000,
			()=>
			{
				//Send
				args.Log.Write("dataDownloadRequired : Body Length " + sSendMsg.Length, sSendMsg);
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
							strErrorMSG = args.CustomerProperty.GetValue("ConnFailMessage");
				      			break;
						case emCommunicationError.Timeout:
							strErrorMSG = args.CustomerProperty.GetValue("TimeoutMessage");
							break;
						case emCommunicationError.Exception:
							strErrorMSG = "Please, Check MES Log.";
							break;
					}
		            		bReturn = false;
				}
				
			},			
			(rcvJob)=>
			{
				XmlDocument xDoc = new XmlDocument();
				
				sRcvMsg = System.Text.Encoding.UTF8.GetString(rcvJob.TotalByte);
		    	      	sRcvMsg = sRcvMsg.Substring(nHeadLength, nRcvMsgLength - nHeadLength);
		    	      	
		    	      	args.Log.Write("dataDownloadRequired_R : Body Length " + sRcvMsg.Length, sRcvMsg);
		    	      	
		    	      	if ( (sRcvMsg.Length > 4) && sRcvMsg.IsXmlDocumentFromString() )
		    	      		xDoc.LoadXml(sRcvMsg);
					    
			      if(xDoc != null)
			      {
					XmlNode xNode = xDoc.SelectSingleNode("/root/event/result");
		
				      if ((xNode != null) && (xNode.Attributes["returnCode"].Value.ToString() == "0"))
				      {
						//Get Job Info, TypeNo
						string sPanelCnt  = string.Empty;
						string sTopJob = string.Empty;
						string sBotJob = string.Empty;
						
						xNode = xDoc.SelectSingleNode("/root/body/items");
                                	foreach (XmlNode xn in xNode.ChildNodes)
                                	{
                                		string sName = xn.Attributes["name"].Value;
                                		string sVal = xn.Attributes["value"].Value;
                                		if(sName == "WorkPart.PartsPerCarrier")
                                		{
                                			sPanelCnt = sVal;
                                		}
                                		else if(sName == "Tool1.Family")
                                		{
                                			args.CustomerProperty.SetValue("Tool1.Family", sVal);
                                		}
                                		else if(sName == "Tool1.Name")
                                		{
                                			args.CustomerProperty.SetValue("Tool1.Name", sVal);
                                		}
                                		else if(sName == "Tool2.Family")
                                		{
                                			args.CustomerProperty.SetValue("Tool2.Family", sVal);
                                		}
                                		else if(sName == "Tool2.Name")
                                		{
                                			args.CustomerProperty.SetValue("Tool2.Name", sVal);
                                		}
                                		else
                                		{
                                			//Top Check
                                			string[] asName = args.CustomerProperty.GetValue("TopProgramName").Split(';');
                                			foreach(string sPara in asName)
                                			{
                                				if(string.Compare(sName, sPara, true) == 0)
                                				{
                                					sTopJob = sVal;
                                					strSide = "T";
                                					break;
                                				}
                                			}
                                			
                                			//Bottom Check
                                			asName = args.CustomerProperty.GetValue("BottomProgramName").Split(';');
                                			foreach(string sPara in asName)
                                			{
                                				if(string.Compare(sName, sPara, true) == 0)
                                				{
                                					sBotJob = sVal;
                                					strSide = "B";
                                					break;
                                				}
                                			}
                                		}
                                	}

						//args.Log.Write("sTopJob : " + sTopJob);
						//args.Log.Write("sBotJob : " + sBotJob);
						if ((sTopJob.Length == 0) && (sBotJob.Length == 0))
						{
			                		strErrorMSG = "Job file name is empty from MES";
			                		bReturn = false;
						}

						string sChangeJob = string.Empty;
						if(strSide == "T")
						{
							sChangeJob = sTopJob + args.CustomerProperty.GetValue("TopProgramSuffix");
							args.CustomerProperty.SetValue("Tool.Side", "T");
						}
						else if(strSide == "B")
						{
							sChangeJob = sBotJob + args.CustomerProperty.GetValue("BottomProgramSuffix");
							args.CustomerProperty.SetValue("Tool.Side", "B");
						}
						
                                	//sChangeJob = Path.GetFileNameWithoutExtension(sChangeJob);
                                	args.Log.Write("sChangeJob : " + sChangeJob);
                                	if(sChangeJob.Length > 0)
                                	{
							args.CustomerProperty.SetValue("changeFlagStatus", "true");
				      			args.CustomerProperty.SetValue("ChangeJobName", sChangeJob);
							args.AOI.Machine.JobChange("AOI", args.Protocol.GetValue("LANE")	, sChangeJob, string.Empty, true, true, null, 60000,
							(r)=>
							{
								if (r.Result == emJobChangeResponseResult.Success)
								{
			                                    	//plcChangeOverStarted
			                                      args.CustomerProperty.SetValue("eventId", (args.CustomerProperty.GetValue("eventId").ToInt() + 1).ToString());
			                                	sSendMsg = args.Xsl.StringTransform(args.CustomerProperty.GetValue("plcChangeOverStarted"));
			                                	
									nBodyLength = sSendMsg.Length;
									abHead = BitConverter.GetBytes(nHeadLength + nBodyLength);
							
									Array.Reverse(abHead);
									Array.Resize(ref abMsg, nHeadLength + nBodyLength);
								      Array.Copy(abHead, abMsg, nHeadLength);
									Array.Copy(System.Text.Encoding.UTF8.GetBytes(sSendMsg), 0, abMsg, nHeadLength, nBodyLength);
									
									args.Log.Write("plcChangeOverStarted : Body Length " + sSendMsg.Length, sSendMsg);		
									//args.SocketClient.SendByte("MES", abMsg);
									args.SocketClient.SendReceiveByte("MES", abMsg,	args.CustomerProperty.GetValue("Timeout").ToInt()*1000,
									()=>
									{
										//Send
										args.Log.Write("plcChangeOverStarted : Body Length " + sSendMsg.Length, sSendMsg);
										nRcvMsgLength = 0;
									},
									(r1)=>
									{
										if(r1.Success)
										{
											if((nRcvMsgLength == 0) && (r1.BytesTransferred >= 6))
											{
												Array.Copy(r1.Bytes, abRcvHead, nHeadLength);
												Array.Reverse(abRcvHead);
												nRcvMsgLength = BitConverter.ToInt32(abRcvHead, 0);
											}
									
											if((nRcvMsgLength > 0) && (r1.BytesTransferred >= nRcvMsgLength))
												r1.Complete();
										}
										else
										{
											switch(r1.Error)
											{
										      		case emCommunicationError.NotConnected:
													strErrorMSG = args.CustomerProperty.GetValue("ConnFailMessage");
										      			break;
												case emCommunicationError.Timeout:
													strErrorMSG = args.CustomerProperty.GetValue("TimeoutMessage");
													break;
												case emCommunicationError.Exception:
													strErrorMSG = "Please, Check MES Log.";
													break;
											}
							                		bReturn = false;
										}
									},
									(r2)=>
									{
										sRcvMsg = System.Text.Encoding.UTF8.GetString(r2.TotalByte);
								    	      	sRcvMsg = sRcvMsg.Substring(nHeadLength, nRcvMsgLength - nHeadLength);
								    	      	
								    	      	args.Log.Write("plcChangeOverStarted_R : Body Length " + sRcvMsg.Length, sRcvMsg);
									});
								}
								else
								{
									args.CustomerProperty.SetValue("changeFlagStatus", "false");
									args.CustomerProperty.SetValue("ChangeJobName", string.Empty);
									strErrorMSG = r.ErrorText;
					                		bReturn = false;
								}
							});							
                                	}
					}
					else if (xNode != null)
			        	{
			        		//NG
		                		strErrorMSG = xNode.InnerText;
		                		bReturn = false;
			        	 }
				        else
				        {
				        	//NG
		                		strErrorMSG = "Return Code is null";
		                		bReturn = false;
				        }
				}
				else
				{
					//NG
	                		strErrorMSG = "dataDownloadRequired response is not valid from MES";
	                		bReturn = false;
				}
			});
			
			return bReturn;
		}
		catch(Exception ex)
		{
            		strErrorMSG = ex.Message;
            		return false;
		}
	}	   

}
