import os
import json
import asyncio
from semantic_kernel import Kernel
from semantic_kernel.connectors.ai.open_ai import AzureChatCompletion
from semantic_kernel.functions import KernelFunction
from semantic_kernel.prompt_template import PromptTemplateConfig
from semantic_kernel.prompt_template.handlebars_prompt_template import HandlebarsPromptTemplate

# Load configuration
file_path = os.path.join(os.path.dirname(__file__), "../appsettings.json")
with open(file_path, 'r') as config_file:
    config = json.load(config_file)

# Set your values from appsettings.json
model_id = config["modelId"]
endpoint = config["endpoint"]
api_key = config["apiKey"]

# Create a kernel with Azure OpenAI chat completion
kernel = Kernel()
chat_completion_service = AzureChatCompletion(deployment_name=model_id, 
                                              endpoint=endpoint, 
                                              api_key=api_key)
# Add the Azure OpenAI chat completion service to the kernel
kernel.add_service(chat_completion_service)

# Import plugins (assuming these are Python equivalents of the C# plugins)
from currency_converter import CurrencyConverter
from flight_booking_plugin import FlightBookingPlugin

# Initialize the plugins
currency_converter = CurrencyConverter()
flight_booking_plugin = FlightBookingPlugin()

# Import skills
kernel.add_plugin(currency_converter, "currency")
kernel.add_plugin(flight_booking_plugin, "flights")

# Create a handlebars prompt for travel itinerary
hbprompt = """
    <message role="system">Instructions: Before providing the user with a travel itinerary, ask how many days their trip is</message>
    <message role="user">I'm going to {{city}}. Can you create an itinerary for me?</message>
    <message role="assistant">Sure, how many days is your trip?</message>
    <message role="user">{{input}}</message>
    <message role="assistant">
    """

# Create prompt template config
prompt_config = PromptTemplateConfig(
    template=hbprompt,
    template_format="handlebars",
    name="GetItinerary"
)

from semantic_kernel.prompt_template.prompt_template_config import PromptTemplateConfig
from semantic_kernel.functions.kernel_function_metadata import KernelFunctionMetadata
from semantic_kernel.contents import ChatHistory

# Create a function from the prompt using the correct API for Python Semantic Kernel
prompt_template = HandlebarsPromptTemplate(
    prompt_template_config=prompt_config
)

# Use the simpler approach that's more likely to be available
# First, create a regular semantic function (using the older API style)

# Define a semantic function directly
prompt_function = KernelFunction.from_prompt(
    prompt_template=prompt_template,
    function_name="GetItinerary",
    description="Creates a travel itinerary based on location and days",
    plugin_name="TravelItinerary"
)

# Add the function to the kernel
kernel.add_plugin(prompt_function, "TravelItinerary")

# Initialize chat history
chat_history = ChatHistory()

# Add system messages to establish the assistant's personality
chat_history.add_system_message("The current date is 01/10/2025")
chat_history.add_system_message("You are a helpful travel assistant.")
chat_history.add_system_message("Before providing destination recommendations, ask the user about their budget.")

# Function to get response from the AI
async def get_reply():
    # Using the service name instead of the service object
    streaming_response = kernel.invoke_stream(
        "openai",  # Use the service ID/name instead of the service object
        input=chat_history
    )
    response = ""
    async for chunk in streaming_response:
        chunk_text = str(chunk)
        response += chunk_text
        print(chunk_text, end="", flush=True)
    print()  # Add a newline after the complete response
    chat_history.add_assistant_message(response)
    return response

# Main async function to run the chat
async def main():
    print("Press enter to exit")
    print("Assistant: How may I help you?")
    print("User: ", end="")
    user_input = input()

    while user_input != "":
        chat_history.add_user_message(user_input)
        await get_reply()
        print("User: ", end="")
        user_input = input()

# Run the async main function
if __name__ == "__main__":
    asyncio.run(main())
