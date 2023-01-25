import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { User } from '../models/user';
import { AuthResponseDto } from '../models/authResponseDto';
import { Subject, of } from 'rxjs';
import { UserForAuthDto } from '../models/userForAuthDto';
import { RegistrationResponseDto } from '../models/registrationResponseDto';
import { UserForRegistrationDto } from '../models/userForRegistrationDto';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  readonly baseURL = 'http://localhost:5000/api';
  
  private authChandeSub = new Subject<boolean>();
  public authChanged = this.authChandeSub.asObservable();

  private verificationSub = new Subject<boolean>();
  public verifChanged = this.verificationSub.asObservable();

  currentAuthResponse!: AuthResponseDto;

  constructor(private http:HttpClient) { }

  public loginUser(body: UserForAuthDto) {
    return this.http.post<AuthResponseDto>(this.baseURL+'/login', body);
  }

  public registerUser(body: UserForRegistrationDto) {
    return this.http.post<RegistrationResponseDto>(this.baseURL+'/login/register', body);
  }

  public sendAuthStateChangeNotification(isAuthenticated: boolean) {
    this.authChandeSub.next(isAuthenticated);
  }

  public sendVerifState(isVerified: boolean) {
    this.verificationSub.next(isVerified);
  }

  public logout() {
    localStorage.clear();
    this.sendAuthStateChangeNotification(false);
  }

  isLoggedIn(): boolean {
    let token = localStorage.getItem('token');
    if(token){
      return true;
    }
    return false;
  }

  isVerified() {
    return Boolean(localStorage.getItem("isVerified"));
  }
}
