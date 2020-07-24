import { Component, OnInit } from '@angular/core';
import { UserServices } from '@app/shared/services/users.services';
import { MessageConstants } from '@app/shared/constants/messages.constant';
import { NzNotificationService, NzNotificationPlacement } from 'ng-zorro-antd/notification';
import { NzTableQueryParams } from 'ng-zorro-antd/table';
import { NzModalRef, NzModalService } from 'ng-zorro-antd/modal';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { error } from 'protractor';
import { AuthService } from '@app/shared/services/auth.service';

@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.scss'],
  providers: [DatePipe]
})
export class UserComponent implements OnInit {

  // Spin
  public isSpinning: boolean;
  // Init form
  public validateForm!: FormGroup;
  

  userId = '';

  constructor(private userServices: UserServices,
    private notification: NzNotificationService,
    private fb: FormBuilder,
    private authServices: AuthService,
    private modal: NzModalService) { }

  ngOnInit(): void {
    this.userId = this.authServices.profile.sub;
    this.validateForm = this.fb.group({
      id: [null],
      userName: [null, [Validators.required]],
      firstName: [null, [Validators.required]],
      lastName: [null, [Validators.required]],
      email: [null, [Validators.email, Validators.required]],
      dob: [null, [Validators.required]],
      phoneNumber: [null, [Validators.required]],
      createDate: [null],
      lastModifiedDate: [null],
    });
    this.getUserDetail(this.userId);
  }

  getUserDetail(userId: string) {
    this.isSpinning = true;
    this.userServices.getDetail(userId)
      .subscribe((res: any) => {
        this.validateForm.setValue({
          id: res.id,
          userName: res.userName,
          firstName: res.firstName,
          lastName: res.lastName,
          email: res.email,
          dob: res.dob,
          phoneNumber: res.phoneNumber,
          createDate: res.createDate,
          lastModifiedDate: res.lastModifiedDate,
        });
        setTimeout(() => {
          this.isSpinning = false;
        }, 500);
      }, errorMessage => {
        this.createNotification(
          MessageConstants.TYPE_NOTIFICATION_ERROR,
          MessageConstants.TITLE_NOTIFICATION_SSO,
          errorMessage,
          'bottomRight'
        );
        setTimeout(() => {
          this.isSpinning = false;
        }, 500);
      });
  }

  // Create new user
  submitValidateForm(value: {
    id: string;
    userName: string;
    firstName: string;
    lastName: string;
    email: string;
    dob: string;
    phoneNumber: string;
    createDate: string;
    lastModifiedDate: string;
  }): void {
    this.isSpinning = true;
    this.userServices.update(this.userId, value)
      .subscribe(() => {
        this.createNotification(
          MessageConstants.TYPE_NOTIFICATION_SUCCESS,
          MessageConstants.TITLE_NOTIFICATION_SSO,
          MessageConstants.NOTIFICATION_ADD,
          'bottomRight');
        setTimeout(() => {
          this.getUserDetail(this.userId);
          this.isSpinning = false;
        }, 500);
      }, errorMessage => {
        this.createNotification(
          MessageConstants.TYPE_NOTIFICATION_ERROR,
          MessageConstants.TITLE_NOTIFICATION_SSO,
          errorMessage,
          'bottomRight'
        );
        setTimeout(() => {
          this.isSpinning = false;
        }, 500);
      });
  }

  delete(): void {
    this.modal.confirm({
      nzTitle: 'Are you sure delete this account?',
      nzOkText: 'Yes',
      nzOkType: 'danger',
      nzOnOk: () => this.deleteAccount(),
      nzCancelText: 'No',
      nzOnCancel: () => console.log('Cancel')
    });
  }

  deleteAccount(): void {
    this.userServices.delete(this.userId)
      .subscribe(() => {
        this.authServices.signOut();
      }, errorMessage => {
        this.createNotification(
          MessageConstants.TYPE_NOTIFICATION_ERROR,
          MessageConstants.TITLE_NOTIFICATION_SSO,
          errorMessage,
          'bottomRight'
        );
        setTimeout(() => {
          this.isSpinning = false;
        }, 500);
      });
  }


  // Notification
  createNotification(type: string, title: string, content: string, position: NzNotificationPlacement): void {
    this.notification.create(type, title, content, { nzPlacement: position });
  }
}
