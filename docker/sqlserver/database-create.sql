USE [master]
GO

IF DB_ID('Weather') IS NOT NULL
  set noexec on 
  
CREATE DATABASE [Weather]
GO

USE [Weather];
GO

CREATE TABLE WeatherForecast (
    Id INT PRIMARY KEY IDENTITY (1, 1),
	Date DATETIME,
	TemperatureC INT NOT NULL,
    Summary VARCHAR (50) NOT NULL,
);
GO