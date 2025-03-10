-- Created by Vertabelo (http://vertabelo.com)

-- tables
-- Table: Models (zmiana z EquipmentModels na Models)
CREATE TABLE Models (
    ModelId int NOT NULL IDENTITY(1,1),  -- Dodanie automatycznego inkrementowania
    Name nvarchar(255) NOT NULL,
    Description nvarchar(500) NOT NULL,
    Price decimal(10,2) NOT NULL,
    CONSTRAINT Models_pk PRIMARY KEY (ModelId)
);

-- Table: Equipments
CREATE TABLE Equipments (
    EquipmentId int NOT NULL IDENTITY(1,1),  -- Dodanie automatycznego inkrementowania
    SerialNumber nvarchar(50) NOT NULL,
    ModelId int NOT NULL,  -- Zmiana z EquipmentModelId na ModelId
    CONSTRAINT Equipments_pk PRIMARY KEY (EquipmentId)
);

-- Table: Rentals
CREATE TABLE Rentals (
    RentalId int NOT NULL IDENTITY(1,1),  -- Dodanie automatycznego inkrementowania
    RentalDate datetime NOT NULL,
    ReturnDate datetime NOT NULL,
    IsReturned bit NOT NULL,
    UserId int NOT NULL,
    EquipmentId int NOT NULL,
    CONSTRAINT Rentals_pk PRIMARY KEY (RentalId)
);

-- Table: Users
CREATE TABLE Users (
    UserId int NOT NULL IDENTITY(1,1),  -- Dodanie automatycznego inkrementowania
    Email nvarchar(255) NOT NULL,
    PasswordHash nvarchar(255) NOT NULL,
    Role nvarchar(50) NOT NULL,
    CONSTRAINT Users_pk PRIMARY KEY (UserId),
    CHECK (Role IN ('Client', 'Admin'))
);

-- Create unique index for Email
CREATE UNIQUE INDEX UniqueEmail ON Users (Email);

-- foreign keys
-- Reference: Equipments_Models (zmiana z Equipment_EquipmentModels na Equipments_Models)
ALTER TABLE Equipments ADD CONSTRAINT Equipments_Models FOREIGN KEY (ModelId)
    REFERENCES Models (ModelId);

-- Reference: Rentals_Equipment (table: Rentals)
ALTER TABLE Rentals ADD CONSTRAINT Rentals_Equipment FOREIGN KEY (EquipmentId)
    REFERENCES Equipments (EquipmentId);

-- Reference: Rentals_User (table: Rentals)
ALTER TABLE Rentals ADD CONSTRAINT Rentals_User FOREIGN KEY (UserId)
    REFERENCES Users (UserId);

-- End of file.
