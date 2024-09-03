using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PreparationTracker.Data;
using PreparationTracker.DTO.RequestDTO;
using PreparationTracker.DTO.ResponseDTO;
using PreparationTracker.Model;
using System;
namespace PreparationTracker.Controllers
{
    [ApiController]
    [Route("/")]
    public class TopicManagementController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public TopicManagementController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("/getAllTopics")]
        public async Task<ActionResult<IEnumerable<TopicResponseDto>>> GetTopics()
        {
            var topics = await _context.Topics.OrderBy(topic => topic.Id).ToListAsync();
            if (topics == null)
            {
                return NoContent();
            }
            var response = _mapper.Map<IEnumerable<TopicResponseDto>>(topics);
            return Ok(response);
        }

        [HttpPost("/addTopic")]
        public async Task<ActionResult<TopicResponseDto>> CreateTopic([FromBody] TopicRequestDto requestDto)
        {
            if (requestDto == null)
            {
                return BadRequest("Topic data is null");
            }
            try
            {
                var topic = _mapper.Map<Topic>(requestDto);
                var maxId = await _context.Topics
                                  .OrderByDescending(t => t.Id)
                                  .Select(t => t.Id)
                                  .FirstOrDefaultAsync();
                topic.Id = maxId + 1;
                await _context.Topics.AddAsync(topic);
                await _context.SaveChangesAsync();
                var response = _mapper.Map<TopicResponseDto>(topic);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateTopic(int id, [FromBody] TopicRequestDto requestDto)
        {
            if (requestDto == null || id <= 0)
            {
                return BadRequest("Invalid topic data");
            }

            var existingTopic = await _context.Topics.FirstOrDefaultAsync(t => t.Id == id);
            if (existingTopic == null)
            {
                return NotFound();
            }

            _mapper.Map(requestDto, existingTopic);
            existingTopic.UpdatedOn = DateTime.UtcNow;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            var response = _mapper.Map<TopicResponseDto>(existingTopic);
            return Ok( response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTopic(int id)
        {
            var topic = await _context.Topics.FirstOrDefaultAsync(t => t.Id == id);
            if (topic == null)
            {
                return NotFound();
            }

             _context.Topics.Remove(topic);
             await  _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("getProblems/{id}")]
        public async Task<ActionResult<IEnumerable<ProblemsResponseDto>>> GetAllProblems(int id)
        {
            var topic = await _context.Topics.FirstOrDefaultAsync(t => t.Id==id);
            if(topic==null)
            {
                return NotFound("Invalid Topic id");
            }
            var problems = await _context.Problems.Where(p=>p.TopicGuid==topic.Guid).ToListAsync();
            if (problems == null)
            {
                return NoContent();
            }
            var response = _mapper.Map<IEnumerable<ProblemsResponseDto>>(problems);
            return Ok(response);

        }

        [HttpPost("addProblems/{id}")]
        public async Task<ActionResult<ProblemsResponseDto>> AddProblem(int id, [FromBody] ProblemsRequestDto request)
        {
            var topic = await _context.Topics.FirstOrDefaultAsync(t => t.Id == id);
            if (topic == null)
            {
                return NotFound("Invalid Topic id");
            }
         
            if (!System.Enum.IsDefined(typeof(PreparationTracker.Enum.ProblemLevel), request.Level)){
                return BadRequest(new { Message = "Invalid value for ProblemLevel." });
            }

            if (!System.Enum.IsDefined(typeof(PreparationTracker.Enum.RequireReWork), request.RequireReWork))
            {
                return BadRequest(new { Message = "Invalid value for RequireReWork." });
            }
            var maxId = await _context.Problems
                                 .OrderByDescending(t => t.Id)
                                 .Select(t => t.Id)
                                 .FirstOrDefaultAsync();
            var problem = _mapper.Map<Problems>(request);
            problem.Id = maxId+1;
            problem.TopicGuid = topic.Guid;
            topic.QuestionSolved = topic.QuestionSolved+1;
            try
            {
               
                await _context.Problems.AddAsync(problem);
                await _context.SaveChangesAsync();
                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            var response = _mapper.Map<ProblemsResponseDto>(problem);
            
            return Ok(response);
        }


        [HttpPut("updateProblem/{id}")]
        public async Task<ActionResult<ProblemsResponseDto>> UpdateProblem(int id, [FromBody] ProblemsRequestDto request)
        {
            var problem = await _context.Problems.FirstOrDefaultAsync(p => p.Id == id);
            if (problem == null)
            {
                return NotFound("Invalid Problem id");
            }

            if (!System.Enum.IsDefined(typeof(PreparationTracker.Enum.ProblemLevel), request.Level))
            {
                return BadRequest(new { Message = "Invalid value for ProblemLevel." });
            }

            if (!System.Enum.IsDefined(typeof(PreparationTracker.Enum.RequireReWork), request.RequireReWork))
            {
                return BadRequest(new { Message = "Invalid value for RequireReWork." });
            }
            problem.Name = request.Name;
            problem.RequireReWork = request.RequireReWork;
            problem.Level = request.Level;
            problem.Link = request.Link;
            problem.UpdatedOn = DateTime.UtcNow;
            try
            {
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            var response = _mapper.Map<ProblemsResponseDto>(problem);
            return Ok(response);
        }

        [HttpDelete("/deleteProblem/{id}")]
        public async Task<ActionResult> DeleteProblem(int id)
        {
            var problem = await _context.Problems.FirstOrDefaultAsync(p => p.Id == id);
            if (problem == null)
            {
                return NotFound("Invalid Problem id");
            }
            var topicGuId = problem.TopicGuid;
            var topic = await _context.Topics.FirstOrDefaultAsync(t=>t.Guid== topicGuId);
            Console.WriteLine(topic.Name);
            _context.Problems.Remove(problem);
            int count = await _context.Problems.CountAsync() -1;
            topic.QuestionSolved = topic.QuestionSolved-1;
            try
            {

                await _context.SaveChangesAsync();
            } catch (Exception e)
            {
                return BadRequest(e.Message);

            }
            return NoContent();
        }
    }
}
