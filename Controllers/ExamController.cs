using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PreparationTracker.DTO.RequestDTO;
using PreparationTracker.DTO.ResponseDTO;
using PreparationTracker.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
namespace PreparationTracker.Controllers
{
    [Authorize]
    [ApiController]
    [Route("/Exam")]
    public class ExamController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ExamServices _examServices;

        public ExamController(IMapper mapper, ExamServices examServices)
        {
            _mapper = mapper;
            _examServices = examServices;
        }

        [HttpPost("addExam/{userId:guid}")]
        public async Task<IActionResult> CreateExam(Guid userId, [FromBody] ExamRequestDto examRequestDto)
        {
            if (examRequestDto == null)
            {
                return BadRequest("Add the required fields");
            }

            try
            {
                var examResponseDto = await _examServices.CreateExam(userId, examRequestDto);
                return Ok(examResponseDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getExams/{userId:guid}")]
        public async Task<ActionResult<IEnumerable<ExamResponseDto>>> GetExams(Guid userId)
        {
            var exams = await _examServices.GetExamsByUserId(userId);
            if (exams == null)
            {
                return NotFound();
            }

            return Ok(exams);
        }

        [HttpPut("updateExam/{examId:guid}")]
        public async Task<ActionResult<ExamResponseDto>> UpdateExam(Guid examId, [FromBody] ExamRequestDto examRequestDto)
        {
            if (examRequestDto == null)
            {
                return BadRequest("Add the required fields");
            }

            try
            {
                var updatedExam = await _examServices.UpdateExam(examId, examRequestDto);
                return Ok(updatedExam);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("deleteExam/{examId:guid}")]
        public async Task<ActionResult> DeleteExam(Guid examId)
        {
            try
            {
                await _examServices.DeleteExam(examId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
