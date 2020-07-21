import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { NzModalRef, NzModalService } from 'ng-zorro-antd/modal';
import { NzNotificationService, NzNotificationPlacement } from 'ng-zorro-antd/notification';
import { ActivatedRoute } from '@angular/router';
import { ClientServices } from '@app/shared/services/clients.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MessageConstants } from '@app/shared/constants/messages.constant';

@Component({
  selector: 'app-client-claim',
  templateUrl: './client-claim.component.html',
  styleUrls: ['./client-claim.component.scss']
})
export class ClientClaimComponent implements OnInit {

  // Spin
  public isSpinning: boolean;

  public clientId: string;

  confirmDeleteClientClaim: NzModalRef;

  public itemClientClaims: any[];

  public claimForm!: FormGroup;

  // Claims
  clientClaim = '';
  claims = ['sub', 'name', 'given_name', 'family_name', 'middle_name',
    'nickname', 'preferred_username', 'profile', 'picture', 'website', 'email', 'email_verified',
    'gender', 'birthdate', 'zoneinfo', 'locale', 'phone_number', 'phone_number_verified', 'address', 'updated_at'];
  inputClaimVisible = false;
  inputClaimValue = '';

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
    // Init form client claims
    this.claimForm = this.fb.group({
      type: [null, [Validators.required]],
      value: [null, [Validators.required]],
    });
    this.getClientClaims(this.clientId);
  }

  // Get client claim
  getClientClaims(clientId: string) {
    this.isSpinning = true;
    this.clientServices.getClientClaims(clientId)
      .subscribe((res: any[]) => {
        this.itemClientClaims = res;
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

  addClaim(claim: string): void {
    if (claim && this.clientClaim.indexOf(claim) === -1) {
      this.clientClaim = claim;
    }
  }

  sliceTagName(tag: string): string {
    const isLongTag = tag.length > 50;
    return isLongTag ? `${tag.slice(0, 50)}...` : tag;
  }

  // Create new client claim
  submitClaimForm(value:
    {
      type: string;
      value: string;
    }): void {
    this.isSpinning = true;
    value.type = this.clientClaim;
    this.clientServices.addClientClaim(this.clientId, value)
      .subscribe(() => {
        this.getClientClaims(this.clientId);
        this.createNotification(
          MessageConstants.TYPE_NOTIFICATION_SUCCESS,
          MessageConstants.TITLE_NOTIFICATION_SSO,
          MessageConstants.NOTIFICATION_ADD,
          'bottomRight');
        this.resetForm();
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

  // Delete client claim
  deleteClientClaim(type: string) {
    this.isSpinning = true;
    this.clientServices.deleteClientClaim(this.clientId, type)
      .subscribe(() => {
        this.createNotification(
          MessageConstants.TYPE_NOTIFICATION_SUCCESS,
          MessageConstants.TITLE_NOTIFICATION_SSO,
          MessageConstants.NOTIFICATION_DELETE,
          'bottomRight'
        );
        this.getClientClaims(this.clientId);
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

  // Delete client Claim
  showDeleteConfirmClientClaim(type: string): void {
    this.confirmDeleteClientClaim = this.modal.confirm({
      nzTitle: 'Do you Want to delete client claim type: ' + type + '?',
      nzOkText: 'Yes',
      nzOkType: 'danger',
      nzOnOk: () =>
        new Promise((resolve, reject) => {
          this.deleteClientClaim(type);
          setTimeout(Math.random() > 0.5 ? resolve : reject, 200);
        })
    });
  }

  resetForm(): void {
    this.claimForm.reset();
    // tslint:disable-next-line: forin
    for (const key in this.claimForm.controls) {
      this.claimForm.controls[key].markAsPristine();
      this.claimForm.controls[key].updateValueAndValidity();
    }
  }

  // notification
  createNotification(type: string, title: string, content: string, position: NzNotificationPlacement): void {
    this.notification.create(type, title, content, { nzPlacement: position });
  }

}
