using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Web.Http;
using Windows.Graphics.Display;
using AutoWallpaper.Background.Models;
using Windows.System.Threading;
using AutoWallpaper.Background.Services;
using Windows.System.UserProfile;

namespace AutoWallpaper.Background
{
    /// <summary>
    /// This will be the main 
    /// </summary>
    public sealed class WallpaperTask : IBackgroundTask
    {
        BackgroundTaskDeferral _deferral; // Note: defined at class scope so that we can mark it complete inside the OnCancel() callback if we choose to support cancellation
        WallpaperService wallpaperService;
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();
            //
            // TODO: Insert code to start one or more asynchronous methods using the
            //       await keyword, for example:
            //
            // await ExampleMethodAsync();
            //
            using (wallpaperService = new WallpaperService())
            {
                var result = await wallpaperService.SetRandomWallpaperAsync(false);
                Debug.WriteLine($"TASK COMPLETED RESULT: {result}");
            }

            

            _deferral.Complete();
        }


    }
}
