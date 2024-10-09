using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PreparationTracker.Data;
using PreparationTracker.DTO.RequestDTO;
using PreparationTracker.DTO.ResponseDTO;
using PreparationTracker.Model;
using PreparationTracker.Services;

namespace PreparationTracker.Controllers
{
    [ApiController]
    [Route("/User")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserServices _userServices;

        public UserController(AppDbContext context, IMapper mapper, UserServices userService)
        {
            _context = context;
            _mapper = mapper;
            _userServices = userService;
        }

        [HttpPost("/createUser")]
        public async Task<ActionResult> AddUser([FromBody] UserSignupRequestDto user)
        {
            try
            {
                await _userServices.RegisterUserAsync(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok("User added successfully");
        }

        [HttpGet("/GetUserByEmail")]
        public async Task<ActionResult<UserLogInResponseDto>> GetUserByEmail([FromBody] UserLogInRequestDto userDetail)
        {
            try
            {
                var user = await _userServices.GetUserByEmail(userDetail.Email, userDetail.Password);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpGet("/GetUserDetail/{userId:guid}")]
        public async Task<ActionResult<UserDetailDto>> GetUserDetail(Guid userId)
        {
            try
            {
                var userDetail = await _userServices.GetUserDetail(userId);
                return Ok(userDetail);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPut("/updateUser/{userId:guid}")]
        public async Task<ActionResult> UpdateUser(Guid userId, [FromBody] UserUpdateRequestDto request)
        {
            try
            {
                await _userServices.UpdateUserAsync(userId, request);
                return Ok("User updated successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpDelete("/deleteUser/{userId:guid}")]
        public async Task<ActionResult> DeleteUser(Guid userId)
        {
            try
            {
                await _userServices.DeleteUserAsync(userId);
                return Ok("User deleted successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

}
