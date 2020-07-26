import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { EditPermissionsComponent } from './edit-permissions/edit-permissions.component';
import { PermissionsComponent } from './permissions.component';

const routes: Routes = [
    {
        path: '',
        component: PermissionsComponent
    },
    {
        path: ':id/edit',
        component: EditPermissionsComponent
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class PermissionsRoutingModule { }