import { Component, OnInit } from '@angular/core';
import { NzModalRef, NzModalService } from 'ng-zorro-antd/modal';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ApiScopeServices } from '@app/shared/services/api-scope.services';
import { ActivatedRoute } from '@angular/router';
import { NzNotificationService, NzNotificationPlacement } from 'ng-zorro-antd/notification';
import { MessageConstants } from '@app/shared/constants/messages.constant';

@Component({
  selector: 'app-scope-property',
  templateUrl: './scope-property.component.html',
  styleUrls: ['./scope-property.component.scss']
})
export class ScopePropertyComponent implements OnInit {

  // Spin
  public isSpinning: boolean;

  // Api name
  public apiName: string;

  // Confirm delete
  public confirmDeleteApiProperty: NzModalRef;

  // Api scope property data
  public itemApiProperties: any[];

  // Form property
  public propertyForm!: FormGroup;

  constructor(
    private notification: NzNotificationService,
    private route: ActivatedRoute,
    private apiScopeServices: ApiScopeServices,
    private fb: FormBuilder,
    private modal: NzModalService
  ) { }

  ngOnInit(): void {
    // Get api name
    this.route.params.subscribe(params => {
      this.apiName = params['name'];
    });
    // Form api scope property
    this.propertyForm = this.fb.group({
      key: [null, [Validators.required]],
      value: [null, [Validators.required]],
    });
    // Get api property
    this.getApiProperties(this.apiName);
  }

  // Get api property
  getApiProperties(apiName: string) {
    this.isSpinning = true;
    this.apiScopeServices.getApiScopeProperty(apiName)
      .subscribe((res: any[]) => {
        this.itemApiProperties = res;
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

  // Create api property
  submitFormApiProperty(value: {
    key: string;
    value: string;
  }): void {
    this.isSpinning = true;
    this.apiScopeServices.addApiScopeProperty(this.apiName, value)
      .subscribe(() => {
        this.getApiProperties(this.apiName);
        this.createNotification(
          MessageConstants.TYPE_NOTIFICATION_SUCCESS,
          MessageConstants.TITLE_NOTIFICATION,
          MessageConstants.NOTIFICATION_ADD,
          'bottomRight');
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

  // Delete api property
  deleteApiProperty(key: string) {
    this.isSpinning = true;
    this.apiScopeServices.deleteApiScopeProperty(this.apiName, key)
      .subscribe(() => {
        this.createNotification(
          MessageConstants.TYPE_NOTIFICATION_SUCCESS,
          MessageConstants.TITLE_NOTIFICATION,
          MessageConstants.NOTIFICATION_DELETE,
          'bottomRight'
        );
        this.getApiProperties(this.apiName);
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

  // Delete api property
  showDeleteConfirmApiProperty(key: string): void {
    this.confirmDeleteApiProperty = this.modal.confirm({
      nzTitle: 'Do you Want to delete api property key?',
      nzOkText: 'Yes',
      nzOkType: 'danger',
      nzOnOk: () =>
        new Promise((resolve, reject) => {
          this.deleteApiProperty(key);
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
