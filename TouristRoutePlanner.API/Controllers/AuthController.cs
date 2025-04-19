using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        public AuthController(UserManager<User> userManager, ITokenRepository tokenRepository,
            IEmailService emailService)
        {
            this.userManager = userManager;
            this.tokenRepository = tokenRepository;
            this.emailService = emailService;
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

                    return Ok("User was registered. Please login.");
                }
            }

            return BadRequest("User registration failed.");
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
                        jwtToken = jwtToken
                    };

                    return Ok(response);
                }
            }

            return BadRequest("Username or password is incorrect");
        }

        [HttpPost]
        [Route("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto forgotPasswordRequestDto)
        {
            var user = await userManager.FindByEmailAsync(forgotPasswordRequestDto.Email);
            if (user == null)
            {
                // Return OK even if user doesn't exist to prevent email enumeration
                return Ok("If your email is registered, you will receive a password reset code.");
            }

            // Generate password reset token
            var token = await userManager.GeneratePasswordResetTokenAsync(user);

            // Send email with reset token
            try
            {
                await emailService.SendPasswordResetEmailAsync(user.Email, token);
                return Ok("Password reset code has been sent to your email.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Error sending password reset email.");
            }
        }

        [HttpPost]
        [Route("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto resetPasswordRequestDto)
        {
            var user = await userManager.FindByEmailAsync(resetPasswordRequestDto.Email);
            if (user == null)
            {
                return BadRequest("Invalid request.");
            }

            var result = await userManager.ResetPasswordAsync(
                user,
                resetPasswordRequestDto.Token,
                resetPasswordRequestDto.NewPassword);

            if (result.Succeeded)
            {
                return Ok("Password has been reset successfully.");
            }

            var errors = result.Errors.Select(e => e.Description);
            return BadRequest(new { Errors = errors });
        }

        [HttpPost]
        [Route("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail([FromBody] EmailConfirmationRequestDto request)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return BadRequest("Invalid email.");
            }

            if (user.EmailConfirmed)
            {
                return BadRequest("Email already confirmed");
            }

            var result = await userManager.ConfirmEmailAsync(user, request.Token);
            if (result.Succeeded)
            {
                return Ok("Email has been confirmed successfully.");
            }

            return BadRequest("Email confirmation failed");
        }

    }
}
