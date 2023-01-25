import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NotesListComponent } from './notes/notes-list/notes-list.component';
import { LoginComponent } from './user/login/login.component';
import { RegisterComponent } from './user/register/register.component';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { VerificationComponent } from './user/login/verification/verification.component';
import { NoteAddComponent } from './notes/note-add/note-add.component';
import { ChangePasswordComponent } from './user/change-password/change-password.component';
import { AuthGuard } from './shared/auth-guard.guard';
import { AuthService } from './user/auth.service';

@NgModule({
  declarations: [
    AppComponent,
    NotesListComponent,
    LoginComponent,
    RegisterComponent,
    VerificationComponent,
    NoteAddComponent,
    ChangePasswordComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    ReactiveFormsModule,
    HttpClientModule
  ],
  providers: [AuthGuard, AuthService],
  bootstrap: [AppComponent]
})
export class AppModule { }
