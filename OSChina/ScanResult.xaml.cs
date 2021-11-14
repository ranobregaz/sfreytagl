using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Navigation;
using WP7_JsonLib;
using OSChina.Model;
using cn.blu10ph.wp.HttpHelper;

namespace OSChina
{
    public partial class ScanResult : WP7_ControlsLib.Controls.ProgressTrayPage
    {
        public ScanResult()
        {
            System.Diagnostics.Debug.WriteLine("Page_Init");
            InitializeComponent();
            this.qrType.Text = "";
            this.qrTitle.Text = "";
            this.qrRequireLogin.Text = "";
            this.qrText.Text = "";
            this.qrText.Visibility = System.Windows.Visibility.Collapsed;
            this.QRType2Str[0] = "未知二维码";
            this.QRType2Str[1] = "活动";
        }
        private bool isOSChinaQRCode = false;
        private string qrCodeString = string.Empty;
        private QRCodeInfo qrInfo = null;
        private string[] QRType2Str = new string[2];

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Page_To");
            if (NavigationContext.QueryString.TryGetValue("result", out qrCodeString))
            {
                try
                {
                    this.qrInfo = JsonUtil.JsonDeSerialize<QRCodeInfo>(qrCodeString);
                    if (this.qrInfo != null && this.qrInfo.url != null && this.qrInfo.title != null)
                    {
                        this.qrType.Text = this.QRType2Str[(this.qrInfo.type >= this.QRType2Str.Length ? 0 : this.qrInfo.type)];
                        this.qrTitle.Text = this.qrInfo.title;
                        this.qrRequireLogin.Text = (this.qrInfo.require_login ? "" : "不") + "需要登录";
                        this.isOSChinaQRCode = true;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("反序列化错误:" + ex.Message);
                    this.qrTitle.Text = "二维码信息:";
                    this.qrText.Text = string.IsNullOrEmpty(this.qrCodeString)?string.Empty:this.qrCodeString;
                    this.qrText.Visibility = System.Windows.Visibility.Visible;
                    this.submitQRInfo.Content = "全部复制";
                }
            }
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Page_From");
            if (this.isOSChinaQRCode)
            {
                var lastPage = NavigationService.BackStack.FirstOrDefault();
                if (lastPage != null && lastPage.Source.ToString().Contains("/ScanCode.xaml"))
                {
                    NavigationService.RemoveBackEntry();
                }
            }
            base.OnNavigatingFrom(e);
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Page_Loaded");
            if (this.isOSChinaQRCode)
            {
                this.processingCodeInfo();
            }
            else
            {
                MessageBox.Show("数据校验异常,请确认二维码来源为开源中国!");
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

            if (this.qrInfo.require_login)
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
            HttpPostHelper client = Tool.SendPostClientByHttpWebRequest(this.qrInfo.url, parameters);
            client.PostCompleted += (s, e1) =>
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

        private void processingCodeInfo()
        {
            System.Diagnostics.Debug.WriteLine("Info_Submit");
            if (this.isOSChinaQRCode)
            {
                switch (this.qrInfo.type)
                {
                    case 1:
                        this.ActionSingQRCode();
                        break;
                    //case 2: result = OtherOSChinaQRCode(qrinfo); break;
                    default:
                        submitQRInfo.IsEnabled = false;
                        submitQRInfo.Content = "提交";
                        break;
                }
            }
            else
            {
                Tool.Copy2Clipboard(this.qrCodeString);
            }
        }

        private void submitQRInfo_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Info_Click");
            this.processingCodeInfo();
        }
    }
}