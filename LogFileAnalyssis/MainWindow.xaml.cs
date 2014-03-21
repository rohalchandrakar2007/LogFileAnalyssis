﻿using System;
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

namespace LogFileAnalyssis
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public class Session
    {
        public int sessionId;
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

        public Session(int id)
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
        }
    }

    public class Request
    {
        System.Windows.Forms.OpenFileDialog openLogFile = new System.Windows.Forms.OpenFileDialog();
        
        //openLogFile.FileName = "Document"; // Default file name
        //openLogFile.DefaultExt = ".txt"; // Default file extension
        //openLogFile.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension
        public String userName;
        public String visitingPath;
        public String pathTraversed;
        public DateTime timeStamp;
        public String pageLastVisited;
        public String sucessRate;
        
        public String url;                               //from where the client has been redirected to the current page
        public String userAgent;
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
                visitingPath = s[1];
                pathTraversed = s[2];
                timeStamp = DateTime.Parse(dateParseBritishFormat(s[3]));// +" " + s[4];
                
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
                    url = "";
                    userAgent = "";
                    logFormatType = "common";
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
            {}
            }
        private void LoadData()
        {
            //SetConnection();
            //sql_con.Open();
            //sql_cmd = sql_con.CreateCommand();
            //string CommandText = "select id, desc from mains";
            //DB = new SQLiteDataAdapter(CommandText, sql_con);
            //DS.Reset();
            //DB.Fill(DS);
            //DT = DS.Tables[0];
            //Grid.DataSource = DT;
            //sql_con.Close();
        }
        private void Add()
{
    int i = 9;
    String sa = "Hi";
    string txtSQLQuery = "insert into  test values ('"+i.ToString()+"','"+sa+"')";
ExecuteQuery(txtSQLQuery);            
}
    }


    public partial class MainWindow : Window
    {

        public String filePathString = "";
        Hashtable sessionNametoIdhashtable = new Hashtable();
        List<Session> session = new List<Session>();
        List<Request> requestList = new List<Request>();
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
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Close();
           
        }


        //  sample Request format : 199.72.81.55 - - [01/Jul/1995:00:00:01 -0400] "GET /history/apollo/ HTTP/1.0" 200 6245

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            //ThreadStart childref = new ThreadStart(CoreProcessing);
            //Thread childThread = new Thread(childref);
            //childThread.Start();

            //ThreadStart statusTheread = new ThreadStart(UpdateStatusBar);
            //Thread cstatusThread = new Thread(statusTheread);
            //cstatusThread.Start();
            statusBar.Content = "hsfjkadhgk";
            //sessionTime = float.Parse(sessionTimeComboBox.SelectedValue.ToString());
            if (!filePathString.Equals(""))
            {
                win.IsEnabled = false;
                this.IsEnabled = false;
                StreamReader reader = new StreamReader(filePathString);

                statusBar.Content = "Please Wait (Parsing the log file...)";
                string line;
                Thread loadingAnimation = new Thread(new ThreadStart(CustomAnimation));
                while ((line = reader.ReadLine()) != null)
                {
                   
                    String[] s = line.Split();

                    requestList.Add(new Request(line));
                    countLines++;
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
                        session.Add(new Session(sessionId));
                        session[sessionId].sessionStartTime = requestList[countLines].timeStamp;
                       
                        session[sessionId].requestIdlist.Add(countLines);
                        UpdateSessionClassVariables();
                        
                        //if ( timeDiffSession > 30)
                        //    sessionTag++;


                    }
                    else
                    {
                        session[(int)sessionNametoIdhashtable[requestList[countLines].userName + " " + requestList[countLines].userAgent + sessionTag]].requestIdlist.Add(countLines);
                        UpdateSessionClassVariables();

                    }
                    
                    //GC.Collect();
                    //GC.WaitForPendingFinalizers();
                }
                CleanDataBase();
                /* code for inserting into database */
                statusBar.Content = "Please Wait (Inserting session detailsinto database...)";
                for (int i = 0; i < sessionId - 1; i++)
                {

                    UpdateDataBase(i);
                }
                
                statusBar.Content = "Processing Completed...";
                reader.Close();
                loadingAnimation.Abort();
            }
            else
            {
                System.Windows.MessageBox.Show("Select any File for Analysis...");
            }

            win.IsEnabled = true;
          
        }

        private void CleanDataBase()
        {
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

            string txtSQLQuery = "insert into  session values ('" + p1 + "','" + p2 + "','" + p3 + "','" + p4 + "','" + p5 + "','" + p6 + "','" + p7 + "','" + p8 + "','" + p9 + "','" + p10 + "','" + p11 + "','" + p12 + "','" + p13 + "','" + p14 + "','" + p15 + "','" + p16 + "','" + p17 + "','" + p18 + "','" + p19 + "','" + p20 + "','" + p21 + "','" + p22 + "','" + p23 + "','" + p24 + "','" + p25 + "')";
            ExecuteQuery(txtSQLQuery);
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
            catch (Exception e)
            { }
        }
        private void LoadData()
        {
            //SetConnection();
            //sql_con.Open();
            //sql_cmd = sql_con.CreateCommand();
            //string CommandText = "select id, desc from mains";
            //DB = new SQLiteDataAdapter(CommandText, sql_con);
            //DS.Reset();
            //DB.Fill(DS);
            //DT = DS.Tables[0];
            //Grid.DataSource = DT;
            //sql_con.Close();
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

        
    }
}
