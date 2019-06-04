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
            taskList.Add(StartWorker());

            Task.WaitAll(taskList.ToArray());
        }

        private static async Task StartWorker()
        {
            var workerId = Guid.NewGuid().ToString();
            Console.WriteLine("*****************************************************");
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
            IOrchestrationServiceClient orchestrationServiceClient = new AzureStorageOrchestrationService(azureStorageOrchestrationSetting);

            var taskHubWorker = new TaskHubWorker(orchestrationService);
            var taskHubClient = new TaskHubClient(orchestrationServiceClient);

            _ = taskHubWorker.AddTaskOrchestrations(typeof(ModelValidationOrchestration));
            _ = taskHubWorker.AddTaskActivities(typeof(ExtractInterfacesActivity));
            _ = taskHubWorker.AddTaskActivities(typeof(ValidateInterfaceActivity));

            await orchestrationService.CreateIfNotExistsAsync().ConfigureAwait(true);

            Task.Run(async () =>
            {
                await Start(taskHubWorker).ConfigureAwait(true);
            }).Wait();

            while (true)
            {
                Thread.Sleep(1000);
                Console.WriteLine("Working: " + DateTime.Now.ToString());
            }
        }

        private static async Task Start(TaskHubWorker taskHubWorker)
        {
            _ = await taskHubWorker.StartAsync().ConfigureAwait(true);
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Worker Started");
        }
    }
}