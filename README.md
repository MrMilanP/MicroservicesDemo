# MicroservicesDemo

## Pregled
Ovo je demo aplikacija kreirana kako bih osvežio znanje o mikroservisima i integraciji između njih. Projekat implementira osnovnu funkcionalnost kreiranja i prikaza korisnika pomoću `UserService` mikroservisa. Aplikacija koristi .NET Core 8, Entity Framework Core za rad sa bazom podataka i ASP.NET Core MVC za frontend deo. Projekat nije namenjen za potpunu produkcijsku funkcionalnost, već kao referentna implementacija za demonstraciju mikroservisne arhitekture.

## Funkcionalnosti
- **Kreiranje korisnika**: Registracija novih korisnika pomoću `UserService` mikroservisa.
- **Prijava korisnika**: Validacija korisničkih kredencijala putem `AuthController` u `UserService`.
- **Migracija baze podataka**: Automatski kreira `User` tabelu u bazi `UserDB` pomoću migracija.
- **Prikaz korisnika**: Dohvatanje liste korisnika direktno putem API poziva na `UserService`.

## API krajnje tačke
- **POST** `/api/users` - Kreira novog korisnika u sistemu.
- **GET** `/api/users` - Dohvata listu korisnika iz baze.
- **POST** `/api/auth/login` - Prijavljuje korisnika i generiše JWT token.

## Tehnologije korišćene
- **.NET Core 8**
- **ASP.NET Core MVC**
- **Entity Framework Core**
- **SQL Server** kao baza podataka

## Postavljanje i pokretanje projekta
1. Kloniraj repozitorijum:
    ```bash
    git clone https://github.com/MrMilanP/MicroservicesDemo
    cd MicroservicesDemo
    ```

2. Otvori rešenje u Visual Studio ili Visual Studio Code.

3. Ažuriraj `appsettings.json` fajl unutar `UserService` mikroservisa sa informacijama o SQL Server bazi podataka (ako je potrebno):

    ```json
    "ConnectionStrings": {
      "UserDatabase": "Server=.;Database=UserDB;Trusted_Connection=True;"
    }
    ```

4. Izvrši migraciju baze podataka:
    ```bash
    Add-Migration InitialCreate -Project UserService -StartupProject MicroservicesDemo
    Update-Database -Project UserService -StartupProject MicroservicesDemo
    ```

5. Pokreni `MicroservicesDemo` projekat kao glavnu aplikaciju (setuj kao `Startup Project`).

6. Pristupi aplikaciji na [http://localhost:7033](http://localhost:7033).

## Detalji o strukturi
- **UserService**: Mikroservis zadužen za rukovanje korisnicima i komunikaciju sa `UserDB` bazom podataka.
- **MicroservicesDemo**: Glavna aplikacija koja koristi `UserService` mikroservis za registraciju i prikaz korisnika.

## Primeri API poziva
- **Kreiranje korisnika**:
    ```bash
    curl -X POST https://localhost:7033/api/users -H 'Content-Type: application/json' -d '{ "email": "test@example.com", "password": "Test123!" }'
    ```

- **Prijava korisnika**:
    ```bash
    curl -X POST https://localhost:7033/api/auth/login -H 'Content-Type: application/json' -d '{ "email": "test@example.com", "password": "Test123!" }'
    ```

### Dodatne izmene

- **Preimenovan `UserService` u `UserMicroservice`**:
  - Projekat je sada pod imenom `UserMicroservice`, i svi relevantni delovi koda su ažurirani.

- **Dodati `IUserService` interfejs i `UserService` implementacija**:
  - `IUserService` definiše metode za rad sa korisnicima (`GetAllUsersAsync`, `AddUserAsync`, `UpdateUserAsync`, `DeleteUserAsync`).
  - `UserService` implementira `IUserService` i koristi `UserRepository` za pristup bazi.

- **Dodati `IUserRepository` interfejs i `UserRepository` implementacija**:
  - `IUserRepository` definiše metode za rad sa bazom podataka (`GetAllAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync`).
  - `UserRepository` koristi `UserDbContext` za CRUD operacije nad bazom podataka.

- **Izmenjen `UserController`**:
  - Izbačen direktan pristup `UserDbContext` i zamenjen inddžektovanjem `IUserService`.
  - Dodata poslovna logika kroz `IUserService`, što omogućava bolju podelu odgovornosti i lakše održavanje.
  - Stare metode su komentarisane i zamenjene novim implementacijama:
    - `GetUsers` je zamenjen sa `GetAllUsers` koji koristi `IUserService`.
    - `CreateUser` je zamenjen sa `AddUser` koji koristi asinhroni poziv ka `IUserService`.
    - Dodate metode `UpdateUser` i `DeleteUser` koje koriste `IUserService` za ažuriranje i brisanje korisnika.

Sve izmene su rađene sa ciljem poboljšanja strukture aplikacije i bolje organizacije koda, uz odvajanje poslovne logike u servise i repozitorijume.


### Dodatak 2 - Refaktorisanje i Centralizacija JWT Konfiguracije

U ovom dodatku opisane su ključne izmene i refaktorisanje koda između `MicroservicesDemo` i `UserMicroservice` projekta kako bi se omogućila bolja organizacija i deljenje konfiguracije između servisa. Sledeće promene su implementirane:

- **Refaktorisanje `AuthController`-a**
  - U `AuthController`-u `UserMicroservice`-a uklonjena je zavisnost od `UserDbContext` i `IConfiguration`.
  - Implementiran je `IUserService` kao apstrakcioni sloj za komunikaciju sa bazom, čime je postignuto bolje razdvajanje odgovornosti.
  - `Login` metoda sada koristi `IUserService` umesto direktnog `DbContext` pristupa. To omogućava lakšu izmenu logike i bolju testabilnost.

- **Centralizacija `JwtSettings`**
  - `JwtSettings` klasa premeštena je u novi zajednički projekat `MicroservicesShared` kako bi oba servisa (`MicroservicesDemo` i `UserMicroservice`) koristila istu konfiguraciju.
  - `JwtSettings` je sada registrovan kao `Singleton` servis i može se koristiti u oba projekta, bez potrebe za dupliranjem konfiguracije.
  - To obezbeđuje doslednost JWT vrednosti (`Issuer`, `Audience`, `Key`) kroz više mikroservisa.

- **Implementacija `SigningCredentials`**
  - `SigningCredentials` je kreiran na osnovu `SymmetricSecurityKey` vrednosti (`jwtSettings.Key`) i registrovan kao `Singleton`.
  - Umesto kreiranja `SigningCredentials` unutar `AuthController`-a, koristi se injektovana vrednost, što omogućava centralizaciju kreiranja JWT tokena.

- **Konfiguracija `Program.cs` u `MicroservicesDemo`**
  - `JwtSettings` se validira u `Program.cs` u `MicroservicesDemo` i koristi se u `TokenValidationParameters`.
  - Kreiran je `SymmetricSecurityKey` na osnovu `jwtSettings.Key`, a `SigningCredentials` se koristi za konfiguraciju JWT Bearer autentifikacije.
  - Ako se `UserMicroservice` pokrene nezavisno, koristi sopstvene `JwtSettings` vrednosti definisane u `MicroservicesShared` projektu.

- **Asinhrono Rukovanje Podacima u `AuthController`**
  - `Login` metoda je refaktorisana da koristi `async/await` za asinhrono dohvaćanje korisnika iz baze putem `IUserService`.
  - To omogućava bolje performanse i efikasnije upravljanje resursima tokom rada sa bazom.

Ovim refaktoringom postignuta je bolja modularnost, doslednost i lakša održivost `MicroservicesDemo` i `UserMicroservice` projekata.


### Dodatak 3 - Implementacija Prilagođenog Swagger UI-a

Implementirana je prilagođena Swagger UI konfiguracija u `UserMicroservice` projektu radi bolje interakcije sa JWT autentifikacijom i prilagođavanja korisničkog interfejsa.

- **Prilagođeni `wwwroot/swagger` direktorijum:**
  - Svi prilagođeni resursi za Swagger UI (kao što su `index.html`, `swagger-ui.css`, i `custom-swagger.js`) nalaze se u `wwwroot/swagger` direktorijumu.
  - Ovi resursi omogućavaju automatsko postavljanje JWT tokena nakon prijave (`Login`), kao i bolju kontrolu nad izgledom Swagger UI-a.

- **Prilagođeni `index.html`:**
  - `index.html` je prilagođen i hostovan unutar `wwwroot/swagger` direktorijuma.
  - Koristi prilagođeni `JavaScript` (`custom-swagger.js`) za automatsko rukovanje `JWT` tokenima i interakciju sa Swagger-om.

- **Automatsko postavljanje JWT tokena:**
  - Implementiran `custom-swagger.js` koji automatski prepoznaje `JWT` token iz `Login` odgovora.
  - Token se smešta u `localStorage` i automatski koristi za sve buduće pozive unutar Swagger UI-a.

- **Swagger konfiguracija u `Program.cs`:**
  - Prilagođena `SwaggerEndpoint` putanja (`/swagger/index.html`) zamenjuje podrazumevanu putanju (`/swagger`) kako bi koristila prilagođeni interfejs.

Ove izmene omogućavaju lakšu interakciju sa API-jem kroz Swagger UI i smanjuju potrebu za manuelnim unosom `JWT` tokena nakon svake prijave. Takođe, prilagođeni `index.html` daje veću kontrolu nad izgledom i funkcionalnošću Swagger UI-a.


## Napomena
Ovaj projekat je primer mikroservisne arhitekture i nije predviđen za produkcijsku upotrebu bez daljih prilagođavanja i bezbednosnih provera.
