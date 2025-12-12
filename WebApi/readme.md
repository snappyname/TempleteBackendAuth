## Base Template Project with JWT registration

---

### Database

* Using SQLite (on appsettings.development.json)
* Using SQLite (on program.cs)

Migrations project - Dal

Migrations Link 
` dotnet ef migrations add InitialCreate -p Dal -s WebApi`

---
### Architecture

* Using controllers
* Using JWT auth on controllers with token validation (+ Expiration time)
* Controllers should implement BaseController. Its helps to get User and UserId in controller 

---
### Endpoints

---
### POST /users/register

Input [LoginModel]

`{"email": "test1@test.test","password": "Pass123!"}`

Output [TokensModel]

`{"jwtToken": "", "refreshToken": "" }`

---
### POST /users/login

Input [LoginModel]

`{"email": "test1@test.test","password": "Pass123!"}`

Output [TokensModel]

`{"jwtToken": "", "refreshToken": "" }`

---
### POST /users/refreshToken

Input [string]

`{"here_can_ba_token"}`

Output [TokensModel]

`{"jwtToken": "", "refreshToken": "" }`

---
### GET /users/me

Output [User]

`{"id": "userId", "userName": "...", "normalizedUserName": "...", "email": "...", "normalizedEmail": "...", "emailConfirmed": ...,}`

--- 

### TODO 

* Add refresh token model

---