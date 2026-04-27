using Microsoft.AspNetCore.Mvc;
using OllamaSharp;

namespace Ollama_learning.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        //private readonly OllamaApiClient _ollama;

       

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost("summarize")]
        public async Task<IActionResult> Summarize([FromBody] string text)
        {
           // private readonly OllamaApiClient _ollama;
             OllamaApiClient _ollama = new OllamaApiClient(new Uri("http://localhost:11434/"), "llama3");
            var prompt = $"Summarize this ERP data in 3 bullet points:\n\n{text}";
            var response =  _ollama.GenerateAsync(prompt);
            var finalResponse = "";

            await foreach (var message in response)
            {
                finalResponse += message.Response;
            }

            return Ok(finalResponse);
        }

        [HttpPost("explain-report")]
        public async Task<IActionResult> ExplainReport([FromBody] string report)
        {
            OllamaApiClient _ollama = new OllamaApiClient(new Uri("http://localhost:11434/"), "llama3");
            var prompt = $"Explain this financial report in simple terms:\n\n{report}";
            var response =  _ollama.GenerateAsync(prompt);
            return Ok(response);
        }
    }
}
