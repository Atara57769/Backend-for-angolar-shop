using Entities;
using Microsoft.AspNetCore.Mvc;
using Services;
using System;
using System.Threading.Tasks;

namespace WebApiShop.Controllers
{
    [Route("api/chatbot")]
    [ApiController]
    public class ChatBotController : ControllerBase
    {
        private readonly IChatBotService _chatBotService;

        public ChatBotController(IChatBotService chatBotService)
        {
            _chatBotService = chatBotService;
        }

        [HttpPost]
        [HttpPost("ask")]
        public async Task<IActionResult> Post([FromBody] ChatBotRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest(new { message = "Message is required." });
            }

            try
            {
                var response = await _chatBotService.AskQuestionAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Return HTTP 500 with meaningful message
                return StatusCode(500, new { message = "Error communicating with the RAG API.", details = ex.Message });
            }
        }

        [HttpPost("update-db")]
        public async Task<IActionResult> UpdateDatabase()
        {
            try
            {
                var success = await _chatBotService.UpdateDatabaseAsync();
                if (success)
                {
                    return Ok(new { message = "Vector database updated successfully." });
                }
                return StatusCode(500, new { message = "Failed to update vector database." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error communicating with the Python service.", details = ex.Message });
            }
        }
    }
}

