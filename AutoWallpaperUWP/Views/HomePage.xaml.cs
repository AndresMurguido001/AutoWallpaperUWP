using AutoWallpaperUWP.Models;
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

        public HomePage()
        {
            this.InitializeComponent();
            this.ViewModel = new CollectionsViewModel();
            ViewModel.InitializeCollections();
        }

        /// <summary>
        /// Chooses either a completely random photo, or a random photo
        /// from the selected collections (If user has selected any collections)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateWallpaper_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine(ViewModel.SelectedCollections.Count());
            // User has selected at least 1 collection
            //if (ViewModel.SelectedCollections.Count() > 0)
            //{

            //}
            //// User has not selected a collection
            //else
            //{

            //}
           
        }

        private bool SetWallpaper(string wallpaperPhoto)
        {
            bool successful = false;
            BitmapImage bitmapImage = new BitmapImage();

            if (UserProfilePersonalizationSettings.IsSupported())
            {

            }
            return successful;
        }
    }
}
