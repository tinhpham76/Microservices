import { Component, OnInit } from '@angular/core';
import { AuthService } from '@app/shared/services/auth.service';
import { Subscription } from 'rxjs';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { MessageConstants } from '@app/shared/constants/messages.constant';
import { Router } from '@angular/router';

@Component({
    selector: 'app-layout',
    templateUrl: './layout.component.html',
    styleUrls: ['./layout.component.scss']
})
export class LayoutComponent implements OnInit {

    subscription: Subscription;
    UserName: string;
    isAuthenticated: boolean;
    FullName: string;
    Email: string;
    constructor(
        private authServices: AuthService,
        private notification: NzNotificationService,
        private router: Router
    ) {
        this.subscription = this.authServices.authNavStatus$.subscribe(status => this.isAuthenticated = status);
        this.UserName = this.authServices.name;
        const profile = this.authServices.profile;
        this.FullName = profile.FullName;
        this.Email = profile.email;
        this.createNotification(this.authServices.name);
    }

    ngOnInit() { }
    async signOut() {
        localStorage.clear();
        sessionStorage.clear();
        this.router.navigate(['/login']);
    }
    createNotification(content: string): void {
        this.notification.create(
            MessageConstants.TYPE_NOTIFICATION_SUCCESS,
            MessageConstants.TITLE_NOTIFICATION_SSO,
            MessageConstants.NOTIFICATION_WELCOME + content
        );
    }

}
