import { Component, OnInit, ElementRef, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { IdentityResourceServices } from '@app/shared/services/identity-resources.service';
import { NzNotificationService, NzNotificationPlacement } from 'ng-zorro-antd/notification';
import { Router } from '@angular/router';
import { MessageConstants } from '@app/shared/constants/messages.constant';

@Component({
  selector: 'app-add-resource',
  templateUrl: './add-resource.component.html',
  styleUrls: ['./add-resource.component.scss']
})
export class AddResourceComponent implements OnInit {

  // Init form
  public validateForm!: FormGroup;

  // Spin
  public isSpinning: boolean;

  // Claims
  userClaims = [];
  claims = ['sub', 'name', 'given_name', 'family_name', 'middle_name',
    'nickname', 'preferred_username', 'profile', 'picture', 'website', 'email', 'email_verified',
    'gender', 'birthdate', 'zoneinfo', 'locale', 'phone_number', 'phone_number_verified', 'address', 'updated_at'];
  inputClaimVisible = false;
  inputClaimValue = '';

  @ViewChild('inputElement', { static: false }) inputElement?: ElementRef;

  constructor(private fb: FormBuilder,
    private identityResourceServices: IdentityResourceServices,
    private notification: NzNotificationService,
    private router: Router) { }

  ngOnInit(): void {
    this.validateForm = this.fb.group({
      name: [null, [Validators.required]],
      displayName: [null, Validators.required],
      description: [null],
      enabled: [true],
      showInDiscoveryDocument: [true],
      required: [false],
      emphasize: [false],
      userClaims: [null]
    });
  }

  // Create new identity resource
  submitValidateForm(value:
    {
      name: string;
      displayName: string;
      description: string;
      enabled: boolean;
      showInDiscoveryDocument: boolean;
      required: boolean;
      emphasize: boolean;
      userClaims;
    }): void {
    this.isSpinning = true;
    value.userClaims = this.userClaims;
    this.identityResourceServices.add(value)
      .subscribe(() => {
        this.createNotification(
          MessageConstants.TYPE_NOTIFICATION_SUCCESS,
          MessageConstants.TITLE_NOTIFICATION,
          MessageConstants.NOTIFICATION_ADD,
          'bottomRight'
        );
        setTimeout(() => {
          this.isSpinning = false;
          this.router.navigate(['/identity-resources']);
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

  // Claims
  closeTagClaim(removedTag: {}): void {
    this.userClaims = this.userClaims.filter(tag => tag !== removedTag);
  }

  showClaimInput(): void {
    this.inputClaimVisible = true;
    setTimeout(() => {
      this.inputElement?.nativeElement.focus();
    }, 10);
  }

  inputConfirmClaim(): void {
    if (this.inputClaimValue && this.userClaims.indexOf(this.inputClaimValue) === -1) {
      this.userClaims = [...this.userClaims, this.inputClaimValue];
    }
    this.inputClaimValue = '';
    this.inputClaimVisible = false;
  }

  addClaim(claim: string): void {
    if (claim && this.userClaims.indexOf(claim) === -1) {
      this.userClaims = [...this.userClaims, claim];
    }
  }

  sliceTagName(tag: string): string {
    const isLongTag = tag.length > 50;
    return isLongTag ? `${tag.slice(0, 50)}...` : tag;
  }

  // Notification
  createNotification(type: string, title: string, content: string, position: NzNotificationPlacement): void {
    this.notification.create(type, title, content, { nzPlacement: position });
  }

}
