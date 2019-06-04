using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DurableTask.Core;
using Libraries.Orchestrations;
using LongRunningApi.Models;
using LongRunningApi.TaskHub;
using Microsoft.AspNetCore.Mvc;

namespace LongRunningApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LongRunningController : ControllerBase
    {
        private readonly TaskHubClient taskHubClient;

        // Read this from Configuration
        private const string storageConnectionString = "UseDevelopmentStorage=true;DevelopmentStorageProxyUri=http://127.0.0.1:10002/";

        // Read this from Configuration
        private const string taskHubName = "PnPRepoValidation";

        public LongRunningController()
        {
            this.taskHubClient = ConnectionManager.Instance.GetTaskHubClient(taskHubName, storageConnectionString);
        }

        [HttpPost]
        public async Task<IActionResult> InvokeLongRunningApiAsync([FromBody]LongRunningRequest longRunningRequest)
        {
            var orchestrationInstance = await this.taskHubClient.CreateOrchestrationInstanceAsync(typeof(ModelValidationOrchestration), longRunningRequest.Value).ConfigureAwait(true);
            var orchestrationState = await this.taskHubClient.WaitForOrchestrationAsync(orchestrationInstance, TimeSpan.FromMilliseconds(1)).ConfigureAwait(true);

            return this.Accepted(orchestrationInstance.InstanceId, orchestrationState);
        }

        [HttpGet]
        [Route("{instanceId}")]
        public async Task<IActionResult> GetLongRunningApiStatusAsync(string instanceId)
        {
            var orchestrationState = await this.taskHubClient.GetOrchestrationStateAsync(instanceId).ConfigureAwait(true);
            return this.Ok(orchestrationState);
        }
    }
}