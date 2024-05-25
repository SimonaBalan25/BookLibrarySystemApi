import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { ApiAuthorizationModule } from 'src/api-authorization/api-authorization.module';
import { AuthorizeGuard } from 'src/api-authorization/authorize.guard';
import { AuthorizeInterceptor } from 'src/api-authorization/authorize.interceptor';
import { BrowserAnimationsModule, NoopAnimationsModule } from '@angular/platform-browser/animations';
import { BooksListComponent } from './books/books-list/books-list.component';
import { UsersListComponent } from './users/users-list/users-list.component';
import { RoleExistsPipe } from './pipes/role-exists.pipe';
import { MaterialModule } from './material/material.module';
import { ScrollingModule } from '@angular/cdk/scrolling';
import { TableVirtualScrollModule } from 'ng-table-virtual-scroll';
import { FilterComponent } from './books/filter/filter.component';
import { EditBookDialogComponent } from './dialogs/edit-book/edit-book.dialog.component';
import { AddBookDialogComponent } from './dialogs/add-book/add-book.dialog.component';
import { DeleteBookDialogComponent } from './dialogs/delete-book/delete-book.dialog.component';
import { BorrowDialogComponent } from './dialogs/borrow/borrow.dialog.component';
import { ReturnBookDialogComponent } from './dialogs/return-book/return-book.dialog.component';
import { AppRoutingModule } from './app-routing.module';
import { HashLocationStrategy, LocationStrategy } from '@angular/common';
import { AuthorsListComponent } from './authors/authors-list/authors-list.component';
import { AddAuthorDialogComponent } from './dialogs/add-author/add-author.dialog.component';
import { EditAuthorDialogComponent } from './dialogs/edit-author/edit-author.dialog.component';
import { DeleteAuthorDialogComponent } from './dialogs/delete-author/delete-author.dialog.component';
import { ManageBooksDialogComponent } from './dialogs/manage-books/manage-books.dialog.component';
import { AddUserDialogComponent } from './dialogs/add-user/add-user.dialog.component';
import { EditUserDialogComponent } from './dialogs/edit-user/edit-user.dialog.component';
import { DeleteUserDialogComponent } from './dialogs/delete-user/delete-user.dialog.component';
import { ViewBookDialogComponent } from './dialogs/view-book/view-book.dialog.component';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    CounterComponent,
    FetchDataComponent,
    AuthorsListComponent,
    BooksListComponent,
    ManageBooksDialogComponent,
    AddBookDialogComponent,
    AddAuthorDialogComponent,
    AddUserDialogComponent,
    EditBookDialogComponent,
    EditAuthorDialogComponent,
    EditUserDialogComponent,
    DeleteBookDialogComponent,
    DeleteAuthorDialogComponent,
    DeleteUserDialogComponent,
    BorrowDialogComponent,
    ReturnBookDialogComponent,
    FilterComponent,
    UsersListComponent,
    RoleExistsPipe,
    ViewBookDialogComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    MaterialModule,
    ScrollingModule,
    TableVirtualScrollModule,
    BrowserAnimationsModule,
    ApiAuthorizationModule,
    HttpClientModule,
    AppRoutingModule,
    NoopAnimationsModule
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: AuthorizeInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
