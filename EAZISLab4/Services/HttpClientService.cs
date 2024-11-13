using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace EAZIS4.Services;

public class HttpClientService
{
    private const string BaseUrl = "https://93fa-37-214-25-2.ngrok-free.app";

    private readonly HttpClient _httpClient;

    public HttpClientService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.BaseAddress = new Uri(BaseUrl);
    }

    public async Task<Response?> SendTextAnalysisQuery(string text)
    {
        var response = await _httpClient.PostAsJsonAsync(ApiPathConstants.TextAnalyze, new { text });

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Response>();
        }

        throw new HttpRequestException($"Response returned {response.StatusCode} status code.");
    }

    public async Task<Response?> TranslateText(string text)
    {
        var response = await _httpClient.PostAsJsonAsync(ApiPathConstants.AnalyzeAndTranslate, new { text });

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Response>();
        }

        throw new HttpRequestException($"Response returned {response.StatusCode} status code.");
    }

    public async Task<byte[]> GetDocument(string urlOfDocument)
    {
        return await _httpClient.GetByteArrayAsync(urlOfDocument);
    }

    public async Task<byte[]> GetTree(int orderOfSentense = 1)
    {
        var response = await _httpClient.PostAsync(ApiPathConstants.TreeReport, JsonContent.Create(new { order = orderOfSentense.ToString()}));

        if (response.IsSuccessStatusCode) 
        {
            return await response.Content.ReadAsByteArrayAsync();
        }

        throw new HttpRequestException($"Response returned {response.StatusCode} status code.");
    }
}

public class Response
{
    public Response()
    {
        ResponseList = new ObservableCollection<ResponseBody>();
    }

    [JsonPropertyName("nltk")]
    public ObservableCollection<ResponseBody> ResponseList { get; set; }

    [JsonPropertyName("text")]
    public string? Translation { get; set; }
    public float Count { get; set; }
}

public class ResponseBody
{
    public int Freq { get; set; }
    public required string Word { get; set; }
    public required string Info { get; set; }
}