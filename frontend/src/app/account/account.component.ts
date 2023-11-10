import { Component, OnInit } from "@angular/core";
import {finalize, firstValueFrom, Observable} from "rxjs";
import { AccountService, AccountUpdate, User } from "./account.service";
import {FormBuilder, Validators} from "@angular/forms";
import {HttpEventType} from "@angular/common/http";

@Component({
  template: `
    <app-title title="Account"></app-title>
    <ion-content>
      <form [formGroup]="form">
        <ion-list class="field-list" *ngIf="!loading; else loadingSpinner">
          <ion-item>
            <ion-input label="Name" formControlName="fullName"></ion-input>
          </ion-item>

          <ion-item>
            <ion-input label="Email" formControlName="email"></ion-input>
          </ion-item>

          <ion-item>
            <ion-img [src]="avatarUrl"></ion-img>
            <ion-input label="Avatar" type="file" formControlName="avatar"
                       accept="image/png, image/jpeg"
                       (change)="onFileChanged($event)"></ion-input>
          </ion-item>

          <ion-item>
            <ion-toggle disabled [checked]="isAdmin">Administrator</ion-toggle>
          </ion-item>
        </ion-list>
        <ion-button [disabled]="uploading" (click)="submit()">Update</ion-button>
      </form>
      <ng-template #loadingSpinner>
        <ion-spinner></ion-spinner>
      </ng-template>
    </ion-content>
  `,
  styleUrls: ['./form.css'],
})
export class AccountComponent implements OnInit {
  form = this.fb.group({
    fullName: ["", Validators.required],
    email: ["", Validators.required],
    avatar: [null as File | null]
  });
  isAdmin?: boolean;
  avatarUrl?: string | ArrayBuffer | null;
  loading = true;
  uploadProgress: number | null = null;
  uploading = false;

  constructor(
    private readonly fb: FormBuilder,
    private readonly service: AccountService
  ) { }

  async ngOnInit() {
    const account = await firstValueFrom(this.service.getCurrentUser());
    this.form.patchValue(account);
    this.isAdmin = account.isAdmin;
    this.avatarUrl = account.avatarUrl;
    this.loading = false;
  }

  onFileChanged($event: Event) {
    const files = ($event.target as HTMLInputElement).files;
    if(!files) return;
    this.form.patchValue({avatar: files[0]});
    this.form.controls.avatar.updateValueAndValidity();
    const reader = new FileReader();
    reader.readAsDataURL(files[0]);
    reader.onload = () => {
      this.avatarUrl = reader.result;
    }
  }

  submit(){
    if(this.form.invalid) return;
    this.uploading = true;
    this.service.update(this.form.value as AccountUpdate)
      .pipe(finalize(() => {
        this.uploading = false;
        this.uploadProgress = null;
      }))
      .subscribe(event => {
        if(event.type == HttpEventType.UploadProgress) {
          this.uploadProgress = Math.round(100 * (event.loaded / (event.total ?? 1)));
        } else if (event.type == HttpEventType.Response && event.body) {
          this.form.patchValue(event.body);
        }
    });
  }
}
