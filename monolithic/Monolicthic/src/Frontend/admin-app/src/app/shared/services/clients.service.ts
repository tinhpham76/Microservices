import { Injectable } from '@angular/core';
import { BaseService } from './base.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '@environments/environment';
import { catchError, map } from 'rxjs/operators';
import { Pagination } from '../models/pagination.model';

@Injectable({ providedIn: 'root' })
export class ClientServices extends BaseService {
    private _sharedHeaders = new HttpHeaders();
    constructor(private http: HttpClient) {
        super();
        this._sharedHeaders = this._sharedHeaders.set('Content-Type', 'application/json');
    }
    add(entity: any) {
        return this.http.post(`${environment.api_url}/api/clients`, JSON.stringify(entity), { headers: this._sharedHeaders })
            .pipe(catchError(this.handleError));
    }

    upload(entity: File) {
        return this.http.post(`${environment.api_url}/api/clients/upload`, entity, { headers: this._sharedHeaders })
            .pipe(catchError(this.handleError));
    }
    getAllPaging(filter, pageIndex, pageSize) {
        return this.http.get<Pagination<any>>(`${environment.api_url}/api/clients/filter?filter=${filter}&pageIndex=${pageIndex}&pageSize=${pageSize}`, { headers: this._sharedHeaders })
            .pipe(catchError(this.handleError));
    }
    delete(id) {
        return this.http.delete(`${environment.api_url}/api/clients/${id}`, { headers: this._sharedHeaders })
            .pipe(catchError(this.handleError));
    }
    // Setting basic
    getBasic(id) {
        return this.http.get(`${environment.api_url}/api/clients/${id}/basics`
            , { headers: this._sharedHeaders }).pipe(catchError(this.handleError));
    }
    putBasic(id, entity: any) {
        return this.http.put(`${environment.api_url}/api/clients/${id}/basics`
            , JSON.stringify(entity), { headers: this._sharedHeaders }).pipe(catchError(this.handleError));
    }
    // Setting setting
    getAllScope() {
        return this.http.get(`${environment.api_url}/api/clients`
            , { headers: this._sharedHeaders }).pipe(catchError(this.handleError));
    }
    getSetting(id) {
        return this.http.get(`${environment.api_url}/api/clients/${id}/settings`
            , { headers: this._sharedHeaders }).pipe(catchError(this.handleError));
    }
    putSetting(id, entity: any) {
        return this.http.put(`${environment.api_url}/api/clients/${id}/settings`
            , JSON.stringify(entity), { headers: this._sharedHeaders }).pipe(catchError(this.handleError));
    }
    getClientSecret(id: string) {
        return this.http.get(`${environment.api_url}/api/clients/${id}/settings/clientSecrets`, { headers: this._sharedHeaders })
            .pipe(catchError(this.handleError));
    }
    addClientSecret(id: string, entity: any) {
        return this.http.post(`${environment.api_url}/api/clients/${id}/settings/clientSecrets`,
            JSON.stringify(entity), { headers: this._sharedHeaders })
            .pipe(catchError(this.handleError));
    }
    deleteClientSecret(id: string, secretId: number) {
        return this.http.delete(`${environment.api_url}/api/clients/${id}/settings/clientSecrets/${secretId}`
            , { headers: this._sharedHeaders }).pipe(catchError(this.handleError));
    }
    getClientProperties(id: string) {
        return this.http.get(`${environment.api_url}/api/clients/${id}/settings/properties`, { headers: this._sharedHeaders })
            .pipe(catchError(this.handleError));
    }
    addClientProperty(id: string, entity: any) {
        return this.http.post(`${environment.api_url}/api/clients/${id}/settings/properties`,
            JSON.stringify(entity), { headers: this._sharedHeaders })
            .pipe(catchError(this.handleError));
    }
    deleteClientProperty(id: string, propertyKey: string) {
        return this.http.delete(`${environment.api_url}/api/clients/${id}/settings/properties/${propertyKey}`
            , { headers: this._sharedHeaders }).pipe(catchError(this.handleError));
    }
    // Setting authentication
    getAuthentication(id) {
        return this.http.get(`${environment.api_url}/api/clients/${id}/Authentications`
            , { headers: this._sharedHeaders }).pipe(catchError(this.handleError));
    }
    putAuthentication(id, entity: any) {
        return this.http.put(`${environment.api_url}/api/clients/${id}/Authentications`
            , JSON.stringify(entity), { headers: this._sharedHeaders }).pipe(catchError(this.handleError));
    }
    // Setting Token
    getToken(id) {
        return this.http.get(`${environment.api_url}/api/clients/${id}/Tokens`
            , { headers: this._sharedHeaders }).pipe(catchError(this.handleError));
    }
    putToken(id, entity: any) {
        return this.http.put(`${environment.api_url}/api/clients/${id}/Tokens`
            , JSON.stringify(entity), { headers: this._sharedHeaders }).pipe(catchError(this.handleError));
    }
    getClientClaims(id: string) {
        return this.http.get(`${environment.api_url}/api/clients/${id}/tokens/clientClaims`, { headers: this._sharedHeaders })
            .pipe(catchError(this.handleError));
    }
    addClientClaim(id: string, entity: any) {
        return this.http.post(`${environment.api_url}/api/clients/${id}/tokens/clientClaims`,
            JSON.stringify(entity), { headers: this._sharedHeaders })
            .pipe(catchError(this.handleError));
    }
    deleteClientClaim(id: string, claimType: string) {
        return this.http.delete(`${environment.api_url}/api/clients/${id}/tokens/clientClaims/${claimType}`
            , { headers: this._sharedHeaders }).pipe(catchError(this.handleError));
    }
    // Setting device flow
    getDeviceFlow(id) {
        return this.http.get(`${environment.api_url}/api/clients/${id}/deviceFlows`
            , { headers: this._sharedHeaders }).pipe(catchError(this.handleError));
    }
    putDeviceFlow(id, entity: any) {
        return this.http.put(`${environment.api_url}/api/clients/${id}/deviceFlows`
            , JSON.stringify(entity), { headers: this._sharedHeaders }).pipe(catchError(this.handleError));
    }
}