using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace MusicList_test.Model
{
    public class Song
    {
        //每个Song的所属编号，外界不可更改
        public int Id { get;}
                
        public string Title { get; set; }
        
        public string Artist { get; set; }

        public string Album { get; set; }

        public BitmapImage Cover { get; set; }

        //链接
        public string Path { get; set; }

        // personalization data
        public int PlayCount { get; set; }

        //构造函数
        public Song(string _title, string _artist, string _album)
            :this(_title, _artist, "", null, 0, "")
        { }

        public Song(
            string _title, string _artist, string _album, 
            BitmapImage cover, int _playcount, string _path)
        {
            Title = _title;
            Path = _path;
            Artist = _artist;
            Album = _album;
            Cover = cover;
            PlayCount = _playcount;
            Id = getNewId();
        }

        public bool EquivalentTo(string _title, string _artist)
        {
            if (Title == Title && Artist == _artist)
                return true;
            else return false;
        }

        //添加新歌曲后id++
        private static int current_id = 0;

        private static int getNewId()
        {
            return (current_id++);
        }
    }
}
