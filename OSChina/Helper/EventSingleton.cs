﻿/*
 * 原作者: 王俊
 * 
 */
using System;
using System. Text;
using System. Windows. Media. Imaging;
using Coding4Fun. Phone. Controls;
using OSChina. Model;
using OSChina. Model. AppOnly;

namespace OSChina
{
    public sealed class EventSingleton
    {
        #region Singleton
        private static readonly EventSingleton instance = new EventSingleton( );
        public static EventSingleton Instance { get { return instance; } }
        private EventSingleton( ) { }
        #endregion

        #region UserNotice事件通知处理
        /// <summary>
        /// 获取用户通知事件
        /// </summary>
        public event EventHandler<TagEventArgs> OnGetUserNotice;

        /// <summary>
        /// 主动触发获取用户通知事件
        /// </summary>
        /// <param name="notice">用户通知</param>
        public void RaiseGetUserNotice( UserNotice notice )
        {
            if ( Config.IsLogin == false )
            {
                return;
            }
            if ( this.OnGetUserNotice != null )
            {
                this. OnGetUserNotice( this, new TagEventArgs { Tag = notice } );
            }
            //Toast通知
            UserNotice inPhone = Config. UserNoticeInPhone;
            if ( notice.atMeCount > inPhone.atMeCount || 
                  notice.reviewCount > inPhone.reviewCount ||
                  notice.msgCount > inPhone.msgCount)
            {
                StringBuilder sb = new StringBuilder( );
                sb. Append( "        " );
                if ( notice.atMeCount > 0 )
                {
                    sb. AppendFormat( "{0} 条提到我", notice. atMeCount );
                }
                if ( notice.reviewCount > 0 )
                {
                    sb. AppendFormat( "  {0} 条评论", notice. reviewCount );
                }
                if ( notice.msgCount > 0 )
                {
                    sb. AppendFormat( "  {0} 条留言", notice. msgCount );
                }
                ToastPrompt toast = ToastMessage( "您有新的消息", sb. ToString( ) );
                if ( toast != null )
                {
                    //注意进入 ActivesPage 后，该显示哪一个我们根据 Config.UserNoticeInPhone 来决定
                    toast. Tap += (s, e) =>
                        {
                            if ( notice.atMeCount > 0 )
                            {
                                Tool. To( string. Format( "/ActivesPage.xaml?catalog={0}", ( int ) ActiveType. AtMe ) );
                            }
                                //如果是留言类型通知
                            else if ( notice.msgCount > 0 )
                            {
                                Tool. To( "/MsgsPage.xaml" );
                            }
                            else if ( notice.reviewCount > 0 )
                            {
                                Tool. To( string. Format( "/ActivesPage.xaml?catalog={0}", ( int ) ActiveType. Comment ) );
                            }
                        };
                }
            }
            Config. UserNoticeInPhone = notice;
        }
        #endregion

        #region 登陆/注销事件通知处理
        /// <summary>
        /// 登录或者注销事件
        /// </summary>
        public event EventHandler<TagEventArgs> OnLoginOrLogout;

        /// <summary>
        /// 主动触发登录或触发事件
        /// </summary>
        public void RaiseLoginOrLogout( )
        {
            if ( this.OnLoginOrLogout != null )
            {
                this. OnLoginOrLogout( this, new TagEventArgs { Tag = Config. IsLogin } );
            }
        }
        #endregion

        #region 修改头像后通知
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler OnUpdatePortrait;
        public void RaiseUpdatePortrait( )
        {
            if ( this.OnUpdatePortrait != null )
            {
                this. OnUpdatePortrait( this, null );
            }
        }
        #endregion

        #region Toast消息通知
        /// <summary>
        /// 顶部弹出消息
        /// </summary>
        /// <param name="title">消息标题</param>
        /// <param name="message">消息主要内容</param>
        /// <returns>ToastPrompt引用</returns>
        public ToastPrompt ToastMessage( string title, string message)
        {
            ToastPrompt toast = new ToastPrompt( );
            toast. FontSize = 19;
            toast. Title = title;
            toast. Message = message;
            if ( title. IsNullOrWhitespace( ) )
            {
                toast. TextOrientation = System. Windows. Controls. Orientation. Horizontal;
            }
            else
            {
                toast. TextOrientation = System. Windows. Controls. Orientation. Vertical;
            }
            toast. ImageSource = new BitmapImage( new Uri( "/Resource/ToastIcon.png", UriKind. RelativeOrAbsolute ) );
            toast. Show( );
            return toast;
        }

        #endregion

        #region 表情选取事件
        /// <summary>
        /// 选取表情事件
        /// </summary>
        public event EventHandler<TagEventArgs> OnNewFace;

        /// <summary>
        /// 主动触发事件 选择了一个表情
        /// </summary>
        /// <param name="faceString">表情对应字符串</param>
        public void RaiseNewFace( string faceString )
        {
            if ( this.OnNewFace != null )
            {
                this. OnNewFace( this, new TagEventArgs { Tag = faceString } );
            }
        }
        #endregion

    }
}
