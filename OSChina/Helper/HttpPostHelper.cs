/*
 * 原作者: blu10ph
 * 
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using OSChina.Controls;
using WP7_ControlsLib.Controls;
using System.Windows.Input;
using System.Net;
using System.IO;
using System.Text;

namespace OSChina
{
    public class HttpPostHelper
    {
        public HttpPostHelper(IDictionary<string, string> parameters)
        {
            byteArray = System.Text.Encoding.UTF8.GetBytes(HttpGetHelper.GetQueryStringByParameters("", new Dictionary<string, string>(parameters)));
        }
        public HttpPostHelper(IList<string> parameters)
        {
            StringBuilder buffer = new StringBuilder();
            int i = 0;
            foreach (string parameter in parameters)
            {
                if (i > 0)
                {
                    buffer.AppendFormat("&{0}", parameter);
                }
                else
                {
                    buffer.AppendFormat("{0}", parameter);
                }
                i++;
            }
            byteArray = System.Text.Encoding.UTF8.GetBytes(buffer.ToString());
        }
        public HttpPostHelper(string parameters)
        {
            byteArray = System.Text.Encoding.UTF8.GetBytes(parameters);
        }

        public string UserAgent { get; set; }

        public event DownloadStringCompletedHandler DownloadStringCompleted;
        public event Action<string> OnGetCookie;

        private byte[] byteArray;

        public void DownloadStringAsync(Uri address, string cookie = null)
        {
            HttpWebRequest request = WebRequest.Create(address) as HttpWebRequest;
            request.Method = "post";
            request.ContentType = "application/x-www-form-urlencoded";
            request.UserAgent = UserAgent;
            if (cookie == string.Empty)
            {
                request.CookieContainer = new CookieContainer();
            }
            else
            {
                request.Headers["Cookie"] = cookie;
            }
            request.BeginGetRequestStream(new AsyncCallback((IAsyncResult asynchronousRequest) =>
            {
                HttpWebRequest _request = asynchronousRequest.AsyncState as HttpWebRequest;
                System.IO.Stream postStream = _request.EndGetRequestStream(asynchronousRequest);
                //POST提交服务器的资源
                postStream.Write(byteArray, 0, byteArray.Length);
                postStream.Flush();
                postStream.Close();
                request.BeginGetResponse(new AsyncCallback((IAsyncResult asynchronousResult) =>
                {
                    try
                    {
                        HttpWebRequest __request = asynchronousResult.AsyncState as HttpWebRequest;
                        HttpWebResponse response = (HttpWebResponse)__request.EndGetResponse(asynchronousResult);
                        Stream streamResponse = response.GetResponseStream();
                        StreamReader streamRead = new StreamReader(streamResponse);

                        string responseString = streamRead.ReadToEnd();
                        streamRead.Close();
                        streamResponse.Close();
                        OnGetCookie(response.Headers["Set-Cookie"]);
                        response.Close();
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            DownloadStringCompleted(this, new DownloadStringCompletedEventArgs(responseString));
                        });
                    }
                    catch (Exception ex)
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            DownloadStringCompleted(this, new DownloadStringCompletedEventArgs(ex));
                        });
                    }
                }), request);
            }), request);
        }

        public delegate void DownloadStringCompletedHandler(object sender, DownloadStringCompletedEventArgs e);
    }
    public class DownloadStringCompletedEventArgs : EventArgs
    {
        public DownloadStringCompletedEventArgs(Exception ex)
        {
            this.error = ex;
        }
        public DownloadStringCompletedEventArgs(string result)
        {
            this.result = result;
        }

        public Exception Error
        {
            get
            {
                return error;
            }
        }
        public string Result
        {
            get
            {
                return result;
            }
        }

        private Exception error = null;
        private string result = string.Empty;
    }
}
