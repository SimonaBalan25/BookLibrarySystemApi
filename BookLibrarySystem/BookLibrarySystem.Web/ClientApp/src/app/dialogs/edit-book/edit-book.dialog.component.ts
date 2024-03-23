import { HttpErrorResponse } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { Book } from 'src/app/models/book';
import { BookService } from 'src/app/services/book.service';

@Component({
  selector: 'app-edit.dialog',
  templateUrl: './edit-book.dialog.component.html',
  styleUrls: ['./edit-book.dialog.component.css']
})
export class EditBookDialogComponent {

  dialogData: any;
  response: boolean;

  constructor(public dialogRef: MatDialogRef<EditBookDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any, public bookService: BookService, private snackBar: MatSnackBar) {
        this.response= false;
     }

  formControl = new FormControl('', [
      Validators.required
      // Validators.email,
  ]);


  submit() {
    // emppty stuff
  }

  getErrorMessage() {
    return this.formControl.hasError('required') ? 'Required field' :
      this.formControl.hasError('email') ? 'Not a valid email' :
        '';
  }

  onNoClick(): void {
    this.dialogRef.close();
  }

  stopEdit(): void {
    this.bookService.updateBook(<Book>this.data).subscribe({
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
