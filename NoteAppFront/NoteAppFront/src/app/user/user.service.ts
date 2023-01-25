import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { VerificationResponseDto } from '../models/verificationResponseDto';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  readonly baseURL = 'http://localhost:5000/api/users';

  constructor(private http: HttpClient) { }

  public createVerificationCode(id: string) {
    const url = `${this.baseURL}/${id}`;
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });

    return this.http.post<VerificationResponseDto>(url + '/verification', id, { headers });
  }

  public verify(id: string, code: string, body:any) {
    const url = `${this.baseURL}/${id}/verify/${code}`;
    const headers = new HttpHeaders({ 'Content-Type': 'application/json', 'Accept': 'application/json', 'Authorization': 'Bearer '+ localStorage.getItem("token") });

    return this.http.patch(url, body, { headers });
  }

  public changePassword(id: string, body: any) {
    const url = `${this.baseURL}/${id}/password`;
    const headers = new HttpHeaders({ 'Content-Type': 'application/json', 'Accept': 'application/json', 'Authorization': 'Bearer '+ localStorage.getItem("token") });

    return this.http.patch(url, body, { headers });
  }

}

