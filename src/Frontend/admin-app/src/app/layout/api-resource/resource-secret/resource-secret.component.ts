import { Component, OnInit } from '@angular/core';
import { NzModalRef, NzModalService } from 'ng-zorro-antd/modal';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ApiResourceServices } from '@app/shared/services/api-resources.service';
import { NzNotificationService, NzNotificationPlacement } from 'ng-zorro-antd/notification';
import { ActivatedRoute } from '@angular/router';
import { MessageConstants } from '@app/shared/constants/messages.constant';

@Component({
  selector: 'app-resource-secret',
  templateUrl: './resource-secret.component.html',
  styleUrls: ['./resource-secret.component.scss']
})
export class ResourceSecretComponent implements OnInit {

  // Spin
  public isSpinning: boolean;

  // Api name
  public apiName: string;

  // Confirm delete
  public confirmDeleteApiSecret?: NzModalRef;

  // Api secret data
  public itemApiSecrets: any[];

  // Form secret
  public secretForm!: FormGroup;

  constructor(
    private apiResourceServices: ApiResourceServices,
    private notification: NzNotificationService,
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private modal: NzModalService
  ) { }

  ngOnInit(): void {
    // Get api name
    this.route.params.subscribe(params => {
      this.apiName = params['name'];
    });
    // Form api secrets
    this.secretForm = this.fb.group({
      type: ['SharedSecret'],
      value: [null, [Validators.required]],
      description: [null],
      expiration: [null],
      hashType: ['Sha256'],
    });
    // Get api secret
    this.getApiSecret(this.apiName);
  }

  // Get api secret
  getApiSecret(apiName: string) {
    this.isSpinning = true;
    this.apiResourceServices.getApiResourceSecret(apiName)
      .subscribe((res: any[]) => {
        this.itemApiSecrets = res;
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

  // Create new api secret
  submitFormApiSecrets(): void {
    this.isSpinning = true;
    const data = this.secretForm.getRawValue();
    console.log(data);
    this.apiResourceServices.addApiResourceSecret(this.apiName, data)
      .subscribe(() => {
        this.getApiSecret(this.apiName);
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

  // Delete api secret
  deleteApiSecret(id: string) {
    this.isSpinning = true;
    this.apiResourceServices.deleteApiResourceSecret(this.apiName, Number(id))
      .subscribe(() => {
        this.createNotification(
          MessageConstants.TYPE_NOTIFICATION_SUCCESS,
          MessageConstants.TITLE_NOTIFICATION,
          MessageConstants.NOTIFICATION_DELETE,
          'bottomRight'
        );
        setTimeout(() => {
          this.getApiSecret(this.apiName);
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

  // Delete api secret
  showDeleteConfirmApiSecrets(id: string): void {
    this.confirmDeleteApiSecret = this.modal.confirm({
      nzTitle: 'Do you Want to delete api secrets?',
      nzOkText: 'Yes',
      nzOkType: 'danger',
      nzOnOk: () =>
        new Promise((resolve, reject) => {
          this.deleteApiSecret(id);
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
