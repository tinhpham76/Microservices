import { Component, OnInit, ElementRef, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ApiResourceServices } from '@app/shared/services/api-resources.service';
import { NzNotificationService, NzNotificationPlacement } from 'ng-zorro-antd/notification';
import { Router, ActivatedRoute } from '@angular/router';
import { MessageConstants } from '@app/shared/constants/messages.constant';

@Component({
  selector: 'app-edit-resource',
  templateUrl: './edit-resource.component.html',
  styleUrls: ['./edit-resource.component.scss']
})
export class EditResourceComponent implements OnInit {

  apiName = '';

  // Init form
  public validateForm!: FormGroup;

  // Spin
  public isSpinning: boolean;

  // Scopes
  scopes = [];
  apiScopes = [];
  inputScopeVisible = false;
  inputScopeValue = '';

  // Claims
  userClaims = [];
  claims = ['sub', 'name', 'given_name', 'family_name', 'middle_name',
    'nickname', 'preferred_username', 'profile', 'picture', 'website', 'email', 'email_verified',
    'gender', 'birthdate', 'zoneinfo', 'locale', 'phone_number', 'phone_number_verified', 'address', 'updated_at'];
  inputClaimVisible = false;
  inputClaimValue = '';

  @ViewChild('inputElement', { static: false }) inputElement?: ElementRef;

  constructor(private fb: FormBuilder,
    private apiResourceServices: ApiResourceServices,
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
      allowedAccessTokenSigningAlgorithms: [null],
      scopes: [null],
      userClaims: [null]
    });
    this.getApiScopes();
    this.getApiResource(this.apiName);
  }

  getApiScopes() {
    this.apiResourceServices.getApiScopes()
      .subscribe((res: any[]) => {
        this.apiScopes = res;
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

  // Get detail api resource
  getApiResource(apiName: string) {
    this.isSpinning = true;
    this.apiResourceServices.getDetail(apiName)
      .subscribe((res: any) => {
        this.validateForm.setValue({
          name: res.name,
          displayName: res.displayName,
          description: res.description,
          enabled: res.enabled,
          showInDiscoveryDocument: res.showInDiscoveryDocument,
          allowedAccessTokenSigningAlgorithms: res.allowedAccessTokenSigningAlgorithms,
          scopes: null,
          userClaims: null
        });
        this.scopes = res.scopes;
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

  // Save change api resource
  submitValidateForm(value:
    {
      name: string;
      displayName: string;
      description: string;
      enabled: boolean;
      showInDiscoveryDocument: boolean;
      allowedAccessTokenSigningAlgorithms: string;
      scopes;
      userClaims;
    }): void {
    this.isSpinning = true;
    value.scopes = this.scopes;
    value.userClaims = this.userClaims;
    this.apiResourceServices.update(this.apiName, value)
      .subscribe(() => {
        this.createNotification(
          MessageConstants.TYPE_NOTIFICATION_SUCCESS,
          MessageConstants.TITLE_NOTIFICATION_SSO,
          MessageConstants.NOTIFICATION_ADD,
          'bottomRight');
          this.getApiResource(this.apiName);
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

  // Scopes
  closeTagScope(removedTag: {}): void {
    this.scopes = this.scopes.filter(tag => tag !== removedTag);
  }

  showScopeInput(): void {
    this.inputScopeVisible = true;
    setTimeout(() => {
      this.inputElement?.nativeElement.focus();
    }, 10);
  }

  inputConfirmScope(): void {
    if (this.inputScopeValue && this.scopes.indexOf(this.inputScopeValue) === -1) {
      this.scopes = [...this.scopes, this.inputScopeValue];
    }
    this.inputScopeValue = '';
    this.inputScopeVisible = false;
  }

  addScope(scope: string): void {
    if (scope && this.scopes.indexOf(scope) === -1) {
      this.scopes = [...this.scopes, scope];
    }
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
