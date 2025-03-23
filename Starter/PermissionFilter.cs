#pragma warning disable SKEXP0001 
using Microsoft.SemanticKernel;

public class PermissionFilter : IFunctionInvocationFilter
{
    public async Task OnFunctionInvocationAsync(FunctionInvocationContext context, Func<FunctionInvocationContext, Task> next)
    {
        // Debugging log to verify filter execution
        Console.WriteLine($"DEBUG: Intercepted function - Plugin: {context.Function.PluginName}, Function: {context.Function.Name}");

        // Ensure we are checking against the correct plugin name
        if (context.Function.PluginName == "FlightBookingPlugin" && context.Function.Name == "book_flight")
        {
            Console.WriteLine("System Message: The agent requires approval to complete this operation. Do you approve? (Y/N)");
            Console.Write("User: ");
            string shouldProceed = Console.ReadLine()!.Trim().ToUpper();

            if (shouldProceed != "Y")
            {
                Console.WriteLine("DEBUG: User denied the booking.");
                context.Result = new FunctionResult(null, "The flight booking operation was denied by the user.");
                return; // Prevent execution from proceeding
            }
            else
            {
                Console.WriteLine("DEBUG: User approved the booking.");
            }
        }

        await next(context);  // Proceed with execution if approved
    }
}