using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using NotesApp.API.Entities;
using NotesApp.API.Helpers;
using NotesApp.API.Models;
using NotesApp.API.ResourceParameters;
using NotesApp.API.Services;
using System;


namespace NotesApp.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserController(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet("{userId}", Name = "GetUser")]
        public IActionResult GetUser(Guid userId)
        {
            if (userId == Guid.Empty) return NotFound();

            var user = _userRepository.GetUser(userId);

            if (user == null) return NotFound();

            return Ok(_mapper.Map<UserDto>(user));
        }

        [HttpGet()]
        public IActionResult GetUser([FromQuery] UserResourceParameters userResourceParameters)
        {
            if (userResourceParameters == null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(userResourceParameters.Email) || string.IsNullOrWhiteSpace(userResourceParameters.Password))
            {
                return NotFound("Prazno");
            }

            var userFromRepo = _userRepository.GetUser(userResourceParameters);


            return Ok(_mapper.Map<UserDto>(userFromRepo));
        }

        [HttpPost]
        public ActionResult<UserDto> CreateUser(UserForCreationDto user)
        {
            return CreatorOfUser(user);
        }

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

            var userForDelete = _userRepository.GetUser(userId);
            _userRepository.DeleteUser(userForDelete, true);
            _userRepository.Save();

            return NoContent();
        }

        [HttpPatch("{userId}/password")]
        public ActionResult ChangeUserPassword(Guid userId, JsonPatchDocument<UserForCreationDto> patchDocument)
        {
            if (!_userRepository.UserExists(userId))
            {
                return NotFound("User is not registered.");
            }

            var userFromRepo = _userRepository.GetUser(userId);
            
            var userToPatch = _mapper.Map<UserForCreationDto>(userFromRepo);
            patchDocument.ApplyTo(userToPatch);

            _mapper.Map(userToPatch, userFromRepo);
            _userRepository.UpdateUser(userFromRepo);
            _userRepository.Save();

            return NoContent();
        }

        private ActionResult<UserDto> CreatorOfUser(UserForCreationDto user)
        {
            var userEntity = _mapper.Map<User>(user);
            _userRepository.AddUser(userEntity);
            _userRepository.Save();

            var userToReturn = _mapper.Map<UserDto>(userEntity);
            return CreatedAtRoute("GetUser",
                new { userId = userToReturn.Id },
                userToReturn);
        }
    }
}
