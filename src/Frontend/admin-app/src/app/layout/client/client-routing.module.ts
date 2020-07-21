import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ClientComponent } from './client.component';
import { AddClientComponent } from './add-client/add-client.component';
import { EditClientComponent } from './edit-client/edit-client.component';
import { ClientSecretComponent } from './client-secret/client-secret.component';
import { ClientPropertyComponent } from './client-property/client-property.component';
import { ClientClaimComponent } from './client-claim/client-claim.component';

const routes: Routes = [
    { path: '', component: ClientComponent },
    { path: 'add', component: AddClientComponent },
    { path: ':id/edit', component: EditClientComponent },
    { path: ':id/secrets', component: ClientSecretComponent },
    { path: ':id/properties', component: ClientPropertyComponent },
    { path: ':id/claims', component: ClientClaimComponent }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ClientRoutingModule { }