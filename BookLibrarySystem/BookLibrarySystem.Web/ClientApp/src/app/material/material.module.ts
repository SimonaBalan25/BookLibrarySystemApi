import { NgModule } from '@angular/core';
import { MatToolbarModule} from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';

 const material = [
  MatToolbarModule,
  MatButtonModule,
  MatInputModule,
  MatIconModule
];


@NgModule({
  exports: [material],
  imports: [material]
})
export class MaterialModule { }
