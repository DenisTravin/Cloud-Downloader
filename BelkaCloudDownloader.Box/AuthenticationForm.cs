namespace BelkaCloudDownloader.BoxDrive
{
    using System;
    using System.IO;
    using System.Windows.Forms;
    using System.Threading;

    public partial class AuthenticationForm : Form
    {
        private string authToken = "";
        private string url = "";

        public AuthenticationForm()
        {
            InitializeComponent();
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Uri url = new Uri($"https://account.box.com/api/oauth2/authorize?response_type=code&client_id={ AppConstants.clientId }&state=security_token");
            browser.Navigate(url);
            browser.ProgressChanged += Browser_ProgressChanged;
        }

        private void Browser_ProgressChanged(object sender, EventArgs e)
        {
            if (browser.Url != null)
            {
                url = browser.Url.ToString();
                if ((url != $"https://account.box.com/api/oauth2/authorize?response_type=code&client_id={ AppConstants.clientId }&state=security_token") && (url.Contains("code")))
                {
                    if (authToken == "")
                    {
                        string authCode = "";
                        int fragment = url.IndexOf("code");
                        int i = 0;
                        int stringSize = url.Length;
                        while (fragment + 5 + i < stringSize)
                        {
                            authCode += url[fragment + 5 + i];
                            i++;
                        }
                        string postUrl = "https://api.box.com/oauth2/token";
                        string postData = $"grant_type=authorization_code&code={ authCode }&client_id={ AppConstants.clientId }&client_secret={ AppConstants.clientSecret }";
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

        private static string POST(string url, string data)
        {
            byte[] sentData = System.Text.Encoding.GetEncoding(1251).GetBytes(data);
            System.Net.WebRequest req = System.Net.WebRequest.Create(url);
            req.Method = "POST";
            req.Timeout = 100000;
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = sentData.Length;
            string outStr = String.Empty;
            try
            {
                Stream sendStream = req.GetRequestStream();
                sendStream.Write(sentData, 0, sentData.Length);
                sendStream.Close();
                System.Net.WebResponse res = req.GetResponse();
                Stream ReceiveStream = res.GetResponseStream();
                StreamReader sr = new StreamReader(ReceiveStream, System.Text.Encoding.UTF8);
                Char[] read = new Char[256];
                int count = sr.Read(read, 0, 256);
                while (count > 0)
                {
                    String str = new String(read, 0, count);
                    outStr += str;
                    count = sr.Read(read, 0, 256);
                }
            }
            catch (System.Net.WebException exception)
            {
                Console.WriteLine("This program is expected to throw WebException on successful run." +
                        "\n\nException Message :" + exception.Message);
                if (exception.Status == System.Net.WebExceptionStatus.ProtocolError)
                {
                    Console.WriteLine("Status Code : {0}", ((System.Net.HttpWebResponse)exception.Response).StatusCode);
                    Console.WriteLine("Status Description : {0}", ((System.Net.HttpWebResponse)exception.Response).StatusDescription);
                    throw;
                }
            }
            return outStr;
        }

        public string ReturnToken()
        {
            return authToken;
        }
    }
}
