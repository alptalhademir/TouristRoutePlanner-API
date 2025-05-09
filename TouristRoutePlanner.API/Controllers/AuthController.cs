using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TouristRoutePlanner.API.DTOs;
using TouristRoutePlanner.API.Models;
using TouristRoutePlanner.API.Repositories.Interfaces;
using TouristRoutePlanner.API.Services.Implementations;
using TouristRoutePlanner.API.Services.Interfaces;

namespace TouristRoutePlanner.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly ITokenRepository tokenRepository;
        private readonly IEmailService emailService;
        private readonly IMapper mapper;

        public AuthController(UserManager<User> userManager, ITokenRepository tokenRepository,
            IEmailService emailService, IMapper mapper)
        {
            this.userManager = userManager;
            this.tokenRepository = tokenRepository;
            this.emailService = emailService;
            this.mapper = mapper;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            var user = new User
            {
                UserName = registerRequestDto.Username,
                Email = registerRequestDto.Username,
                FirstName = registerRequestDto.FirstName,
                LastName = registerRequestDto.LastName,
                DateOfBirth = registerRequestDto.DateOfBirth
            };

            var result = await userManager.CreateAsync(user, registerRequestDto.Password);

            if (result.Succeeded)
            {
                result = await userManager.AddToRoleAsync(user, "User");

                if (result.Succeeded)
                {
                    var confirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(user);

                    await emailService.SendEmailConfirmationAsync(user.Email, confirmationToken);

                    return Ok(new { message = "User was registered. Please confirm your email." });
                }
            }

            return BadRequest(new { message = "User registration failed." });
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var user = await userManager.FindByEmailAsync(loginRequestDto.Username);

            if (user != null)
            {
                if (!user.EmailConfirmed)
                {
                    return BadRequest(new { message = "Please confirm your email before logging in." });
                }

                var checkPasswordResult = await userManager.CheckPasswordAsync(user, loginRequestDto.Password);

                if (checkPasswordResult)
                {
                    var roles = await userManager.GetRolesAsync(user);

                    var jwtToken = tokenRepository.CreateJWTToken(user, roles.ToList());

                    var response = new LoginResponseDto
                    {
                        jwtToken = jwtToken,
                        User = mapper.Map<UserDto>(user)
                    };

                    return Ok(response);
                }
            }

            return BadRequest(new { message = "Username or password is incorrect" });
        }

        [HttpPost]
        [Route("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto forgotPasswordRequestDto)
        {
            var user = await userManager.FindByEmailAsync(forgotPasswordRequestDto.Email);
            if (user == null)
            {
                // Return OK even if user doesn't exist to prevent email enumeration
                return Ok(new { message = "If your email is registered, you will receive a password reset code." });
            }

            // Generate password reset token
            var token = await userManager.GeneratePasswordResetTokenAsync(user);

            // Send email with reset token
            try
            {
                await emailService.SendPasswordResetEmailAsync(user.Email, token);
                return Ok(new { message = "Password reset code has been sent to your email." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Error sending password reset email." });
            }
        }

        [HttpPost]
        [Route("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto resetPasswordRequestDto)
        {
            var user = await userManager.FindByEmailAsync(resetPasswordRequestDto.Email);
            if (user == null)
            {
                return BadRequest(new { message = "Invalid request." });
            }

            var result = await userManager.ResetPasswordAsync(
                user,
                resetPasswordRequestDto.Token,
                resetPasswordRequestDto.NewPassword);

            if (result.Succeeded)
            {
                return Ok(new { message = "Password has been reset successfully." });
            }

            var errors = result.Errors.Select(e => e.Description);
            return BadRequest(new { message = "Password reset failed", Errors = errors });
        }

        [HttpPost]
        [Route("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail([FromBody] EmailConfirmationRequestDto request)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return BadRequest(new { message = "Invalid email" });
            }

            if (user.EmailConfirmed)
            {
                return BadRequest(new { message = "Email already confirmed" });
            }

            var result = await userManager.ConfirmEmailAsync(user, request.Token);
            if (result.Succeeded)
            {
                return Ok(new { message = "Email has been confirmed successfully" });
            }

            return BadRequest(new { message = "Email confirmation failed" });
        }

        [HttpGet]
        [Route("Profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Please login to proceed.");
            

            var user = await userManager.FindByIdAsync(userId);
            if (user == null) 
                return NotFound("User not found.");

            var userDto = mapper.Map<UserDto>(user);
            return Ok(userDto);

        }

        [HttpPut]
        [Route("Profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserRequestDto updateUserDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Please login to proceed." });

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound(new { message = "User not found." });

            // Update basic info
            user.FirstName = updateUserDto.FirstName;
            user.LastName = updateUserDto.LastName;
            user.DateOfBirth = updateUserDto.DateOfBirth;

            // Handle password change if provided
            if (!string.IsNullOrEmpty(updateUserDto.CurrentPassword) &&
                !string.IsNullOrEmpty(updateUserDto.NewPassword))
            {
                var passwordCheck = await userManager.CheckPasswordAsync(user, updateUserDto.CurrentPassword);
                if (!passwordCheck)
                    return BadRequest(new { message = "Current password is incorrect." });

                var passwordResult = await userManager.ChangePasswordAsync(
                    user,
                    updateUserDto.CurrentPassword,
                    updateUserDto.NewPassword);

                if (!passwordResult.Succeeded)
                    return BadRequest(new { message = "Password change failed.", errors = passwordResult.Errors });
            }

            // Save other changes
            var updateResult = await userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return BadRequest(new { message = "Profile update failed.", errors = updateResult.Errors });

            // Return updated user info
            var userDto = mapper.Map<UserDto>(user);
            return Ok(new { message = "Profile updated successfully.", user = userDto });
        }

    }
}
