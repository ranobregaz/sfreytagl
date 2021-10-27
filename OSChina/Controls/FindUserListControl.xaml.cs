/*
 * 原作者: 王俊
 * 
 */
using System;
using System. Collections. Generic;
using System. Linq;
using System. Net;
using System. Windows;
using System. Windows. Controls;
using System. Windows. Media. Imaging;
using OSChina. Model;

namespace OSChina. Controls
{
    public partial class FindUserListControl : UserControl
    {
        #region Properties
        /// <summary>
        /// 是粉丝列表 还是 我关注的 列表
        /// </summary>
        public string SearchName { get; set; }

        /// <summary>
        /// ListBox辅助对象
        /// </summary>
        private ListBoxHelper listBoxHelper { get; set; }
        #endregion

        #region Construct
        /// <summary>
        /// Construct
        /// </summary>
        public FindUserListControl( )
        {
            InitializeComponent( );
            this. Loaded += (s, e) =>
                {
                    this.listBoxHelper = new ListBoxHelper(this.list_Fans, System.Windows.Media.Colors.Transparent, false, false);
                    this. listBoxHelper. ReloadDelegate += new Action( listBoxHelper_ReloadDelegate );
                };
            
            this. Unloaded += (s, e) =>
            {
                this. listBoxHelper. Clear( );
            };
        }

        #endregion

        #region Public functions
        /// <summary>
        /// 继续加载好友列表
        /// </summary>
        public void listBoxHelper_ReloadDelegate( )
        {
            if ( this.listBoxHelper == null )
            {
                return;
            }
            if (this.listBoxHelper.isLoading == true)
            {
                return;
            }
            if ("".Equals(this.SearchName) || null == this.SearchName)
            {
                return;
            }
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                {"name",  SearchName},
                {"pageIndex", (this.listBoxHelper.allCount / 20).ToString()},
                {"pageSize", "20"},
            };
            WebClient client = Tool. SendWebClient( Config.api_find_user, parameters );
            this. listBoxHelper. isLoading = true;
            client. DownloadStringCompleted += (s, e) =>
            {
                this. listBoxHelper. isLoading = false;
                if ( e. Error != null )
                {
                    System. Diagnostics. Debug. WriteLine( "获取 FriendsList 网络错误: {0}", e. Error. Message );
                    return;
                }
                FriendUnit[] findUsers = Tool.GetFindUserList(e.Result);
                if ( findUsers != null )
                {
                    this. listBoxHelper. allCount += findUsers. Length;
                    this. listBoxHelper. isLoadOver = findUsers. Length < 20;
                    //筛选
                    int[ ] ids = Tool. GetIDS_ID<FriendUnit>( this. listBoxHelper. datas );
                    findUsers = findUsers. Where( f => ids. Contains( f. id ) == false ). ToArray( );

                    //去除最后的提示文字
                    if ( this. listBoxHelper. datas. Count > 3 )
                    {
                        for ( int i = 0 ; i < 3 ; i++ )
                        {
                            this. listBoxHelper. datas. RemoveAt( this. listBoxHelper. datas. Count - 1 );
                        }
                    }
                    //添加刚才的元素
                    findUsers. ForAll( f => this. listBoxHelper. datas. Add( f ) );
                    //添加最后的提示文字
                    if ( this. listBoxHelper. isLoadOver == false )
                    {
                        this. listBoxHelper. datas. Add( new TextBlock { Visibility = System.Windows.Visibility.Collapsed } );
                        this. listBoxHelper. datas. Add( new TextBlock { Text = "    正 在", FontSize=30, HorizontalAlignment = System. Windows. HorizontalAlignment. Right } );
                        this. listBoxHelper. datas. Add( new TextBlock { Text = "刷 新", FontSize=30, HorizontalAlignment = System. Windows. HorizontalAlignment. Left } );
                    }
                }
            };
        }

        public void Refresh()
        {
            //清空数据
            this.listBoxHelper.isLoading = false;
            this.listBoxHelper.isLoadOver = false;
            this.listBoxHelper.datas.Clear();
            this.listBoxHelper.allCount = 0;
        }

        #endregion

        #region Private functions
        /// <summary>
        /// 点击某个好友进入他的用户专页
        /// </summary>
        private void list_Fans_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FriendUnit f = this. list_Fans. SelectedItem as FriendUnit;
            this. list_Fans. SelectedItem = null;
            if ( f != null )
            {
                Tool. ToUserPage( f. id );
            }
        }

        /// <summary>
        /// 好友头像加载失败 则使用默认头像
        /// </summary>
        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            Image img = sender as Image;
            FriendUnit f = img. DataContext as FriendUnit;
            if ( f != null )
            {
                img. Source = new BitmapImage( new Uri( "/Resource/big_avatar.png", UriKind. Relative ) );
            }
        }
        
        #endregion
    }
}
