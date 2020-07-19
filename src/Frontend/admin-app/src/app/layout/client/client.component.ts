import { Component, OnInit } from '@angular/core';
import { NzModalService, NzModalRef } from 'ng-zorro-antd/modal';
import { NzNotificationService, NzNotificationPlacement } from 'ng-zorro-antd/notification';
import { ClientServices } from '@app/shared/services/clients.service';
import { catchError } from 'rxjs/operators';
import { MessageConstants } from '@app/shared/constants/messages.constant';
import { throwError } from 'rxjs';
import { NzTableQueryParams } from 'ng-zorro-antd/table';
import { ClientQuickViews } from '@app/shared/models/views/client-quick-views.model';

@Component({
  selector: 'app-client',
  templateUrl: './client.component.html',
  styleUrls: ['./client.component.scss']
})
export class ClientComponent implements OnInit {

  // Load user data
  public filter = '';
  public pageIndex = 1;
  public pageSize = 10;
  public items: ClientQuickViews[];
  public totalRecords: number;

  // Spin
  public isSpinning: boolean;

  // Confirm delete client
  public confirmDeleteModal?: NzModalRef;

  // Search
  searchValue = '';
  visible = false;

  constructor(private clientServices: ClientServices,
    private notification: NzNotificationService,
    private modal: NzModalService) { }

  ngOnInit(): void {
    this.loadClientData(this.filter, this.pageIndex, this.pageSize);
  }

  // Load client data
  loadClientData(filter: string, pageIndex: number, pageSize: number): void {
    this.isSpinning = true;
    this.clientServices.getAllPaging(filter, pageIndex, pageSize)
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

  onQueryParamsChange(params: NzTableQueryParams): void {
    const { pageSize, pageIndex } = params;
    this.loadClientData(this.filter, pageIndex, pageSize);
  }

  reset(): void {
    this.searchValue = '';
    this.search();
  }

  search(): void {
    this.visible = false;
    this.loadClientData(this.searchValue, this.pageIndex, this.pageSize);
  }

  // Delete client
  delete(clientId: String) {
    this.isSpinning = true;
    this.clientServices.delete(clientId)
      .subscribe(() => {
        this.createNotification(
          MessageConstants.TYPE_NOTIFICATION_SUCCESS,
          MessageConstants.TITLE_NOTIFICATION_SSO,
          MessageConstants.NOTIFICATION_DELETE,
          'bottomRight'
        );
        this.loadClientData(this.filter, this.pageIndex, this.pageSize);
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

  showDeleteConfirm(clientId: string): void {
    this.confirmDeleteModal = this.modal.confirm({
      nzTitle: 'Do you Want to delete ' + clientId + ' client ?',
      nzOkText: 'Yes',
      nzOkType: 'danger',
      nzOnOk: () =>
        new Promise((resolve, reject) => {
          this.delete(clientId);
          setTimeout(Math.random() > 0.5 ? resolve : reject, 200);
        })
    });
  }

  // Notification
  createNotification(type: string, title: string, content: string, position: NzNotificationPlacement): void {
    this.notification.create(type, title, content, { nzPlacement: position });
  }

}
