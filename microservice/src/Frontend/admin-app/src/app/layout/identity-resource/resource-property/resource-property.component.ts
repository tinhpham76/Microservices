import { Component, OnInit } from '@angular/core';
import { NzModalRef, NzModalService } from 'ng-zorro-antd/modal';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NzNotificationService, NzNotificationPlacement } from 'ng-zorro-antd/notification';
import { ActivatedRoute } from '@angular/router';
import { IdentityResourceServices } from '@app/shared/services/identity-resources.service';
import { MessageConstants } from '@app/shared/constants/messages.constant';

@Component({
  selector: 'app-resource-property',
  templateUrl: './resource-property.component.html',
  styleUrls: ['./resource-property.component.scss']
})
export class ResourcePropertyComponent implements OnInit {

  // Spin
  public isSpinning: boolean;

  // Identity name
  public identityName: string;

  // Confirm delete
  public confirmDeleteIdentityProperty: NzModalRef;

  // Identity property data
  public itemIdentityProperties: any[];

  // Form property
  public propertyForm!: FormGroup;

  constructor(
    private notification: NzNotificationService,
    private route: ActivatedRoute,
    private identityResourceServices: IdentityResourceServices,
    private fb: FormBuilder,
    private modal: NzModalService
  ) { }

  ngOnInit(): void {
    // Get identity name
    this.route.params.subscribe(params => {
      this.identityName = params['name'];
    });
    // Form identity property
    this.propertyForm = this.fb.group({
      key: [null, [Validators.required]],
      value: [null, [Validators.required]],
    });
    // Load identity property data
    this.getIdentityProperties(this.identityName);
  }

  // Get identity property
  getIdentityProperties(identityName: string) {
    this.isSpinning = true;
    this.identityResourceServices.getIdentityProperty(identityName)
      .subscribe((res: any[]) => {
        this.itemIdentityProperties = res;
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

  // Create identity property
  submitFormIdentityProperty(value: {
    key: string;
    value: string;
  }): void {
    this.isSpinning = true;
    this.identityResourceServices.addIdentityProperty(this.identityName, value)
      .subscribe(() => {
        this.getIdentityProperties(this.identityName);
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

  // Delete identity property
  deleteIdentityProperty(key: string) {
    this.isSpinning = true;
    this.identityResourceServices.deleteIdentityProperty(this.identityName, key)
      .subscribe(() => {
        this.createNotification(
          MessageConstants.TYPE_NOTIFICATION_SUCCESS,
          MessageConstants.TITLE_NOTIFICATION,
          MessageConstants.NOTIFICATION_DELETE,
          'bottomRight'
        );
        this.getIdentityProperties(this.identityName);
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

  // Delete identity property
  showDeleteConfirmIdentityProperty(key: string): void {
    this.confirmDeleteIdentityProperty = this.modal.confirm({
      nzTitle: 'Do you Want to delete identity property?',
      nzOkText: 'Yes',
      nzOkType: 'danger',
      nzOnOk: () =>
        new Promise((resolve, reject) => {
          this.deleteIdentityProperty(key);
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
