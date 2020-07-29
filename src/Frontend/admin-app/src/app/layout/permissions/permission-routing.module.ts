import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { EditPermissionsComponent } from './edit-permissions/edit-permissions.component';
import { PermissionsComponent } from './permissions.component';
import { ClientPermissionComponent } from './client-permission/client-permission.component';

const routes: Routes = [
    {
        path: '',
        component: PermissionsComponent
    },
    {
        path: ':id/editPermissons',
        component: EditPermissionsComponent
    },
    {
        path: ':id/clientPermissons',
        component: ClientPermissionComponent
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class PermissionsRoutingModule { }