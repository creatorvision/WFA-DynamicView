using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Forms;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;


using System.IO;

namespace revoWorkbench
{

    public partial class Form1 : Form
    {
        private readonly string S3BrowserExe = "TestS3BrowserAutomation.exe";
        private readonly string configFile = "S3BrowserConfiguration.json";

        public Form1()
        {

            S3 s3 = new S3();
            //s3.bucketsLists();
            //s3.bucketElements("revoworkbench");
            s3.downloadFile(System.Configuration.ConfigurationSettings.AppSettings["bucketName"], System.Configuration.ConfigurationSettings.AppSettings["fileName"]);


            InitializeComponent();

            //Layout
            this.Width = 1080;
            this.Height = 720;
            flowLayoutPanel1.Height = 720;
            flowLayoutPanel1.Width = 756;
            flowLayoutPanel2.Height = 720;
            flowLayoutPanel2.Width = 324;
            flowLayoutPanel3.Height = 710;
            flowLayoutPanel3.Width = 756;

            label1.Height = 10;
            label1.Width = 756;


            //Open the file              
            var stream = File.OpenText(Path.GetTempPath()+System.Configuration.ConfigurationSettings.AppSettings["datajson"]);
            //Read the file              
            string st = stream.ReadToEnd();
            var Jresult = JArray.Parse(st);
            var tCount = Jresult.Count;
            for (int i = 0; i < tCount; i++)
            {
                Userview uv = new Userview();

                uv = JsonConvert.DeserializeObject<Userview>(Jresult[i].ToString());

                FlowLayoutPanel newpnl = new FlowLayoutPanel();
                newpnl.BackColor = Color.Lavender;
                newpnl.Margin = new Padding(1);

                //BROWSER
                Label lbrowser = new Label();
                lbrowser.Text = uv.browserName;

                //LOGO
                Label ll = new Label();
                ll.Text = uv.logo;
                PictureBox llogo = new PictureBox();
                llogo.Cursor = Cursors.Hand;
                llogo.Size = new Size(220, 220);
                llogo.SizeMode = PictureBoxSizeMode.StretchImage;
                llogo.ImageLocation = uv.logo;
                llogo.MouseClick += (sender, e) =>
                {
                    if(uv.Type == "Desktop")
                    {
                        startProcess(uv, e);
                    }
                    else
                    {
                        Process.Start(uv.browserName, uv.Url);
                    }
                };

                //Name for the Tumbnail
                Label name = new Label()
                {
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.None,
                    Left = 10,
                    Width = llogo.Width - 10
                }; 
                name.Text = uv.Name;
                name.Font = new Font("Arial", 14F, FontStyle.Italic, GraphicsUnit.Point, ((byte)(0)));

                newpnl.Width = 220;
                newpnl.Height = 220;
                newpnl.Controls.Add(llogo);
                newpnl.Controls.Add(name);
                newpnl.FlowDirection = FlowDirection.TopDown;

                Rectangle r = new Rectangle(0, 0, newpnl.Width, newpnl.Height);
                System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();
                int d = 60;
                gp.AddArc(r.X, r.Y, d, d, 180, 90);
                gp.AddArc(r.X + r.Width - d, r.Y, d, d, 270, 90);
                gp.AddArc(r.X + r.Width - d, r.Y + r.Height - d, d, d, 0, 90);
                gp.AddArc(r.X, r.Y + r.Height - d, d, d, 90, 90);
                newpnl.Region = new Region(gp);

                Rectangle r1 = new Rectangle(0, 0, flowLayoutPanel5.Width, flowLayoutPanel5.Height);
                System.Drawing.Drawing2D.GraphicsPath gp1 = new System.Drawing.Drawing2D.GraphicsPath();
                int d1 = 5;
                gp1.AddArc(r1.X, r1.Y, d1, d1, 180, 90);
                gp1.AddArc(r1.X + r1.Width - d1, r1.Y, d1, d1, 270, 90);
                gp1.AddArc(r1.X + r1.Width - d1, r1.Y + r1.Height - d1, d1, d1, 0, 90);
                gp1.AddArc(r1.X, r1.Y + r1.Height - d1, d1, d1, 90, 90);
                flowLayoutPanel5.Region = new Region(gp1);

                FlowLayoutPanel thumbnail = new FlowLayoutPanel();
                thumbnail.FlowDirection = FlowDirection.TopDown;
                thumbnail.Width = 230;
                thumbnail.Height = 260;

                thumbnail.Controls.Add(newpnl);
                thumbnail.Controls.Add(name);
                flowLayoutPanel3.Controls.Add(thumbnail);              
            }

            linkLabel1.Click += (sender, e) =>
            {
                string json = ReadJson(System.Configuration.ConfigurationSettings.AppSettings["path"] + "\\Resources\\" + configFile);
                S3BrowserCredentials s3BrowserCredentials = getCredentialsFromJson(json);
                string args = s3BrowserCredentials.AccountName + " " + s3BrowserCredentials.AccessKeyId + " " +
                              s3BrowserCredentials.SecretAccessKey;
                var proc = System.Diagnostics.Process.Start(System.Configuration.ConfigurationSettings.AppSettings["path"] + "\\Resources\\" + S3BrowserExe, args);
            };
            label2.Text= System.Security.Principal.WindowsIdentity.GetCurrent().Name ;
            flowLayoutPanel2.AutoScroll = true;
            flowLayoutPanel2.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel2.WrapContents = false; // Vertical rather than horizontal scrolling
        }

        private string ReadJson(string filePath)
        {
            if (filePath == null)
                return null;
            var file = File.OpenText(filePath);
            string stream = file.ReadToEnd();
            var jResult = JArray.Parse(stream);
            var count = jResult.Count;
            if (count > 0)
            {
                return jResult[0].ToString();
            }
            return null;
        }

        private S3BrowserCredentials getCredentialsFromJson(string jsonObject)
        {
            var s3BrowserCredentials = JsonConvert.DeserializeObject<S3BrowserCredentials>(jsonObject);
            return s3BrowserCredentials;
        }

        private void startProcess(Userview uv, EventArgs e)
        {
            string activationKey = System.Configuration.ConfigurationSettings.AppSettings["TableauKey"];
            string tableauExePath = GetTableauExePath();
            var tableauAutomation = new TableauAutomation(tableauExePath);
            tableauAutomation.AutomateTableau(activationKey, uv);
        }
        private string GetTableauExePath()
        {
            try
            {
                var folders = Directory.GetDirectories(System.Configuration.ConfigurationSettings.AppSettings["TableauPath"]);
                string tableauVersion = "";
                foreach (var folder in folders)
                {
                    if (folder.Contains("Tableau"))
                        tableauVersion = folder.ToString();
                }
                string tableauExePath = tableauVersion + "\\" + "bin\\tableau.exe";
                return tableauExePath;
            }
            catch(Exception e)
            {
                // Audit Log  -- later
            }
            return null;
        }

    }
}
