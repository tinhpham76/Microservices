import { Component, OnInit } from '@angular/core';
import { NzNotificationService, NzNotificationPlacement } from 'ng-zorro-antd/notification';
import { ActivatedRoute } from '@angular/router';
import { ClientServices } from '@app/shared/services/clients.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NzModalService, NzModalRef } from 'ng-zorro-antd/modal';
import { MessageConstants } from '@app/shared/constants/messages.constant';

@Component({
  selector: 'app-client-secret',
  templateUrl: './client-secret.component.html',
  styleUrls: ['./client-secret.component.scss']
})
export class ClientSecretComponent implements OnInit {

  // Spin
  public isSpinning: boolean;

  // Client id
  public clientId: string;

  // Confirm delete
  public confirmDeleteClientSecret?: NzModalRef;

  // Client secret data
  public itemClientSecrets: any[];

  // Form secret
  public secretForm!: FormGroup;

  constructor(
    private notification: NzNotificationService,
    private route: ActivatedRoute,
    private clientServices: ClientServices,
    private fb: FormBuilder,
    private modal: NzModalService
    ) { }

  ngOnInit(): void {
    // Get client id
    this.route.params.subscribe(params => {
      this.clientId = params['id'];
    });
    // Form client secrets
    this.secretForm = this.fb.group({
      type: ['SharedSecret'],
      value: [null, [Validators.required]],
      description: [null],
      expiration: [null],
      hashType: ['Sha256'],
    });
    // Get client secret
    this.getClientSecret(this.clientId);
  }

  // Get client secret
  getClientSecret(clientId: string) {
    this.isSpinning = true;
    this.clientServices.getClientSecret(clientId)
      .subscribe((res: any[]) => {
        this.itemClientSecrets = res;
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

  // Create client secret
  submitFormClientSecrets(): void {
    this.isSpinning = true;
    const data = this.secretForm.getRawValue();
    this.clientServices.addClientSecret(this.clientId, data)
      .subscribe(() => {
        this.getClientSecret(this.clientId);
        this.createNotification(
          MessageConstants.TYPE_NOTIFICATION_SUCCESS,
          MessageConstants.TITLE_NOTIFICATION,
          MessageConstants.NOTIFICATION_ADD,
          'bottomRight'
          );
        this.resetForm();
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

  // Delete client secret
  deleteClientSecret(id: string) {
    this.isSpinning = true;
    this.clientServices.deleteClientSecret(this.clientId, Number(id))
      .subscribe(() => {
        this.createNotification(
          MessageConstants.TYPE_NOTIFICATION_SUCCESS,
          MessageConstants.TITLE_NOTIFICATION,
          MessageConstants.NOTIFICATION_DELETE,
           'bottomRight'
           );
        setTimeout(() => {
          this.getClientSecret(this.clientId);
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

  // Delete client secret
  showDeleteConfirmClientSecrets(id: string): void {
    this.confirmDeleteClientSecret = this.modal.confirm({
      nzTitle: 'Do you Want to delete client secrets?',
      nzOkText: 'Yes',
      nzOkType: 'danger',
      nzOnOk: () =>
        new Promise((resolve, reject) => {
          this.deleteClientSecret(id);
          setTimeout(Math.random() > 0.5 ? resolve : reject, 200);
        })
    });
  }

  resetForm(): void {
    this.secretForm.reset();
    this.secretForm.setValue({
      type: 'SharedSecret',
      value: null,
      description: null,
      expiration: null,
      hashType: 'Sha256',
    });
  }

  // notification
  createNotification(type: string, title: string, content: string, position: NzNotificationPlacement): void {
    this.notification.create(type, title, content, { nzPlacement: position });
  }

}
