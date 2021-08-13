wp7-app
=======
#开源中国社区 Windows Phone 客户端项目简析

直接启动 OSChina.sln，编译即可，当然请确保你的SDK是Windows Phone 7.1或以上。<br/>
如果出现编译错误，请修改 OSChina\obj\Release 目录下的如下四个文件

>LoginPage.g.i.cs
>
>PubMsgPage.g.i.cs
>
>PubPostPage.g.i.cs
>
>PubTweetPage.g.i.cs

将它们的类设置为派生自 WP7_ControlsLib. Controls. ProgressTrayPage<br/>
例如 public partial class LoginPage : Microsoft.Phone.Controls.PhoneApplicationPage 
<br/>改为
#####public partial class LoginPage : WP7_ControlsLib. Controls. ProgressTrayPage

####改了以后 不要在Visual studio 中关闭这4个C#文件，让Visual Studio 里始终保持开启着这几个文件。
<img src="http://static.oschina.net/uploads/space/2012/0831/110731_Vy3b_213217.jpg" />

####本项目采用 GPL 授权协议，欢迎大家在这个基础上进行改进，并与大家分享！作者E-mail: wangjuntom360@hotmail.com

#####对OSChina/Lib 下的几个Dll说明<br/>
Code4Fun.xxx 是来自 http://coding4fun.codeplex.com/ 上的开源项目<br/>
WP7_XXXLib 是来自美盛新创提供的Dll，所有Dll都没有混淆，反编译即可查看到C#源码

App/Config.cs
>包含应用的配置信息

Control/ActiveListControl.xaml
>动态列表控件

Control/BlogListControl.xaml
>博客列表控件

Control/CommentListControl.xaml
>评论列表控件

Control/FaceControl.xaml
>表情控件

Control/FavListControl.xaml
>收藏列表控件

Control/FriendListControl.xaml
>粉丝列表控件

Control/LoadNextTip.xaml
>列表加载更多项控件

Control/MsgListControl.xaml
>留言列表控件

Control/NewsListControl.xaml
>新闻列表控件

Control/PopUpImage.xaml
>查看动弹大图控件

Control/PostsListControl.xaml
>问答列表控件

Control/TweetListControl.xaml
>动弹列表控件

Control/TweetPortal.xaml
>用在首页的动弹页入口控件

Helper/EventSingleton.cs
>负责在整个程序中消息通知

Helper/Tool.cs
>项目辅助类

####Model 文件夹的类代表网络传输的实体对象

ActivesPage.xaml
>动态页

App.xaml
>应用程序入口页

DetailPage2.xaml
>新闻,问答,动弹,博客,软件详细内容页

FavPage.xaml
>收藏页

FeedbackPage.xaml
>关于我们页

LoginPage.xaml
>登录页

MainPage.xaml
>应用程序首页

MsgsPage.xaml
>留言列表页

PostsPage.xaml
>问答列表页

PubPostPage.xaml
>发表问答页

PubTweetPage.xaml
>发表动弹页

SearchPage.xaml
>搜索页

SettingPage.xaml
>应用设置页

TweetsPage.xaml
>动弹列表页

UserPage.xaml
>用户个人专页

WordsPage.xaml
>与某用户的会话页

####项目 OSChinaScheduledTask_Notice 
>为应用不启动时的后台线程，可以轮询获取用户的最新通知数，用户可手动关闭后台是否轮询搜索