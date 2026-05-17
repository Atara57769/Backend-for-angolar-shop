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

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] ChatBotRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Question))
            {
                return BadRequest(new { message = "Question is required." });
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
    }
}
