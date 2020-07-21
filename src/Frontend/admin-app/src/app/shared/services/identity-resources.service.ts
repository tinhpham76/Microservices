import { Injectable } from "@angular/core";
import { BaseService } from './base.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Role } from '../models/role.model';
import { environment } from '@environments/environment';
import { catchError, map } from 'rxjs/operators';
import { Pagination } from '../models/pagination.model';
import { IdentityResource } from '../models/identity-resource.model';

@Injectable({ providedIn: 'root' })
export class IdentityResourceServices extends BaseService {
    private _sharedHeaders = new HttpHeaders();
    constructor(private http: HttpClient) {
        super();
        this._sharedHeaders = this._sharedHeaders.set('Content-Type', 'application/json');
    }
    add(entity: IdentityResource) {
        return this.http.post(`${environment.admin_api_url}/api/IdentityResources`,
         JSON.stringify(entity), { headers: this._sharedHeaders })
            .pipe(catchError(this.handleError));
    }
    update(name: string, entity: IdentityResource) {
        return this.http.put(`${environment.admin_api_url}/api/IdentityResources/${name}`,
            JSON.stringify(entity), { headers: this._sharedHeaders }).pipe(catchError(this.handleError));
    }
    getDetail(name: string) {
        return this.http.get<IdentityResource>(`${environment.admin_api_url}/api/IdentityResources/${name}`,
            { headers: this._sharedHeaders }).pipe(catchError(this.handleError));

    }
    getAllPaging(filter, pageIndex, pageSize) {
        return this.http.get<Pagination<IdentityResource>>(`${environment.admin_api_url}/api/IdentityResources/filter?filter=${filter}&pageIndex=${pageIndex}&pageSize=${pageSize}`, { headers: this._sharedHeaders })
            .pipe(catchError(this.handleError));
    }
    delete(name: string) {
        return this.http.delete(`${environment.admin_api_url}/api/IdentityResources/${name}`, { headers: this._sharedHeaders })
            .pipe(catchError(this.handleError));
    }
    getIdentityProperty(name: string) {
        return this.http.get(`${environment.admin_api_url}/api/IdentityResources/${name}/properties`, { headers: this._sharedHeaders })
            .pipe(catchError(this.handleError));
    }
    addIdentityProperty(name: string, entity: any) {
        return this.http.post(`${environment.admin_api_url}/api/IdentityResources/${name}/properties`,
            JSON.stringify(entity), { headers: this._sharedHeaders })
            .pipe(catchError(this.handleError));
    }
    deleteIdentityProperty(name: string, propertyKey: string) {
        return this.http.delete(`${environment.admin_api_url}/api/IdentityResources/${name}/properties/${propertyKey}`
            , { headers: this._sharedHeaders }).pipe(catchError(this.handleError));
    }
}