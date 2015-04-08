using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Storage;

namespace Pollution
{
    public static class BackgroundService
    {
        internal async static void RegisterTask()
        {
            var task = RegisterBackgroundTask("BackgroundTask.TileTask", "TileTask", new SystemCondition(SystemConditionType.InternetAvailable));
            await task;
            
        }

        private static async Task<BackgroundTaskRegistration> RegisterBackgroundTask(String taskEntryPoint, String name, IBackgroundCondition condition)
        {
            //await BackgroundExecutionManager.RequestAccessAsync();

            var builder = new BackgroundTaskBuilder();

            builder.Name = name;
            builder.TaskEntryPoint = taskEntryPoint;
            builder.SetTrigger(new TimeTrigger(15, false));
            if (condition != null)
            {
                builder.AddCondition(condition);

                //
                // If the condition changes while the background task is executing then it will
                // be canceled.
                //
                builder.CancelOnConditionLoss = true;
            }

            foreach (var cur in BackgroundTaskRegistration.AllTasks)
            {
                //if (cur.Value.Name == name)
                //{
                    cur.Value.Unregister(true);
                //}
            }

            //UpdateBackgroundTaskStatus(name, false);

            BackgroundTaskRegistration task = builder.Register();

            //UpdateBackgroundTaskStatus(name, true);

            //
            // Remove previous completion status from local settings.
            //
            //var settings = ApplicationData.Current.LocalSettings;
            //settings.Values.Remove(name);

            return task;
        }
































        public static BackgroundTaskRegistration RegisterBackgroundTask(string taskEntryPoint, string name, IBackgroundTrigger trigger, IBackgroundCondition condition)
        {
            // Check for existing registrations of this background task.
            foreach (var cur in BackgroundTaskRegistration.AllTasks)
            {
                if (cur.Value.Name == name)
                {
                    // The task is already registered.
                    return (BackgroundTaskRegistration)(cur.Value);
                }
            }

            // Register the background task.
            var builder = new BackgroundTaskBuilder();

            builder.Name = name;
            builder.TaskEntryPoint = taskEntryPoint;
            builder.SetTrigger(trigger);

            if (condition != null)
            {

                builder.AddCondition(condition);
            }

            BackgroundTaskRegistration task = builder.Register();

            return task;

        }

        internal static async void RegisterTileTask()
        {
            string tileTaskName = "TileTask";
            string tileTaskEntryPoint = "BackgroundTask.TileTask";

            Task delay = Task.Run(() => { Task.Delay(1000); });

            /*try
            {
                var backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();
                if (backgroundAccessStatus == BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity || backgroundAccessStatus == BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity)
                {*/
                    foreach (var task in BackgroundTaskRegistration.AllTasks)
                    {
                        if (task.Value.Name == tileTaskName)
                        {
                            task.Value.Unregister(true);
                        }
                    }

                    BackgroundTaskBuilder taskBuilder = new BackgroundTaskBuilder();
                    taskBuilder.Name = tileTaskName;
                    taskBuilder.TaskEntryPoint = tileTaskEntryPoint;
                    taskBuilder.SetTrigger(new TimeTrigger(30, false));
                    var registration = taskBuilder.Register();
                /*}
            }
            catch (Exception)
            {
                foreach (var task in BackgroundTaskRegistration.AllTasks)
                {
                    if (task.Value.Name == tileTaskName)
                    {
                        task.Value.Unregister(true);
                    }
                }

                BackgroundTaskBuilder taskBuilder = new BackgroundTaskBuilder();
                taskBuilder.Name = tileTaskName;
                taskBuilder.TaskEntryPoint = tileTaskEntryPoint;
                taskBuilder.SetTrigger(new TimeTrigger(30, false));
                var registration = taskBuilder.Register();
            }*/
            
        }

        
    }
}
