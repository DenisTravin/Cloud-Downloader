namespace BelkaCloudDownloader.BoxDrive
{
    using System;
    using System.IO;
    using System.Windows.Forms;
    using Box.V2.Models;

    public class BoxItemWithPath
    {
        public BoxItem boxItem { get; set; }
        public string extraPath { get; set; }

        public BoxItemWithPath(BoxItem boxItem, string extraPath)
        {
            this.boxItem = boxItem;
            this.extraPath = extraPath;
        }
    }

    public partial class AuthenticationForm : Form
    {
        private Uri redirect_uri = new Uri(AppConstants.redirectUrl);
        private string authToken = "";
        private string URL = "";

        public AuthenticationForm()
        {
            InitializeComponent();
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Uri url = new Uri("https://account.box.com/api/oauth2/authorize?response_type=code&client_id=" + AppConstants.clientId + "&state=security_token");
            browser.Navigate(url);
            browser.ProgressChanged += Browser_ProgressChanged;
        }

        private void Browser_ProgressChanged(object sender, EventArgs e)
        {
            if (browser.Url != null)
            {
                URL = browser.Url.ToString();
                if ((URL != "https://account.box.com/api/oauth2/authorize?response_type=code&client_id=" + AppConstants.clientId + "&state=security_token") && (URL.Contains("code")))
                {
                    if (authToken == "")
                    {
                        string authCode = "";
                        int fragment = URL.IndexOf("code");
                        int i = 0;
                        int stringSize = URL.Length;
                        while (fragment + 5 + i < stringSize)
                        {
                            authCode += URL[fragment + 5 + i];
                            i++;
                        }
                        string postUrl = "https://api.box.com/oauth2/token";
                        string postData = "grant_type=authorization_code&code=" + authCode + "&client_id=" + AppConstants.clientId + "&client_secret=" + AppConstants.clientSecret;
                        string respons = POST(postUrl, postData);
                        i = 0;
                        fragment = respons.IndexOf("access_token");
                        while (respons[fragment + 15 + i] != '"')
                        {
                            authToken += respons[fragment + 15 + i];
                            i++;
                        }
                    }
                    Close();
                }
            }
        }

        private static string POST(string Url, string Data)
        {
            System.Net.WebRequest req = System.Net.WebRequest.Create(Url);
            req.Method = "POST";
            req.Timeout = 100000;
            req.ContentType = "application/x-www-form-urlencoded";
            byte[] sentData = System.Text.Encoding.GetEncoding(1251).GetBytes(Data);
            req.ContentLength = sentData.Length;
            Stream sendStream = req.GetRequestStream();
            sendStream.Write(sentData, 0, sentData.Length);
            sendStream.Close();
            System.Net.WebResponse res = req.GetResponse();
            Stream ReceiveStream = res.GetResponseStream();
            StreamReader sr = new StreamReader(ReceiveStream, System.Text.Encoding.UTF8);
            Char[] read = new Char[256];
            int count = sr.Read(read, 0, 256);
            string Out = String.Empty;
            while (count > 0)
            {
                String str = new String(read, 0, count);
                Out += str;
                count = sr.Read(read, 0, 256);
            }
            return Out;
        }

        public string ReturnToken()
        {
            return authToken;
        }
    }
}
