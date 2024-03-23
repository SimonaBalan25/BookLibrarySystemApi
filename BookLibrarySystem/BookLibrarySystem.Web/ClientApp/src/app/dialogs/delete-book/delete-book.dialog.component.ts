import { HttpErrorResponse } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { BookService } from 'src/app/services/book.service';

@Component({
  selector: 'app-delete.dialog',
  templateUrl: './delete-book.dialog.component.html',
  styleUrls: ['./delete-book.dialog.component.css']
})
export class DeleteBookDialogComponent {
  response:boolean;

  constructor(public dialogRef: MatDialogRef<DeleteBookDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any, public dataService: BookService, private snackBar: MatSnackBar) { }

  onNoClick(): void {
    this.dialogRef.close();
  }

  confirmDelete(): void {
    this.dataService.deleteBook(this.data.id).subscribe({
      next: data => {
        this.response = data as boolean;
        this.snackBar.open("Book deleted successfully", "Okay!", {duration: 3000});
        this.dialogRef.close(1);
      },
      error: err => {
        this.snackBar.open('Error occurred. Details: ' + err.name + ' ' + err.message, "Okay!", {duration: 8000});
        this.dialogRef.close(0);
      }
    });
  }
}
