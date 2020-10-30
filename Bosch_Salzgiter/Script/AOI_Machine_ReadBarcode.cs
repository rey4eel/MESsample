 //css_reference System.Core;
//css_reference  System.Data.DataSetExtensions;
//css_reference  System.Xml;
//css_reference  System.Xml.Linq;

using System;
using System.IO;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Threading;
using System.Windows.Forms;
using KSMART.BRM;


public class Script
{
    static Form m_bTest;
    static bool m_bReturn = false;
    
    public static void Machine_ReadBarcode(KSMART.BRM.Script.AOIMachineReadBarcodeArgs args)
    {
    		if (args.AOI.Machine.Current.UseMES("AOI") != true)
    		{
    			args.SendOK();
    			return;
    		}
    		
		args.CustomerProperty.SetValue("Barcode", args.PCBBarcode);
		args.CustomerProperty.SetValue("Side",  args.TB);
		args.CustomerProperty.SetValue("Lane",  args.Lane);
    		SendPartReceived(args);
    	/*
		args.CustomerProperty.SetValue("MachinePCHK", "ON");
		try
		{
			args.CustomerProperty.SetValue("eventId", (args.CustomerProperty.GetValue("eventId").ToInt() + 1).ToString());
			args.CustomerProperty.SetValue("Barcode", args.PCBBarcode);
			args.CustomerProperty.SetValue("Side",  args.TB);
			args.CustomerProperty.SetValue("Lane",  args.Lane);
			
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
					if((nRcvMsgLength == 0) && (recv.BytesTransferred >= 6))
					{
						Array.Copy(recv.Bytes, abRcvHead, nHeadLength);
						Array.Reverse(abRcvHead);
						nRcvMsgLength = BitConverter.ToInt32(abRcvHead, 0);
					}
					
					if((nRcvMsgLength > 0) && (recv.BytesTransferred >= nRcvMsgLength))
						recv.Complete();			
				},			
				(rcv)=>
				{
					  //Receive
					sRcvMsg = System.Text.Encoding.UTF8.GetString(rcv.TotalBytes);
					sRcvMsg = sRcvMsg.Substring(nHeadLength, nRcvMsgLength - nHeadLength);
					
					args.Log.Write("partReceived_R : Body Length " + sRcvMsg.Length, sRcvMsg);
					
					RcvPartReceived(args, sRcvMsg);
				},
				(error)=>
				{
					//Exception
					switch(error.CommunicationError)
					{
							case emCommunicationError.NotConnected:
						//Not Connected
						args.SendNG(args.CustomerProperty.GetValue("ConnFailMessage"));
								break;
					case emCommunicationError.Timeout:
						//Timeout Alarm Message
						args.SendNG(args.CustomerProperty.GetValue("TimeoutMessage"));
						break;
					case emCommunicationError.Exception:
						args.SendNG("Please, Check MES Log.");
						break;
					}
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
		}*/
	}
	
	private static void SendPartReceived(KSMART.BRM.Script.AOIMachineReadBarcodeArgs args)
	{
		args.CustomerProperty.SetValue("MachinePCHK", "ON");
		try
		{
			args.CustomerProperty.SetValue("eventId", (args.CustomerProperty.GetValue("eventId").ToInt() + 1).ToString());
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
							//Not Connected
							args.SendNG(args.CustomerProperty.GetValue("ConnFailMessage"));
							break;
						case emCommunicationError.Timeout:
							//Timeout Alarm Message
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
				
				args.Log.Write("partReceived_R : Body Length " + sRcvMsg.Length, sRcvMsg);
				
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
    
    	private static void RcvPartReceived(KSMART.BRM.Script.AOIMachineReadBarcodeArgs args, string sRcvMsg)
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
		                         	batch = xNode.Attributes["batch"].Value.ToString();	
	
						//for Test
						//args.WriteLog("FrontTypeNo : " + args.CustomerProperty["FrontTypeNo"] + ", RearTypeNo : " + args.CustomerProperty["RearTypeNo"]);
						//check processNo & nextProcessNo
						if(!changeOver)
						{
							if(!partForStation)
							{
			                         		//NG
			                         		args.SendNG(args.CustomerProperty.GetValue("BoardBadmarkMessage"));
			                         		return;
							}
							
							if (args.Lane == "0")
							{
								if(args.TB == "T")
								{
									processNo = args.CustomerProperty.GetValue("FrontTopProcessNo");
								}
								else
								{
									processNo = args.CustomerProperty.GetValue("FrontBottomProcessNo");
								}
							}
							else
							{
								if(args.TB == "T")
								{
									processNo = args.CustomerProperty.GetValue("RearTopProcessNo");
								}
								else
								{
									processNo = args.CustomerProperty.GetValue("RearBottomProcessNo");
								}
							}
							
							if(processNo != nextProcessNo)
							{
								if(nextProcessNo == args.CustomerProperty.GetValue("FrontTopProcessNo"))
								{
									args.CustomerProperty.SetValue("Lane",  "0");
									args.CustomerProperty.SetValue("Side",  "T");
								}
								else if(nextProcessNo == args.CustomerProperty.GetValue("FrontBottomProcessNo"))
								{
									args.CustomerProperty.SetValue("Lane",  "0");
									args.CustomerProperty.SetValue("Side",  "B");
								}
								else if(nextProcessNo == args.CustomerProperty.GetValue("RearTopProcessNo"))
								{
									args.CustomerProperty.SetValue("Lane",  "1");
									args.CustomerProperty.SetValue("Side",  "T");
								}
								else if(nextProcessNo == args.CustomerProperty.GetValue("RearBottomProcessNo"))
								{
									args.CustomerProperty.SetValue("Lane",  "1");
									args.CustomerProperty.SetValue("Side",  "B");
								}
								else
								{
									args.SendNG("nextProcessNo  is wrong. Please check BRM configration.");
									return;
								}
								
								args.CustomerProperty.SetValue("FrontTypeNo", typeNo);
								args.CustomerProperty.SetValue("FrontTypeVar", typeVar);		
								args.CustomerProperty.SetValue("RearTypeNo", typeNo);
								args.CustomerProperty.SetValue("RearTypeVar", typeVar);
								args.CustomerProperty.SetValue("batchStatus", batch);
								
								SendPartReceived(args);
								return;
							}
						}
						
						if(changeOver) //Job Change
						{
							///////////////////////////////////////////////////////////////////////////													
							//Job Change - Start
							///////////////////////////////////////////////////////////////////////////
							
							//args.MessageBox.Show("", "ChangeOver", MessageBoxButtons.OK);
				      			RtCmdResponseArgs var = args.AOI.Machine.RtCmdStop("AOI");
				      			Thread.Sleep(1000);
				      			var = args.AOI.Machine.RtCmdReset("AOI");
				      			Thread.Sleep(1000);
					    		//args.MessageBox.Show("changeOver flag is true. You have to changeOver.", "ChangeOver", MessageBoxButtons.OK);
					    		//args.AOI.Machine.RtCmdNotify("AOI", "ChangeOver", "changeOver flag is true. You have to changeOver.");
						    	args.CustomMessageForm.ShowModal("ChangeOver flag is true.\r\nYou have to changeOver.", emMessageIcon.Infomation, 
						    	(f)=>
						    	{	
						    		m_bTest = f.Form;
						    		f.Form.Text = "ChangeOver";
						    		f.Button1Text = "OK";
						    		f.UseButton1 = true;
						    		f.Form.Width = 500;
						    	}
						    	,(r)=>
						    	{
								m_bTest.Close();

								args.CustomerProperty.SetValue("COFrontTypeNo", typeNo);
								args.CustomerProperty.SetValue("COFrontTypeVar", typeVar);		
								args.CustomerProperty.SetValue("CORearTypeNo", typeNo);
								args.CustomerProperty.SetValue("CORearTypeVar", typeVar);
								
								if(DoChangeOver(args) == true)
								{
									args.CustomerProperty.SetValue("FrontTypeNo", typeNo);
									args.CustomerProperty.SetValue("FrontTypeVar", typeVar);		
									args.CustomerProperty.SetValue("RearTypeNo", typeNo);
									args.CustomerProperty.SetValue("RearTypeVar", typeVar);
									args.CustomerProperty.SetValue("batchStatus", batch);
								}
						    	});
					    		
							return;
						}
						else 
						{
	                                	//Update
	                                	//typeNo, typeVar, nextProcessNo -> processNo, workingCode
							args.CustomerProperty.SetValue("FrontTypeNo", typeNo);
							args.CustomerProperty.SetValue("FrontTypeVar", typeVar);		
							args.CustomerProperty.SetValue("RearTypeNo", typeNo);
							args.CustomerProperty.SetValue("RearTypeVar", typeVar);
							args.CustomerProperty.SetValue("batchStatus", batch);
	                                	
	                                	//workingCode		                                	
	                                	args.CustomerProperty.SetValue("workingCode",  xNode.Attributes["workingCode"].Value.ToString());
							if (args.Lane == "0")
							{
								args.CustomerProperty.SetValue("statIdx", args.CustomerProperty.GetValue("FrontStatIdx"));
							}
							else
							{
								args.CustomerProperty.SetValue("statIdx", args.CustomerProperty.GetValue("RearStatIdx"));
							}
							
							
	                                	xNode = xDoc.SelectSingleNode("/root/body/structArrays/array[@name='tools']/values");
	                                	foreach (XmlNode xn in xNode.ChildNodes)
	                                	{
	                                		string sToolID = xn.Attributes["toolID"].Value;
	                                		int nState = xn.Attributes["state"].Value.ToInt();
                                			if(nState != 0)
                                			{
	                                			if(nState == 1)
	                                			{
	                                				args.SendNG("The tool is warning limit : " + sToolID);
	                                				return;
	                                			}
	                                			else
	                                			{
	                                				args.SendNG("The tool is blocked : " + sToolID);
	                                				return;
	                                			}
                                			}
	                                	}
						
	                                	//Barcode, Badmark Check
	                                	//Dictionary<object,object> dicArray = new Dictionary<object,object>();
	                                	List<int> lstArrayBadmark = new List<int>();
	                                	List<string> lstArrayBarcode = new List<string>();
	                                	
	                                	xNode = xDoc.SelectSingleNode("/root/body/structArrays/array[@name='workItems']/values");
	                                	if(xNode.IsNull() == true)
							{
								if (args.ArrayCount.ToInt() != 1)
								{
									args.SendNG(args.CustomerProperty.GetValue("ArrayCountDifferentMessage"));	
								}
								else
								{
									args.SendOK();
									
			                                      args.CustomerProperty.SetValue("eventId", (args.CustomerProperty.GetValue("eventId").ToInt() + 1).ToString());
			                                    	
			                                	string sSendMsg = args.Xsl.StringTransform(args.CustomerProperty.GetValue("partProcessingStarted"));
			                                	
			                                	int nHeadLength = 4;
									int nBodyLength = sSendMsg.Length;
									byte[] abHead = new byte[nHeadLength];
									abHead = BitConverter.GetBytes(nHeadLength + nBodyLength);
									byte[] abMsg = new byte[nHeadLength + nBodyLength];
							
									Array.Reverse(abHead);
									Array.Resize(ref abMsg, nHeadLength + nBodyLength);
								      Array.Copy(abHead, abMsg, nHeadLength);
									Array.Copy(System.Text.Encoding.UTF8.GetBytes(sSendMsg), 0, abMsg, nHeadLength, nBodyLength);
									args.SocketClient.SendByte("MES", abMsg);
									
									args.Log.Write("partProcessingStarted : Body Length " + sSendMsg.Length, sSendMsg);	
									args.CustomerProperty.SetValue("NowProcessing", "1");
								}
							}	
	                                	//bool bAllFalsePartForStation = true;
	                                	else if (args.ArrayCount.ToInt() == xNode.ChildNodes.Count)
	                                	{
		                                	foreach (XmlNode xn in xNode.ChildNodes)
		                                	{
		                                		int nArrayIdx = xn.Attributes["pos"].Value.ToInt();
		                                		string sBarcode = xn.Attributes["id"].Value;
		                                		//bool bBadMarkN = (xn.Attributes["state"].Value.ToInt() == 12) ? true : false;
		                                		args.Log.Write("pos : " + nArrayIdx.ToString() + "   id : " + sBarcode );
		                                		/*
		                                		if (!bBadMarkN)
		                                		{
		                                			if (bAllFalsePartForStation)
		                                				bAllFalsePartForStation = false;
		                                		}
		                                		else //Badmark
		                                		{
		                                			lstArrayBadmark.Add(nArrayIdx);
		                                		}*/
		                                		lstArrayBarcode.Add(sBarcode);
		                                	}
		                                	
		                                	//if (!bAllFalsePartForStation)
		                                	//{
									if(lstArrayBarcode.Count > 0)
										args.SendOK(null, lstArrayBarcode);
									else
										args.SendOK();

			                                    	//partProcessingStarted
			                                      args.CustomerProperty.SetValue("eventId", (args.CustomerProperty.GetValue("eventId").ToInt() + 1).ToString());
			                                    	
			                                	string sSendMsg = args.Xsl.StringTransform(args.CustomerProperty.GetValue("partProcessingStarted"));
			                                	//sSendMsg = sSendMsg.Length.ToString("0000") + sSendMsg;		
			                                	//args.SendMessageSocketClient("MES", sSendMsg);
			                                	
			                                	int nHeadLength = 4;
									int nBodyLength = sSendMsg.Length;
									byte[] abHead = new byte[nHeadLength];
									abHead = BitConverter.GetBytes(nHeadLength + nBodyLength);
									byte[] abMsg = new byte[nHeadLength + nBodyLength];
							
									Array.Reverse(abHead);
									Array.Resize(ref abMsg, nHeadLength + nBodyLength);
								      Array.Copy(abHead, abMsg, nHeadLength);
									Array.Copy(System.Text.Encoding.UTF8.GetBytes(sSendMsg), 0, abMsg, nHeadLength, nBodyLength);
									args.SocketClient.SendByte("MES", abMsg);
									
									args.Log.Write("partProcessingStarted : Body Length " + sSendMsg.Length, sSendMsg);	
									args.CustomerProperty.SetValue("NowProcessing", "1");
		                                	//}
		                                	//else
		                                	//{
									//All False in PartForStation : All Arrays are Badmark
								//	args.SendNG(args.CustomerProperty.GetValue("AllArrayBadmarkMessage"));
		                                	//}
		                                }
	                                	else
	                                	{
	                                		//ARRAY_CNT != values count
								args.SendNG(args.CustomerProperty.GetValue("ArrayCountDifferentMessage"));	
	                                	}						
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
					if(xNode.Attributes["returnCode"].Value.ToInt() > 0)
					{
						args.SendNG("Return Code : " + xNode.InnerText);
					}
					else
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
    	
    	private static bool SendplcChangeOverStarted(KSMART.BRM.Script.AOIMachineReadBarcodeArgs args)
    	{
    		try
    		{
    			m_bReturn = false;
    			
	             args.CustomerProperty.SetValue("eventId", (args.CustomerProperty.GetValue("eventId").ToInt() + 1).ToString());
	            	string sSendMsg = args.Xsl.StringTransform(args.CustomerProperty.GetValue("plcChangeOverStarted"));
	            	
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
	                	
			nBodyLength = sSendMsg.Length;
			abHead = BitConverter.GetBytes(nHeadLength + nBodyLength);
	
			Array.Reverse(abHead);
			Array.Resize(ref abMsg, nHeadLength + nBodyLength);
		      Array.Copy(abHead, abMsg, nHeadLength);
			Array.Copy(System.Text.Encoding.UTF8.GetBytes(sSendMsg), 0, abMsg, nHeadLength, nBodyLength);
			
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
							args.SendNG(args.CustomerProperty.GetValue("ConnFailMessage"));
				      			break;
						case emCommunicationError.Timeout:
							args.SendNG(args.CustomerProperty.GetValue("TimeoutMessage"));
							break;
						case emCommunicationError.Exception:
							args.SendNG("Please, Check MES Log.");
							break;
						default:
							args.SendNG("Communication Error.");
							break;
				      }
				}
			},			
			(r2)=>
			{
				sRcvMsg = System.Text.Encoding.UTF8.GetString(r2.TotalByte);
		    	      	sRcvMsg = sRcvMsg.Substring(nHeadLength, nRcvMsgLength - nHeadLength);
		    	      	
		    	      	args.Log.Write("plcChangeOverStarted_R : Body Length " + sRcvMsg.Length, sRcvMsg);
		    	      	
				XmlDocument xDoc = new XmlDocument();
		    	      	if ( (sRcvMsg.Length > 4) && sRcvMsg.IsXmlDocumentFromString() )
		    	      		xDoc.LoadXml(sRcvMsg);
				
			      if(xDoc != null)
			      {
					XmlNode xNode = xDoc.SelectSingleNode("/root/event/result");
		
				      if ((xNode != null) && (xNode.Attributes["returnCode"].Value.ToString() == "0"))
				      {
				      		m_bReturn = true;
				      }
					else if (xNode != null)
					{
						 args.SendNG( xNode.InnerText);
					}
					else
					{
						 args.SendNG("Return Code is null");
					}
				}
				else
				{
					args.SendNG("plcChangeOverStarted response is not valid from MES");		
				}
			});
			
			return m_bReturn;
		}
		catch(Exception ex)
		{
			args.Log.WriteException("plcChangeOverStarted", ex);
			args.SendNG(ex.Message);
			return false;
		}
    	}

	private static bool DoChangeOver(KSMART.BRM.Script.AOIMachineReadBarcodeArgs args)
	{
		try
		{
			if (SendplcChangeOverStarted(args) == false) return false;
			
		     	args.CustomerProperty.SetValue("eventId", (args.CustomerProperty.GetValue("eventId").ToInt() + 1).ToString());
			string sSendMsg = args.Xsl.StringTransform(args.CustomerProperty.GetValue("dataDownloadRequired"));
	            	
	            	m_bReturn = true;
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

                    	
			nBodyLength = sSendMsg.Length;
			abHead = BitConverter.GetBytes(nHeadLength + nBodyLength);
	
			Array.Reverse(abHead);
			Array.Resize(ref abMsg, nHeadLength + nBodyLength);
		      Array.Copy(abHead, abMsg, nHeadLength);
			Array.Copy(System.Text.Encoding.UTF8.GetBytes(sSendMsg), 0, abMsg, nHeadLength, nBodyLength);
			
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
					//Exception
				      switch(recv.Error)
				      {
				      		case emCommunicationError.NotConnected:
							//Not Connected
							args.SendNG(args.CustomerProperty.GetValue("ConnFailMessage"));
				      			break;
						case emCommunicationError.Timeout:
							//Timeout Alarm Message
							args.SendNG(args.CustomerProperty.GetValue("TimeoutMessage"));
							break;
						case emCommunicationError.Exception:
							args.SendNG("Please, Check MES Log.");
							break;
				      }
				      m_bReturn = false;
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
						string strSide = "N";
						
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
							args.SendNG("Job file name is empty from MES");
							m_bReturn = false;
							return;
						}

						string sChangeJob = string.Empty;
						if(strSide == "T")
						{
							sChangeJob = sTopJob + args.CustomerProperty.GetValue("TopProgramSuffix");
						}
						else if(strSide == "B")
						{
							sChangeJob = sBotJob + args.CustomerProperty.GetValue("BottomProgramSuffix");
						}
						else
						{
							if(args.TB == "T")
	                                	{
	                                		sChangeJob = sTopJob + args.CustomerProperty.GetValue("TopProgramSuffix");
	                                	} 
	                                	else if (sBotJob.Length > 0)
	                                	{
	                                		sChangeJob = sBotJob + args.CustomerProperty.GetValue("BottomProgramSuffix");
	                                	}	
	                                }

                                	//sChangeJob = Path.GetFileNameWithoutExtension(sChangeJob);
                                	args.Log.Write("sChangeJob : " + sChangeJob);
                                	if(sChangeJob.Length > 0)
                                	{
                                		m_bReturn = true;
							args.CustomerProperty.SetValue("changeFlagStatus", "true");                                	
				      			args.CustomerProperty.SetValue("ChangeJobName", sChangeJob);
				      			
				      			RtCmdResponseArgs var = args.AOI.Machine.RtCmdJobChange("AOI", args.Lane,  sChangeJob,  string.Empty, true, null, 30000);
				      			if(var.Success == false)
				      			{
								args.CustomerProperty.SetValue("changeFlagStatus", "false");
								args.CustomerProperty.SetValue("ChangeJobName", string.Empty);
								 
								args.SendNG("[CHANGEOVER " +var.ErrorCode + "] " + var.ErrorText);
								m_bReturn = false;
				      			}
				      			
				      			/*
							args.AOI.Machine.JobChange("AOI", args.Lane, sChangeJob,  string.Empty, false, true, null, 30000,
							(r)=>
							{
								if (r.Result != emJobChangeResponseResult.Success)
								{
									args.CustomerProperty.SetValue("changeFlagStatus", "false");
									args.CustomerProperty.SetValue("ChangeJobName", string.Empty);
									
									args.SendNG("[CHANGEOVER " + r.Result.ToString() + "] " + r.ErrorText);
								}
							});
							*/
                                	}
					}
					else if (xNode != null)
			        	{
			        		//NG
			        		args.SendNG("Return Code : " + xNode.InnerText);
			        		m_bReturn = false;
			        	 }
				        else
				        {
				        	//NG
		                    	args.SendNG("Return Code : null");
		                    	m_bReturn = false;
				        }
				}
				else
				{
					//NG
					args.SendNG("dataDownloadRequired response is not valid from MES");
					m_bReturn = false;					
				}
			});
			
			return m_bReturn;
		}
		catch(Exception ex)
		{
			args.Log.WriteException("DoChangeOver", ex);
			return false;
		}
	}	   

}