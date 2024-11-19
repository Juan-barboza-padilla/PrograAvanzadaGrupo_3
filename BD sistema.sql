CREATE DATABASE TaskQueueGrupo3DB;
GO

USE TaskQueueGrupo3DB;

CREATE TABLE Tasks (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(MAX),
    Priority NVARCHAR(10) NOT NULL, -- "Alta", "Media", "Baja"
    ExecutionDate DATETIME NOT NULL,
    Status NVARCHAR(20) NOT NULL, -- "Pendiente", "En Proceso", "Finalizada", "Fallida"
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE()
);

CREATE TABLE TaskLogs (
    Id INT PRIMARY KEY IDENTITY(1,1),
    TaskId INT FOREIGN KEY REFERENCES Tasks(Id),
    LogMessage NVARCHAR(MAX),
    LogDate DATETIME DEFAULT GETDATE()
);
