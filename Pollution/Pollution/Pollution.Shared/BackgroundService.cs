using System;
using System.Collections.Generic;
using System.Text;
using Windows.ApplicationModel.Background;

namespace Pollution
{
    public static class BackgroundService
    {
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
