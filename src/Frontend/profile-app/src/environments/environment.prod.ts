// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environment = {
  production: false,
  storage_api_url: 'http://localhost:5007',
  apiUrl: 'http://localhost:5005',
  authority: 'http://localhost:5001',
  client_id: 'angular_user_profile',
  redirect_uri: 'http://localhost:4300/auth-callback',
  post_logout_redirect_uri: 'http://localhost:4300/',
  scope: 'USER_API AUTH_SERVER openid profile',
  silent_redirect_uri: 'http://localhost:4300/silent-refresh.html'
};


/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.
