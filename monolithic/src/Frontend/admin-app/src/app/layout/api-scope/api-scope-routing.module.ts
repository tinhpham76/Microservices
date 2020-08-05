import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ApiScopeComponent } from './api-scope.component';
import { AddScopeComponent } from './add-scope/add-scope.component';
import { EditScopeComponent } from './edit-scope/edit-scope.component';
import { ScopePropertyComponent } from './scope-property/scope-property.component';

const routes: Routes = [
    {
        path: '',
        component: ApiScopeComponent
    },
    {
        path: 'add',
        component: AddScopeComponent
    },
    {
        path: ':name/edit',
        component: EditScopeComponent
    },
    {
        path: ':name/properties',
        component: ScopePropertyComponent
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ApiScopeRoutingModule { }