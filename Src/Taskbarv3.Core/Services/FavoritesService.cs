using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text;
using Taskbarv3.Core.Interfaces;

namespace Taskbarv3.Core.Services
{
    public class FavoritesService(IStatusService statusService, ISongViewerService songViewerService) : IFavoritesService
    {
        private readonly IStatusService statusService = statusService;
        private readonly ISongViewerService songViewerService = songViewerService;

        private static readonly string? favoritesM3u = ConfigurationManager.AppSettings["FavoritesM3uName"];
        private static readonly string? favoritesTxt = ConfigurationManager.AppSettings["FavoritesTxtName"];
        private string lastCurrentSongPath = string.Empty;

        public bool PlayFavorites()
        {
            statusService.SetStatus("Starting favorites");
            ProcessStartInfo processStartInfo = new()
            {
                FileName = ConfigurationManager.AppSettings["Foobar2000Location"],
                WorkingDirectory = ConfigurationManager.AppSettings["Foobar2000WorkingDirectory"],
                Arguments = "\"" + Directory.GetCurrentDirectory() + $"\\{favoritesM3u}\""
            };
            Process myProcess = new() { StartInfo = processStartInfo };
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

        public bool AddToFavorites()
        {
            if (!songViewerService.IsOnline || string.IsNullOrWhiteSpace(favoritesM3u) || string.IsNullOrWhiteSpace(favoritesTxt))
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
                using (FileStream fs = new(favoritesM3u, FileMode.Append, FileAccess.Write))
                using (StreamWriter sw = new(fs, Encoding.UTF8))
                {
                    sw.WriteLine(currentSongPath);
                    lastCurrentSongPath = currentSongPath;
                }
                using (FileStream fs = new(favoritesTxt, FileMode.Append, FileAccess.Write))
                using (StreamWriter sw = new(fs, Encoding.UTF8))
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
            List<string> result = [];
            using (StreamReader sr = new(favoritesTXT))
            {
                string? s;
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
            if (!string.IsNullOrWhiteSpace(favoritesM3u))
            {
                File.Delete(favoritesM3u);
            }
            if (!string.IsNullOrWhiteSpace(favoritesTxt))
            {
                File.Delete(favoritesTxt);
            }
        }
    }
}
