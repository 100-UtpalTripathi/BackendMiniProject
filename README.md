# Car Booking App

## Table of Contents
- [Introduction](#introduction)
- [Features](#features)
- [Technologies Used](#technologies-used)
- [Setup](#setup)
- [Admin Module](#admin-module)
- [Customer Module](#customer-module)
- [Discount Logics](#discount-logics)
- [Cancellation Charges](#cancellation-charges)

## Introduction
The Car Booking App is a web application that allows admins to manage cars, cities, customer queries, and bookings, while customers can view available cars, make bookings, manage their profiles, and submit queries. The app is built using HTML, CSS, Bootstrap, JavaScript for the frontend, and C#, .NET, ASP.NET Web API, EF Core, and Microsoft SQL Server for the backend.

## Features

### Admin Module
- **Login/Logout**: Authentication using JWT.
- **Manage Cities**: Add, edit, and delete city records.
- **Manage Cars**:
  - Register new cars.
  - Edit or delete existing cars.
- **Contact Us Queries**: View, respond to, and close customer queries.
- **View and Manage Bookings**: View all bookings, cancel bookings, and handle booking-related issues.
- **View and Manage Customers**: Manage customer records.

### Customer Module
- **Login/Logout**: User authentication using JWT.
- **View Cars**: Browse available cars.
- **Book a Car**: Select a car, specify booking dates, and confirm bookings.
- **Contact Admin**: Submit queries or issues, track query status, and view responses.
- **View Bookings**: View details of previous and upcoming bookings.
- **Cancel Bookings**: Cancel existing bookings.
- **Manage Profile**: Edit profile details such as email, phone number, and address.
- **Rate and Review Cars**: Rate a car based on the booking experience.

## Technologies Used
- **Frontend**: HTML, CSS, Bootstrap, JavaScript
- **Backend**: C#, .NET, ASP.NET Web API, EF Core
- **Database**: Microsoft SQL Server
- **Authentication**: JWT

## Setup
1. **Clone the repository**:
    ```bash
    git clone https://github.com/your-username/car-booking-app.git
    cd car-booking-app
    ```

2. **Install dependencies**:
    ```bash
    # For frontend
    npm install
    
    # For backend
    dotnet restore
    ```

3. **Setup the database**:
    - Ensure Microsoft SQL Server is installed and running.
    - Update the connection string in `appsettings.json`.

4. **Run the application**:
    ```bash
    # For frontend
    npm start
    
    # For backend
    dotnet run
    ```

## Admin Module

### 1. Login / Logout
Admins can log in and out using JWT authentication.

### 2. Manage Cities
Admins can add, edit, and delete city records.

### 3. Add a new Car
Admins can register new cars in the database.

### 4. Edit/Delete Existing Car
Admins can update car details or remove cars from the database.

### 5. Contact Us Queries
Admins can view, respond to, and close customer queries.

### 6. View and Manage Bookings
Admins can view all bookings, cancel bookings if necessary, and handle booking-related issues.

### 7. View and Manage Customers
Admins can manage customer records.

## Customer Module

### 1. Login / Logout
Customers can log in and out using JWT authentication.

### 2. View Cars
Customers can browse available cars.

### 3. Booking a Car
Customers can select a car, specify booking dates, and confirm bookings.

### 4. Ask Queries through Contact Admin
Customers can submit queries or issues to the admin, track query status, and view responses.

### 5. View Bookings
Customers can view details of previous and upcoming bookings.

### 6. Cancel Bookings
Customers can cancel existing bookings.

### 7. Manage Profile
Customers can edit their profile details such as email, phone number, and address.

### 8. Rate and Review a Car
Customers can rate a car based on their booking experience.

## Discount Logics

### 1. Seasonal Discount
- 10% off in December.

### 2. Loyalty Discount
- 5% off for customers with more than 5 bookings.

## Cancellation Charges

### 1. Free Cancellation
- No fee for cancellations made 48 hours or more before the booking start date.

### 2. Cancellation Fee
- Dynamic cancellation fee for cancellations made within 48 hours of the booking start date, based on the number of hours after the free cancellation period.

## Contributing
Contributions are welcome! Please fork the repository and submit a pull request for review.

## License
No License Needed! Thanks...
