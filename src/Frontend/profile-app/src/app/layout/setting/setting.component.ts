import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { AuthService } from '@app/shared/services/auth.service';
import { UserServices } from '@app/shared/services/users.services';
import { NzNotificationService, NzNotificationPlacement } from 'ng-zorro-antd/notification';
import { MessageConstants } from '@app/shared/constants/messages.constant';
import { NzModalService } from 'ng-zorro-antd/modal';

@Component({
  selector: 'app-setting',
  templateUrl: './setting.component.html',
  styleUrls: ['./setting.component.scss']
})
export class SettingComponent implements OnInit {

  // Spin
  public isSpinning: boolean;
  // Init form
  public validateForm!: FormGroup;

  userId = '';

  constructor(private userServices: UserServices,
    private notification: NzNotificationService,
    private fb: FormBuilder,
    private authServices: AuthService
    ) { }

  ngOnInit(): void {
    this.userId = this.authServices.profile.sub;
    this.validateForm = this.fb.group({
      currentPassword: [null, [Validators.required]],
      newPassword: [null, [Validators.required]],
      checkPassword: [null, [Validators.required, this.confirmationValidator]],
    });
  }

  submitValidateForm(): void {
    this.isSpinning = true;
    this.userServices.changePassword(this.userId, this.validateForm.getRawValue())
      .subscribe(() => {
        this.createNotification(
          MessageConstants.TYPE_NOTIFICATION_SUCCESS,
          MessageConstants.TITLE_NOTIFICATION_SSO,
          MessageConstants.NOTIFICATION_UPDATE,
          'bottomRight');
        this.ngOnInit();
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

  // Validator
  confirmationValidator = (control: FormControl): { [s: string]: boolean } => {
    if (!control.value) {
      return { required: true };
    } else if (control.value !== this.validateForm.controls.newPassword.value) {
      return { confirm: true, error: true };
    }
    return {};
  }

  updateConfirmValidator(): void {
    /** wait for refresh value */
    Promise.resolve().then(() => this.validateForm.controls.checkPassword.updateValueAndValidity());
  }

  // Notification
  createNotification(type: string, title: string, content: string, position: NzNotificationPlacement): void {
    this.notification.create(type, title, content, { nzPlacement: position });
  }

}
