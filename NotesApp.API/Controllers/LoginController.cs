using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NotesApp.API.Entities;
using NotesApp.API.Helpers;
using NotesApp.API.Models;
using NotesApp.API.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NotesApp.API.Controllers
{
    [ApiController]
    [Route("api/login")]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IJwtTokenManager _jwtTokenManager;

        public LoginController(IUserRepository userRepository, IMapper mapper/*, IMailService mailService*/,
           IJwtTokenManager jwtTokenManager, IConfiguration configuration)
        {
            _configuration = configuration;
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            //_mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _jwtTokenManager = jwtTokenManager ?? throw new ArgumentNullException(nameof(jwtTokenManager));
        }


        [HttpGet()]
        public ActionResult LoginAsync([FromQuery] UserResourceParametersDto userResourceParameters)
        {
            if (!MailValidator.IsValidEmail(userResourceParameters.Email))
            {
                return BadRequest("Wrong email format.");
            }

            try
            {
                var userFromRepo = _userRepository.GetUser(userResourceParameters);

                if(userFromRepo == null)
                {
                    return NoContent();
                }

                var token = _jwtTokenManager.Authenticate(userResourceParameters.Email/*, userResourceParameters.Password*/);
                if (string.IsNullOrEmpty(token))
                {
                    return Unauthorized("Token error.");
                }

                if (PasswordHasher.HashPassword(userResourceParameters.Password) == userFromRepo.Password)
                {
                    return Ok("Token:  " + token + "  UserId:  " + userFromRepo.UserId);
                }

                return Ok("User does not exist.");
                
            }
            catch (ArgumentNullException)
            {
                return BadRequest("Fill in all the fields or wrong parameters.");
            }
            catch (Exception)
            {
                return BadRequest("Email or password are empty.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUserAsync(UserForCreationDto user)
        {
            return await CreatorOfUserAsync(user);
        }

        [HttpPost("{generate}")]
        public async Task<ActionResult<UserDto>> CreateUserWithGeneratePasswordAsync(UserForCreationDto user)
        {
            user.Password = PasswordGenerator.GetRandomPassword();

            return await CreatorOfUserAsync(user);
        }

        private async Task<ActionResult<UserDto>> CreatorOfUserAsync(UserForCreationDto user)
        {
            if (!MailValidator.IsValidEmail(user.Email))
            {
                return BadRequest("Wrong email format.");
            }

            var usersFromRepo = await _userRepository.GetUsersAsync();

            if (usersFromRepo.Any(x => x.Email.Equals(user.Email)))
            {
                return BadRequest("Email already exist.");
            }

            user.Password = PasswordHasher.HashPassword(user.Password);

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
