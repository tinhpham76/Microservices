import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { catchError } from 'rxjs/operators';
import { NzNotificationPlacement, NzNotificationService } from 'ng-zorro-antd/notification';
import { UserServices } from '@app/shared/services/users.services';
import { MessageConstants } from '@app/shared/constants/messages.constant';
import { throwError } from 'rxjs';
import { Router } from '@angular/router';
import { environment } from '@environments/environment';
import { NzUploadChangeParam } from 'ng-zorro-antd/upload';
import { NzMessageService } from 'ng-zorro-antd/message';

@Component({
  selector: 'app-add-user',
  templateUrl: './add-user.component.html',
  styleUrls: ['./add-user.component.scss']
})
export class AddUserComponent implements OnInit {

  // Avatar uri
  public avatar = '';

  // Api upload file url
  public api_upload = (`${environment.api_url}/api/files/upload`);

  // Spin
  public isSpinning: boolean;

  // Form user
  public validateForm!: FormGroup;

  constructor(
    private fb: FormBuilder,
    private userServices: UserServices,
    private notification: NzNotificationService,
    private router: Router,
    private msg: NzMessageService
  ) { }

  ngOnInit(): void {
    this.validateForm = this.fb.group({
      userName: [null, [Validators.required]],
      firstName: [null, [Validators.required]],
      lastName: [null, [Validators.required]],
      email: [null, [Validators.email, Validators.required]],
      dob: [null, [Validators.required]],
      password: [null, [Validators.required]],
      checkPassword: [null, [Validators.required, this.confirmationValidator]],
      phoneNumberPrefix: ['+84'],
      phoneNumber: [null, [Validators.required]],
      avatarUri: [null, [Validators.required]]
    });
  }

  // Event upload file
  handleChange(info: NzUploadChangeParam): void {
    if (info.file.status !== 'uploading') {
    }
    if (info.file.status === 'done') {
      this.msg.success(`${info.file.name} file uploaded successfully`);
      this.avatar = (`${environment.api_url}${info.file.response.filePath}`);
    } else if (info.file.status === 'error') {
      this.msg.error(`${info.file.name} file upload failed.`);
    }
  }

  // Validator
  confirmationValidator = (control: FormControl): { [s: string]: boolean } => {
    if (!control.value) {
      return { required: true };
    } else if (control.value !== this.validateForm.controls.password.value) {
      return { confirm: true, error: true };
    }
    return {};
  }

  updateConfirmValidator(): void {
    /** wait for refresh value */
    Promise.resolve().then(() => this.validateForm.controls.checkPassword.updateValueAndValidity());
  }

  // Create new user
  submitValidateForm(value: {
    userName: string;
    firstName: string;
    lastName: string;
    email: string;
    dob: string;
    password: string;
    phoneNumber: string;
    phoneNumberPrefix: string;
    avatarUri: string;
  }): void {
    if (value.password.length < 8) {
      this.createNotification(
        MessageConstants.TYPE_NOTIFICATION_WARNING,
        MessageConstants.TITLE_NOTIFICATION,
        'Password required 8 charter!',
        'bottomRight'
      );
    } else {
      this.isSpinning = true;
      value.phoneNumber = value.phoneNumberPrefix + value.phoneNumber;
      this.userServices.add(value)
        .subscribe(() => {
          this.createNotification(
            MessageConstants.TYPE_NOTIFICATION_SUCCESS,
            MessageConstants.TITLE_NOTIFICATION,
            MessageConstants.NOTIFICATION_ADD,
            'bottomRight');
          setTimeout(() => {
            this.router.navigate(['/users']);
            this.isSpinning = false;
          }, 500);
        }, errorMessage => {
          this.createNotification(
            MessageConstants.TYPE_NOTIFICATION_ERROR,
            MessageConstants.TITLE_NOTIFICATION,
            errorMessage,
            'bottomRight'
          );
          setTimeout(() => {
            this.isSpinning = false;
          }, 500);
        });
    }
  }

  // Notification
  createNotification(type: string, title: string, content: string, position: NzNotificationPlacement): void {
    this.notification.create(type, title, content, { nzPlacement: position });
  }

}
