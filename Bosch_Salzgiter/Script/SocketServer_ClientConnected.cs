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
using System.IO;
using MySql.Data.MySqlClient;

public class Script
{
    public static void SocketServer_ClientConnected(KSMART.BRM.Script.SocketServerClientConnectedArgs args)
    {
    		
			if (args.Name == "AOI")
			{
					
			var ini = args.INI.Load(@"C:\KohYoung\KSA\KsaConf.dat");	
			string ip = ini.Read("Config","ServerIP").ToString();		
			string userId = "root";
			string password = "akstnanrkd123@";
			string database = "klib_migration";
			
			string Query = string.Format("SELECT JOB_NM From klib_migration.t_job  Order By ACES_DT DESC limit 1;");
			
			DataTable dt = new DataTable();
			
			string cs = string.Format(@"server={0};userid={1};password={2};database={3}",ip,userId,password,database);
			
			MySqlConnection con = new MySqlConnection(cs);
			con.Open();
			
			MySqlCommand cmd = new MySqlCommand(Query, con);
			
			MySqlDataAdapter MyAdapter = new MySqlDataAdapter();    
			MyAdapter.SelectCommand = cmd;  
			DataTable dTable = new DataTable();  
			MyAdapter.Fill(dTable);  

			string data = string.Empty;

			foreach (DataRow dr in dTable.Rows)
			{
				data = dr["JOB_NM"].ToString();
			}			
			

				
			
				args.Alert.Show("Job file auto loading","Wait a second job file will be load soon",emAlertStatus.Info);
				
				Thread.Sleep(15000);

				args.Alert.Show("Job file auto loading","Preparing the job file",emAlertStatus.Info);
				
				Thread.Sleep(2000);
				
				args.AOI.Machine.JobChange("AOI","0",data,string.Empty,false,false,null,10000);
			}
    }
   
}