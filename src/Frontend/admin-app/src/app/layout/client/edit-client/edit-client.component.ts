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
  allGrantTypes = [
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

  // Tab Authentication
  public authenticationForm!: FormGroup;
  postLogoutRedirectUris = [];
  inputPostLogoutRedirectUrisVisible = false;
  inputPostLogoutRedirectUrisValue = '';
  @ViewChild('inputElement', { static: false }) inputElement?: ElementRef;

  // Tab Token
  public tokenForm!: FormGroup;

  // Tab Device Flow
  public deviceFlowForm!: FormGroup;

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

    // Init form authentication
    this.authenticationForm = this.fb.group({
      enableLocalLogin: [null],
      postLogoutRedirectUris: [null],
      frontChannelLogoutUri: [null],
      frontChannelLogoutSessionRequired: [null],
      backChannelLogoutUri: [null],
      backChannelLogoutSessionRequired: [null],
      userSsoLifetime: [null],
    });

    // Init form authentication
    this.tokenForm = this.fb.group({
      identityTokenLifetime: [null, [Validators.required]],
      accessTokenLifetime: [null, [Validators.required]],
      accessTokenType: [null],
      authorizationCodeLifetime: [null, [Validators.required]],
      absoluteRefreshTokenLifetime: [null, [Validators.required]],
      slidingRefreshTokenLifetime: [null, [Validators.required]],
      refreshTokenUsage: [null],
      refreshTokenExpiration: [null],
      updateAccessTokenClaimsOnRefresh: [null],
      includeJwtId: [null],
      alwaysSendClientClaims: [null],
      alwaysIncludeUserClaimsInIdToken: [null],
      pairWiseSubjectSalt: [null],
      clientClaimsPrefix: [null],
    });

    // Init form device flow
    this.deviceFlowForm = this.fb.group({
      userCodeType: [null],
      deviceCodeLifetime: [null, [Validators.required]]
    });

    this.getBasicSetting(this.clientId);
    this.getSettingClient(this.clientId);
    this.getAllScopes();
    this.getAuthenticationSetting(this.clientId);
    this.getTokenSetting(this.clientId);
    this.getDeviceFlowSetting(this.clientId);
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
          MessageConstants.NOTIFICATION_UPDATE,
          'bottomRight');
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
          MessageConstants.NOTIFICATION_UPDATE,
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

  // Client authentication setting
  getAuthenticationSetting(clientId: string) {
    this.isSpinning = true;
    this.clientServices.getAuthentication(clientId)
      .subscribe((res: any) => {
        this.authenticationForm.setValue({
          enableLocalLogin: res.enableLocalLogin,
          postLogoutRedirectUris: null,
          frontChannelLogoutUri: res.frontChannelLogoutUri,
          frontChannelLogoutSessionRequired: res.frontChannelLogoutSessionRequired,
          backChannelLogoutUri: res.backChannelLogoutUri,
          backChannelLogoutSessionRequired: res.backChannelLogoutSessionRequired,
          userSsoLifetime: res.userSsoLifetime,
        });
        this.postLogoutRedirectUris = res.postLogoutRedirectUris;
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

  submitAuthenticationForm(value:
    {
      enableLocalLogin: boolean;
      postLogoutRedirectUris;
      frontChannelLogoutUri: string;
      frontChannelLogoutSessionRequired: boolean;
      backChannelLogoutUri: string;
      backChannelLogoutSessionRequired: boolean;
      userSsoLifetime: number
    }
  ): void {
    value.postLogoutRedirectUris = this.postLogoutRedirectUris;
    this.isSpinning = true;
    this.clientServices.putAuthentication(this.clientId, value)
      .subscribe(() => {
        this.createNotification(
          MessageConstants.TYPE_NOTIFICATION_SUCCESS,
          MessageConstants.TITLE_NOTIFICATION_SSO,
          MessageConstants.NOTIFICATION_UPDATE,
          'bottomRight');
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

  // Cors Origins
  closeTagPostLogoutRedirectUris(removedTag: {}): void {
    this.postLogoutRedirectUris = this.postLogoutRedirectUris.filter(tag => tag !== removedTag);
  }

  inputConfirmPostLogoutRedirectUris(): void {
    if (this.inputPostLogoutRedirectUrisValue && this.postLogoutRedirectUris.indexOf(this.inputPostLogoutRedirectUrisValue) === -1) {
      this.postLogoutRedirectUris = [...this.postLogoutRedirectUris, this.inputPostLogoutRedirectUrisValue];
    }
    this.inputPostLogoutRedirectUrisValue = '';
    this.inputPostLogoutRedirectUrisVisible = false;
  }

  showPostLogoutRedirectUrisInput(): void {
    this.inputPostLogoutRedirectUrisVisible = true;
    setTimeout(() => {
      this.inputElement?.nativeElement.focus();
    }, 10);
  }

  // Token setting
  getTokenSetting(clientId: string) {
    this.isSpinning = true;
    this.clientServices.getToken(clientId)
      .subscribe((res: any) => {
        this.tokenForm.setValue({
          identityTokenLifetime: res.identityTokenLifetime,
          accessTokenLifetime: res.accessTokenLifetime,
          accessTokenType: res.accessTokenType,
          authorizationCodeLifetime: res.authorizationCodeLifetime,
          absoluteRefreshTokenLifetime: res.absoluteRefreshTokenLifetime,
          slidingRefreshTokenLifetime: res.slidingRefreshTokenLifetime,
          refreshTokenUsage: res.refreshTokenUsage,
          refreshTokenExpiration: res.refreshTokenExpiration,
          updateAccessTokenClaimsOnRefresh: res.updateAccessTokenClaimsOnRefresh,
          includeJwtId: res.includeJwtId,
          alwaysSendClientClaims: res.alwaysSendClientClaims,
          alwaysIncludeUserClaimsInIdToken: res.alwaysIncludeUserClaimsInIdToken,
          pairWiseSubjectSalt: res.pairWiseSubjectSalt,
          clientClaimsPrefix: res.clientClaimsPrefix
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

  submitTokenForm(value:
    {
      identityTokenLifetime: number;
      accessTokenLifetime: number;
      accessTokenType: string;
      authorizationCodeLifetime: number;
      absoluteRefreshTokenLifetime: number;
      slidingRefreshTokenLifetime: number;
      refreshTokenUsage: string;
      refreshTokenExpiration: string;
      updateAccessTokenClaimsOnRefresh: boolean;
      includeJwtId: boolean;
      alwaysSendClientClaims: boolean;
      alwaysIncludeUserClaimsInIdToken: boolean;
      pairWiseSubjectSalt: string;
      clientClaimsPrefix: string;
    }
  ): void {
    this.isSpinning = true;
    this.clientServices.putToken(this.clientId, value)
      .subscribe(() => {
        this.createNotification(
          MessageConstants.TYPE_NOTIFICATION_SUCCESS,
          MessageConstants.TITLE_NOTIFICATION_SSO,
          MessageConstants.NOTIFICATION_UPDATE,
          'bottomRight');
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

  // Client setting device flow
  getDeviceFlowSetting(clientId: string) {
    this.isSpinning = true;
    this.clientServices.getDeviceFlow(clientId)
      .subscribe((res: any) => {
        this.deviceFlowForm.setValue({
          userCodeType: res.userCodeType,
          deviceCodeLifetime: res.deviceCodeLifetime
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

  submitDeviceFlowForm(value:
    {
      userCodeType: string;
      deviceCodeLifetime: number;
    }
  ): void {
    this.isSpinning = true;
    this.clientServices.putDeviceFlow(this.clientId, value)
      .subscribe(() => {
        this.createNotification(
          MessageConstants.TYPE_NOTIFICATION_SUCCESS,
          MessageConstants.TITLE_NOTIFICATION_SSO,
          MessageConstants.NOTIFICATION_UPDATE,
          'bottomRight');
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

  // notification
  createNotification(type: string, title: string, content: string, position: NzNotificationPlacement): void {
    this.notification.create(type, title, content, { nzPlacement: position });
  }

  sliceTagName(tag: string): string {
    const isLongTag = tag.length > 50;
    return isLongTag ? `${tag.slice(0, 50)}...` : tag;
  }
}
