import { Injectable } from "@angular/core";
import { BaseService } from './base.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '@environments/environment';
import { catchError, map } from 'rxjs/operators';
import { Pagination } from '../models/pagination.model';

@Injectable({ providedIn: 'root' })
export class PermissionServices extends BaseService {
    private _sharedHeaders = new HttpHeaders();
    constructor(private http: HttpClient) {
        super();
        this._sharedHeaders = this._sharedHeaders.set('Content-Type', 'application/json');
    }
    add(entity: any) {
        return this.http.post(`${environment.user_api_url}/api/roles`, JSON.stringify(entity), { headers: this._sharedHeaders })
            .pipe(catchError(this.handleError));
    }
    getDetail(id: string) {
        return this.http.get<any>(`${environment.user_api_url}/api/roles/${id}`, { headers: this._sharedHeaders })
            .pipe(catchError(this.handleError));
    }
    getAllPaging(filter, pageIndex, pageSize) {
        return this.http.get<Pagination<any>>(`${environment.user_api_url}/api/roles/filter?filter=${filter}&pageIndex=${pageIndex}&pageSize=${pageSize}`, { headers: this._sharedHeaders })
            .pipe(catchError(this.handleError));
    }
    delete(id) {
        return this.http.delete(`${environment.user_api_url}/api/roles/${id}`, { headers: this._sharedHeaders })
            .pipe(catchError(this.handleError));
    }
    getPermissions(roleId: string, filter, pageIndex, pageSize) {
        return this.http.get<Pagination<any>>(`${environment.user_api_url}/api/roles/${roleId}/claims/filter?filter=${filter}&pageIndex=${pageIndex}&pageSize=${pageSize}`, { headers: this._sharedHeaders })
            .pipe(catchError(this.handleError));
    }
    postPermissions(roleId: string, entity: any) {
        return this.http.post(`${environment.user_api_url}/api/roles/${roleId}/claims`,
            JSON.stringify(entity),
            { headers: this._sharedHeaders })
            .pipe(catchError(this.handleError));
    }

    getClientPermissions(roleId: string, filter, pageIndex, pageSize) {
        return this.http.get<Pagination<any>>(`${environment.user_api_url}/api/roles/${roleId}/clients/filter?filter=${filter}&pageIndex=${pageIndex}&pageSize=${pageSize}`, { headers: this._sharedHeaders })
            .pipe(catchError(this.handleError));
    }
    postClientPermissions(roleId: string, entity: any) {
        return this.http.post(`${environment.user_api_url}/api/roles/${roleId}/clients`,
            JSON.stringify(entity),
            { headers: this._sharedHeaders })
            .pipe(catchError(this.handleError));
    }
}