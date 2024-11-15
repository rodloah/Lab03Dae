using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace MyMicroservice.Controllers // Define el namespace del proyecto
{
    [ApiController]
    [Route("api/[controller]")]
    public class TenorController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public TenorController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchGifs([FromQuery] string query, [FromQuery] int limit = 8)
        {
            string apiKey = "AIzaSyC5dgc9JQ4fPLJ6sF7jGQlHN8X_nZbrokQ"; // Cambia por tu clave de API
            string clientKey = "lab03";
            string apiUrl = $"https://tenor.googleapis.com/v2/search?q={query}&key={apiKey}&client_key={clientKey}&limit={limit}";

            try
            {
                var response = await _httpClient.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                var tenorData = JsonSerializer.Deserialize<TenorResponse>(responseBody);

                var gifs = tenorData.Results.Select(result => new
                {
                    Id = result.Id,
                    GifUrl = result.MediaFormats?.Gif?.Url,
                    Mp4Url = result.MediaFormats?.Mp4?.Url
                }).ToList();

                return Ok(gifs);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, "Error fetching data from Tenor API: " + ex.Message);
            }
        }
    }

    public class TenorResponse
    {
        [JsonPropertyName("results")]
        public List<TenorResult> Results { get; set; }
    }

    public class TenorResult
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("media_formats")]
        public TenorMediaFormats MediaFormats { get; set; }
    }

    public class TenorMediaFormats
    {
        [JsonPropertyName("gif")]
        public TenorMedia Gif { get; set; }

        [JsonPropertyName("mp4")]
        public TenorMedia Mp4 { get; set; }
    }

    public class TenorMedia
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
}
