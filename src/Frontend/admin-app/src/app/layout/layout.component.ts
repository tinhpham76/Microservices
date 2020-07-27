import { Component, OnInit } from '@angular/core';
import { AuthService } from '@app/shared/services/auth.service';
import { Subscription } from 'rxjs';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { MessageConstants } from '@app/shared/constants/messages.constant';
import { Router } from '@angular/router';
import { environment } from '@environments/environment';

@Component({
    selector: 'app-layout',
    templateUrl: './layout.component.html',
    styleUrls: ['./layout.component.scss']
})
export class LayoutComponent implements OnInit {

    public authority = `${environment.authority}`;
    public profile_app_url = `${environment.profile_app_url}`;
    public subscription: Subscription;
    public UserName: string;
    public isAuthenticated: boolean;
    public FullName: string;
    public Email: string;
    public Avatar: string;
    constructor(
        private authServices: AuthService,
        private notification: NzNotificationService,
        private router: Router
    ) {
        this.subscription = this.authServices.authNavStatus$.subscribe(status => this.isAuthenticated = status);
        const profile = this.authServices.profile;
        this.FullName = profile.FullName;
        this.Email = profile.Email;
        this.UserName = profile.UserName;
        this.Avatar = profile.Avatar;
        this.createNotification(profile.UserName);
    }

    ngOnInit() { }

    // event sign out
    async signOut() {
        localStorage.clear();
        sessionStorage.clear();
        this.router.navigate(['/login']);
    }

    createNotification(content: string): void {
        this.notification.create(
            MessageConstants.TYPE_NOTIFICATION_SUCCESS,
            MessageConstants.TITLE_NOTIFICATION,
            MessageConstants.NOTIFICATION_WELCOME + content
        );
    }

}
