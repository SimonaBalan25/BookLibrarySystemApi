import { Component, Inject, OnInit } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Author } from 'src/app/models/author';
import { Book } from 'src/app/models/book';
import { AuthorsService } from 'src/app/services/authors.service';
import { BookService } from 'src/app/services/book.service';

@Component({
  selector: 'app-edit-author.dialog',
  templateUrl: './edit-author.dialog.component.html',
  styleUrls: ['./edit-author.dialog.component.css']
})
export class EditAuthorDialogComponent implements OnInit {
  dialogData: any;
  response: boolean;
  allBooks: Book[];

  constructor(public dialogRef: MatDialogRef<EditAuthorDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any, public authorsService: AuthorsService,
    public booksService: BookService, private snackBar: MatSnackBar) {
        this.response= false;
     }

  formControl = new FormControl('', [
      Validators.required
      // Validators.email,
  ]);

  ngOnInit(){
    this.booksService.getBooksAsync().subscribe(data => {
      this.allBooks = data;
  });
  }


  submit() {
    // emppty stuff
  }

  getErrorMessage() {
    return this.formControl.hasError('required') ? 'Required field' :
        '';
  }

  onNoClick(): void {
    this.dialogRef.close();
  }

  stopEdit(): void {
    this.authorsService.updateAuthor(<Author>this.data).subscribe({
      next: data => {
        this.response = data as boolean;
        this.snackBar.open('Successfully edited', "Okay!", {duration: 3000});
        this.dialogRef.close(1);
      },
      error: err => {
        this.snackBar.open('Error occurred. Details: ' + err.name + ' ' + err.message, "Okay!", {duration: 8000});
        this.dialogRef.close(0);
      }
    });
  }
}
