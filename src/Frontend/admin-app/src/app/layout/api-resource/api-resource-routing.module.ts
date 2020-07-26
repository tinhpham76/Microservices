import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ApiResourceComponent } from './api-resource.component';
import { AddResourceComponent } from './add-resource/add-resource.component';
import { EditResourceComponent } from './edit-resource/edit-resource.component';
import { ResourceSecretComponent } from './resource-secret/resource-secret.component';
import { ResourcePropertyComponent } from './resource-property/resource-property.component';

const routes: Routes = [
    {
        path: '',
        component: ApiResourceComponent
    },
    {
        path: 'add',
        component: AddResourceComponent
    },
    {
        path: ':name/edit',
        component: EditResourceComponent
    },
    {
        path: ':name/secrets',
        component: ResourceSecretComponent
    },
    {
        path: ':name/properties',
        component: ResourcePropertyComponent
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ApiResourceRoutingModule { }