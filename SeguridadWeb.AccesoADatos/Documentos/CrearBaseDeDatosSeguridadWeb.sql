xUSE [master]
GO
CREATE DATABASE [SeguridadWebdb]
GO
USE [SeguridadWebdb]
GO
-- Crear la tabla de Rol
CREATE TABLE [dbo].[Rol](
  [Id] [int] PRIMARY KEY IDENTITY(1,1) NOT NULL,
  [Nombre] [varchar](30) NOT NULL
)
GO
-- Crear la tabla de Usuario
CREATE TABLE [dbo].[Usuario](
   [Id] [int] PRIMARY KEY IDENTITY(1,1) NOT NULL,
   [IdRol] [int] NOT NULL,
   [Nombre] [varchar](30) NOT NULL,
   [Apellido] [varchar](30) NOT NULL,
   [Login] [varchar](25) NOT NULL,
   [Password] [char](32) NOT NULL,
   [Estatus] [tinyint] NOT NULL,
   [FechaRegistro] [datetime] NOT NULL,
   CONSTRAINT FK1_Rol_Usuario FOREIGN KEY (IdRol) REFERENCES Rol (Id)
)
GO
-- Crear la tabla Categoria
CREATE TABLE Categoria (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Nombre NVARCHAR(255) NOT NULL,
    Top_Aux INT NULL
);
GO
-- Crear la tabla Producto
CREATE TABLE Producto (
    Id INT PRIMARY KEY IDENTITY(1,1),
    IdCategoria INT NOT NULL,
    Nombre NVARCHAR(255) NOT NULL,
    Precio DECIMAL(10, 2) NOT NULL,
    Cantidad INT NOT NULL,
    Descripcion NVARCHAR(MAX) NULL,
    FotoProducto VARBINARY(MAX) NULL,
    Estatus TINYINT NOT NULL,
    Top_Aux INT NULL,
    FOREIGN KEY (IdCategoria) REFERENCES Categoria(Id)
);
