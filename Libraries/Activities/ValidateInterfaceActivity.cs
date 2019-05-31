using DurableTask.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Libraries.Activities
{
    public class ValidateInterfaceActivity : TaskActivity<string, string>
    {
        protected override string Execute(TaskContext context, string input)
        {
            return input;
        }
    }
}