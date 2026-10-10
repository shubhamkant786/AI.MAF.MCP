using AI.MAF.MCP.Tools.Tools;
using Microsoft.Extensions.AI;
using ModelContextProtocol.AspNetCore;
using ModelContextProtocol.Server;
using Scalar.AspNetCore;
var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5123); // Listen on port 5123 for HTTP
    options.ListenAnyIP(7266, listenOptions =>
    {
        listenOptions.UseHttps(); // Listen on port 7266 for HTTPS
        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2;
    });
});

// Add services to the container.

//builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

var agentFunction = AIFunctionFactory.Create(FunctionTool.Add);
// Register as an MCP server tool and serve over HTTP
var mcpServerTool = McpServerTool.Create(agentFunction);

builder.Services
    .AddMcpServer()
    //.WithStdioServerTransport()
    .WithHttpTransport(options =>
    {
        options.SessionMode = HttpServerSessionMode.Stateless;
    })
    .WithTools(new[] { mcpServerTool })
    .WithTools<WeatherTool>()
    .WithTools<LocationTool>()
    ;

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.MapOpenApi();
//    app.MapScalarApiReference();
//}

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

app.MapMcp("/mcp");

app.MapGet("/", () => Results.Ok(new
{
    name = "MyMcpServer",
    status = "Running",
    endpoint = "/mcp"
}));

app.Run();
