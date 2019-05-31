using DurableTask.AzureStorage;
using DurableTask.Core;
using Libraries.Activities;
using Libraries.Orchestrations;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace WorkerService
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var taskList = new List<Task>();
            for (int i = 0; i < 1; i++)
            {
                taskList.Add(StartWorker());
            }

            Task.WaitAll(taskList.ToArray());
        }

        private static async Task StartWorker()
        {
            var workerId = Guid.NewGuid().ToString();
            Console.WriteLine("***********************************************");
            Console.WriteLine("Starting Worker Service: " + workerId);

            var storageConnectionString = ConfigurationManager.AppSettings["StorageConnectionString"];
            var taskHubName = ConfigurationManager.AppSettings["TaskHubName"];

            var azureStorageOrchestrationSetting = new AzureStorageOrchestrationServiceSettings()
            {
                TaskHubName = taskHubName,
                StorageConnectionString = storageConnectionString,
                WorkerId = workerId
            };

            IOrchestrationService orchestrationService = new AzureStorageOrchestrationService(azureStorageOrchestrationSetting);
            var taskHubWorker = new TaskHubWorker(orchestrationService);

            _ = taskHubWorker.AddTaskOrchestrations(typeof(ModelValidationOrchestration));
            _ = taskHubWorker.AddTaskActivities(typeof(ExtractInterfacesActivity));
            _ = taskHubWorker.AddTaskActivities(typeof(ValidateInterfaceActivity));

            await orchestrationService.CreateIfNotExistsAsync().ConfigureAwait(true);

            _ = await taskHubWorker.StartAsync().ConfigureAwait(true);

            Console.WriteLine("Executing Work..." + workerId);
        }
    }
}