import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {map} from 'rxjs/operators';

import { JwtHelperService } from '@auth0/angular-jwt';
import {environment} from '../..//environments/environment';
import { User } from '../_models/user';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  baseURL = environment.apiUrl + 'auth/';
  jwtHelper = new JwtHelperService();
  decodedToken: any;
  currentUser: User;
  photoUrl = new BehaviorSubject<string>('../../assets/user.jpg');
  currentPhotoUrl = this.photoUrl.asObservable();
  
constructor(private http: HttpClient) { }

changePhotoUrl(photoUrl){
  this.photoUrl.next(photoUrl);
}

login(model: any) {

  return this.http.post(this.baseURL + 'login', model)
    .pipe(
      map((response: any) => {
          const user = response;
          if (user) {
            localStorage.setItem('token', user.token);
            localStorage.setItem('user', JSON.stringify(user.user));
            this.decodedToken = this.jwtHelper.decodeToken(user.token);
            this.currentUser = user.user;
            this.changePhotoUrl(this.currentUser.photoUrl);
          }
          })
    );

}

register(user: User) {

  return this.http.post(this.baseURL + 'register', user);
}

loggetOn() {
  const myRawToken =  localStorage.getItem('token');
  return !this.jwtHelper.isTokenExpired(myRawToken);
}
}
