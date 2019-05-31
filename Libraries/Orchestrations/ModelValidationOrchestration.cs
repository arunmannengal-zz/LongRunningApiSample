using DurableTask.Core;
using Libraries.Activities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Libraries.Orchestrations
{
    public class ModelValidationOrchestration : TaskOrchestration<string, string>
    {
        public override async Task<string> RunTask(OrchestrationContext context, string input)
        {
            var values = await context.ScheduleTask<IList<string>>(typeof(ExtractInterfacesActivity), input).ConfigureAwait(true);
            var assembledValue = string.Empty;
            foreach (var value in values)
            {
                var result = await context.ScheduleTask<string>(typeof(ValidateInterfaceActivity), value).ConfigureAwait(true);
                assembledValue = string.Concat(assembledValue, ":", result);
            }

            return assembledValue;
        }
    }
}