import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { IdentityResourceComponent } from './identity-resource.component';
import { ResourcePropertyComponent } from './resource-property/resource-property.component';
import { AddResourceComponent } from './add-resource/add-resource.component';
import { EditResourceComponent } from './edit-resource/edit-resource.component';

const routes: Routes = [
    { path: '', component: IdentityResourceComponent },
    { path: 'add', component: AddResourceComponent },
    { path: ':name/edit', component: EditResourceComponent },
    { path: ':name/properties', component: ResourcePropertyComponent }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class IdentityResourceRoutingModule { }