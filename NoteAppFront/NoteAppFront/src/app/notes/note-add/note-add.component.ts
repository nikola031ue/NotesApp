import { HttpErrorResponse } from '@angular/common/http';
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { Note } from 'src/app/models/note';
import { NoteForCreationDto } from 'src/app/models/noteForCreationDto';
import { NoteService } from 'src/app/shared/note.service';

@Component({
  selector: 'app-note-add',
  templateUrl: './note-add.component.html',
  styleUrls: ['./note-add.component.css']
})
export class NoteAddComponent {
  mouseoverRegister: any;
  titlePage: string;
  showError: boolean;
  buttonName: string;
  errorMessage: string;
  noteForm: FormGroup;
  colors: string[] = ['#FF0000', '#66FFFF', '#FFFF66'];
  note: Note;
  noteId: string;
  private sub: Subscription;

  constructor(private router:Router,
              private fb: FormBuilder,
              private route: ActivatedRoute,
              private noteService: NoteService) {}

  ngOnInit(): void {
    this.noteForm = this.fb.group({
      title: ['', [Validators.required, Validators.maxLength(150)]],
      content: ['', [Validators.maxLength(2000)]],
      color: [this.colors[0]]
    });

    this.sub = this.route.paramMap.subscribe(
      params => {
        this.noteId = params.get('noteId');
        if(this.noteId == "0") {
          this.buttonName = "Save";
          this.titlePage = "Add note";
        } else {
          this.buttonName = "Edit";
          this.titlePage = "Edit note";
          this.getNote(localStorage.getItem("id"), this.noteId);
        }
      }
    )
  }

  getNote(userId: string, noteId: string) {
    this.noteService.getNote(userId, noteId)
      .subscribe({
        next: (note: Note) => this.displayNote(note),
        error: err => this.errorMessage = err
      });
  }

  displayNote(note: Note): void {
    if(this.noteForm) {
      this.noteForm.reset();
    }
    this.note = note;

    this.noteForm.patchValue({
      title: this.note.title,
      content: this.note.content,
      color: this.note.color
    });

  }

  createNote(noteForm: any) {
    const noteValues = { ...noteForm };
    const note: NoteForCreationDto = {
      title: noteValues.title,
      content: noteValues.content,
      color: noteValues.color
    };
    if(this.noteId == '0') {
      this.noteService.createNote(localStorage.getItem("id"), note)
        .subscribe({
          next: (res: Note) => {
            console.log("Note create.");
            this.router.navigate(['/notes']);
          },
          error: (err: HttpErrorResponse) => console.log(err.error)
        })
    } else {
      this.noteService.updateNote(localStorage.getItem("id"), this.note.noteId, note)
        .subscribe({
          next: (res) => {
            console.log("Note update." + this.note.noteId);
            this.router.navigate(['/notes']);
          },
          error: (err: HttpErrorResponse) => console.log(err.error)
        })
    }
  }

  onSaveComplete(): void {
    // Reset the form to clear the flags
    this.noteForm.reset();
    this.router.navigate(['/notes']);
  }

  cancel() {
    this.router.navigate(['/notes'])
  }

  validateControl(controlName: string) {
    return this.noteForm.get(controlName)?.invalid && this.noteForm.get(controlName)?.touched
  }

  hasError(controlName: string, errorName: string) {
    return this.noteForm.get(controlName)?.hasError(errorName)
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }
}
