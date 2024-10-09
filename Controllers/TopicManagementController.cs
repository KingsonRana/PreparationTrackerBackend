using Microsoft.AspNetCore.Mvc;
using PreparationTracker.DTO.RequestDTO;
using PreparationTracker.DTO.ResponseDTO;
using PreparationTracker.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PreparationTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TopicManagementController : ControllerBase
    {
        private readonly TopicProblemService _topicService;

        public TopicManagementController(TopicProblemService topicService)
        {
            _topicService = topicService;
        }

        // GET: api/TopicManagement/topics/{examId}
        [HttpGet("topics/{examId}")]
        public async Task<IActionResult> GetTopics(Guid examId)
        {
            var topics = await _topicService.GetTopicsAsync(examId);
            return Ok(topics);
        }

        // GET: api/TopicManagement/subtopics/{parentId}
        [HttpGet("subtopics/{parentId}")]
        public async Task<IActionResult> GetSubTopics(Guid parentId)
        {
            var subTopics = await _topicService.GetSubTopicsAsync(parentId);
            return Ok(subTopics);
        }

        // POST: api/TopicManagement/topics/{examId}
        [HttpPost("topics/{examId}")]
        public async Task<IActionResult> CreateTopic(Guid examId, [FromBody] TopicRequestDto requestDto)
        {
            var createdTopic = await _topicService.CreateTopicAsync(examId, requestDto);
            return CreatedAtAction(nameof(GetTopics), new { examId = examId }, createdTopic);
        }

        // POST: api/TopicManagement/subtopics/{parentId}/{examId}
        [HttpPost("subtopics/{parentId}/{examId}")]
        public async Task<IActionResult> CreateSubTopic(Guid parentId, Guid examId, [FromBody] TopicRequestDto requestDto)
        {
            var createdSubTopic = await _topicService.CreateSubTopicAsync(parentId, examId, requestDto);
            return CreatedAtAction(nameof(GetSubTopics), new { parentId = parentId }, createdSubTopic);
        }

        // PUT: api/TopicManagement/topics/{id}
        [HttpPut("topics/{id}")]
        public async Task<IActionResult> UpdateTopic(Guid id, [FromBody] TopicRequestDto requestDto)
        {
            var updatedTopic = await _topicService.UpdateTopicAsync(id, requestDto);
            return Ok(updatedTopic);
        }

        // DELETE: api/TopicManagement/topics/{id}
        [HttpDelete("topics/{id}")]
        public async Task<IActionResult> DeleteTopic(Guid id)
        {
            await _topicService.DeleteTopicAsync(id);
            return NoContent();
        }

        // GET: api/TopicManagement/problems/{topicId}
        [HttpGet("problems/{topicId}")]
        public async Task<IActionResult> GetProblems(Guid topicId)
        {
            var problems = await _topicService.GetProblemsAsync(topicId);
            return Ok(problems);
        }

        // POST: api/TopicManagement/problems/{topicId}
        [HttpPost("problems/{topicId}")]
        public async Task<IActionResult> AddProblem(Guid topicId, [FromBody] ProblemsRequestDto request)
        {
            var createdProblem = await _topicService.AddProblemAsync(topicId, request);
            return CreatedAtAction(nameof(GetProblems), new { topicId = topicId }, createdProblem);
        }

        // PUT: api/TopicManagement/problems/{id}
        [HttpPut("problems/{id}")]
        public async Task<IActionResult> UpdateProblem(Guid id, [FromBody] ProblemsRequestDto request)
        {
            var updatedProblem = await _topicService.UpdateProblemAsync(id, request);
            return Ok(updatedProblem);
        }

        // DELETE: api/TopicManagement/problems/{id}
        [HttpDelete("problems/{id}")]
        public async Task<IActionResult> DeleteProblem(Guid id)
        {
            await _topicService.DeleteProblemAsync(id);
            return NoContent();
        }
    }
}
