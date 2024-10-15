using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using EAZIS1.Services;

namespace EAZIS2.Services;

public class HttpClientService
{
    private const string BaseUrl = "http://192.168.193.163";
    private const string HandledDocumentsSection = "paths.txt";

    private readonly HttpClient _httpClient;

    public HttpClientService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(BaseUrl);
    }

    public async Task<Response?> SendQuery(string query, int textLength)
    {
        var queryObject = new QueryBody { Query = query, Limit = textLength };
        var response = await _httpClient.PostAsJsonAsync(ApiPathConstants.MainQueryPath, queryObject);

        if (response.IsSuccessStatusCode)
        {
            var queryWords = query.Split(new[] { ' ', ',', '.', ';', ':' }, StringSplitOptions.RemoveEmptyEntries);
            var responseContent = await response.Content.ReadFromJsonAsync<Response>();

            if (string.IsNullOrEmpty(query)) return responseContent;

            responseContent?.Results.ForEach(rb =>
            {
                var matchingWords = queryWords.Where(word => rb.Text.Contains(word, StringComparison.OrdinalIgnoreCase));

                rb.QueryText = string.Join(" ", matchingWords);
            });

            return responseContent;
        }

        throw new HttpRequestException($"Response returned {response.StatusCode} status code.");
    }
}

public class Response
{
    public Response()
    {
        Results = new List<ResponseBody>();
    }

    public List<ResponseBody> Results { get; set; }

    public float Recall { get; set; }

    public float Error { get; set; }

    public float Accuracy { get; set; }

    public float Precision { get; set; }

    [JsonPropertyName("f_measure")] public float FMeasure { get; set; }
}

public class QueryBody
{
    public string Query { get; set; }
    public int Limit { get; set; }
}

public class ResponseBody
{
    public string Doc { get; set; }
    public string Text { get; set; }
    public string? QueryText { get; set; }
}