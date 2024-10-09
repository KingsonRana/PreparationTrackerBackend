using PreparationTracker.DTO.RequestDTO;
using PreparationTracker.DTO.ResponseDTO;
using PreparationTracker.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PreparationTracker.Data;
using PreparationTracker.DTO.RequestDTO;
using PreparationTracker.DTO.ResponseDTO;
using PreparationTracker.Model;
using System.Linq;
using PreparationTracker.Utilities;


namespace PreparationTracker.Services
{
    public interface ITopicService
    {
        Task<IEnumerable<TopicResponseDto>> GetTopicsAsync(Guid examId);
        Task<IEnumerable<TopicResponseDto>> GetSubTopicsAsync(Guid parentId);
        Task<TopicResponseDto> CreateTopicAsync(Guid examId, TopicRequestDto requestDto);
        Task<TopicResponseDto> CreateSubTopicAsync(Guid parentId, Guid examId, TopicRequestDto requestDto);
        Task<TopicResponseDto> UpdateTopicAsync(Guid id, TopicRequestDto requestDto);
        Task DeleteTopicAsync(Guid id);
        Task<IEnumerable<ProblemsResponseDto>> GetProblemsAsync(Guid id);
        Task<ProblemsResponseDto> AddProblemAsync(Guid id, ProblemsRequestDto request);
        Task<ProblemsResponseDto> UpdateProblemAsync(Guid id, ProblemsRequestDto request);
        Task DeleteProblemAsync(Guid id);
    }
    public class TopicProblemService : ITopicService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ExamUtilities _examUtility;

        public TopicProblemService(AppDbContext context, IMapper mapper, ExamUtilities examUtilities)
        {
            _context = context;
            _mapper = mapper;
            _examUtility = examUtilities;
        }

        public async Task<IEnumerable<TopicResponseDto>> GetTopicsAsync(Guid examId)
        {
            await _examUtility.VerifyExamExistsAsync(examId);
            var topics = await _context.Topics
                .Where(t => t.ExamId == examId && t.ParentId == null)
                .OrderBy(topic => topic.Id)
                .ToListAsync();

            return _mapper.Map<IEnumerable<TopicResponseDto>>(topics);
        }

        public async Task<IEnumerable<TopicResponseDto>> GetSubTopicsAsync(Guid parentId)
        {
            var topic = await _context.Topics.Include(t => t.SubTopics).FirstOrDefaultAsync(t => t.Guid == parentId);
            return _mapper.Map<IEnumerable<TopicResponseDto>>(topic?.SubTopics);
        }

        public async Task<TopicResponseDto> CreateTopicAsync(Guid examId, TopicRequestDto requestDto)
        {
            await _examUtility.VerifyExamExistsAsync(examId);

            var topic = _mapper.Map<Topic>(requestDto);
            var maxId = await _context.Topics.OrderByDescending(t => t.Id).Select(t => t.Id).FirstOrDefaultAsync();
            topic.Id = maxId + 1;
            topic.ExamId = examId;
            await _context.Topics.AddAsync(topic);
            await _context.SaveChangesAsync();

            return _mapper.Map<TopicResponseDto>(topic);
        }

        public async Task<TopicResponseDto> CreateSubTopicAsync(Guid parentId, Guid examId, TopicRequestDto requestDto)
        {
            await _examUtility.VerifyExamExistsAsync(examId);

            var topic = _mapper.Map<Topic>(requestDto);
            var maxId = await _context.Topics.OrderByDescending(t => t.Id).Select(t => t.Id).FirstOrDefaultAsync();
            topic.Id = maxId + 1;
            topic.ParentId = parentId;
            topic.ExamId = examId;
            await _context.Topics.AddAsync(topic);
            await _context.SaveChangesAsync();

            return _mapper.Map<TopicResponseDto>(topic);
        }

        public async Task<TopicResponseDto> UpdateTopicAsync(Guid id, TopicRequestDto requestDto)
        {
            var existingTopic = await _context.Topics.FirstOrDefaultAsync(t => t.Guid == id);
            if (existingTopic == null)
            {
                throw new Exception("Topic not found");
            }

            _mapper.Map(requestDto, existingTopic);
            existingTopic.UpdatedOn = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return _mapper.Map<TopicResponseDto>(existingTopic);
        }

        public async Task DeleteTopicAsync(Guid id)
        {
            var topic = await _context.Topics.Include(t => t.SubTopics).Include(t => t.Problems).FirstOrDefaultAsync(t => t.Guid == id);
            if (topic == null) throw new Exception("Topic not found");

            async Task DeleteSubTopics(IEnumerable<Topic> subTopics)
            {
                foreach (var subTopic in subTopics.ToList())
                {
                    var subSubTopics = await _context.Topics.Where(t => t.ParentId == subTopic.Guid).ToListAsync();
                    await DeleteSubTopics(subSubTopics);
                    _context.Topics.Remove(subTopic);
                }
            }

            await DeleteSubTopics(topic.SubTopics);
            _context.Problems.RemoveRange(topic.Problems);
            _context.Topics.Remove(topic);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ProblemsResponseDto>> GetProblemsAsync(Guid id)
        {
            var topic = await _context.Topics.Include(t => t.Problems).FirstOrDefaultAsync(t => t.Guid == id);
            return _mapper.Map<IEnumerable<ProblemsResponseDto>>(topic?.Problems);
        }

        public async Task<ProblemsResponseDto> AddProblemAsync(Guid id, ProblemsRequestDto request)
        {
            var topic = await _context.Topics.FirstOrDefaultAsync(t => t.Guid == id);
            if (topic == null) throw new Exception("Topic not found");

            var maxId = await _context.Problems.OrderByDescending(p => p.Id).Select(p => p.Id).FirstOrDefaultAsync();
            var problem = _mapper.Map<Problems>(request);
            problem.Id = maxId + 1;
            problem.TopicGuid = id;
            topic.QuestionSolved += 1;
            await _context.Problems.AddAsync(problem);
            await _context.SaveChangesAsync();

            return _mapper.Map<ProblemsResponseDto>(problem);
        }

        public async Task<ProblemsResponseDto> UpdateProblemAsync(Guid id, ProblemsRequestDto request)
        {
            var problem = await _context.Problems.FirstOrDefaultAsync(p => p.Guid == id);
            if (problem == null) throw new Exception("Problem not found");

            _mapper.Map(request, problem);
            problem.UpdatedOn = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return _mapper.Map<ProblemsResponseDto>(problem);
        }

        public async Task DeleteProblemAsync(Guid id)
        {
            var problem = await _context.Problems.FirstOrDefaultAsync(p => p.Guid == id);
            if (problem == null) throw new Exception("Problem not found");

            _context.Problems.Remove(problem);
            await _context.SaveChangesAsync();
        }
    }
}

