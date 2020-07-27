// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environment = {
  production: false,
  storage_api_url: 'https://localhost:5006',
  admin_api_url: 'https://localhost:5002',
  user_api_url: 'https://localhost:5004',
  authority: 'https://localhost:5000',
  profile_app_url: 'http://localhost:4300',
  client_id: 'angular_admin_dashboard',
  redirect_uri: 'http://localhost:4200/auth-callback',
  post_logout_redirect_uri: 'http://localhost:4200/',
  scope: 'AUTH_SERVER ADMIN_API USER_API openid profile',
  silent_redirect_uri: 'http://localhost:4200/silent-renew.html'
};


/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.
