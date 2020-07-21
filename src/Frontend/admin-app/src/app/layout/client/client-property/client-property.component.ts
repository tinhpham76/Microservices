import { Component, OnInit } from '@angular/core';
import { NzModalRef, NzModalService } from 'ng-zorro-antd/modal';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NzNotificationService, NzNotificationPlacement } from 'ng-zorro-antd/notification';
import { ActivatedRoute } from '@angular/router';
import { ClientServices } from '@app/shared/services/clients.service';
import { MessageConstants } from '@app/shared/constants/messages.constant';

@Component({
  selector: 'app-client-property',
  templateUrl: './client-property.component.html',
  styleUrls: ['./client-property.component.scss']
})
export class ClientPropertyComponent implements OnInit {

  // Spin
  public isSpinning: boolean;

  public clientId: string;

  confirmDeleteClientProperty: NzModalRef;

  public itemClientProperties: any[];

  public propertyForm!: FormGroup;

  constructor(
    private notification: NzNotificationService,
    private route: ActivatedRoute,
    private clientServices: ClientServices,
    private fb: FormBuilder,
    private modal: NzModalService
  ) { }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.clientId = params['id'];
    });
    // Init form client secrets
    this.propertyForm = this.fb.group({
      key: [null, [Validators.required]],
      value: [null, [Validators.required]],
    });
    this.getClientProperties(this.clientId);
  }

  // Get client secret
  getClientProperties(clientId: string) {
    this.isSpinning = true;
    this.clientServices.getClientProperties(clientId)
      .subscribe((res: any[]) => {
        this.itemClientProperties = res;
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

  // Create new client secret
  submitFormClientProperty(value: {
    key: string;
    value: string;
    }): void {
    this.isSpinning = true;
    this.clientServices.addClientProperty(this.clientId, value)
      .subscribe(() => {
        this.getClientProperties(this.clientId);
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

  // Delete client property
  deleteClientProperty(key: string) {
    this.isSpinning = true;
    this.clientServices.deleteClientProperty(this.clientId, key)
      .subscribe(() => {
        this.createNotification(
          MessageConstants.TYPE_NOTIFICATION_SUCCESS,
          MessageConstants.TITLE_NOTIFICATION_SSO,
          MessageConstants.NOTIFICATION_DELETE,
          'bottomRight'
        );
        this.getClientProperties(this.clientId);
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

  // Delete client property
  showDeleteConfirmClientProperty(key: string): void {
    this.confirmDeleteClientProperty = this.modal.confirm({
      nzTitle: 'Do you Want to delete client property key: ' + key + ' ?',
      nzOkText: 'Yes',
      nzOkType: 'danger',
      nzOnOk: () =>
        new Promise((resolve, reject) => {
          this.deleteClientProperty(key);
          setTimeout(Math.random() > 0.5 ? resolve : reject, 200);
        })
    });
  }

  resetForm(): void {
    this.propertyForm.reset();
    // tslint:disable-next-line: forin
    for (const key in this.propertyForm.controls) {
      this.propertyForm.controls[key].markAsPristine();
      this.propertyForm.controls[key].updateValueAndValidity();
    }
  }

  // notification
  createNotification(type: string, title: string, content: string, position: NzNotificationPlacement): void {
    this.notification.create(type, title, content, { nzPlacement: position });
  }

}
