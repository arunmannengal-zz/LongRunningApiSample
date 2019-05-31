using DurableTask.AzureStorage;
using DurableTask.Core;
using System;

namespace LongRunningApi.TaskHub
{
    internal sealed class ConnectionManager
    {
        private static readonly Lazy<ConnectionManager> LazyConnectionManager =
            new Lazy<ConnectionManager>(() =>
            {
                return new ConnectionManager();
            });

        /// <summary>
        /// Prevents a default instance of the <see cref="ConnectionManager"/> class from being created.
        /// </summary>
        private ConnectionManager()
        {
            this.taskHubClient = null;
        }

        private TaskHubClient taskHubClient;

        public static ConnectionManager Instance
        {
            get
            {
                return LazyConnectionManager.Value;
            }
        }

        public TaskHubClient GetTaskHubClient(string taskHubName, string storageConnectionString)
        {
            if (this.taskHubClient == null)
            {
                var azureStorageOrchestrationSetting = new AzureStorageOrchestrationServiceSettings()
                {
                    TaskHubName = taskHubName,
                    StorageConnectionString = storageConnectionString
                };

                IOrchestrationServiceClient orchestrationServiceClient = new AzureStorageOrchestrationService(azureStorageOrchestrationSetting);

                this.taskHubClient = new TaskHubClient(orchestrationServiceClient);
            }
            return this.taskHubClient;
        }
    }
}