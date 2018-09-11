using MusicList_test.Model;
using MusicList_test.ViewModel;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace MusicList_test
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public MainPage()
        {
            this.InitializeComponent();
            SongManager songManager = new SongManager();
        }
        
        ViewModel.SongVM songList = SongVM.Single();
        int index;
        int IsPlaying = 0;
        
        private void Element_MediaOpened(object sender, RoutedEventArgs e)
        {
            TimeLine.Value = 0;
            TimeLine.Maximum = MediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
        }

        private void Element_MediaEnded(object sender, RoutedEventArgs e)
        {
            
            if(index >= songList.AllItems.Count-1)
            {
                index = 0;
            }
            else
            {
                index++;
            }
            //更改当前选择歌曲
            SongList.SelectedItem = songList.AllItems[index];
            songList.SelectedItem = songList.AllItems[index];
            Uri SongUri = new Uri(songList.SelectedItem.Path);
            MediaPlayer.Source = SongUri;
            MediaPlayer.Play();
            TimeLine.Value = 0;
            TimeLine.Maximum = MediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;


            Title.Text = songList.SelectedItem.Title;
            Artist.Text = songList.SelectedItem.Artist;
            Path.Text = songList.SelectedItem.Path;

            //实现自动播放下一曲
        }

        private void ChangeMediaVolume(object sender, RoutedEventArgs e)
        {
            MediaPlayer.Volume = (double)VolumeLine.Value / 100.0;
        }

        private void PlayClick(object sender, RoutedEventArgs e)
        {
            ToolTip toolTip = new ToolTip();

            if (songList.SelectedItem == null)
            {
                return;
            }
            
            if(IsPlaying == 0)
            {
                MediaPlayer.Play();
                IsPlaying = 1;
                PlayPauseButton.Icon = new SymbolIcon(Symbol.Pause);
                toolTip.Content = "Pause";
                ToolTipService.SetToolTip(PlayPauseButton, toolTip);
            }
            else
            {
                MediaPlayer.Pause();
                IsPlaying = 0;
                PlayPauseButton.Icon = new SymbolIcon(Symbol.Play);
                toolTip.Content = "Play";
                ToolTipService.SetToolTip(PlayPauseButton, toolTip);
            }
        }

        private void NextSong(object sender, RoutedEventArgs e)
        {
            Element_MediaEnded(sender, e);
        }

        private void PrevSong(object sender, RoutedEventArgs e)
        {
            if (index == 0)
            {
                index = songList.AllItems.Count - 1;
            }
            else
            {
                index--;
            }
            //更改当前选择歌曲
            SongList.SelectedItem = songList.AllItems[index];
            songList.SelectedItem = songList.AllItems[index];
            Uri SongUri = new Uri(songList.SelectedItem.Path);
            MediaPlayer.Source = SongUri;
            MediaPlayer.Play();
            TimeLine.Value = 0;
            TimeLine.Maximum = MediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;

            Title.Text = songList.SelectedItem.Title;
            Artist.Text = songList.SelectedItem.Artist;
            Path.Text = songList.SelectedItem.Path;
        }

        private int GetId()
        {
            string query = "%%";
            int id = 0;
            string now_name = songList.SelectedItem.Title;
            string now_singer = songList.SelectedItem.Artist;
            using (var statement = App.conn.Prepare("SELECT Id, Name, Singer FROM Customer WHERE Id LIKE ? OR Name LIKE ? OR Singer LIKE ? "))
            {
                statement.Bind(1, query);
                statement.Bind(2, query);
                statement.Bind(3, query);

                while (statement.Step() != SQLiteResult.DONE)
                {
                    string did = statement[0].ToString();
                    string dname = statement[1].ToString();
                    string dsinger = statement[2].ToString();
                    if (now_name == dname &&
                        now_singer == dsinger)
                    {
                        id = int.Parse(did);
                        break;
                    }
                }
            }
            return id;
        }

        //private void UpdateSong(object sender, RoutedEventArgs e)
        //{
        //    if (songList.SelectedItem != null)
        //    {
        //        using (var custstmt = App.conn.Prepare("UPDATE Customer SET Name = ?, Singer = ?, Path = ? WHERE Id = ?"))
        //        {
        //            custstmt.Bind(1, Title.Text);
        //            custstmt.Bind(2, Artist.Text);
        //            custstmt.Bind(3, Path.Text);
        //            custstmt.Bind(4, GetId());
        //            custstmt.Step();
        //        }
        //        songList.UpdateSong(songList.SelectedItem.Id, Title.Text, Artist.Text, Path.Text);
        //        var j = new MessageDialog("更新成功!").ShowAsync();
        //        //磁贴更新update(sender, e);
        //         Frame.Navigate(typeof(MainPage), songList);
        //    }
        //}

        private void BackButtonClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(IntroPage));
        }

        private void MusicButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SymbolIcon_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MediaPlayer.Volume = 0;
            VolumeLine.Value = 0;
        }

        private void ChangeMediaVolume(object sender, RangeBaseValueChangedEventArgs e)
        {

        }

        private void ShuffleIcon_Tapped(object sender, TappedRoutedEventArgs e)
        {
            
        }

        private void SearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            ObservableCollection<Song> Suggetion = new ObservableCollection<Song>();

            string para = "%" + sender.Text + "%";
            
            string dtitle;
            string dartist;
            string dalbum;

            using (var statement = App.conn.Prepare(
                "SELECT Title, Artist, Album FROM Customer WHERE Name LIKE ? OR Singer LIKE ? OR Album LIKE ?"))
            {
                statement.Bind(1, para);
                statement.Bind(2, para);
                statement.Bind(3, para);

                while (statement.Step() != SQLiteResult.DONE)
                {
                    dtitle = statement[0].ToString();
                    dartist = statement[1].ToString();
                    dalbum = statement[2].ToString();
                }

                //TODO 

                SearchBox.ItemsSource = 

            }
        }
    }
}
