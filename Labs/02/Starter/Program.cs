using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;

// Load configuration
string filePath = Path.GetFullPath("../../appsettings.json");
var config = new ConfigurationBuilder()
    .AddJsonFile(filePath)
    .Build();

// Set your values in appsettings.json
string modelId = config["modelId"]!;
string endpoint = config["endpoint"]!;
string apiKey = config["apiKey"]!;

// Create a kernel with Azure OpenAI chat completion
var builder = Kernel.CreateBuilder();
builder.AddAzureOpenAIChatCompletion(modelId, endpoint, apiKey);
Kernel kernel = builder.Build();
kernel.ImportPluginFromType<CurrencyConverter>();
kernel.ImportPluginFromType<FlightBookingPlugin>();
//kernel.ImportPluginFromType<CurrencyConverterPlugin>();

kernel.FunctionInvocationFilters.Add(new PermissionFilter());


var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
};

// Setup the assistant chat
var history = new ChatHistory();
Console.WriteLine("Press enter to exit");
Console.WriteLine("Assistant: How may I help you?");
Console.Write("User: ");
string input = Console.ReadLine()!;

while (input != "")
{
    history.AddUserMessage(input);
    await GetReply();
    input = GetInput();
}

string GetInput()
{
    Console.Write("User: ");
    string input = Console.ReadLine()!;
    history.AddUserMessage(input);
    return input;
}

async Task GetReply()
{
    ChatMessageContent reply = await chatCompletionService.GetChatMessageContentAsync(
        history,
        executionSettings: openAIPromptExecutionSettings,
        kernel: kernel
    );
    Console.WriteLine("Assistant: " + reply.ToString());
    history.AddAssistantMessage(reply.ToString());
}


string hbprompt = """
    <message role="system">Instructions: Before providing the user with a travel itinerary, ask how many days their trip is</message>
    <message role="user">I'm going to {{city}}. Can you create an itinerary for me?</message>
    <message role="assistant">Sure, how many days is your trip?</message>
    <message role="user">{{input}}</message>
    <message role="assistant">
    """;

// Create the prompt template config using Handlebars format
var templateFactory = new HandlebarsPromptTemplateFactory();
var promptTemplateConfig = new PromptTemplateConfig()
{
    Template = hbprompt,
    TemplateFormat = "handlebars",
    Name = "GetItinerary",
};

// Create a plugin from the prompt
var promptFunction = kernel.CreateFunctionFromPrompt(promptTemplateConfig, templateFactory);
var itineraryPlugin = kernel.CreatePluginFromFunctions("TravelItinerary", [promptFunction]);

// Add the new plugin to the kernel
kernel.Plugins.Add(itineraryPlugin);

// Setup the assistant chat
history.AddSystemMessage("The current date is 01/10/2025");
history.AddSystemMessage("You are a helpful travel assistant.");
history.AddSystemMessage("Before providing destination recommendations, ask the user about their budget.");


