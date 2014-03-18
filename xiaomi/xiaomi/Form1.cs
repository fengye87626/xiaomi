using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Linq;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Web;
namespace xiaomi
{
    public partial class Form1 : Form
    {
        private static System.Timers.Timer _queuetimer;
        private static string cookies = string.Empty;      //公有Cookie
        private static string codeCookie = string.Empty;
        public Form1()
        {
            InitializeComponent();
            txtUserName.Text = ConfigurationManager.AppSettings["userName"].ToString();
            txtPwd.Text = ConfigurationManager.AppSettings["userPwd"].ToString();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                int t = Convert.ToInt32(txtMilliseconds.Text);
                TimerStart(t);
            }
            else
            {
                QiangGou();
            }

            btnStart.Enabled = false;
            btnStop.Enabled = true;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            TimerStop();
            btnStart.Enabled = true;
            btnStop.Enabled = false;
        }

        public void TimerStart(int Interval)
        {
            if (_queuetimer == null)
            {
                _queuetimer = new System.Timers.Timer();
            }
            else
            {
                _queuetimer.Close(); _queuetimer = new System.Timers.Timer();
            }



            _queuetimer.Interval = Interval;
            _queuetimer.Elapsed += (sender, e) => _queuetimer_Elapsed(sender, e);

            _queuetimer.AutoReset = true;
            _queuetimer.Enabled = true;
        }


        public void TimerStop()
        {
            if (_queuetimer != null)
            {
                _queuetimer.Enabled = false;
                _queuetimer.Stop();
                _queuetimer.Close();
            }

        }

        void _queuetimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Parallel.Invoke(CreateTaskArray(10, QiangGou));
        }

        /// <summary>
        /// 创建多个任务
        /// </summary>
        /// <param name="taskCount"></param>
        /// <returns></returns>
        private static Action[] CreateTaskArray(int taskCount, Action Dequeue)
        {
            var actions = new Action[taskCount];
            for (int i = 0; i < taskCount; i++)
            {
                actions[i] = Dequeue;
            }
            return actions;
        }

        public void QiangGou()
        {

            //AppendText(cookies);//打印
            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                URL = "http://tc.hd.xiaomi.com/hdget?callback=hdcontrol",
                UserAgent = "Mozilla/5.0 (Linux; U; Android 4.0.4; zh-cn; MI-ONE C1 Build/IMM76D) UC AppleWebKit/534.31 (KHTML, like Gecko) Mobile Safari/534.31",
                Host = "tc.hd.xiaomi.com",
                Cookie = cookies
            };
            HttpResult result = http.GetHtml(item);
            string strJson = result.Html;
            if (strJson.Contains("hdcontrol"))
            {
                strJson = strJson.Replace("hdcontrol(", "").Replace(")", "");
                xiaomiEntity xm = JsonConvert.DeserializeObject<xiaomiEntity>(strJson);
                bool allow = xm.Status.Allow;
                string Hdurl = xm.Status.Miphone.Hdurl;

                if (!string.IsNullOrEmpty(Hdurl))
                {
                    string url = "http://t.hd.xiaomi.com/s/" + xm.Status.Miphone.Hdurl + "&_m=1";
                    if (allow)
                    {
                        lblAllow.Invoke(new Action(delegate() { lblAllow.Text = allow.ToString(); }));
                        txtUrl.Invoke(new Action(delegate() { txtUrl.Text = url; }));

                        TimerStop();
                    }
                    else
                    {
                        bool allowchecked = false;
                        ckbAllow.Invoke(new Action(delegate() { allowchecked = ckbAllow.Checked; }));
                        if (allowchecked == true)
                        {
                            txtUrl.Invoke(new Action(delegate() { txtUrl.Text = url; }));

                        }
                    }
                }
            }

            else
            {
                lblAllow.Invoke(new Action(delegate() { lblAllow.Text = "尚未开放"; }));
            }





        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUserName.Text;
            string password = txtPwd.Text;

            HttpItem itemSign = new HttpItem()
            {
                URL = "https://account.xiaomi.com/pass/serviceLogin",
            };
            HttpHelper helperSign = new HttpHelper();
            HttpResult resultSign = helperSign.GetHtml(itemSign);
            string signHtml = resultSign.Html;
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(signHtml);
            var htmlnodes = doc.DocumentNode.Descendants("input");
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (var item in htmlnodes)
            {
                if (item.Attributes["name"] != null && item.Attributes["value"] != null)
                {
                    dict.Add(item.Attributes["name"].Value, item.Attributes["value"].Value);
                }
            }
            string passToken = HttpUtility.UrlEncode(dict["passToken"]);
            string callback = HttpUtility.UrlEncode(dict["callback"]);
            string sid = HttpUtility.UrlEncode(dict["sid"]);
            string qs = HttpUtility.UrlEncode(dict["qs"]);
            string hidden = HttpUtility.UrlEncode(dict["hidden"]);
            string _sign = HttpUtility.UrlEncode(dict["_sign"]);
            string auto = HttpUtility.UrlEncode(dict["auto"]);

            cookies = resultSign.Cookie;

            HttpItem itemLogin = new HttpItem()         //登陆Post
            {
                URL = "https://account.xiaomi.com/pass/serviceLoginAuth2",
                Method = "POST",
                Cookie = cookies,
                Referer = "https://account.xiaomi.com/pass/serviceLogin",
                ContentType = "application/x-www-form-urlencoded",
                Postdata = string.Format("passToken={0}&user={1}&pwd={2}&callback={3}&sid={4}&qs={5}&hidden={6}&_sign={7}&auto={8}", passToken, username, password, callback, sid, qs, hidden, _sign, auto)
            };
            HttpHelper helperLogin = new HttpHelper();
            HttpResult resultLogin = helperLogin.GetHtml(itemLogin);

            if (resultLogin.Html.Contains("小米帐户 - 登录"))
            {
                AppendText(username + "登陆失败\n"); return;
            }
            AppendText(username + "登陆成功");
            cookies += ";" + resultLogin.Cookie;
            // AppendText(cookies);

        }





        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtUrl.Text))
            {
                Clipboard.SetDataObject(txtUrl.Text, true);
            }
        }


        protected void AppendText(string info)
        {
            txtInfo.Invoke((MethodInvoker)delegate
            {
                txtInfo.AppendText(info + Environment.NewLine);

                txtInfo.SelectionStart = txtInfo.Text.Length;
                txtInfo.ScrollToCaret();
            });
        }

    }
}
