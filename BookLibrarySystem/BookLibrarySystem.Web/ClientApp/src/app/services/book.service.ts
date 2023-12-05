import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { BehaviorSubject, Observable, map } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Book } from '../models/book';

@Injectable({
  providedIn: 'root'
})
export class BookService {
   dialogData: any;
   dataChange: BehaviorSubject<Book[]> = new BehaviorSubject<Book[]>([]);

  constructor(private http: HttpClient) {

   }

   getBooks() :Observable<Book[]> {
    return this.http.get(environment.baseApiUrl + 'books/').pipe(
      map((response: any) => {
        console.log(response);
        return response;

      }));
   }

   getDialogData(){
    return this.dialogData;
   }
}
