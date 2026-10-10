using Azure.Core.Pipeline;
using Microsoft.Agents.AI.Hosting;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;
using OpenAI;
using System.ClientModel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


const string deploymentName = "gpt-5.2";
const string endpointOpenAI = "https://shubham-ms-foundry.services.ai.azure.com/openai/v1";
const string apiKey = $"";

var openAIClient = new OpenAIClient(new ApiKeyCredential(apiKey),
         new OpenAIClientOptions() { Endpoint = new Uri(endpointOpenAI) });


IChatClient chatClient = openAIClient.GetChatClient(deploymentName).AsIChatClient()
    .AsBuilder()
    .UseOpenTelemetry(configure: c => c.EnableSensitiveData = true)
    .Build();
builder.Services.AddSingleton(chatClient);

await using var learnMcp = await McpClient.CreateAsync(new ModelContextProtocol.Client
    .HttpClientTransport(new()
    {
        Endpoint = new Uri("https://localhost:7266/mcp"),
        Name = "Weather Zero MCP client",
        TransportMode = HttpTransportMode.StreamableHttp
    }));

var clientTools = await learnMcp.ListToolsAsync();
AITool serverTool = new HostedMcpServerTool(
    serverName: "get_weather_forecast",
    serverAddress: "https://localhost:7266/mcp")
{
    AllowedTools = ["get_weather_forecast"],
    ApprovalMode = HostedMcpServerToolApprovalMode.NeverRequire
};
List<AITool> allTools =
[
    .. clientTools.Where(tool => tool.Name != "get_weather_forecast").Cast<AITool>(),
    serverTool
];

builder.AddAIAgent("mcpToolAgent", (services, name) =>
{

    var chatClient = services.GetRequiredService<IChatClient>();
    return chatClient.AsAIAgent(
    name: name,
        instructions: """
            You are a helpful weather assistant.
            Use the available tools to search and retrieve weather information.
            Be concise and provide direct answers with relevant links.
            """,
        tools: allTools);
});




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
