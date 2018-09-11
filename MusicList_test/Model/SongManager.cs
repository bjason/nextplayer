using SQLitePCL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml.Media.Imaging;

namespace MusicList_test.Model
{
    class SongManager
    {
        private ObservableCollection<Song> allSongs;
        private ObservableCollection<Song> NewAddSongs;
        private ArrayList ExistedSongsId;

        public SongManager()
        {
            allSongs = new ObservableCollection<Song>();
            NewAddSongs = new ObservableCollection<Song>();
            ExistedSongsId = new ArrayList();

            // TODO change to thread
            UpdateMusicLibAsync();
        }

        public ObservableCollection<Song> GetAllSongs()
        {
            return allSongs;
        }

        //
        private async void UpdateMusicLibAsync()
        {
            StorageFolder storageFolder = KnownFolders.MusicLibrary;

            await RetrieveMP3InFolders(storageFolder);

            // TODO get rid of songs that not exist
            // new allsongs vs previous all songs

            RemoveNotExistedSongs();
        }

        private void RemoveNotExistedSongs()
        {
            // get biggest id in database
            var allId = new ArrayList();
            using (var statement = App.conn.Prepare("SELECT Id FROM MusicLib"))
            {
                while (statement.Step() != SQLiteResult.DONE)
                {
                    allId.Add(int.Parse(statement[0].ToString()));
                }
            }
            allId.Sort();
            var max = allId[allId.Count - 1];
            
            // get rid of those existed id
            foreach (var id in ExistedSongsId)
            {
                if (allId.Contains(id))
                {
                    allId.Remove(id);
                }
                
            }

            // delete the id according to what was left
            string para = "(";
            for (int i = 0; i < allId.Count - 1; i++)
            {
                para += (allId[i] + ",");
            }
            para += (allId[allId.Count -  1] + ")");

            using (var statement = App.conn.Prepare("DELETE FROM MusicLib WHERE Id <> ?"))
            {
                statement.Bind(1, int.Parse(para));
                statement.Step();
            }

        }

        private async Task RetrieveMP3InFolders(StorageFolder folder)
        {
            // retrieve mp3 files
            foreach (var file in await folder.GetFilesAsync())
            {
                if (file.FileType == ".mp3")
                {
                    var musicProperty = await file.Properties.GetMusicPropertiesAsync();
                    var thumb = await file.GetThumbnailAsync(
                         ThumbnailMode.MusicView, 200, ThumbnailOptions.UseCurrentScale);
                    var cover = new BitmapImage();
                    cover.SetSource(thumb);

                    string[] para = new string[3];
                    para[0] = musicProperty.Title;
                    para[1] = musicProperty.Artist;
                    para[2] = musicProperty.Album;

                    string[] result = new string[4];
                    Song song;

                    // to detect whether the song is in the library

                    if (!ExistInDataBase(para, result))
                    {
                        song = new Song(
                            musicProperty.Title,
                            musicProperty.Artist,
                            musicProperty.Album,
                            cover,
                            0,
                            file.Path);
                        NewAddSongs.Add(song);
                    }
                    else
                    {
                        int playcount = int.Parse(result[3]);
                        song = new Song(
                            musicProperty.Title,
                            musicProperty.Artist,
                            musicProperty.Album,
                            cover,
                            playcount,
                            file.Path);
                    }
                    allSongs.Add(song);
                }
            }

            // retrieve the children folders
            foreach (var child in await folder.GetFoldersAsync())
                await RetrieveMP3InFolders(child);
        }

        private bool ExistInDataBase(string[] para, string[] result)
        {
            string dtitle = null, dartist = null, dalbum = null, dplaycount = null, did = null;

            using (var statement = App.conn.Prepare(
                "SELECT Id, Title, Artist, Album, Playcount FROM MusicLib WHERE Title LIKE ? AND Artist LIKE ? AND Album LIKE ?"))
            {
                statement.Bind(1, para[0]);
                statement.Bind(2, para[1]);
                statement.Bind(3, para[2]);

                while (statement.Step() != SQLiteResult.DONE)
                {
                    did = statement[0].ToString();
                    dtitle = statement[1].ToString();
                    dartist = statement[2].ToString();
                    dalbum = statement[3].ToString();
                    dplaycount = statement[4].ToString();
                }

                if (dtitle != null && dartist != null && dalbum != null)
                {
                    ExistedSongsId.Add(int.Parse(did));
                    result[0] = dtitle;
                    result[1] = dartist;
                    result[2] = dalbum;
                    result[3] = dplaycount;

                    return true;
                }
                else return false;
            }
        }
    }
}
