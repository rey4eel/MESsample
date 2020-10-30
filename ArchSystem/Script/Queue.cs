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
using KSMART.BRM;
using KSMART.BRM.Core;
using System.IO;

public class Queue
{
    public static bool Create(KSMART.BRM.Script.ScriptArgs args, List<string> columns, string table = "Queue")
    {
        bool success = false;

        try
        {
            string database = Path.Combine(args.Info.StartupPath, "Queue.db");

            var format = new Dictionary<string, string>();

            format.Add("GUID", "TEXT");
            format.Add("DateTime", "DATETIME");

            foreach (var column in columns)
            {
                format.Add(column, "TEXT");
            }

            if (File.Exists(database) == true)
            {
                var result = args.SQLite.Select(database, string.Format("PRAGMA table_info({0})", table));

                var names = result.AsEnumerable().Select(col => col.Field<string>("name")).ToList();

                if (format.Keys.ToList().SequenceEqual(names) == false)
                {
                    File.Delete(database);
                }
            }

            args.SQLite.CreateTable(database, table, format);

            args.Log.WriteSuccess("Queue.Create", ()=>log(database, table));

            success = true;
        }
        catch (Exception ex)
        {
            args.Log.WriteException("Queue.Create", ex);
        }

        return success;
    }

    public static bool Clear(KSMART.BRM.Script.ScriptArgs args, string table = "Queue")
    {
        bool success = false;

        try
        {
            string database = Path.Combine(args.Info.StartupPath, "Queue.db");

            args.SQLite.Delete(database, table);

            args.Log.WriteSuccess("Queue.Clear", ()=>log(database, table));

            success = true;
        }
        catch (Exception ex)
        {
            args.Log.WriteException("Queue.Clear", ex);
        }

        return success;
    }

    public static bool Delete(KSMART.BRM.Script.ScriptArgs args, string condition = "DateTime IN (SELECT DateTime FROM {0} ORDER BY DateTime LIMIT 1)", string table = "Queue")
    {
        bool success = false;

        try
        {
            string database = Path.Combine(args.Info.StartupPath, "Queue.db");

            args.SQLite.Delete(database, table, string.Format(condition, table), null);

            args.Log.WriteSuccess("Queue.Delete", ()=>log(database, table));

            success = true;
        }
        catch (Exception ex)
        {
            args.Log.WriteException("Queue.Delete", ex);
        }

        return success;
    }

    public static bool Peek(KSMART.BRM.Script.ScriptArgs args, ref Dictionary<string, object> data, string condition = "ORDER BY DateTime LIMIT 1", string table = "Queue")
    {
        bool success = false;

        try
        {
            string database = Path.Combine(args.Info.StartupPath, "Queue.db");

            var result = args.SQLite.Select(database, string.Format("SELECT * FROM {0} {1}", table, condition));

            if (result.Rows.Count > 0)
            {
                var row = new Dictionary<string, object>();

                foreach (DataColumn column in result.Columns)
                {
                    row.Add(column.ColumnName, result.Rows[0][column]);
                }

                data = row;

                args.Log.WriteSuccess("Queue.Peek", ()=>log(database, table));

                success = true;
            }
        }
        catch (Exception ex)
        {
            args.Log.WriteException("Queue.Peek", ex);
        }

        return success;
    }

    public static bool Pop(KSMART.BRM.Script.ScriptArgs args, ref Dictionary<string, object> data, string condition = "ORDER BY DateTime LIMIT 1", string table = "Queue")
    {
        bool success = false;

        try
        {
            string database = Path.Combine(args.Info.StartupPath, "Queue.db");

            var result = args.SQLite.Select(database, string.Format("SELECT * FROM {0} {1}", table, condition));

            if (result.Rows.Count > 0)
            {
                var row = new Dictionary<string, object>();

                foreach (DataColumn column in result.Columns)
                {
                    row.Add(column.ColumnName, result.Rows[0][column]);
                }

                data = row;

                args.SQLite.Delete(database, table, "GUID=@GUID", row);

                args.Log.WriteSuccess("Queue.Pop", ()=>log(database, table));

                success = true;
            }
        }
        catch (Exception ex)
        {
            args.Log.WriteException("Queue.Pop", ex);
        }

        return success;
    }

    public static bool Push(KSMART.BRM.Script.ScriptArgs args, Dictionary<string, object> data, string table = "Queue")
    {
        bool success = false;

        try
        {
            string database = Path.Combine(args.Info.StartupPath, "Queue.db");

            var row = new Dictionary<string, object>();

            row.Add("GUID", Guid.NewGuid().ToString());
            row.Add("DateTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

            foreach (var item in data)
            {
                row.Add(item.Key, item.Value);
            }

            args.SQLite.Insert(database, table, row);

            args.Log.WriteSuccess("Queue.Push", ()=>log(database, table));

            success = true;
        }
        catch (Exception ex)
        {
            args.Log.WriteException("Queue.Push", ex);
        }

        return success;
    }

    public static int Count(KSMART.BRM.Script.ScriptArgs args, string condition = "", string table = "Queue")
    {
        int count = 0;

        try
        {
            string database = Path.Combine(args.Info.StartupPath, "Queue.db");

            var result = args.SQLite.Select(database, string.Format("SELECT COUNT(*) FROM {0} {1}", table, condition));

            count = result.Rows[0]["COUNT(*)"].ToInt();
        }
        catch (Exception ex)
        {
            args.Log.WriteException("Queue.Count", ex);
        }

        return count;
    }

    private static string log(string filename, string table)
    {
        var parameter = new Dictionary<string, object>();

        parameter.Add("FileName", filename);
        parameter.Add("Table", table);

        return parameter.ToParametersEquals();
    }
}
