# ToDo-Liste

## Phase 1 – Modelle

- [✅] Document.cs überarbeiten
- [✅] DocumentVersion.cs erstellen
- [✅] User.cs anpassen
- [✅] DocumentAccess.cs bereinigen {Korrektur: ID gelöscht}
- [✅] Role enum erstellen {Korrektur: Rolle ist jetzt getrennte Klass}

## Phase 2 – DbContext und Beziehungen

- [✅] AppDbContext erweitern
- [✅] Beziehungen in OnModelCreating konfigurieren
- [✅] Constraints setzen

## Phase 3 – Datenbank

- [✅] Alte Migrationen löschen
- [✅] Neue Migration erstellen
- [✅] Datenbank updaten
- [✅] Seed-User anlegen

## Phase 4 – Basis-Endpunkte

- [✅] POST /api/document
- [✅] GET /api/document?username=...
- [✅] GET /api/document?docId=...&username=...

## Phase 5 – Versionen

- [✅] POST /api/document/versions
- [ ] GET /api/document/versions
- [ ] GET /api/document/versions/latest
- [ ] GET /api/document/version
- [ ] GET /api/document/download
- [ ] GET /api/document/version/download

## Phase 6 – Zugriffsrechte

- [ ] GET /api/document/access
- [ ] POST /api/document/access
- [ ] PUT /api/document/access
- [ ] DELETE /api/document/access

## Phase 7 – Owner-Transfer

- [ ] PUT /api/document/owner

## Phase 8 – DTOs

- [✅] UploadDocumentRequest anpassen
- [✅] AddDocumentVersionRequest erstellen
- [ ] AddDocumentAccessRequest erstellen
- [ ] ChangeDocumentRoleRequest erstellen
- [ ] TransferOwnerRequest erstellen

## Phase 9 – Hilfsmethoden

- [ ] FindUserByUserName
- [ ] HasAccess
- [ ] IsOwner
- [ ] IsEditorOrOwner
      ...

## Phase 10 – Tests

- [ ] Testfälle durchgehen
- [ ] Fehlercodes prüfen

Legenda:
✅
❌
🔄
⭐
🔥
❗
