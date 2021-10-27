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
using System. Windows. Input;
using Microsoft. Phone. Controls;
using OSChina. Model;

namespace OSChina
{
    public partial class FindUser : PhoneApplicationPage
    {
        #region Construct
        public FindUser()
        {
            InitializeComponent();
            this.resultUserList.listBoxHelper_ReloadDelegate();
        }
        #endregion

        #region Private functions
        protected override void OnNavigatedFrom(System. Windows. Navigation. NavigationEventArgs e)
        {
            GC. Collect( );
            base. OnNavigatedFrom( e );
        }

        private void iconSearch_Click(object sender, EventArgs e)
        {
            //关闭键盘
            this. Focus( );
            if ( this.txtSearch != null && this. txtSearch. Text. Trim( ) == "" )
            {
                return;
            }
            //清空数据
            this.resultUserList.SearchName = this.txtSearch.Text.Trim();
            this.resultUserList.Refresh();
            this.resultUserList.listBoxHelper_ReloadDelegate();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if ( e.Key == Key.Enter)
            {
                this. iconSearch_Click( null, null );
            }
            base. OnKeyDown( e );
        }
        #endregion
    }
}