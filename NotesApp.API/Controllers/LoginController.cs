using AutoMapper;
using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using NotesApp.API.Entities;
using NotesApp.API.Helpers;
using NotesApp.API.Models;
using NotesApp.API.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

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


        [HttpPost]
        public ActionResult LoginAsync([FromBody] UserResourceParametersDto userResourceParameters)
        {
            if (!MailValidator.IsValidEmail(userResourceParameters.Email))
            {
                return BadRequest("Wrong email format.");
            }

            try
            {
                var userFromRepo = _userRepository.GetUser(userResourceParameters);

                if (userFromRepo == null)
                {
                    return NotFound();
                }

                var token = _jwtTokenManager.Authenticate(userResourceParameters.Email/*, userResourceParameters.Password*/);
                if (string.IsNullOrEmpty(token))
                {
                    return Unauthorized("Token error.");
                }
;
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(userResourceParameters.Password, _configuration["salt"]);
                if (BCrypt.Net.BCrypt.Verify(userResourceParameters.Password, userFromRepo.Password))
                {
                    return Ok(new AuthResponseDto { IsAuthSuccessful = true, IsVerified = userFromRepo.Verified, Token = token, UserId = userFromRepo.UserId.ToString()});
                }

                return NotFound("User does not exist.");

            }
            catch (ArgumentNullException)
            {
                return BadRequest("Fill in all the fields or wrong parameters.");
            }
            catch(SaltParseException)
            {
                return BadRequest("Salt exception");
            }
            catch (Exception)
            {
                return BadRequest("Email or password are empty.");
            }
        }
        

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> CreateUserAsync([FromBody] UserForCreationDto user)
        {
            if (!MailValidator.IsValidEmail(user.Email))
            {
                return BadRequest("Wrong email format.");
            }

            var usersFromRepo = await _userRepository.GetUsersAsync();

            if (usersFromRepo.Any(x => x.Email.Equals(user.Email)))
            {
                return NotFound("Email already exist.");
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password, _configuration["salt"]);

            var userEntity = _mapper.Map<User>(user);
            await _userRepository.AddUser(userEntity);
            await _userRepository.SaveAsync();

            var userToReturn = _mapper.Map<UserDto>(userEntity);
            //return CreatedAtRoute("GetUser",
            //    new { userId = userToReturn.Id },
            //    userToReturn);
            return new AuthResponseDto { IsAuthSuccessful = false, Token = null, UserId = userToReturn.Id.ToString() };
        }

    }
}
