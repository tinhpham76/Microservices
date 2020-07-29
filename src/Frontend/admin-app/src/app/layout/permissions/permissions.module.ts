import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PermissionsComponent } from './permissions.component';
import { EditPermissionsComponent } from './edit-permissions/edit-permissions.component';
import { PermissionsRoutingModule } from './permission-routing.module';
import { NzTabsModule } from 'ng-zorro-antd/tabs';
import { NzDatePickerModule } from 'ng-zorro-antd/date-picker';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzDividerModule } from 'ng-zorro-antd/divider';
import { NzCollapseModule } from 'ng-zorro-antd/collapse';
import { NzDropDownModule } from 'ng-zorro-antd/dropdown';
import { NzSwitchModule } from 'ng-zorro-antd/switch';
import { NzSpinModule } from 'ng-zorro-antd/spin';
import { NzDescriptionsModule } from 'ng-zorro-antd/descriptions';
import { NzNotificationModule } from 'ng-zorro-antd/notification';
import { NzCheckboxModule } from 'ng-zorro-antd/checkbox';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { NzToolTipModule } from 'ng-zorro-antd/tooltip';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NzDrawerModule } from 'ng-zorro-antd/drawer';
import { NzGridModule } from 'ng-zorro-antd/grid';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzAlertModule } from 'ng-zorro-antd/alert';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzTableModule } from 'ng-zorro-antd/table';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzTransferModule } from 'ng-zorro-antd/transfer';
import { NzTreeSelectModule } from 'ng-zorro-antd/tree-select';
import { ClientPermissionComponent } from './client-permission/client-permission.component';

@NgModule({
  declarations: [PermissionsComponent, EditPermissionsComponent, ClientPermissionComponent],
  imports: [
    CommonModule,
    NzTableModule,
    NzTreeSelectModule,
    NzTabsModule,
    NzDatePickerModule,
    NzButtonModule,
    NzDividerModule,
    NzCollapseModule,
    NzDropDownModule,
    NzTransferModule,
    NzSwitchModule,
    NzSpinModule,
    NzDescriptionsModule,
    NzNotificationModule,
    NzCardModule,
    NzCheckboxModule,
    NzButtonModule,
    NzModalModule,
    NzTagModule,
    NzToolTipModule,
    NzIconModule,
    FormsModule,
    ReactiveFormsModule,
    NzDrawerModule,
    NzGridModule,
    NzDatePickerModule,
    NzFormModule,
    NzAlertModule,
    NzInputModule,
    NzModalModule,
    NzSelectModule,
    CommonModule,
    PermissionsRoutingModule,
  ]
})
export class PermissionsModule { }
