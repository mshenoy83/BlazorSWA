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
    private readonly HttpClient _httpClient;

    public RedirectionTrigger(ILoggerFactory loggerFactory, HttpClient httpClient)
    {
        _httpClient = httpClient;
        _logger = loggerFactory.CreateLogger<RedirectionTrigger>();
    }

    [Function("RedirectionTrigger")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req,
        FunctionContext executionContext)
    {
        var originalUrl = req.Headers.FirstOrDefault(e=>e.Key== "x-ms-original-url");
        _logger.LogInformation("C# HTTP trigger function processed a request for original url. {0}",originalUrl);

        switch (req.Method)
        {
            case "GET":
                return await ProcessGetRequest(req);
            case "POST":
                return await ProcessPostRequest(req);
            default:
                return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
        }
    }

    private async Task<HttpResponseData> ProcessPostRequest(HttpRequestData req)
    {
        var originalUrl = req.Headers.FirstOrDefault(e=>e.Key== "x-ms-original-url");
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var data = JsonConvert.DeserializeObject(requestBody);

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

        response.WriteString("Welcome to Azure Functions!");
        response.WriteString($"Original Url : {originalUrl}");

        return response;
    }

    private async Task<HttpResponseData> ProcessGetRequest(HttpRequestData req)
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