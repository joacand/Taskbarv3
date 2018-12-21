using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using Taskbarv3.Core.Interfaces;

namespace Taskbarv3.Core.Services
{
    public class FavoritesService : IFavoritesService
    {
        private readonly IStatusService statusService;
        private readonly ISongViewerService songViewerService;

        private static readonly string favoritesM3u = ConfigurationManager.AppSettings["FavoritesM3uName"];
        private static readonly string favoritesTxt = ConfigurationManager.AppSettings["FavoritesTxtName"];
        private string lastCurrentSongPath = string.Empty;

        public FavoritesService(IStatusService statusService, ISongViewerService songViewerService)
        {
            this.statusService = statusService;
            this.songViewerService = songViewerService;
        }

        public bool PlayFavorites()
        {
            statusService.SetStatus("Starting favorites");
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = ConfigurationManager.AppSettings["Foobar2000Location"],
                WorkingDirectory = ConfigurationManager.AppSettings["Foobar2000WorkingDirectory"],
                Arguments = "\"" + Directory.GetCurrentDirectory() + $"\\{favoritesM3u}\""
            };
            Process myProcess = new Process { StartInfo = processStartInfo };
            try
            {
                return myProcess.Start();
            }
            catch (Exception e)
            {
                statusService.SetStatus($"Exception when starting process: {e}");
                return false;
            }
        }

        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public bool AddToFavorites()
        {
            if (!songViewerService.IsOnline)
            {
                return false;
            }

            string currentSongPath = songViewerService.FilePath;
            string currentSong = songViewerService.CurrentSong;

            if (currentSongPath.Equals("")
                || currentSong.Equals("No song playing")
                || lastCurrentSongPath.Equals(currentSongPath))
            {
                return false;
            }

            if (!File.Exists(favoritesM3u) || !File.Exists(favoritesTxt))
            {
                using (File.Create(favoritesM3u)) { }
                using (File.Create(favoritesTxt)) { }
            }

            if (GetFavorites(favoritesTxt).Contains(currentSong))
            {
                statusService.SetStatus("Already in playlist");
            }
            else
            {
                using (FileStream fs = new FileStream(favoritesM3u, FileMode.Append, FileAccess.Write))
                using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    sw.WriteLine(currentSongPath);
                    lastCurrentSongPath = currentSongPath;
                }
                using (FileStream fs = new FileStream(favoritesTxt, FileMode.Append, FileAccess.Write))
                using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    sw.WriteLine(currentSong);
                }
                statusService.SetStatus("Added to playlist");
                return true;
            }
            return false;
        }

        private List<string> GetFavorites(string favoritesTXT)
        {
            List<string> result = new List<string>();
            using (StreamReader sr = new StreamReader(favoritesTXT))
            {
                string s;
                while ((s = sr.ReadLine()) != null)
                {
                    result.Add(s);
                }
            }
            return result;
        }

        public void Reset()
        {
            lastCurrentSongPath = "";
        }

        public void ClearFavorites()
        {
            File.Delete(favoritesM3u);
            File.Delete(favoritesTxt);
        }
    }
}
