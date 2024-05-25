import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { BookService } from 'src/app/services/book.service';

@Component({
  selector: 'app-reserve-book.dialog',
  templateUrl: './reserve-book.dialog.component.html',
  styleUrls: ['./reserve-book.dialog.component.css']
})
export class ReserveBookDialogComponent {
  response:boolean;
  isReserve: boolean;

  constructor(public dialogRef: MatDialogRef<ReserveBookDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any, public dataService: BookService, private snackBar: MatSnackBar) { }

  onNoClick(): void {
    this.dialogRef.close();
  }

  confirmReserve(): void {
    this.dataService.reserveBook(this.data.id, this.data.user).subscribe({
      next: data => {
        this.response = data as boolean;
        this.snackBar.open("Book reserved successfully", "Okay!", {duration: 3000});
        this.dialogRef.close(1);
      },
      error: err => {
        this.snackBar.open('Error occurred. Details: ' + err.name + ' ' + err.error.toString(), "Okay!", {duration: 8000});
        this.dialogRef.close(0);
      }
    });
  }

  confirmCancelReserve(): void {
    this.dataService.cancelReserveBook(this.data.id, this.data.user).subscribe({
      next: data => {
        this.response = data as boolean;
        this.snackBar.open("Book cancelled successfully", "Okay!", {duration: 3000});
        this.dialogRef.close(1);
      },
      error: err => {
        this.snackBar.open('Error occurred. Details: ' + err.name + ' ' + err.error.toString(), "Okay!", {duration: 8000});
        this.dialogRef.close(0);
      }
    });
  }
}
