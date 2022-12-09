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

        [Authorize]
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<NoteDto>>> GetNotesForUser(Guid userId)
        {
            if (!await _userRepository.UserExistsAsync(userId))
            {
                return NotFound();
            }

            var notesForUser = _userRepository.GetNotesAsync(userId).Result;
            return Ok(_mapper.Map<IEnumerable<NoteDto>>(notesForUser));
        }

        [HttpGet("{noteId}", Name = "GetNoteForUser")]
        public async Task<ActionResult<NoteDto>> GetNoteForUser(Guid userId, Guid noteId)
        {
            if (!await _userRepository.UserExistsAsync(userId))
            {
                return NotFound();
            }

            var noteForId = await _userRepository.GetNoteAsync(noteId);
            return Ok(_mapper.Map<NoteDto>(noteForId));
        }

        [HttpPost]
        public async Task<ActionResult<NoteDto>> CreateNoteForUserAsync(Guid userId, NoteForCreationDto note)
        {
            if (!await _userRepository.UserExistsAsync(userId))
            {
                return NotFound();
            }

            var noteEntity = _mapper.Map<Note>(note);
            _userRepository.AddNote(noteEntity, userId);
            await _userRepository.SaveAsync();

            var noteToReturn = _mapper.Map<NoteDto>(noteEntity);

            return CreatedAtRoute("GetNoteForUser",
                new { userId = userId, noteId = noteToReturn.NoteId },
                noteToReturn);
        }

        //TODO return GETNoteForUser
        [HttpDelete("{noteId}")]
        public async Task<ActionResult> DeleteNoteForUser(Guid noteId, Guid userId)
        {
            if (!await _userRepository.UserExistsAsync(userId))
            {
                return NotFound();
            }

            var noteForUser = await _userRepository.GetNoteAsync(noteId, userId);

            if (noteForUser == null)
            {
                return NotFound();
            }

            _userRepository.DeleteNote(noteForUser);
            await _userRepository.SaveAsync();

            return NoContent();
        }

        [HttpPatch("{noteId}/color")]
        public async Task<ActionResult> ChangeNoteColor(Guid noteId, Guid userId, JsonPatchDocument<NoteForCreationDto> patchDocument)
        {
            if (noteId == Guid.Empty)
            {
                return BadRequest("Wrong note id");
            }

            if (!await _userRepository.UserExistsAsync(userId))
            {
                return NotFound("nesto");
            }

            return UpdateNote(noteId, userId, patchDocument).Result;
        }

        [HttpPatch("{noteId}/title")]
        public async Task<ActionResult> ChangeNoteTitle(Guid noteId, Guid userId, JsonPatchDocument<NoteForCreationDto> patchDocument)
        {
            if (noteId == Guid.Empty)
            {
                return BadRequest("Wrong note id");
            }

            if(!await _userRepository.UserExistsAsync(userId))
            {
                return NotFound("No user");
            }

            return UpdateNote(noteId, userId, patchDocument).Result;
        }

        [HttpPatch("{noteId}/content")]
        public async Task<ActionResult> ChangeNoteContent(Guid noteId, Guid userId, JsonPatchDocument<NoteForCreationDto> patchDocument)
        {
            if (noteId == Guid.Empty)
            {
                return BadRequest("Note does not exist.");
            }

            if (!await _userRepository.UserExistsAsync(userId))
            {
                return NotFound("No user with this id.");
            }

            return await UpdateNote(noteId, userId, patchDocument);
        }

        private async Task<ActionResult> UpdateNote(Guid noteId, Guid userId, JsonPatchDocument<NoteForCreationDto> patchDocument)
        {
            var noteFromRepo = await _userRepository.GetNoteAsync(noteId, userId);
            //ako nema pravimo ga
            if (noteFromRepo == null)
            {
                var noteDto = new NoteForCreationDto();
                patchDocument.ApplyTo(noteDto);

                var noteToAdd = _mapper.Map<Note>(noteDto);
                noteToAdd.NoteId = noteId;

                _userRepository.AddNote(noteToAdd, userId);
                await _userRepository.SaveAsync();

                var noteToReturn = _mapper.Map<NoteDto>(noteToAdd);
                return CreatedAtRoute("GetNoteForUser",
                    new { userId, noteId = noteToReturn.NoteId },
                    noteToReturn);
            }

            var noteToPatch = _mapper.Map<NoteForCreationDto>(noteFromRepo);
            patchDocument.ApplyTo(noteToPatch);

            _mapper.Map(noteToPatch, noteFromRepo);
            _userRepository.UpdateNote(noteFromRepo);
            await _userRepository.SaveAsync();

            return NoContent();
        }
    }
}