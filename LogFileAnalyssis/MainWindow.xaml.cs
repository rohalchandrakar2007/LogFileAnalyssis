using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using Finisar.SQLite;
using System.IO;
using System.Data;
using System.Collections;
using System.Data.SqlClient;
using OxyPlot;
using OxyPlot.Series;
using System.Diagnostics;

namespace LogFileAnalyssis
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public class Session
    {
        public int sessionId;
        public string sessionUsername;
        public string sessionUseragent;
        public int sessionType;
        public bool isRobotSession;
        public List<int> requestIdlist = new List<int>();

        /* features variables for the session class */
        public long totalNoOfPagesRequestedInSession;
        public long noOfImagePagesRequestedInSession;
        public long noOfBinaryDocumentsRequestedInSession;
        public long noOfBinaryExeFileRequestedInSession;
        public long noOfHTMLFileRequestedInSession;
        public long noOfAsciiFilerequestedInSession;
        public long noOfCompressedFileRequestedInSession;
        public long noOfMultimediaFileRequestedInSession;
        public long noOfOtherFileFormatRequestedInSession;
        public float totalTimeOfTheSession;
        public float avgTimeBetweenTowHTMLRequests;
        public float standardDeviationOfTimeBetweenRequests;
        public long noOfPagesRequestedInNightTime;
        public long noOfRequestReapted;
        public long onOfrequestesWithErrors;
        public long noOfRequestWithGETMethod;
        public long noOfRequestWithPOSTMethod;
        public long noOfRequestWithOtherMethod;
        public long widthOfTheTraversal;
        public long depthOfTheTraversal;
        public bool isMultipleIPSEssion;
        public bool isMultiAgentSession;
      
        /* Features for classifiying the Session */
        public bool isRobotstxtVisited;
        public long noOfRequestWithHEADMethod;
        public long noOfRequestWithUnassignedReferer;

        public DateTime sessionStartTime;

        public Session(int id, string un, string ua, string[] type1, string[] type2, string[] type3, string[] type4)
        {
            sessionId = id;
            totalNoOfPagesRequestedInSession = 0;
            noOfImagePagesRequestedInSession = 0;
            noOfBinaryDocumentsRequestedInSession = 0;
            noOfBinaryExeFileRequestedInSession = 0;
            noOfHTMLFileRequestedInSession = 0;
            noOfAsciiFilerequestedInSession = 0;
            noOfCompressedFileRequestedInSession = 0;
            noOfMultimediaFileRequestedInSession = 0;
            noOfOtherFileFormatRequestedInSession = 0;
            totalTimeOfTheSession = 0;
            avgTimeBetweenTowHTMLRequests = 0;
            standardDeviationOfTimeBetweenRequests = 0;
            noOfPagesRequestedInNightTime = 0;
            noOfRequestReapted = 0;
            onOfrequestesWithErrors = 0;
            noOfRequestWithGETMethod = 0;
            noOfRequestWithPOSTMethod = 0;
            noOfRequestWithOtherMethod = 0;
            widthOfTheTraversal = 0;
            depthOfTheTraversal = 0;
            isMultipleIPSEssion = false;
            isMultiAgentSession = false;
            isRobotstxtVisited = false;
            noOfRequestWithHEADMethod = 0;
            noOfRequestWithUnassignedReferer = 0;
            //input.Replace("\"", "");
            sessionUsername = un.Replace("\"", "");
            sessionUseragent = ua.Replace("\"", "");
            sessionType = getSessionType(ua, type1, type2, type3, type4);
            isRobotSession = false;
            
        }

        private int getSessionType(string ua , string[] type1, string[] type2, string[] type3, string[] type4)
        {
            if (Array.Exists(type1, element => element == ua))
                return 1;
            if (Array.Exists(type2, element => element == ua))
                return 2;
            if (Array.Exists(type3, element => element == ua))
                return 3;
            if (Array.Exists(type4, element => element == ua))
                return 4;
            return 0;
        }
    }

    public class Request
    {
        System.Windows.Forms.OpenFileDialog openLogFile = new System.Windows.Forms.OpenFileDialog();
        
        //openLogFile.FileName = "Document"; // Default file name
        //openLogFile.DefaultExt = ".txt"; // Default file extension
        //openLogFile.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension
        public String userName;
        //public String visitingPath;
        //public String pathTraversed;
        public DateTime timeStamp;
        public String pageLastVisited;
        public String sucessRate;
        
        public String url="";                               //from where the client has been redirected to the current page
        public String userAgent="";
        public String logFormatType="common";                     // "combined" or "common" 
        public String pageRequestMethod;                   // GET or POST

        private SQLiteConnection sql_con;
        private SQLiteCommand sql_cmd;
        private SQLiteDataAdapter DB;
        private DataSet DS = new DataSet();
        private DataTable DT = new DataTable();


        public Request(String line)
        {
            String[] s = line.Split();
            if (s.Length > 8)
            {
                userName = s[0];
                //visitingPath = s[1];
                //pathTraversed = s[2];
                timeStamp = DateTime.Parse(dateParseBritishFormat(s[3]));
                pageLastVisited = s[6];
                pageRequestMethod = s[5].TrimStart('\"');
                sucessRate = s[8];
                if (s.Length > 11)
                {
                    url = s[10];
                    userAgent = s[11];
                    logFormatType = "combined";
                }
                else
                {
                    //url = "";
                    //userAgent = "";
                    //logFormatType = "common";
                }
              //  Add();
            }
        }

        private String dateParse(String dateTime)
        {
            //dateTime = dateTime.TrimStart();
            if (dateTime.Contains("Jan"))
                return "01/"+dateTime.Substring(1,2)+dateTime.Substring(7,5)+" "+dateTime.Substring(13,8);
            if (dateTime.Contains("Feb"))
                return "02/" + dateTime.Substring(1, 2) + dateTime.Substring(7, 5) + " " + dateTime.Substring(13, 8);
            if (dateTime.Contains("Mar"))
                return "03/" + dateTime.Substring(1, 2) + dateTime.Substring(7, 5) + " " + dateTime.Substring(13, 8);
            if (dateTime.Contains("Apr"))
                return "04/" + dateTime.Substring(1, 2) + dateTime.Substring(7, 5) + " " + dateTime.Substring(13, 8);
            if (dateTime.Contains("May"))
                return "05/" + dateTime.Substring(1, 2) + dateTime.Substring(7, 5) + " " + dateTime.Substring(13, 8);
            if (dateTime.Contains("Jun"))
                return "06/" + dateTime.Substring(1, 2) + dateTime.Substring(7, 5) + " " + dateTime.Substring(13, 8);
            if (dateTime.Contains("Jul"))
                return "07/" + dateTime.Substring(1, 2) + dateTime.Substring(7, 5) + " " + dateTime.Substring(13, 8);
            if (dateTime.Contains("Aug"))
                return "08/" + dateTime.Substring(1, 2) + dateTime.Substring(7, 5) + " " + dateTime.Substring(13, 8);
            if (dateTime.Contains("Sep"))
                return "09/" + dateTime.Substring(1, 2) + dateTime.Substring(7, 5) + " " + dateTime.Substring(13, 8);
            if (dateTime.Contains("Oct"))
                return "10/" + dateTime.Substring(1, 2) + dateTime.Substring(7, 5) + " " + dateTime.Substring(13, 8);
            if (dateTime.Contains("Sep"))
                return "11/" + dateTime.Substring(1, 2) + dateTime.Substring(7, 5) + " " + dateTime.Substring(13, 8);
            if (dateTime.Contains("Dec"))
                return "12/" + dateTime.Substring(1, 2) + dateTime.Substring(7, 5) + " " + dateTime.Substring(13, 8);
            return dateTime.Substring(4,2) + dateTime.Substring(1, 2) + dateTime.Substring(6, 5) + " " + dateTime.Substring(12, 8);
        }

        private String dateParseBritishFormat(String dateTime)
        {
            //dateTime = dateTime.TrimStart();
            if (dateTime.Contains("Jan"))
                return dateTime.Substring(1, 3) + "01" + dateTime.Substring(7, 5) + " " + dateTime.Substring(13, 8);
            if (dateTime.Contains("Feb"))
                return dateTime.Substring(1, 3) + "02" + dateTime.Substring(7, 5) + " " + dateTime.Substring(13, 8);
            if (dateTime.Contains("Mar"))
                return dateTime.Substring(1, 3) + "03" + dateTime.Substring(7, 5) + " " + dateTime.Substring(13, 8);
            if (dateTime.Contains("Apr"))
                return dateTime.Substring(1, 3) + "04" + dateTime.Substring(7, 5) + " " + dateTime.Substring(13, 8);
            if (dateTime.Contains("May"))
                return dateTime.Substring(1, 3) + "05" + dateTime.Substring(7, 5) + " " + dateTime.Substring(13, 8);
            if (dateTime.Contains("Jun"))
                return dateTime.Substring(1, 3) + "06" + dateTime.Substring(7, 5) + " " + dateTime.Substring(13, 8);
            if (dateTime.Contains("Jul"))
                return dateTime.Substring(1, 3) + "07" + dateTime.Substring(7, 5) + " " + dateTime.Substring(13, 8);
            if (dateTime.Contains("Aug"))
                return dateTime.Substring(1, 3) + "08" + dateTime.Substring(7, 5) + " " + dateTime.Substring(13, 8);
            if (dateTime.Contains("Sep"))
                return dateTime.Substring(1, 3) + "09" + dateTime.Substring(7, 5) + " " + dateTime.Substring(13, 8);
            if (dateTime.Contains("Oct"))
                return dateTime.Substring(1, 3) + "10" + dateTime.Substring(7, 5) + " " + dateTime.Substring(13, 8);
            if (dateTime.Contains("Sep"))
                return dateTime.Substring(1, 3) + "11" + dateTime.Substring(7, 5) + " " + dateTime.Substring(13, 8);
            if (dateTime.Contains("Dec"))
                return dateTime.Substring(1, 3) + "12" + dateTime.Substring(7, 5) + " " + dateTime.Substring(13, 8);
            return dateTime.Substring(1, 3) + dateTime.Substring(4, 2) + dateTime.Substring(7, 5) + " " + dateTime.Substring(12, 8);
        }

        private void SetConnection()
        {
            sql_con = new SQLiteConnection
                ("Data Source=RequestTable;Version=3;New=False;Compress=True;");
        }

        private void ExecuteQuery(string txtQuery)
        {
            try
            {
                SetConnection();
                sql_con.Open();
                sql_cmd = sql_con.CreateCommand();
                sql_cmd.CommandText = txtQuery;
                sql_cmd.ExecuteNonQuery();
                sql_con.Close();
            }
            catch(Exception e)
            {
            
            }
            }

        private void LoadData()
        {
           
        }

        private void Add()
{
    int i = 9;
    String sa = "Hi";
    string txtSQLQuery = "insert into  test values ('"+i.ToString()+"','"+sa+"')";
ExecuteQuery(txtSQLQuery);            
}
    }

    public class CsvRow : List<string>
    {
        public string LineText { get; set; }
    }

    /// <summary>
    /// Class to write data to a CSV file
    /// </summary>
    public class CsvFileWriter : StreamWriter
    {
        public CsvFileWriter(Stream stream)
            : base(stream)
        {
        }

        public CsvFileWriter(string filename)
            : base(filename)
        {
        }

        /// <summary>
        /// Writes a single row to a CSV file.
        /// </summary>
        /// <param name="row">The row to be written</param>
        public void WriteRow(CsvRow row)
        {
            StringBuilder builder = new StringBuilder();
            bool firstColumn = true;
            foreach (string value in row)
            {
                // Add separator if this isn't the first value
                if (!firstColumn)
                    builder.Append(',');
                // Implement special handling for values that contain comma or quote
                // Enclose in quotes and double up any double quotes
                if (value.IndexOfAny(new char[] { '"', ',' }) != -1)
                    builder.AppendFormat("\"{0}\"", value.Replace("\"", "\"\""));
                else
                    builder.Append(value);
                firstColumn = false;
            }
            row.LineText = builder.ToString();
            WriteLine(row.LineText);
        }
    }

    /// <summary>
    /// Class to read data from a CSV file
    /// </summary>
    public class CsvFileReader : StreamReader
    {
        public CsvFileReader(Stream stream)
            : base(stream)
        {
        }

        public CsvFileReader(string filename)
            : base(filename)
        {
        }

        /// <summary>
        /// Reads a row of data from a CSV file
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public bool ReadRow(CsvRow row)
        {
            row.LineText = ReadLine();
            if (String.IsNullOrEmpty(row.LineText))
                return false;

            int pos = 0;
            int rows = 0;

            while (pos < row.LineText.Length)
            {
                string value;

                // Special handling for quoted field
                if (row.LineText[pos] == '"')
                {
                    // Skip initial quote
                    pos++;

                    // Parse quoted value
                    int start = pos;
                    while (pos < row.LineText.Length)
                    {
                        // Test for quote character
                        if (row.LineText[pos] == '"')
                        {
                            // Found one
                            pos++;

                            // If two quotes together, keep one
                            // Otherwise, indicates end of value
                            if (pos >= row.LineText.Length || row.LineText[pos] != '"')
                            {
                                pos--;
                                break;
                            }
                        }
                        pos++;
                    }
                    value = row.LineText.Substring(start, pos - start);
                    value = value.Replace("\"\"", "\"");
                }
                else
                {
                    // Parse unquoted value
                    int start = pos;
                    while (pos < row.LineText.Length && row.LineText[pos] != ',')
                        pos++;
                    value = row.LineText.Substring(start, pos - start);
                }

                // Add field to list
                if (rows < row.Count)
                    row[rows] = value;
                else
                    row.Add(value);
                rows++;

                // Eat up to and including next comma
                while (pos < row.LineText.Length && row.LineText[pos] != ',')
                    pos++;
                if (pos < row.LineText.Length)
                    pos++;
            }
            // Delete any unused items
            while (row.Count > rows)
                row.RemoveAt(rows);

            // Return true if any columns read
            return (row.Count > 0);
        }
    }
    public partial class MainWindow : Window
    {

        public String filePathString = "";
        public static List<Session> sessionforExport = new List<Session>();
        Hashtable sessionNametoIdhashtable = new Hashtable();
        List<Session> session = new List<Session>();
        List<Request> requestList = new List<Request>();
        List<string> type1 = new List<string>();
        List<string> type2 = new List<string>();
        List<string> type3 = new List<string>();
        List<string> type4 = new List<string>();
        int countLines = -1;
        int sessionId =-1,tempInt=0 , sessionTag =0;
        private SQLiteConnection sql_con;
        private SQLiteCommand sql_cmd;
        private SQLiteDataAdapter DB;
        private DataSet DS = new DataSet();
        private DataTable DT = new DataTable();
        private String statusCode = "";
        private float sessionTime = 30;
        private bool isUserAgentInvolved= false;
        private float timeDiffSession = 0;
        public MainWindow()
        {
            InitializeComponent();
            
            sessionTimeComboBox.Items.Add("Select Session Time in Minuts");
            sessionTimeComboBox.Items.Add("5");
            sessionTimeComboBox.Items.Add("10");
            sessionTimeComboBox.Items.Add("15");
            sessionTimeComboBox.Items.Add("20");
            sessionTimeComboBox.Items.Add("25");
            sessionTimeComboBox.Items.Add("30");
            sessionTimeComboBox.Items.Add("35");
            sessionTimeComboBox.SelectedValue = "Select Session Time in Minuts";

            /* Update the constrains for clssifiying session by useragents */
            System.IO.StreamReader file =
            new System.IO.StreamReader("sessionUseragent.txt");
            int typeCount = 0;
            string line;
            while ((line = file.ReadLine()) != null)
            {
                if (line.Equals("Type1") || line.Equals("Type2") || line.Equals("Type3") || line.Equals("Type4"))
                {
                    if (line.Equals("Type1"))
                        typeCount = 1;
                    if (line.Equals("Type2"))
                        typeCount = 2;
                    if (line.Equals("Type3"))
                        typeCount = 3;
                    if (line.Equals("Type4"))
                        typeCount = 4;
                }
                else
                {
                    if (typeCount == 1)
                        type1.Add(line);
                    if (typeCount == 2)
                        type2.Add(line);
                    if (typeCount == 3)
                        type3.Add(line);
                    if (typeCount == 4)
                        type4.Add(line);

                }
                //Console.WriteLine(line);
               // counter++;
            }

            file.Close();
        }
        public PlotModel MyModel { get; private set; }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            sessionforExport = session;
            Export_Settings frm = new Export_Settings();
            frm.Show();
            //Close();
           
        }


        //  sample Request format : 199.72.81.55 - - [01/Jul/1995:00:00:01 -0400] "GET /history/apollo/ HTTP/1.0" 200 6245

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

            ThreadStart childref = new ThreadStart(CallToChildThread);
            //Console.WriteLine("In Main: Creating the Child thread");
            Thread childThread = new Thread(childref);
            childThread.Start();

            //statusBar.Content = "hsfjkadhgk";
            //sessionTime = float.Parse(sessionTimeComboBox.SelectedValue.ToString());
            if (!filePathString.Equals(""))
            {
                win.IsEnabled = false;
                this.IsEnabled = false;
                StreamReader reader = new StreamReader(filePathString);
                countLines = 0;
                statusBar.Content = "Please Wait (Parsing the log file...)";
                string line;
                Thread loadingAnimation = new Thread(new ThreadStart(CustomAnimation));
                while ((line = reader.ReadLine()) != null)
                {
                   
                    //String[] s = line.Split();

                    requestList.Add(new Request(line));
                    //if(countLines==-1)
                    //countLines++;
                    if ((bool)sessionTimeCheckBox.IsChecked)
                    {
                        sessionTime = float.Parse(sessionTimeComboBox.SelectedValue.ToString());
                        if (sessionId == -1)
                            timeDiffSession = 0;
                        else
                            timeDiffSession = (float)(requestList[countLines].timeStamp - session[sessionId].sessionStartTime).TotalMinutes;
                        if (timeDiffSession > sessionTime)
                            sessionTag++;
                    }
                    if (!sessionNametoIdhashtable.Contains(requestList[countLines].userName + " " + requestList[countLines].userAgent + sessionTag))
                    {
                        sessionId++;
                        sessionNametoIdhashtable.Add(requestList[countLines].userName + " " + requestList[countLines].userAgent + sessionTag, sessionId);
                        session.Add(new Session(sessionId , requestList[countLines].userName , requestList[countLines].userAgent , type1.ToArray() , type2.ToArray() , type3.ToArray() ,type4.ToArray()));
                        session[sessionId].sessionStartTime = requestList[countLines].timeStamp;
                        session[sessionId].requestIdlist.Add(countLines);
                        UpdateSessionClassVariables();
                    }
                    else
                    {
                        session[(int)sessionNametoIdhashtable[requestList[countLines].userName + " " + requestList[countLines].userAgent + sessionTag]].requestIdlist.Add(countLines);
                        UpdateSessionClassVariables();

                    }
                   
                    
                        requestList.Clear();
                    
                }
                //statusBar.Content = session[0].sessionUseragent;
                CleanDataBase();
                bulkInsertDatabase();
                

                statusBar.Content = "Processing Completed...";
                reader.Close();
                loadingAnimation.Abort();
                sessionforExport = session;
            }
            else
            {
                System.Windows.MessageBox.Show("Select any File for Analysis...");
            }

            win.IsEnabled = true;
          
        }

        public void CallToChildThread()
        {
            
        }

        private void bulkInsertDatabase()
        {
            sql_con = new SQLiteConnection("Data Source=RequestTable;Version=3;New=False;Compress=True;");
            sql_con.Open();
            sql_cmd = sql_con.CreateCommand();
            
            using (var transaction = sql_con.BeginTransaction())
            {
                // 100,000 inserts
                for (var tempSessionId = 0; tempSessionId < sessionId - 1 ; tempSessionId++)
                {
                    try
                    {
                        int p1 = tempSessionId;
                        long p2 = session[tempSessionId].totalNoOfPagesRequestedInSession;
                        float p3 = ((float)session[tempSessionId].noOfImagePagesRequestedInSession / p2) * 100;
                        float p4 = ((float)session[tempSessionId].noOfBinaryDocumentsRequestedInSession / p2) * 100;
                        float p5 = ((float)session[tempSessionId].noOfBinaryExeFileRequestedInSession / p2) * 100;
                        String p6 = session[tempSessionId].isRobotstxtVisited.ToString();
                        float p7 = ((float)session[tempSessionId].noOfHTMLFileRequestedInSession / p2) * 100;
                        float p8 = ((float)session[tempSessionId].noOfAsciiFilerequestedInSession / p2) * 100;
                        float p9 = ((float)session[tempSessionId].noOfCompressedFileRequestedInSession / p2) * 100;
                        float p10 = ((float)session[tempSessionId].noOfMultimediaFileRequestedInSession / p2) * 100;
                        float p11 = ((float)session[tempSessionId].noOfOtherFileFormatRequestedInSession / p2) * 100;
                        float p12 = System.Math.Abs(session[tempSessionId].totalTimeOfTheSession);
                        float p13 = System.Math.Abs(session[tempSessionId].avgTimeBetweenTowHTMLRequests);
                        long p14 = session[tempSessionId].noOfPagesRequestedInNightTime;
                        long p15 = session[tempSessionId].noOfRequestReapted;
                        float p16 = ((float)session[tempSessionId].onOfrequestesWithErrors / p2) * 100;
                        float p17 = ((float)session[tempSessionId].noOfRequestWithGETMethod / p2) * 100;
                        float p18 = ((float)session[tempSessionId].noOfRequestWithPOSTMethod / p2) * 100;
                        float p19 = ((float)session[tempSessionId].noOfRequestWithHEADMethod / p2) * 100;
                        float p20 = ((float)session[tempSessionId].noOfRequestWithOtherMethod / p2) * 100;
                        long p21 = session[tempSessionId].depthOfTheTraversal;
                        long p22 = session[tempSessionId].noOfHTMLFileRequestedInSession;
                        float p23 = ((float)session[tempSessionId].noOfRequestWithUnassignedReferer / p2) * 100;
                        String p24 = session[tempSessionId].isMultipleIPSEssion.ToString();
                        String p25 = session[tempSessionId].isMultiAgentSession.ToString();
                        String p26 = session[tempSessionId].sessionUsername.ToString();
                        String p27 = session[tempSessionId].sessionUseragent.ToString();
                        int p28 = session[tempSessionId].sessionType;
                        String p29 = session[tempSessionId].isRobotSession.ToString();
                        sql_cmd.CommandText = "insert into  session values ('" + p1 + "','" + p2 + "','" + p3 + "','" + p4 + "','" + p5 + "','" + p6 + "','" + p7 + "','" + p8 + "','" + p9 + "','" + p10 + "','" + p11 + "','" + p12 + "','" + p13 + "','" + p14 + "','" + p15 + "','" + p16 + "','" + p17 + "','" + p18 + "','" + p19 + "','" + p20 + "','" + p21 + "','" + p22 + "','" + p23 + "','" + p24 + "','" + p25 + "','" + p26 + "','" + p27 + "','" + p28 + "','" + p29 + "')";
                        sql_cmd.ExecuteNonQuery();
                    }
                    catch(Exception ex)
                    {
                    
                    }
                }

                transaction.Commit();
            }
            //    }

            //    Console.WriteLine("{0} seconds with one transaction.", stopwatch.Elapsed.TotalSeconds);

            //    conn.Close();
            //}
            //throw new NotImplementedException();
        }
        
        private void insertCSVFile()
        {
            using (CsvFileWriter writer = new CsvFileWriter("Session.csv"))
            {

                CsvRow row1 = new CsvRow();
                row1.Add(String.Format("sessionId"));
                row1.Add(String.Format("totalPages"));
                row1.Add(String.Format("percentageImagePages"));
                row1.Add(String.Format("percentageBinaryDocPages"));
                row1.Add(String.Format("percentageBinaryExePages"));
                row1.Add(String.Format("isRobotstxtVisited"));
                row1.Add(String.Format("percentageHTMLPages"));
                row1.Add(String.Format("percentageAsciiPages"));
                row1.Add(String.Format("percentageCompressedFilePages"));
                row1.Add(String.Format("percentageMultimediaPages"));
                row1.Add(String.Format("percentageOtherPages"));
                row1.Add(String.Format("totalSessionTime"));
                row1.Add(String.Format("avgHTMLReqTime"));
                row1.Add(String.Format("totalNightTimeReq"));
                row1.Add(String.Format("totalReapeatedReq"));
                row1.Add(String.Format("percentageOfErrorPages"));
                row1.Add(String.Format("percentageGETMethodReq"));
                row1.Add(String.Format("percentagePOSTMethodReq"));
                row1.Add(String.Format("percentageHEADMethodReq"));
                row1.Add(String.Format("percentageOtherMethodReq"));
                row1.Add(String.Format("depthOfTraversal"));
                row1.Add(String.Format("lengthOfSession"));
                row1.Add(String.Format("percentageReqWithUnassignedReferrer"));
                row1.Add(String.Format("isSessionMultiIP"));
                row1.Add(String.Format("isSessionMultiAgent"));
                row1.Add(String.Format("sessionUsername"));
                row1.Add(String.Format("sessionUseragent"));
                row1.Add(String.Format("useAgentType"));
                row1.Add(String.Format("isRobotSession"));
                writer.WriteRow(row1);

                for (var tempSessionId = 0; tempSessionId < 100 ; tempSessionId++)
                {
                    try
                    {
                        int p1 = tempSessionId;
                        long p2 = session[tempSessionId].totalNoOfPagesRequestedInSession;
                        float p3 = ((float)session[tempSessionId].noOfImagePagesRequestedInSession / p2) * 100;
                        float p4 = ((float)session[tempSessionId].noOfBinaryDocumentsRequestedInSession / p2) * 100;
                        float p5 = ((float)session[tempSessionId].noOfBinaryExeFileRequestedInSession / p2) * 100;
                        String p6 = session[tempSessionId].isRobotstxtVisited.ToString();
                        float p7 = ((float)session[tempSessionId].noOfHTMLFileRequestedInSession / p2) * 100;
                        float p8 = ((float)session[tempSessionId].noOfAsciiFilerequestedInSession / p2) * 100;
                        float p9 = ((float)session[tempSessionId].noOfCompressedFileRequestedInSession / p2) * 100;
                        float p10 = ((float)session[tempSessionId].noOfMultimediaFileRequestedInSession / p2) * 100;
                        float p11 = ((float)session[tempSessionId].noOfOtherFileFormatRequestedInSession / p2) * 100;
                        float p12 = System.Math.Abs(session[tempSessionId].totalTimeOfTheSession);
                        float p13 = System.Math.Abs(session[tempSessionId].avgTimeBetweenTowHTMLRequests);
                        long p14 = session[tempSessionId].noOfPagesRequestedInNightTime;
                        long p15 = session[tempSessionId].noOfRequestReapted;
                        float p16 = ((float)session[tempSessionId].onOfrequestesWithErrors / p2) * 100;
                        float p17 = ((float)session[tempSessionId].noOfRequestWithGETMethod / p2) * 100;
                        float p18 = ((float)session[tempSessionId].noOfRequestWithPOSTMethod / p2) * 100;
                        float p19 = ((float)session[tempSessionId].noOfRequestWithHEADMethod / p2) * 100;
                        float p20 = ((float)session[tempSessionId].noOfRequestWithOtherMethod / p2) * 100;
                        long p21 = session[tempSessionId].depthOfTheTraversal;
                        long p22 = session[tempSessionId].noOfHTMLFileRequestedInSession;
                        float p23 = ((float)session[tempSessionId].noOfRequestWithUnassignedReferer / p2) * 100;
                        String p24 = session[tempSessionId].isMultipleIPSEssion.ToString();
                        String p25 = session[tempSessionId].isMultiAgentSession.ToString();
                        String p26 = session[tempSessionId].sessionUsername.ToString();
                        String p27 = session[tempSessionId].sessionUseragent.ToString();
                        int p28 = session[tempSessionId].sessionType;
                        String p29 = session[tempSessionId].isRobotSession.ToString();
                        CsvRow row = new CsvRow();
                        row.Add(String.Format(p1.ToString()));
                        row.Add(String.Format(p2.ToString()));
                        row.Add(String.Format(p3.ToString()));
                        row.Add(String.Format(p4.ToString()));
                        row.Add(String.Format(p5.ToString()));
                        row.Add(String.Format(p6.ToString()));
                        row.Add(String.Format(p7.ToString()));
                        row.Add(String.Format(p8.ToString()));
                        row.Add(String.Format(p9.ToString()));
                        row.Add(String.Format(p10.ToString()));
                        row.Add(String.Format(p11.ToString()));
                        row.Add(String.Format(p12.ToString()));
                        row.Add(String.Format(p13.ToString()));
                        row.Add(String.Format(p14.ToString()));
                        row.Add(String.Format(p15.ToString()));
                        row.Add(String.Format(p16.ToString()));
                        row.Add(String.Format(p17.ToString()));
                        row.Add(String.Format(p18.ToString()));
                        row.Add(String.Format(p19.ToString()));
                        row.Add(String.Format(p20.ToString()));
                        row.Add(String.Format(p21.ToString()));
                        row.Add(String.Format(p22.ToString()));
                        row.Add(String.Format(p23.ToString()));
                        row.Add(String.Format(p24.ToString()));
                        row.Add(String.Format(p25.ToString()));
                        row.Add(String.Format(p26.ToString()));
                        row.Add(String.Format(p27.ToString()));
                        row.Add(String.Format(p28.ToString()));
                        row.Add(String.Format(p29.ToString()));
                        writer.WriteRow(row);
                    }
                    catch(Exception ex)
                    {
                    
                    }
                }
                //for (int i = 0; i < 100; i++)
                //{
                //    CsvRow row = new CsvRow();
                //    for (int j = 0; j < 5; j++)
                //        row.Add(String.Format("Column{0}", j));
                //    writer.WriteRow(row);
                //}
            }
        }

        private void CleanDataBase()
        {
            sql_con = new SQLiteConnection("Data Source=RequestTable;Version=3;New=False;Compress=True;");
            sql_con.Open();
            sql_cmd = sql_con.CreateCommand();
            string txtSQLQuery = "Delete from session" ;
            ExecuteQuery(txtSQLQuery);
        }

        private void UpdateStatusBar()
        {
           // statusBar.Content = statusCode;
        }

        private void CustomAnimation()
        {
            statusBar.Content = 3;
        }

        private void UpdateSessionClassVariables()
        {
            DateTime tFirstHTMLReq = session[sessionId].sessionStartTime, tLastHTMLReq = session[sessionId].sessionStartTime;
            /* Updating no of page request part */
            session[sessionId].totalNoOfPagesRequestedInSession++;
            try
            {
                if (requestList[countLines].pageLastVisited.Contains(".gif") || requestList[countLines].pageLastVisited.Contains(".jpeg") || requestList[countLines].pageLastVisited.Contains(".jpg") || requestList[countLines].pageLastVisited.Contains(".png"))
                {
                    session[sessionId].noOfImagePagesRequestedInSession++;

                }
                else if (requestList[countLines].pageLastVisited.Contains(".ps") || requestList[countLines].pageLastVisited.Contains(".pdf"))
                {
                    session[sessionId].noOfBinaryDocumentsRequestedInSession++;

                }
                else if (requestList[countLines].pageLastVisited.Contains(".cgi") || requestList[countLines].pageLastVisited.Contains(".exe"))
                {
                    session[sessionId].noOfBinaryExeFileRequestedInSession++;

                }
                else if (requestList[countLines].pageLastVisited.Contains("robots.txt"))
                {
                    session[sessionId].isRobotstxtVisited = true;

                }
                else if (requestList[countLines].pageLastVisited.Contains(".htm") || requestList[countLines].pageLastVisited.Contains(".html"))
                {
                    session[sessionId].noOfHTMLFileRequestedInSession++;
                    if (session[sessionId].noOfHTMLFileRequestedInSession == 1)
                        tFirstHTMLReq = requestList[countLines].timeStamp;
                    else
                        tLastHTMLReq = requestList[countLines].timeStamp;
                    // average time in minutes//
                    session[sessionId].avgTimeBetweenTowHTMLRequests = (float)(((tLastHTMLReq - tFirstHTMLReq).TotalMinutes) / (session[sessionId].noOfHTMLFileRequestedInSession));
                    session[sessionId].totalTimeOfTheSession = (float)(tLastHTMLReq - tFirstHTMLReq).TotalMinutes;
                }
                else if (requestList[countLines].pageLastVisited.Contains(".txt") || requestList[countLines].pageLastVisited.Contains(".c") || requestList[countLines].pageLastVisited.Contains(".java") || requestList[countLines].pageLastVisited.Contains(".php"))
                {
                    session[sessionId].noOfAsciiFilerequestedInSession++;

                }
                else if (requestList[countLines].pageLastVisited.Contains(".zip") || requestList[countLines].pageLastVisited.Contains(".gz") || requestList[countLines].pageLastVisited.Contains(".rar"))
                {
                    session[sessionId].noOfCompressedFileRequestedInSession++;

                }
                else if (requestList[countLines].pageLastVisited.Contains(".wav") || requestList[countLines].pageLastVisited.Contains(".mpg") || requestList[countLines].pageLastVisited.Contains(".mpeg") || requestList[countLines].pageLastVisited.Contains(".avi") || requestList[countLines].pageLastVisited.Contains(".mp4") || requestList[countLines].pageLastVisited.Contains(".3gp") || requestList[countLines].pageLastVisited.Contains(".flv"))
                {
                    session[sessionId].noOfMultimediaFileRequestedInSession++;

                }
                else
                {
                    session[sessionId].noOfOtherFileFormatRequestedInSession++;

                }
                /* Updating the time period */
                if (requestList[countLines].timeStamp.Hour <= 7 && requestList[countLines].timeStamp.ToString().Contains("AM"))
                {
                    session[sessionId].noOfPagesRequestedInNightTime++;
                }

                if (Convert.ToInt32(requestList[countLines].sucessRate) == 400)
                {
                    session[sessionId].onOfrequestesWithErrors++;
                }
                if (requestList[countLines].pageRequestMethod.Contains("GET"))
                {
                    session[sessionId].noOfRequestWithGETMethod++;
                }
                else if (requestList[countLines].pageRequestMethod.Contains("POST"))
                {
                    session[sessionId].noOfRequestWithPOSTMethod++;
                }
                else if (requestList[countLines].pageRequestMethod.Contains("HEAD"))
                {
                    session[sessionId].noOfRequestWithHEADMethod++;
                }
                else
                {
                    session[sessionId].noOfRequestWithOtherMethod++;
                }
                if (requestList[countLines].url.Equals("-"))
                {
                    session[sessionId].noOfRequestWithUnassignedReferer++;
                }

            }
            catch(Exception e)
            {}
        }

        private void UpdateDataBase(int tempSessionId)
        {
            try
            {
                int p1 = tempSessionId;
                long p2 = session[tempSessionId].totalNoOfPagesRequestedInSession;
                float p3 = ((float)session[tempSessionId].noOfImagePagesRequestedInSession / p2) * 100;
                float p4 = ((float)session[tempSessionId].noOfBinaryDocumentsRequestedInSession / p2) * 100;
                float p5 = ((float)session[tempSessionId].noOfBinaryExeFileRequestedInSession / p2) * 100;
                String p6 = session[tempSessionId].isRobotstxtVisited.ToString();
                float p7 = ((float)session[tempSessionId].noOfHTMLFileRequestedInSession / p2) * 100;
                float p8 = ((float)session[tempSessionId].noOfAsciiFilerequestedInSession / p2) * 100;
                float p9 = ((float)session[tempSessionId].noOfCompressedFileRequestedInSession / p2) * 100;
                float p10 = ((float)session[tempSessionId].noOfMultimediaFileRequestedInSession / p2) * 100;
                float p11 = ((float)session[tempSessionId].noOfOtherFileFormatRequestedInSession / p2) * 100;
                float p12 = System.Math.Abs(session[tempSessionId].totalTimeOfTheSession);
                float p13 = System.Math.Abs(session[tempSessionId].avgTimeBetweenTowHTMLRequests);
                long p14 = session[tempSessionId].noOfPagesRequestedInNightTime;
                long p15 = session[tempSessionId].noOfRequestReapted;
                float p16 = ((float)session[tempSessionId].onOfrequestesWithErrors / p2) * 100;
                float p17 = ((float)session[tempSessionId].noOfRequestWithGETMethod / p2) * 100;
                float p18 = ((float)session[tempSessionId].noOfRequestWithPOSTMethod / p2) * 100;
                float p19 = ((float)session[tempSessionId].noOfRequestWithHEADMethod / p2) * 100;
                float p20 = ((float)session[tempSessionId].noOfRequestWithOtherMethod / p2) * 100;
                long p21 = session[tempSessionId].depthOfTheTraversal;
                long p22 = session[tempSessionId].noOfHTMLFileRequestedInSession;
                float p23 = ((float)session[tempSessionId].noOfRequestWithUnassignedReferer / p2) * 100;
                String p24 = session[tempSessionId].isMultipleIPSEssion.ToString();
                String p25 = session[tempSessionId].isMultiAgentSession.ToString();
                String p26 = session[tempSessionId].sessionUsername.ToString();
                String p27 = session[tempSessionId].sessionUseragent.ToString();
                int p28 = session[tempSessionId].sessionType;
                String p29 = session[tempSessionId].isRobotSession.ToString();

                //string txtSQLQuery = "insert into  session values ('" + p1 + "','" + p2 + "','" + p3 + "','" + p4 + "','" + p5 + "','" + p6 + "','" + p7 + "','" + p8 + "','" + p9 + "','" + p10 + "','" + p11 + "','" + p12 + "','" + p13 + "','" + p14 + "','" + p15 + "','" + p16 + "','" + p17 + "','" + p18 + "','" + p19 + "','" + p20 + "','" + p21 + "','" + p22 + "','" + p23 + "','" + p24 + "','" + p25 + "','" + p26 + "','" + p27 + "','" + p28 + "','" + p29 + "')";
                //ExecuteQuery(txtSQLQuery);
            }
            catch(Exception e)
            {}
        }

        private void SetConnection()
        {
            sql_con = new SQLiteConnection("Data Source=RequestTable;Version=3;New=False;Compress=True;");
        }

        private void ExecuteQuery(string txtQuery)
        {
            try
            {
                
                
                sql_cmd = sql_con.CreateCommand();
                sql_cmd.CommandText = txtQuery;
                sql_cmd.ExecuteNonQuery();
                
            }
            catch (Exception e)
            { }
        }

        private void LoadData(String selectQuery)
        {
            sql_con = new SQLiteConnection("Data Source=RequestTable;Version=3;New=False;Compress=True;");
            sql_con.Open();
            sql_cmd = sql_con.CreateCommand();
            string CommandText = selectQuery;
            DB = new SQLiteDataAdapter(CommandText, sql_con);
            DS.Reset();
            DB.Fill(DS);
            DT = DS.Tables[0];
            //statusBar.Content = DT.Rows[3]["totalPages"].ToString();
            //Grid.DataSource = DT;
            sql_con.Close();
        }

        private void Add()
        {
            int i = 9;
            String sa = "Hi";
           // string txtSQLQuery = "insert into  test values ('" + i.ToString() + "','" + sa + "')";
           // ExecuteQuery(txtSQLQuery);
        }

        private void bOpenFileDialog_Click(object sender, RoutedEventArgs e)
        {
            Stream checkStream = null;
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();

            openFileDialog.Multiselect = false;
            //openFileDialog.InitialDirectory = "c:\\";
            //if you want filter only .txt file
            //dlg.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            //if you want filter all files            
            openFileDialog.Filter = "All Files | *.*";
            if ((bool)openFileDialog.ShowDialog())
            {
                try
                {
                    if ((checkStream = openFileDialog.OpenFile()) != null)
                    {
                        //MyImage.Source = new BitmapImage(new Uri(openFileDialog.FileName, UriKind.Absolute));
                        filePath.Text = openFileDialog.FileName;
                        filePathString = openFileDialog.FileName;
                       
                    }
                }
                catch (Exception ex)
                {
                   System.Windows.MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
            else
            {
                   System.Windows.MessageBox.Show("Problem occured, try again later");
            }
        }

        private bool isNewSession(String tempSessionName)
        {
            return true;
        }

        private void sessionTimeCheckBox_Checked(object sender, RoutedEventArgs e)
        {
           sessionTimeComboBox.IsEnabled = true;
        }

        private void sessionTimeCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            sessionTimeComboBox.IsEnabled = false;
        }

        private void userAgentInvolved_Checked(object sender, RoutedEventArgs e)
        {
            isUserAgentInvolved = true;
        }

        private void userAgentInvolved_Unchecked(object sender, RoutedEventArgs e)
        {
            isUserAgentInvolved = false;
        }

        private void buttonClassify(object sender, RoutedEventArgs e)
        {

        }

        private void dataBaseOperationForClassification()
        {
            try
            {
               
                SetConnection();
            sql_con.Open();
                sql_cmd = sql_con.CreateCommand();
                sql_cmd.CommandText = "SELECT * FROM session";
                    statusBar.Content = "1";
                    using (SQLiteDataReader reader = sql_cmd.ExecuteReader())
                    {
                        statusBar.Content = "2";
                        while (reader.Read())
                        {
                            statusBar.Content = reader.GetString(1);
                        }
                        // users.Add(reader.GetInt32(0), reader.GetString(1));
                        reader.Close();
                    }
                    sql_con.Close();
                }
            
            catch(Exception e)
            {
                statusBar.Content = "Exception occured!!!";
            }
        }

        private void classify_Click(object sender, RoutedEventArgs e)
        {
           // dataBaseOperationForClassification();
            
            statusBar.Content = "Labeling sessions...";
            LoadData("select sessionId , isRobotstxtVisited , percentageHEADMethodReq , percentageReqWithUnassignedReferrer , sessionUsername , sessionUseragent , useAgentType from session");
            int lableCount = 0;
            sql_con = new SQLiteConnection("Data Source=RequestTable;Version=3;New=False;Compress=True;");
            sql_con.Open();
            sql_cmd = sql_con.CreateCommand();
            //ExecuteQuery("UPDATE test SET test2='beeeeee' where test1 = '1'");
            //statusBar.Content = DT.Rows[3]["sessionUsername"].ToString();
            for (;lableCount<DT.Rows.Count ;lableCount++)
            {
                if (DT.Rows[lableCount]["isRobotstxtVisited"].ToString().Equals("True"))
                { 
                    /* Class 1 */
                    session[lableCount].isRobotSession = true;
                    ExecuteQuery("UPDATE session SET isRobotSession='True' WHERE sessionId ='" + DT.Rows[lableCount]["sessionId"].ToString() + "'");
                }
                if (DT.Rows[lableCount]["useAgentType"].ToString().Equals("2") || DT.Rows[lableCount]["useAgentType"].ToString().Equals("4") || DT.Rows[lableCount]["useAgentType"].ToString().Equals("0"))
                { 
                    /* Class 0 */
                    if (DT.Rows[lableCount]["percentageHEADMethodReq"].ToString().Equals("100") || (DT.Rows[lableCount]["percentageReqWithUnassignedReferrer"].ToString().Equals("100") && !DT.Rows[lableCount]["percentageReqWithUnassignedReferrer"].ToString().Equals("")))
                    {
                        /* Class 1 */
                        session[lableCount].isRobotSession = true;
                        ExecuteQuery("UPDATE session SET isRobotSession='True' WHERE sessionId ='" + DT.Rows[lableCount]["sessionId"].ToString() + "'");
                    }
                }
                else
                { 
                    /* Class 1 */
                    ExecuteQuery("UPDATE session SET isRobotSession='True' WHERE sessionId ='" + DT.Rows[lableCount]["sessionId"].ToString() + "'");
                }
            }
            sessionforExport = session;
            statusBar.Content = "Session Labeling completed...";
        }

        private void minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void generateCSVFile_Click(object sender, RoutedEventArgs e)
        {
            //insertCSVFile();
          
        }

        private void closeApp(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void userAgentSettings(object sender, RoutedEventArgs e)
        {
            UserAgentsSettings frm = new UserAgentsSettings();
            frm.Show();
        }

        
    }
}
