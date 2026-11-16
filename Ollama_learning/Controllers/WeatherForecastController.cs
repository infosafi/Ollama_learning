using Microsoft.AspNetCore.Mvc;
using OllamaSharp;
using OllamaSharp.Models;

namespace Ollama_learning.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly OllamaApiClient _ollama;
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
            _ollama = new OllamaApiClient(new Uri("http://localhost:11434"), "llama3");
        }

        [HttpPost("summarize")]
        public async Task<IActionResult> Summarize([FromBody] string text)
        {
            var prompt = $"Summarize this ERP data in 3 clear bullet points:\n\n{text}";
            var result = await GenerateTextAsync(prompt);
            return Ok(result);
        }

        [HttpPost("explain-report")]
        public async Task<IActionResult> ExplainReport([FromBody] string report)
        {
            var prompt = $"Explain this financial report in simple language:\n\n{report}";
            var result = await GenerateTextAsync(prompt);
            return Ok(result);
        }

        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody] string message)
        {
            var result = await GenerateTextAsync(message);
            return Ok(result);
        }

        private async Task<string> GenerateTextAsync(string prompt)
        {
            var request = new GenerateRequest
            {
                Model = _ollama.SelectedModel,
                Prompt = prompt,
                Stream = false
            };

            var response = "";

            await foreach (var item in _ollama.GenerateAsync(request))
            {
                if (!string.IsNullOrEmpty(item?.Response))
                {
                    response += item.Response;
                }
            }

            return response;
        }
    }
}