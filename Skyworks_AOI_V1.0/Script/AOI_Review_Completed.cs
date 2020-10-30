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
using System.Data.SQLite;
using System.IO;

public class Script
{
    private  static SQLiteConnection conn =  null;

    public static void Review_Completed(KSMART.BRM.Script.AOIReviewCompletedArgs args)
    {
        // Export heat map data
        Review_HeatMap(args);

        // Unlock review judgment
        Review_AutoClassfication(args);
    }

    private static void Review_HeatMap(KSMART.BRM.Script.AOIReviewCompletedArgs args)
    {
        SQLiteConnection conn =  null;

        try
        {
            if(args.CustomerProperty.GetValue("bUseHeatMap")=="0")
            {
                args.SendOK();
                return;
            }

                //args.Log.WriteExecute("Start Time",DateTime.Now.ToString("yyyyMMdd hhmmss"));
            args.Debug.Print(DateTime.Now.ToString("yyyyMMdd hhmmss"));
            string strJobList = @"\JobList.db";
            string strQuery = string.Empty;
            string strJobName = args.ResultData.Tables["PCB"].Rows[0]["JobFileName"].ToString().ToUpper();
            string strJobPath = args.ResultData.Tables["PCB"].Rows[0]["JobFileIDLocal"].ToString();
            DateTime dResultTime = args.ResultData.Tables["PCB"].Rows[0]["EndDateTime"].ToDateTime();  //검사 결과 시간  등록 EndDateTime
            string strEndUnixTime = ConvertToUnixTimestamp(dResultTime).ToString();
            args.Log.Write("EndTIME", dResultTime.ToString("yyyyMMdd hhmmss"));
            args.Log.Write("UNIXTIME", strEndUnixTime);
            string sRow = "0";
            string sCol = "0"; // row col 값이 필요함
            int nResult = 0;

            var Massge = args.Xsl.StringTransform(args.ResultData, args.CustomerProperty.GetValue("ResultData"));
            var messageItems = Massge.Split(System.Environment.NewLine.ToCharArray()).ToList();
            messageItems.RemoveAll(r => r.IsNullEmpty());

            /*
            36
            1548
            253
            0
            kyadmin,790,bacode,lot,2,253,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
            1,2,43,7,36,7,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
            2,2,43,7,36,7,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
            3,2,43,7,36,7,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
            4,2,43,7,36,7,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
            5,2,43,7,36,7,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
            6,2,43,7,36,7,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
            7,2,43,7,36,7,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
            8,2,43,7,36,7,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
            9,2,43,7,36,7,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
            10,2,43,7,36,7,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
            11,2,43,7,36,7,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
            12,2,43,7,36,7,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
            13,2,43,7,36,7,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
            14,2,43,7,36,7,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
            15,2,43,7,36,7,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
            16,2,43,7,36,7,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
            17,2,43,7,36,7,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
            18,2,43,7,36,7,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
            19,2,43,7,36,7,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
            20,2,43,7,36,7,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
            21,2,43,7,36,7,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
            22,2,43,7,36,7,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
            23,2,43,7,36,7,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
            24,2,43,7,36,7,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
            25,2,43,8,35,8,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
            26,2,43,7,36,7,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
            27,2,43,7,36,7,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
            28,2,43,7,36,7,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
            29,2,43,7,36,7,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
            30,2,43,7,36,7,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
            31,2,43,7,36,7,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
            32,2,43,7,36,7,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
            33,2,43,7,36,7,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
            34,2,43,7,36,7,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
            35,2,43,7,36,7,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
            36,2,43,7,36,7,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
            */

            int iPanelCnt = 0;
            int iPCBCompCnt = 0;
            int iPCBCompNGCnt = 0;
            int iPCBCompPASSCnt = 0;
            string strPCBResult = string.Empty;
            List<string> strPanelResult = new List<string>();

            for(int i = 0 ; i < messageItems.Count ;i++)
                {
                    switch(i)
                {
                    case 0 :
                        iPanelCnt = messageItems[i].ToInt();
                        break;
                    case 1 :
                        iPCBCompCnt = messageItems[i].ToInt();
                        break;
                    case 2 :
                        iPCBCompNGCnt = messageItems[i].ToInt();
                        break;
                    case 3 :
                        iPCBCompPASSCnt =    messageItems[i].ToInt();
                        break;
                    case 4 :
                        strPCBResult = messageItems[i].ToString();
                        break;
                    default :
                        strPanelResult.Add( messageItems[i].ToString());

                    break;
                }
            }

            string[] strPCBDefect = strPCBResult.Split(',');

            SQLiteCommand cmdSQL =  null;
            SQLiteDataReader rdr = null;

            DirectoryInfo di = new DirectoryInfo(args.CustomerProperty.GetValue("strDBPath"));
            if (di.Exists == false)
            {
                di.Create();
            }

            //JobList Start

            conn = new SQLiteConnection(string.Format("Data Source={0}",args.CustomerProperty.GetValue("strDBPath")+strJobList));

            FileInfo fi = new FileInfo(args.CustomerProperty.GetValue("strDBPath")+strJobList);
            if(fi.Exists){
                conn.Open();
                args.Debug.Print("OPEN");
            }else{
                conn.Open();
                args.Debug.Print("CREATE");
                CreateDBTable(1, conn, args);
            }

            strQuery =  "select JobUniqueID from JobList where Name='" + strJobName + "'";
            cmdSQL = new SQLiteCommand(strQuery,conn);
            rdr = cmdSQL.ExecuteReader();

            string strRtn = string.Empty;
            while (rdr.Read()) {
              strRtn = rdr["JobUniqueID"].ToString();
              break;
            }

            args.Debug.Print(strRtn);

            //JobList Insert Data
            if(strRtn=="")
            {
                strQuery = "INSERT INTO JobList (" +
                            "Name, FullPath, BoardImagePath, LineID, MachineID, EndDateTime, ROW, COL) " +
                            "VALUES(" +
                            "'" + strJobName + "'," +
                            "'" + strJobPath + "'," +
                             "'" + strJobPath + "WholeBoard.jpg', " +
                            "'" + args.CustomerProperty.GetValue("LINE_ID").ToString() + "', " +
                            "'" + SystemInformation.ComputerName.ToString() + "', " +
                            strEndUnixTime + ", " +
                           sRow + ", " +
                           sCol+ ");";
                  args.Debug.Print(strQuery);
            }
            else
            {
                 strQuery = "UPDATE JobList SET EndDateTime=" + strEndUnixTime +
                        ",Name='" + strJobName + "'" +
                        ",FullPath='" + strJobPath + "'" +
                        ",BoardImagePath='"  + strJobPath +  "WholeBoard.jpg'"+
                        ",LineID='" +args.CustomerProperty.GetValue("LINE_ID").ToString()  + "'" +
                        ",MachineID='" + SystemInformation.ComputerName.ToString() + "'" +
                        ",EndDateTime=" + strEndUnixTime +
                        ",ROW=" +  sRow  +
                        ",COL=" + sCol +
                        " WHERE JobUniqueID ='" + strRtn + "'";
                args.Debug.Print(strQuery);
            }

            cmdSQL = new SQLiteCommand(strQuery,conn);
            nResult = cmdSQL.ExecuteNonQuery();
            args.Log.WriteSuccess("Insert JobList Table",strQuery);


            strQuery =  "select JobUniqueID from JobList where Name='" + strJobName + "'";
            cmdSQL = new SQLiteCommand(strQuery,conn);
            rdr = cmdSQL.ExecuteReader();

            strRtn = string.Empty; //JobUniqueID
            while (rdr.Read()) {
              strRtn = rdr["JobUniqueID"].ToString();
              break;
            }

            conn.Close();
            //JobList End

            //Panel_N Start
            di = new DirectoryInfo(args.CustomerProperty.GetValue("strDBPath")+@"\"+ strJobName  );
            if (di.Exists == false)
            {
                di.Create();
            }
            string strPanelDBName = string.Format(@"{0}\{1}\{2}_Panel_{3}.db",args.CustomerProperty.GetValue("strDBPath"),strJobName,strJobName,"1");
            conn = new SQLiteConnection(string.Format("Data Source={0}",strPanelDBName));
            double dFileSize = 0;
            int iCount = 1;
            bool bCheck = false;

            fi = new FileInfo(strPanelDBName);
            if(fi.Exists){

                foreach(FileInfo fInfo in  di.GetFiles())
                {

                    args.Debug.Print(fInfo.Length.ToString());
                    args.Debug.Print(SetMBytes(fInfo.Length).ToString());
                    args.Debug.Print(Path.GetFileNameWithoutExtension(fInfo.FullName).Substring(fInfo.Name.IndexOf("Panel_")+6));



                    if(fInfo.Name.IndexOf(string.Format("{0}_Panel_",strJobName)) > -1)
                    {
                        dFileSize =SetMBytes(fInfo.Length);

                        if(dFileSize <  args.CustomerProperty.GetValue("FILE_LIMIT").ToDouble())
                        {
                            iCount = Path.GetFileNameWithoutExtension(fInfo.FullName).Substring(fInfo.Name.IndexOf("Panel_")+6).ToInt();
                            args.Debug.Print("OPEN_"+ iCount.ToString() );
                            bCheck = true;
                            break;
                        }else{
                            if(iCount <= Path.GetFileNameWithoutExtension(fInfo.FullName).Substring(fInfo.Name.IndexOf("Panel_")+6).ToInt())
                            {
                                iCount =  Path.GetFileNameWithoutExtension(fInfo.FullName).Substring(fInfo.Name.IndexOf("Panel_")+6).ToInt() + 1;
                            }
                             strPanelDBName = string.Format(@"{0}\{1}\{2}_Panel_{3}.db",args.CustomerProperty.GetValue("strDBPath"),strJobName,strJobName, iCount.ToString());
                             conn = new SQLiteConnection(string.Format("Data Source={0}",strPanelDBName));
                        }
                    }
                }

                conn.Open();

                if(!bCheck)
                {
                    args.Log.Write("strPanelDBName1",strPanelDBName);
                    args.Debug.Print("CREATE");
                    CreateDBTable(3, conn, args);
                }

            }else{
                conn.Open();
                args.Log.Write("strPanelDBName2",strPanelDBName);
                args.Debug.Print("CREATE");
                CreateDBTable(3, conn, args);
            }
            //Panel_N Insert Data
            string[] strPanelDefect = null;

            using(SQLiteTransaction tr = conn.BeginTransaction())
            {
                using(SQLiteCommand cmd = conn.CreateCommand())
                {

                    foreach(string strPanelData in strPanelResult)
                    {
                        strPanelDefect = strPanelData.Split(',');

                        strQuery = "INSERT INTO ResultPanel (" +
                                "EndDateTime, JobUniqueID, PanelID, PanelResult, TotalComponentCount, NGComponentCount, PASSComponentCount" +
                                    ", Defect1NGCount, Defect2NGCount, Defect3NGCount, Defect4NGCount, Defect5NGCount, Defect6NGCount, Defect7NGCount, Defect8NGCount, Defect9NGCount, Defect10NGCount" +
                                    ", Defect11NGCount, Defect12NGCount, Defect13NGCount, Defect14NGCount, Defect15NGCount, Defect16NGCount, Defect17NGCount, Defect18NGCount, Defect19NGCount, Defect20NGCount" +
                                    ", Defect21NGCount, Defect22NGCount, Defect23NGCount, Defect24NGCount"+
                                    ", Defect1PASSCount, Defect2PASSCount, Defect3PASSCount, Defect4PASSCount, Defect5PASSCount, Defect6PASSCount, Defect7PASSCount, Defect8PASSCount, Defect9PASSCount, Defect10PASSCount" +
                                    ", Defect11PASSCount, Defect12PASSCount, Defect13PASSCount, Defect14PASSCount, Defect15PASSCount, Defect16PASSCount, Defect17PASSCount, Defect18PASSCount, Defect19PASSCount, Defect20PASSCount" +
                                    ", Defect21PASSCount, Defect22PASSCount, Defect23PASSCount, Defect24PASSCount)" +
                                    " VALUES(" +
                                     strEndUnixTime + "," +
                                     strRtn + "," +
                                    strPanelDefect[0] + "," +
                                    strPanelDefect[1] + "," +
                                    strPanelDefect[2] + "," +
                                    strPanelDefect[3] + "," +
                                    strPanelDefect[4]+ "," +
                                    strPanelDefect[5] + ", " + strPanelDefect[6] + ", " + strPanelDefect[7]+ ", " + strPanelDefect[8]+ ", " + strPanelDefect[9] + ", " + strPanelDefect[10] + ", " + strPanelDefect[11] + ", " + strPanelDefect[12] + ", " + strPanelDefect[13] + ", " + strPanelDefect[14] + ", " +
                                    strPanelDefect[15] + ", " + strPanelDefect[16] + ", " + strPanelDefect[17]+ ", " + strPanelDefect[18]+ ", " + strPanelDefect[19] + ", " + strPanelDefect[20] + ", " + strPanelDefect[21] + ", " + strPanelDefect[22] + ", " + strPanelDefect[23] + ", " + strPanelDefect[24] + ", " +
                                    strPanelDefect[25] + ", " + strPanelDefect[26] + ", " + strPanelDefect[27]+ ", " + strPanelDefect[28]+ ", " +
                                strPanelDefect[29] + ", " + strPanelDefect[30] + ", " + strPanelDefect[31]+ ", " + strPanelDefect[32]+ ", " + strPanelDefect[33] + ", " + strPanelDefect[34] + ", " + strPanelDefect[35] + ", " + strPanelDefect[36] + ", " + strPanelDefect[37] + ", " + strPanelDefect[38] + ", " +
                                    strPanelDefect[39] + ", " + strPanelDefect[40] + ", " + strPanelDefect[41]+ ", " + strPanelDefect[42]+ ", " + strPanelDefect[43] + ", " + strPanelDefect[44] + ", " + strPanelDefect[45] + ", " + strPanelDefect[46] + ", " + strPanelDefect[47] + ", " + strPanelDefect[48] + ", " +
                                strPanelDefect[49] + ", " + strPanelDefect[50] + ", " + strPanelDefect[51]+ ", " + strPanelDefect[52] + ");";

                        //cmdSQL = new SQLiteCommand(strQuery,conn);
                        //nResult = cmdSQL.ExecuteNonQuery();
                        cmd.CommandText = strQuery;
                        cmd.ExecuteNonQuery();
                    }
                }
                tr.Commit();
            }

            conn.Close();

            args.Log.WriteSuccess("Insert ResultPanel Table");
            //Panel_N End

            //JOBNAME Start
            string strJobDBName = string.Format(@"{0}\{1}\{2}.db",args.CustomerProperty.GetValue("strDBPath"),strJobName,strJobName);
            conn = new SQLiteConnection(string.Format("Data Source={0}",strJobDBName));

            fi = new FileInfo(strJobDBName);
            if(fi.Exists){
                conn.Open();
                args.Debug.Print("OPEN");
            }else{
                conn.Open();
                args.Debug.Print("CREATE");
                CreateDBTable(2, conn, args);
            }

            strQuery = "INSERT INTO ResultPCB (" +
                           "EndDateTime, JobUniqueID, PanelDBID, USERID, PCBID, Barcode, LOTID, Barcode_CRC, LOTID_CRC, PCBResult, TotalPanelCount, TotalComponentCount, NGComponentCount, PassComponentCount" +
                           ", Defect1NGCount, Defect2NGCount, Defect3NGCount, Defect4NGCount, Defect5NGCount, Defect6NGCount, Defect7NGCount, Defect8NGCount, Defect9NGCount, Defect10NGCount" +
                           ", Defect11NGCount, Defect12NGCount, Defect13NGCount, Defect14NGCount, Defect15NGCount, Defect16NGCount, Defect17NGCount, Defect18NGCount, Defect19NGCount, Defect20NGCount" +
                           ", Defect21NGCount, Defect22NGCount, Defect23NGCount, Defect24NGCount" +
                           ", Defect1PASSCount, Defect2PASSCount, Defect3PASSCount, Defect4PASSCount, Defect5PASSCount, Defect6PASSCount, Defect7PASSCount, Defect8PASSCount, Defect9PASSCount, Defect10PASSCount" +
                           ", Defect11PASSCount, Defect12PASSCount, Defect13PASSCount, Defect14PASSCount, Defect15PASSCount, Defect16PASSCount, Defect17PASSCount, Defect18PASSCount, Defect19PASSCount, Defect20PASSCount" +
                      ", Defect21PASSCount, Defect22PASSCount, Defect23PASSCount, Defect24PASSCount) " +
                          " VALUES(" +
                           strEndUnixTime + "," +
                           strRtn + "," +
                           iCount.ToString() + "," +
                           "'" +strPCBDefect[0] + "', " +
                           strPCBDefect[1]  + ", " +
                           "'" +  strPCBDefect[2] + "', " +
                           "'" +  strPCBDefect[3] + "', " +
                           "''" + ", " +
                           "''" + ", " +
                           strPCBDefect[4] + ", " +
                           iPanelCnt.ToString() + ", " +
                           iPCBCompCnt.ToString() + ", " +
                           iPCBCompNGCnt.ToString() + ", " +
                           iPCBCompPASSCnt.ToString() + ", " +
                           strPCBDefect[5]+ ", " + strPCBDefect[6] + ", " + strPCBDefect[7] + ", " + strPCBDefect[8] + ", " + strPCBDefect[9] + ", " + strPCBDefect[10] + ", " + strPCBDefect[11] + ", " + strPCBDefect[12] + ", " + strPCBDefect[13] + ", " + strPCBDefect[14] + ", " +
                           strPCBDefect[15]+ ", " + strPCBDefect[16] + ", " + strPCBDefect[17] + ", " + strPCBDefect[18] + ", " + strPCBDefect[19] + ", " + strPCBDefect[20] + ", " + strPCBDefect[21] + ", " + strPCBDefect[22] + ", " + strPCBDefect[23] + ", " + strPCBDefect[24] + ", " +
                           strPCBDefect[25]+ ", " + strPCBDefect[26] + ", " + strPCBDefect[27] + ", " + strPCBDefect[28] + ", " +
                           strPCBDefect[29]+ ", " + strPCBDefect[30] + ", " + strPCBDefect[31] + ", " + strPCBDefect[32] + ", " + strPCBDefect[33] + ", " + strPCBDefect[34] + ", " + strPCBDefect[35] + ", " + strPCBDefect[36] + ", " + strPCBDefect[37] + ", " + strPCBDefect[38] + ", " +
                           strPCBDefect[39]+ ", " + strPCBDefect[40] + ", " + strPCBDefect[41] + ", " + strPCBDefect[42] + ", " + strPCBDefect[43] + ", " + strPCBDefect[44] + ", " + strPCBDefect[45] + ", " + strPCBDefect[46] + ", " + strPCBDefect[47] + ", " + strPCBDefect[48] + ", " +
                           strPCBDefect[49]+ ", " + strPCBDefect[50] + ", " + strPCBDefect[51] + ", " + strPCBDefect[52] + ");";

            cmdSQL = new SQLiteCommand(strQuery,conn);
            nResult = cmdSQL.ExecuteNonQuery();
            args.Log.WriteExecute("Insert ResultPCB Table",strQuery);

            conn.Close();

            args.SendOK();
        }
        catch (Exception ex)
        {
            args.Debug.Print(ex.Message);
            conn.Close();
            args.SendException(ex);
        }
    }

    public static void CreateDBTable(int iType, SQLiteConnection conn ,KSMART.BRM.Script.AOIReviewCompletedArgs args)
    {
        string strQuery = string.Empty;

        switch (iType)
        {
            case 1:
                  strQuery = "CREATE TABLE JobList(" +
                    "JobUniqueID INTEGER PRIMARY KEY AUTOINCREMENT," +
                    "Name VARCHAR(50) NOT NULL," +
                    "FullPath VARCHAR(255) NOT NULL," +
                    "BoardImagePath VARCHAR(50) NOT NULL," +
                    "LineID VARCHAR(10) NOT NULL," +
                    "MachineID VARCHAR(20) NOT NULL, " +
                    "EndDateTime INT NOT NULL," +
                    "ROW INT," +
                    "COL INT);";
            break;
            case 2:
                 strQuery = "CREATE TABLE ResultPCB(" +
                    "EndDateTime INT NOT NULL," +
                    "JobUniqueID INT NOT NULL," +
                    "PanelDBID INT NOT NULL," +
                    "USERID VARCHAR(50) NOT NULL," +
                    "PCBID INT NOT NULL," +
                    "BARCODE VARCHAR(50)," +
                    "LOTID VARCHAR(50)," +
                    "BARCODE_CRC VARCHAR(50)," +
                    "LOTID_CRC VARCHAR(50)," +
                    "PCBRESULT INT NOT NULL," +
                    "TotalPanelCount INT NOT NULL," +
                    "TotalComponentCount INT NOT NULL," +
                    "NGComponentCount INT NOT NULL," +
                    "PASSComponentCount INT NOT NULL," +
                    "Defect1NGCount INT,Defect2NGCount INT,Defect3NGCount INT,Defect4NGCount INT,Defect5NGCount INT,Defect6NGCount INT,Defect7NGCount INT,Defect8NGCount INT,Defect9NGCount INT,Defect10NGCount INT," +
                    "Defect11NGCount INT,Defect12NGCount INT,Defect13NGCount INT,Defect14NGCount INT,Defect15NGCount INT,Defect16NGCount INT, Defect17NGCount INT,Defect18NGCount INT,Defect19NGCount INT, Defect20NGCount INT," +
                    "Defect21NGCount INT,Defect22NGCount INT,Defect23NGCount INT,Defect24NGCount INT,"+
                    "Defect1PASSCount INT,Defect2PASSCount INT,Defect3PASSCount INT,Defect4PASSCount INT,Defect5PASSCount INT,Defect6PASSCount INT,Defect7PASSCount INT,Defect8PASSCount INT,Defect9PASSCount INT,Defect10PASSCount INT," +
                    "Defect11PASSCount INT,Defect12PASSCount INT,Defect13PASSCount INT,Defect14PASSCount INT,Defect15PASSCount INT,Defect16PASSCount INT,Defect17PASSCount INT,Defect18PASSCount INT,Defect19PASSCount INT,Defect20PASSCount INT," +
                    "Defect21PASSCount INT,Defect22PASSCount INT,Defect23PASSCount INT,Defect24PASSCount INT );";
                break;
            case 3:
                strQuery = "CREATE TABLE ResultPanel(" +
                    "EndDateTime INT NOT NULL," +
                    "JobUniqueID INT NOT NULL," +
                    "PanelID INT NOT NULL," +
                    "PanelResult INT NOT NULL," +
                    "TotalComponentCount INT NOT NULL," +
                    "NGComponentCount INT NOT NULL," +
                    "PassComponentCount INT NOT NULL," +
                    "Defect1NGCount INT,Defect2NGCount INT,Defect3NGCount INT,Defect4NGCount INT,Defect5NGCount INT,Defect6NGCount INT,Defect7NGCount INT,Defect8NGCount INT,Defect9NGCount INT,Defect10NGCount INT," +
                    "Defect11NGCount INT,Defect12NGCount INT,Defect13NGCount INT,Defect14NGCount INT,Defect15NGCount INT,Defect16NGCount INT,Defect17NGCount INT,Defect18NGCount INT,Defect19NGCount INT,Defect20NGCount INT," +
                    "Defect21NGCount INT,Defect22NGCount INT,Defect23NGCount INT,Defect24NGCount INT," +
                    "Defect1PASSCount INT,Defect2PASSCount INT,Defect3PASSCount INT,Defect4PASSCount INT,Defect5PASSCount INT,Defect6PASSCount INT,Defect7PASSCount INT,Defect8PASSCount INT,Defect9PASSCount INT,Defect10PASSCount INT," +
                    "Defect11PASSCount INT,Defect12PASSCount INT,Defect13PASSCount INT,Defect14PASSCount INT,Defect15PASSCount INT,Defect16PASSCount INT,Defect17PASSCount INT,Defect18PASSCount INT,Defect19PASSCount INT,Defect20PASSCount INT," +
                    "Defect21PASSCount INT,Defect22PASSCount INT,Defect23PASSCount INT,Defect24PASSCount INT );";
                break;
            default:
                break;
        }

        args.Log.WriteExecute("Create Table",strQuery);
        SQLiteCommand cmdSQL = new SQLiteCommand(strQuery,conn);
        int nResult  = cmdSQL.ExecuteNonQuery();
     }

    private static void Review_AutoClassfication(KSMART.BRM.Script.AOIReviewCompletedArgs args)
    {
        args.AOI.Review.RtCmdUnLockJudgment("REVIEW");
    }

    public static double ConvertToUnixTimestamp(DateTime date)
    {
        DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime();
        TimeSpan diff = date - origin;
        return System.Math.Floor(diff.TotalSeconds);
    }

    public static double SetMBytes(long iByte)
    {
        return System.Math.Round((iByte/1024/1024).ToDouble(),3) ;
    }
}
