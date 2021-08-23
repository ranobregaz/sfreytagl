using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Devices;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using ZXing.Common;
using ZXing;
using ZXing.QrCode;

namespace OSChina
{
    public partial class ScanCode : PhoneApplicationPage
    {
        private PhotoCamera _photoCamera;
        private PhotoCameraLuminanceSource _luminance;
        private readonly DispatcherTimer _timer;
        private BarcodeReader _reader = null;
        private bool _iswork = false;

        public ScanCode()
        {
            InitializeComponent();
            _timer = new DispatcherTimer();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _timer.Interval = TimeSpan.FromMilliseconds(250);
            _timer.Tick += (o, arg) => ScanPreviewBuffer();
            _photoCamera = new PhotoCamera();
            _photoCamera.Initialized += new EventHandler<CameraOperationCompletedEventArgs>(cam_Initialized);
            _videoBrush.SetSource(_photoCamera);
            ScanCodeRectInitial();
        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            if (_photoCamera != null )
            {
                _timer.Stop();
                if (_iswork)
                {
                    _photoCamera.CancelFocus();
                    _photoCamera.Dispose();
                }
            }

            base.OnNavigatingFrom(e);
        }

        void cam_Initialized(object sender, CameraOperationCompletedEventArgs e)
        {
                System.Diagnostics.Debug.WriteLine("摄像头初始化" + (e.Succeeded ? "成功" : "失败"));
                System.Diagnostics.Debug.WriteLine(e.Exception);
                if (e.Succeeded)
                {
                    int width = Convert.ToInt32(_photoCamera.PreviewResolution.Width);
                    int height = Convert.ToInt32(_photoCamera.PreviewResolution.Height);
                    _luminance = new PhotoCameraLuminanceSource(width, height);

                    Dispatcher.BeginInvoke(() =>
                    {
                        _previewTransform.Rotation = _photoCamera.Orientation;
                        _timer.Start();
                    });
                    _photoCamera.FlashMode = FlashMode.Auto;
                    _photoCamera.Focus();
                    _reader = new BarcodeReader();
                    _reader.Options.TryHarder = true;
                    _reader.ResultFound += _bcReader_ResultFound;
                }
                else
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        MessageBox.Show("摄像头初始化失败!"); 
                        this.IsEnabled = false;
                        base.NavigationService.GoBack();
                    });
                }
        }

        private void _bcReader_ResultFound(Result result)
        {
            if (result != null)
            {
                _timer.Stop();
                ScanCodeRectSuccess();
                if (!string.IsNullOrEmpty(result.Text))
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        NavigationService.Navigate(new Uri("/ScanResult.xaml?result=" + result.Text, UriKind.Relative));
                    });
                }
            }
            else
            {
                _photoCamera.Focus();
            }
        }

        private void ScanPreviewBuffer()
        {
            _photoCamera.GetPreviewBufferY(_luminance.PreviewBufferY);
            _reader.Decode(_luminance);
            _photoCamera.Focus();
        }

        void ScanCodeRectSuccess()
        {
            Dispatcher.BeginInvoke(() =>
            {
                _marker1.Fill = new SolidColorBrush(Colors.Green);
                _marker2.Fill = new SolidColorBrush(Colors.Green);
                _marker3.Fill = new SolidColorBrush(Colors.Green);
                _marker4.Fill = new SolidColorBrush(Colors.Green);
                _marker5.Fill = new SolidColorBrush(Colors.Green);
                _marker6.Fill = new SolidColorBrush(Colors.Green);
                _marker7.Fill = new SolidColorBrush(Colors.Green);
                _marker8.Fill = new SolidColorBrush(Colors.Green);
            });
        }

        void ScanCodeRectInitial()
        {
            _marker1.Fill = new SolidColorBrush(Colors.Red);
            _marker2.Fill = new SolidColorBrush(Colors.Red);
            _marker3.Fill = new SolidColorBrush(Colors.Red);
            _marker4.Fill = new SolidColorBrush(Colors.Red);
            _marker5.Fill = new SolidColorBrush(Colors.Red);
            _marker6.Fill = new SolidColorBrush(Colors.Red);
            _marker7.Fill = new SolidColorBrush(Colors.Red);
            _marker8.Fill = new SolidColorBrush(Colors.Red);
        }
    }
}