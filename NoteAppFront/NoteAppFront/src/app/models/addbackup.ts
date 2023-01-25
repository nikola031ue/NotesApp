// import { HttpErrorResponse } from '@angular/common/http';
// import { Component } from '@angular/core';
// import { FormBuilder, FormGroup, Validators } from '@angular/forms';
// import { Router } from '@angular/router';
// import { Note } from 'src/app/models/note';
// import { NoteForCreationDto } from 'src/app/models/noteForCreationDto';
// import { NoteService } from 'src/app/shared/note.service';

// @Component({
//   selector: 'app-note-add',
//   templateUrl: './note-add.component.html',
//   styleUrls: ['./note-add.component.css']
// })
// export class NoteAddComponent {
//   mouseoverRegister: any;
//   showError: boolean;
//   errorMessage: string;
//   noteForm: FormGroup;
//   colors: string[] = ['#FF0000', '#66FFFF', '#FFFF66'];

//   constructor(private router:Router,
//               private fb: FormBuilder,
//               private noteService: NoteService) {}

//   ngOnInit(): void {
//     this.noteForm = this.fb.group({
//       title: ['', [Validators.required, Validators.maxLength(150)]],
//       content: ['', [Validators.maxLength(2000)]],
//       color: [this.colors[0]]
//     });
//   }

//   createNote(noteForm: any) {
//     const noteValues = { ...noteForm };
//     const note: NoteForCreationDto = {
//       title: noteValues.title,
//       content: noteValues.content,
//       color: noteValues.color
//     };

//     this.noteService.createNote(localStorage.getItem("id"), note)
//       .subscribe({
//         next: (res: Note) => {
//           console.log("Note create.");
//           this.router.navigate(['/notes']);
//         },
//         error: (err: HttpErrorResponse) => console.log(err.error)
//       })
//   }



//   cancel() {
//     this.router.navigate(['/notes'])
//   }

//   validateControl = (controlName: string) => {
//     return this.noteForm.get(controlName)?.invalid && this.noteForm.get(controlName)?.touched
//   }

//   hasError = (controlName: string, errorName: string) => {
//     return this.noteForm.get(controlName)?.hasError(errorName)
//   }

// }
