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
    static string m_strErrorMSG = "";
    static string m_strSide = "T";
    static bool m_bJigOpen = false;
    static bool m_bReturn = false;
    static Form m_bTest;
    
    public static void Machine_UserAction(KSMART.BRM.Script.AOIMachineUserActionArgs args)
    {
    		if (args.AOI.Machine.Current.UseMES("AOI") != true)
    		{
    			if (args.Event == "CHANGE_OVER")
    			{
    				args.MessageBox.Show("[Use MES] option must be turned on.", "Infomation");
    			}
    			
    			return;
    		}
    		
    		//if (args.Event != "JIG_OPEN") return;
    		if (args.Event != "CHANGE_OVER") return;
    		
		args.CustomVerticalGridForm.Show(args.CustomerProperty.GetValue("ToolChange"),
		(form) =>
		{
			form.UseOK = false;
			form.Form.Width = 700;
			form.Form.Height = 500;
			form.Form.Text = "Change Over";
			form.Form.MaximizeBox = false;
			m_strErrorMSG = "";
			m_strSide = "T";
			m_bJigOpen = false;
			
			args.AOI.Machine.RtCmdRunDisable("AOI");
			form.Grid.SetCellValue("txtInfo", "Scanning change-over label.\r\n");
			form.Grid.SetFocusEditor("txtChangeOverLabel");
			
			form.FormClosed += (s,e)=>
			{
				args.Log.Write("Close.");
			};
			
			form.OnValueChanged = (f, bt) =>
			{
				if(bt.FieldName == "txtChangeOverLabel")
				{
					string strValue = bt.Value.ToString();
					args.CustomerProperty.SetValue("ToolFamily", strValue);
					
					strValue = strValue.Replace(".", "");
					if (strValue.Length  >= 14)
					{
						form.Grid.SetCellValue("txtInfo", "\r\n");
						if (args.MessageBox.Show("Do you want to start change over?", "Infomation", MessageBoxButtons.YesNo) == DialogResult.Yes)
						{
			                        	string typeNo = strValue.Left(10);
			                        	string typeVar = strValue.Right(4);
			                        	
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
								
								form.Grid.SetFocusEditor("txtInCarrier");
								form.Grid.SetCellValue("txtInfo", "Scanning inserted carrier.\r\n");
								form.Grid.SetCellValue("txtNewProgram", args.CustomerProperty.GetValue("Prog1.Name"));
								form.Grid.SetCellValue("txtNeedCarrier", args.CustomerProperty.GetValue("Tool1.Name"));
							}
							else
							{
								args.MessageBox.Show(m_strErrorMSG, "Infomation");
								form.Grid.SetCellValue("txtInfo", "Scanning change-over label.\r\n");
								form.Grid.SetCellValue("txtChangeOverLabel", "");
								form.Grid.SetFocusEditor("txtChangeOverLabel");
							}
						}
						else
						{
							args.AOI.Machine.RtCmdRunEnable("AOI");	
							form.Close();
						}
					}
				}
				if(bt.FieldName == "txtInCarrier")
				{
					 if(bt.Value.ToString().Trim().Length > 0 && args.CustomerProperty.GetValue("Tool1.Name").ToString().Trim().Length > 0 &&
					 bt.Value.ToString().Trim().Length ==args.CustomerProperty.GetValue("Tool1.Name").ToString().Trim().Length)
					 {
					 	if(bt.Value.ToString().ToUpper() == args.CustomerProperty.GetValue("Tool1.Name").ToString().ToUpper())
					 	{
							//args.MessageBox.Show("Change over successful.", "Infomation",MessageBoxButtons.OK,MessageBoxIcon.Information);
							//args.AOI.Machine.RtCmdNotify("AOI", "Infomation", "Change over successful.");
							form.Close();
					 		if(m_bJigOpen == true)
					 		{
								ToolChangeEnd(args);
								
								Dictionary<string,object> dic = new Dictionary<string,object>();
								dic.Add("LANE", "0");
								RtCmdResponseArgs rtn = args.AOI.Machine.RtCmdCustom("AOI","JIG_CLOSE", dic);
								if(!rtn.Success)
								{
									args.Log.Write("JIG_CLOSE FAIL Response Error", rtn.ErrorText  );
								}
					 		}
							args.AOI.Machine.RtCmdRunEnable("AOI");	
							
						    	args.CustomMessageForm.ShowModal("Change over successful.", emMessageIcon.Infomation, 
						    	(cf)=>
						    	{	
						    		m_bTest = f.Form;
						    		cf.Form.Text = "Infomation";
						    		cf.Button1Text = "OK";
						    		cf.UseButton1 = true;
						    		cf.Form.Width = 500;
						    	}
						    	,(cr)=>
						    	{
								m_bTest.Close();
								string sNoti = string.Format("STD|NET|GUI|RPT|NAME=TOOL_CHANGE|LANE=0|TOOL_ID={0}|FAMILY={1}|COUNT={2}", args.CustomerProperty.GetValue("ToolID.Current"), args.CustomerProperty.GetValue("ToolFamily"), args.CustomerProperty.GetValue("ToolCounter") );
								args.SocketServer.SendMessage("AOI", sNoti);
						    	});
						    	
					 	}
					 	else
					 	{
					 		string strInfo = "Please change the carrier.\r\nCarrier needed : " + args.CustomerProperty.GetValue("Tool1.Name").ToString();
					 		form.Grid.SetCellValue("txtInfo", strInfo);
	
					 		form.Grid.SetCellValue("txtInCarrier", "");
					 		form.Grid.SetFocusEditor("txtInCarrier");
	
							Dictionary<string,object> dic = new Dictionary<string,object>();
							dic.Add("LANE", "0");
							RtCmdResponseArgs rtn = args.AOI.Machine.RtCmdCustom("AOI","JIG_OPEN", dic);
							if(!rtn.Success)
							{
								args.Log.Write("JIG_OPEN FAIL Response Error", rtn.ErrorText  );
							}
							
							if (m_bJigOpen == false)
							{
								ToolChangeStart(args, args.CustomerProperty.GetValue("Tool1.Name").ToString().Trim());
							}
							
							m_bJigOpen = true;
							
					 	}
					 }
				}
			};
		});
    }
    
    public static bool ToolChangeStart(KSMART.BRM.Script.AOIMachineUserActionArgs args, string ToolID)
    {
	try
	{
		m_bReturn = true;
		
		args.CustomerProperty.SetValue("eventId", (args.CustomerProperty.GetValue("eventId").ToInt() + 1).ToString());
		args.CustomerProperty.SetValue("ToolOldID", args.CustomerProperty.GetValue("ToolID.Current"));
		args.CustomerProperty.SetValue("ToolID", ToolID);
		
		string sSendMsg = args.Xsl.StringTransform(args.CustomerProperty.GetValue("plcToolChangeStarted"));
		
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
				args.Log.Write("plcToolChangeStarted : Body Length " + sSendMsg.Length, sSendMsg);
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
							m_strErrorMSG = args.CustomerProperty.GetValue("ConnFailMessage");
							args.MessageBox.Show(m_strErrorMSG, "Infomation");
				      			break;
						case emCommunicationError.Timeout:
							//Timeout Alarm Message
							m_strErrorMSG = args.CustomerProperty.GetValue("TimeoutMessage");
							args.MessageBox.Show(m_strErrorMSG, "Infomation");
							break;
						case emCommunicationError.Exception:
							m_strErrorMSG = "Please, Check MES Log.";
							args.MessageBox.Show(m_strErrorMSG, "Infomation");
							break;
				      }
					m_bReturn = false;				      
				}

			},
			(rcv)=>
			{
				  //Receive
				sRcvMsg = System.Text.Encoding.UTF8.GetString(rcv.TotalByte);
				sRcvMsg = sRcvMsg.Substring(nHeadLength, nRcvMsgLength - nHeadLength);
				
				args.Log.Write("plcToolChangeStarted_R : Body Length " + sRcvMsg.Length, sRcvMsg);
				
				XmlDocument xDoc = new XmlDocument();
				
		    	      	if ( (sRcvMsg.Length > 4) && sRcvMsg.IsXmlDocumentFromString() )
		    	      		xDoc.LoadXml(sRcvMsg);
	
			      if(xDoc != null)
			      {
					XmlNode xNode = xDoc.SelectSingleNode("/root/event/result");
					
				      if ((xNode != null) && (xNode.Attributes["returnCode"].Value.ToString() == "0"))
				      {
						var ini = args.INI.Load(@"C:\KohYoung\KSMART Business Rules Management\bosch.ini");
						ini.Write("MES", "ToolIDCurrent", ToolID);
						ini.Write("MES", "ToolIDOld", args.CustomerProperty.GetValue("ToolID.Current"));		
						ini.Save();
						
		                        	xNode = xDoc.SelectSingleNode("/root/body/structArrays/array/values/item");
		
		                        	if(xNode != null)
		                        	{
			                        	//string ToolID = string.Empty;
			                        	string Family = string.Empty;
			                        	string Counter = string.Empty;
			                        	
			                         	//ToolID =  xNode.Attributes["toolID"].Value.ToString();
			                         	Family = xNode.Attributes["family"].Value.ToString();	
			                         	Counter = xNode.Attributes["counter"].Value.ToString();	
		
							//args.CustomerProperty.SetValue("ToolFamily", Family);
							args.CustomerProperty.SetValue("ToolCounter", Counter);
		                        	}
		                        	else
		                        	{
		                        		//NG
		                        		m_strErrorMSG = "No workPart telegram";
		                        		args.MessageBox.Show(m_strErrorMSG, "Infomation");
		                        		m_bReturn = false;
		                        	}
		                    
				      }
					else if (xNode != null)
					{
						//NG
						XmlNode xNode1 = xDoc.SelectSingleNode("/root/event/trace/trace");
						if (xNode1 != null)
						{
							m_strErrorMSG = xNode1.Attributes["text"].Value.ToString();
						}
						else
						{
							m_strErrorMSG = xNode.InnerText;
						}
						
	                        		args.MessageBox.Show(m_strErrorMSG, "Infomation");
	                        		m_bReturn = false;
					}
					else
					{
						//NG
	                        		m_strErrorMSG = "Return Code is null";
	                        		args.MessageBox.Show(m_strErrorMSG, "Infomation");
	                        		m_bReturn = false;
					}
			        }
			        else
			        {
			           //NG
	                		m_strErrorMSG = "Return is null";
	                		args.MessageBox.Show(m_strErrorMSG, "Infomation");
	                		m_bReturn = false;
			        }
			});
			
		return m_bReturn;
	}
	catch(Exception ex) 
	{		
		m_strErrorMSG = ex.Message;
		args.MessageBox.Show(m_strErrorMSG, "Infomation");
		return false;
	}
    }

    public static bool ToolChangeEnd(KSMART.BRM.Script.AOIMachineUserActionArgs args)
    {
	try
	{
		m_bReturn = true;
		
		args.CustomerProperty.SetValue("eventId", (args.CustomerProperty.GetValue("eventId").ToInt() + 1).ToString());
		string sSendMsg = args.Xsl.StringTransform(args.CustomerProperty.GetValue("plcToolChanged"));
		
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
				args.Log.Write("plcToolChanged : Body Length " + sSendMsg.Length, sSendMsg);
				nRcvMsgLength = 0;
			},
			(recv)=>
			{
				if (recv.Success)
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
							m_strErrorMSG = args.CustomerProperty.GetValue("ConnFailMessage");
							args.MessageBox.Show(m_strErrorMSG, "Infomation");
				      			break;
						case emCommunicationError.Timeout:
							m_strErrorMSG = args.CustomerProperty.GetValue("TimeoutMessage");
							args.MessageBox.Show(m_strErrorMSG, "Infomation");
							break;
						case emCommunicationError.Exception:
							m_strErrorMSG = "Please, Check MES Log.";
							args.MessageBox.Show(m_strErrorMSG, "Infomation");
							break;
				      }
					m_bReturn = false;				      
				}
			},
			(rcv)=>
			{
				  //Receive
				sRcvMsg = System.Text.Encoding.UTF8.GetString(rcv.TotalByte);
				sRcvMsg = sRcvMsg.Substring(nHeadLength, nRcvMsgLength - nHeadLength);
				
				args.Log.Write("plcToolChanged_R : Body Length " + sRcvMsg.Length, sRcvMsg);
				
				XmlDocument xDoc = new XmlDocument();
				
		    	      	if ( (sRcvMsg.Length > 4) && sRcvMsg.IsXmlDocumentFromString() )
		    	      		xDoc.LoadXml(sRcvMsg);
	
			      if(xDoc != null)
			      {
					XmlNode xNode = xDoc.SelectSingleNode("/root/event/result");
					
				      if ((xNode != null) && (xNode.Attributes["returnCode"].Value.ToString() == "0"))
				      {
						args.CustomerProperty.SetValue("ToolID.Current", args.CustomerProperty.GetValue("ToolID"));
	                        		m_bReturn = true;
				      }
					else if (xNode != null)
					{
						//NG
	                        		m_strErrorMSG = xNode.InnerText;
	                        		args.MessageBox.Show(m_strErrorMSG, "Infomation");
	                        		m_bReturn = false;
					}
					else
					{
						//NG
	                        		m_strErrorMSG = "Return Code is null";
	                        		args.MessageBox.Show(m_strErrorMSG, "Infomation");
	                        		m_bReturn = false;
					}
			        }
			        else
			        {
			           //NG
	                		m_strErrorMSG = "Return is null";
	                		args.MessageBox.Show(m_strErrorMSG, "Infomation");
                        		m_bReturn = false;
			        }
			});
			
		return m_bReturn;
	}
	catch(Exception ex) 
	{		
		m_strErrorMSG = ex.Message;
		args.MessageBox.Show(m_strErrorMSG, "Infomation");
		return false;
	}
    }

    	private static bool SendplcChangeOverStarted(KSMART.BRM.Script.AOIMachineUserActionArgs args)
    	{
    		try
    		{
			m_bReturn = true;
			
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
							m_strErrorMSG = args.CustomerProperty.GetValue("ConnFailMessage");
				      			break;
						case emCommunicationError.Timeout:
							m_strErrorMSG = args.CustomerProperty.GetValue("TimeoutMessage");
							break;
						case emCommunicationError.Exception:
							m_strErrorMSG = "Please, Check MES Log.";
							break;
						default:
							m_strErrorMSG = "Communication Error.";
							break;
				      }
				      m_bReturn = false;
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
						m_strErrorMSG = xNode.InnerText;
						m_bReturn = false;
					}
					else
					{
						m_strErrorMSG = "Return Code is null";
						m_bReturn = false;
					}
				}
				else
				{
					m_strErrorMSG = "plcChangeOverStarted response is not valid from MES";
					m_bReturn = false;
				}
			});
			
			return m_bReturn;
		}
		catch(Exception ex)
		{
			args.Log.WriteException("plcChangeOverStarted", ex);
			m_strErrorMSG = ex.Message;
			return false;
		}
    	}
/*
    	private static bool RcvPartReceived(KSMART.BRM.Script.AOIMachineUserActionArgs args, string sRcvMsg)
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
		                        	bool bTool = false;
		                        	
		                         	typeNo =  xNode.Attributes["typeNo"].Value.ToString();
		                         	typeVar = xNode.Attributes["typeVar"].Value.ToString();	
		                         	batch = xNode.Attributes["batch"].Value.ToString();	
	
						args.CustomerProperty.SetValue("FrontTypeNo", typeNo);
						args.CustomerProperty.SetValue("FrontTypeVar", typeVar);
						args.CustomerProperty.SetValue("RearTypeNo", typeNo);
						args.CustomerProperty.SetValue("RearTypeVar", typeVar);
						args.CustomerProperty.SetValue("batchStatus", batch);
						
                                	xNode = xDoc.SelectSingleNode("/root/body/structArrays/array[@name='tools']/values");
                                	foreach (XmlNode xn in xNode.ChildNodes)
                                	{
                                		string sToolID = xn.Attributes["toolID"].Value;
                                		int nState = xn.Attributes["state"].Value.ToInt();
                                		
                                		args.CustomerProperty.SetValue("Tool.Code", sToolID);
                                		args.Log.Write("Rcv Tool : " + sToolID);
                                		if (sToolID == m_strToolCode)
                                		{
                                			
                                			if(nState == 0)
                                			{
                                				bTool = true;
                                			}
                                			else if(nState == 1)
                                			{
                                				m_strErrorMSG = "The tool is warning limit : " + sToolID;
                                			}
                                			else
                                			{
                                				m_strErrorMSG = "The tool is blocked : " + sToolID;
                                			}
								break;                                			
                                		}
                                		else
                                		{
                                			m_strErrorMSG = "You have to change Tool!";
                                			return false;
                                		}
                                	}
						
						if (bTool == true)
						{
							return DoChangeOver(args);
						}
						else
						{
							return false;
						}
	                        	}
	                        	else
	                        	{
	                        		//NG
	                        		m_strErrorMSG = "No workPart telegram";
	                        		return false;
	                        	}
	                    
			      }
				else if (xNode != null)
				{
					//NG
                        		m_strErrorMSG = xNode.InnerText;
                        		return false;
				}
				else
				{
					//NG
                        		m_strErrorMSG = "Return Code is null";
                        		return false;
				}
		        }
		        else
		        {
		           //NG
                		m_strErrorMSG = "Return is null";
                		return false;
		        }
		}
		catch(Exception ex) 
		{
            		m_strErrorMSG = ex.Message;
            		return false;
		}		
    	}

    	private static bool RcvData(KSMART.BRM.Script.AOIMachineUserActionArgs args, string sRcvMsg)
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
					return true;	                    
			      }
				else if (xNode != null)
				{
					//NG
                        		m_strErrorMSG = xNode.InnerText;
                        		return false;
				}
				else
				{
					//NG
                        		m_strErrorMSG = "Return Code is null";
                        		return false;
				}
		        }
		        else
		        {
		           //NG
                		m_strErrorMSG = "Return is null";
                		return false;
		        }
		}
		catch(Exception ex) 
		{
            		m_strErrorMSG = ex.Message;
            		return false;
		}		
    	}
*/
	private static bool DoChangeOver(KSMART.BRM.Script.AOIMachineUserActionArgs args)
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
							m_strErrorMSG = args.CustomerProperty.GetValue("ConnFailMessage");
				      			break;
						case emCommunicationError.Timeout:
							m_strErrorMSG = args.CustomerProperty.GetValue("TimeoutMessage");
							break;
						case emCommunicationError.Exception:
							m_strErrorMSG = "Please, Check MES Log.";
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
                        			args.CustomerProperty.SetValue("Prog1.Name", string.Empty);                                			
                        			args.CustomerProperty.SetValue("Tool1.Family", string.Empty);                                			
                        			args.CustomerProperty.SetValue("Tool1.Name", string.Empty);
                        			args.CustomerProperty.SetValue("Tool2.Family", string.Empty);
                        			args.CustomerProperty.SetValue("Tool2.Name", string.Empty);
						
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
                                					m_strSide = "T";
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
                                					m_strSide = "B";
                                					break;
                                				}
                                			}
                                		}
                                	}

						//args.Log.Write("sTopJob : " + sTopJob);
						//args.Log.Write("sBotJob : " + sBotJob);
						if ((sTopJob.Length == 0) && (sBotJob.Length == 0))
						{
			                		m_strErrorMSG = "Job file name is empty from MES";
			                		m_bReturn = false;
			                		return;
						}

						string sChangeJob = string.Empty;
						if(m_strSide == "T")
						{
							sChangeJob = sTopJob + args.CustomerProperty.GetValue("TopProgramSuffix");
							args.CustomerProperty.SetValue("Tool.Side", "T");
						}
						else if(m_strSide == "B")
						{
							sChangeJob = sBotJob + args.CustomerProperty.GetValue("BottomProgramSuffix");
							args.CustomerProperty.SetValue("Tool.Side", "B");
						}
						
                                	//sChangeJob = Path.GetFileNameWithoutExtension(sChangeJob);
                                	args.Log.Write("sChangeJob : " + sChangeJob);
                                	args.CustomerProperty.SetValue("Prog1.Name", sChangeJob);
                                	if(sChangeJob.Length > 0)
                                	{
							m_bReturn = true;
							args.CustomerProperty.SetValue("changeFlagStatus", "true");
				      			args.CustomerProperty.SetValue("ChangeJobName", sChangeJob);
							args.AOI.Machine.JobChange("AOI", "0", sChangeJob,  string.Empty, false, false, null	, 30000, 
							(r)=>
							{
								if (r.Result != emJobChangeResponseResult.Success)
								{
									args.CustomerProperty.SetValue("changeFlagStatus", "false");
									args.CustomerProperty.SetValue("ChangeJobName", string.Empty);
									m_strErrorMSG = "[CHANGEOVER " + r.Result.ToString() + "] " + r.ErrorText;
					                		m_bReturn = false;
					                	}
							});
                                	}
					}
					else if (xNode != null)
			        	{
			        		//NG
		                		m_strErrorMSG = xNode.InnerText;
		                		m_bReturn = false;
			        	 }
				        else
				        {
				        	//NG
		                		m_strErrorMSG = "Return Code is null";
		                		m_bReturn = false;
				        }
				}
				else
				{
					//NG
	                		m_strErrorMSG = "dataDownloadRequired response is not valid from MES";
	                		m_bReturn = false;
				}
			});
			
			return m_bReturn;
		}
		catch(Exception ex)
		{
            		m_strErrorMSG = ex.Message;
            		return false;
		}
	}
}

