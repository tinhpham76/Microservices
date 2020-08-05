import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NzModalRef, NzModalService } from 'ng-zorro-antd/modal';
import { NzNotificationService, NzNotificationPlacement } from 'ng-zorro-antd/notification';
import { NzTableQueryParams } from 'ng-zorro-antd/table';
import { MessageConstants } from '@app/shared/constants/messages.constant';
import { ApiScopeServices } from '@app/shared/services/api-scope.services';

@Component({
  selector: 'app-api-scope',
  templateUrl: './api-scope.component.html',
  styleUrls: ['./api-scope.component.scss']
})
export class ApiScopeComponent implements OnInit {

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

  // Search value
  public searchValue = '';

  constructor(
    private apiScopeServices: ApiScopeServices,
    private notification: NzNotificationService,
    private modal: NzModalService
  ) { }

  ngOnInit(): void {
    // Load scopes
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

  // Load api scopes data
  loadApiData(filter: string, pageIndex: number, pageSize: number): void {
    this.isSpinning = true;
    this.apiScopeServices.getAllPaging(filter, pageIndex, pageSize)
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

  // Delete api scope
  delete(name: string) {
    this.isSpinning = true;
    this.apiScopeServices.delete(name)
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
      nzTitle: 'Do you Want to delete api scope?',
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
