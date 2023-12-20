// filter.component.ts
import { Component, EventEmitter, Output } from '@angular/core';

@Component({
  selector: 'app-filter',
  templateUrl: './filter.component.html',
  styleUrls: ['./filter.component.css'],
})
export class FilterComponent {
  @Output() filterChange = new EventEmitter<Record<string, string>>();
  selected:number=0;
//: Record<string, string>
  filters : {[prop:string] : string} = {
    title: 'rev',
    releaseYear: '',
    genre: '',
    status: '',
  };

  applyFilters() {
    this.filterChange.emit(this.filters);
  }
}
