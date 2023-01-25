import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Note } from '../models/note';
import { Observable, Subject, of } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators'
import { NoteForCreationDto } from '../models/noteForCreationDto';

@Injectable({
  providedIn: 'root'
})
export class NoteService {
  readonly baseURL = 'http://localhost:5000/api/users';

  constructor(private http: HttpClient) { }

  private _refresh$ = new Subject<void>();
  get refresh() {
    return this._refresh$;
  }


  public getNotes(userId: string): Observable<Note[]> {
    const url = `${this.baseURL}/${userId}/notes`;
    const headers = new HttpHeaders({ 'Content-Type': 'application/json', 'Accept': 'application/json', 'Authorization': 'Bearer '+ localStorage.getItem("token") });

    return this.http.get<Note[]>(url, { headers })
      .pipe(catchError(this.handleError<Note[]>('getNotes', [])))
  }

  public getNote(userId: string, noteId: string) {
    const url = `${this.baseURL}/${userId}/notes/${noteId}`;
    const headers = new HttpHeaders({ 'Content-Type': 'application/json', 'Accept': 'application/json', 'Authorization': 'Bearer '+ localStorage.getItem("token") });

    return this.http.get<Note>(url, { headers });
  }

  public updateNote(userId: string, noteId: string, note: NoteForCreationDto) {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json', 'Accept': 'application/json', 'Authorization': 'Bearer '+ localStorage.getItem("token") });
    const url = `${this.baseURL}/${userId}/notes/${noteId}`;

    return this.http.put<NoteForCreationDto>(url, note, { headers })
      .pipe(
        tap(() => console.log('Update note: ' + noteId)),
        //map(() => note),
        catchError(this.handleError<Note[]>('getNotes', []))
      );
  }

  public createNote(id: string, body: NoteForCreationDto) {
    const url = `${this.baseURL}/${id}/notes`;
    const headers = new HttpHeaders({ 'Content-Type': 'application/json', 'Accept': 'application/json', 'Authorization': 'Bearer '+ localStorage.getItem("token") });

    return this.http.post<Note>(url, body, { headers });
  }

  public deleteNote(id: string, noteId: string) {
    const url = `${this.baseURL}/${id}/notes/${noteId}`;
    const headers = new HttpHeaders({ 'Content-Type': 'application/json', 'Accept': 'application/json', 'Authorization': 'Bearer '+ localStorage.getItem("token") });
    return this.http.delete(url, { headers })
      .pipe(
        tap(() => {
          this._refresh$.next();
        })
      );
  }

  private handleError<T> (operation = 'operation', result?: T) {
    return (error: any): Observable<T> => {
      console.error(error);
      return of(result as T);
    }
  }

}
