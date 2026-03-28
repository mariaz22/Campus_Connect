# CampusConnect - Environment-uri și CI/CD

## 1. Descrierea Environment-urilor

### 1.1 Mediul de Dezvoltare 

**Scop:** Acest mediu este utilizat pentru scrierea codului, debugging si testarea functionalitatilor in timp real.

**Frontend** -Ruleaza pe masina locala folosind serverul de dezvoltare Vite (`npm run dev`), accesibil la `http://localhost:5173`.
**Hot Module Replacement** pentru actualizari instantanee ale interfetei. 
**Backend** -Ruleaza local prin **Kestrel** sau IIS Express, conectandu-se la o baza de date de dezvoltare (LocalDB). 
**Baza de date**(SQL Server LocalDB) - instanta locala
**Configurare**-Variabilele de mediu sunt gestionate prin fișiere `.env.development` și `appsettings.Development.json`. |

### 1.2 Mediul de Productie

**Scop:** Mediul live, optimizat pentru performanta si securitate, accesibil utilizatorilor finali.

**Frontend** -Este gazduit pe platforma **Vercel**. Codul  este compilat in fisiere HTML/CSS/JS minificate si distribuite global printr-o retea **CDN** 
**Backend**- Este containerizat folosind **Docker** si gazduit pe platforma **Render**. Ruleaza o versiune a aplicatiei .NET 9.
**Bază de date** (**Azure SQL Database**) - baza de date cloud managed, cu backup automat.
**Configurare**- Variabilele de mediu sunt injectate prin dashboard-urile Vercel și Render (Environment Variables).

---

## 2. Diferente intre Environment-uri

Principalele diferente tehnice care asigura separarea responsabilitatilor:

| Diferenta | Mediul de Dezvoltare| Mediul de Productie|
|----------------|------------------------------|----------------------------|
| **Hosting** | Localhost (Resurse proprii) | Cloud PaaS (Vercel & Render) |
| **Accesibilitate** | Privata (doar dezvoltatorul) | Publica |
| **Frontend URL** | `http://localhost:5173` | `https://campusconnect.vercel.app` |
| **API URL** | `http://localhost:5099/api` | `https://campus-api.onrender.com/api` |
| **Backend Runtime** | .NET SDK (Rulare directa din sursa) | Docker Container |
| **Bază de date** | LocalDB (SQL Server Express) | Azure SQL Database |
| **Politica CORS** | Permisiva (`AllowAll`) | Restrictiva (Accepta doar originea Vercel) |
| **Seed Data** | Da | Nu |
| **Log Level** | `Information`| `Warning`  |
| **Swagger UI** | Activat | Activat |

---

## 3. Configurari Specifice si Pipeline CI/CD

Pentru a automatiza tranzitia codului de la dezvoltare la productie si a rezolva provocarile specifice structurii proiectului, am implementat urmatoarele configurari:

### A. Gestionarea Structurii Monorepo în CI/CD

Am configurat pipeline-ul GitHub Actions (`main-deploy.yml`) sa utilizeze parametrul `working-directory` pentru fiecare job:

- **Frontend Build:** `working-directory: ./src/campusconnect-client`
- **Backend Build:** `working-directory: ./src/CampusConnect/CampusConnect.Api`

### B. Containerizare Personalizată (.NET 9)

Platforma Render nu suporta nativ .NET 9 ca optiune predefinita, așa ca am creat un **Dockerfile multi-stage**

### C. Configurare Root Directory în Vercel

Pentru a permite platformei Vercel sa detecteze aplicatia React, am modificat setarea **Root Directory** in `src/campusconnect-client`. Acest lucru ii spune procesului de build sa ignore radacina repository-ului si sa se concentreze doar pe folderul aplicatiei client.


### D. Injectarea Dinamică a URL-ului API


- **Local:** VITE_BASE_URL Este citita din `.env.development`
- **Producție:** Este injectata din panoul de configurare Vercel (Environment Variables)

## 5. Arhitectura de Deployment

### Fluxul de Deploy Automat

```
1. Developer face git push pe main
          │
          ▼
2. GitHub Actions detecteaza push-ul
          │
          ├──► build-backend (dotnet build)
          │
          ├──► build-frontend (npm run build)
          │
          ▼
3. Daca build-urile reusesc:
          │
          ├──► Render detecteaza push → Rebuild Docker → Deploy Backend
          │
          └──► Vercel detecteaza push → npm build → Deploy Frontend
          │
          ▼
4. Aplicatia devine live
   • Frontend: https://campusconnect.vercel.app
   • Backend:  https://campus-api.onrender.com
   • Swagger:  https://campus-api.onrender.com/swagger
```