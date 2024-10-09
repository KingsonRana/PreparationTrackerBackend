using Microsoft.EntityFrameworkCore;
using PreparationTracker.Data;

namespace PreparationTracker.Utilities
{
    public  class ExamUtilities
    {
        private readonly AppDbContext _context;

        public ExamUtilities(AppDbContext context)
        {
            _context = context;
        }

        public async Task VerifyExamExistsAsync(Guid examId)
        {
            var exists = await _context.Exam.AnyAsync(e => e.ExamId == examId);
            if (!exists)
            {
                throw new Exception("Exam not found");
            }
        }

    }
}
