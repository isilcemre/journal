IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508102113_InitialCreate'
)
BEGIN
    CREATE TABLE [Users] (
        [Id] int NOT NULL IDENTITY,
        [Username] nvarchar(max) NOT NULL,
        [PasswordHash] nvarchar(max) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508102113_InitialCreate'
)
BEGIN
    CREATE TABLE [JournalEntries] (
        [Id] int NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [Title] nvarchar(max) NOT NULL,
        [Content] nvarchar(max) NOT NULL,
        [ImageData] varbinary(max) NULL,
        [ImageContentType] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_JournalEntries] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_JournalEntries_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508102113_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_JournalEntries_UserId] ON [JournalEntries] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508102113_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260508102113_InitialCreate', N'8.0.8');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508114718_AddMultiImages'
)
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[JournalEntries]') AND [c].[name] = N'ImageContentType');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [JournalEntries] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [JournalEntries] DROP COLUMN [ImageContentType];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508114718_AddMultiImages'
)
BEGIN
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[JournalEntries]') AND [c].[name] = N'ImageData');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [JournalEntries] DROP CONSTRAINT [' + @var1 + '];');
    ALTER TABLE [JournalEntries] DROP COLUMN [ImageData];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508114718_AddMultiImages'
)
BEGIN
    CREATE TABLE [JournalImages] (
        [Id] int NOT NULL IDENTITY,
        [JournalEntryId] int NOT NULL,
        [ImageData] varbinary(max) NOT NULL,
        [ImageContentType] nvarchar(max) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_JournalImages] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_JournalImages_JournalEntries_JournalEntryId] FOREIGN KEY ([JournalEntryId]) REFERENCES [JournalEntries] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508114718_AddMultiImages'
)
BEGIN
    CREATE INDEX [IX_JournalImages_JournalEntryId] ON [JournalImages] ([JournalEntryId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508114718_AddMultiImages'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260508114718_AddMultiImages', N'8.0.8');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260517135040_AddFolderModel'
)
BEGIN
    ALTER TABLE [JournalEntries] ADD [FolderId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260517135040_AddFolderModel'
)
BEGIN
    CREATE TABLE [Folders] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(60) NOT NULL,
        [Emoji] nvarchar(10) NOT NULL,
        [IsPreset] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_Folders] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260517135040_AddFolderModel'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'Emoji', N'IsPreset', N'Name') AND [object_id] = OBJECT_ID(N'[Folders]'))
        SET IDENTITY_INSERT [Folders] ON;
    EXEC(N'INSERT INTO [Folders] ([Id], [CreatedAt], [Emoji], [IsPreset], [Name])
    VALUES (1, ''2024-01-01T00:00:00.0000000'', N''🧳'', CAST(1 AS bit), N''Gezi''),
    (2, ''2024-01-01T00:00:00.0000000'', N''🏠'', CAST(1 AS bit), N''Aile''),
    (3, ''2024-01-01T00:00:00.0000000'', N''💼'', CAST(1 AS bit), N''İş''),
    (4, ''2024-01-01T00:00:00.0000000'', N''🌟'', CAST(1 AS bit), N''Anılar''),
    (5, ''2024-01-01T00:00:00.0000000'', N''🤝'', CAST(1 AS bit), N''Arkadaşlar''),
    (6, ''2024-01-01T00:00:00.0000000'', N''🌱'', CAST(1 AS bit), N''Kişisel'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'Emoji', N'IsPreset', N'Name') AND [object_id] = OBJECT_ID(N'[Folders]'))
        SET IDENTITY_INSERT [Folders] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260517135040_AddFolderModel'
)
BEGIN
    CREATE INDEX [IX_JournalEntries_FolderId] ON [JournalEntries] ([FolderId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260517135040_AddFolderModel'
)
BEGIN
    ALTER TABLE [JournalEntries] ADD CONSTRAINT [FK_JournalEntries_Folders_FolderId] FOREIGN KEY ([FolderId]) REFERENCES [Folders] ([Id]) ON DELETE SET NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260517135040_AddFolderModel'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260517135040_AddFolderModel', N'8.0.8');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260517152734_AddUserIdToFolder'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260517152734_AddUserIdToFolder', N'8.0.8');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260518065359_AddFolders'
)
BEGIN
    ALTER TABLE [JournalEntries] DROP CONSTRAINT [FK_JournalEntries_Folders_FolderId];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260518065359_AddFolders'
)
BEGIN
    EXEC(N'DELETE FROM [Folders]
    WHERE [Id] = 1;
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260518065359_AddFolders'
)
BEGIN
    EXEC(N'DELETE FROM [Folders]
    WHERE [Id] = 2;
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260518065359_AddFolders'
)
BEGIN
    EXEC(N'DELETE FROM [Folders]
    WHERE [Id] = 3;
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260518065359_AddFolders'
)
BEGIN
    EXEC(N'DELETE FROM [Folders]
    WHERE [Id] = 4;
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260518065359_AddFolders'
)
BEGIN
    EXEC(N'DELETE FROM [Folders]
    WHERE [Id] = 5;
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260518065359_AddFolders'
)
BEGIN
    EXEC(N'DELETE FROM [Folders]
    WHERE [Id] = 6;
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260518065359_AddFolders'
)
BEGIN
    EXEC sp_rename N'[Folders].[IsPreset]', N'IsDefault', N'COLUMN';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260518065359_AddFolders'
)
BEGIN
    DECLARE @var2 sysname;
    SELECT @var2 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Folders]') AND [c].[name] = N'Name');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Folders] DROP CONSTRAINT [' + @var2 + '];');
    ALTER TABLE [Folders] ALTER COLUMN [Name] nvarchar(max) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260518065359_AddFolders'
)
BEGIN
    DECLARE @var3 sysname;
    SELECT @var3 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Folders]') AND [c].[name] = N'Emoji');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [Folders] DROP CONSTRAINT [' + @var3 + '];');
    ALTER TABLE [Folders] ALTER COLUMN [Emoji] nvarchar(max) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260518065359_AddFolders'
)
BEGIN
    ALTER TABLE [Folders] ADD [Description] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260518065359_AddFolders'
)
BEGIN
    ALTER TABLE [Folders] ADD [UserId] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260518065359_AddFolders'
)
BEGIN
    CREATE TABLE [JournalEntryFolders] (
        [Id] int NOT NULL IDENTITY,
        [JournalEntryId] int NOT NULL,
        [FolderId] int NOT NULL,
        CONSTRAINT [PK_JournalEntryFolders] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_JournalEntryFolders_Folders_FolderId] FOREIGN KEY ([FolderId]) REFERENCES [Folders] ([Id]),
        CONSTRAINT [FK_JournalEntryFolders_JournalEntries_JournalEntryId] FOREIGN KEY ([JournalEntryId]) REFERENCES [JournalEntries] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260518065359_AddFolders'
)
BEGIN
    CREATE INDEX [IX_Folders_UserId] ON [Folders] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260518065359_AddFolders'
)
BEGIN
    CREATE INDEX [IX_JournalEntryFolders_FolderId] ON [JournalEntryFolders] ([FolderId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260518065359_AddFolders'
)
BEGIN
    CREATE INDEX [IX_JournalEntryFolders_JournalEntryId] ON [JournalEntryFolders] ([JournalEntryId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260518065359_AddFolders'
)
BEGIN
    ALTER TABLE [Folders] ADD CONSTRAINT [FK_Folders_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260518065359_AddFolders'
)
BEGIN
    ALTER TABLE [JournalEntries] ADD CONSTRAINT [FK_JournalEntries_Folders_FolderId] FOREIGN KEY ([FolderId]) REFERENCES [Folders] ([Id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260518065359_AddFolders'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260518065359_AddFolders', N'8.0.8');
END;
GO

COMMIT;
GO