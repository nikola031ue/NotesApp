using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using NotesApp.API.Entities;
using NotesApp.API.Helpers;
using NotesApp.API.Models;
using NotesApp.API.ResourceParameters;
using NotesApp.API.Services;
using System;
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
        public ActionResult GetUser(Guid userId)
        {
            if (userId == Guid.Empty) return NotFound();

            var user = _userRepository.GetUserAsync(userId).Result;

            if (user == null) return NotFound();

            return Ok(_mapper.Map<UserDto>(user));
        }

        [HttpGet()]
        public async Task<ActionResult> GetUserAsync([FromQuery] UserResourceParameters userResourceParameters)
        {
            if (userResourceParameters == null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(userResourceParameters.Email) || string.IsNullOrWhiteSpace(userResourceParameters.Password))
            {
                return NotFound("Prazno");
            }

            if (!MailValidator.IsValidEmail(userResourceParameters.Email))
            {
                return BadRequest("Wrong email format.");
            }

            var userFromRepo = _userRepository.GetUser(userResourceParameters);
            var token = _jwtTokenManager.Authenticate(userResourceParameters.Email, userResourceParameters.Password);
            if(string.IsNullOrEmpty(token))
            {
                return Unauthorized();
            }

            if(userFromRepo != null)
            {
                await _mailService.SendEmailAsync(userFromRepo.Email,
                    "New Login", "<h1>New login</h1>");
                //return Ok(_mapper.Map<UserDto>(userFromRepo));
                return Ok(token);
            } 
            else
            {
                return Ok("Ne postoji korisnik");
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult<UserDto> CreateUser(UserForCreationDto user)
        {
            return CreatorOfUser(user);
        }

        [AllowAnonymous]
        [HttpPost("{generate}")]
        public ActionResult<UserDto> CreateUserWithGeneratePassword(UserForCreationDto user)
        {
            string password = PasswordGenerator.GetRandomPassword();

            user.Password = password;
            return CreatorOfUser(user);
        }

        // TODO Token auth
        // https://bitoftech.net/2014/06/01/token-based-authentication-asp-net-web-api-2-owin-asp-net-identity/
        [HttpDelete("{userId}")]
        public ActionResult DeleteUser(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest("No user id.");
            }

            if (!_userRepository.UserExists(userId))
            {
                return NotFound();
            }

            var userForDelete = _userRepository.GetUserAsync(userId).Result;
            _userRepository.DeleteUser(userForDelete, true);
            _userRepository.SaveAsync();

            return NoContent();
        }

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

            return NoContent();
        }

        private ActionResult<UserDto> CreatorOfUser(UserForCreationDto user)
        {
            if (!MailValidator.IsValidEmail(user.Email))
            {
                return BadRequest("Wrong email format.");
            }


            user.Password = user.Password;

            var userEntity = _mapper.Map<User>(user);
            _userRepository.AddUser(userEntity);
            _userRepository.SaveAsync();

            var userToReturn = _mapper.Map<UserDto>(userEntity);
            return CreatedAtRoute("GetUser",
                new { userId = userToReturn.Id },
                userToReturn);
        }
    }
}
