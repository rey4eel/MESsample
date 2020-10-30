//Add Reference
//css_reference System.Core;
//css_reference System.Data;
//css_reference System.Data.DataSetExtensions;
//css_reference System.Xml;
//css_reference System.Xml.Linq;

//Import
//css_import C:\KohYoung\KSMART Business Rules Management\Script\Queue.cs;

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
using System.Timers;
using System.Net;
using QueueClass = Queue;

public static class WebService
{
    public enum Event
    {
        Heartbeat,
        StatusChange,
        InspectStart,
        InspectEnd,
        ReviewStart,
        ReviewEnd
    }

    private static Dictionary<Event, string> EventName = new Dictionary<Event, string>
    {
        { Event.Heartbeat, "Heartbeat" },
        { Event.StatusChange, "StatusChange" },
        { Event.InspectStart, "InspectStart" },
        { Event.InspectEnd, "InspectEnd" },
        { Event.ReviewStart, "ReviewStart" },
        { Event.ReviewEnd, "ReviewEnd" }
    };

    public static class Message
    {
        public static bool Send(KSMART.BRM.Script.ScriptArgs args, string lane, Event eventid, string message = "", bool test = false)
        {
            bool success = true;

            // Get message information
            int id = 0;
            string name = EventName.GetValue(eventid);
            string timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");
            string address = args.CustomerProperty.GetValue("WebServiceAddress").Trim();
            string content = args.WebService.CONTENT_TYPE_JSON;
            string accept = args.WebService.ACCEPT_JSON;

            // Check if web service option is enabled or test message
            if (((args.CustomerProperty.GetValue("WebServiceEvents").Contains(name) == true) &&
                (args.CustomerProperty.GetValue("WebServiceLane").Contains(lane) == true)) ||
                (test == true))
            {
                // Build web service message
                if (message.IsNullEmpty() == true)
                {
                    id = WebService.Message.Id(args);

                    args.Log.WriteExecute("WebService.Event", ()=>log(lane, id, name, timestamp, address));

                    message = WebService.Message.Build(args, lane, eventid);
                }

                args.Log.WriteExecute("WebService.Message", message, ()=>log(lane, id, name, timestamp, address));

                try
                {
                    // Send message to endpoint
                    string result = args.WebService.ExecutePost(address, message, content, accept);

                    args.Log.WriteSuccess("WebService.Status", result, ()=>log(lane, id, name, timestamp, address));
                }
                catch (WebException ex)
                {
                    args.Log.WriteError("WebService.Status", ex.Message, ()=>log(lane, id, name, timestamp, address));

                    if (id > 0)
                    {
                        // Queue message
                        WebService.Queue.Push(args, lane, eventid, message);
                    }

                    success = false;
                }
            }
            else
            {
                args.Log.WriteWarning("WebService.Status", "Disabled", ()=>log(lane, id, name, timestamp, address));
            }

            return success;
        }

        public static void Test(KSMART.BRM.Script.ScriptArgs args)
        {
            var message = string.Empty;
            var param = new Dictionary<string, string>();
            var resultData = new XDocument();

            var result = args.CustomMessageForm.ShowModal(
                "Select machine event.",
                emMessageIcon.Infomation,
                (load)=>
                {
                    load.Form.Text = "Web Service";
                    load.UseButton1 = true;
                    load.Button1Text = "StatusChange";
                    load.UseButton2 = true;
                    load.Button2Text = "InspectStart";
                    load.UseButton3 = true;
                    load.Button3Text = "InspectEnd";
                    load.UseButton4 = true;
                    load.Button4Text = "ReviewStart";
                    load.UseButton5 = true;
                    load.Button5Text = "ReviewEnd";
                }
            );

            var eventid = (Event) result.ToInt();

            if (eventid > 0)
            {
                // Get message information
                int id = 0;
                string lane = "0";
                string name = EventName.GetValue(eventid);
                string timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");
                string address = args.CustomerProperty.GetValue("WebServiceAddress").Trim();

                args.Log.WriteExecute("WebService.Test", ()=>log(lane, id, name, timestamp, address));

                switch (eventid)
                {
                case Event.StatusChange:

                    var statusChangeMessage = args.CustomerProperty.GetValue("SPI_StatusChange");

                    param.Add("Id", Guid.NewGuid().ToString());
                    param.Add("Machine", "KSMART-BRM-MachineID");
                    param.Add("Lane", lane);
                    param.Add("Program", "KSMART-BRM");
                    param.Add("Model", "KSMART-BRM");
                    param.Add("Side", "Top");
                    param.Add("Lot", "LOT001");
                    param.Add("User", "KohYoung");
                    param.Add("StatusCode", "25");
                    param.Add("StatusName", "JobLoad");
                    param.Add("AlarmCode", "1204");
                    param.Add("AlarmName", "The front door is opened");

                    // Build web service message
                    message = args.Xsl.StringTransform(statusChangeMessage, param);

                    break;

                case Event.InspectStart:

                    var inspectStartMessage = args.CustomerProperty.GetValue("SPI_InspectStart");

                    param.Add("Id", Guid.NewGuid().ToString());
                    param.Add("Machine", "KSMART-BRM-MachineID");
                    param.Add("Lane", lane);
                    param.Add("Program", "KSMART-BRM");
                    param.Add("Model", "KSMART-BRM");
                    param.Add("Side", "Top");
                    param.Add("Lot", "LOT001");
                    param.Add("User", "KohYoung");
                    param.Add("PanelCount", "2");
                    param.Add("PCBBarcode", "BOARD001");
                    param.Add("PanelBarcode", "PANEL001,PANEL002");

                    // Build web service message
                    message = args.Xsl.StringTransform(inspectStartMessage, param);

                    break;

                case Event.InspectEnd:

                    var inspectEndMessage = args.CustomerProperty.GetValue("SPI_InspectEnd");

                    resultData = XDocument.Load(@"C:\KohYoung\KSMART Business Rules Management\XML\SPI_Sample.xml");

                    // Build web service message
                    message = args.Xsl.StringTransform(resultData, inspectEndMessage);

                    break;

                case Event.ReviewStart:

                    var reviewStartMessage = args.CustomerProperty.GetValue("SPI_ReviewStart");

                    resultData = XDocument.Load(@"C:\KohYoung\KSMART Business Rules Management\XML\SPI_Sample.xml");

                    // Build web service message
                    message = args.Xsl.StringTransform(resultData, reviewStartMessage);

                    break;

                case Event.ReviewEnd:

                    var reviewEndMessage = args.CustomerProperty.GetValue("SPI_ReviewEnd");

                    resultData = XDocument.Load(@"C:\KohYoung\KSMART Business Rules Management\XML\SPI_Sample.xml");

                    // Build web service message
                    message = args.Xsl.StringTransform(resultData, reviewEndMessage);

                    break;
                }

                // Send test message to endpoint
                if (WebService.Message.Send(args, lane, eventid, message, true) == true)
                {
                    args.MessageBox.Show(
                        "Test message successfully sent.",
                        "Web Service",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
                else
                {
                    args.MessageBox.Show(
                        "Failed to send test message.",
                        "Web Service",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }

        private static int Id(KSMART.BRM.Script.ScriptArgs args)
        {
            int id = (args.KeyValue.GetValue("Id").ToInt() % 999) + 1;

            args.KeyValue.SetValue("Id", id.ToString());

            return id;
        }

        private static string Build(KSMART.BRM.Script.ScriptArgs args, string lane, Event eventid)
        {
            var message = string.Empty;
            var param = new Dictionary<string, string>();
            var resultData = new XDocument();
            var ini = args.INI.Load(@"C:\KohYoung\KY-3030\KY3030.ini");
            var machineName = ini.Read("HW", "MACHINE_NAME");

            switch (eventid)
            {
                case Event.Heartbeat:

                var heartbeatMessage = args.CustomerProperty.GetValue("SPI_Heartbeat");

                param.Add("Id", Guid.NewGuid().ToString());
                param.Add("Machine", machineName);
                param.Add("Lane", lane);
                param.Add("Program", args.SPI.Machine.Current.Job("SPI", lane).PCBName);
                param.Add("Model", args.SPI.Machine.Current.Job("SPI", lane).PCBName);
                param.Add("Side", args.SPI.Machine.Current.Job("SPI", lane).Side);
                param.Add("Lot", args.SPI.Machine.Current.LotName("SPI", lane));
                param.Add("User", args.SPI.Machine.Current.UserID("SPI"));
                param.Add("StatusCode", args.SPI.Machine.Current.MachineStatusCode("SPI"));
                param.Add("StatusName", args.SPI.Machine.Current.MachineStatusName("SPI"));
                param.Add("AlarmCode", args.SPI.Machine.Current.MachineAlarmCode("SPI"));
                param.Add("AlarmName", args.SPI.Machine.Current.MachineAlarmText("SPI"));

                // Build web service message
                message = args.Xsl.StringTransform(heartbeatMessage, param);

                break;

            case Event.StatusChange:

                var statusChangeArgs = (KSMART.BRM.Script.SPIMachineStatusChangedArgs) args;
                var statusChangeMessage = args.CustomerProperty.GetValue("SPI_StatusChange");

                param.Add("Id", Guid.NewGuid().ToString());
                param.Add("Machine", machineName);
                param.Add("Lane", lane);
                param.Add("Program", args.SPI.Machine.Current.Job("SPI", lane).PCBName);
                param.Add("Model", args.SPI.Machine.Current.Job("SPI", lane).PCBName);
                param.Add("Side", args.SPI.Machine.Current.Job("SPI", lane).Side);
                param.Add("Lot", args.SPI.Machine.Current.LotName("SPI", lane));
                param.Add("User", args.SPI.Machine.Current.UserID("SPI"));
                param.Add("StatusCode", statusChangeArgs.StatusCode);
                param.Add("StatusName", statusChangeArgs.StatusName);
                param.Add("AlarmCode", statusChangeArgs.ErrorCode);
                param.Add("AlarmName", statusChangeArgs.ErrorText);

                // Build web service message
                message = args.Xsl.StringTransform(statusChangeMessage, param);

                break;

            case Event.InspectStart:

                var inspectStartArgs = (KSMART.BRM.Script.SPIMachineReadBarcodeArgs) args;
                var inspectStartMessage = args.CustomerProperty.GetValue("SPI_InspectStart");

                param.Add("Id", Guid.NewGuid().ToString());
                param.Add("Machine", machineName);
                param.Add("Lane", lane);
                param.Add("Program", args.SPI.Machine.Current.JobFileName("SPI", lane));
                param.Add("Model", args.SPI.Machine.Current.Job("SPI", lane).PCBName);
                param.Add("Side", args.SPI.Machine.Current.Job("SPI", lane).Side);
                param.Add("Lot", args.SPI.Machine.Current.LotName("SPI", lane));
                param.Add("User", args.SPI.Machine.Current.UserID("SPI"));
                param.Add("PanelCount", inspectStartArgs.PanelCount.ToString());
                param.Add("PCBBarcode", inspectStartArgs.PCBBarcode);
                param.Add("PanelBarcode", inspectStartArgs.PanelBarcode);

                // Build web service message
                message = args.Xsl.StringTransform(inspectStartMessage, param);

                break;

            case Event.InspectEnd:

                var inspectEndArgs = (KSMART.BRM.Script.SPIMachineInspectResultCompletedArgs) args;
                var inspectEndMessage = args.CustomerProperty.GetValue("SPI_InspectEnd");

                // Build web service message
                message = args.Xsl.StringTransform(inspectEndArgs.ResultData, inspectEndMessage);

                break;

            case Event.ReviewStart:

                var reviewStartArgs = (KSMART.BRM.Script.SPIReviewBoardLoadArgs) args;
                var reviewStartMessage = args.CustomerProperty.GetValue("SPI_ReviewStart");

                resultData = args.SPI.GetInspectResultData(
                    reviewStartArgs.Lane,
                    reviewStartArgs.Queue.ToString(),
                    false,
                    emSPIVerificationType.DefectView,
                    string.Empty,
                    -1
                ).ToXDocument();

                // Build web service message
                message = args.Xsl.StringTransform(resultData, reviewStartMessage);

                break;

            case Event.ReviewEnd:

                var reviewEndArgs = (KSMART.BRM.Script.SPIReviewCompletedArgs) args;
                var reviewEndMessage = args.CustomerProperty.GetValue("SPI_ReviewEnd");

                // Build web service message
                message = args.Xsl.StringTransform(reviewEndArgs.ResultData, reviewEndMessage);

                break;
            }

            return message;
        }
    }

    public static class Heartbeat
    {
        private const int HEARTBEAT_INTERVAL_1SEC = 1000;

        public static void Init(KSMART.BRM.Script.ScriptArgs args)
        {
            if (args.Memory.IsObject("HeartbeatTimer") == false)
            {
                var HeartbeatTimer = new System.Timers.Timer();

                args.Memory.SetObject("HeartbeatTimer", HeartbeatTimer);

                HeartbeatTimer.Elapsed += WebService.Heartbeat.Handler;
                HeartbeatTimer.Interval = HEARTBEAT_INTERVAL_1SEC;
                HeartbeatTimer.AutoReset = true;
                HeartbeatTimer.Enabled = true;
            }
        }

        public static void DeInit(KSMART.BRM.Script.ScriptArgs args)
        {
            if (args.Memory.IsObject("HeartbeatTimer") == true)
            {
                var HeartbeatTimer = (System.Timers.Timer) args.Memory.GetObject("HeartbeatTimer");

                HeartbeatTimer.Dispose();
            }
        }

        private static void Handler(object sender, ElapsedEventArgs e)
        {
            var args = new KSMART.BRM.Script.ScriptArgs();
            var timer = (System.Timers.Timer) sender;
            var ini = args.INI.Load(@"C:\KohYoung\KY-3030\KY3030.ini");
            int laneCount = ini.Read("HW", "LANE_COUNT").ToInt();
            int interval = args.CustomerProperty.GetValue("HeartbeatInterval").ToInt();

            timer.Enabled = false;

            if (args.CustomerProperty.GetValue("HeartbeatUse").ToBoolean() == true)
            {
                if (interval > 0)
                {
                    timer.Interval = interval * 1000;

                    for (int lane = 0; lane < laneCount; lane++)
                    {
                        // Send heartbeat message
                        WebService.Message.Send(args, lane.ToString(), WebService.Event.Heartbeat);
                    }
                }
            }

            timer.Enabled = true;
        }
    }

    public static class Queue
    {
        private const int QUEUE_INTERVAL_MIN = 100;
        private const int QUEUE_INTERVAL_MAX = 60000;

        public static void Init(KSMART.BRM.Script.ScriptArgs args)
        {
            var columns = new List<string>();

            columns.Add("Lane");
            columns.Add("Event");
            columns.Add("Message");

            // Create message queue
            QueueClass.Create(args, columns);

            if (args.Memory.IsObject("QueueTimer") == false)
            {
                var QueueTimer = new System.Timers.Timer();

                args.Memory.SetObject("QueueTimer", QueueTimer);

                QueueTimer.Elapsed += WebService.Queue.Handler;
                QueueTimer.Interval = QUEUE_INTERVAL_MIN;
                QueueTimer.AutoReset = true;
                QueueTimer.Enabled = true;
            }
        }

        public static void DeInit(KSMART.BRM.Script.ScriptArgs args)
        {
            if (args.Memory.IsObject("QueueTimer") == true)
            {
                var QueueTimer = (System.Timers.Timer) args.Memory.GetObject("QueueTimer");

                QueueTimer.Dispose();
            }
        }

        public static void Push(KSMART.BRM.Script.ScriptArgs args, string lane, Event eventid, string message)
        {
            // Check if queue option is enabled
            if (args.CustomerProperty.GetValue("QueueUse").ToBoolean() == true)
            {
                var data = new Dictionary<string, object>();

                data.Add("Lane", lane);
                data.Add("Event", eventid.ToInt());
                data.Add("Message", message);

                // Queue message
                QueueClass.Push(args, data);

                // Update message count
                args.CustomerProperty.SetValue("QueuePending", QueueClass.Count(args).ToString());
            }
        }

        public static void Clear(KSMART.BRM.Script.ScriptArgs args)
        {
            if (QueueClass.Count(args) > 0)
            {
                var result = args.MessageBox.Show(
                    "Do you want to delete all pending messages?",
                    "Web Service",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (result == DialogResult.Yes)
                {
                    // Clear message queue
                    if (QueueClass.Clear(args) == true)
                    {
                        args.MessageBox.Show(
                            "Messages successfully deleted.",
                            "Web Service",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                    }
                    else
                    {
                        args.MessageBox.Show(
                            "Failed to delete messages. Please try again.",
                            "Web Service",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }

                    // Update message count
                    args.CustomerProperty.SetValue("QueuePending", QueueClass.Count(args).ToString());
                }
            }
        }

        private static void Handler(object sender, ElapsedEventArgs e)
        {
            var args = new KSMART.BRM.Script.ScriptArgs();
            var timer = (System.Timers.Timer) sender;

            timer.Enabled = false;

            var data = new Dictionary<string, object>();

            if (QueueClass.Peek(args, ref data) == true)
            {
                // Get message information
                var lane = data.GetValue("Lane").ToString();
                var eventid = (Event) data.GetValue("Event").ToInt();
                var message = data.GetValue("Message").ToString();

                // Send machine event
                if (WebService.Message.Send(args, lane, eventid, message) == true)
                {
                    timer.Interval = QUEUE_INTERVAL_MIN;

                    // Delete message
                    QueueClass.Delete(args);
                }
                else
                {
                    timer.Interval = QUEUE_INTERVAL_MAX;

                    int retention = args.CustomerProperty.GetValue("QueueRetention").ToInt();

                    if (retention > 0)
                    {
                        // Delete old messages
                        QueueClass.Delete(args, string.Format("DateTime < datetime('now', '-{0} days', 'localtime')", retention));
                    }
                }

                // Update message count
                args.CustomerProperty.SetValue("QueuePending", QueueClass.Count(args).ToString());
            }

            timer.Enabled = true;
        }
    }

    private static string log(string lane, int id, string name, string timestamp, string address)
    {
        var parameter = new Dictionary<string, object>();

        parameter.Add("Lane", lane);
        parameter.Add("Id", id);
        parameter.Add("Name", name);
        parameter.Add("Timestamp", timestamp);
        parameter.Add("Address", address);

        return parameter.ToParametersEquals();
    }
}
