using System.Net.Http.Json;
using MessageService.Domain.Models;
using MessageService.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using VSMS.Common.Settings;
using static MessageService.Domain.Constants.MessageServiceUriEndpoints;

namespace MessageService.Infrastructure.Services;

public class MessageServiceClient : IMessageServiceClient
{
    private readonly ILogger<MessageServiceClient> _logger;
    private readonly HttpClient _httpClient;

    private const string ApiUri = "api/Message";

    public MessageServiceClient(
        ILogger<MessageServiceClient> logger,
        IIntegrationSettings integrationSettings)
    {
        _logger = logger;

        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(integrationSettings.MessageServiceConnectionAddress),
            Timeout = TimeSpan.FromMinutes(1)
        };
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new("application/json"));
    }

    public async Task<bool> SendMessage(SendMessageModel messageModel)
    {
        try
        {
            var result = await _httpClient.PostAsJsonAsync(FormUri(Send), messageModel);
            return result.IsSuccessStatusCode;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }

    private static string FormUri(string endpoint)
    {
        return $"{ApiUri}/{endpoint}";
    }
}
