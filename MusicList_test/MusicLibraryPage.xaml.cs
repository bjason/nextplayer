using MusicList_test.Model;
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
    public sealed partial class MusicLibraryPage : Page
    {
        public MusicLibraryPage()
        {
            this.InitializeComponent();
        }

        ViewModel.SongVM songList = SongVM.Single();
        int index;
        int IsPlaying = 0;

        private void SongClick(object sender, ItemClickEventArgs e)
        {

            songList.SelectedItem = (Song)(e.ClickedItem);
            //动态调整没写
            Title.Text = songList.SelectedItem.Title;
            Artist.Text = songList.SelectedItem.Artist;
            Path.Text = songList.SelectedItem.Path;

            try
            {
                Uri pathUri = new Uri(songList.SelectedItem.Path);
                mediaPlayer.Source = pathUri;
                index = songList.SelectedItem.Id;
                mediaPlayer.Play();
                IsPlaying = 1;
                PlayPauseButton.Icon = new SymbolIcon(Symbol.Pause);
                PlayPauseButton.Label = "Pause";
            }
            catch (Exception ex)
            {
                if (ex is FormatException)
                {
                    System.Diagnostics.Debug.WriteLine("MusicError");
                    // handle exception. 
                    // For example: Log error or notify user problem with file
                }
            }
        }

        private void DeleteSong(object sender, RoutedEventArgs e)
        {
            using (var statement = App.conn.Prepare("DELETE FROM MusicLib WHERE Id = ?"))
            {
                statement.Bind(1, GetId());
                statement.Step();
            }
            songList.RemoveSong();
            var j = new MessageDialog("删除成功!").ShowAsync();
            //update(sender, e);
            Title.Text = "";
            Artist.Text = "";
            Path.Text = "";
        }
    }
}
