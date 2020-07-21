import { Component, OnInit } from '@angular/core';
import { NzModalRef, NzModalService } from 'ng-zorro-antd/modal';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NzNotificationService, NzNotificationPlacement } from 'ng-zorro-antd/notification';
import { ActivatedRoute } from '@angular/router';
import { ApiResourceServices } from '@app/shared/services/api-resources.service';
import { MessageConstants } from '@app/shared/constants/messages.constant';

@Component({
  selector: 'app-resource-property',
  templateUrl: './resource-property.component.html',
  styleUrls: ['./resource-property.component.scss']
})
export class ResourcePropertyComponent implements OnInit {

   // Spin
   public isSpinning: boolean;

   public apiName: string;
 
   confirmDeleteApiProperty: NzModalRef;
 
   public itemApiProperties: any[];
 
   public propertyForm!: FormGroup;

  constructor(private notification: NzNotificationService,
    private route: ActivatedRoute,
    private apiResourceServices: ApiResourceServices,
    private fb: FormBuilder,
    private modal: NzModalService) { }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.apiName = params['name'];
    });
    // Init form api property
    this.propertyForm = this.fb.group({
      key: [null, [Validators.required]],
      value: [null, [Validators.required]],
    });
    this.getApiProperties(this.apiName);
  }

  // Get api property
  getApiProperties(apiName: string) {
    this.isSpinning = true;
    this.apiResourceServices.getApiResourceProperty(apiName)
      .subscribe((res: any[]) => {
        this.itemApiProperties = res;
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

  // Create new api secret
  submitFormApiProperty(value: {
    key: string;
    value: string;
    }): void {
    this.isSpinning = true;
    this.apiResourceServices.addApiResourceProperty(this.apiName, value)
      .subscribe(() => {
        this.getApiProperties(this.apiName);
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

  // Delete api property
  deleteApiProperty(key: string) {
    this.isSpinning = true;
    this.apiResourceServices.deleteApiResourceProperty(this.apiName, key)
      .subscribe(() => {
        this.createNotification(
          MessageConstants.TYPE_NOTIFICATION_SUCCESS,
          MessageConstants.TITLE_NOTIFICATION_SSO,
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
          MessageConstants.TITLE_NOTIFICATION_SSO,
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
      nzTitle: 'Do you Want to delete api property key: ' + key + '?',
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
