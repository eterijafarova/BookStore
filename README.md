﻿# BookShop
# 🎩 CheshireShelf 📚

## 🏷 Название проекта

CheshireShelf

## 💬 Краткое описание

Виртуальный книжный магазин для удобного управления каталогом книг, заказами и пользователями. SPA на React + TypeScript (Vite) и RESTful API на ASP.NET Core (.NET 8).

## 📋 Требования

* 🔧 **.NET SDK 8.0**
* 🔧 **Node.js v16+** и npm/yarn
* 🔧 **SQL Server / LocalDB**
* 🔧 **Azure Storage Account** (для обложек)

## 🛠 Использованные технологии

* **Backend:** ASP.NET Core (.NET 8), EF Core, SQL Server, FluentValidation, JWT, Swagger (Swashbuckle)
* **Frontend:** React 18, TypeScript, Vite, Tailwind CSS, Redux Toolkit, Axios, React Router
* **Хранение файлов:** Azure Blob Storage
* **Тестирование:** xUnit, Moq, FluentAssertions

## 🚀 Установка

1. **Клонирование репозиториев**

   ```bash
   git clone https://github.com/eterijafarova/BookStore.git
   git clone https://github.com/Dottdost/BookShopFront.git
   ```
2. **Backend**

   ```bash
   cd BookShop
   dotnet restore        # Установка пакетов
   dotnet ef database update  # Миграции БД
   ```
3. **Файл окружения (Backend)**
   Создайте `BookShop/.env`:

   ```env
   ASPNETCORE_ENVIRONMENT=Development
   ConnectionStrings__DefaultConnection="Server=.;Database=BookShop;Trusted_Connection=True;"
   Jwt__Key="ваш_секрет"
   Jwt__Issuer="CheshireShelf"
   Jwt__Audience="CheshireShelfUsers"
   Azure__Blob__ConnectionString="<AZURE_CONN_STRING>"
   Azure__Blob__ContainerName="bookcovers"
   ```
4. **Frontend**

   ```bash
   cd ../BookShopFront
   npm install           # Установка npm зависимостей
   ```
5. **Файл окружения (Frontend)**
   Создайте `BookShopFront/.env`:

   ```env
   VITE_API_URL="http://localhost:5000/api/v1"
   ```

## ⚡ Использование

* **Запуск API (Backend):**

  ```bash
  cd BookShop
  dotnet run
  ```

  Доступно: `http://localhost:5000/api/v1`

* **Запуск UI (Frontend):**

  ```bash
  cd BookShopFront
  npm run dev
  ```

  Доступно: `http://localhost:5173`

## 💻 Примеры команд

* Получить список книг:

  ```bash
  curl -X GET "http://localhost:5000/api/v1/books?page=1&pageSize=5" \
    -H "Authorization: Bearer <TOKEN>"
  ```

## 👤 Авторы

Email : schelfsheshire@gmail.com

*Спасибо за использование CheshireShelf!*
