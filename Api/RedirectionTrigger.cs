using System.Collections.Generic;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Api;

public class RedirectionTrigger
{
    private readonly ILogger _logger;

    public RedirectionTrigger(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<RedirectionTrigger>();
    }

    [Function("RedirectionTrigger")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "put","delete")] HttpRequestData req,
        FunctionContext executionContext)
    {
        var originalUrl = req.Headers.FirstOrDefault(e=>e.Key == "x-ms-original-url").Value.FirstOrDefault()?.ToString();
        _logger.LogInformation("C# HTTP trigger function processed a request for original url. {0}",originalUrl);
        
        switch (req.Method)
        {
            case "GET":
                return ProcessGetRequest(req);
            case "POST":
                return await ProcessPostRequest(req);
            default:
                var response = req.CreateResponse(HttpStatusCode.OK);
                response.WriteString("Welcome to Azure Functions!");
                response.WriteString($"Original Url : {originalUrl}");
                response.WriteString($"Original Method : {req.Method}");
                return response;
        }
    }

    private HttpResponseData ProcessGetRequest(HttpRequestData req)
    {
        var originalUrl = req.Headers.FirstOrDefault(e=>e.Key == "x-ms-original-url").Value.FirstOrDefault()?.ToString();
    

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

        response.WriteString("Welcome to Azure Functions!");
        response.WriteString($"Original Url : {originalUrl}");

        return response;
    }

    private async Task<HttpResponseData> ProcessPostRequest(HttpRequestData req)
    {
        // Read the request body
        var originalUrl = req.Headers.FirstOrDefault(e=>e.Key== "x-ms-original-url");
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var data = JsonConvert.DeserializeObject(requestBody);

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

        response.WriteString("Welcome to Azure Functions!");
        response.WriteString($"Original Url : {originalUrl}");
        response.WriteString($"Body : {data}");

        return response;
    }
}