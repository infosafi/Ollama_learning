using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;
using OllamaSharp;
using System.ComponentModel;
using Microsoft.Agents.AI;

namespace Ollama_learning.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OllamaController : Controller
    {
        private readonly AIAgent _agent;
        private readonly ILogger<OllamaController> _logger;

        public OllamaController(ILogger<OllamaController> logger)
        {
            _logger = logger;

            // 1. Create Ollama client
            var ollamaClient = new OllamaApiClient(new Uri("http://localhost:11434"), "llama3.2"); // use your model name

            // 2. Create Agent with Tools
            _agent = new ChatClientAgent(
                ollamaClient,
                instructions: @"You are a helpful Business Assistant for an ERP system.
                                You can use tools when needed.
                                Always be clear and professional.",
                name: "BusinessAssistant",
                tools:
                [
                    AIFunctionFactory.Create(GetCurrentTime),
                    AIFunctionFactory.Create(Calculate),
                    AIFunctionFactory.Create(GetOrderStatus)
                ]
            );
        }

        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody] string message)
        {
            var response = await _agent.RunAsync(message);
            return Ok(response.ToString());
        }

        // ===================== TOOLS =====================

        [Description("Get the current date and time")]
        private static string GetCurrentTime()
        {
            return DateTime.Now.ToString("dddd, dd MMMM yyyy hh:mm tt");
        }

        [Description("Calculate two numbers. Operations: add, subtract, multiply, divide")]
        private static string Calculate(
            [Description("First number")] double a,
            [Description("Second number")] double b,
            [Description("Operation name")] string operation)
        {
            return operation.ToLower() switch
            {
                "add" or "+" => $"{a} + {b} = {a + b}",
                "subtract" or "-" => $"{a} - {b} = {a - b}",
                "multiply" or "*" => $"{a} * {b} = {a * b}",
                "divide" or "/" => b == 0 ? "Cannot divide by zero" : $"{a} / {b} = {a / b}",
                _ => "Unknown operation"
            };
        }

        [Description("Get status of a sales order by Order ID")]
        private static string GetOrderStatus(
            [Description("The Order ID")] string orderId)
        {
            // Fake data (later you will connect real ERP database)
            var orders = new Dictionary<string, string>
            {
                { "SO-1001", "Confirmed - Ready for Delivery" },
                { "SO-1002", "Pending Approval" },
                { "SO-1003", "Shipped - Tracking: TRK998877" },
                { "SO-1004", "Cancelled" }
            };

            return orders.TryGetValue(orderId.ToUpper(), out var status)
                ? $"Order {orderId}: {status}"
                : $"Order {orderId} not found";
        }
    }
}
