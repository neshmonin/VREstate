using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace HttpRequestTester
{
    public partial class Form1 : Form
    {
        private int _requestTimeoutSec = 600;
        private byte[] _rqData = null;

        private Dictionary<string, string> _respHeaders = new Dictionary<string, string>();
        private Dictionary<string, string> _respProperties = new Dictionary<string, string>();
        private Dictionary<string, string> _respCookies = new Dictionary<string, string>();

        private static string collectionToString(Dictionary<string, string> coll, bool list)
        {
            StringBuilder result = new StringBuilder();
            bool delim = false;

            foreach (KeyValuePair<string, string> kvp in coll)
            {
                if (delim) result.Append(list ? "\r\n" : "; ");
                result.AppendFormat("{0}: {1}", kvp.Key, kvp.Value);
                delim = true;
            }

            return result.ToString();
        }

        private static ICollection<Control> disableControls(Control root)
        {
            ICollection<Control> controls = new List<Control>();
            disableControls(root, ref controls);
            return controls;
        }

        private static void disableControls(Control root, ref ICollection<Control> controls)
        {
            foreach (Control c in root.Controls)
            {
                if (c.Enabled)
                {
                    c.Enabled = false;
                    controls.Add(c);
                }
                disableControls(c, ref controls);
            }
        }

        private static void enableControls(ICollection<Control> controls)
        {
            foreach (Control c in controls) c.Enabled = true;
        }

        private void showCollection(Dictionary<string, string> coll, TextBox control)
        {
            control.Text = collectionToString(coll, true);
            control.Visible = true;
            control.Focus();
            control.Capture = true;

            Size size = TextRenderer.MeasureText(control.Text, control.Font);
            control.Height = size.Height;
        }

        private void selectorControl_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ('\x0D' == e.KeyChar)
            {
                btnQuery_Click(sender, e);
                e.Handled = true;
            }
        }

        private void popupTextBox_Leave(object sender, EventArgs e)
        {
            Control ctl = sender as Control;
            if (ctl != null) ctl.Visible = false;
        }

        private void popupTextBox_MouseClick(object sender, MouseEventArgs e)
        {
            Control ctl = sender as Control;
            if (ctl != null)
            {
                if (!ctl.Bounds.Contains(e.Location))
                {
                    ctl.Capture = false;
                    ctl.Visible = false;
                }
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            Cursor saved = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;
            ICollection<Control> disabled = disableControls(this);

            string body = string.Empty;

            try
            {
                string requestUri = cbxTarget.Text;
                if (!requestUri.EndsWith("/")) requestUri += "/";

                requestUri += cbxService.Text;
                if (requestUri.EndsWith("/")) requestUri = requestUri.Substring(0, requestUri.Length - 1);

                if (!string.IsNullOrWhiteSpace(tbQuery.Text))
                    requestUri += "?" + tbQuery.Text.Trim();

                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(requestUri);
                request.Timeout = _requestTimeoutSec * 1000;

                request.Method = cbxRequestType.Text;

                if (!string.IsNullOrWhiteSpace(tbSID.Text))
                    request.Headers.Add("sid", tbSID.Text.Trim());

                if (!string.IsNullOrWhiteSpace(cbxBodyType.Text))
                {
                    request.ContentType = cbxBodyType.Text;
                    Stream output = request.GetRequestStream();
                    using (StreamWriter w = new StreamWriter(output))
                        if (_rqData != null)
                        {
                            w.Write(_rqData);
                            //request.ContentLength = _rqData.Length;
                        }
                        else
                        {
                            w.Write(tbBodyText.Text);
                            //request.ContentLength = tbBodyText.Text.Length;
                        }
                }

                HttpWebResponse response;

                try
                {
                    response = request.GetResponse() as HttpWebResponse;
                }
                catch (WebException ex)
                {
                    response = ex.Response as HttpWebResponse;
                }
                if (null == response) throw new InvalidOperationException("Http request returned invalid result.");

                lblHttpResponseCode.Text = response.StatusCode.ToString() + ": " + response.StatusDescription;
                lblResponseContentType.Text = response.ContentType;

                Stream input = response.GetResponseStream();
                using (StreamReader r = new StreamReader(input))
                    body = r.ReadToEnd();

                _respHeaders.Clear();
                foreach (string key in response.Headers.Keys)
                    _respHeaders.Add(key, response.Headers[key]);

                _respProperties.Clear();
                _respProperties.Add("charset", response.CharacterSet);
                _respProperties.Add("encoding", response.ContentEncoding);
                _respProperties.Add("length", response.ContentLength.ToString());
                _respProperties.Add("type", response.ContentType);
                _respProperties.Add("fromCache", response.IsFromCache.ToString());
                _respProperties.Add("isMutuallyAuth'd", response.IsMutuallyAuthenticated.ToString());
                _respProperties.Add("lastModified", response.LastModified.ToString());
                _respProperties.Add("method", response.Method);
                _respProperties.Add("protoVer", response.ProtocolVersion.ToString());
                _respProperties.Add("respUri", response.ResponseUri.ToString());
                _respProperties.Add("server", response.Server);

                _respCookies.Clear();
                foreach (var c in response.Cookies)
                    _respCookies.Add(c.ToString(), "");

                lblHeaders.Text = collectionToString(_respHeaders, false);
                lblProperties.Text = collectionToString(_respProperties, false);
                lblRespCookies.Text = collectionToString(_respCookies, false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                enableControls(disabled);
                Cursor.Current = saved;
            }

            tbResponseText.Enabled = false;
            tbResponseText.Text = "...";
            tbResponseText.Cursor = Cursors.WaitCursor;
            Application.DoEvents();

            tbResponseText.Text = body;  // <-- THIS IS SLOW!!!
            tbResponseText.Enabled = true;
            tbResponseText.Cursor = Cursors.Default;
        }

        private void lblHeaders_Click(object sender, EventArgs e)
        {
            showCollection(_respHeaders, tbRespHeaders);
        }

        private void lblProperties_Click(object sender, EventArgs e)
        {
            showCollection(_respProperties, tbRespProperties);
        }

        private void lblRespCookies_MouseClick(object sender, MouseEventArgs e)
        {
            showCollection(_respCookies, tbRespCookies);
        }

        private void btn3DCX_Click(object sender, EventArgs e)
        {
            mnu3DCX.Show(btn3DCX, btn3DCX.Size.Width, 0);
        }

        private void miWebLogin_Click(object sender, EventArgs e)
        {
            Cursor saved = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;
            ICollection<Control> disabled = disableControls(this);

            try
            {
                string requestUri = cbxTarget.Text;
                if (!requestUri.EndsWith("/")) requestUri += "/";

                requestUri += "program?q=login&role=visitor&uid=web&pwd=web";
                if (requestUri.EndsWith("/")) requestUri = requestUri.Substring(0, requestUri.Length - 1);

                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(requestUri);
                request.Timeout = _requestTimeoutSec * 1000;

                request.Method = "GET";

                HttpWebResponse response;

                try
                {
                    response = request.GetResponse() as HttpWebResponse;
                }
                catch (WebException ex)
                {
                    response = ex.Response as HttpWebResponse;
                }
                if (null == response) throw new InvalidOperationException("Http request returned invalid result.");

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string body;
                    Stream input = response.GetResponseStream();
                    using (StreamReader r = new StreamReader(input))
                        body = r.ReadToEnd();

                    int idx = body.IndexOf("\"sid\":\"");
                    if (idx > 0)
                    {
                        int idx2 = body.IndexOf("\"", idx + 7);
                        tbSID.Text = body.Substring(idx + 7, idx2 - idx - 7);
                    }
                }
                else
                {
                    MessageBox.Show(response.StatusCode.ToString() + ": " + response.StatusDescription);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                enableControls(disabled);
                Cursor.Current = saved;
            }
        }
    }
}
