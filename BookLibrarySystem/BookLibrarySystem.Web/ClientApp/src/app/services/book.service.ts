import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, map, of, tap } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Book } from '../models/book';
import { BookBase } from '../models/book-base';
import { BookWithRelatedInfo } from '../models/book-related-info';

@Injectable({
  providedIn: 'root'
})
export class BookService {
   dialogData: any;
   private booksCache: BookBase[] | null = null;
   dataChange: BehaviorSubject<Book[]> = new BehaviorSubject<Book[]>([]);

  constructor(private http: HttpClient) {

   }

   getBooksAsync() :Observable<Book[]> {
    return this.http.get(environment.baseApiUrl + 'books/').pipe(
      map((response: any) => {
        console.log(response);
        return response;

      }));
   }

   getBooksWithRelatedInfoAsync(userId: string):Observable<BookWithRelatedInfo[]>{
    return this.http.get(environment.baseApiUrl + `books/getAllWithRelatedInfo/${userId}`).pipe(
      map((response:any) => {
        console.log(response);
        return response;
      })
    );
   }

   getBooksForListingAsync(forceRefresh: boolean = false) : Observable<BookBase[]> {
    if (this.booksCache && !forceRefresh ) { //&& (this.lastTimeChecked - Date.now > 15)
      // Return a cached version if available
      return of(this.booksCache);
    }

    return this.http.get(environment.baseApiUrl + 'books/getAllForListing').pipe(
      tap(books => this.booksCache = books as any),
      map((response:any) => {
        console.log(response);
        return response;
      })
    );
   }

   getBooksBySearchCriteria(pageIndex: number, pageSize: number, sortColumn: string, sortDirection:string, filters: any): Observable<any>{
      let params = new HttpParams()
      .set('pageIndex', pageIndex.toString())
      .set('pageSize', pageSize.toString())
      .set('sortColumn', sortColumn)
      .set('sortDirection', sortDirection);

      // Add filter parameters
      for (const key in filters) {
        if (filters.hasOwnProperty(key)) {
          params = params.set(`filters[${key}]`, filters[key]);
        }
      }

      return this.http.get(environment.serviceUrl + 'books/getBySearchCriteria', { params });
   }

   getDialogData(){
    return this.dialogData;
   }

   addBook(newBook: Book) {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json', // Set the Content-Type to JSON
    });

    return this.http.post(`${environment.serviceUrl}books/`, newBook, {headers});
   }

   updateBook(updatedBook: Book) {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json', // Set the Content-Type to JSON
    });

    return this.http.put(`${environment.serviceUrl}books/${updatedBook.id}`, updatedBook, {headers});
   }

   deleteBook(id:number) {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json', // Set the Content-Type to JSON
    });

    return this.http.delete<boolean>(`${environment.serviceUrl}books/${id}`, {headers});
   }

   borrowBook(bookId: number, userId: string) {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json'
    });

    return this.http.post<number>(`${environment.serviceUrl}books/borrow?bookId=${bookId}&&appUserId=${userId}`, {headers});
   }

   returnBook(bookId: number, userId: string) {
      const headers = new HttpHeaders({
        'Content-Type': 'application/json'
      });

      return this.http.put<number>(`${environment.serviceUrl}books/return?bookId=${bookId}&&appUserId=${userId}`, {headers});
   }

   reserveBook(bookId: number, userId: string){
    const headers = new HttpHeaders({
      'Content-Type': 'application/json'
    });

    return this.http.post<boolean>(`${environment.serviceUrl}books/reserve?bookId=${bookId}&&appUserId=${userId}`, {headers});
   }

   cancelReserveBook(bookId: number, userId: string){
    const headers = new HttpHeaders({
      'Content-Type': 'application/json'
    });

    return this.http.put<boolean>(`${environment.serviceUrl}books/cancelReservation?bookId=${bookId}&&appUserId=${userId}`, {headers});
   }

   assignBooks(authorId:number) {

   }
}
