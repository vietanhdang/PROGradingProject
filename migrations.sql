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

CREATE TABLE [Accounts] (
    [AccountId] int NOT NULL IDENTITY,
    [Email] nvarchar(max) NOT NULL,
    [Password] nvarchar(max) NOT NULL,
    [Role] int NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedDate] datetime2 NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [UpdatedBy] nvarchar(max) NULL,
    CONSTRAINT [PK_Accounts] PRIMARY KEY ([AccountId])
);
GO

CREATE TABLE [Students] (
    [StudentId] int NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [Code] nvarchar(450) NOT NULL,
    [Phone] nvarchar(max) NULL,
    [Address] nvarchar(max) NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedDate] datetime2 NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [UpdatedBy] nvarchar(max) NULL,
    CONSTRAINT [PK_Students] PRIMARY KEY ([StudentId]),
    CONSTRAINT [FK_Students_Accounts_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Accounts] ([AccountId])
);
GO

CREATE TABLE [Teachers] (
    [TeacherId] int NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [Code] nvarchar(450) NOT NULL,
    [Phone] nvarchar(max) NULL,
    [Address] nvarchar(max) NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedDate] datetime2 NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [UpdatedBy] nvarchar(max) NULL,
    CONSTRAINT [PK_Teachers] PRIMARY KEY ([TeacherId]),
    CONSTRAINT [FK_Teachers_Accounts_TeacherId] FOREIGN KEY ([TeacherId]) REFERENCES [Accounts] ([AccountId])
);
GO

CREATE TABLE [Exams] (
    [ExamId] int NOT NULL IDENTITY,
    [TeacherId] int NOT NULL,
    [ExamName] nvarchar(max) NOT NULL,
    [ExamCode] nvarchar(max) NOT NULL,
    [Password] nvarchar(max) NOT NULL,
    [QuestionFolder] nvarchar(max) NOT NULL,
    [AnswerFolder] nvarchar(max) NOT NULL,
    [TotalQuestions] int NOT NULL,
    [StartTime] datetime2 NOT NULL,
    [EndTime] datetime2 NOT NULL,
    [Status] int NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedDate] datetime2 NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [UpdatedBy] nvarchar(max) NULL,
    CONSTRAINT [PK_Exams] PRIMARY KEY ([ExamId]),
    CONSTRAINT [FK_Exams_Teachers_TeacherId] FOREIGN KEY ([TeacherId]) REFERENCES [Teachers] ([TeacherId]) ON DELETE CASCADE
);
GO

CREATE TABLE [ExamStudents] (
    [ExamStudentId] int NOT NULL IDENTITY,
    [Score] int NOT NULL,
    [StartTime] int NOT NULL,
    [SubmitedTime] int NOT NULL,
    [SubmitedFolder] nvarchar(max) NOT NULL,
    [Status] int NOT NULL,
    [MarkLog] nvarchar(max) NOT NULL,
    [StudentId] int NOT NULL,
    [ExamId] int NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedDate] datetime2 NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [UpdatedBy] nvarchar(max) NULL,
    CONSTRAINT [PK_ExamStudents] PRIMARY KEY ([ExamStudentId]),
    CONSTRAINT [FK_ExamStudents_Exams_ExamId] FOREIGN KEY ([ExamId]) REFERENCES [Exams] ([ExamId]),
    CONSTRAINT [FK_ExamStudents_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Students] ([StudentId])
);
GO

CREATE INDEX [IX_Exams_TeacherId] ON [Exams] ([TeacherId]);
GO

CREATE INDEX [IX_ExamStudents_ExamId] ON [ExamStudents] ([ExamId]);
GO

CREATE INDEX [IX_ExamStudents_StudentId] ON [ExamStudents] ([StudentId]);
GO

CREATE UNIQUE INDEX [IX_Students_Code] ON [Students] ([Code]);
GO

CREATE UNIQUE INDEX [IX_Teachers_Code] ON [Teachers] ([Code]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230618142826_GradingSystemV1', N'6.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [ExamStudents] ADD [CountTimeSubmit] int NOT NULL DEFAULT 0;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230620151822_GradingSystemV2', N'6.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Exams] ADD [TestCaseFolder] nvarchar(max) NOT NULL DEFAULT N'';
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230620165848_GradingSystemV3', N'6.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ExamStudents]') AND [c].[name] = N'SubmitedTime');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [ExamStudents] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [ExamStudents] ALTER COLUMN [SubmitedTime] int NULL;
GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ExamStudents]') AND [c].[name] = N'SubmitedFolder');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [ExamStudents] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [ExamStudents] ALTER COLUMN [SubmitedFolder] nvarchar(max) NULL;
GO

DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ExamStudents]') AND [c].[name] = N'StartTime');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [ExamStudents] DROP CONSTRAINT [' + @var2 + '];');
ALTER TABLE [ExamStudents] ALTER COLUMN [StartTime] int NULL;
GO

DECLARE @var3 sysname;
SELECT @var3 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ExamStudents]') AND [c].[name] = N'Score');
IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [ExamStudents] DROP CONSTRAINT [' + @var3 + '];');
ALTER TABLE [ExamStudents] ALTER COLUMN [Score] int NULL;
GO

DECLARE @var4 sysname;
SELECT @var4 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ExamStudents]') AND [c].[name] = N'MarkLog');
IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [ExamStudents] DROP CONSTRAINT [' + @var4 + '];');
ALTER TABLE [ExamStudents] ALTER COLUMN [MarkLog] nvarchar(max) NULL;
GO

DECLARE @var5 sysname;
SELECT @var5 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ExamStudents]') AND [c].[name] = N'CountTimeSubmit');
IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [ExamStudents] DROP CONSTRAINT [' + @var5 + '];');
ALTER TABLE [ExamStudents] ALTER COLUMN [CountTimeSubmit] int NULL;
GO

DECLARE @var6 sysname;
SELECT @var6 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Exams]') AND [c].[name] = N'Password');
IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [Exams] DROP CONSTRAINT [' + @var6 + '];');
ALTER TABLE [Exams] ALTER COLUMN [Password] nvarchar(max) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230620172901_GradingSystemV4', N'6.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var7 sysname;
SELECT @var7 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ExamStudents]') AND [c].[name] = N'SubmitedTime');
IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [ExamStudents] DROP CONSTRAINT [' + @var7 + '];');
ALTER TABLE [ExamStudents] ALTER COLUMN [SubmitedTime] datetime2 NOT NULL;
ALTER TABLE [ExamStudents] ADD DEFAULT '0001-01-01T00:00:00.0000000' FOR [SubmitedTime];
GO

DECLARE @var8 sysname;
SELECT @var8 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ExamStudents]') AND [c].[name] = N'StartTime');
IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [ExamStudents] DROP CONSTRAINT [' + @var8 + '];');
ALTER TABLE [ExamStudents] ALTER COLUMN [StartTime] datetime2 NOT NULL;
ALTER TABLE [ExamStudents] ADD DEFAULT '0001-01-01T00:00:00.0000000' FOR [StartTime];
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230620175748_GradingSystemV5', N'6.0.11');
GO

COMMIT;
GO

