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


## Napomena
Ovaj projekat je primer mikroservisne arhitekture i nije predviđen za produkcijsku upotrebu bez daljih prilagođavanja i bezbednosnih provera.
