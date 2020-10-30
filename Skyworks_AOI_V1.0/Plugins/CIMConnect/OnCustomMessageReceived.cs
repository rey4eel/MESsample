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
using System.Text;
using System.Diagnostics;
using KSMART.BRM;
using KSMART.BRM.Script;

public class Script
{
    public static void OnCustomMessageReceived(KSMART.BRM.Script.CIMConnect.DefaultArgs args , int connId, int stream, int func, string message, int transactionID)
    {
        if (stream == 14 && func == 2)
        {
            args.KeyValue.SetValue("S14F2_Message", message);
            args.KeyValue.SetValue("S14F2_Error", GetError(message));

            args.SetResponse(KSMART.BRM.Plugins.CIMConnect.emCIMMessageResults.mNoReply);
            args.SetResponseData("");
        }
    }

    public static string GetError(string message)
    {
        /*
            L
            {U1 1}
            {L
            {A NOMAP}
            {A There is no data.}
            }
        */

        int index = 0;
        string code = string.Empty;
        string desc = string.Empty;
        string error = string.Empty;

        index = message.LastIndexOf("{U");

        if (index >= 0)
        {
            message = message.Substring(index);
            index = message.IndexOf("}");

            if (index >= 0)
            {
                message = message.Substring(index - 1);
                code = message.Left(1);

                if (code != "0")
                {
                    index = message.IndexOf("{A");

                    if (index >= 0)
                    {
                        message = message.Substring(index+3);
                        index = message.IndexOf("}");
                        code = message.Left(index);

                        message = message.Substring(index);
                        index = message.IndexOf("{A");

                        if (index >= 0)
                        {
                            message = message.Substring(index+3);
                            index = message.IndexOf("}");
                            desc = message.Left(index);

                            error = string.Format("{0} ({1})", desc, code);
                        }
                        else
                        {
                            error = code;
                        }
                    }
                }
            }
        }

        return error;
    }
}

//OnRemoteCommandRegister Example
//args.Plugin.CIMConnect.RegisterCommandHandler("START");


//OnRemoteCommandReceived Example
//args.SetResponse(emCIMCommandResults.cmdPerformed);


//OnCustomMessageRegister Example
//args.Plugin.CIMConnect.RegisterCustomMsgHandler(1, 100, 1)  //S100F1


//OnCustomMessageReceived Example
//args.SetResponse(emCIMMessageResult.mReply);
//args.SetResponseData("L {A testdata1}{A testdata2}");


//Variable Set
//args.Plugins.CIMConnect.SetValueByString(1, "MDLN", "SPI001");
//args.Plugins.CIMConnect.SetValueByFormatString(1, "PPExecName", "L {A Job1}{A Job2}");


//EventReportSend Example S6F11
//string[] varNames = new[] { "LaneID", "PCBID", "ProgramName" };
//string[] varValues = new[] { "0", "barcod21234", "kohyoung" };
//args.Plugins.CIMConnect.SetTriggerEvent("BarcodeReadingCompleted", varNames, varValues);


//OnHostPPLoadInqAck Example S7F2
//RecipeHandling 1=File-Based
//args.Plugins.CIMConnect.PPSend(1, "Kohyoung", string.Empty);  //S7F3
//RecipeHandling 2=Value-Based
//args.Plugins.CIMConnect.PPSend(1, "Kohyoung", "A RecipeData Any ASCII Data");  //S7F3
//args.Plugins.CIMConnect.PPSend(1, "Kohyoung", "Bi 00 12 34");  //S7F3


//OnPPLoadInquire Example S7F1
//args.SetResponse((int)emCIMRecipeGrant.rgOk);  //S7F2

//OnPPRequest, OnPPSendFile, OnPPSendFileVerify Example
//args.SetResponse((int)emCIMRecipeAck.raAccepted);


//OnFmtPPSend(int connId, string cxvRecipe) Example S7F23 Formatted Process Program Send
//cxvRecipe : "L {A PPID}{A MDLN}{A SOFTREV}{L {L {A }{L {L {A PPNAME1}{A PPVALUE1}}{L {A PPNAME2}{A PPVALUE2}}}}}"
//PPNAME : ValueName, PPVALUE : Value of PPNAME


//OnFmtPPAck(int connId, int nACKC7)  Example S7F24 Formatted Process Program Acknowledge
//nACKC7 :  Accepted = 0, PermissionNotGranted = 1, LengthError = 2, MatrixOverflow = 3, PpidNotFound = 4, ModeUnsupported = 5, WillBePerformed = 6


//OnFmtPPRequest Example S7F25
//args.SetResponse(emCIMMessageResults.mReply);
//args.SetResponseData("L {A PPID}{A MDLN}{A SOFTREV}{L {L {A }{L {L {A PPNAME1}{A PPVALUE1}}{L {A PPNAME2}{A PPVALUE2}}}}}");  //S7F26
//PPID:recipeName, MDLN:ModelName in EPJ, SOFTREV:revision in EPJ, PPNAME:recipe param name, PPVALUE:recipe param value.

//OnFmtPPData(int connId, string cxvRecipe)  Example S7F26 Formatted Process Program Data
//cxvRecipe : "L {A PPID}{A MDLN}{A SOFTREV}{L {L {A }{L {L {A PPNAME1}{A PPVALUE1}}{L {A PPNAME2}{A PPVALUE2}}}}}"
//PPNAME : ValueName, PPVALUE : Value of PPNAME

//TerminalMsgSend
//args.Plugins.CIMConnect.SendTerminalMsg(1, 0, "testMsg!");
