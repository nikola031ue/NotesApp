using AutoMapper;
using EllipticCurve.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NotesApp.API.Entities;
using NotesApp.API.Models;
using NotesApp.API.Services;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

namespace NotesApp.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        //private readonly IMailService _mailService;
        private readonly IJwtTokenManager _jwtTokenManager;

        public UserController(IUserRepository userRepository, IMapper mapper/*, IMailService mailService*/,
            IJwtTokenManager jwtTokenManager, IConfiguration configuration)
        {
            _configuration = configuration;
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            //_mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _jwtTokenManager = jwtTokenManager ?? throw new ArgumentNullException(nameof(jwtTokenManager));
        }

        [AllowAnonymous]
        [HttpGet("{userId}", Name = "GetUser")]
        public async Task<ActionResult> GetUserAsync(Guid userId)
        {
            try
            {
                var user = await _userRepository.GetUserAsync(userId);

                if (user == null) return NotFound();

                return Ok(_mapper.Map<UserDto>(user));
            }
            catch (ArgumentNullException)
            {
                return BadRequest("The id does not exist or is in a bad format.");
            }
        }

        [AllowAnonymous]
        [HttpPost("{userId}/verification", Name = "UserVerification")]
        public async Task<ActionResult> UserVerification(string userid)
        {
            Guid userId = new Guid(userid);
            try
            {
                if (!_userRepository.UserExists(userId))
                {
                    return NotFound("User does not exist.");
                }

                Verification userVerification = new Verification();
                userVerification.VerificationCode = GenerateVerificationCode();

                _userRepository.AddVerification(userVerification, userId);

                await _userRepository.SaveAsync();

                return Ok(userVerification);
            }
            catch (ArgumentNullException)
            {
                return BadRequest("The id does not exist or is in a bad format or verification user does not exist.");

            }
        }

        [HttpDelete("{userId}")]
        public async Task<ActionResult> DeleteUserAsync(Guid userId)
        {
            try
            {
                if (!_userRepository.UserExists(userId))
                {
                    return NotFound("User does not exist.");
                }

                var userForDelete = await _userRepository.GetUserAsync(userId);
                _userRepository.DeleteUser(userForDelete, true);
                await _userRepository.SaveAsync();

                return Ok("Deleted successfully.");
            }
            catch (ArgumentNullException)
            {
                return BadRequest("The id does not exist or is in a bad format.");
            }
        }

        [HttpPatch("{userId}/verify/{code}")]
        public async Task<ActionResult> Verified(int code, string userid, JsonPatchDocument<UserForCreationDto> patchDocument)
        {
            
            try
            {
                Guid userId = new Guid(userid);
                //int code = Int32.Parse(codeString);
                if (!await _userRepository.UserExistsAsync(userId))
                {
                    return NotFound("User is not registered.");
                }

                var userFromRepo = await _userRepository.GetUserAsync(userId);

                var verificationFromRepo = await _userRepository.GetVerificationAsync(userId);

                if (userFromRepo.Verified)
                {
                    return NoContent();
                }

                if (code == verificationFromRepo.VerificationCode)
                {
                    userFromRepo.Verified = true;
                    var userToPatch = _mapper.Map<UserForCreationDto>(userFromRepo);
                    patchDocument.ApplyTo(userToPatch);

                    _mapper.Map(userToPatch, userFromRepo);
                    _userRepository.UpdateUser(userFromRepo);
                    await _userRepository.SaveAsync();

                    return Ok("User verified successfully.");
                }
                else
                {
                    return BadRequest("Wrong verification code.");
                }
            }
            catch (ArgumentNullException)
            {
                return BadRequest("The id does not exist or is in a bad format.");
            }

        }

        [HttpPatch("{userId}/password")]
        public async Task<ActionResult> ChangeUserPassword(Guid userId, JsonPatchDocument<UserForCreationDto> patchDocument)
        {
            try
            {
                if (!await _userRepository.UserExistsAsync(userId))
                {
                    return NotFound("User is not registered.");
                }

                var userFromRepo = await _userRepository.GetUserAsync(userId);
                //user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password, _configuration["salt"]);
                
                var userToPatch = _mapper.Map<UserForCreationDto>(userFromRepo);
                patchDocument.ApplyTo(userToPatch);
                userToPatch.Password = BCrypt.Net.BCrypt.HashPassword(userToPatch.Password, _configuration["salt"]);
                _mapper.Map(userToPatch, userFromRepo);
                _userRepository.UpdateUser(userFromRepo);
                await _userRepository.SaveAsync();

                return Ok("Successfully changed the password.");
            }
            catch (ArgumentNullException)
            {
                return BadRequest("The id does not exist or is in a bad format.");
            }
        }

        private int GenerateVerificationCode()
        {
            Random generator = new Random();
            int number = generator.Next(1000000);
            return number;
        }

        public async Task SendEmailAsync(string toMail, string subject, string content)
        {
            var apiKey = _configuration["SendGridAPIKey"];

            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("nikola.pfc.kostic22@gmail.com", "JWT Auth");
            var to = new EmailAddress(toMail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, content, content);
            var response = await client.SendEmailAsync(msg);
        }
    }
}
