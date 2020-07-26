import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { NzNotificationService, NzNotificationPlacement } from 'ng-zorro-antd/notification';
import { UserServices } from '@app/shared/services/users.services';
import { MessageConstants } from '@app/shared/constants/messages.constant';
import { NzSelectSizeType } from 'ng-zorro-antd/select';
import { environment } from '@environments/environment';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzUploadChangeParam } from 'ng-zorro-antd/upload';

@Component({
  selector: 'app-edit-user',
  templateUrl: './edit-user.component.html',
  styleUrls: ['./edit-user.component.scss']
})
export class EditUserComponent implements OnInit {

  // Spin
  public isSpinning: boolean;

  // Init form
  public validateForm!: FormGroup;

  // User id
  public userId = '';

  // Avatar uri
  public avatar = '';

  // Api upload file url
  public api_upload = (`${environment.storage_api_url}/api/files/upload`);

  // List role
  public listOfOptionRoles: Array<{ label: string; value: string }> = [];

  constructor(
    private fb: FormBuilder,
    private userServices: UserServices,
    private notification: NzNotificationService,
    private route: ActivatedRoute,
    private msg: NzMessageService
  ) { }

  ngOnInit(): void {
    // Get user id
    this.route.params.subscribe(params => {
      this.userId = params['id'];
    });
    // Form user
    this.validateForm = this.fb.group({
      id: [null],
      userName: [null, [Validators.required]],
      firstName: [null, [Validators.required]],
      lastName: [null, [Validators.required]],
      email: [null, [Validators.email, Validators.required]],
      dob: [null, [Validators.required]],
      phoneNumber: [null],
      createDate: [null],
      lastModifiedDate: [null],
      userRoles: [null],
      avatarUri: [null]
    });
    // Get user detail
    this.getUserDetail(this.userId);
  }

  // Get user
  getUserDetail(userId: string) {
    this.isSpinning = true;
    this.userServices.getUserWithRoles(userId)
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
          userRoles: res.userRoles,
          avatarUri: res.avatarUri
        });
        setTimeout(() => {
          this.avatar = res.avatarUri;
          this.listOfOptionRoles = res.roles;
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

  // Event upload file
  handleChange(info: NzUploadChangeParam): void {
    if (info.file.status !== 'uploading') {
    }
    if (info.file.status === 'done') {
      this.msg.success(`${info.file.name} file uploaded successfully`);
      this.avatar = (`${environment.storage_api_url}${info.file.response.filePath}`);
    } else if (info.file.status === 'error') {
      this.msg.error(`${info.file.name} file upload failed.`);
    }
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
    userRoles: any[]
  }): void {
    this.isSpinning = true;
    this.userServices.updateUserWithRoles(this.userId, value)
      .subscribe(() => {
        this.createNotification(
          MessageConstants.TYPE_NOTIFICATION_SUCCESS,
          MessageConstants.TITLE_NOTIFICATION,
          MessageConstants.NOTIFICATION_ADD,
          'bottomRight'
        );
        setTimeout(() => {
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

  // Reset password
  resetPassword() {
    this.isSpinning = true;
    this.userServices.resetUserPassword(this.userId)
      .subscribe(() => {
        this.createNotification(
          MessageConstants.TYPE_NOTIFICATION_SUCCESS,
          MessageConstants.TITLE_NOTIFICATION,
          'Success reset password to ' + 'User@123',
          'bottomRight'
        );
        setTimeout(() => {
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

  // Notification
  createNotification(type: string, title: string, content: string, position: NzNotificationPlacement): void {
    this.notification.create(type, title, content, { nzPlacement: position });
  }

}
