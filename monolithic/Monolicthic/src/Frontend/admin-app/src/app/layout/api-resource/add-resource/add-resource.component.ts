import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ApiResourceServices } from '@app/shared/services/api-resources.service';
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

  // Scopes
  public scopes = [];
  public apiScopes = [];
  public inputScopeVisible = false;
  public inputScopeValue = '';

  // Claims
  public userClaims = [];
  public claims = [
    'sub', 'name', 'given_name', 'family_name', 'middle_name',
    'nickname', 'preferred_username', 'profile', 'picture', 'website',
    'email', 'email_verified', 'gender', 'birthdate', 'zoneinfo',
    'locale', 'phone_number', 'phone_number_verified', 'address', 'updated_at'
  ];
  public inputClaimVisible = false;
  public inputClaimValue = '';

  @ViewChild('inputElement', { static: false }) inputElement?: ElementRef;

  constructor(
    private fb: FormBuilder,
    private apiResourceServices: ApiResourceServices,
    private notification: NzNotificationService,
    private router: Router
  ) { }

  ngOnInit(): void {

    // Form add api
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

    // Load all scopes
    this.getApiScopes();
  }

  // Create api
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
    console.log(value);
    this.apiResourceServices.add(value)
      .subscribe(() => {
        this.createNotification(
          MessageConstants.TYPE_NOTIFICATION_SUCCESS,
          MessageConstants.TITLE_NOTIFICATION,
          MessageConstants.NOTIFICATION_ADD,
          'bottomRight'
        );
        setTimeout(() => {
          this.isSpinning = false;
          this.router.navigate(['/api-resources']);
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

  // Load all scopes
  getApiScopes() {
    this.apiResourceServices.getApiScopes()
      .subscribe((res: any[]) => {
        this.apiScopes = res;
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