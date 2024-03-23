import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AuthorsService } from 'src/app/services/authors.service';

@Component({
  selector: 'app-delete-author.dialog',
  templateUrl: './delete-author.dialog.component.html',
  styleUrls: ['./delete-author.dialog.component.css']
})
export class DeleteAuthorDialogComponent {
  response:boolean;

  constructor(public dialogRef: MatDialogRef<DeleteAuthorDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any, public dataService: AuthorsService, private snackBar: MatSnackBar) { }

  onNoClick(): void {
    this.dialogRef.close();
  }

  confirmDelete(): void {
    this.dataService.deleteAuthor(this.data.id).subscribe({
      next: data => {
        this.response = data as boolean;
        this.snackBar.open("Author deleted successfully", "Okay!", {duration: 3000});
        this.dialogRef.close(1);
      },
      error: err => {
        this.snackBar.open('Error occurred. Details: ' + err.name + ' ' + err.message, "Okay!", {duration: 8000});
        this.dialogRef.close(0);
      }
    });
  }
}
