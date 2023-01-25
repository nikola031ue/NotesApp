import { Component } from '@angular/core';
import { Note } from 'src/app/models/note';
import { NoteService } from '../../shared/note.service';
import { Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';

@Component({
  selector: 'app-noteslist',
  templateUrl: './notes-list.component.html',
  styleUrls: ['./notes-list.component.css']
})
export class NotesListComponent {
  title = "Note List";
  errorMessage = '';
  notes$: Observable<Note[]>;
  userId = localStorage.getItem("id");

  constructor(private noteService: NoteService,
              private router:Router) {}

  ngOnInit() : void {
    this.noteService.refresh
      .subscribe(() => {
       this.notes$ = this.noteService.getNotes(this.userId) //za delete refresh
      });
    this.notes$ = this.noteService.getNotes(this.userId)
  }

  delete(noteId: string) {
    this.noteService.deleteNote(localStorage.getItem("id"), noteId)
      .subscribe( {
        next: (res) => {
          console.log(res);
         // this.notes$ = this.noteService.getNotes(this.userId);
        },
        error: (err: HttpErrorResponse) => console.log(err.error)
      });
  }
}
