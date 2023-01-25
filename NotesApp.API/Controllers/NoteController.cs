using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using NotesApp.API.Entities;
using NotesApp.API.Models;
using NotesApp.API.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace NotesApp.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/users/{userId}/notes")]
    public class NoteController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public NoteController(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet()]
        public async Task<ActionResult<IEnumerable<NoteDto>>> GetNotesForUser(Guid userId)
        {
            try
            {
                if (!await _userRepository.UserExistsAsync(userId))
                {
                    return NotFound("User does not registered.");
                }

                var notesForUser = _userRepository.GetNotesAsync(userId).Result;
                return Ok(_mapper.Map<IEnumerable<NoteDto>>(notesForUser));
            }
            catch (ArgumentNullException)
            {
                return BadRequest("The id does not exist or is in a bad format.");
            }
        }

        [HttpGet("{noteId}", Name = "GetNoteForUser")]
        public async Task<ActionResult<NoteDto>> GetNoteForUser(Guid userId, Guid noteId)
        {
            try
            {
                if (!await _userRepository.UserExistsAsync(userId))
                {
                    return NotFound("User does not registered.");
                }

                var noteForId = await _userRepository.GetNoteAsync(noteId);
                return Ok(_mapper.Map<NoteDto>(noteForId));
            }
            catch (ArgumentNullException)
            {
                return BadRequest("The id does not exist or is in a bad format.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<NoteDto>> CreateNoteForUserAsync(Guid userId, NoteForCreationDto note)
        {
            try
            {
                if (!await _userRepository.UserExistsAsync(userId))
                {
                    return NotFound("User does not registered.");
                }

                var noteEntity = _mapper.Map<Note>(note);
                _userRepository.AddNote(noteEntity, userId);
                await _userRepository.SaveAsync();

                var noteToReturn = _mapper.Map<NoteDto>(noteEntity);

                return CreatedAtRoute("GetNoteForUser",
                    new { userId = userId, noteId = noteToReturn.NoteId },
                    noteToReturn);
            }
            catch (ArgumentNullException)
            {
                return BadRequest("The id does not exist or is in a bad format.");
            }
        }

        [HttpDelete("{noteId}")]
        public async Task<ActionResult> DeleteNoteForUser(Guid noteId, Guid userId)
        {
            try
            {
                if (!await _userRepository.UserExistsAsync(userId))
                {
                    return NotFound("User does not registered.");
                }

                var noteForUser = await _userRepository.GetNoteAsync(noteId, userId);

                if (noteForUser == null)
                {
                    return Ok("User do not have any notes.");
                }

                _userRepository.DeleteNote(noteForUser);
                await _userRepository.SaveAsync();

                return Ok("Deleted successfully.");
            }
            catch (ArgumentNullException)
            {
                return BadRequest("The id does not exist or is in a bad format.");
            }
        }

        [HttpPut("{noteId}")]
        public async Task<ActionResult> ChangeNote(string noteid, string userid, NoteForCreationDto note)
        {
            Guid userId = new Guid(userid);
            Guid noteId = new Guid(noteid);
            try
            {
                if(noteId == Guid.Empty)
                {
                    return BadRequest("The note id does not exist or is in a bad format.");
                }
                
                if (!await _userRepository.UserExistsAsync(userId))
                {
                    return NotFound("User does not registered.");
                }

                var noteFromRepo = await _userRepository.GetNoteAsync(noteId, userId);

                if (noteFromRepo == null)
                {
                    return BadRequest("Note dose not exist.");
                }

                _mapper.Map(note, noteFromRepo);
                _userRepository.UpdateNote(noteFromRepo);
                await _userRepository.SaveAsync();

                return Ok("Note successfully edited.");
            } 
            catch(ArgumentNullException)
            {
                return BadRequest("The user id does not exist or is in a bad format.");
            }
        }

        [HttpPatch("{noteId}/color")]
        public async Task<ActionResult> ChangeNoteColor(Guid noteId, Guid userId, JsonPatchDocument<NoteForCreationDto> patchDocument)
        {
            try
            {
                if (noteId == Guid.Empty)
                {
                    return BadRequest("The note id does not exist or is in a bad format.");
                }

                if (!await _userRepository.UserExistsAsync(userId))
                {
                    return NotFound("User does not registered.");
                }

                return UpdateNote(noteId, userId, patchDocument).Result;
            }
            catch (ArgumentNullException)
            {
                return BadRequest("The user id does not exist or is in a bad format.");
            }
        }

        [HttpPatch("{noteId}/title")]
        public async Task<ActionResult> ChangeNoteTitle(Guid noteId, Guid userId, JsonPatchDocument<NoteForCreationDto> patchDocument)
        {
            try
            {
                if (noteId == Guid.Empty)
                {
                    return BadRequest("The note id does not exist or is in a bad format.");
                }

                if (!await _userRepository.UserExistsAsync(userId))
                {
                    return NotFound("User does not registered.");
                }

                return UpdateNote(noteId, userId, patchDocument).Result;
            }
            catch (ArgumentNullException)
            {
                return BadRequest("The user id does not exist or is in a bad format.");
            }
        }

        [HttpPatch("{noteId}/content")]
        public async Task<ActionResult> ChangeNoteContent(Guid noteId, Guid userId, JsonPatchDocument<NoteForCreationDto> patchDocument)
        {
            try
            {
                if (noteId == Guid.Empty)
                {
                    return BadRequest("The note id does not exist or is in a bad format.");
                }

                if (!await _userRepository.UserExistsAsync(userId))
                {
                    return NotFound("User does not registered.");
                }

                return UpdateNote(noteId, userId, patchDocument).Result;
            }
            catch (ArgumentNullException)
            {
                return BadRequest("The user id does not exist or is in a bad format.");
            }
        }

        private async Task<ActionResult> UpdateNote(Guid noteId, Guid userId, JsonPatchDocument<NoteForCreationDto> patchDocument)
        {
            var noteFromRepo = await _userRepository.GetNoteAsync(noteId, userId);

            if (noteFromRepo == null)
            {
                return BadRequest("Note dose not exist.");
            }

            var noteToPatch = _mapper.Map<NoteForCreationDto>(noteFromRepo);
            patchDocument.ApplyTo(noteToPatch);

            _mapper.Map(noteToPatch, noteFromRepo);
            _userRepository.UpdateNote(noteFromRepo);
            await _userRepository.SaveAsync();

            return Ok("Note successfully edited.");
        }
    }
}