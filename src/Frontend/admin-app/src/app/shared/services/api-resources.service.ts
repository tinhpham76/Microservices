import { Injectable } from "@angular/core";
import { BaseService } from './base.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '@environments/environment';
import { catchError, map } from 'rxjs/operators';
import { Pagination } from '../models/pagination.model';
import { ApiResource } from '../models/api-resource.model';

@Injectable({ providedIn: 'root' })
export class ApiResourceServices extends BaseService {
    private _sharedHeaders = new HttpHeaders();
    constructor(private http: HttpClient) {
        super();
        this._sharedHeaders = this._sharedHeaders.set('Content-Type', 'application/json');
    }
    add(entity: ApiResource) {
        return this.http.post(`${environment.admin_api_url}/api/ApiResources`, JSON.stringify(entity), { headers: this._sharedHeaders })
            .pipe(catchError(this.handleError));
    }
    update(name: string, entity: ApiResource) {
        return this.http.put(`${environment.admin_api_url}/api/ApiResources/${name}`,
            JSON.stringify(entity), { headers: this._sharedHeaders }).pipe(catchError(this.handleError));
    }
    getDetail(name: string) {
        return this.http.get<ApiResource>(`${environment.admin_api_url}/api/ApiResources/${name}`,
            { headers: this._sharedHeaders }).pipe(catchError(this.handleError));

    }
    getAllPaging(filter, pageIndex, pageSize) {
        return this.http.get<Pagination<ApiResource>>(`${environment.admin_api_url}/api/ApiResources/filter?filter=${filter}&pageIndex=${pageIndex}&pageSize=${pageSize}`, { headers: this._sharedHeaders })
            .pipe(catchError(this.handleError));
    }
    delete(name: string) {
        return this.http.delete(`${environment.admin_api_url}/api/ApiResources/${name}`, { headers: this._sharedHeaders })
            .pipe(catchError(this.handleError));
    }
    
    getApiScopes(){
        return this.http.get(`${environment.admin_api_url}/api/ApiResources`, { headers: this._sharedHeaders })
            .pipe(catchError(this.handleError));
    }

    getApiResourceSecret(name: string) {
        return this.http.get(`${environment.admin_api_url}/api/ApiResources/${name}/secrets`, { headers: this._sharedHeaders })
            .pipe(catchError(this.handleError));
    }
    addApiResourceSecret(name: string, entity: any) {
        return this.http.post(`${environment.admin_api_url}/api/ApiResources/${name}/secrets`,
            JSON.stringify(entity), { headers: this._sharedHeaders })
            .pipe(catchError(this.handleError));
    }
    deleteApiResourceSecret(name: string, secretId: number) {
        return this.http.delete(`${environment.admin_api_url}/api/ApiResources/${name}/secrets/${secretId}`
            , { headers: this._sharedHeaders }).pipe(catchError(this.handleError));
    }
    getApiResourceProperty(name: string) {
        return this.http.get(`${environment.admin_api_url}/api/ApiResources/${name}/properties`, { headers: this._sharedHeaders })
            .pipe(catchError(this.handleError));
    }
    addApiResourceProperty(name: string, entity: any) {
        return this.http.post(`${environment.admin_api_url}/api/ApiResources/${name}/properties`,
            JSON.stringify(entity), { headers: this._sharedHeaders })
            .pipe(catchError(this.handleError));
    }
    deleteApiResourceProperty(name: string, propertyKey: string) {
        return this.http.delete(`${environment.admin_api_url}/api/ApiResources/${name}/properties/${propertyKey}`
            , { headers: this._sharedHeaders }).pipe(catchError(this.handleError));
    }
}