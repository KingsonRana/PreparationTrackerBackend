using Microsoft.EntityFrameworkCore;
using PreparationTracker.Data;
using PreparationTracker.DTO.RequestDTO;
using PreparationTracker.DTO.ResponseDTO;
using PreparationTracker.Model;
using PreparationTracker.Utilities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PreparationTracker.Services
{
    public interface IExamServices
    {
        Task<ExamResponseDto> CreateExam(Guid userId, ExamRequestDto examRequestDto);
        Task<IEnumerable<ExamResponseDto>> GetExamsByUserId(Guid userId);
        Task<ExamResponseDto> UpdateExam(Guid examId, ExamRequestDto examRequestDto);
        Task DeleteExam(Guid examId);
    }

    public class ExamServices : IExamServices
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly TopicProblemService _problemService;
        private readonly ExamUtilities _examUtility;

        public ExamServices(AppDbContext context, IMapper mapper, TopicProblemService problemService, ExamUtilities examUtility)
        {
            _context = context;
            _mapper = mapper;
            _problemService = problemService;
            _examUtility = examUtility;
        }

        public async Task<ExamResponseDto> CreateExam(Guid userId, ExamRequestDto examRequestDto)
        {
            var exam = _mapper.Map<Exam>(examRequestDto);
            exam.CreatedBy = userId;
            exam.UpdatedBy = userId;
            exam.UpdatedOn = DateTime.UtcNow;
            exam.UserId = userId;

            await _context.Exam.AddAsync(exam);
            await _context.SaveChangesAsync();

            return _mapper.Map<ExamResponseDto>(exam);
        }

        public async Task<IEnumerable<ExamResponseDto>> GetExamsByUserId(Guid userId)
        {
            var exams = await _context.Exam.Where(e => e.UserId == userId).ToListAsync();
            return _mapper.Map<IEnumerable<ExamResponseDto>>(exams);
        }

        public async Task<ExamResponseDto> UpdateExam(Guid examId, ExamRequestDto examRequestDto)
        {
            await _examUtility.VerifyExamExistsAsync(examId); 

            var existingExam = await _context.Exam.FirstOrDefaultAsync(e => e.ExamId == examId);

            
            if (!string.IsNullOrEmpty(examRequestDto.ExamName))
            {
                existingExam.ExamName = examRequestDto.ExamName;
            }

            if (examRequestDto.ExamDate.HasValue)
            {
                existingExam.ExamDate = examRequestDto.ExamDate.Value;
            }

           
            if (examRequestDto.DailyHoursSpent != 0)
            {
                existingExam.DailyHoursSpent = examRequestDto.DailyHoursSpent;
            }

            
            existingExam.UpdatedOn = DateTime.UtcNow;
            existingExam.UpdatedBy = existingExam.UserId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("An error occurred while updating the exam. Please try again later.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred. Please try again later.", ex);
            }

            return _mapper.Map<ExamResponseDto>(existingExam);
        }


        public async Task DeleteExam(Guid examId)
        {
            await _examUtility.VerifyExamExistsAsync(examId); // Using utility method

            var exam = await _context.Exam.Include(e => e.Topics).FirstOrDefaultAsync(e => e.ExamId == examId);

            foreach (Topic topic in exam.Topics)
            {
                await _problemService.DeleteTopicAsync(topic.Guid);
            }

            _context.Exam.Remove(exam);
            await _context.SaveChangesAsync();
        }
    }
}
