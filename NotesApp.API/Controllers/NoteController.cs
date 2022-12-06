using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using NotesApp.API.Entities;
using NotesApp.API.Models;
using NotesApp.API.Services;
using System;
using System.Collections.Generic;

namespace NotesApp.API.Controllers
{
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
        public ActionResult<IEnumerable<NoteDto>> GetNotesForUser(Guid userId)
        {
            if (!_userRepository.UserExists(userId))
            {
                return NotFound();
            }

            var notesForUser = _userRepository.GetNotes(userId);
            return Ok(_mapper.Map<IEnumerable<NoteDto>>(notesForUser));
        }

        [HttpGet("{noteId}", Name = "GetNoteForUser")]
        public ActionResult<NoteDto> GetNoteForUser(Guid userId, Guid noteId)
        {
            if (!_userRepository.UserExists(userId))
            {
                return NotFound();
            }

            var noteForId = _userRepository.GetNote(noteId);
            return Ok(_mapper.Map<NoteDto>(noteForId));
        }

        [HttpPost]
        public ActionResult<NoteDto> CreateNoteForUser(Guid userId, NoteForCreationDto note)
        {
            if (!_userRepository.UserExists(userId))
            {
                return NotFound();
            }

            var noteEntity = _mapper.Map<Note>(note);
            _userRepository.AddNote(noteEntity, userId);
            _userRepository.Save();

            var noteToReturn = _mapper.Map<NoteDto>(noteEntity);

            return CreatedAtRoute("GetNoteForUser",
                new { userId = userId, noteId = noteToReturn.NoteId },
                noteToReturn);
        }

        //TODO return GETNoteForUser
        [HttpDelete("{noteId}")]
        public ActionResult DeleteNoteForUser(Guid noteId, Guid userId)
        {
            if (!_userRepository.UserExists(userId))
            {
                return NotFound();
            }

            var noteForUser = _userRepository.GetNote(noteId, userId);

            if (noteForUser == null)
            {
                return NotFound();
            }

            _userRepository.DeleteNote(noteForUser);
            _userRepository.Save();

            return NoContent();
        }

        [HttpPatch("{noteId}/color")]
        public ActionResult ChangeNoteColor(Guid noteId, Guid userId, JsonPatchDocument<NoteForCreationDto> patchDocument)
        {
            if (noteId == Guid.Empty)
            {
                return BadRequest("Wrong note id");
            }

            if (!_userRepository.UserExists(userId))
            {
                return NotFound("nesto");
            }

            return UpdateNote(noteId, userId, patchDocument);
        }

        [HttpPatch("{noteId}/title")]
        public ActionResult ChangeNoteTitle(Guid noteId, Guid userId, JsonPatchDocument<NoteForCreationDto> patchDocument)
        {
            if (noteId == Guid.Empty)
            {
                return BadRequest("Wrong note id");
            }

            if(!_userRepository.UserExists(userId))
            {
                return NotFound("No user");
            }

            return UpdateNote(noteId, userId, patchDocument);
        }

        [HttpPatch("{noteId}/content")]
        public ActionResult ChangeNoteContent(Guid noteId, Guid userId, JsonPatchDocument<NoteForCreationDto> patchDocument)
        {
            if (noteId == Guid.Empty)
            {
                return BadRequest("Note does not exist.");
            }

            if (!_userRepository.UserExists(userId))
            {
                return NotFound("No user with this id.");
            }

            return UpdateNote(noteId, userId, patchDocument);
        }

        private ActionResult UpdateNote(Guid noteId, Guid userId, JsonPatchDocument<NoteForCreationDto> patchDocument)
        {
            var noteFromRepo = _userRepository.GetNote(noteId, userId);
            //ako nema pravimo ga
            if (noteFromRepo == null)
            {
                var noteDto = new NoteForCreationDto();
                patchDocument.ApplyTo(noteDto);

                var noteToAdd = _mapper.Map<Note>(noteDto);
                noteToAdd.NoteId = noteId;

                _userRepository.AddNote(noteToAdd, userId);
                _userRepository.Save();

                var noteToReturn = _mapper.Map<NoteDto>(noteToAdd);
                return CreatedAtRoute("GetNoteForUser",
                    new { userId, noteId = noteToReturn.NoteId },
                    noteToReturn);
            }

            var noteToPatch = _mapper.Map<NoteForCreationDto>(noteFromRepo);
            patchDocument.ApplyTo(noteToPatch);

            _mapper.Map(noteToPatch, noteFromRepo);
            _userRepository.UpdateNote(noteFromRepo);
            _userRepository.Save();

            return NoContent();
        }
    }
}