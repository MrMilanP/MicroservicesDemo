# README

## Pregled
Ovo je demo projekat urađen za 3-4 sata kako bih osvežio znanje o mikroservisima. Omogućava osnovnu funkcionalnost kreiranja korisnika preko mikroservisa i prikaza korisnika direktnim pozivom na API. Projekat takođe uključuje migraciju baze podataka koja kreira potrebne tabele prema modelu korisnika. Projekat nije namenjen za potpunu funkcionalnost, već kao referentna implementacija.

## Funkcionalnosti
- **Kreiranje korisnika**: Kreira korisnika preko mikroservisa UserService.
- **API pozivi**: Omogućava prikaz korisnika direktno putem API poziva.
- **Migracija baze podataka**: Automatski kreira tabelu User u bazi UserDB prilikom pokretanja migracija.

## API krajnje tačke
- **POST** `/api/users` - Kreira novog korisnika u sistemu.
- **GET** `/api/users` - Dohvata listu korisnika iz baze.

## Postavljanje i pokretanje projekta
Kloniraj repozitorijum i otvori ga u svom omiljenom IDE-u:

```bash
git clone https://github.com/MrMilanP/MicroservicesDemo

```


## Migracija baze podataka
Pokreni sledeće komande iz root direktorijuma projekta kako bi izvršio migraciju baze:
```bash
Add-Migration InitialCreate -Project UserService -StartupProject MicroservicesDemo
Update-Database -Project UserService -StartupProject MicroservicesDemo
```

## Pokreni aplikaciju
Obezbedi da API i mikroservisi rade, i pristupi im putem URL-a.

## Detalji o strukturi
UserService: Mikroservis koji se bavi rukovanjem korisnicima i bazom podataka.

MicroservicesDemo: Glavni projekat u kojem se prikazuje rad sa mikroservisima.

Tehnologije korišćene

.NET Core

Entity Framework Core

ASP.NET Core

