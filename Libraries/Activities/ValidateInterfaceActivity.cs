using DurableTask.Core;

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