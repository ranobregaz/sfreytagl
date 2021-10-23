/*
 * 原作者: 王俊
 * 
 */
using System;
using System. Collections. Generic;
using System. Collections. ObjectModel;
using System. Windows;
using System. Windows. Controls;
using System. Windows. Media;
using OSChina. Controls;
using WP7_ControlsLib. Controls;
using System.Windows.Input;

namespace OSChina
{
    public class ListBoxHelper
    {
        public bool isLoadOver { get; set; }
        public bool isLoading { get; set; }
        public ObservableCollection<object> datas { get; set; }
        private ScrollViewer listBox_ScrollViewer;
        private double lastListBoxScrollableVerticalOffset = 0;
        private double actuableOffset, validStartOffset;
        private bool mplStarted, canRefresh, isPullRefresh;
        public int allCount;
        //刷新验证专用
        public List<int> ids4Refresh = new List<int>();

        private ListBox listBox { get; set; }

        public ListBoxHelper(ListBox listBox, Color foreground, bool isPullRefresh = false, bool isNeedFirstTip = true)
        {
            this.listBox = listBox;
            this.datas = new ObservableCollection<object>();
            if (isNeedFirstTip)
            {
                LoadNextTip tip = this.GetLoadTip;
                tip.Foreground = new SolidColorBrush(foreground);
                this.datas.Add(tip);
            }
            this.listBox.ItemsSource = this.datas;
            this.loadNextTip.Click += (s, e) =>
            {
                this.Reload();
            };
            //滚动检测
            this.listBox.LayoutUpdated += new EventHandler(listBox_LayoutUpdated);
            if (isPullRefresh)
            {
                this.isPullRefresh = isPullRefresh;
                this.listBox.AddHandler(ListBox.ManipulationStartedEvent, new EventHandler<ManipulationStartedEventArgs>(ListBox_ManipulationStarted), true);
                this.listBox.AddHandler(ListBox.ManipulationCompletedEvent, new EventHandler<ManipulationCompletedEventArgs>(ListBox_ManipulationCompleted), true);
            }
        }

        private void ListBox_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            this.mplStarted = true;
        }

        void listBox_LayoutUpdated(object sender, EventArgs e)
        {
            if (this.listBox_ScrollViewer != null && isLoadOver == false)
            {

                System.Diagnostics.Debug.WriteLine("LayoutUpdated:    VerticalOffset:" + this.listBox_ScrollViewer.VerticalOffset + ";    ScrollableHeight:" + this.listBox_ScrollViewer.ScrollableHeight + ";");
                //是否往下滑动
                bool isGoingDown = this.listBox_ScrollViewer.VerticalOffset > this.lastListBoxScrollableVerticalOffset;
                this.lastListBoxScrollableVerticalOffset = this.listBox_ScrollViewer.VerticalOffset;
                //是否在监控的最底部区域
                bool isInReloadRegion = this.listBox_ScrollViewer.VerticalOffset >= (this.listBox_ScrollViewer.ScrollableHeight - Config.ScrollBarOffset);
                if (isInReloadRegion && isGoingDown)
                {
                    this.Reload();
                }
            }
            else
            {
                this.listBox_ScrollViewer = ControlHelper.FindChildOfType<ScrollViewer>(this.listBox);
                if (this.isPullRefresh)
                {
                    this.listBox_ScrollViewer.MouseMove += new MouseEventHandler(listBox_ScrollViewer_MouseMove);
                }
            }
        }

        void listBox_ScrollViewer_MouseMove(object sender, MouseEventArgs e)
        {
            if (Math.Round(this.listBox_ScrollViewer.VerticalOffset, 0) == 0)
            {
                if (this.mplStarted)
                {
                    this.mplStarted = false;
                    this.canRefresh = false;
                    this.validStartOffset = e.GetPosition(null).Y;
                }
                this.actuableOffset = e.GetPosition(null).Y - this.validStartOffset;
                if (this.actuableOffset > Config.PullDownOffset)
                {
                    this.canRefresh = true;
                }
                System.Diagnostics.Debug.WriteLine("MouseMove:    actuableOffset:" + this.actuableOffset + ";    VerticalOffset:" + this.validStartOffset + ";    ScrollViewer_VerticalOffset:" + this.listBox_ScrollViewer.VerticalOffset + ";");
            }
        }

        private void ListBox_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("ListBox_ManipulationCompleted    actuableOffset:" + actuableOffset + ";    ScrollViewer VerticalOffset:" + this.listBox_ScrollViewer.VerticalOffset + ";");
            if (this.canRefresh)
            {
                System.Diagnostics.Debug.WriteLine("Head pull bingo!");
                this.canRefresh = false;
                Refresh();
            }
        }

        /// <summary>
        /// 清空
        /// </summary>
        public void Clear()
        {
            this.ids4Refresh.Clear();
            this.datas.Clear();
            this.ReloadDelegate = null;
        }

        /// <summary>
        /// 刷新操作
        /// </summary>
        public void Refresh( )
        {
            isLoading = false;
            isLoadOver = false;
            this. datas. Clear( );
            this. datas. Add( this. GetLoadTip );
            allCount = 0;
            this. Reload( );
        }

        public event Action ReloadDelegate;
        private void Reload()
        {
            if (this.ReloadDelegate != null)
            {
                this.ReloadDelegate();
            }
        }

        /// <summary>
        /// 获取最下面的继续加载提示符
        /// </summary>
        public LoadNextTip GetLoadTip
        {
            get { return isLoadOver ? this. loadOverTip : this. loadNextTip; }
        }
        private LoadNextTip loadNextTip = new LoadNextTip
        {
            Text = "正在刷新",
            HorizontalAlignment = HorizontalAlignment. Center,
            HorizontalContentAlignment = HorizontalAlignment. Center,
            VerticalContentAlignment = System.Windows.VerticalAlignment.Top,
            Width = 480,
            MinWidth = 480,
            Height = 100, 
            MinHeight = 100,
        };
        private LoadNextTip loadOverTip = new LoadNextTip
        {
            Text = "已经加载所有项",
            HorizontalAlignment = HorizontalAlignment. Center,
            HorizontalContentAlignment = HorizontalAlignment. Center,
            VerticalContentAlignment = System. Windows. VerticalAlignment. Top,
            Width = 480,
            MinWidth = 480,
            Height = 100,
            MinHeight = 100,
        };
    }
}
