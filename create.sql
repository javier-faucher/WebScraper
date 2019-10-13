IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

CREATE TABLE [sessions] (
    [requestTime] datetime2 NOT NULL,
    [keyWords] nvarchar(max) NULL,
    [requestedUrl] nvarchar(max) NULL,
    [query] nvarchar(max) NULL,
    [appearedList] nvarchar(max) NULL,
    [numberOfResults] int NOT NULL,
    CONSTRAINT [PK_sessions] PRIMARY KEY ([requestTime])
);

GO

CREATE TABLE [singleResults] (
    [Id] int NOT NULL IDENTITY,
    [url] nvarchar(max) NULL,
    [sessionrequestTime] datetime2 NULL,
    CONSTRAINT [PK_singleResults] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_singleResults_sessions_sessionrequestTime] FOREIGN KEY ([sessionrequestTime]) REFERENCES [sessions] ([requestTime]) ON DELETE NO ACTION
);

GO

CREATE INDEX [IX_singleResults_sessionrequestTime] ON [singleResults] ([sessionrequestTime]);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20191013142407_initCreate', N'2.1.11-servicing-32099');

GO

