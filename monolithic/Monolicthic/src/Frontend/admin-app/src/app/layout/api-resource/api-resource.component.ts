import { Component, OnInit } from '@angular/core';
import { NzModalRef, NzModalService } from 'ng-zorro-antd/modal';
import { ApiResourceServices } from '@app/shared/services/api-resources.service';
import { NzNotificationService, NzNotificationPlacement } from 'ng-zorro-antd/notification';
import { NzTableQueryParams } from 'ng-zorro-antd/table';
import { MessageConstants } from '@app/shared/constants/messages.constant';

@Component({
  selector: 'app-api-resource',
  templateUrl: './api-resource.component.html',
  styleUrls: ['./api-resource.component.scss']
})
export class ApiResourceComponent implements OnInit {

  // Load api data
  public filter = '';
  public pageIndex = 1;
  public pageSize = 10;
  public items: any[];
  public totalRecords: number;
  public name: string;

  // Spin
  public isSpinning: boolean;

  // Modal
  public confirmDeleteModal?: NzModalRef;

  // Search
  public searchValue = '';

  constructor(
    private apiResourceServices: ApiResourceServices,
    private notification: NzNotificationService,
    private modal: NzModalService
  ) { }

  ngOnInit(): void {
    // Load api data
    this.loadApiData(this.filter, this.pageIndex, this.pageSize);
  }

  // Event change page
  onQueryParamsChange(params: NzTableQueryParams): void {
    const { pageSize, pageIndex } = params;
    this.loadApiData(this.searchValue, pageIndex, pageSize);
  }

  // Event search
  handleInputConfirm(): void {
    this.loadApiData(this.searchValue, this.pageIndex, this.pageSize);
  }

  // Load api data
  loadApiData(filter: string, pageIndex: number, pageSize: number): void {
    this.isSpinning = true;
    this.apiResourceServices.getAllPaging(filter, pageIndex, pageSize)
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

  // Delete api
  delete(name: string) {
    this.isSpinning = true;
    this.apiResourceServices.delete(name)
      .subscribe(() => {
        this.createNotification(
          MessageConstants.TYPE_NOTIFICATION_SUCCESS,
          MessageConstants.TITLE_NOTIFICATION,
          MessageConstants.NOTIFICATION_DELETE,
          'bottomRight'
        );
        this.ngOnInit();
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

  showDeleteConfirm(name: string): void {
    this.confirmDeleteModal = this.modal.confirm({
      nzTitle: 'Do you Want to delete api resource?',
      nzOkText: 'Yes',
      nzOkType: 'danger',
      nzOnOk: () =>
        new Promise((resolve, reject) => {
          this.delete(name);
          setTimeout(Math.random() > 0.5 ? resolve : reject, 200);
        })
    });
  }

  // Notification
  createNotification(type: string, title: string, content: string, position: NzNotificationPlacement): void {
    this.notification.create(type, title, content, { nzPlacement: position });
  }

}