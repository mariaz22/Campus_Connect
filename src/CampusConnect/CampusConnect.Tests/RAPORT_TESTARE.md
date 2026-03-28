# Raport de Testare - CampusConnect Unit Tests

**Data:** 31 Ianuarie 2026
**Proiect:** CampusConnect - Aplicație pentru managementul campusului universitar
**Autor:** Echipa de dezvoltare

---

## 1. Obiectivele Testării

### Artefacte testate:
| Controller | Descriere | Nivel |
|------------|-----------|-------|
| **AuthController** | Autentificare și înregistrare utilizatori | Unit |
| **UserController** | Gestionarea profilurilor utilizatorilor | Unit |
| **GradeController** | Gestionarea notelor studenților | Unit |
| **SubjectController** | Gestionarea materiilor | Unit |
| **RoomBookingController** | Rezervări săli de curs | Unit |
| **EventController** | Gestionarea evenimentelor | Unit |
| **BuildingController** | Gestionarea clădirilor din campus | Unit |

### Nivel de testare:
- **Unit Testing** - Testare la nivel de componente izolate
- Fiecare controller este testat independent cu mock-uri pentru servicii

---

## 2. Procesul Testării (Faza SDLC)

Testarea a fost implementată în faza de **Development**, urmând principiile:

| Fază | Descriere |
|------|-----------|
| **Development** | Testele sunt scrise în paralel cu codul |
| **Continuous Integration** | Testele pot fi integrate în pipeline-ul CI/CD |
| **Regression Testing** | La fiecare modificare, testele verifică că funcționalitățile existente nu sunt afectate |

**Când se aplică fiecare metodă:**
- Unit tests → În timpul dezvoltării, la fiecare commit
- Regression tests → Înainte de merge în main branch
- Smoke tests → După deployment în mediul de test

---

## 3. Metodele Testării

### 3.1 Framework-uri și Biblioteci Utilizate

| Tehnologie | Versiune | Scop |
|------------|----------|------|
| **xUnit** | 3.x | Framework principal de testare |
| **Moq** | 4.20.72 | Mocking framework pentru simularea dependențelor |
| **FluentAssertions** | 6.12.0 | Aserțiuni expresive și citibile |
| **Microsoft.AspNetCore.Mvc.Testing** | 9.0.0 | Suport pentru testare ASP.NET Core |
| **Microsoft.EntityFrameworkCore.InMemory** | 9.0.0 | Bază de date în memorie pentru teste |

### 3.2 Metode Aplicate

| Metodă | Descriere | Justificare |
|--------|-----------|-------------|
| **Unit Testing** | Testare izolată a controllerelor | Permite testarea rapidă și independentă a logicii din controllere fără dependențe externe |
| **Mocking** | Simularea serviciilor (IAuthService, IGradeService, etc.) | Izolează componenta testată, testele rulează rapid fără acces la baza de date |
| **In-Memory Database** | Folosit pentru EventController | Testează interacțiunea cu baza de date fără persistență |
| **AAA Pattern** | Arrange-Act-Assert | Structură clară și consistentă pentru toate testele |
| **Boundary Testing** | Testarea limitelor (ex: notă 0-10) | Verifică comportamentul la extreme |
| **Role-Based Testing** | Testarea permisiunilor (User, Professor, Admin) | Asigură securitatea aplicației |

### 3.3 Exemplu de Test (Pattern AAA)

```csharp
[Fact]
public async Task CreateGrade_WithValidRequest_ReturnsOkWithGrade()
{
    // Arrange - Pregătirea datelor și mock-urilor
    var professorId = 1;
    SetupUserContext(professorId, "Professor");
    var request = new CreateGradeRequest { SubjectId = 1, StudentId = 2, Value = 9.5m };
    _mockGradeService.Setup(x => x.CreateGradeAsync(professorId, request))
        .ReturnsAsync(new GradeDto { Id = 1, Value = 9.5m });

    // Act - Executarea acțiunii testate
    var result = await _controller.CreateGrade(request);

    // Assert - Verificarea rezultatului
    var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
    var returnedGrade = okResult.Value.Should().BeOfType<GradeDto>().Subject;
    returnedGrade.Value.Should().Be(9.5m);
}
```

---

## 4. Rezultatele Testării

### 4.1 Sumar Execuție

```
═══════════════════════════════════════════════════════
  Test Run Successful.
  Total tests: 125
       Passed: 125
       Failed: 0
   Total time: 2.00 Seconds
═══════════════════════════════════════════════════════
```

### 4.2 Rezultate per Controller

| Controller | Total Teste | ✅ Passed | ❌ Failed | Rate |
|------------|-------------|-----------|-----------|------|
| AuthController | 14 | 14 | 0 | 100% |
| UserController | 18 | 18 | 0 | 100% |
| GradeController | 17 | 17 | 0 | 100% |
| SubjectController | 12 | 12 | 0 | 100% |
| RoomBookingController | 21 | 21 | 0 | 100% |
| EventController | 32 | 32 | 0 | 100% |
| BuildingController | 11 | 11 | 0 | 100% |
| **TOTAL** | **125** | **125** | **0** | **100%** |

### 4.3 Detalii Teste AuthController (14 teste)

| Test | Scenariu | Status |
|------|----------|--------|
| Register_WithValidRequest_ReturnsOkResult | Înregistrare cu date valide | ✅ |
| Register_WithInvalidEmail_ReturnsBadRequest | Email domain invalid (@gmail.com) | ✅ |
| Register_WithExistingEmail_ReturnsBadRequest | Email deja înregistrat | ✅ |
| Register_WithWeakPassword_ReturnsBadRequest | Parolă sub 8 caractere | ✅ |
| Login_WithValidCredentials_ReturnsOkWithToken | Login corect, returnează JWT | ✅ |
| Login_WithInvalidCredentials_ReturnsUnauthorized | Parolă greșită | ✅ |
| Login_WithUnconfirmedEmail_ReturnsUnauthorized | Email neconfirmat | ✅ |
| Login_WithLockedAccount_ReturnsUnauthorized | Cont blocat | ✅ |
| ConfirmEmail_WithValidToken_ReturnsOk | Token valid | ✅ |
| ConfirmEmail_WithInvalidUserId_ReturnsBadRequest | UserId = 0 | ✅ |
| ConfirmEmail_WithEmptyToken_ReturnsBadRequest | Token gol | ✅ |
| ConfirmEmail_WithExpiredToken_ReturnsBadRequest | Token expirat | ✅ |
| ResendConfirmation_WithValidEmail_ReturnsOk | Email valid | ✅ |
| ResendConfirmation_WithEmptyEmail_ReturnsBadRequest | Email gol | ✅ |

### 4.4 Detalii Teste UserController (18 teste)

| Test | Scenariu | Status |
|------|----------|--------|
| GetUserDetails_WithValidUser_ReturnsOkWithUserProfile | Obținere profil utilizator | ✅ |
| GetUserDetails_WithNoUser_ReturnsUnauthorized | Utilizator neautentificat | ✅ |
| GetUserDetails_WithNonExistentUser_ReturnsNotFound | Utilizator inexistent | ✅ |
| GetPublicUserDetails_WithValidId_ReturnsOkWithUserData | Profil public | ✅ |
| GetPublicUserDetails_WithNonExistentId_ReturnsNotFound | ID inexistent | ✅ |
| UpdateUser_WithValidData_ReturnsOk | Actualizare profil | ✅ |
| UpdateUser_WithNoUser_ReturnsUnauthorized | Neautentificat | ✅ |
| UpdateUser_WithNonExistentUser_ReturnsNotFound | Utilizator inexistent | ✅ |
| DeleteUser_OwnAccount_ReturnsNoContent | Ștergere cont propriu | ✅ |
| DeleteUser_AsAdmin_ReturnsNoContent | Admin șterge alt cont | ✅ |
| DeleteUser_OtherUserWithoutAdmin_ReturnsUnauthorized | User șterge alt cont (interzis) | ✅ |
| DeleteUser_WithNoUser_ReturnsUnauthorized | Neautentificat | ✅ |
| DeleteUser_NonExistentUser_ReturnsNotFound | Utilizator inexistent | ✅ |
| ToggleAdminRole_AsAdmin_ReturnsOkWithNewRole | Toggle rol admin | ✅ |
| ToggleAdminRole_WithNonExistentUser_ReturnsNotFound | Utilizator inexistent | ✅ |
| SearchUsers_WithSearchTerm_ReturnsMatchingUsers | Căutare utilizatori | ✅ |
| SearchUsers_WithEmptySearch_ReturnsAllUsers | Fără termen de căutare | ✅ |
| SearchUsers_WithAdminRole_ReturnsCorrectRole | Verificare rol afișat | ✅ |

### 4.5 Detalii Teste GradeController (17 teste)

| Test | Scenariu | Status |
|------|----------|--------|
| CreateGrade_WithValidRequest_ReturnsOkWithGrade | Creare notă validă | ✅ |
| CreateGrade_WithInvalidSubject_ReturnsBadRequest | Materie inexistentă | ✅ |
| CreateGrade_WithInvalidGradeValue_ReturnsBadRequest | Notă > 10 | ✅ |
| UpdateGrade_WithValidRequest_ReturnsOkWithUpdatedGrade | Actualizare notă | ✅ |
| UpdateGrade_WithNonExistentGrade_ReturnsBadRequest | Notă inexistentă | ✅ |
| DeleteGrade_WithValidId_ReturnsOk | Ștergere notă | ✅ |
| DeleteGrade_WithNonExistentId_ReturnsNotFound | ID inexistent | ✅ |
| GetGrade_WithValidId_ReturnsOkWithGrade | Obținere notă | ✅ |
| GetGrade_WithNonExistentId_ReturnsNotFound | ID inexistent | ✅ |
| GetGradesBySubject_ReturnsOkWithGrades | Liste note per materie | ✅ |
| GetGradesByStudent_AsStudent_OwnGrades_ReturnsOk | Student vede notele proprii | ✅ |
| GetGradesByStudent_AsStudent_OtherStudentGrades_ReturnsForbid | Student NU vede notele altora | ✅ |
| GetGradesByStudent_AsProfessor_AnyStudent_ReturnsOk | Profesor vede orice note | ✅ |
| GetGradesByStudent_AsAdmin_AnyStudent_ReturnsOk | Admin vede orice note | ✅ |
| GetStudentGradesGrouped_ReturnsGroupedGrades | Note grupate per materie | ✅ |
| GetStudentGradesGrouped_StudentNotFound_ReturnsNotFound | Student inexistent | ✅ |
| GetMyGrades_ReturnsCurrentUserGrades | Notele utilizatorului curent | ✅ |

### 4.6 Detalii Teste SubjectController (12 teste)

| Test | Scenariu | Status |
|------|----------|--------|
| CreateSubject_WithValidRequest_ReturnsOkWithSubject | Creare materie | ✅ |
| CreateSubject_WithDuplicateCode_ReturnsBadRequest | Cod duplicat | ✅ |
| UpdateSubject_WithValidRequest_ReturnsOkWithUpdatedSubject | Actualizare materie | ✅ |
| UpdateSubject_WithNonExistentId_ReturnsBadRequest | ID inexistent | ✅ |
| UpdateSubject_WithoutPermission_ReturnsBadRequest | Fără permisiune | ✅ |
| DeleteSubject_WithValidId_ReturnsOk | Ștergere materie | ✅ |
| DeleteSubject_WithNonExistentId_ReturnsNotFound | ID inexistent | ✅ |
| GetSubject_WithValidId_ReturnsOkWithSubject | Obținere materie | ✅ |
| GetSubject_WithNonExistentId_ReturnsNotFound | ID inexistent | ✅ |
| GetMySubjects_ReturnsProfessorSubjects | Materiile profesorului | ✅ |
| GetMySubjects_WhenNoSubjects_ReturnsEmptyList | Fără materii | ✅ |
| GetAllSubjects_ReturnsAllSubjects | Toate materiile | ✅ |

### 4.7 Detalii Teste RoomBookingController (21 teste)

| Test | Scenariu | Status |
|------|----------|--------|
| CreateBookingRequest_WithValidRequest_ReturnsCreatedAtAction | Creare rezervare | ✅ |
| CreateBookingRequest_WithNoUser_ReturnsUnauthorized | Utilizator neautentificat | ✅ |
| CreateBookingRequest_WithConflict_ReturnsBadRequest | Conflict orar | ✅ |
| CreateBookingRequest_WithPastDate_ReturnsBadRequest | Dată în trecut | ✅ |
| GetPendingRequests_ReturnsOkWithPendingRequests | Liste cereri în așteptare | ✅ |
| GetPendingRequests_WhenNoPending_ReturnsEmptyList | Fără cereri | ✅ |
| GetMyRequests_ReturnsUserRequests | Cererile utilizatorului | ✅ |
| GetMyRequests_WithNoUser_ReturnsUnauthorized | Neautentificat | ✅ |
| GetRequestById_WithValidId_ReturnsOk | Obținere cerere | ✅ |
| GetRequestById_WithInvalidId_ReturnsNotFound | ID inexistent | ✅ |
| ApproveRequest_WithValidId_ReturnsOkWithApprovedRequest | Aprobare cerere | ✅ |
| ApproveRequest_WithNoAdmin_ReturnsUnauthorized | Nu e admin | ✅ |
| ApproveRequest_WithAlreadyApproved_ReturnsBadRequest | Deja aprobată | ✅ |
| RejectRequest_WithValidIdAndReason_ReturnsOkWithRejectedRequest | Respingere cu motiv | ✅ |
| RejectRequest_WithNoAdmin_ReturnsUnauthorized | Nu e admin | ✅ |
| DeleteRequest_WithValidId_ReturnsNoContent | Ștergere cerere | ✅ |
| DeleteRequest_WithInvalidId_ReturnsNotFound | ID inexistent | ✅ |
| DeleteRequest_WithNoUser_ReturnsUnauthorized | Neautentificat | ✅ |
| DeleteRequest_WithOtherUsersRequest_ReturnsBadRequest | Cererea altui user | ✅ |

### 4.8 Detalii Teste EventController (32 teste)

| Test | Scenariu | Status |
|------|----------|--------|
| GetUpcomingEvents_ReturnsUpcomingEvents | Evenimente viitoare | ✅ |
| GetUpcomingEvents_WithSearchTerm_ReturnsMatchingEvents | Căutare evenimente | ✅ |
| GetById_WithValidId_ReturnsEvent | Obținere eveniment | ✅ |
| GetById_WithInvalidId_ReturnsNotFound | ID inexistent | ✅ |
| Create_AsAdmin_ReturnsCreatedAtAction | Creare ca admin | ✅ |
| Create_AsProfessor_ReturnsCreatedAtAction | Creare ca profesor | ✅ |
| Create_AsRegularUser_ReturnsUnauthorized | User normal nu poate crea | ✅ |
| Create_WithNoUser_ReturnsUnauthorized | Neautentificat | ✅ |
| Update_AsOrganizer_ReturnsNoContent | Actualizare ca organizator | ✅ |
| Update_AsAdmin_ReturnsNoContent | Actualizare ca admin | ✅ |
| Update_WithNonExistentEvent_ReturnsNotFound | Eveniment inexistent | ✅ |
| Update_ByNonOrganizer_ReturnsUnauthorized | Neorganizator | ✅ |
| Delete_AsOrganizer_ReturnsNoContent | Ștergere ca organizator | ✅ |
| Delete_AsAdmin_ReturnsNoContent | Ștergere ca admin | ✅ |
| Delete_ByNonOrganizer_ReturnsForbid | Interzis non-organizator | ✅ |
| Delete_WithNonExistentEvent_ReturnsNotFound | Eveniment inexistent | ✅ |
| Participate_WithValidEvent_ReturnsOk | Participare la eveniment | ✅ |
| Participate_WhenAlreadyParticipating_ReturnsBadRequest | Deja participant | ✅ |
| Participate_WithNoUser_ReturnsUnauthorized | Neautentificat | ✅ |
| Participate_WithNonExistentEvent_ReturnsNotFound | Eveniment inexistent | ✅ |
| Withdraw_WhenParticipating_ReturnsOk | Retragere din eveniment | ✅ |
| Withdraw_WhenNotParticipating_ReturnsBadRequest | Nu participă | ✅ |
| Withdraw_WithNoUser_ReturnsUnauthorized | Neautentificat | ✅ |
| SaveEvent_WithValidEvent_ReturnsOk | Salvare eveniment | ✅ |
| SaveEvent_WhenAlreadySaved_ReturnsBadRequest | Deja salvat | ✅ |
| SaveEvent_WithNonExistentEvent_ReturnsNotFound | Eveniment inexistent | ✅ |
| UnsaveEvent_WhenSaved_ReturnsOk | Anulare salvare | ✅ |
| UnsaveEvent_WhenNotSaved_ReturnsBadRequest | Nu e salvat | ✅ |
| GetSavedEvents_ReturnsUserSavedEvents | Evenimente salvate | ✅ |
| GetSavedEvents_WithNoUser_ReturnsUnauthorized | Neautentificat | ✅ |
| GetMyParticipatingEvents_ReturnsUserEvents | Evenimente la care participi | ✅ |
| GetMyParticipatingEvents_WithNoUser_ReturnsUnauthorized | Neautentificat | ✅ |

### 4.9 Detalii Teste BuildingController (11 teste)

| Test | Scenariu | Status |
|------|----------|--------|
| GetAllBuildings_ReturnsOkWithBuildings | Obținere toate clădirile | ✅ |
| GetAllBuildings_WhenEmpty_ReturnsEmptyList | Fără clădiri | ✅ |
| GetBuildingById_WithValidId_ReturnsOkWithBuilding | Obținere clădire | ✅ |
| GetBuildingById_WithInvalidId_ReturnsNotFound | ID inexistent | ✅ |
| CreateBuilding_AsAdmin_ReturnsCreatedAtAction | Creare clădire (admin) | ✅ |
| CreateBuilding_WithGeoJsonPolygon_ReturnsCreatedAtAction | Creare cu polygon GeoJSON | ✅ |
| UpdateBuilding_AsAdmin_ReturnsOkWithUpdatedBuilding | Actualizare clădire | ✅ |
| UpdateBuilding_WithNonExistentId_ReturnsNotFound | ID inexistent | ✅ |
| UpdateBuilding_PartialUpdate_ReturnsOk | Actualizare parțială | ✅ |
| DeleteBuilding_AsAdmin_ReturnsNoContent | Ștergere clădire | ✅ |
| DeleteBuilding_WithNonExistentId_ReturnsNotFound | ID inexistent | ✅ |

---

## 5. Observații și Recomandări

### 5.1 Puncte Forte
- ✅ **100% rata de succes** - Toate cele 125 de teste trec
- ✅ **Acoperire scenarii negative** - Testăm atât cazurile de succes cât și cele de eroare
- ✅ **Testare permisiuni** - Verificăm rolurile User, Professor, Admin
- ✅ **Timp rapid de execuție** - ~2 secunde pentru toate testele
- ✅ **Cod curat** - Pattern AAA consistent în toate testele
- ✅ **Acoperire extinsă** - 7 controllere testate complet



## 6. Structura Proiectului de Teste

```
CampusConnect.Tests/
├── Controllers/
│   ├── Auth/
│   │   ├── AuthControllerTests.cs (14 teste)
│   │   └── UserControllerTests.cs (18 teste)
│   ├── Academic/
│   │   ├── GradeControllerTests.cs (17 teste)
│   │   └── SubjectControllerTests.cs (12 teste)
│   ├── Facilities/
│   │   ├── RoomBookingControllerTests.cs (21 teste)
│   │   └── BuildingControllerTests.cs (11 teste)
│   └── Social/
│       └── EventControllerTests.cs (32 teste)
├── CampusConnect.Tests.csproj
└── RAPORT_TESTARE.md
```

---

## 7. Comenzi Utile

```bash
# Rulare toate testele
dotnet test CampusConnect.Tests

# Rulare cu output detaliat
dotnet test CampusConnect.Tests --logger "console;verbosity=detailed"

# Rulare teste specifice unui controller
dotnet test CampusConnect.Tests --filter "FullyQualifiedName~AuthController"
dotnet test CampusConnect.Tests --filter "FullyQualifiedName~EventController"



---
**Versiune:** 2.0
