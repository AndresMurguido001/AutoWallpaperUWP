using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace AutoWallpaperUWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        private const string UnsplashApiKey = "UnsplashApiKey";
        private const string WallpaperChangeTime = "WallpaperChangeTime";

        ApplicationDataContainer localSettings;


        public SettingsPage()
        {
            this.InitializeComponent();
            localSettings = ApplicationData.Current.LocalSettings;
            InitializeSettings();
        }
        // If localSettings already contains an API key, we will populate the text box with it
        // otherwise, we will set the value to null
        private void InitializeSettings()
        {
            // Initialize UnsplashApiKey settings value
            if (localSettings.Values[UnsplashApiKey] != null)
            {
                unsplashKey.Text = localSettings.Values[UnsplashApiKey].ToString();
            }
            else
            {
                localSettings.Values[UnsplashApiKey] = null;
            }

            
            // Initialize WallpaperChangeTime settings value
            if (localSettings.Values[WallpaperChangeTime] != null)
            {
                wallpaperChangeTime.Time = TimeSpan.Parse(localSettings.Values[WallpaperChangeTime].ToString());
            }
            else
            {
                localSettings.Values[WallpaperChangeTime] = TimeSpan.Parse("9:00:00");
            }

        }

        private void updateCredentials_Click(object sender, RoutedEventArgs e)
        {
            localSettings.Values.Clear();
            localSettings.Values[UnsplashApiKey] = unsplashKey.Text;
            localSettings.Values[WallpaperChangeTime] = wallpaperChangeTime.Time.ToString();
        }
    }
}
