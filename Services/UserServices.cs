using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PreparationTracker.Data;
using PreparationTracker.DTO.RequestDTO;
using PreparationTracker.DTO.ResponseDTO;
using PreparationTracker.Model;
using PreparationTracker.Utilities;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
namespace PreparationTracker.Services
{
    public interface IUserService
    {
        Task RegisterUserAsync(UserSignupRequestDto request);
        Task<UserLogInResponseDto> GetUserByEmail(string email, string password);
        Task UpdateUserAsync(Guid userId, UserUpdateRequestDto request);
        Task DeleteUserAsync(Guid userId);
        Task<UserDetailDto> GetUserDetail(Guid userId);

    }
    public class UserServices : IUserService
    {
        private readonly AppDbContext _context;
        private PasswordHasher _passwordHasher;
        private readonly IMapper _mapper;
        private readonly ExamServices _examServices;
        private readonly IConfiguration _configuration;

        public UserServices(AppDbContext context, PasswordHasher passwordHasher, IMapper mapper, ExamServices examServices, IConfiguration configuration)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
            _examServices = examServices;
            _configuration = configuration;
        }

        public async Task RegisterUserAsync(UserSignupRequestDto request)
        {
            if (!UserUtilities.verifyRequiredFields(request))
            {
                throw new Exception("Bad Data");
            }
            try
            {
                var user = _mapper.Map<User>(request);
                user.Password = _passwordHasher.HashPassword(new User(), user.Password);
                await _context.AddAsync(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) {
                throw new Exception("User could not be registered");
            }
        }

        public async Task<UserLogInResponseDto> GetUserByEmail(string email, string password)
        {
            if (email == null || password == null)
            {
                throw new Exception("Missing required field");
            }
            if (!UserUtilities.IsValidEmail(email))
            {
                throw new Exception("Email format invalid");
            }

            User user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (user == null)
            {
                throw new Exception("Email not found");
            }
            if (_passwordHasher.VerifyPassword(new User(), password, user.Password))
            {
                String token = GenerateToken(user);
                var response = _mapper.Map<UserLogInResponseDto>(user);
                response.Token = token;
                return response;
            }
            else
            {
                throw new Exception("Wrong Password");
            }
        }
        public async Task<UserDetailDto> GetUserDetail(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) {
                throw new Exception("User not Found");
            }
            var userDetail = _mapper.Map<UserDetailDto>(user);
            return userDetail;
        }

        public async Task UpdateUserAsync(Guid userId, UserUpdateRequestDto request)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            // Conditionally update only the fields that are present in the request
            if (!string.IsNullOrEmpty(request.Name))
            {
                user.Name = request.Name;
            }

            if (!string.IsNullOrEmpty(request.Email))
            {
                user.Email = request.Email;
            }

            if (!string.IsNullOrEmpty(request.PhoneNumber))
            {
                user.Phone = request.PhoneNumber;
            }

            if (request.DOB.HasValue)
            {
                user.DOB = request.DOB.Value;
            }

            user.UpdatedOn = DateTime.UtcNow;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("User updation failed", ex);
            }
        }


        public async Task DeleteUserAsync(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            foreach (Exam exam in user.Exams)
            {
                _examServices.DeleteExam(exam.ExamId);
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        private string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,_configuration["Jwt:Subject"]),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("UserId", user.UserId.ToString()),
                new Claim("UserEmail", user.Email.ToString())

            };


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1), // Token expiration time
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}


