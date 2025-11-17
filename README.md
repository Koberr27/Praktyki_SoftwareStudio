# Movie Library - Biblioteka Filmowa



Aplikacja webowa CRUD do zarządzania biblioteką filmową.



## Technologie



### Backend

- .NET 8.0

- Entity Framework Core

- ASP.NET Core Web API



### Frontend

- Vue.js 3

- Bootstrap 5

- Vuelidate

- Axios



## Funkcje

- Dodawanie filmów

- Edycja filmów

- Usuwanie filmów

- Import filmów z zewnętrznego API

- Walidacja formularzy (tytuł max 200 znaków, rok 1900-2200)



## Instalacja i uruchomienie



### Backend

```bash

cd backend/MovieLibrary.API

dotnet restore

dotnet ef database update

dotnet run

```



Backend będzie dostępny na: `http://localhost:5048`

Swagger: `http://localhost:5048/swagger`



### Frontend

```bash

cd frontend/movie-app

npm install

npm run dev

```



Frontend będzie dostępny na: `http://localhost:5173`



## Wymagania



- .NET SDK 8.0 lub nowszy

- Node.js 16+ i npm

- Git



## Autor



Natan Dutkiewicz



