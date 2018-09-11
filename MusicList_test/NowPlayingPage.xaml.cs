using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace MusicList_test
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class NowPlayingPage : Page
    {
        public NowPlayingPage()
        {
            this.InitializeComponent();
        }

        //
        //share song part
        private void ShareSong(object sender, RoutedEventArgs e)
        {
            if (songList.SelectedItem != null)
            {
                DataTransferManager.ShowShareUI();
            }
            else
            {
                var j = new MessageDialog("请选择item!").ShowAsync();
            }
        }

        async void ShareRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {

            DataRequest request = args.Request;
            DataRequestDeferral getFile = args.Request.GetDeferral();
            if (songList.SelectedItem != null)
            {
                request.Data.Properties.Title = "给你分享一首好歌：" + songList.SelectedItem.Title;
                request.Data.SetText("歌手是" + songList.SelectedItem.Artist + "\n" + "复制链接到浏览器收听" + songList.SelectedItem.Path);
            }
            else
            {
                request.Data.Properties.Title = Title.Text;
                request.Data.SetText(Artist.Text);
            }

            try
            {
                //图片分享未完成
                //StorageFile image_File = await StorageFile.GetFileFromApplicationUriAsync(new Uri("Assets/音符.png"));
                //request.Data.Properties.Thumbnail = RandomAccessStreamReference.CreateFromFile(image_File);
                //request.Data.SetBitmap(RandomAccessStreamReference.CreateFromFile(image_File));
            }
            finally
            {
                getFile.Complete();
            }
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DataTransferManager.GetForCurrentView().DataRequested += ShareRequested;

            //Windows.UI.Core.SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = Windows.UI.Core.AppViewBackButtonVisibility.Collapsed;
        }

    }
}
