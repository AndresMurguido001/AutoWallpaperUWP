﻿using AutoWallpaperUWP.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

namespace AutoWallpaperUWP.Services
{
    public class UnsplashApiService : IDisposable
    {
        private HttpClient httpClient;
        private string baseUri = "https://api.unsplash.com";

        private const string UnsplashApiKey = "UnsplashApiKey";
        
        readonly ApplicationDataContainer localSettings;

        public UnsplashApiService()
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

        public async Task<ObservableCollection<Collection>> ListCollections() 
        {
            var response = await httpClient.GetAsync(new Uri($"{baseUri}/collections"));
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ObservableCollection<Collection>>(responseBody);
        }

        public async Task<ObservableCollection<Photo>> GetCollectionPhotos(string collectionId)
        {
            var response = await httpClient.GetAsync(new Uri($"{baseUri}/collections/{collectionId}/photos"));
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ObservableCollection<Photo>>(responseBody);
        }

        public async Task<Photo> GetRandomImageFromCollections(List<string> collectionIds)
        {
            string commaSeperatedList = string.Join(",", collectionIds);
            var response = await httpClient.GetAsync(new Uri($"{baseUri}/photos/random?collections={commaSeperatedList}"));
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Photo>(responseBody);
        }

        /// <summary>
        /// Downloads image for use with UserProfileService SetWallpaper
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public async Task<BitmapImage> LoadImage(Uri uri)
        {
            BitmapImage bitmapImage = new BitmapImage();

            using (var response = await httpClient.GetAsync(uri))
            {
                response.EnsureSuccessStatusCode();

                using (MemoryStream inputStream = new MemoryStream())
                {
                    await inputStream.CopyToAsync(inputStream);
                    bitmapImage.SetSource(inputStream.AsRandomAccessStream());
                }
            }

            return bitmapImage;
            
        }

        private string GenerateOptimalResolutionUri(Photo wallpaperPhoto)
        {
            var displayInformation = DisplayInformation.GetForCurrentView();
            var screenWidth = displayInformation.ScreenWidthInRawPixels;

            // Raw url is the base url for the image
            string baseUri = wallpaperPhoto.Urls["raw"];
            // Here we will append the width and DPR to the query params
            return $"{baseUri}&w={screenWidth}&dpr=2";
        }

        public void Dispose()
        {
            this.httpClient.Dispose();
        }
    }
}
