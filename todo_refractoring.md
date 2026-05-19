## 1. Gemeinsame ServiceResult-Struktur erstellen

- [ ] Datei `Services/ServiceResult.cs` erstellen
- [ ] Generischen Rückgabetyp `ServiceResult<T>` erstellen
- [ ] Erfolgsfall mit `Ok(T value)` abbilden
- [ ] Fehlerfall mit `Fail(string errorMessage, int statusCode)` abbilden
- [ ] Projekt bauen und prüfen

---

## 2. ApiControllerBase erweitern

- [ ] In `ApiControllerBase.cs` Methode `ToActionResult<T>()` hinzufügen
- [ ] `ServiceResult<T>` in HTTP-Response umwandeln
- [ ] Fehler mit passendem StatusCode zurückgeben
- [ ] Erfolgreiche Ergebnisse mit `Ok(result.Value)` zurückgeben
- [ ] Bestehende Methode `TryGetAccessError()` vorerst behalten
- [ ] Projekt bauen und prüfen

---

## 3. UserController verschlanken

- [ ] Datei `Services/UserService.cs` erstellen
- [ ] Methode `GetAllUsers()` in `UserService` auslagern
- [ ] Methode `CreateUser(...)` in `UserService` auslagern
- [ ] Methode `DeleteUser(...)` in `UserService` auslagern
- [ ] `UsersController` so umbauen, dass er nur noch `UserService` aufruft
- [ ] Direkten Zugriff auf `_context` aus `UsersController` entfernen
- [ ] `UserService` in `Program.cs` registrieren
- [ ] Projekt bauen und Swagger-Tests prüfen

---

## 4. DocumentController verschlanken

- [ ] Datei `Services/DocumentService.cs` erstellen
- [ ] Upload-Logik aus `DocumentsController` in `DocumentService` verschieben
- [ ] Datei-Validierung in `DocumentService` verschieben
- [ ] Byte-Array-Erstellung aus `IFormFile` in `DocumentService` verschieben
- [ ] Erstellung von `Document` und erster `DocumentVersion` in `DocumentService` verschieben
- [ ] Erstellung des Owner-Zugriffs in `DocumentService` verschieben
- [ ] Logik für Dokumentliste in `DocumentService` verschieben
- [ ] Logik für einzelnes Dokument in `DocumentService` verschieben
- [ ] Direkten Zugriff auf `_context` aus `DocumentsController` entfernen
- [ ] `DocumentService` in `Program.cs` registrieren
- [ ] Projekt bauen und Swagger-Tests prüfen

---

## 5. AccessController verschlanken

- [ ] Methode `GetAccesses(...)` vollständig in `AccessService` auslagern
- [ ] Methode `AddAccess(...)` vollständig in `AccessService` auslagern
- [ ] Methode `ChangeAccessRole(...)` vollständig in `AccessService` auslagern
- [ ] Methode `DeleteAccess(...)` vollständig in `AccessService` auslagern
- [ ] Methode `TransferOwner(...)` vollständig in `AccessService` auslagern
- [ ] Owner-Prüfungen in `AccessService` zentralisieren
- [ ] Prüfung `Owner` darf nur durch Transfer gesetzt werden in `AccessService` verschieben
- [ ] Prüfung `TargetUser` existiert in `AccessService` verschieben
- [ ] Prüfung `TargetUser` hat Zugriff in `AccessService` verschieben
- [ ] Prüfung `TargetUser` ist bereits Owner in `AccessService` verschieben
- [ ] `SaveChangesAsync()` aus Controller entfernen
- [ ] Direkten Zugriff auf `_context` aus `AccessController` entfernen
- [ ] Controller-Methoden auf `ToActionResult(result)` reduzieren
- [ ] Projekt bauen und Swagger-Tests prüfen

---

## 6. VersionController verschlanken

- [ ] Methode `GetVersions(...)` in `VersionService` auslagern
- [ ] Methode `GetLatestVersion(...)` in `VersionService` auslagern
- [ ] Methode `GetVersion(...)` in `VersionService` auslagern
- [ ] Methode `AddVersion(...)` vollständig in `VersionService` auslagern
- [ ] Download-Logik für neueste Version in `VersionService` auslagern
- [ ] Download-Logik für bestimmte Version in `VersionService` auslagern
- [ ] DTO `FileDownloadResult` erstellen
- [ ] Controller soll bei Downloads nur noch `File(...)` zurückgeben
- [ ] Direkten Zugriff auf `_context` aus `VersionsController` entfernen
- [ ] Projekt bauen und Swagger-Tests prüfen

---

## 7. Common-Klassen prüfen

- [ ] `ErrorMessage.cs` auf doppelte oder unklare Fehlermeldungen prüfen
- [ ] Einheitliche englische Fehlermeldungen verwenden
- [ ] `ApiResponse.cs` auf doppelte Methoden prüfen
- [ ] `ApiResponse.FromServiceResult(...)` ergänzen oder durch `ToActionResult(...)` ersetzen
- [ ] Nicht mehr benötigte Response-Methoden entfernen
- [ ] Projekt bauen und prüfen

---

## 8. Dependency Injection prüfen

- [ ] Alle Services in `Program.cs` registrieren
- [ ] Prüfen, ob Controller nur noch Services injizieren
- [ ] Prüfen, ob nur Services `AppDbContext` injizieren
- [ ] Keine ungenutzten using-Anweisungen mehr in Controllern
- [ ] Projekt bauen und prüfen

---
