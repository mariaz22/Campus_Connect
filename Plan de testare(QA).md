# Plan de Testare (QA) - CampusConnect

## 1. Introducere și Scop

Acest document descrie planul de testare pentru aplicația **CampusConnect**, o platformă academică destinată Universității din București. Scopul testării este validarea funcționalităților principale ale aplicației din perspectiva utilizatorului final, pentru a verifica dacă sistemul răspunde corect cerințelor funcționale și nevoilor definite în etapa de analiză.

Testarea se concentrează pe fluxurile reale de utilizare ale platformei, așa cum sunt acestea prezentate și în demonstrația aplicației.

---

## 2. Tipurile de testare alese

Pentru CampusConnect au fost alese **două tipuri complementare de testare**:

### 2.1. Testare Unit (Unit Testing)

**Testare automată** la nivel de componente individuale ale aplicației, folosind **xUnit** pentru backend (.NET).

**Scopul:** Validarea corectitudinii comportamentului componentelor individuale în izolare, asigurând că fiecare unitate de cod funcționează conform specificațiilor și răspunde corect la diferite scenarii de input.

### 2.2. Testare Funcțională Manuală (End-to-End Manual Testing)

**Testare manuală bazată pe scenarii** (scenario-based testing), la nivel de sistem complet, prin interacțiune directă cu aplicația.

**Scopul:** Validarea fluxurilor complete de utilizare din perspectiva utilizatorului final și verificarea integrării corecte a tuturor componentelor.

**Acoperire:**
- Fluxuri complete user journey (înregistrare → confirmare email → login → utilizare funcționalități)
- Interacțiuni complexe între module
- Experiența utilizatorului (UI/UX)
- Verificarea restricțiilor de acces bazate pe roluri

### Motivația alegerii abordării hibride

- **Unit testing** asigură calitatea codului la nivel de componente și permite refactorizări sigure
- **Testarea manuală** validează fluxurile complexe de utilizare și experiența utilizatorului
- Aplicația este orientată pe **fluxuri complexe de utilizare**, dependente de rolul utilizatorului
- Funcționalitățile sunt strâns legate între ele (autentificare → rol → acces la funcții)
- Abordarea combină **rapiditatea și acuratețea testării automate** cu **validarea completă a experienței utilizatorului**

---

## 3. Procesul de testare

Testarea a fost realizată **în paralel cu dezvoltarea și după finalizarea implementării**, combinând testare automată cu validare finală manuală.

### 3.1. Procesul de testare unit (automatizată)

**Abordare:** Teste scrise în paralel cu dezvoltarea funcționalităților pentru validarea componentelor individuale

1. **Identificarea componentelor critice de testat**
2. **Scriere teste pentru comportamentele esențiale**
3. **Verificare acoperire cod pentru componentele testate**
4. **Refactorizare și corectare bazată pe rezultatele testelor**

**Framework-uri utilizate:**
- **Backend:** xUnit pentru .NET

### 3.2. Procesul de testare manuală (end-to-end)

**Abordare:** End-of-cycle testing pentru validare completă

1. **Identificarea funcționalităților critice ale aplicației**
2. **Definirea scenariilor de utilizare corespunzătoare**
3. **Rularea manuală a scenariilor în aplicație**
4. **Compararea rezultatelor obținute cu rezultatele așteptate**
5. **Notarea și corectarea eventualelor probleme identificate**
6. **Verificare cross-browser și responsiveness**

---

## 4. Mediu de testare

### 4.1. Mediu pentru Unit Testing

- **Backend:** .NET 8, xUnit 
- **Test isolation:** Componente testate în izolare
- **Rulare:** Local development environment
- **Integrare:** Teste rulate înainte de commit-uri majore

### 4.2. Mediu pentru Testare Manuală

- **Environment:** local (development)
- **Platformă:** aplicație web (Chrome, Edge, Firefox)
- **Roluri de testare:** Student, Profesor, Administrator (conturi de test)
- **Date de test:** adrese instituționale @unibuc.ro, @s.unibuc.ro, conturi și date predefinite pentru testare
- **Database:** SQL Server LocalDB cu date mock

---

## 5. Scenarii de testare (Test Cases)

### **Autentificare și Management Utilizatori**

#### TC-01 – Înregistrare cu email instituțional valid
- **Rol:** User
- **Pași:**
  1. Utilizatorul accesează pagina de înregistrare
  2. Introduce un email instituțional valid și o parolă
  3. Trimite formularul
- **Rezultat așteptat:** Contul este creat cu succes, iar rolul este atribuit automat în funcție de domeniul emailului

#### TC-02 – Înregistrare cu email neinstitutional
- **Rol:** User
- **Pași:**
  1. Introduce un email care nu aparține domeniilor acceptate
  2. Încearcă să creeze un cont
- **Rezultat așteptat:** Înregistrarea este respinsă, iar utilizatorul primește un mesaj de eroare

#### TC-03 – Autentificare utilizator
- **Rol:** User
- **Pași:**
  1. Introduce email și parolă valide
  2. Apasă butonul de login
- **Rezultat așteptat:** Utilizatorul este autentificat și redirecționat către platformă

#### TC-04 – Acces la funcționalități în funcție de rol
- **Rol:** Student / Profesor / Administrator
- **Pași:**
  1. Utilizatorul se autentifică
  2. Accesează meniul aplicației
- **Rezultat așteptat:** Sunt afișate doar funcționalitățile corespunzătoare rolului utilizatorului

#### TC-05 – Vizualizare și editare profil
- **Rol:** User
- **Pași:**
  1. Accesează pagina de profil
  2. Modifică datele personale
  3. Salvează modificările
- **Rezultat așteptat:** Datele sunt actualizate și persistă la reautentificare

#### TC-06 – Gestionare utilizatori
- **Rol:** Administrator
- **Pași:**
  1. Caută și selectează un utilizator
  2. Atribuie rol de administrator
- **Rezultat așteptat:** Drepturile utilizatorului sunt actualizate corect

---

### **Evenimente**

#### TC-07 – Creare eveniment (Professor/Admin)
- **Rol:** Professor / Administrator
- **Pași:**
  1. Utilizatorul accesează secțiunea Evenimente
  2. Apasă butonul "Creare eveniment nou"
  3. Completează detaliile (titlu, descriere, dată, locație, categorie)
  4. Salvează evenimentul
- **Rezultat așteptat:** Evenimentul este creat și apare în lista evenimentelor viitoare

#### TC-08 – RSVP la eveniment
- **Rol:** User
- **Pași:**
  1. Utilizatorul vizualizează lista evenimentelor
  2. Selectează un eveniment
  3. Apasă butonul "Participă/RSVP"
- **Rezultat așteptat:** Utilizatorul este înregistrat ca participant, contorul de participanți crește

#### TC-09 – Retragere participare eveniment
- **Rol:** User
- **Pași:**
  1. Utilizatorul accesează un eveniment la care participă
  2. Apasă butonul "Renunță la participare"
- **Rezultat așteptat:** Utilizatorul este eliminat din lista participanților

#### TC-10 – Salvare eveniment
- **Rol:** User
- **Pași:**
  1. Utilizatorul vizualizează un eveniment
  2. Apasă iconița "Save/Bookmark"
- **Rezultat așteptat:** Evenimentul apare în lista "Evenimente salvate"

#### TC-11 – Editare eveniment (Creator/Admin)
- **Rol:** Professor / Administrator
- **Pași:**
  1. Utilizatorul accesează un eveniment creat de el
  2. Apasă "Editează"
  3. Modifică detaliile
  4. Salvează modificările
- **Rezultat așteptat:** Evenimentul este actualizat cu noile informații

#### TC-12 – Ștergere eveniment (Creator/Admin)
- **Rol:** Professor / Administrator
- **Pași:**
  1. Utilizatorul selectează un eveniment
  2. Apasă "Șterge"
  3. Confirmă ștergerea
- **Rezultat așteptat:** Evenimentul este eliminat permanent

#### TC-13 – Filtrare evenimente după categorie
- **Rol:** User
- **Pași:**
  1. Utilizatorul accesează secțiunea Evenimente
  2. Selectează o categorie (Academic, Social, Sports)
- **Rezultat așteptat:** Sunt afișate doar evenimentele din categoria selectată

#### TC-14 – Căutare evenimente
- **Rol:** User
- **Pași:**
  1. Utilizatorul accesează secțiunea Evenimente
  2. Introduce un termen de căutare în search box
- **Rezultat așteptat:** Sunt afișate doar evenimentele care conțin termenul căutat

---

### **Grupuri de Studiu**

#### TC-15 – Creare grup (Professor)
- **Rol:** Professor
- **Pași:**
  1. Profesorul accesează secțiunea Grupuri
  2. Apasă "Creare grup nou"
  3. Completează nume, materie, descriere
  4. Salvează grupul
- **Rezultat așteptat:** Grupul este creat, profesorul devine proprietar

#### TC-16 – Înscriere în grup
- **Rol:** User
- **Pași:**
  1. Utilizatorul vizualizează grupurile disponibile
  2. Selectează un grup
  3. Apasă "Join/Înscrie-te"
- **Rezultat așteptat:** Utilizatorul devine membru al grupului

#### TC-17 – Părăsire grup
- **Rol:** User
- **Pași:**
  1. Utilizatorul accesează un grup din care face parte
  2. Apasă "Leave/Părăsește grupul"
- **Rezultat așteptat:** Utilizatorul este eliminat din membrii grupului

#### TC-18 – Creare task în grup (Professor)
- **Rol:** Professor
- **Pași:**
  1. Profesorul accesează un grup creat de el
  2. Navighează la secțiunea Tasks
  3. Apasă "Adaugă task nou"
  4. Completează titlu, descriere, deadline
  5. Salvează taskul
- **Rezultat așteptat:** Taskul este vizibil tuturor membrilor grupului

#### TC-19 – Salvare task personal
- **Rol:** User
- **Pași:**
  1. Utilizatorul vizualizează taskurile unui grup
  2. Selectează un task
  3. Apasă "Save to my tasks"
- **Rezultat așteptat:** Taskul apare în lista personală de taskuri

#### TC-20 – Marcare task ca finalizat
- **Rol:** User
- **Pași:**
  1. Utilizatorul accesează taskurile sale salvate
  2. Selectează un task
  3. Apasă "Mark as complete"
- **Rezultat așteptat:** Taskul este marcat ca finalizat, achievement poate fi deblocat

#### TC-21 – Upload material de curs în grup (Professor)
- **Rol:** Professor
- **Pași:**
  1. Profesorul accesează un grup
  2. Navighează la secțiunea Materials
  3. Apasă "Upload material"
  4. Selectează fișierul și adaugă titlu
  5. Confirmă uploadul
- **Rezultat așteptat:** Materialul este disponibil membrilor grupului pentru download

#### TC-22 – Forward anunț către grup (Professor)
- **Rol:** Professor
- **Pași:**
  1. Profesorul vizualizează un anunț
  2. Apasă "Forward to group"
  3. Selectează grupul țintă
  4. Confirmă
- **Rezultat așteptat:** Anunțul apare în feed-ul grupului selectat

#### TC-23 – Ștergere grup (Professor/Admin)
- **Rol:** Professor / Administrator
- **Pași:**
  1. Utilizatorul selectează un grup
  2. Apasă "Delete group"
  3. Confirmă ștergerea
- **Rezultat așteptat:** Grupul și toate datele asociate sunt șterse

---

### **Subscription System**

#### TC-24 – Abonare la categorie anunțuri
- **Rol:** User
- **Pași:**
  1. Utilizatorul accesează setările de notificări
  2. Selectează o categorie (Academic, Sports, etc.)
  3. Apasă "Subscribe"
- **Rezultat așteptat:** Utilizatorul primește notificări pentru anunțurile din acea categorie

#### TC-25 – Dezabonare de la categorie
- **Rol:** User
- **Pași:**
  1. Utilizatorul accesează lista de subscripții
  2. Selectează o categorie abonată
  3. Apasă "Unsubscribe"
- **Rezultat așteptat:** Utilizatorul nu mai primește notificări pentru acea categorie

#### TC-26 – Vizualizare categorii abonat
- **Rol:** User
- **Pași:**
  1. Utilizatorul accesează secțiunea Subscripții
- **Rezultat așteptat:** Sunt afișate toate categoriile la care utilizatorul este abonat

---

### **Notificări**

#### TC-27 – Vizualizare notificări
- **Rol:** User
- **Pași:**
  1. Utilizatorul apasă iconița de notificări
- **Rezultat așteptat:** Sunt afișate toate notificările necitite și citite recent

#### TC-28 – Marcare notificare ca citită
- **Rol:** User
- **Pași:**
  1. Utilizatorul vizualizează lista de notificări
  2. Selectează o notificare
  3. Apasă pe notificare sau butonul "Mark as read"
- **Rezultat așteptat:** Notificarea este marcată ca citită, contorul de notificări scade

#### TC-29 – Marcare toate notificările ca citite
- **Rol:** User
- **Pași:**
  1. Utilizatorul accesează notificările
  2. Apasă "Mark all as read"
- **Rezultat așteptat:** Toate notificările sunt marcate ca citite

---

### **Achievements și Gamification**

#### TC-30 – Vizualizare achievements proprii
- **Rol:** User
- **Pași:**
  1. Utilizatorul accesează profilul propriu
  2. Navighează la secțiunea Achievements
- **Rezultat așteptat:** Sunt afișate toate achievements-urile deblocate și cele disponibile

#### TC-31 – Deblocare achievement automat
- **Rol:** User
- **Pași:**
  1. Utilizatorul participă la primul eveniment (sau îndeplinește altă condiție)
- **Rezultat așteptat:** Achievement-ul corespunzător este deblocat automat, notificare primită

#### TC-32 – Vizualizare achievements alți utilizatori
- **Rol:** User
- **Pași:**
  1. Utilizatorul accesează profilul altui utilizator
  2. Vizualizează secțiunea Achievements
- **Rezultat așteptat:** Sunt afișate achievements-urile publice ale utilizatorului

#### TC-33 – Creare achievement nou (Admin)
- **Rol:** Administrator
- **Pași:**
  1. Administratorul accesează panoul de achievements
  2. Apasă "Create new achievement"
  3. Completează titlu, descriere, criterii, icon
  4. Salvează
- **Rezultat așteptat:** Achievement-ul nou este disponibil pentru unlock

---

### **Activity Logging**

#### TC-34 – Vizualizare activități recente
- **Rol:** User
- **Pași:**
  1. Utilizatorul accesează dashboard-ul sau secțiunea Activity
- **Rezultat așteptat:** Sunt afișate ultimele 3-5 activități ale utilizatorului

#### TC-35 – Vizualizare istoric complet activități
- **Rol:** User
- **Pași:**
  1. Utilizatorul accesează secțiunea "All activities"
- **Rezultat așteptat:** Este afișat istoricul complet cu paginare

---

### **Room Booking System**

#### TC-36 – Creare cerere rezervare sală
- **Rol:** User
- **Pași:**
  1. Utilizatorul selectează o sală disponibilă
  2. Alege interval orar
  3. Completează scopul rezervării
  4. Trimite cererea
- **Rezultat așteptat:** Cererea este creată cu status "Pending", admin primește notificare

#### TC-37 – Aprobare cerere rezervare (Admin)
- **Rol:** Administrator
- **Pași:**
  1. Administratorul accesează cererile pending
  2. Selectează o cerere
  3. Apasă "Approve"
- **Rezultat așteptat:** Cererea devine "Approved", sala este rezervată, utilizatorul primește notificare

#### TC-38 – Respingere cerere rezervare (Admin)
- **Rol:** Administrator
- **Pași:**
  1. Administratorul accesează cererile pending
  2. Selectează o cerere
  3. Apasă "Reject"
  4. Introduce motiv respingere
- **Rezultat așteptat:** Cererea devine "Rejected", utilizatorul primește notificare cu motivul

#### TC-39 – Vizualizare cereri proprii
- **Rol:** User
- **Pași:**
  1. Utilizatorul accesează "My booking requests"
- **Rezultat așteptat:** Sunt afișate toate cererile cu statusurile lor (Pending, Approved, Rejected)

#### TC-40 – Anulare cerere rezervare
- **Rol:** User
- **Pași:**
  1. Utilizatorul accesează cererea sa
  2. Apasă "Cancel request"
- **Rezultat așteptat:** Cererea este ștearsă dacă este Pending, sau sala devine disponibilă dacă era Approved

---

### **Schedule Management**

#### TC-41 – Creare programare sală (Professor/Admin)
- **Rol:** Professor / Administrator
- **Pași:**
  1. Utilizatorul selectează o sală
  2. Apasă "Add schedule"
  3. Completează materie, zi, oră start, oră end
  4. Salvează
- **Rezultat așteptat:** Programarea apare în orarul sălii, sala devine ocupată în acel interval

#### TC-42 – Editare programare (Professor/Admin)
- **Rol:** Professor / Administrator
- **Pași:**
  1. Utilizatorul selectează o programare existentă
  2. Modifică detaliile
  3. Salvează
- **Rezultat așteptat:** Programarea este actualizată

#### TC-43 – Ștergere programare (Professor/Admin)
- **Rol:** Professor / Administrator
- **Pași:**
  1. Utilizatorul selectează o programare
  2. Apasă "Delete"
  3. Confirmă
- **Rezultat așteptat:** Programarea este ștearsă, intervalul devine disponibil

#### TC-44 – Vizualizare orar săptămânal sală
- **Rol:** User
- **Pași:**
  1. Utilizatorul selectează o sală
  2. Vizualizează schedule-ul săptămânal
- **Rezultat așteptat:** Este afișat un calendar cu toate programările săptămânii

---

### **Bibliotecă Digitală - Funcții Avansate**

#### TC-45 – Creare folder bibliotecă (Professor/Admin)
- **Rol:** Professor / Administrator
- **Pași:**
  1. Utilizatorul accesează biblioteca
  2. Apasă "Create folder"
  3. Introduce numele folderului
  4. Salvează
- **Rezultat așteptat:** Folderul nou apare în listă

#### TC-46 – Ștergere folder (Professor/Admin)
- **Rol:** Professor / Administrator
- **Pași:**
  1. Utilizatorul selectează un folder
  2. Apasă "Delete folder"
  3. Confirmă ștergerea
- **Rezultat așteptat:** Folderul și toate fișierele din el sunt șterse

#### TC-47 – Adăugare link extern bibliotecă (Professor/Admin)
- **Rol:** Professor / Administrator
- **Pași:**
  1. Utilizatorul selectează un folder
  2. Apasă "Add link"
  3. Completează titlu și URL
  4. Salvează
- **Rezultat așteptat:** Link-ul apare în folder și poate fi accesat

#### TC-48 – Ștergere item bibliotecă (Professor/Admin)
- **Rol:** Professor / Administrator
- **Pași:**
  1. Utilizatorul selectează un fișier sau link
  2. Apasă "Delete"
  3. Confirmă
- **Rezultat așteptat:** Item-ul este șters permanent

---

### **Management Note - Funcții Complete**

#### TC-49 – Creare notă (Professor)
- **Rol:** Professor
- **Pași:**
  1. Profesorul accesează secțiunea Grades
  2. Selectează cursul și studentul
  3. Introduce nota, data, tip evaluare
  4. Salvează
- **Rezultat așteptat:** Nota apare în catalogul studentului

#### TC-50 – Editare notă (Professor/Admin)
- **Rol:** Professor / Administrator
- **Pași:**
  1. Utilizatorul selectează o notă existentă
  2. Modifică valoarea
  3. Salvează
- **Rezultat așteptat:** Nota este actualizată, modificarea este logată

#### TC-51 – Ștergere notă (Professor/Admin)
- **Rol:** Professor / Administrator
- **Pași:**
  1. Utilizatorul selectează o notă
  2. Apasă "Delete"
  3. Confirmă
- **Rezultat așteptat:** Nota este ștearsă din catalog

#### TC-52 – Vizualizare note per materie (Professor)
- **Rol:** Professor
- **Pași:**
  1. Profesorul selectează o materie pe care o predă
  2. Vizualizează toate notele studenților
- **Rezultat așteptat:** Este afișată lista cu note pentru toți studenții înscriși la materie

#### TC-53 – Validare permisiuni notare
- **Rol:** Professor
- **Pași:**
  1. Profesorul încearcă să noteze un student la o materie pe care NU o predă
- **Rezultat așteptat:** Sistemul respinge operațiunea cu mesaj de eroare

---

### **Email Confirmation System**

#### TC-54 – Retrimite email confirmare
- **Rol:** User (unconfirmed)
- **Pași:**
  1. Utilizatorul încearcă să se autentifice dar emailul nu este confirmat
  2. Apasă "Resend confirmation email"
- **Rezultat așteptat:** Un nou email de confirmare este trimis

#### TC-55 – Link confirmare expirat
- **Rol:** User
- **Pași:**
  1. Utilizatorul accesează un link de confirmare mai vechi de 24 ore
- **Rezultat așteptat:** Mesaj de eroare că linkul a expirat, opțiune de retrimite

---

### **Management Facilități - Funcții Complete**

#### TC-56 – Creare clădire nouă (Admin)
- **Rol:** Administrator
- **Pași:**
  1. Administratorul accesează Campus Map
  2. Apasă "Add building"
  3. Completează nume, adresă, coordonate
  4. Salvează
- **Rezultat așteptat:** Clădirea apare pe hartă

#### TC-57 – Editare detalii clădire (Admin)
- **Rol:** Administrator
- **Pași:**
  1. Administratorul selectează o clădire
  2. Modifică detaliile
  3. Salvează
- **Rezultat așteptat:** Informațiile clădirii sunt actualizate

#### TC-58 – Ștergere clădire (Admin)
- **Rol:** Administrator
- **Pași:**
  1. Administratorul selectează o clădire
  2. Apasă "Delete"
  3. Confirmă
- **Rezultat așteptat:** Clădirea și toate sălile asociate sunt șterse

#### TC-59 – Creare sală nouă (Admin)
- **Rol:** Administrator
- **Pași:**
  1. Administratorul selectează o clădire
  2. Apasă "Add room"
  3. Completează nume, capacitate, tip
  4. Salvează
- **Rezultat așteptat:** Sala apare în lista sălilor clădirii

#### TC-60 – Editare detalii sală (Admin)
- **Rol:** Administrator
- **Pași:**
  1. Administratorul selectează o sală
  2. Modifică capacitatea sau tipul
  3. Salvează
- **Rezultat așteptat:** Detaliile sălii sunt actualizate

#### TC-61 – Ștergere sală (Admin)
- **Rol:** Administrator
- **Pași:**
  1. Administratorul selectează o sală
  2. Apasă "Delete"
  3. Confirmă
- **Rezultat așteptat:** Sala este ștearsă, programările asociate sunt anulate

---

### **AI Assistant**

#### TC-62 – Inițiere conversație cu AI
- **Rol:** User
- **Pași:**
  1. Utilizatorul apasă iconița chatbot
  2. Introduce o întrebare
  3. Trimite mesajul
- **Rezultat așteptat:** AI-ul răspunde cu informații relevante despre platformă

#### TC-63 – Context-aware assistance
- **Rol:** User
- **Pași:**
  1. Utilizatorul este pe pagina de evenimente
  2. Întreabă AI-ul "Cum particip la un eveniment?"
- **Rezultat așteptat:** AI-ul oferă ghidare specifică contextualizată

---

### **Securitate și Edge Cases**

#### TC-64 – Tentativă acces neautorizat
- **Rol:** User (Student)
- **Pași:**
  1. Studentul încearcă să acceseze direct endpoint-ul de admin
- **Rezultat așteptat:** 403 Forbidden, acces respins

#### TC-65 – Sesiune expirată
- **Rol:** User
- **Pași:**
  1. Utilizatorul rămâne inactiv peste 1 oră
  2. Încearcă să efectueze o acțiune
- **Rezultat așteptat:** 401 Unauthorized, redirect la login

#### TC-66 – Upload fișier tip invalid
- **Rol:** User
- **Pași:**
  1. Utilizatorul încearcă să uploadeze un fișier .exe
- **Rezultat așteptat:** Eroare de validare, fișierul este respins

#### TC-67 – Upload fișier prea mare
- **Rol:** User
- **Pași:**
  1. Utilizatorul încearcă să uploadeze un fișier de 100MB
- **Rezultat așteptat:** Eroare că fișierul depășește limita de 50MB

#### TC-68 – SQL Injection attempt
- **Rol:** Attacker
- **Pași:**
  1. Input în search box: `' OR '1'='1`
- **Rezultat așteptat:** Query-ul este sanitizat, nu produce rezultate neașteptate

#### TC-69 – XSS attempt
- **Rol:** Attacker
- **Pași:**
  1. Input în anunț: `<script>alert('XSS')</script>`
- **Rezultat așteptat:** Script-ul este escaped, afișat ca text simplu

---

## 6. Concluzii și Rezultate

### 6.1. Acoperire Testare

Planul de testare combină **două niveluri complementare** de validare pentru asigurarea calității:

#### **Testare Automată (Unit Testing)**
- Acoperire adecvată pentru funcționalitățile esențiale ale aplicației
- Rulare rapidă pentru feedback imediat în procesul de development
- Detectare timpurie a regresiilor și bug-urilor la nivel de cod

#### **Testare Manuală End-to-End**

**69 de scenarii manuale** care validează toate funcționalitățile critice:

- **Autentificare și management utilizatori** (6 scenarii)
- **Evenimente** (8 scenarii)
- **Grupuri de studiu** (9 scenarii)
- **Subscription system** (3 scenarii)
- **Notificări** (3 scenarii)
- **Achievements** (4 scenarii)
- **Activity logging** (2 scenarii)
- **Room booking** (5 scenarii)
- **Schedule management** (4 scenarii)
- **Bibliotecă digitală avansată** (4 scenarii)
- **Management note** (5 scenarii)
- **Email confirmation** (2 scenarii)
- **Management facilități** (6 scenarii)
- **AI Assistant** (2 scenarii)
- **Securitate și edge cases** (6 scenarii)

### 6.2. Beneficii Abordare Hibridă

**Testarea automată** asigură stabilitatea componentelor individuale și permite refactorizări sigure, în timp ce **testarea manuală end-to-end** validează fluxurile complete de utilizare și experiența utilizatorului final. Această combinație oferă confidence că aplicația funcționează corect atât la nivel tehnic, cât și din perspectiva utilizatorului.

---

## 7. Rezultatele testării

În urma rulării testelor automate și a scenariilor manuale:

**Unit Tests:**
- Toate testele automate trec cu succes
- Componentele testate răspund corect la scenariile definite
- Nu au fost identificate regresii în funcționalitățile validate

**Manual E2E Tests:**
- Funcționalitățile principale ale aplicației au fost validate cu succes
- Aplicația respectă restricțiile de acces bazate pe roluri
- Au fost identificate și corectate probleme minore de validare și afișare înainte de livrare
- Nu au fost identificate erori critice care să împiedice utilizarea aplicației

---

## 8. Concluzie Finală

**Abordarea hibridă de testare** (unit testing + testare manuală end-to-end) s-a dovedit eficientă pentru CampusConnect:

✓ **Testarea automată** oferă siguranță la nivel de componente și permite detectarea rapidă a problemelor

✓ **Testarea manuală** validează fluxurile complete de utilizare și experiența utilizatorului din perspectiva tuturor rolurilor (Student, Profesor, Administrator)

✓ **Combinația celor două abordări** asigură că aplicația este **stabilă, coerentă și pregătită pentru utilizare în context academic**

**Concluzie:** Aplicația CampusConnect a fost validată prin teste la multiple niveluri și este pregătită pentru livrare, oferind funcționalitățile promise cu un nivel ridicat de calitate și stabilitate.

---

**Document Version:** 1.0  
**Data:** 30 ianuarie 2026  
**Status:** Ready for Testing
