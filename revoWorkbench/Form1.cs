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
            s3.bucketsLists();
            s3.bucketElements("revoworkbench");
            s3.downloadFile("revoworkbench", "userView.json");


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
            var stream = File.OpenText(System.Configuration.ConfigurationSettings.AppSettings["datajson"]);
            //Read the file              
            string st = stream.ReadToEnd();
            var Jresult = JArray.Parse(st);
            var tCount = Jresult.Count;
            for (int i = 0; i < tCount; i++)
            {
                Userview uv = new Userview();

                uv = JsonConvert.DeserializeObject<Userview>(Jresult[i].ToString());

                Panel newpnl = new Panel();
                newpnl.BackColor = Color.Lavender;
                newpnl.Margin = new Padding(1);

                //BROWSER
                Label lbrowser = new Label();
                lbrowser.Text = uv.browserName;

                //LOGO
                Label ll = new Label();
                ll.Text = uv.logo;
                PictureBox llogo = new PictureBox();
                llogo.Size = new Size(250, 250);
                llogo.SizeMode = PictureBoxSizeMode.StretchImage;
                llogo.ImageLocation = uv.logo;
                llogo.MouseClick += (sender, e) =>
                {
                    Process.Start(uv.browserName, uv.Url);
                };
                newpnl.Width = 250;
                newpnl.Height = 250;
                newpnl.Controls.Add(llogo);
                flowLayoutPanel3.Controls.Add(newpnl);

                linkLabel1.Click += (sender, e) =>
                {
                    string json = ReadJson(System.Configuration.ConfigurationSettings.AppSettings["path"] + "\\Resources\\" + configFile);
                    S3BrowserCredentials s3BrowserCredentials = getCredentialsFromJson(json);
                    string args = s3BrowserCredentials.AccountName + " " + s3BrowserCredentials.AccessKeyId + " " +
                                  s3BrowserCredentials.SecretAccessKey;
                    var proc = System.Diagnostics.Process.Start(System.Configuration.ConfigurationSettings.AppSettings["path"] + "\\Resources\\" + S3BrowserExe, args);
                };
                
            }

            label2.Text= "(  "+System.Security.Principal.WindowsIdentity.GetCurrent().Name +" )";
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
    }
}
