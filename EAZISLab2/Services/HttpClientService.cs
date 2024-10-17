using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace EAZIS2.Services;

public class HttpClientService
{
    private const string BaseUrl = "http://192.168.112.163";

    private readonly HttpClient _httpClient;

    public HttpClientService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.BaseAddress = new Uri(BaseUrl);
    }

    public async Task<Response?> SendQuery(Dictionary<string, byte[]> files, string recognitionMethod)
    {
        using var formData = new MultipartFormDataContent();

        foreach (var file in files)
        {
            formData.Add(new ByteArrayContent(file.Value)
            {
                Headers = { ContentType = MediaTypeHeaderValue.Parse("text/html") }
            }, "files", file.Key);
        }

        var response = await _httpClient.PostAsync(ApiPathConstants.MainQueryPath + $"?recognition_method={recognitionMethod}", formData);

        if (!response.IsSuccessStatusCode) throw new HttpRequestException($"Response returned {response.StatusCode} status code.");

        var responseContent = await response.Content.ReadFromJsonAsync<Response>();

        return responseContent;
    }
}

public class Response
{
    public Response()
    {
        ResponseList = new List<ResponseBody>();
    }

    [JsonPropertyName("response")] public List<ResponseBody> ResponseList { get; set; }

    public float Precision { get; set; }


    public override string ToString()
    {
        var result = string.Empty;

        foreach (var responseBody in ResponseList)
        {
            result += $"{responseBody.Doc}\\t{responseBody.Language}\n";
        }

        result += $"\nPrecision: {Precision}";

        return result;
    }
}

public class ResponseBody
{
    public string Doc { get; set; }
    public string Language { get; set; }
}