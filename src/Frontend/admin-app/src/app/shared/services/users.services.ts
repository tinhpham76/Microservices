import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BaseService } from './base.service';
import { catchError, map } from 'rxjs/operators';
import { environment } from '@environments/environment';
import { User } from '../models/user.model';
import { Pagination } from '../models/pagination.model';
import { ChangePassword } from '../models/changePassword.model';

@Injectable({ providedIn: 'root' })
export class UserServices extends BaseService {
    private _sharedHeaders = new HttpHeaders();

    constructor(private http: HttpClient) {
        super();
        this._sharedHeaders = this._sharedHeaders.set('Content-Type', 'application/json');

    }
    add(entity: any) {
        return this.http.post(`${environment.user_api_url}/api/users`, JSON.stringify(entity), { headers: this._sharedHeaders })
            .pipe(catchError(this.handleError));
    }
    getAllPaging(filter, pageIndex, pageSize) {
        return this.http.get<Pagination<any>>(`${environment.user_api_url}/api/users/filter?pageIndex=${pageIndex}&pageSize=${pageSize}&filter=${filter}`, { headers: this._sharedHeaders })
            .pipe(map((response: Pagination<User>) => {
                return response;
            }), catchError(this.handleError));
    }
    delete(id) {
        return this.http.delete(environment.user_api_url + '/api/users/' + id, { headers: this._sharedHeaders })
            .pipe(
                catchError(this.handleError)
            );
    }
    getUserWithRoles(userId: string) {
        return this.http.get(`${environment.user_api_url}/api/users/${userId}/userRoles`, { headers: this._sharedHeaders })
            .pipe(catchError(this.handleError));
    }
    updateUserWithRoles(id: string, entity: any) {
        return this.http.put(`${environment.user_api_url}/api/users/${id}/userRoles`,
         JSON.stringify(entity), { headers: this._sharedHeaders })
            .pipe(catchError(this.handleError));
    }
    resetUserPassword(userId: string) {
        return this.http.put(`${environment.user_api_url}/api/users/${userId}/reset-password`, { headers: this._sharedHeaders })
            .pipe(catchError(this.handleError));
    }
}