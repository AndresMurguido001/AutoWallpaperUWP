using AutoWallpaperUWP.Models;
using AutoWallpaperUWP.Services;
using AutoWallpaperUWP.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System.UserProfile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace AutoWallpaperUWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        MainPage rootPage = MainPage.Current;
        public CollectionsViewModel ViewModel { get; set; }
        private UnsplashApiService unsplashApiService;

        public HomePage()
        {
            this.InitializeComponent();
            unsplashApiService = new UnsplashApiService();
            this.ViewModel = new CollectionsViewModel();
            ViewModel.InitializeCollections();
        }

        /// <summary>
        /// Chooses either a completely random photo, or a random photo
        /// from the selected collections (If user has selected any collections)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void UpdateWallpaper_Click(object sender, RoutedEventArgs e)
        {
            bool wallpaperSet;

            if (ViewModel.Collections.Count() > 0)
            {
                wallpaperSet = await SetRandomWallpaper(true);
            }
            else
            {
                wallpaperSet = await SetRandomWallpaper(false);
            }
            NotifyUserWallpaperStatus(wallpaperSet);
        }

        private void NotifyUserWallpaperStatus(bool wallpaperSet)
        {
            if (wallpaperSet)
            {
                rootPage.DisplayError("Success!", "Wallpaper was successfully changed");
            }
            else
            {
                rootPage.DisplayError("Error!", "Something went wrong");
            }
        }
        /// <summary>
        /// Sets a random wallpaper. 
        /// </summary>
        /// <param name="wallpaperPhoto"></param>
        /// <returns></returns>
        private async Task<bool> SetRandomWallpaper(bool fromCollections)
        {
            Photo wallpaperPhoto; 

            if (fromCollections)
            {
                wallpaperPhoto = await unsplashApiService.GetRandomImageFromCollections(ViewModel.SelectedCollections);
            }
            else
            {
                wallpaperPhoto = await unsplashApiService.GetRandomImage();
            }
            // Downloads the random image to wallpapers/wallpaper
            Uri completedWallpaperUri = unsplashApiService.GenerateOptimalResolutionUri(wallpaperPhoto);
            StorageFile downloadedImage = await unsplashApiService.LoadImage(completedWallpaperUri);
            return await SetWallpaper(downloadedImage);
        }

        
        private async Task<bool> SetWallpaper(StorageFile wallpaperImage)
        {
            bool successful = false;

            if (UserProfilePersonalizationSettings.IsSupported())
            {
                UserProfilePersonalizationSettings profileSettings = UserProfilePersonalizationSettings.Current;
                successful = await profileSettings.TrySetWallpaperImageAsync(wallpaperImage);
            }
            return successful;
        }
    }
}
