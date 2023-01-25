import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { NotesListComponent } from './notes/notes-list/notes-list.component';
import { LoginComponent } from './user/login/login.component';
import { RegisterComponent } from './user/register/register.component';
import { VerificationComponent } from './user/login/verification/verification.component';
import { NoteAddComponent } from './notes/note-add/note-add.component';
import { ChangePasswordComponent } from './user/change-password/change-password.component';
import { AuthGuard } from './shared/auth-guard.guard';


const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'notes', component: NotesListComponent, canActivate: [AuthGuard] },
  { path: 'verify', component: VerificationComponent, canActivate: [AuthGuard] },
  { path: 'add-note/:noteId', component: NoteAddComponent, canActivate: [AuthGuard] },
  { path: 'notes/:noteId', component: NoteAddComponent, canActivate: [AuthGuard] },
  { path: 'password', component: ChangePasswordComponent, canActivate: [AuthGuard] },
  { path: '**', component: LoginComponent}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
