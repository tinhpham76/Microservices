import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ApiScopeServices } from '@app/shared/services/api-scope.services';
import { NzNotificationService, NzNotificationPlacement } from 'ng-zorro-antd/notification';
import { Router, ActivatedRoute } from '@angular/router';
import { MessageConstants } from '@app/shared/constants/messages.constant';

@Component({
  selector: 'app-edit-scope',
  templateUrl: './edit-scope.component.html',
  styleUrls: ['./edit-scope.component.scss']
})
export class EditScopeComponent implements OnInit {

  // Init form
  public validateForm!: FormGroup;

  // Spin
  public isSpinning: boolean;

  apiName = '';

  // Claims
  userClaims = [];
  claims = ['sub', 'name', 'given_name', 'family_name', 'middle_name',
    'nickname', 'preferred_username', 'profile', 'picture', 'website', 'email', 'email_verified',
    'gender', 'birthdate', 'zoneinfo', 'locale', 'phone_number', 'phone_number_verified', 'address', 'updated_at'];
  inputClaimVisible = false;
  inputClaimValue = '';

  @ViewChild('inputElement', { static: false }) inputElement?: ElementRef;

  constructor(private fb: FormBuilder,
    private apiScopeServices: ApiScopeServices,
    private notification: NzNotificationService,
    private router: Router,
    private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.apiName = params['name'];
    });
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
    this.getApiScope(this.apiName);
  }

  // Get detail api resource
  getApiScope(apiName: string) {
    this.isSpinning = true;
    this.apiScopeServices.getDetail(apiName)
      .subscribe((res: any) => {
        this.validateForm.setValue({
          name: res.name,
          displayName: res.displayName,
          description: res.description,
          enabled: res.enabled,
          showInDiscoveryDocument: res.showInDiscoveryDocument,
          required: res.required,
          emphasize: res.emphasize,
          userClaims: null
        });
        this.userClaims = res.userClaims;
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
    this.apiScopeServices.update(this.apiName, value)
      .subscribe(() => {
        this.createNotification(
          MessageConstants.TYPE_NOTIFICATION_SUCCESS,
          MessageConstants.TITLE_NOTIFICATION_SSO,
          MessageConstants.NOTIFICATION_UPDATE,
          'bottomRight');
        setTimeout(() => {
          this.getApiScope(this.apiName);
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
