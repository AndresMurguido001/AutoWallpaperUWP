using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Web.Http;
using Windows.Web.Http.Headers;
using AutoWallpaper.Background.Models;
using Windows.System.UserProfile;
using Windows.Foundation;

namespace AutoWallpaper.Background.Services
{
    /// <summary>
    /// WallpaperService will handle the main logic behind fetching,
    /// downloading, and setting the images used for desktop wallpaper
    /// </summary>
    public sealed class WallpaperService : IDisposable
    {
        private HttpClient httpClient;
        private string baseUri = "https://api.unsplash.com";

        private const string UnsplashApiKey = "UnsplashApiKey";
        
        readonly ApplicationDataContainer localSettings;

        public WallpaperService()
        {
            localSettings = ApplicationData.Current.LocalSettings;

            httpClient = new HttpClient();

            var unsplashCreds = localSettings.Values[UnsplashApiKey];

            if (unsplashCreds != null)
            {
                var headers = httpClient.DefaultRequestHeaders;
                headers.Add("Authorization", $"Client-ID {unsplashCreds}");
            }
        }

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            Debug.WriteLine("Background " + taskInstance.Task.Name + " Starting...");
        }

        public IAsyncOperation<IList<Collection>> ListCollectionsAsync() 
        {
            return Task.Run<IList<Collection>>(async () =>
            {

                var response = await httpClient.GetAsync(new Uri($"{baseUri}/collections"));
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IList<Collection>>(responseBody);
            }).AsAsyncOperation();
        }


        public IAsyncOperation<IList<Photo>> GetCollectionPhotosAsync(string collectionId)
        {
            return Task.Run<IList<Photo>>(async () =>
            {
                var response = await httpClient.GetAsync(new Uri($"{baseUri}/collections/{collectionId}/photos"));
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IList<Photo>>(responseBody);
            }).AsAsyncOperation();
        }

        public IAsyncOperation<Photo> GetRandomImageFromCollectionsAsync(IEnumerable<Collection> selectedCollections)
        {
            return Task.Run<Photo>(async () =>
            {
                IEnumerable<string> collectionIds = selectedCollections.Select(c => c.Id);
                string commaSeperatedList = string.Join(",", collectionIds);
                var response = await httpClient.GetAsync(new Uri($"{baseUri}/photos/random?collections={commaSeperatedList}"));
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Photo>(responseBody);
            }).AsAsyncOperation();
        }

        public IAsyncOperation<Photo> GetRandomImageAsync()
        {
            return Task.Run<Photo>(async () =>
            {
                var response = await httpClient.GetAsync(new Uri($"{baseUri}/photos/random"));
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Photo>(responseBody);
            }).AsAsyncOperation();
        }

        /// <summary>
        /// Downloads image to wallpapers/currentWallpaper
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public IAsyncOperation<StorageFile> LoadImageAsync(Uri uri)
        {
            return Task.Run<StorageFile>(async () =>
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                StorageFolder wallpapersFolder = await localFolder.CreateFolderAsync("wallpapers", CreationCollisionOption.OpenIfExists);

                StorageFile imageFile = await wallpapersFolder.CreateFileAsync("currentWallpaper", CreationCollisionOption.ReplaceExisting);

                var image = await httpClient.GetAsync(uri);

                using (var inputStream = await image.Content.ReadAsInputStreamAsync())
                {
                    using (var outputStream = await imageFile.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        await RandomAccessStream.CopyAndCloseAsync(inputStream, outputStream);
                    }
                }
                return imageFile;
            }).AsAsyncOperation();
        }

        public Uri GenerateOptimalResolutionUri(Photo wallpaperPhoto)
        {
            //var displayInformation = DisplayInformation.GetForCurrentView();
            //var screenWidth = displayInformation.ScreenWidthInRawPixels;
            var screenWidth = 2560;
            // Raw url is the base url for the image
            string baseUri = wallpaperPhoto.Urls["raw"];
            // Here we will append the width and DPR to the query params
            return new Uri($"{baseUri}&w={screenWidth}&dpr=2");
        }

        #region Wallpaper related operations
        /// <summary>
        /// Sets a random wallpaper. 
        /// </summary>
        /// <param name="wallpaperPhoto"></param>
        /// <returns></returns>
        public IAsyncOperation<bool> SetRandomWallpaperAsync(bool fromCollections)
        {
            return Task.Run<bool>(async () =>
            {
                Photo wallpaperPhoto;
                wallpaperPhoto = await GetRandomImageAsync();
                // Downloads the random image to wallpapers/wallpaper
                Uri completedWallpaperUri = GenerateOptimalResolutionUri(wallpaperPhoto);
                StorageFile downloadedImage = await LoadImageAsync(completedWallpaperUri);
                var success = await SetWallpaperAsync(downloadedImage);
                return success;
            }).AsAsyncOperation();
        }


        private IAsyncOperation<bool> SetWallpaperAsync(StorageFile wallpaperImage)
        {
            bool successful = false;
            return Task.Run<bool>(async () =>
            {
                if (UserProfilePersonalizationSettings.IsSupported())
                {
                    UserProfilePersonalizationSettings profileSettings = UserProfilePersonalizationSettings.Current;
                    successful = await profileSettings.TrySetWallpaperImageAsync(wallpaperImage);
                }
                return successful;
            }).AsAsyncOperation();
        }

        #endregion
        public void Dispose()
        {
            this.httpClient.Dispose();
        }

    }
}
