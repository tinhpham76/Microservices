import { Component, OnInit, ElementRef, ViewChild } from '@angular/core';
import { NzNotificationService, NzNotificationPlacement } from 'ng-zorro-antd/notification';
import { ActivatedRoute } from '@angular/router';
import { ClientServices } from '@app/shared/services/clients.service';
import { MessageConstants } from '@app/shared/constants/messages.constant';
import { FormGroup, FormBuilder, Validators, Form } from '@angular/forms';
import { NzModalService } from 'ng-zorro-antd/modal';

@Component({
  selector: 'app-edit-client',
  templateUrl: './edit-client.component.html',
  styleUrls: ['./edit-client.component.scss']
})
export class EditClientComponent implements OnInit {

  // Spin
  public isSpinning: boolean;

  // Client
  public clientId: string;
  // Tab basic setting
  public basicForm!: FormGroup;
  allowedCorsOrigins = [];
  inputAllowedCorsOriginsVisible = false;
  inputAllowedCorsOriginsValue = '';

  // Tab setting
  public settingForm!: FormGroup;
  allGrantTypes =[
    'authorization_code',
    'client_credentials',
    'refresh_token',
    'implicit',
    'password',
    'urn:ietf:params:oauth:grant-type:device_code'];
  allScopes = [];
  allowedScopes = [];
  redirectUris = [];
  allowedGrantTypes = [];
  inputAllowedScopesVisible = false;
  inputAllowedScopesValue = '';
  inputRedirectUrisVisible = false;
  inputRedirectUrisValue = '';
  inputAllowedGrantTypesVisible = false;
  inputAllowedGrantTypesValue = '';



  @ViewChild('inputElement', { static: false }) inputElement?: ElementRef;


  constructor(private notification: NzNotificationService,
    private route: ActivatedRoute,
    private clientServices: ClientServices,
    private fb: FormBuilder,
    private modal: NzModalService) { }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.clientId = params['id'];
    });
    // Init form basic
    this.basicForm = this.fb.group({
      clientId: [null, [Validators.required]],
      clientName: [null, [Validators.required]],
      description: [null],
      clientUri: [null, [Validators.required]],
      logoUri: [null],
      allowedCorsOrigins: [null]
    });
    // Init form setting
    this.settingForm = this.fb.group({
      enabled: [null],
      requireConsent: [null],
      allowRememberConsent: [null],
      allowOfflineAccess: [null],
      requireClientSecret: [null],
      protocolType: [null],
      requirePkce: [null],
      allowPlainTextPkce: [null],
      allowAccessTokensViaBrowser: [null],
      allowedScopes: [null],
      redirectUris: [null],
      allowedGrantTypes: [null]
    });

    this.getBasicSetting(this.clientId);
    this.getSettingClient(this.clientId);
    this.getAllScopes();
  }

  // Client setting basic
  getBasicSetting(clientId: string) {
    this.isSpinning = true;
    this.clientServices.getBasic(clientId)
      .subscribe((res: any) => {
        this.basicForm.setValue({
          clientId: res.clientId,
          clientName: res.clientName,
          description: res.description,
          clientUri: res.clientUri,
          logoUri: res.logoUri,
          allowedCorsOrigins: null
        });
        this.allowedCorsOrigins = res.allowedCorsOrigins;
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

  submitBasicForm(value:
    {
      clientId: string;
      clientName: string;
      description: string;
      clientUri: string;
      logoUri: string;
      allowedCorsOrigins
    }
  ): void {
    value.allowedCorsOrigins = this.allowedCorsOrigins;
    this.isSpinning = true;
    this.clientServices.putBasic(this.clientId, value)
      .subscribe(() => {
        this.createNotification(
          MessageConstants.TYPE_NOTIFICATION_SUCCESS,
          MessageConstants.TITLE_NOTIFICATION_SSO,
          MessageConstants.NOTIFICATION_ADD,
          'bottomRight');
        setTimeout(() => {
          this.getBasicSetting(this.clientId);
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
          this.ngOnInit();
          this.isSpinning = false;
        }, 500);
      });
  }

  // Cors Origins
  closeTagAllowedCorsOrigins(removedTag: {}): void {
    this.allowedCorsOrigins = this.allowedCorsOrigins.filter(tag => tag !== removedTag);
  }

  inputConfirmAllowedCorsOrigins(): void {
    if (this.inputAllowedCorsOriginsValue && this.allowedCorsOrigins.indexOf(this.inputAllowedCorsOriginsValue) === -1) {
      this.allowedCorsOrigins = [...this.allowedCorsOrigins, this.inputAllowedCorsOriginsValue];
    }
    this.inputAllowedCorsOriginsValue = '';
    this.inputAllowedCorsOriginsVisible = false;
  }

  showAllowedCorsOriginsInput(): void {
    this.inputAllowedCorsOriginsVisible = true;
    setTimeout(() => {
      this.inputElement?.nativeElement.focus();
    }, 10);
  }

  // Client Setting
  getSettingClient(clientId: string) {
    this.isSpinning = true;
    this.clientServices.getSetting(clientId)
      .subscribe((res: any) => {
        this.settingForm.setValue({
          enabled: res.enabled,
          requireConsent: res.requireConsent,
          allowRememberConsent: res.allowRememberConsent,
          allowOfflineAccess: res.allowOfflineAccess,
          requireClientSecret: res.requireClientSecret,
          protocolType: res.protocolType,
          requirePkce: res.requirePkce,
          allowPlainTextPkce: res.allowPlainTextPkce,
          allowAccessTokensViaBrowser: res.allowAccessTokensViaBrowser,
          allowedScopes: null,
          redirectUris: null,
          allowedGrantTypes: null
        });
        this.allowedScopes = res.allowedScopes;
        this.redirectUris = res.redirectUris;
        this.allowedGrantTypes = res.allowedGrantTypes;
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

  getAllScopes() {
    this.clientServices.getAllScope()
      .subscribe((res: any[]) => {
        this.allScopes = res;
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

  submitSettingForm(value:
    {
      enabled: boolean;
      requireConsent: boolean;
      allowRememberConsent: boolean;
      allowOfflineAccess: boolean;
      requireClientSecret: boolean;
      protocolType: string;
      requirePkce: boolean;
      allowPlainTextPkce: boolean;
      allowAccessTokensViaBrowser: boolean;
      allowedScopes;
      redirectUris;
      allowedGrantTypes;

    }
  ): void {
    value.allowedScopes = this.allowedScopes;
    value.redirectUris = this.redirectUris;
    value.allowedGrantTypes = this.allowedGrantTypes;
    this.isSpinning = true;
    this.clientServices.putSetting(this.clientId, value)
      .subscribe(() => {
        this.createNotification(
          MessageConstants.TYPE_NOTIFICATION_SUCCESS,
          MessageConstants.TITLE_NOTIFICATION_SSO,
          MessageConstants.NOTIFICATION_ADD,
          'bottomRight');
        setTimeout(() => {
          this.getBasicSetting(this.clientId);
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
          this.ngOnInit();
          this.isSpinning = false;
        }, 500);
      });
  }

  // Allowed Scopes
  addAllowedScope(scope: string) {   
    if (scope && this.allowedScopes.indexOf(scope) === -1) {
      this.allowedScopes = [...this.allowedScopes, scope];
    }
  }
  closeTagAllowedScopes(removedTag: {}): void {
    this.allowedScopes = this.allowedScopes.filter(tag => tag !== removedTag);
  }

  // Redirect Uris
  closeTagRedirectUris(removedTag: {}): void {
    this.redirectUris = this.redirectUris.filter(tag => tag !== removedTag);
  }

  inputConfirmRedirectUris(): void {
    if (this.inputRedirectUrisValue && this.redirectUris.indexOf(this.inputRedirectUrisValue) === -1) {
      this.redirectUris = [...this.redirectUris, this.inputRedirectUrisValue];
    }
    this.inputRedirectUrisValue = '';
    this.inputRedirectUrisVisible = false;
  }

  showRedirectUrisInput(): void {
    this.inputRedirectUrisVisible = true;
    setTimeout(() => {
      this.inputElement?.nativeElement.focus();
    }, 10);
  }

  // Allow Grant Types
  closeTagAllowedGrantTypes(removedTag: {}): void {
    this.allowedGrantTypes = this.allowedGrantTypes.filter(tag => tag !== removedTag);
  }

  addAllowedGrantTypes(grantType: string): void {
    if (grantType && this.allowedGrantTypes.indexOf(grantType) === -1) {
      this.allowedGrantTypes = [...this.allowedGrantTypes, grantType];
    }
  }


  // notification
  createNotification(type: string, title: string, content: string, position: NzNotificationPlacement): void {
    this.notification.create(type, title, content, { nzPlacement: position });
  }

  sliceTagName(tag: string): string {
    const isLongTag = tag.length > 50;
    return isLongTag ? `${tag.slice(0, 50)}...` : tag;
  }

}
