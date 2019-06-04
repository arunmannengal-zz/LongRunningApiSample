using DurableTask.Core;

using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Libraries.Activities
{
    public class ExtractInterfacesActivity : TaskActivity<string, IList<string>>
    {
        protected override IList<string> Execute(TaskContext context, string input)
        {
            Thread.Sleep(100);
            return input.Split().ToList();
        }
    }
}