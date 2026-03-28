# Descriere Arhitecturală – CampusConnect

## 1. Introducere

### 1.1 Contextul sistemului

CampusConnect este o platformă integrată de management universitar destinată studenților, profesorilor și administratorilor din cadrul Universității din București. Sistemul centralizează activități academice și de campus esențiale, precum publicarea anunțurilor și evenimentelor, accesul la biblioteca digitală, vizualizarea situației școlare, generarea documentelor oficiale și gestionarea facilităților campusului, incluzând hărți, săli și fluxuri de rezervare.

### 1.2 Scopul documentului

Acest document prezintă deciziile arhitecturale care au stat la baza dezvoltării CampusConnect, explicând structura aleasă a sistemului, motivația principalelor decizii tehnologice și modul în care arhitectura răspunde cerințelor non-funcționale. Totodată, include o sinteză a produsului final, evidențiind evoluția față de propunerea inițială.

---

## 2. Sinteza produsului rezultat

### 2.1 Capacitățile sistemului final

Sistemul livrat este o aplicație web de tip **Single Page Application** care comunică securizat prin **API-uri REST** cu un backend dedicat. Platforma oferă următoarele funcționalități:

#### Autentificare și Management Utilizatori
- **Autentificare JWT** bazată pe email instituțional (@unibuc.ro, @s.unibuc.ro)
- **Confirmare email** prin link de activare
- **Management profil** - editare date personale, poză de profil
- **Roluri și permisiuni** - Student, Professor, Admin cu acces diferențiat
- **Administrare utilizatori** - activare/dezactivare conturi, gestionare roluri
- **Căutare utilizatori** cu filtrare după rol

#### Anunțuri și Evenimente
- **Publicare anunțuri** cu categorii (Academic, Sports, Social, etc.)
- **Filtrare și căutare** în anunțuri după categorie și cuvinte cheie
- **Bookmark/Save anunțuri** - salvare anunțuri favorite pentru acces rapid
- **Abonare la categorii** - primire notificări automate pentru categoriile de interes
- **Notificări în timp real** la publicarea anunțurilor noi
- **Administrare evenimente** cu detalii (dată, locație, descriere)
- **RSVP la evenimente** - înregistrare și retragere participare
- **Save evenimente** - marcare evenimente de interes
- **Tracking participări** - vizualizare evenimente la care s-a participat

#### Grupuri de Studiu
- **Creare și administrare grupuri** (Professor/Admin)
- **Join/Leave grupuri** - înscriere și părăsire grupuri
- **Task-uri de grup** cu deadlines și descrieri
- **Save și complete tasks** - marcare task-uri personale și finalizare
- **Upload materiale de curs** - resurse specifice fiecărui grup
- **Forward anunțuri către grupuri** - distribuire selectivă anunțuri
- **Anunțuri specifice grupului** - comunicare internă

#### Bibliotecă Digitală
- **Organizare pe foldere** pentru structurare resurse
- **Upload fișiere multiple** (PDF, DOCX, PPTX, XLSX, imagini)
- **Adăugare link-uri externe** - resurse web
- **Download fișiere** - acces rapid la materiale
- **Restricții și validări** - control dimensiune și tip fișier
- **Management conținut** (Professor/Admin) - ștergere și organizare

#### Situație Școlară
- **Vizualizare note** - acces studenți la propriile rezultate
- **Management note** - CRUD complet pentru profesori și administratori
- **Vizualizare per curs** - profesori văd notele doar pentru cursurile proprii
- **Validări și restricții** - control acces bazat pe rol și curs

#### Documente Oficiale
- **Generare Adeverință Student** - document PDF personalizat
- **Generare Situație Școlară** - transcript academic complet
- **Template-uri profesionale** - documente formatate conform standardelor
- **Download instant** - fără timp de așteptare

#### Facilități Campus
- **Hartă interactivă** - vizualizare clădiri și săli
- **Disponibilitate săli** - verificare status în timp real
- **Schedule săli** - vizualizare orar complet zilnic/săptămânal
- **Room booking** - sistem cereri rezervare cu justificare
- **Workflow aprobare** - admin aprobă sau respinge cereri
- **Management facilități** (Admin) - CRUD clădiri, săli și program

### 2.2 Comparația cu planificarea inițială

În raport cu propunerea inițială, majoritatea funcționalităților planificate au fost implementate integral. În plus, au fost adăugate **îmbunătățiri** care cresc valoarea practică a platformei:

- Mecanisme de **abonare la conținut**
- **Logging al activităților** pentru audit
- Un **sistem de achievements** cu rol de gamificare
- Un **chatbox bazat pe inteligență artificială** care asistă utilizatorii în navigarea și utilizarea platformei

---

## 3. Descrierea arhitecturii prin diagrame C4

Arhitectura sistemului CampusConnect este documentată folosind modelul **C4 (Context, Containers, Components)**, care oferă o vizualizare ierarhică și progresiv detaliată a structurii sistemului.

### 3.1 Context Diagram

![Context Diagram](Final%20C4%20Diagrams/Final%20C4%20Context%20Diagram.png)

**Figura 1:** Context Diagram - Vizualizarea actorilor și sistemelor externe care interacționează cu CampusConnect

### 3.2 Container Diagram

![Container Diagram](Final%20C4%20Diagrams/Final%20C4%20Container%20Diagram.png)

**Figura 2:** Container Diagram - Arhitectura la nivel de containere și comunicarea dintre acestea

### 3.3 Component Diagram

![Component Diagram](Final%20C4%20Diagrams/Final%20C4%20Component%20Diagram.png)

**Figura 3:** Component Diagram - Structura detaliată a componentelor backend și dependențele dintre acestea

---

## 4. Decizii arhitecturale

### 4.1 Adoptarea Clean Architecture

Backend-ul a fost structurat conform principiilor **Clean Architecture**, separând clar prezentarea, logica aplicației, domeniul și infrastructura. Această organizare asigură:

- **Independența logicii de business** față de detaliile tehnologice
- **Facilitarea testării** componentelor
- **Mentenanță simplificată**
- **Evoluția incrementală** a sistemului

### 3.2 Abordarea monolit modular

În locul unei arhitecturi distribuite bazate pe microservicii, sistemul a fost dezvoltat ca un **monolit modular**. Domeniile funcționale sunt bine delimitate intern, dar sunt livrate într-un singur backend. Această alegere:

- **Reduce complexitatea operațională**
- **Păstrează posibilitatea de separare viitoare** a componentelor
- Simplifică deployment-ul și debugging-ul

### 3.3 Alegerea stack-ului tehnologic

#### Backend: ASP.NET Core 8

**ASP.NET Core 8** a fost utilizat pentru backend datorită:

- **Performanței ridicate**
- **Stabilității** și maturității framework-ului
- **Integrării excelente** cu mecanisme moderne de dezvoltare

#### Persistență: Entity Framework Core + SQL Server

**Entity Framework Core** împreună cu **SQL Server** oferă:

- Un **model relațional potrivit** pentru datele sistemului
- Un **proces eficient de evoluție** a schemei de date prin migrations
- **Type-safe queries** și abstractizare ORM

#### Autentificare: JWT

**Autentificarea JWT** permite:

- **Sesiuni stateless**
- **Suport pentru scalare orizontală**
- **Securitate robustă** și standard industry

#### Frontend: React + TypeScript

Frontend-ul a fost realizat cu **React** și **TypeScript** pentru:

- O **experiență fluidă** pentru utilizatori
- **Consistență** și **siguranță la nivel de cod** prin type checking
- **Ecosystem bogat** de librării și tooling modern

### 3.4 Aplicarea pattern-urilor de design

Arhitectura aplică consecvent mai multe pattern-uri de design:

- **Repository Pattern**: Accesul la date este abstractizat prin repository-uri
- **Service Layer Pattern**: Logica de business este centralizată în service layer
- **DTO Pattern**: DTO-urile definesc clar contractul API
- **Dependency Injection**: Utilizat extensiv pentru decuplare și testabilitate

---

## 4. Cerințe non-funcționale și suportul arhitectural

### 4.1 Securitate și protecția datelor

CampusConnect respectă cerințele de securitate prin:

- **Autentificare JWT**: Token-uri care elimină stocarea sesiunilor pe server și reduc suprafața de atac
- **Hash-uri securizate**: Parolele sunt stocate exclusiv sub formă de hash
- **Autorizare pe roluri**: Accesul la resurse este controlat prin mecanisme RBAC și validare a proprietății datelor
- **Validare multi-strat**: Input-ul utilizatorilor este verificat pe mai multe niveluri, prevenind introducerea de date malițioase
- **Protecție SQL Injection**: Utilizarea unui ORM cu interogări parametrizate
- **Protecție XSS**: Randarea controlată a conținutului în frontend
- **Comunicare securizată**: Exclusiv prin canale HTTPS

### 4.2 Performanță și răspuns rapid

Arhitectura CampusConnect asigură timpi de răspuns reduși prin:

- **Operațiuni asincrone**: Procesarea eficientă a cererilor concurente în backend
- **Optimizări database**:
  - Încărcarea selectivă a relațiilor necesare (eager loading)
  - Evitarea urmăririi entităților pentru interogările read-only (AsNoTracking)
  - Indexare corespunzătoare pe coloanele frecvent utilizate
- **DTO-uri optimizate**: Payload-uri limitate la informațiile strict necesare
- **Caching client-side**: Mecanisme de caching și încărcare progresivă în frontend

### 4.3 Scalabilitate

CampusConnect este construit pentru scalare prin:

- **Model stateless**: Autentificarea bazată pe token-uri permite rularea mai multor instanțe backend fără sincronizare
- **Separarea frontend/backend**: Permite scalarea independentă a fiecărei componente
- **Pregătire pentru extindere**:
  - Introducerea caching-ului distribuit (Redis)
  - Migrarea fișierelor către servicii de storage externe (Azure Blob Storage)
  - Utilizarea replicilor de citire pentru baza de date

### 4.4 Mențineabilitate

Structura sistemului facilitează mentenanța prin:

- **Arhitectură pe straturi**: Reducerea dependențelor directe dintre zone
- **Organizare modulară**: Componente bine delimitate funcțional
- **Separarea logicii de business**: Independență față de detalii de infrastructură
- **Convenții clare**: Organizare consecventă a codului
- **Pattern-uri consistente**: Aplicare uniformă în întregul sistem

### 4.5 Testabilitate și fiabilitate

CampusConnect permite testare comprehensivă prin:

- **Izolarea logicii**: Business logic separată în service layer
- **Abstractizare repository**: Acces la date decuplat de implementare
- **Dependency Injection**: Dependințele pot fi ușor simulate în teste
- **Teste unitare**: Componente independente testabile izolat
- **Teste de integrare**: Verificarea scenariilor complete
- **Gestionare centralizată a erorilor**: Comportamente previzibile ale API-ului

### 4.6 Uzabilitate

Platforma oferă o experiență utilizator superioară prin:

- **Single Page Application**: Tranziții rapide între funcționalități fără reîncărcarea paginii
- **Design responsive**: Accesibilitate pe dispozitive variate (desktop, tablet, mobile)
- **Feedback vizual**: Loading states, toast notifications, confirmări acțiuni
- **Validare în timp real**: Feedback instant la input utilizator
- **Mesaje clare de eroare**: Ghidare utilizator la probleme
- **Navigare intuitivă**: Structură logică și ușor de înțeles

---

## 5. Trade-off-uri și limitări

### 5.1 Decizii de compromis acceptate

**Microserviciile nu au fost adoptate** din start deoarece:
- Ar fi introdus **complexitate operațională ridicată**
- Provocări specifice sistemelor distribuite (eventual consistency, distributed transactions)
- **Fără beneficii reale** pentru dimensiunea actuală a proiectului
- Arhitectura modulară permite **migrare incrementală** când va fi necesar

**Funcționalitățile în timp real simplificate**:
- **Polling** în loc de WebSockets/SignalR pentru notificări
- Suficient pentru use-case-urile actuale (anunțuri academice nu sunt time-critical)
- Reduce complexitatea deployment-ului

**Subsisteme cu potențial de extindere**:
- **Storage fișiere**: Implementat local în wwwroot, migrare planificată către blob storage
- **Workflow-uri de aprobare**: Implementare basic, extensibilă către workflow engine
- **Search**: SQL LIKE queries, migrare posibilă către Elasticsearch la volume mari

---

## 6. Direcții de evoluție

### 6.1 Evoluții pe termen scurt (3-6 luni)

- Consolidarea sistemului de notificări în timp real
- Introducerea caching-ului distribuit cu Redis
- Migrarea fișierelor către Azure Blob Storage
- Completarea funcționalităților amânate (Digital Student Card)

### 6.2 Evoluții pe termen mediu (6-12 luni)

- Integrări cu sisteme universitare externe (Moodle, sisteme facultate)
- Extinderea zonei AI (recommendation engine, predictive analytics)
- Mobile app (React Native)
- Analytics dashboard pentru administrare

### 6.3 Evoluții pe termen lung (1-2 ani)

- Tranziția graduală către arhitectură distribuită (dacă volumul o justifică)
- Microservices pentru bounded contexts (Academic, Social, Facilities)
- API Gateway pentru routing centralizat
- Service mesh pentru inter-service communication

---

## 7. Concluzie

CampusConnect oferă o **arhitectură solidă și extensibilă**, care îmbină bunele practici din ingineria software cu decizii pragmatice adaptate contextului proiectului.

Compromisurile asumate sunt **justificate** și nu limitează evoluția viitoare a platformei. Arhitectura modulară permite extindere fără refactoring major, iar path-ul către scalare este clar definit.

---


