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

        [HttpGet("/getSubTopics/{id}")]
        public async Task<ActionResult<IEnumerable<TopicResponseDto>>> GetTopics(Guid id)
        {
            var topics = await _context.Topics.Include(t=>t.SubTopics).FirstOrDefaultAsync(t => t.Guid == id);
            if (topics == null)
            {
                return NoContent();
            }
            var subTopics = topics.SubTopics;
            Console.WriteLine("Number of childrens = " + subTopics.Count);

            var response = _mapper.Map<IEnumerable<TopicResponseDto>>(subTopics);
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
        public async Task<ActionResult> UpdateTopic(Guid id, [FromBody] TopicRequestDto requestDto)
        {
            if (requestDto == null || id == Guid.Empty)
            {
                return BadRequest("Invalid topic data");
            }

            var existingTopic = await _context.Topics.FirstOrDefaultAsync(t => t.Guid == id);
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
        public async Task<ActionResult> DeleteTopic(Guid id)
        {
            
            var topic = await _context.Topics
                .Include(t => t.SubTopics)
                .Include(t => t.Problems)
                .FirstOrDefaultAsync(t => t.Guid == id);

            if (topic == null)
            {
                return NotFound();
            }

            
            async Task DeleteSubTopics(IEnumerable<Topic> subTopics)
            {
                foreach (var subTopic in subTopics.ToList()) 
                {
                   
                    var subSubTopics = await _context.Topics
                        .Where(t => t.ParentId == subTopic.Guid)
                        .ToListAsync();

         
                    await DeleteSubTopics(subSubTopics);

                    _context.Topics.Remove(subTopic);
                }
            }

           
            await DeleteSubTopics(topic.SubTopics);

            _context.Problems.RemoveRange(topic.Problems); 
            _context.Topics.Remove(topic);

          
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpGet("getProblems/{id}")]
        public async Task<ActionResult<IEnumerable<ProblemsResponseDto>>> GetAllProblems(Guid id)
        {
            var topic = await _context.Topics
        .Include(t => t.Problems) // Include problems navigation property
        .FirstOrDefaultAsync(t => t.Guid == id);

            if (topic == null)
            {
                return NotFound("Invalid Topic ID");
            }

            var problems = topic.Problems; // Access problems via navigation property

            if (problems == null || !problems.Any())
            {
                return NoContent(); // Return NoContent if no problems found
            }

            var response = _mapper.Map<IEnumerable<ProblemsResponseDto>>(problems);
            return Ok(response);

        }

        [HttpPost("addProblems/{id}")]
        public async Task<ActionResult<ProblemsResponseDto>> AddProblem(Guid id, [FromBody] ProblemsRequestDto request)
        {
            var topic = await _context.Topics.FirstOrDefaultAsync(t => t.Guid == id);
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
            problem.TopicGuid = id;
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
        public async Task<ActionResult<ProblemsResponseDto>> UpdateProblem(Guid id, [FromBody] ProblemsRequestDto request)
        {
            var problem = await _context.Problems.FirstOrDefaultAsync(p => p.Guid == id);
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
        public async Task<ActionResult> DeleteProblem(Guid id)
        {
            var problem = await _context.Problems.FirstOrDefaultAsync(p => p.Guid == id);
            if (problem == null)
            {
                return NotFound("Invalid Problem id");
            }
            var topicGuId = problem.TopicGuid;
            var topic = await _context.Topics.FirstOrDefaultAsync(t=>t.Guid== topicGuId);
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
