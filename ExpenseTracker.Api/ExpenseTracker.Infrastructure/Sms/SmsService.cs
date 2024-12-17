using ExpenseTracker.Application.Configurations;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Infrastructure.Sms.Models;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace ExpenseTracker.Infrastructure.Sms;

internal sealed class SmsService : ISmsService
{
    private readonly HttpClient _client;
    private readonly SmsOptions _options;

    public SmsService(HttpClient client, IOptionsMonitor<SmsOptions> options)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _options = options.CurrentValue ?? throw new ArgumentNullException(nameof(options));

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _options.Token);
    }

    public async Task SendMessage(SmsMessage message)
    {
        var content = new MultipartFormDataContent
        {
            { new StringContent(message.ToNumber), "mobile_phone" },
            { new StringContent(message.Message), "message" },
            { new StringContent(_options.FromNumber.ToString()), "from" }
        };

        var response = await _client.PostAsync(_options.Url, content);
        response.EnsureSuccessStatusCode();
    }
}
