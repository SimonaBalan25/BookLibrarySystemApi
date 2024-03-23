import { HttpErrorResponse } from '@angular/common/http';
import { Component, Inject, OnInit } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Author } from 'src/app/models/author';
import { BookBase } from 'src/app/models/book-base';
import { AuthorsService } from 'src/app/services/authors.service';
import { BookService } from 'src/app/services/book.service';

@Component({
  selector: 'app-add-author.dialog',
  templateUrl: './add-author.dialog.component.html',
  styleUrls: ['./add-author.dialog.component.css']
})
export class AddAuthorDialogComponent implements OnInit {
  newAuthor: Author;
  response: boolean;
  allBooks: BookBase[];
  selectedBook: string;
  selected: string;

  constructor(public dialogRef: MatDialogRef<AddAuthorDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { version: Uint8Array },
    public authorsService: AuthorsService, private bookService: BookService, private snackBar: MatSnackBar) {

      // Initialize newBook object with default properties
      this.newAuthor = {
        id: 0,
        name: '',
        country: '',
        books: []
      };
    }

    ngOnInit(): void {
      //this.selectedBook = '';
      this.bookService.getBooksForListingAsync().subscribe(data => {
        this.allBooks = data;
    });
    }

    formControl = new FormControl('', [
      Validators.required
      // Validators.email,
      ]);

  getErrorMessage() {
    return this.formControl.hasError('required') ? 'Required field' :
    this.formControl.hasError('email') ? 'Not a valid email' :
    '';
  }

  submit() {
    // empty stuff
  }

  onNoClick(): void {
    this.dialogRef.close();
  }

  public confirmAdd(): void {
    this.authorsService.addAuthor(this.newAuthor).subscribe(returnedData => {
      this.response = returnedData as boolean;
      this.snackBar.open('Successfully added', "Okay!", {duration: 3000});
      this.dialogRef.close(1);
    },
    (err: HttpErrorResponse) => {
      this.snackBar.open('Error occurred. Details: ' + err.name + ' ' + err.message, "Okay!", {duration: 8000});
      this.dialogRef.close(0);
    }
    );
  }
}
