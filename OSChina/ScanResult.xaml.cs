using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Windows.Navigation;
using WP7_JsonLib;
using OSChina.Model;
using WP7_WebLib.HttpPost;

namespace OSChina
{
    public partial class ScanResult : WP7_ControlsLib.Controls.ProgressTrayPage
    {
        public ScanResult()
        {
            System.Diagnostics.Debug.WriteLine("Page_Init");
            InitializeComponent();
            qrType.Text = "";
            qrTitle.Text = "";
            qrRequireLogin.Text = "";
            QRType2Str[0] = "未知";
            QRType2Str[1] = "活动";
        }
        bool isLoadSuccess = false;
        private string qrcodestring = string.Empty;
        QRCodeInfo qrinfo = null;
        string[] QRType2Str = new string[2];

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Page_To");
            if (NavigationContext.QueryString.TryGetValue("result", out qrcodestring))
            {
                try
                {
                    qrinfo = JsonUtil.JsonDeSerialize<QRCodeInfo>(qrcodestring);
                    if (qrinfo != null && qrinfo.url != null && qrinfo.title != null)
                    {
                        qrType.Text = QRType2Str[(qrinfo.type >= QRType2Str.Length ? 0 : qrinfo.type)];
                        qrTitle.Text = qrinfo.title;
                        qrRequireLogin.Text = (qrinfo.require_login ? "" : "不") + "需要登录";
                        isLoadSuccess = true;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("反序列化错误:" + ex.Message);
                    MessageBox.Show("解码失败,请确认二维码来源为开源中国!");
                }
            }
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Page_From");
            var lastPage = NavigationService.BackStack.FirstOrDefault();
            if (lastPage != null && lastPage.Source.ToString().Contains("/ScanCode.xaml"))
            {
                NavigationService.RemoveBackEntry();
            }

            base.OnNavigatingFrom(e);
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Page_Loaded");
            if (isLoadSuccess)
            {
                SubmitInfo2OSChina();
            }
            else
            {
                MessageBox.Show("数据校验异常,请确认二维码来源为开源中国!");
                this.IsEnabled = false;
                base.NavigationService.GoBack();
            }
        }

        private void ActionSingQRCode()
        {
            int flg = 0;
            System.Diagnostics.Debug.WriteLine("Info_ActionSing");
            submitQRInfo.IsEnabled = false;
            submitQRInfo.Content = "提交中...";
            this.LoadingText = "正在提交";
            this.ProgressIndicatorIsVisible = true;
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            
            if (qrinfo.require_login)
            {
                if (Tool.CheckLogin("请登陆后再进行提交操作", "温馨提示") == false)
                {
                    submitQRInfo.IsEnabled = true;
                    submitQRInfo.Content = "重新提交";
                    this.ProgressIndicatorIsVisible = false;
                    return;
                }
                parameters.Add("Cookie",Config.Cookie);
            }
            PostClient client = Tool.SendPostClient(qrinfo.url,parameters);
            client.DownloadStringCompleted += (s, e1) =>
            {
                this.ProgressIndicatorIsVisible = false;
                if (e1.Error != null)
                {
                    System.Diagnostics.Debug.WriteLine("提交签到时网络错误: {0}", e1.Error.Message);
                    MessageBox.Show("提交签到时网络错误,请重试!");
                }
                else
                {
                    ActionSing result = null;
                    try
                    {
                        result = JsonUtil.JsonDeSerialize<ActionSing>(e1.Result);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("反序列化错误:" + ex.Message);
                    }
                    if (result == null)
                    {
                        MessageBox.Show("获取返回信息失败,请重试!");
                    }
                    else
					{
                        if (!string.IsNullOrEmpty(result.error))
                        {
                            MessageBox.Show(result.error);
                        }
                        else if (!string.IsNullOrEmpty(result.msg))
                        {
                            flg = 1;
                            MessageBoxResult showMsg = MessageBox.Show(result.msg, "提示", MessageBoxButton.OK);
                            if (showMsg == MessageBoxResult.OK)
                            {
                                if (this.NavigationService.CanGoBack)
                                {
                                    this.NavigationService.GoBack();
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("获取的返回信息不正确,请重试!");
                        }
                    }
                    if (flg == 0)
                    {
                        submitQRInfo.IsEnabled = true;
                        submitQRInfo.Content = "重新提交";
                    }
                    else
                    {
                        submitQRInfo.Content = "已提交";
                    }
                }
            };
        }

        private void SubmitInfo2OSChina()
        {
            System.Diagnostics.Debug.WriteLine("Info_Submit");
            switch (qrinfo.type)
            {
                case 1: ActionSingQRCode(); break;
                //case 2: result = OtherQRCode(qrinfo); break;
                default: 
                    submitQRInfo.IsEnabled = false;
                    submitQRInfo.Content = "提交"; 
                    break;
            }
        }

        private void submitQRInfo_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Info_Click");
            SubmitInfo2OSChina();
        }
    }
}