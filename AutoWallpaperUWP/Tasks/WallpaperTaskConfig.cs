using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;
using Windows.Storage;

namespace AutoWallpaperUWP.Tasks
{
    public class WallpaperTaskConfig
    {
        public const string WallpaperTaskEntryPoint = "AutoWallpaper.Background.WallpaperTask";
        public const string WallpaperTaskName = "WallpaperTask";
        public static string WallpaperTaskProgress = "";
        public static bool WallpaperTaskRegistered = false;

        public static PropertySet TaskStatuses = new PropertySet();

        /// <summary>
        /// Register a background task with the specified taskEntryPoint, name, trigger,
        /// and condition (optional).
        /// </summary>
        /// <param name="taskEntryPoint">Task entry point for the background task.</param>
        /// <param name="name">A name for the background task.</param>
        /// <param name="trigger">The trigger for the background task.</param>
        /// <param name="condition">An optional conditional event that must be true for the task to fire.</param>
        public static BackgroundTaskRegistration RegisterBackgroundTask(String taskEntryPoint, String name, IBackgroundTrigger trigger, IBackgroundCondition condition, BackgroundTaskRegistrationGroup group = null)
        {
            if (TaskRequiresBackgroundAccess(name))
            {
                // If the user denies access, the task will not run.
                var requestTask = BackgroundExecutionManager.RequestAccessAsync();
            }

            var builder = new BackgroundTaskBuilder();

            builder.Name = name;
            builder.TaskEntryPoint = taskEntryPoint;
            builder.SetTrigger(trigger);

            if (condition != null)
            {
                builder.AddCondition(condition);

                //
                // If the condition changes while the background task is executing then it will
                // be canceled.
                //
                builder.CancelOnConditionLoss = true;
            }

            if (group != null)
            {
                builder.TaskGroup = group;
            }

            BackgroundTaskRegistration task = builder.Register();

            UpdateBackgroundTaskRegistrationStatus(name, true);

            //
            // Remove previous completion status.
            //
            var settings = ApplicationData.Current.LocalSettings;
            settings.Values.Remove(name);

            return task;
        }

        /// <summary>
        /// Unregister background tasks with specified name.
        /// </summary>
        /// <param name="name">Name of the background task to unregister.</param>
        public static void UnregisterBackgroundTasks(String name, BackgroundTaskRegistrationGroup group = null)
        {
            //
            // If the given task group is registered then loop through all background tasks associated with it
            // and unregister any with the given name.
            //
            if (group != null)
            {
                foreach (var cur in group.AllTasks)
                {
                    if (cur.Value.Name == name)
                    {
                        cur.Value.Unregister(true);
                    }
                }
            }
            else
            {
                //
                // Loop through all ungrouped background tasks and unregister any with the given name.
                //
                foreach (var cur in BackgroundTaskRegistration.AllTasks)
                {
                    if (cur.Value.Name == name)
                    {
                        cur.Value.Unregister(true);
                    }
                }
            }

            UpdateBackgroundTaskRegistrationStatus(name, false);
        }

        /// <summary>
        /// Store the registration status of a background task with a given name.
        /// </summary>
        /// <param name="name">Name of background task to store registration status for.</param>
        /// <param name="registered">TRUE if registered, FALSE if unregistered.</param>
        public static void UpdateBackgroundTaskRegistrationStatus(String name, bool registered)
        {
            if (name == WallpaperTaskName)
            {
                WallpaperTaskRegistered = true;
            } 
            else
            {
                WallpaperTaskRegistered = false;
            }
        }

        /// <summary>
        /// Get the registration / completion status of the background task with
        /// given name.
        /// </summary>
        /// <param name="name">Name of background task to retreive registration status.</param>
        public static String GetBackgroundTaskStatus(String name)
        {
            var registered = false;
            switch (name)
            {
                case WallpaperTaskName:
                    registered = WallpaperTaskRegistered;
                    break;
            }

            var status = registered ? "Registered" : "Unregistered";

            object taskStatus;
            var settings = ApplicationData.Current.LocalSettings;
            if (settings.Values.TryGetValue(name, out taskStatus))
            {
                status += " - " + taskStatus.ToString();
            }

            return status;
        }

        /// <summary>
        /// Determine if task with given name requires background access.
        /// </summary>
        /// <param name="name">Name of background task to query background access requirement.</param>
        public static bool TaskRequiresBackgroundAccess(String name)
        {
            if (name == WallpaperTaskName)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
