# Admin app

This project was generated with [Angular CLI](https://github.com/angular/angular-cli) version 10.0.4.

## Introduction

The profile app is angular webapp. The admin app is responsible for managing client information, api information, api permissions information, and the data that `Auth Server` will add in the token when users log in, manage users and their rights over declared APIs,...

## Technology Stack

- [Angular](https://angular.io/) 
- [Bootstrap](https://ng-bootstrap.github.io/#/home)
- [Ng Zorro](https://ng.ant.design/docs/introduce/en)
- [Oidc-client](https://github.com/IdentityModel/oidc-client-js)

## Screenshot

- Login page
![alt tag](./src/assets/git-images/login-page.png)

- Home page
![alt tag](./src/assets/git-images/home-page.png)

- Client page
![alt tag](./src/assets/git-images/client-page.png)
![alt tag](./src/assets/git-images/edit-client-page.png)

- Api resource page
![alt tag](./src/assets/git-images/api-resource-page.png)
![alt tag](./src/assets/git-images/edit-api-resource-page.png)

- Api scopes page
![alt tag](./src/assets/git-images/api-scope-page.png)
![alt tag](./src/assets/git-images/edit-api-scope-page.png)

- Identity resources page
![alt tag](./src/assets/git-images/identity-resource-page.png)
![alt tag](./src/assets/git-images/edit-identity-resource-page.png)

- Users page
![alt tag](./src/assets/git-images/user-page.png)
![alt tag](./src/assets/git-images/edit-user-page.png)

- Permissons page
![alt tag](./src/assets/git-images/permission-page.png)
![alt tag](./src/assets/git-images/edit-permission-page.png)

## Structure of the app 

```
● src
+---● app
|   +--● access-denied
|   |--access-denied.component.ts|.html|.scss
|   +--● auth-callback
|   |--auth-callback.component.ts|.html|.scss
|   +--● layout
|   |  +--● api-resource
|   |  |  |--● add-resource
|   |  |  |--add-resource.component.ts|.html|.scss
|   |  |  |--● edit-resource
|   |  |  |--edit-resource.component.ts|.html|.scss
|   |  |  |--● resource-property
|   |  |  |--resource-property.component.ts|.html|.scss
|   |  |  |--● resource-secret
|   |  |  |--resource-secret.component.ts|.html|.scss
|   |  |--api-resource.component.ts|.html|.scss
|   |  +--● api-scope
|   |  |  |--● add-scope
|   |  |  |--add-scope.component.ts|.html|.scss
|   |  |  |--● edit-scope
|   |  |  |--edit-scope.component.ts|.html|.scss
|   |  |  |--● scope-property
|   |  |  |--scope-property.component.ts|.html|.scss
|   |  |--api-scope.component.ts|.html|.scss
|   |  +--● client
|   |  |  |--● add-client
|   |  |  |--add-client.component.ts|.html|.scss
|   |  |  |--● client-claim
|   |  |  |--client-claim.component.ts|.html|.scss
|   |  |  |--● client-property
|   |  |  |--client-property.component.ts|.html|.scss
|   |  |  |--● client-secret
|   |  |  |--client-secret.component.ts|.html|.scss
|   |  |  |--● edit-client
|   |  |  |--edit-client.component.ts|.html|.scss
|   |  |--client.component.ts|.html|.scss
|   |  +--● home
|   |  |--home.component.ts|.html|.scss
|   |  +--● identity-resource
|   |  |  |--● add-resource
|   |  |  |--add-resource.component.ts|.html|.scss
|   |  |  |--● edit-resource
|   |  |  |--edit-resource.component.ts|.html|.scss
|   |  |  |--● identity-property
|   |  |  |--identity-property.component.ts|.html|.scss
|   |  |--identity-resource.component.ts|.html|.scss
|   |  +--● permission
|   |  |  |--● edit-permission
|   |  |  |--edit-permission.component.ts|.html|.scs
|   |  |--permission.component.ts|.html|.scss
|   |  +--● user
|   |  |  |--● add-user
|   |  |  |--add-user.component.ts|.html|.scss
|   |  |  |--● edit-user
|   |  |  |--edit-user.component.ts|.html|.scs
|   |  |--user.component.ts|.html|.scss
|   |--layout.component.ts|.html|.scss
|   +--● login
|   |--login.component.ts|.html|.scss
|   +--● not-found
|   |--not-found.component.ts|.html|.scss
|   +--● server-error
|   |--server-error.component.ts|.html|.scss
|   +--● shared
|   |  +--● constants
|   |  |--messages.constant.ts
|   |  +--● guard
|   |  |--auth.guard.ts
|   |  +--● interceptors
|   |  |--auth.interceptors.ts
|   |  +--● modules
|   |  |--language-translation
|   |  +--● pipes
|   |  |--shared-pipes.module.ts
|   |  +--● servicess
|   |  |--auth|base|utilities|api-resources|api-scopes|clients|identity-resources|permissions|users|.services.ts
|   |--app.component.ts|.html|.scss
|   |--app.module.ts
|
|--index.html (cdn path for bootstrap & fa icons)
```

## Code scaffolding

Run `ng generate component component-name` to generate a new component. You can also use `ng generate directive|pipe|service|class|guard|interface|enum|module`.

## Development server

Run `ng serve --port 4200` for a dev server. Navigate to `http://localhost:4200/`. The app will automatically reload if you change any of the source files.

## Build

Run `ng build` to build the project. The build artifacts will be stored in the `dist/` directory. Use the `--prod` flag for a production build.

## Running unit tests

Run `ng test` to execute the unit tests via [Karma](https://karma-runner.github.io).

## Running end-to-end tests

Run `ng e2e` to execute the end-to-end tests via [Protractor](http://www.protractortest.org/).

## Further help

To get more help on the Angular CLI use `ng help` or go check out the [Angular CLI README](https://github.com/angular/angular-cli/blob/master/README.md).