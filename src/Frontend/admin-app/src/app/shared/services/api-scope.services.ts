import { Injectable } from "@angular/core";
import { BaseService } from './base.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '@environments/environment';
import { catchError, map } from 'rxjs/operators';
import { Pagination } from '../models/pagination.model';
import { ApiScope } from '../models/api-scope.model';

@Injectable({ providedIn: 'root' })
export class ApiScopeServices extends BaseService {
    private _sharedHeaders = new HttpHeaders();
    constructor(private http: HttpClient) {
        super();
        this._sharedHeaders = this._sharedHeaders.set('Content-Type', 'application/json');
    }
    add(entity: ApiScope) {
        return this.http.post(`${environment.admin_api_url}/api/ApiScopes`, JSON.stringify(entity), { headers: this._sharedHeaders })
            .pipe(catchError(this.handleError));
    }
    update(name: string, entity: ApiScope) {
        return this.http.put(`${environment.admin_api_url}/api/ApiScopes/${name}`,
            JSON.stringify(entity), { headers: this._sharedHeaders }).pipe(catchError(this.handleError));
    }
    getDetail(name: string) {
        return this.http.get<ApiScope>(`${environment.admin_api_url}/api/ApiScopes/${name}`,
            { headers: this._sharedHeaders }).pipe(catchError(this.handleError));

    }
    getAllPaging(filter, pageIndex, pageSize) {
        return this.http.get<Pagination<ApiScope>>(`${environment.admin_api_url}/api/ApiScopes/filter?filter=${filter}&pageIndex=${pageIndex}&pageSize=${pageSize}`, { headers: this._sharedHeaders })
            .pipe(catchError(this.handleError));
    }
    delete(name: string) {
        return this.http.delete(`${environment.admin_api_url}/api/ApiScopes/${name}`, { headers: this._sharedHeaders })
            .pipe(catchError(this.handleError));
    }
    
    getApiScopeProperty(name: string) {
        return this.http.get(`${environment.admin_api_url}/api/ApiScopes/${name}/properties`, { headers: this._sharedHeaders })
            .pipe(catchError(this.handleError));
    }
    addApiScopeProperty(name: string, entity: any) {
        return this.http.post(`${environment.admin_api_url}/api/ApiScopes/${name}/properties`,
            JSON.stringify(entity), { headers: this._sharedHeaders })
            .pipe(catchError(this.handleError));
    }
    deleteApiScopeProperty(name: string, propertyKey: string) {
        return this.http.delete(`${environment.admin_api_url}/api/ApiScopes/${name}/properties/${propertyKey}`
            , { headers: this._sharedHeaders }).pipe(catchError(this.handleError));
    }
}