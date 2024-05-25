import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DeactivateUserDialogComponent } from './deactivate-user.dialog.component';

describe('DeactivateUserDialogComponent', () => {
  let component: DeactivateUserDialogComponent;
  let fixture: ComponentFixture<DeactivateUserDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DeactivateUserDialogComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DeactivateUserDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
