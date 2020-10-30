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
using System.Threading.Tasks;
using System.Text;
using System.Diagnostics;
using KSMART.BRM;
using KSMART.BRM.Script;
using System.Xml;

public class Script
{
    public static void Machine_ReadBarcode(KSMART.BRM.Script.AOIMachineReadBarcodeArgs args)
    {
        // Get bad mark data (eMap)
        Machine_GetBadmark(args);
    }

    private static void Machine_GetBadmark(KSMART.BRM.Script.AOIMachineReadBarcodeArgs args)
    {
        // Check if SECS/GEM option is enabled
        if (args.CustomerProperty.GetValue("SECSGEMEnable").ToBoolean() == true)
        {
            string response = string.Empty;
            string error = string.Empty;
            int timeout = 30000;
            args.KeyValue.SetValue("S14F2_Message", string.Empty);
            args.KeyValue.SetValue("S14F2_Error", string.Empty);

            try
            {
                string message = "L {A }{A Substrate}{L {A " + args.PCBBarcode + "}}{L {A SubstrateType}{A Strip}{U1 0}}{L {A MapData}}";

                args.Log.WriteExecute("SECSGEM.S14F1", message, ()=>machine(args.Lane));

                args.Plugins.CIMConnect.SendCustomMessage(1, 14, 1, message, true);

                args.ExecuteTask(
                    ()=>
                    {
                        while (response == string.Empty)
                        {
                            Thread.Sleep(50);

                            response = args.KeyValue.GetValue("S14F2_Message");
                        }
                    }, timeout
                );

                if (response != string.Empty)
                {
                    error = args.KeyValue.GetValue("S14F2_Error");

                    if (error == string.Empty)
                    {
                        args.Log.WriteExecute("SECSGEM.S14F2", response, ()=>machine(args.Lane));

                        try
                        {
                            var name = string.Empty;
                            var mapData = XDocument.Parse(response);

                            if(mapData.Root.Attributes().Count() > 0 && mapData.Root.FirstAttribute.Name == "xmlns")
                            {
                                name = mapData.Root.Attribute("xmlns").Value.ToString();
                            }

                            var ns = new XmlNamespaceManager(new NameTable());
                            ns.AddNamespace("ns", name);

                            var binCodeMap = mapData.XPathSelectElement("ns:MapData/ns:SubstrateMaps/ns:SubstrateMap/ns:Overlay/ns:BinCodeMap", ns);
                            var binDefinition = binCodeMap.XPathSelectElements("ns:BinDefinitions/ns:BinDefinition", ns);
                            var failDefinition = binDefinition.Where(definition => definition.Attribute("BinQuality").Value == "Fail");
                            var failCode = failDefinition.FirstOrDefault().Attribute("BinCode").Value;
                            var binCode = binCodeMap.XPathSelectElements("ns:BinCode", ns);

                            int rows = binCode.Count();
                            int columns = binCode.First().Value.Length / 2;
                            int binCount = rows * columns;
                            var badmark = new List<int>();

                            // Validate mapping information
                            if (binCount == args.PanelCount)
                            {
                                // Retrieve bad mark data
                                for (int row = 1; row <= rows; row++)
                                {
                                    for (int column = 1; column <= columns; column+=2)
                                    {
                                        if (binCode.ElementAt(row - 1).Value.Substring(column - 1, 2) == failCode)
                                        {
                                            badmark.Add(row * column);

                                            args.Debug.Print(string.Format("Badmark: R{0}, C{1}", row, column));
                                        }
                                    }
                                }

                                // Apply bin code mapping
                                args.SetPanelBadMark(badmark);

                                args.SendOK();
                            }
                            else
                            {
                                args.Log.WriteError("SECSGEM.S14F2", string.Format("Array mismatch ({0})", binCount), ()=>machine(args.Lane));

                                args.SendNG(string.Format("SECSGEM: Array mismatch ({0})", binCount));
                            }
                        }
                        catch (Exception ex)
                        {
                            args.Log.WriteError("SECSGEM.S14F2", "Invalid message", ()=>machine(args.Lane));

                            args.SendNG("SECSGEM: Invalid message (S14F2)");
                        }
                    }
                    else
                    {
                        args.Log.WriteError("SECSGEM.S14F2", error, ()=>machine(args.Lane));

                        args.SendNG(string.Format("SECSGEM: {0}", error));
                    }
                }
                else
                {
                    args.Log.WriteError("SECSGEM.S14F2", "Timeout", ()=>machine(args.Lane));

                    args.SendNG("SECSGEM: Timeout (S14F2)");
                }
            }
            catch(Exception ex)
            {
                args.SendException(ex);
            }
        }
        else
        {
            args.Log.WriteWarning("SECSGEM.Status", "Disabled", ()=>machine(args.Lane));

            args.SendOK();
        }
    }

    private static string machine(string lane)
    {
        var parameter = new Dictionary<string, object>();

        parameter.Add("Lane", lane);

        return parameter.ToParametersEquals();
    }
}
