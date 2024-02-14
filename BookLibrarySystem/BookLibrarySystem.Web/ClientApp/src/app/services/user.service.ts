import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from 'src/environments/environment';
import { User } from '../models/user';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  baseApiUrl : string = environment.baseApiUrl;

  constructor(private http: HttpClient)
  {
   }

  getUserRoles(token:string|null): Observable<string[]> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    });
    return this.http.get<string[]>(this.baseApiUrl + 'Users/user-roles', {headers});
  }

  getUsersAsync():Observable<User[]> {

    return this.http.get(`${environment.baseApiUrl}Users/getUsers`).pipe(
      map((response: any) => {
        console.log(response);
        return response;

      }));
  }
}
