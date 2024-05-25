import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { UserService } from 'src/app/services/user.service';

@Component({
  selector: 'app-deactivate-user.dialog',
  templateUrl: './deactivate-user.dialog.component.html',
  styleUrls: ['./deactivate-user.dialog.component.css']
})
export class DeactivateUserDialogComponent {
  response:boolean;

  constructor(public dialogRef: MatDialogRef<DeactivateUserDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any, public userService: UserService, private snackBar: MatSnackBar) { }

  onNoClick(): void {
    this.dialogRef.close();
  }

  confirmDeactivate(id:string): void {
    this.userService.deactivateUser(this.data.id).subscribe({
      next: data => {
        this.response = data as boolean;
        this.snackBar.open("User deactivated successfully", "Okay!", {duration: 3000});
        this.dialogRef.close(1);
      },
      error: err => {
        this.snackBar.open('Error occurred. Details: ' + err.name + ' ' + err.message, "Okay!", {duration: 8000});
        this.dialogRef.close(0);
      }
    });
  }
}
