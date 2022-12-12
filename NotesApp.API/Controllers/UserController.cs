using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using NotesApp.API.Entities;
using NotesApp.API.Helpers;
using NotesApp.API.Models;
using NotesApp.API.ResourceParameters;
using NotesApp.API.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NotesApp.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IMailService _mailService;
        private readonly IJwtTokenManager _jwtTokenManager;

        public UserController(IUserRepository userRepository, IMapper mapper, IMailService mailService, IJwtTokenManager jwtTokenManager)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _jwtTokenManager = jwtTokenManager ?? throw new ArgumentNullException(nameof(jwtTokenManager));
        }

        [HttpGet("{userId}", Name = "GetUser")]
        public async Task<ActionResult> GetUserAsync(Guid userId)
        {
            if (userId == Guid.Empty) return NotFound();

            var user = await _userRepository.GetUserAsync(userId);

            if (user == null) return NotFound();

            return Ok(_mapper.Map<UserDto>(user));
        }

        [HttpGet()]
        public async Task<ActionResult> GetUserAsync([FromQuery] UserResourceParameters userResourceParameters)
        {
            if (userResourceParameters == null)
            {
                return BadRequest("Fill in all the fields.");
            }

            if (string.IsNullOrWhiteSpace(userResourceParameters.Email) || string.IsNullOrWhiteSpace(userResourceParameters.Password))
            {
                return BadRequest("Email or password are empty.");
            }

            if (!MailValidator.IsValidEmail(userResourceParameters.Email))
            {
                return BadRequest("Wrong email format.");
            }

            var userFromRepo = _userRepository.GetUser(userResourceParameters);
           
            var token = _jwtTokenManager.Authenticate(userResourceParameters.Email, userResourceParameters.Password);
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("Token error.");
            }

            if (userFromRepo != null)
            {
                await _mailService.SendEmailAsync(userFromRepo.Email,
                    "New Login", "<h1>New login</h1>");
                //return Ok(_mapper.Map<UserDto>(userFromRepo));
                return Ok(token);
            }
            else
            {
                return NoContent();
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUserAsync(UserForCreationDto user)
        {
            return await CreatorOfUserAsync(user);
        }

        [AllowAnonymous]
        [HttpPost("{generate}")]
        public async Task<ActionResult<UserDto>> CreateUserWithGeneratePasswordAsync(UserForCreationDto user)
        {
            string password = PasswordGenerator.GetRandomPassword();

            user.Password = password;
            return await CreatorOfUserAsync(user);
        }

        [Authorize]
        [HttpPost("{userId}/verification", Name ="UserVerification")]
        public async Task<ActionResult> UserVerification(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest("No user id.");
            }

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

        [Authorize]
        [HttpDelete("{userId}")]
        public async Task<ActionResult> DeleteUserAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest("No user id.");
            }

            if (!_userRepository.UserExists(userId))
            {
                return NotFound("User does not exist.");
            }

            var userForDelete = await _userRepository.GetUserAsync(userId);
            _userRepository.DeleteUser(userForDelete, true);
            await _userRepository.SaveAsync();

            return Ok("Deleted successfully.");
        }

        [Authorize]
        [HttpPatch("{userId}/verify/{code}")]
        public async Task<ActionResult> Verified(int code, Guid userId, JsonPatchDocument<UserForCreationDto> patchDocument)
        {
            if (!await _userRepository.UserExistsAsync(userId))
            {
                return NotFound("User is not registered.");
            }

            var userFromRepo = await _userRepository.GetUserAsync(userId);
            var verificationFromRepo = await _userRepository.GetVerificationAsync(userId);

            
            if(code == verificationFromRepo.VerificationCode)
            {
                userFromRepo.Verified = true;
                var userToPatch = _mapper.Map<UserForCreationDto>(userFromRepo);
                patchDocument.ApplyTo(userToPatch);

                _mapper.Map(userToPatch, userFromRepo);
                _userRepository.UpdateUser(userFromRepo);
                await _userRepository.SaveAsync();

                return Ok("User verifier successfully.");
            }

            return BadRequest("User can not be verified.");
        }

        [Authorize]
        [HttpPatch("{userId}/password")]
        public async Task<ActionResult> ChangeUserPassword(Guid userId, JsonPatchDocument<UserForCreationDto> patchDocument)
        {
            if (!await _userRepository.UserExistsAsync(userId))
            {
                return NotFound("User is not registered.");
            }

            var userFromRepo = await _userRepository.GetUserAsync(userId);

            var userToPatch = _mapper.Map<UserForCreationDto>(userFromRepo);
            patchDocument.ApplyTo(userToPatch);

            _mapper.Map(userToPatch, userFromRepo);
            _userRepository.UpdateUser(userFromRepo);
            await _userRepository.SaveAsync();

            return Ok("Successfully changed the password.");
        }

        private int GenerateVerificationCode()
        {
            Random generator = new Random();
            int number = generator.Next(1000000);
            return number;
        }

        private async Task<ActionResult<UserDto>> CreatorOfUserAsync(UserForCreationDto user)
        {
            if (!MailValidator.IsValidEmail(user.Email))
            {
                return BadRequest("Wrong email format.");
            }

            var usersFromRepo = await _userRepository.GetUsersAsync();

            if(usersFromRepo.Any(x => x.Email.Equals(user.Email)))
            {
                return BadRequest("Email already exist.");
            }

            user.Password = user.Password;

            var userEntity = _mapper.Map<User>(user);
            await _userRepository.AddUser(userEntity);
            await _userRepository.SaveAsync();

            var userToReturn = _mapper.Map<UserDto>(userEntity);
            return CreatedAtRoute("GetUser",
                new { userId = userToReturn.Id },
                userToReturn);

        }
    }
}
