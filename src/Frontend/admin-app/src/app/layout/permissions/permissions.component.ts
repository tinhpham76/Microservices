import { Component, OnInit } from '@angular/core';
import { NzModalRef, NzModalService } from 'ng-zorro-antd/modal';
import { PermissionServices } from '@app/shared/services/permissions.service';
import { NzNotificationService, NzNotificationPlacement } from 'ng-zorro-antd/notification';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NzTableQueryParams } from 'ng-zorro-antd/table';
import { MessageConstants } from '@app/shared/constants/messages.constant';
import { NzDrawerPlacement } from 'ng-zorro-antd/drawer';

@Component({
  selector: 'app-permissions',
  templateUrl: './permissions.component.html',
  styleUrls: ['./permissions.component.scss']
})
export class PermissionsComponent implements OnInit {

  validateForm!: FormGroup;
  // load role data
  public filter = '';
  public pageIndex = 1;
  public pageSize = 10;
  public items: any[];
  public totalRecords: number;

  // button confirmModal
  public confirmModal?: NzModalRef;
  // Spin
  public isSpinning: boolean;

  searchValue = '';

  visible = false;

  constructor(private permissionServices: PermissionServices,
    private notification: NzNotificationService,
    private modal: NzModalService,
    private fb: FormBuilder) { }

  ngOnInit(): void {
    this.loadRoleData(this.filter, this.pageIndex, this.pageSize);
    this.validateForm = this.fb.group({
      id: [null, [Validators.required]],
      name: [null, [Validators.required]],
      normalizedName: [null]
    });
  }

  open(): void {
    this.visible = true;
  }

  close(): void {
    this.visible = false;
  }

  handleInputConfirm(): void {
    this.loadRoleData(this.searchValue, this.pageIndex, this.pageSize);
  }

  // Load Role
  loadRoleData(filter: string, pageIndex: number, pageSize: number): void {
    this.isSpinning = true;
    this.permissionServices.getAllPaging(filter, pageIndex, pageSize)
      .subscribe(res => {
        this.items = res.items;
        this.totalRecords = res.totalRecords;
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

  // Delete Role
  delete(roleId: string) {
    if (roleId === 'Admin') {
      this.createNotification(
        MessageConstants.TYPE_NOTIFICATION_ERROR,
        MessageConstants.TITLE_NOTIFICATION_SSO,
        'Role Admin cannot delete!',
        'bottomRight'
      );
    } else {
      this.isSpinning = true;
      this.permissionServices.delete(roleId)
        .subscribe(() => {
          setTimeout(() => {
            this.isSpinning = false;
            this.ngOnInit();
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
            this.ngOnInit();
          }, 500);
        });
    }

  }

  showConfirm(roleId: string): void {
    this.confirmModal = this.modal.confirm({
      nzTitle: 'Do you Want to delete ' + roleId + ' role ?',
      nzOkText: 'Yes',
      nzOkType: 'danger',
      nzOnOk: () =>
        new Promise((resolve, reject) => {
          this.delete(roleId);
          setTimeout(Math.random() > 0.5 ? resolve : reject, 200);
        })
    });
  }

  // Create new client
  submitForm(value: { id: string, name: string, normalizedName: string }): void {
    this.isSpinning = true;
    this.permissionServices.add(value)
      .subscribe(() => {
        this.createNotification(
          MessageConstants.TYPE_NOTIFICATION_SUCCESS,
          MessageConstants.TITLE_NOTIFICATION_SSO,
          MessageConstants.NOTIFICATION_ADD,
          'bottomRight');
        setTimeout(() => {
          this.close();
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



  // Hiển thị thêm dữ liệu khác
  onQueryParamsChange(params: NzTableQueryParams): void {
    const { pageSize, pageIndex } = params;
    this.loadRoleData(this.filter, pageIndex, pageSize);
  }

  // Tạo thông báo
  createNotification(type: string, title: string, content: string, position: NzNotificationPlacement): void {
    this.notification.create(type, title, content, { nzPlacement: position });
  }
}
