# Security Analysis – CampusConnect

## 1. Context

CampusConnect este o aplicație web de tip client–server, formată dintr-un frontend React și un backend ASP.NET Core API.

Aplicația gestionează conturi de utilizatori (student, profesor, administrator), date academice (note, discipline), conținut generat de utilizatori (anunțuri, grupuri), rezervări de săli, documente PDF și un modul de asistență AI.

## 2. Active protejate

- Date personale (PII): nume, email, identificator student
- Date academice: note, discipline, situație școlară
- Token-uri de autentificare JWT
- Secrete de aplicație (JWT Secret, chei API externe)
- Disponibilitatea serviciilor backend

## 3. Abordarea securității în aplicație

Securitatea aplicației a fost tratată ca un aspect esențial încă din faza de proiectare, fiind implementate mecanisme standard și validate pentru protejarea datelor, controlul accesului și prevenirea abuzurilor.

## 4. Măsuri de securitate implementate

### 4.1 Autentificare și gestionarea conturilor

- Autentificarea este realizată folosind **ASP.NET Identity**.
- Parolele sunt stocate sub formă de hash și salt, respectând politici stricte:
  - lungime minimă de 8 caractere
  - literă mare, literă mică și cifră obligatorii
- Conturile sunt blocate temporar după mai multe încercări eșuate de autentificare.
- Accesul este permis doar utilizatorilor cu email confirmat.
- Autentificarea între client și server se face prin **JWT**, cu validarea semnăturii, a emitentului și a duratei de viață.

### 4.2 Autorizare și controlul accesului

- Accesul la endpoint-uri este protejat prin atributul `[Authorize]`.
- Funcționalitățile sensibile sunt restricționate pe bază de roluri (`User`, `Professor`, `Admin`).
- Pentru resursele asociate unui utilizator, este realizată verificarea explicită a proprietarului (ownerId),
  prevenind accesul neautorizat la datele altor utilizatori (IDOR).

### 4.3 Protecția împotriva atacurilor de tip brute-force și DoS

- Endpoint-urile sensibile (ex. autentificare, AI assistant) sunt protejate prin **rate limiting**.
- Numărul de cereri permise într-un interval de timp este limitat, iar depășirea pragului returnează codul HTTP `429`.

### 4.4 Protecția datelor și prevenirea injection

- Accesul la baza de date este realizat prin **Entity Framework Core**, utilizând query-uri parametrizate.
- Datele de intrare sunt validate prin modele și DTO-uri, reducând riscul de SQL Injection.
- Nu sunt utilizate concatenări directe de string-uri pentru interogări SQL.

### 4.5 Gestionarea secretelor

- Secretele aplicației (JWT Secret, chei API) sunt extrase din fișiere de configurare și variabile de mediu.
- Acestea nu sunt expuse în codul sursă sau în log-uri.

### 4.6 Integrarea serviciului AI

- Modulul AI funcționează printr-un serviciu dedicat, izolat de restul aplicației.
- Cererile către serviciul extern sunt limitate și monitorizate.
- Sunt transmise doar datele necesare funcționării corecte a asistentului.

### 4.7 Logging și monitorizare

- Evenimentele importante (autentificare, erori, acces neautorizat) sunt logate.
- Informațiile sensibile, precum parolele sau token-urile, nu sunt incluse în log-uri.

## 5. Testarea securității

Au fost realizate teste manuale pentru verificarea mecanismelor de securitate:

- accesarea endpoint-urilor fără autentificare → `401 Unauthorized`
- accesarea funcțiilor fără rol corespunzător → `403 Forbidden`
- depășirea limitei de cereri → `429 Too Many Requests`
- accesarea resurselor altui utilizator → acces refuzat

## 6. Concluzie

Prin utilizarea mecanismelor standard oferite de platforma .NET și prin implementarea unor controale suplimentare,
aplicația CampusConnect oferă un nivel adecvat de securitate pentru protejarea datelor și funcționalităților sale.

Măsurile implementate reduc semnificativ riscurile de acces neautorizat, abuz și compromitere a datelor.
