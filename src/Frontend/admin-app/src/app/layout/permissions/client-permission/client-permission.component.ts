import { Component, OnInit } from '@angular/core';
import { MessageConstants } from '@app/shared/constants/messages.constant';
import { PermissionServices } from '@app/shared/services/permissions.service';
import { NzNotificationService, NzNotificationPlacement } from 'ng-zorro-antd/notification';
import { ActivatedRoute } from '@angular/router';
import { NzTableQueryParams } from 'ng-zorro-antd/table';

@Component({
  selector: 'app-client-permission',
  templateUrl: './client-permission.component.html',
  styleUrls: ['./client-permission.component.scss']
})
export class ClientPermissionComponent implements OnInit {

  // Spin
  public isSpinning = false;

  // Search value
  public searchValue = '';

  // load role data
  public filter = '';
  public pageIndex = 1;
  public pageSize = 10;
  public items: any[];
  public totalRecords: number;

  // Role id
  public roleId = '';

  constructor(
    private permissionServices: PermissionServices,
    private notification: NzNotificationService,
    private route: ActivatedRoute
  ) { }

  ngOnInit(): void {
    // Get role id
    this.route.params.subscribe(params => {
      this.roleId = params['id'];
    });
    // Load permission
    this.loadPermissions(this.roleId, this.filter, this.pageIndex, this.pageSize);
  }

  // Event change page
  onQueryParamsChange(params: NzTableQueryParams): void {
    const { pageSize, pageIndex } = params;
    this.loadPermissions(this.roleId, this.searchValue, pageIndex, pageSize);
  }

  // Event search
  handleInputConfirm(): void {
    this.loadPermissions(this.roleId, this.searchValue, this.pageIndex, this.pageSize);
  }

  // Load permission
  loadPermissions(roleId: string, filter: string, pageIndex: number, pageSize: number) {
    this.isSpinning = true;
    this.permissionServices.getClientPermissions(roleId, filter, pageIndex, pageSize)
      .subscribe(res => {
        this.items = res.items;
        this.totalRecords = res.totalRecords;
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

  // Notification
  createNotification(type: string, title: string, content: string, position: NzNotificationPlacement): void {
    this.notification.create(type, title, content, { nzPlacement: position });
  }

  editPermission(data) {
    this.isSpinning = true;
    this.permissionServices.postClientPermissions(this.roleId, data)
      .subscribe(() => {
        this.createNotification(
          MessageConstants.TYPE_NOTIFICATION_SUCCESS,
          MessageConstants.TITLE_NOTIFICATION,
          MessageConstants.NOTIFICATION_UPDATE,
          'bottomRight'
        );
        setTimeout(() => {
          this.loadPermissions(this.roleId, this.filter, this.pageIndex, this.pageSize);
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

}
