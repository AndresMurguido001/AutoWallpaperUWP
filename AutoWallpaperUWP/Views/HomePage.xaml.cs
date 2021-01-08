using AutoWallpaperUWP.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.System.UserProfile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using AutoWallpaper.Background.Models;
using AutoWallpaperUWP.Tasks;
using AutoWallpaperUWP.Services;
using Windows.UI.Core;

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
        private WallpaperService unsplashApiService;

        ApplicationTrigger trigger;

        public HomePage()
        {
            this.InitializeComponent();
            unsplashApiService = new WallpaperService();
            this.ViewModel = new CollectionsViewModel();
            ViewModel.InitializeCollections();
        }
        #region Task related operations

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == WallpaperTaskConfig.WallpaperTaskName)
                {
                    WallpaperTaskConfig.UpdateBackgroundTaskRegistrationStatus(WallpaperTaskConfig.WallpaperTaskName, true);
                    break;
                }
            }
            trigger = new ApplicationTrigger();
        }

        private void RegisterBackgroundTask()
        {
            var settings = ApplicationData.Current.LocalSettings;
            
            var task = WallpaperTaskConfig.RegisterBackgroundTask(WallpaperTaskConfig.WallpaperTaskEntryPoint,
                                                                  WallpaperTaskConfig.WallpaperTaskName,
                                                                  trigger,
                                                                  null);
            AttachProgressAndCompletedHandlers(task);
        }

        /// <summary>
        /// Attach progress and completed handers to a background task.
        /// </summary>
        /// <param name="task">The task to attach progress and completed handlers to.</param>
        private void AttachProgressAndCompletedHandlers(IBackgroundTaskRegistration task)
        {
            //task.Progress += new BackgroundTaskProgressEventHandler(OnProgress);
            task.Completed += new BackgroundTaskCompletedEventHandler(OnCompleted);
        }

        /// <summary>
        /// Handle background task completion.
        /// </summary>
        /// <param name="task">The task that is reporting completion.</param>
        /// <param name="e">Arguments of the completion report.</param>
        private void OnCompleted(IBackgroundTaskRegistration task, BackgroundTaskCompletedEventArgs args)
        {
            NotifyUser();
        }

        private async void NotifyUser()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                rootPage.DisplayError("Task Completed", "Successfully completed task");
            });
        }

        private void RegisterTask_Click(object sender, RoutedEventArgs e)
        {
            RegisterBackgroundTask();
        }

        private void UnregisterTask_Click(object sender, RoutedEventArgs e)
        {
            WallpaperTaskConfig.UnregisterBackgroundTasks(WallpaperTaskConfig.WallpaperTaskName);
        }

        /// <summary>
        /// Signal a ApplicationTriggerTask.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SignalBackgroundTask(object sender, RoutedEventArgs e)
        {
            // Reset the completion status
            var settings = ApplicationData.Current.LocalSettings;

            //Signal the ApplicationTrigger
            var result = await trigger.RequestAsync();
        }
        #endregion
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
