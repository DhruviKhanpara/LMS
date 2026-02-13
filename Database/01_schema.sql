/*
==================================================================================
This script creates:
- DB, DB user / roles, Grant privileges
==================================================================================
*/

-- ============================================================================
-- All tables includeing the log tables with constraints and indexes
-- ============================================================================

USE LibraryManagementSys;
GO

-- ============================================================================
-- SECTION 1: CREATE SCHEMAS
-- ============================================================================
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'logging')
BEGIN
    EXEC('CREATE SCHEMA [logging] AUTHORIZATION [dbo];');
END
GO

-- ============================================================================
-- SECTION 2: CREATE TABLES - LOOKUP/REFERENCE TABLES FIRST
-- ============================================================================

-- Status Type Table
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'StatusType' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
	CREATE TABLE [dbo].[StatusType] (
	    [Id]          BIGINT         IDENTITY (1, 1) NOT NULL,
	    [Label]       NVARCHAR (50)  NOT NULL,
	    [Description] NVARCHAR (500) NOT NULL,
	    [IsActive]    BIT            CONSTRAINT [DF_StatusType_IsActive] DEFAULT ((1)) NOT NULL,
	    CONSTRAINT [PK_StatusType] PRIMARY KEY CLUSTERED ([Id] ASC)
	);
	
	--CREATE NONCLUSTERED INDEX IX_Genre_IsActive ON [dbo].[StatusType](IsActive);
END
GO

-- Status Table
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Status' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[Status] (
	    [Id]           BIGINT             IDENTITY (1, 1) NOT NULL,
	    [StatusTypeId] BIGINT             NOT NULL,
	    [Label]        NVARCHAR (50)      NOT NULL,
	    [Description]  NVARCHAR (500)     NOT NULL,
	    [Color]        NVARCHAR (50)      CONSTRAINT [DF_Status_Color] DEFAULT (N'#ffffff') NOT NULL,
	    [IsActive]     BIT                CONSTRAINT [DF_Status_IsActive] DEFAULT ((1)) NOT NULL,
	    [CreatedBy]    BIGINT             NULL,
	    [CreatedAt]    DATETIMEOFFSET (7) CONSTRAINT [DF_Status_CreatedAt] DEFAULT (getdate()) NULL,
	    [ModifiedBy]   BIGINT             NULL,
	    [ModifiedAt]   DATETIMEOFFSET (7) NULL,
	    [DeletedBy]    BIGINT             NULL,
	    [DeletedAt]    DATETIMEOFFSET (7) NULL,
	    CONSTRAINT [PK_Status] PRIMARY KEY CLUSTERED ([Id] ASC),
	    CONSTRAINT [FK_Status_StatusType_StatusTypeId] FOREIGN KEY ([StatusTypeId]) REFERENCES [dbo].[StatusType] ([Id]),
	    CONSTRAINT [FK_Status_User_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[User] ([Id]),
	    CONSTRAINT [FK_Status_User_DeletedBy] FOREIGN KEY ([DeletedBy]) REFERENCES [dbo].[User] ([Id]),
	    CONSTRAINT [FK_Status_User_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [dbo].[User] ([Id])
	);

	--CREATE NONCLUSTERED INDEX IX_Genre_IsActive ON [dbo].[Status](IsActive);
END
GO

-- Role Table
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'RoleList' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
	CREATE TABLE [dbo].[RoleList] (
	    [Id]          BIGINT             IDENTITY (1, 1) NOT NULL,
	    [Label]       NVARCHAR (100)     NOT NULL,
	    [Description] NVARCHAR (500)     NOT NULL,
	    [IsActive]    BIT                CONSTRAINT [DF_RoleList_IsActive] DEFAULT ((1)) NOT NULL,
	    [CreatedBy]   BIGINT             NULL,
	    [CreatedAt]   DATETIMEOFFSET (7) CONSTRAINT [DF_RoleList_CreatedAt] DEFAULT (getdate()) NOT NULL,
	    [ModifiedBy]  BIGINT             NULL,
	    [ModifiedAt]  DATETIMEOFFSET (7) NULL,
	    [DeletedBy]   BIGINT             NULL,
	    [DeletedAt]   DATETIMEOFFSET (7) NULL,
	    CONSTRAINT [PK_RoleList] PRIMARY KEY CLUSTERED ([Id] ASC),
	    CONSTRAINT [FK_RoleList_User_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[User] ([Id]),
	    CONSTRAINT [FK_RoleList_User_DeletedBy] FOREIGN KEY ([DeletedBy]) REFERENCES [dbo].[User] ([Id]),
	    CONSTRAINT [FK_RoleList_User_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [dbo].[User] ([Id])
	);

	--CREATE NONCLUSTERED INDEX IX_Genre_IsActive ON [dbo].[RoleList](IsActive);
END
GO

-- Genre Table
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Genre' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
	CREATE TABLE [dbo].[Genre] (
	    [Id]          BIGINT             IDENTITY (1, 1) NOT NULL,
	    [Name]        NVARCHAR (100)     NOT NULL,
	    [Description] NVARCHAR (500)     NOT NULL,
	    [IsActive]    BIT                CONSTRAINT [DF_Genre_IsActive] DEFAULT ((1)) NOT NULL,
	    [CreatedBy]   BIGINT             NOT NULL,
	    [CreatedAt]   DATETIMEOFFSET (7) CONSTRAINT [DF_Genre_CreatedAt] DEFAULT (getdate()) NOT NULL,
	    [ModifiedBy]  BIGINT             NULL,
	    [ModifiedAt]  DATETIMEOFFSET (7) NULL,
	    [DeletedBy]   BIGINT             NULL,
	    [DeletedAt]   DATETIMEOFFSET (7) NULL,
	    CONSTRAINT [PK_Genre] PRIMARY KEY CLUSTERED ([Id] ASC),
	    CONSTRAINT [FK_Genre_User_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[User] ([Id]),
	    CONSTRAINT [FK_Genre_User_DeletedBy] FOREIGN KEY ([DeletedBy]) REFERENCES [dbo].[User] ([Id]),
	    CONSTRAINT [FK_Genre_User_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [dbo].[User] ([Id])
	);
    
    --CREATE NONCLUSTERED INDEX IX_Genre_IsActive ON [dbo].[Genre](IsActive);
END
GO

-- Penalty Type Table
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PenaltyType' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[PenaltyType] (
	    [Id]          BIGINT             IDENTITY (1, 1) NOT NULL,
	    [Label]       NVARCHAR (50)      NOT NULL,
	    [Description] NVARCHAR (500)     NOT NULL,
	    [IsActive]    BIT                CONSTRAINT [DF_PenaltyType_IsActive] DEFAULT ((1)) NOT NULL,
	    [CreatedBy]   BIGINT             NULL,
	    [CreatedAt]   DATETIMEOFFSET (7) CONSTRAINT [DF_PenaltyType_CreatedAt] DEFAULT (getdate()) NOT NULL,
	    [ModifiedBy]  BIGINT             NULL,
	    [ModifiedAt]  DATETIMEOFFSET (7) NULL,
	    [DeletedBy]   BIGINT             NULL,
	    [DeletedAt]   DATETIMEOFFSET (7) NULL,
	    CONSTRAINT [PK_PenaltyType] PRIMARY KEY CLUSTERED ([Id] ASC),
	    CONSTRAINT [FK_PenaltyType_User_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[User] ([Id]),
	    CONSTRAINT [FK_PenaltyType_User_DeletedBy] FOREIGN KEY ([DeletedBy]) REFERENCES [dbo].[User] ([Id]),
	    CONSTRAINT [FK_PenaltyType_User_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [dbo].[User] ([Id])
	);
    
    --CREATE NONCLUSTERED INDEX IX_PenaltyType_IsActive ON [dbo].[PenaltyType](IsActive);
END
GO

-- Membership Table
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Membership' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[Membership] (
	    [Id]               BIGINT             IDENTITY (1, 1) NOT NULL,
	    [Type]             NVARCHAR (255)     NOT NULL,
	    [Description]      NVARCHAR (500)     NOT NULL,
	    [BorrowLimit]      BIGINT             NOT NULL,
	    [ReservationLimit] BIGINT             NOT NULL,
	    [Duration]         BIGINT             NOT NULL,
	    [Cost]             DECIMAL (10, 2)    NOT NULL,
	    [Discount]         DECIMAL (10, 2)    NOT NULL,
	    [IsActive]         BIT                CONSTRAINT [DF_Membership_IsActive] DEFAULT ((1)) NOT NULL,
	    [CreatedBy]        BIGINT             NOT NULL,
	    [CreatedAt]        DATETIMEOFFSET (7) CONSTRAINT [DF_Membership_CreatedAt] DEFAULT (getdate()) NOT NULL,
	    [ModifiedBy]       BIGINT             NULL,
	    [ModifiedAt]       DATETIMEOFFSET (7) NULL,
	    [DeletedBy]        BIGINT             NULL,
	    [DeletedAt]        DATETIMEOFFSET (7) NULL,
	    CONSTRAINT [PK_Membership] PRIMARY KEY CLUSTERED ([Id] ASC),
	    CONSTRAINT [FK_Membership_User_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[User] ([Id]),
	    CONSTRAINT [FK_Membership_User_DeletedBy] FOREIGN KEY ([DeletedBy]) REFERENCES [dbo].[User] ([Id]),
	    CONSTRAINT [FK_Membership_User_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [dbo].[User] ([Id])
	);
    
    --CREATE NONCLUSTERED INDEX IX_Membership_IsActive ON [dbo].[Membership](IsActive);
END
GO

-- Configs Table
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Configs' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
	CREATE TABLE [dbo].[Configs] (
	    [Id]          BIGINT             IDENTITY (1, 1) NOT NULL,
	    [KeyName]     NVARCHAR (50)      NOT NULL,
	    [KeyValue]    NVARCHAR (1000)    NOT NULL,
	    [Description] NVARCHAR (500)     NOT NULL,
	    [IsActive]    BIT                CONSTRAINT [DF_Configs_IsActive] DEFAULT ((1)) NOT NULL,
	    [CreatedBy]   BIGINT             NULL,
	    [CreatedAt]   DATETIMEOFFSET (7) CONSTRAINT [DF_Configs_CreatedAt] DEFAULT (getdate()) NOT NULL,
	    [ModifiedBy]  BIGINT             NULL,
	    [ModifiedAt]  DATETIMEOFFSET (7) NULL,
	    [DeletedBy]   BIGINT             NULL,
	    [DeletedAt]   DATETIMEOFFSET (7) NULL,
	    CONSTRAINT [PK_Configs] PRIMARY KEY CLUSTERED ([Id] ASC),
	    CONSTRAINT [FK_Configs_User_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[User] ([Id]),
	    CONSTRAINT [FK_Configs_User_DeletedBy] FOREIGN KEY ([DeletedBy]) REFERENCES [dbo].[User] ([Id]),
	    CONSTRAINT [FK_Configs_User_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [dbo].[User] ([Id])
	);

	--CREATE NONCLUSTERED INDEX IX_Membership_IsActive ON [dbo].[Configs](IsActive);
END
GO

-- ============================================================================
-- SECTION 3: CREATE MAIN TABLES
-- ============================================================================

-- User Table
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'User' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
	CREATE TABLE [dbo].[User] (
	    [Id]                BIGINT             IDENTITY (1, 1) NOT NULL,
	    [RoleId]            BIGINT             NOT NULL,
	    [FirstName]         VARCHAR (100)      NOT NULL,
	    [MiddleName]        VARCHAR (100)      NULL,
	    [LastName]          VARCHAR (100)      NOT NULL,
	    [DOB]               DATETIMEOFFSET (7) NOT NULL,
	    [Gender]            VARCHAR (50)       NOT NULL,
	    [Address]           NVARCHAR (255)     NOT NULL,
	    [MobileNo]          VARCHAR (12)       NOT NULL,
	    [Email]             NVARCHAR (255)     NOT NULL,
	    [Username]          NVARCHAR (50)      NOT NULL,
	    [PasswordHash]      VARBINARY (MAX)    NOT NULL,
	    [PasswordSolt]      VARBINARY (MAX)    NOT NULL,
	    [ProfilePhoto]      NVARCHAR (MAX)     NULL,
	    [LibraryCardNumber] NVARCHAR (50)      NULL,
	    [JoiningDate]       DATETIMEOFFSET (7) CONSTRAINT [DF_User_JoiningDate] DEFAULT (getdate()) NOT NULL,
	    [IsActive]          BIT                CONSTRAINT [DF_User_IsActive] DEFAULT ((1)) NOT NULL,
	    [CreatedBy]         BIGINT             NULL,
	    [CreatedAt]         DATETIMEOFFSET (7) CONSTRAINT [DF_User_CreatedAt] DEFAULT (getdate()) NOT NULL,
	    [ModifiedBy]        BIGINT             NULL,
	    [ModifiedAt]        DATETIMEOFFSET (7) NULL,
	    [DeletedBy]         BIGINT             NULL,
	    [DeletedAt]         DATETIMEOFFSET (7) NULL,
	    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([Id] ASC),
	    CONSTRAINT [FK_User_User_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[User] ([Id]),
	    CONSTRAINT [FK_User_User_DeletedBy] FOREIGN KEY ([DeletedBy]) REFERENCES [dbo].[User] ([Id]),
	    CONSTRAINT [FK_User_User_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [dbo].[User] ([Id]),
		CONSTRAINT [UK_User_UsernameEmailRole] UNIQUE NONCLUSTERED ([Username] ASC, [Email] ASC, [RoleId] ASC)
	);

	IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UK_User_LibraryCardNumber' AND object_id = OBJECT_ID('[dbo].[User]'))
    BEGIN
        EXEC sp_executesql N'CREATE UNIQUE NONCLUSTERED INDEX [UK_User_LibraryCardNumber]
        ON [dbo].[User]([LibraryCardNumber] ASC) WHERE ([LibraryCardNumber] IS NOT NULL);'
    END
END
GO

-- User Membership Mapping Table
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'UserMembershipMapping' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[UserMembershipMapping] (
	    [Id]                 BIGINT             IDENTITY (1, 1) NOT NULL,
	    [UserId]             BIGINT             NOT NULL,
	    [MembershipId]       BIGINT             NOT NULL,
	    [EffectiveStartDate] DATETIMEOFFSET (7) NOT NULL,
	    [ExpirationDate]     DATETIMEOFFSET (7) NOT NULL,
	    [BorrowLimit]        BIGINT             NOT NULL,
	    [ReservationLimit]   BIGINT             NOT NULL,
	    [MembershipCost]     DECIMAL (10, 2)    NOT NULL,
	    [Discount]           DECIMAL (10, 2)    NOT NULL,
	    [PaidAmount]         AS                 ([MembershipCost]-[Discount]) PERSISTED,
	    [IsActive]           BIT                CONSTRAINT [DF_UserMembershipMapping_IsActive] DEFAULT ((1)) NOT NULL,
	    [CreatedBy]          BIGINT             NOT NULL,
	    [CreatedAt]          DATETIMEOFFSET (7) CONSTRAINT [DF_UserMembershipMapping_CreatedAt] DEFAULT (getdate()) NOT NULL,
	    [ModifiedBy]         BIGINT             NULL,
	    [ModifiedAt]         DATETIMEOFFSET (7) NULL,
	    [DeletedBy]          BIGINT             NULL,
	    [DeletedAt]          DATETIMEOFFSET (7) NULL,
	    CONSTRAINT [PK_UserMembershipMapping] PRIMARY KEY CLUSTERED ([Id] ASC),
	    CONSTRAINT [FK_UserMembershipMapping_Membership_MembershipId] FOREIGN KEY ([MembershipId]) REFERENCES [dbo].[Membership] ([Id]),
	    CONSTRAINT [FK_UserMembershipMapping_User_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[User] ([Id]),
	    CONSTRAINT [FK_UserMembershipMapping_User_DeletedBy] FOREIGN KEY ([DeletedBy]) REFERENCES [dbo].[User] ([Id]),
	    CONSTRAINT [FK_UserMembershipMapping_User_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [dbo].[User] ([Id])
	);
END
GO

-- Books Table
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Books' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[Books] (
	    [Id]                BIGINT             IDENTITY (1, 1) NOT NULL,
	    [GenreId]           BIGINT             NOT NULL,
	    [StatusId]          BIGINT             NOT NULL,
	    [Title]             NVARCHAR (255)     NOT NULL,
	    [BookDescription]   NVARCHAR (MAX)     NOT NULL,
	    [Price]             DECIMAL (18, 2)    NOT NULL,
	    [ISBN]              NVARCHAR (50)      NOT NULL,
	    [Author]            NVARCHAR (255)     NOT NULL,
	    [AuthorDescription] NVARCHAR (MAX)     NOT NULL,
	    [Publisher]         NVARCHAR (255)     NOT NULL,
	    [PublishAt]         DATETIMEOFFSET (7) NOT NULL,
	    [TotalCopies]       BIGINT             NOT NULL,
	    [AvailableCopies]   BIGINT             NOT NULL,
	    [IsActive]          BIT                CONSTRAINT [DF_Books_IsActive] DEFAULT ((1)) NOT NULL,
	    [CreatedBy]         BIGINT             NOT NULL,
	    [CreatedAt]         DATETIMEOFFSET (7) CONSTRAINT [DF_Books_CreatedAt] DEFAULT (getdate()) NOT NULL,
	    [ModifiedBy]        BIGINT             NULL,
	    [ModifiedAt]        DATETIMEOFFSET (7) NULL,
	    [DeletedBy]         BIGINT             NULL,
	    [DeletedAt]         DATETIMEOFFSET (7) NULL,
	    CONSTRAINT [PK_Books] PRIMARY KEY CLUSTERED ([Id] ASC),
	    CONSTRAINT [FK_Books_Genre_GenreId] FOREIGN KEY ([GenreId]) REFERENCES [dbo].[Genre] ([Id]),
	    CONSTRAINT [FK_Books_Status_StatusId] FOREIGN KEY ([StatusId]) REFERENCES [dbo].[Status] ([Id]),
	    CONSTRAINT [FK_Books_User_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[User] ([Id]),
	    CONSTRAINT [FK_Books_User_DeletedBy] FOREIGN KEY ([DeletedBy]) REFERENCES [dbo].[User] ([Id]),
	    CONSTRAINT [FK_Books_User_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [dbo].[User] ([Id]),
	    CONSTRAINT [UK_Book_ISBN] UNIQUE NONCLUSTERED ([ISBN] ASC)
	);
END
GO

-- BookFileMapping Table
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'BookFileMapping' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[BookFileMapping] (
	    [Id]           BIGINT             IDENTITY (1, 1) NOT NULL,
	    [BookId]       BIGINT             NOT NULL,
	    [Label]        NVARCHAR (50)      NOT NULL,
	    [FileLocation] NVARCHAR (MAX)     NOT NULL,
	    [IsActive]     BIT                CONSTRAINT [DF_BookFileMapping_IsActive] DEFAULT ((1)) NOT NULL,
	    [CreatedBy]    BIGINT             NOT NULL,
	    [CreatedAt]    DATETIMEOFFSET (7) CONSTRAINT [DF_BookFileMapping_CreatedAt] DEFAULT (getdate()) NOT NULL,
	    [ModifiedBy]   BIGINT             NULL,
	    [ModifiedAt]   DATETIMEOFFSET (7) NULL,
	    [DeletedBy]    BIGINT             NULL,
	    [DeletedAt]    DATETIMEOFFSET (7) NULL,
	    CONSTRAINT [PK_BookFileMapping] PRIMARY KEY CLUSTERED ([Id] ASC),
	    CONSTRAINT [FK_BookFileMapping_Books_BookId] FOREIGN KEY ([BookId]) REFERENCES [dbo].[Books] ([Id]),
	    CONSTRAINT [FK_BookFileMapping_User_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[User] ([Id]),
	    CONSTRAINT [FK_BookFileMapping_User_DeletedBy] FOREIGN KEY ([DeletedBy]) REFERENCES [dbo].[User] ([Id]),
	    CONSTRAINT [FK_BookFileMapping_User_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [dbo].[User] ([Id])
	);
END
GO

-- Transaction Table
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Transection' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[Transection] (
	    [Id]            BIGINT             IDENTITY (1, 1) NOT NULL,
	    [UserId]        BIGINT             NOT NULL,
	    [BookId]        BIGINT             NOT NULL,
	    [StatusId]      BIGINT             NOT NULL,
	    [BorrowDate]    DATETIMEOFFSET (7) CONSTRAINT [DF_Transection_BorrowDate] DEFAULT (getdate()) NOT NULL,
	    [RenewCount]    INT                CONSTRAINT [DF_Transection_RenewCount] DEFAULT ((0)) NOT NULL,
	    [RenewDate]     DATETIMEOFFSET (7) NULL,
	    [DueDate]       DATETIMEOFFSET (7) CONSTRAINT [DF_Transection_DueDate] DEFAULT (getdate()) NOT NULL,
	    [ReturnDate]    DATETIMEOFFSET (7) NULL,
	    [LostClaimDate] DATETIMEOFFSET (7) NULL,
	    [IsActive]      BIT                CONSTRAINT [DF_Transection_IsActive] DEFAULT ((1)) NOT NULL,
	    [CreatedBy]     BIGINT             NOT NULL,
	    [CreatedAt]     DATETIMEOFFSET (7) CONSTRAINT [DF_Transection_CreatedAt] DEFAULT (getdate()) NOT NULL,
	    [ModifiedBy]    BIGINT             NULL,
	    [ModifiedAt]    DATETIMEOFFSET (7) NULL,
	    [DeletedBy]     BIGINT             NULL,
	    [DeletedAt]     DATETIMEOFFSET (7) NULL,
	    CONSTRAINT [PK_Transection] PRIMARY KEY CLUSTERED ([Id] ASC),
	    CONSTRAINT [FK_Transection_Book_BookId] FOREIGN KEY ([BookId]) REFERENCES [dbo].[Books] ([Id]),
	    CONSTRAINT [FK_Transection_Status_StatusId] FOREIGN KEY ([StatusId]) REFERENCES [dbo].[Status] ([Id]),
	    CONSTRAINT [FK_Transection_User_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[User] ([Id]),
	    CONSTRAINT [FK_Transection_User_DeletedBy] FOREIGN KEY ([DeletedBy]) REFERENCES [dbo].[User] ([Id]),
	    CONSTRAINT [FK_Transection_User_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [dbo].[User] ([Id])
	);
END
GO

-- Reservation Table
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Reservation' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[Reservation] (
	    [Id]                      BIGINT             IDENTITY (1, 1) NOT NULL,
	    [UserId]                  BIGINT             NOT NULL,
	    [BookId]                  BIGINT             NOT NULL,
	    [StatusId]                BIGINT             NOT NULL,
	    [ReservationDate]         DATETIMEOFFSET (7) CONSTRAINT [DF_Reservation_ReservationDate] DEFAULT (getdate()) NOT NULL,
	    [AllocateAfter]           DATETIMEOFFSET (7) CONSTRAINT [DF_Reservation_AllocateAfter] DEFAULT (getdate()) NOT NULL,
	    [IsAllocated]             BIT                CONSTRAINT [DF_Reservation_IsAllocated] DEFAULT ((0)) NOT NULL,
	    [AllocatedAt]             DATETIMEOFFSET (7) NULL,
	    [TransferAllocationCount] INT                CONSTRAINT [DF_Reservation_TransferAllocationCount] DEFAULT ((0)) NOT NULL,
	    [CancelDate]              DATETIMEOFFSET (7) NULL, 
	    [CancelReason]            VARCHAR (255)      NULL,
	    [IsActive]                BIT                CONSTRAINT [DF_Reservation_IsActive] DEFAULT ((1)) NOT NULL,
	    [CreatedBy]               BIGINT             NOT NULL,
	    [CreatedAt]               DATETIMEOFFSET (7) CONSTRAINT [DF_Reservation_CreatedAt] DEFAULT (getdate()) NOT NULL,
	    [ModifiedBy]              BIGINT             NULL,
	    [ModifiedAt]              DATETIMEOFFSET (7) NULL,
	    [DeletedBy]               BIGINT             NULL,
	    [DeletedAt]               DATETIMEOFFSET (7) NULL,
	    CONSTRAINT [PK_Reservation] PRIMARY KEY CLUSTERED ([Id] ASC),
	    CONSTRAINT [FK_Reservation_Book_BookId] FOREIGN KEY ([BookId]) REFERENCES [dbo].[Books] ([Id]),
	    CONSTRAINT [FK_Reservation_Status_StatusId] FOREIGN KEY ([StatusId]) REFERENCES [dbo].[Status] ([Id]),
	    CONSTRAINT [FK_Reservation_User_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[User] ([Id]),
	    CONSTRAINT [FK_Reservation_User_DeletedBy] FOREIGN KEY ([DeletedBy]) REFERENCES [dbo].[User] ([Id]),
	    CONSTRAINT [FK_Reservation_User_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [dbo].[User] ([Id])
	);
END
GO

-- Penalty Table
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Penalty' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[Penalty] (
	    [Id]            BIGINT             IDENTITY (1, 1) NOT NULL,
	    [UserId]        BIGINT             NOT NULL,
	    [TransectionId] BIGINT             NULL,
	    [StatusId]      BIGINT             NOT NULL,
	    [PenaltyTypeId] BIGINT             NOT NULL,
	    [Description]   NVARCHAR (500)     NOT NULL,
	    [Amount]        DECIMAL (18, 2)    NOT NULL,
	    [OverDueDays]   INT                NULL,
	    [IsActive]      BIT                CONSTRAINT [DF_Penalty_IsActive] DEFAULT ((1)) NOT NULL,
	    [CreatedBy]     BIGINT             NOT NULL,
	    [CreatedAt]     DATETIMEOFFSET (7) CONSTRAINT [DF_Penalty_CreatedAt] DEFAULT (getdate()) NOT NULL,
	    [ModifiedBy]    BIGINT             NULL,
	    [ModifiedAt]    DATETIMEOFFSET (7) NULL,
	    [DeletedBy]     BIGINT             NULL,
	    [DeletedAt]     DATETIMEOFFSET (7) NULL,
	    CONSTRAINT [PK_Penalty] PRIMARY KEY CLUSTERED ([Id] ASC),
	    CONSTRAINT [FK_Penalty_PenaltyType_PenaltyTypeId] FOREIGN KEY ([PenaltyTypeId]) REFERENCES [dbo].[PenaltyType] ([Id]),
	    CONSTRAINT [FK_Penalty_Status_StatusId] FOREIGN KEY ([StatusId]) REFERENCES [dbo].[Status] ([Id]),
	    CONSTRAINT [FK_Penalty_User_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[User] ([Id]),
	    CONSTRAINT [FK_Penalty_User_DeletedBy] FOREIGN KEY ([DeletedBy]) REFERENCES [dbo].[User] ([Id]),
	    CONSTRAINT [FK_Penalty_User_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [dbo].[User] ([Id]),
	    CONSTRAINT [FK_Penalty_User_TransectionId] FOREIGN KEY ([TransectionId]) REFERENCES [dbo].[Transection] ([Id])
	);
END
GO

-- Email Outbox Table
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'OutboxMessages' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[OutboxMessages](
		[Id]			BIGINT				IDENTITY(1, 1) NOT NULL,
		[Type]			NVARCHAR (50)		NOT NULL,
		[Payload]		NVARCHAR (MAX)		NOT NULL,
		[RetryCount]	INT					NOT NULL,
		[CreatedAt]		DATETIMEOFFSET (7)	NOT NULL,
		[CreatedBy]		BIGINT				NOT NULL,
		[IsProcessed]	BIT					NOT NULL,
		[ProcessedAt]	DATETIMEOFFSET (7)	NULL,
		[NextAttemptAt] DATETIMEOFFSET (7)	NULL,
	    [IsActive]      BIT                 CONSTRAINT [DF_OutBoxMessages_IsActive] DEFAULT ((1)) NOT NULL,
	    CONSTRAINT [PK_OutboxMessages] PRIMARY KEY CLUSTERED ([Id] ASC),
	    CONSTRAINT [FK_OutboxMessages_User_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[User] ([Id])
	);
END
GO

-- ============================================================================
-- SECTION 4: CREATE LOGGING TABLES (in logging schema)
-- ============================================================================

-- BookFileMapping Log Table
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'BookFileMappingLog' AND schema_id = SCHEMA_ID('logging'))
BEGIN
    CREATE TABLE [logging].[BookFileMappingLog] (
	    [SerialNumber] BIGINT             IDENTITY (1, 1) NOT NULL,
	    [Operation]    VARCHAR (100)      NULL,
	    [LogType]      VARCHAR (100)      NULL,
	    [LogTime]      DATETIMEOFFSET (7) NULL,
	    [Id]           BIGINT             NULL,
	    [BookId]       BIGINT             NULL,
	    [Label]        NVARCHAR (50)      NULL,
	    [FileLocation] NVARCHAR (MAX)     NULL,
	    [IsActive]     BIT                NULL,
	    [CreatedBy]    BIGINT             NULL,
	    [CreatedAt]    DATETIMEOFFSET (7) NULL,
	    [ModifiedBy]   BIGINT             NULL,
	    [ModifiedAt]   DATETIMEOFFSET (7) NULL,
	    [DeletedBy]    BIGINT             NULL,
	    [DeletedAt]    DATETIMEOFFSET (7) NULL,
	    CONSTRAINT [PK_BookFileMappingLog] PRIMARY KEY CLUSTERED ([SerialNumber] ASC)
	);
END
GO

-- Book Log Table
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'BookFileMappingLog' AND schema_id = SCHEMA_ID('logging'))
BEGIN
    CREATE TABLE [logging].[BooksLog](
		[SerialNumber] [bigint] IDENTITY(1,1) NOT NULL,
		[Operation] [varchar](100) NULL,
		[LogType] [varchar](100) NULL,
		[LogTime] [datetimeoffset](7) NULL,
		[Id] [bigint] NULL,
		[GenreId] [bigint] NULL,
		[StatusId] [bigint] NULL,
		[Title] [nvarchar](255) NULL,
		[BookDescription] [nvarchar](max) NULL,
		[Price] [decimal](18, 2) NULL,
		[ISBN] [nvarchar](50) NULL,
		[Author] [nvarchar](255) NULL,
		[AuthorDescription] [nvarchar](max) NULL,
		[Publisher] [nvarchar](255) NULL,
		[PublishAt] [datetimeoffset](7) NULL,
		[TotalCopies] [bigint] NULL,
		[AvailableCopies] [bigint] NULL,
		[IsActive] [bit] NULL,
		[CreatedBy] [bigint] NULL,
		[CreatedAt] [datetimeoffset](7) NULL,
		[ModifiedBy] [bigint] NULL,
		[ModifiedAt] [datetimeoffset](7) NULL,
		[DeletedBy] [bigint] NULL,
		[DeletedAt] [datetimeoffset](7) NULL,
	    CONSTRAINT [PK_BooksLog] PRIMARY KEY CLUSTERED ([SerialNumber] ASC)
	);

	IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_BooksLog_LogTime' AND object_id = OBJECT_ID('[logging].[BooksLog]'))
    BEGIN
        EXEC sp_executesql N'CREATE NONCLUSTERED INDEX IX_BooksLog_LogTime ON [logging].[BooksLog](LogTime DESC);'
    END

	IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_BooksLog_EntityId_LogTime' AND object_id = OBJECT_ID('[logging].[BooksLog]'))
    BEGIN
        EXEC sp_executesql N'CREATE NONCLUSTERED INDEX IX_BooksLog_EntityId_LogTime ON [logging].[BooksLog](Id, LogTime DESC);'
    END
END
GO

-- Communication Log Table
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CommunicationLog' AND schema_id = SCHEMA_ID('logging'))
BEGIN
    CREATE TABLE [logging].[CommunicationLog](
		[Id] [bigint] IDENTITY(1,1) NOT NULL,
		[Message] [nvarchar](max) NULL,
		[MessageTemplate] [nvarchar](max) NULL,
		[Level] [nvarchar](max) NULL,
		[TimeStamp] [datetime] NULL,
		[LogEvent] [nvarchar](max) NULL,
		[UserName] [nvarchar](max) NULL,
		[DeliveryMethod] [nvarchar](max) NULL,
		[DeliveryStatus] [nvarchar](max) NULL,
		CONSTRAINT [PK_CommunicationLog] PRIMARY KEY CLUSTERED ([Id] ASC)
	);
END
GO

-- Configs Log Table
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ConfigsLog' AND schema_id = SCHEMA_ID('logging'))
BEGIN
    CREATE TABLE [logging].[ConfigsLog] (
	    [SerialNumber] BIGINT             IDENTITY (1, 1) NOT NULL,
		[Operation]    VARCHAR (100)      NULL,
		[LogType]      VARCHAR (100)      NULL,
		[LogTime]      DATETIMEOFFSET (7) NULL,
		[Id]           BIGINT             NULL,
		[KeyName]      NVARCHAR (50)      NULL,
		[KeyValue]     NVARCHAR (1000)    NULL,
		[Description]  NVARCHAR (500)     NULL,
		[IsActive]     BIT                NULL,
		[CreatedBy]    BIGINT             NULL,
		[CreatedAt]    DATETIMEOFFSET (7) NULL,
		[ModifiedBy]   BIGINT             NULL,
		[ModifiedAt]   DATETIMEOFFSET (7) NULL,
		[DeletedBy]    BIGINT             NULL,
		[DeletedAt]    DATETIMEOFFSET (7) NULL,
		CONSTRAINT [PK_ConfigsLog] PRIMARY KEY CLUSTERED ([SerialNumber] ASC)
	);

	IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ConfigsLog_LogTime' AND object_id = OBJECT_ID('[logging].[ConfigsLog]'))
    BEGIN
        EXEC sp_executesql N'CREATE NONCLUSTERED INDEX IX_ConfigsLog_LogTime ON [logging].[ConfigsLog](LogTime DESC);'
    END
	
	IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ConfigsLog_EntityId_LogTime' AND object_id = OBJECT_ID('[logging].[ConfigsLog]'))
    BEGIN
        EXEC sp_executesql N'CREATE NONCLUSTERED INDEX IX_ConfigsLog_EntityId_LogTime ON [logging].[ConfigsLog](Id, LogTime DESC);'
    END
END
GO

-- Genre Log Table
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'GenreLog' AND schema_id = SCHEMA_ID('logging'))
BEGIN
    CREATE TABLE [logging].[GenreLog] (
	    [SerialNumber] BIGINT             IDENTITY (1, 1) NOT NULL,
	    [Operation]    VARCHAR (100)      NULL,
	    [LogType]      VARCHAR (100)      NULL,
	    [LogTime]      DATETIMEOFFSET (7) NULL,
	    [Id]           BIGINT             NULL,
	    [Name]         NVARCHAR (100)     NULL,
	    [Description]  NVARCHAR (500)     NULL,
	    [IsActive]     BIT                NULL,
	    [CreatedBy]    BIGINT             NULL,
	    [CreatedAt]    DATETIMEOFFSET (7) NULL,
	    [ModifiedBy]   BIGINT             NULL,
	    [ModifiedAt]   DATETIMEOFFSET (7) NULL,
	    [DeletedBy]    BIGINT             NULL,
	    [DeletedAt]    DATETIMEOFFSET (7) NULL,
	    CONSTRAINT [PK_GenreLog] PRIMARY KEY CLUSTERED ([SerialNumber] ASC)
	);

	IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_GenreLog_LogTime' AND object_id = OBJECT_ID('[logging].[GenreLog]'))
    BEGIN
        EXEC sp_executesql N'CREATE NONCLUSTERED INDEX IX_GenreLog_LogTime ON [logging].[GenreLog](LogTime DESC);'
    END

	IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_GenreLog_EntityId_LogTime' AND object_id = OBJECT_ID('[logging].[GenreLog]'))
    BEGIN
        EXEC sp_executesql N'CREATE NONCLUSTERED INDEX IX_GenreLog_EntityId_LogTime ON [logging].[GenreLog](Id, LogTime DESC);'
    END
END
GO

-- Membership Log Table
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'MembershipLog' AND schema_id = SCHEMA_ID('logging'))
BEGIN
    CREATE TABLE [logging].[MembershipLog] (
	    [SerialNumber]     BIGINT             IDENTITY (1, 1) NOT NULL,
	    [Operation]        VARCHAR (100)      NULL,
	    [LogType]          VARCHAR (100)      NULL,
	    [LogTime]          DATETIMEOFFSET (7) NULL,
	    [Id]               BIGINT             NULL,
	    [Type]             NVARCHAR (255)     NULL,
	    [Description]      NVARCHAR (500)     NULL,
	    [BorrowLimit]      BIGINT             NULL,
	    [ReservationLimit] BIGINT             NULL,
	    [Duration]         BIGINT             NULL,
	    [Cost]             DECIMAL (10, 2)    NULL,
	    [Discount]         DECIMAL (10, 2)    NULL,
	    [IsActive]         BIT                NULL,
	    [CreatedBy]        BIGINT             NULL,
	    [CreatedAt]        DATETIMEOFFSET (7) NULL,
	    [ModifiedBy]       BIGINT             NULL,
	    [ModifiedAt]       DATETIMEOFFSET (7) NULL,
	    [DeletedBy]        BIGINT             NULL,
	    [DeletedAt]        DATETIMEOFFSET (7) NULL,
	    CONSTRAINT [PK_MembershipLog] PRIMARY KEY CLUSTERED ([SerialNumber] ASC)
	);

	IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_MembershipLog_LogTime' AND object_id = OBJECT_ID('[logging].[MembershipLog]'))
    BEGIN
        EXEC sp_executesql N'CREATE NONCLUSTERED INDEX IX_MembershipLog_LogTime ON [logging].[MembershipLog](LogTime DESC);'
    END

	IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_MembershipLog_EntityId_LogTime' AND object_id = OBJECT_ID('[logging].[MembershipLog]'))
    BEGIN
        EXEC sp_executesql N'CREATE NONCLUSTERED INDEX IX_MembershipLog_EntityId_LogTime ON [logging].[MembershipLog](Id, LogTime DESC);'
    END
END
GO

-- OutboxMessages Log Table
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'OutboxMessagesLog' AND schema_id = SCHEMA_ID('logging'))
BEGIN
    CREATE TABLE [logging].[OutboxMessagesLog](
		[SerialNumber]     BIGINT             IDENTITY (1, 1) NOT NULL,
	    [Operation]        VARCHAR (100)      NULL,
	    [LogType]          VARCHAR (100)      NULL,
	    [LogTime]          DATETIMEOFFSET (7) NULL,
		[Id]			   BIGINT			  NULL,
		[Type]			   NVARCHAR (50)	  NULL,
		[Payload]		   NVARCHAR (MAX)	  NULL,
		[RetryCount]	   INT				  NULL,
		[CreatedAt]		   DATETIMEOFFSET (7) NULL,
		[CreatedBy]		   BIGINT			  NULL,
		[IsProcessed]	   BIT				  NULL,
		[ProcessedAt]	   DATETIMEOFFSET (7) NULL,
		[NextAttemptAt]	   DATETIMEOFFSET (7) NULL,
	    [IsActive]         BIT                NULL,
	    CONSTRAINT [PK_OutboxMessagesLog] PRIMARY KEY CLUSTERED ([SerialNumber] ASC)
	);
END
GO

-- Penalty Log Table
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PenaltyLog' AND schema_id = SCHEMA_ID('logging'))
BEGIN
    CREATE TABLE [logging].[PenaltyLog] (
	    [SerialNumber]  BIGINT             IDENTITY (1, 1) NOT NULL,
	    [Operation]     VARCHAR (100)      NULL,
	    [LogType]       VARCHAR (100)      NULL,
	    [LogTime]       DATETIMEOFFSET (7) NULL,
	    [Id]            BIGINT             NULL,
	    [UserId]        BIGINT             NULL,
	    [TransectionId] BIGINT             NULL,
	    [StatusId]      BIGINT             NULL,
	    [PenaltyTypeId] BIGINT             NULL,
	    [Description]   NVARCHAR (500)     NULL,
	    [Amount]        DECIMAL (18, 2)    NULL,
	    [OverDueDays]   INT                NULL,
	    [IsActive]      BIT                NULL,
	    [CreatedBy]     BIGINT             NULL,
	    [CreatedAt]     DATETIMEOFFSET (7) NULL,
	    [ModifiedBy]    BIGINT             NULL,
	    [ModifiedAt]    DATETIMEOFFSET (7) NULL,
	    [DeletedBy]     BIGINT             NULL,
	    [DeletedAt]     DATETIMEOFFSET (7) NULL,
	    CONSTRAINT [PK_PenaltyLog] PRIMARY KEY CLUSTERED ([SerialNumber] ASC)
	);

	IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_PenaltyLog_LogTime' AND object_id = OBJECT_ID('[logging].[PenaltyLog]'))
    BEGIN
        EXEC sp_executesql N'CREATE NONCLUSTERED INDEX IX_PenaltyLog_LogTime ON [logging].[PenaltyLog](LogTime DESC);'
    END

	IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_PenaltyLog_EntityId_LogTime' AND object_id = OBJECT_ID('[logging].[PenaltyLog]'))
    BEGIN
        EXEC sp_executesql N'CREATE NONCLUSTERED INDEX IX_PenaltyLog_EntityId_LogTime ON [logging].[PenaltyLog](Id, LogTime DESC);'
    END
END
GO

-- PenaltyType Log Table
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PenaltyTypeLog' AND schema_id = SCHEMA_ID('logging'))
BEGIN
    CREATE TABLE [logging].[PenaltyTypeLog] (
	    [SerialNumber] BIGINT             IDENTITY (1, 1) NOT NULL,
	    [Operation]    VARCHAR (100)      NULL,
	    [LogType]      VARCHAR (100)      NULL,
	    [LogTime]      DATETIMEOFFSET (7) NULL,
	    [Id]           BIGINT             NULL,
	    [Label]        NVARCHAR (50)      NULL,
	    [Description]  NVARCHAR (500)     NULL,
	    [IsActive]     BIT                NULL,
	    [CreatedBy]    BIGINT             NULL,
	    [CreatedAt]    DATETIMEOFFSET (7) NULL,
	    [ModifiedBy]   BIGINT             NULL,
	    [ModifiedAt]   DATETIMEOFFSET (7) NULL,
	    [DeletedBy]    BIGINT             NULL,
	    [DeletedAt]    DATETIMEOFFSET (7) NULL,
	    CONSTRAINT [PK_PenaltyTypeLog] PRIMARY KEY CLUSTERED ([SerialNumber] ASC)
	);
END
GO

-- Reservation Log Table
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ReservationLog' AND schema_id = SCHEMA_ID('logging'))
BEGIN
    CREATE TABLE [logging].[ReservationLog] (
	    [SerialNumber]            BIGINT             IDENTITY (1, 1) NOT NULL,
	    [Operation]               VARCHAR (100)      NULL,
	    [LogType]                 VARCHAR (100)      NULL,
	    [LogTime]                 DATETIMEOFFSET (7) NULL,
	    [Id]                      BIGINT             NULL,
	    [UserId]                  BIGINT             NULL,
	    [BookId]                  BIGINT             NULL,
	    [StatusId]                BIGINT             NULL,
	    [ReservationDate]         DATETIMEOFFSET (7) NULL,
	    [AllocateAfter]           DATETIMEOFFSET (7) NULL,
	    [IsAllocated]             BIT                NULL,
	    [AllocatedAt]             DATETIMEOFFSET (7) NULL,
	    [TransferAllocationCount] INT                NOT NULL,
	    [CancelDate]              DATETIMEOFFSET (7) NULL, 
	    [CancelReason]            VARCHAR (255)      NULL,
	    [IsActive]                BIT                NULL,
	    [CreatedBy]               BIGINT             NULL,
	    [CreatedAt]               DATETIMEOFFSET (7) NULL,
	    [ModifiedBy]              BIGINT             NULL,
	    [ModifiedAt]              DATETIMEOFFSET (7) NULL,
	    [DeletedBy]               BIGINT             NULL,
	    [DeletedAt]               DATETIMEOFFSET (7) NULL,
	    CONSTRAINT [PK_ReservationLog] PRIMARY KEY CLUSTERED ([SerialNumber] ASC)
	);

	IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ReservationLog_LogTime' AND object_id = OBJECT_ID('[logging].[ReservationLog]'))
    BEGIN
        EXEC sp_executesql N'CREATE NONCLUSTERED INDEX IX_ReservationLog_LogTime ON [logging].[ReservationLog](LogTime DESC);'
    END

	IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ReservationLog_EntityId_LogTime' AND object_id = OBJECT_ID('[logging].[ReservationLog]'))
    BEGIN
        EXEC sp_executesql N'CREATE NONCLUSTERED INDEX IX_ReservationLog_EntityId_LogTime ON [logging].[ReservationLog](Id, LogTime DESC);'
    END
END
GO

-- RoleList Log Table
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'RoleListLog' AND schema_id = SCHEMA_ID('logging'))
BEGIN
    CREATE TABLE [logging].[RoleListLog] (
	    [SerialNumber] BIGINT             IDENTITY (1, 1) NOT NULL,
	    [Operation]    VARCHAR (100)      NULL,
	    [LogType]      VARCHAR (100)      NULL,
	    [LogTime]      DATETIMEOFFSET (7) NULL,
	    [Id]           BIGINT             NULL,
	    [Label]        NVARCHAR (100)     NULL,
	    [Description]  NVARCHAR (500)     NULL,
	    [IsActive]     BIT                NULL,
	    [CreatedBy]    BIGINT             NULL,
	    [CreatedAt]    DATETIMEOFFSET (7) NULL,
	    [ModifiedBy]   BIGINT             NULL,
	    [ModifiedAt]   DATETIMEOFFSET (7) NULL,
	    [DeletedBy]    BIGINT             NULL,
	    [DeletedAt]    DATETIMEOFFSET (7) NULL,
	    CONSTRAINT [PK_RoleListLog] PRIMARY KEY CLUSTERED ([SerialNumber] ASC)
	);
END
GO

-- Status Log Table
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'StatusLog' AND schema_id = SCHEMA_ID('logging'))
BEGIN
    CREATE TABLE [logging].[StatusLog] (
	    [SerialNumber] BIGINT             IDENTITY (1, 1) NOT NULL,
	    [Operation]    VARCHAR (100)      NULL,
	    [LogType]      VARCHAR (100)      NULL,
	    [LogTime]      DATETIMEOFFSET (7) NULL,
	    [Id]           BIGINT             NULL,
	    [StatusTypeId] BIGINT             NULL,
	    [Label]        NVARCHAR (50)      NULL,
	    [Description]  NVARCHAR (500)     NULL,
	    [Color]        NVARCHAR (50)      NULL,
	    [IsActive]     BIT                NULL,
	    [CreatedBy]    BIGINT             NULL,
	    [CreatedAt]    DATETIMEOFFSET (7) NULL,
	    [ModifiedBy]   BIGINT             NULL,
	    [ModifiedAt]   DATETIMEOFFSET (7) NULL,
	    [DeletedBy]    BIGINT             NULL,
	    [DeletedAt]    DATETIMEOFFSET (7) NULL,
	    CONSTRAINT [PK_StatusLog] PRIMARY KEY CLUSTERED ([SerialNumber] ASC)
	);
END
GO

-- Status Type Log Table
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'StatusTypeLog' AND schema_id = SCHEMA_ID('logging'))
BEGIN
    CREATE TABLE [logging].[StatusTypeLog] (
	    [SerialNumber] BIGINT             IDENTITY (1, 1) NOT NULL,
	    [Operation]    VARCHAR (100)      NULL,
	    [LogType]      VARCHAR (100)      NULL,
	    [LogTime]      DATETIMEOFFSET (7) NULL,
	    [Id]           BIGINT             NULL,
	    [Label]        NVARCHAR (50)      NULL,
	    [Description]  NVARCHAR (500)     NULL,
	    [IsActive]     BIT                NULL,
	    CONSTRAINT [PK_StatusTypeLog] PRIMARY KEY CLUSTERED ([SerialNumber] ASC)
	);
END
GO

-- Transection Log Table
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TransectionLog' AND schema_id = SCHEMA_ID('logging'))
BEGIN
    CREATE TABLE [logging].[TransectionLog] (
	    [SerialNumber]  BIGINT             IDENTITY (1, 1) NOT NULL,
	    [Operation]     VARCHAR (100)      NULL,
	    [LogType]       VARCHAR (100)      NULL,
	    [LogTime]       DATETIMEOFFSET (7) NULL,
	    [Id]            BIGINT             NULL,
	    [UserId]        BIGINT             NULL,
	    [BookId]        BIGINT             NULL,
	    [StatusId]      BIGINT             NULL,
	    [BorrowDate]    DATETIMEOFFSET (7) NULL,
	    [RenewCount]    INT                NULL,
	    [RenewDate]     DATETIMEOFFSET (7) NULL,
	    [DueDate]       DATETIMEOFFSET (7) NULL,
	    [ReturnDate]    DATETIMEOFFSET (7) NULL,
	    [LostClaimDate] DATETIMEOFFSET (7) NULL,
	    [IsActive]      BIT                NULL,
	    [CreatedBy]     BIGINT             NULL,
	    [CreatedAt]     DATETIMEOFFSET (7) NULL,
	    [ModifiedBy]    BIGINT             NULL,
	    [ModifiedAt]    DATETIMEOFFSET (7) NULL,
	    [DeletedBy]     BIGINT             NULL,
	    [DeletedAt]     DATETIMEOFFSET (7) NULL,
	    CONSTRAINT [PK_TransectionLog] PRIMARY KEY CLUSTERED ([SerialNumber] ASC)
	);

	IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_TransectionLog_LogTime' AND object_id = OBJECT_ID('[logging].[TransectionLog]'))
    BEGIN
        EXEC sp_executesql N'CREATE NONCLUSTERED INDEX IX_TransectionLog_LogTime ON [logging].[TransectionLog](LogTime DESC);'
    END

	IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_TransectionLog_EntityId_LogTime' AND object_id = OBJECT_ID('[logging].[TransectionLog]'))
    BEGIN
        EXEC sp_executesql N'CREATE NONCLUSTERED INDEX IX_TransectionLog_EntityId_LogTime ON [logging].[TransectionLog](Id, LogTime DESC);'
    END
END
GO

-- User Log Table
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'UserLog' AND schema_id = SCHEMA_ID('logging'))
BEGIN
    CREATE TABLE [logging].[UserLog] (
	    [SerialNumber]      BIGINT             IDENTITY (1, 1) NOT NULL,
	    [Operation]         VARCHAR (100)      NULL,
	    [LogType]           VARCHAR (100)      NULL,
	    [LogTime]           DATETIMEOFFSET (7) NULL,
	    [Id]                BIGINT             NULL,
	    [RoleId]            BIGINT             NULL,
	    [FirstName]         VARCHAR (100)      NULL,
	    [MiddleName]        VARCHAR (100)      NULL,
	    [LastName]          VARCHAR (100)      NULL,
	    [DOB]               DATETIMEOFFSET (7) NULL,
	    [Gender]            VARCHAR (50)       NULL,
	    [Address]           NVARCHAR (255)     NULL,
	    [MobileNo]          VARCHAR (12)       NULL,
	    [Email]             NVARCHAR (255)     NULL,
	    [Username]          NVARCHAR (50)      NULL,
	    [PasswordHash]      VARBINARY (MAX)    NULL,
	    [PasswordSolt]      VARBINARY (MAX)    NULL,
	    [ProfilePhoto]      NVARCHAR (MAX)     NULL,
	    [LibraryCardNumber] NVARCHAR (50)      NULL,
	    [JoiningDate]       DATETIMEOFFSET (7) NULL,
	    [IsActive]          BIT                NULL,
	    [CreatedBy]         BIGINT             NULL,
	    [CreatedAt]         DATETIMEOFFSET (7) NULL,
	    [ModifiedBy]        BIGINT             NULL,
	    [ModifiedAt]        DATETIMEOFFSET (7) NULL,
	    [DeletedBy]         BIGINT             NULL,
	    [DeletedAt]         DATETIMEOFFSET (7) NULL,
	    CONSTRAINT [PK_UserLog] PRIMARY KEY CLUSTERED ([SerialNumber] ASC)
	);

	IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_UserLog_LogTime' AND object_id = OBJECT_ID('[logging].[UserLog]'))
    BEGIN
        EXEC sp_executesql N'CREATE NONCLUSTERED INDEX IX_UserLog_LogTime ON [logging].[UserLog](LogTime DESC);'
    END

	IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_UserLog_EntityId_LogTime' AND object_id = OBJECT_ID('[logging].[UserLog]'))
    BEGIN
        EXEC sp_executesql N'CREATE NONCLUSTERED INDEX IX_UserLog_EntityId_LogTime ON [logging].[UserLog](Id, LogTime DESC);'
    END
END
GO

-- UserMembershipMapping Log Table
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'UserMembershipMappingLog' AND schema_id = SCHEMA_ID('logging'))
BEGIN
    CREATE TABLE [logging].[UserMembershipMappingLog] (
	    [SerialNumber]       BIGINT             IDENTITY (1, 1) NOT NULL,
	    [Operation]          VARCHAR (100)      NULL,
	    [LogType]            VARCHAR (100)      NULL,
	    [LogTime]            DATETIMEOFFSET (7) NULL,
	    [Id]                 BIGINT             NULL,
	    [UserId]             BIGINT             NULL,
	    [MembershipId]       BIGINT             NULL,
	    [EffectiveStartDate] DATETIMEOFFSET (7) NULL,
	    [ExpirationDate]     DATETIMEOFFSET (7) NULL,
	    [BorrowLimit]        BIGINT             NULL,
	    [ReservationLimit]   BIGINT             NULL,
	    [MembershipCost]     DECIMAL (10, 2)    NULL,
	    [Discount]           DECIMAL (10, 2)    NULL,
	    [PaidAmount]         DECIMAL (10, 2)    NULL,
	    [IsActive]           BIT                NULL,
	    [CreatedBy]          BIGINT             NULL,
	    [CreatedAt]          DATETIMEOFFSET (7) NULL,
	    [ModifiedBy]         BIGINT             NULL,
	    [ModifiedAt]         DATETIMEOFFSET (7) NULL,
	    [DeletedBy]          BIGINT             NULL,
	    [DeletedAt]          DATETIMEOFFSET (7) NULL,
	    CONSTRAINT [PK_UserMembershipMappingLog] PRIMARY KEY CLUSTERED ([SerialNumber] ASC)
	);

	IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_UserMembershipMappingLog_LogTime' AND object_id = OBJECT_ID('[logging].[UserMembershipMappingLog]'))
    BEGIN
        EXEC sp_executesql N'CREATE NONCLUSTERED INDEX IX_UserMembershipMappingLog_LogTime ON [logging].[UserMembershipMappingLog](LogTime DESC);'
    END

	IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_UserMembershipMappingLog_EntityId_LogTime' AND object_id = OBJECT_ID('[logging].[UserMembershipMappingLog]'))
    BEGIN
        EXEC sp_executesql N'CREATE NONCLUSTERED INDEX IX_UserMembershipMappingLog_EntityId_LogTime ON [logging].[UserMembershipMappingLog](Id, LogTime DESC);'
    END
END
GO

-- System Log Table
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SystemLog' AND schema_id = SCHEMA_ID('logging'))
BEGIN
    CREATE TABLE [logging].[SystemLog] (
	    [Id]              BIGINT            IDENTITY (1, 1) NOT NULL,
	    [Message]         NVARCHAR (MAX) NULL,
	    [MessageTemplate] NVARCHAR (MAX) NULL,
	    [Level]           NVARCHAR (MAX) NULL,
	    [TimeStamp]       DATETIME       NULL,
	    [Exception]       NVARCHAR (MAX) NULL,
	    [MethodType]      NVARCHAR (MAX) NULL,
	    [Origin]          NVARCHAR (MAX) NULL,
	    [Platform]        NVARCHAR (MAX) NULL,
	    [Path]            NVARCHAR (MAX) NULL,
	    [UserAgent]       NVARCHAR (MAX) NULL,
	    [UserName]        NVARCHAR (MAX) NULL,
	    [ServerName]      NVARCHAR (MAX) NULL,
	    CONSTRAINT [PK_SystemLog] PRIMARY KEY CLUSTERED ([Id] ASC)
	);
END
GO

-- Exception Log Table
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ExceptionLog' AND schema_id = SCHEMA_ID('logging'))
BEGIN
    CREATE TABLE [logging].[ExceptionLog] (
	    [Id]             BIGINT             IDENTITY (1, 1) NOT NULL,
	    [UserId]         BIGINT             NULL,
	    [MachineIp]      VARCHAR (45)       NULL,
	    [Type]           NVARCHAR (50)      NULL,
	    [Message]        NVARCHAR (4000)    NULL,
	    [ErrorNumber]    INT                NULL,
	    [ErrorProcedure] NVARCHAR (128)     NULL,
	    [ErrorLine]      INT                NULL,
	    [ErrorTime]      DATETIMEOFFSET (7) NULL,
	    CONSTRAINT [PK_ExceptionLog] PRIMARY KEY CLUSTERED ([Id] ASC)
	);
END
GO

-- ============================================================================
-- SECTION 5: CREATE VIEWS
-- ============================================================================

-- BookPopularity View
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Purpose: Most popular books overall.

CREATE OR ALTER VIEW [dbo].[BookPopularity] AS
WITH StatusIds AS (
    SELECT 
        [dbo].[udfGetStatusNumber]('Transaction', 'Cancelled')   AS TransectionCancelled,
        [dbo].[udfGetStatusNumber]('Transaction', 'Borrowed')    AS TransectionBorrowed,
        [dbo].[udfGetStatusNumber]('Transaction', 'Returned')    AS TransectionReturned,
        [dbo].[udfGetStatusNumber]('Transaction', 'Overdue')     AS TransectionOverdue,
        [dbo].[udfGetStatusNumber]('Transaction', 'Renewed')     AS TransectionRenewed,
        [dbo].[udfGetStatusNumber]('Transaction', 'ClaimedLost') AS TransectionClaimedLost,

        [dbo].[udfGetStatusNumber]('Reservations', 'Cancelled')   AS ReservationCancelled,
        [dbo].[udfGetStatusNumber]('Reservations', 'Reserved')    AS ReservationReserved,
        [dbo].[udfGetStatusNumber]('Reservations', 'Allocated')   AS ReservationAllocated,
        [dbo].[udfGetStatusNumber]('Reservations', 'Fulfilled')   AS ReservationFulfilled
)
SELECT 
    b.Id AS BookId, 
    b.Title,
    b.Author AS AuthorName,
    b.GenreId,
    g.Name AS GenreName,

    -- Popularity signals
    COUNT(CASE WHEN t.StatusId IN (s.TransectionBorrowed, s.TransectionReturned, s.TransectionOverdue, s.TransectionRenewed, s.TransectionClaimedLost) THEN 1 END) AS BorrowCount, 
    COUNT(CASE WHEN r.StatusId IN (s.ReservationReserved, s.ReservationAllocated, s.ReservationFulfilled) THEN 1 END) AS ReservationCount, 
    MAX(t.BorrowDate) AS LastBorrowDate, 
    MAX(r.AllocatedAt) AS LastAllocationDate,
    AVG(DATEDIFF(DAY, t.BorrowDate, ISNULL(t.ReturnDate, GETDATE()))) AS AverageBorrowDuration,

    -- Penalty signals
    COUNT(p.Id) AS PenaltyCount,
    SUM(ISNULL(p.Amount,0)) AS TotalPenaltyAmount,
    CAST(COUNT(p.Id) AS FLOAT) / NULLIF(COUNT(CASE WHEN t.StatusId IN (s.TransectionBorrowed, s.TransectionReturned, s.TransectionOverdue, s.TransectionRenewed, s.TransectionClaimedLost) THEN 1 END),0) AS PenaltyRate,

    -- Availability signals
    b.AvailableCopies,
    CAST(b.AvailableCopies AS FLOAT) / NULLIF(b.TotalCopies,0) AS AvailabilityRatio,

    -- Queue signals
    COUNT(CASE WHEN r.StatusId = s.ReservationReserved THEN 1 END) AS QueueLength,

    -- Composite score
    (
        COUNT(CASE WHEN t.StatusId IN (s.TransectionBorrowed, s.TransectionReturned, s.TransectionOverdue, s.TransectionRenewed, s.TransectionClaimedLost) THEN 1 END) * 1
        + SUM(CASE WHEN t.StatusId IN (s.TransectionBorrowed, s.TransectionReturned, s.TransectionOverdue, s.TransectionRenewed) THEN t.RenewCount ELSE 0 END) * 2
        + COUNT(CASE WHEN r.StatusId IN (s.ReservationReserved, s.ReservationAllocated, s.ReservationFulfilled) THEN 1 END) * 1.5
        + CASE WHEN DATEDIFF(DAY, MAX(t.BorrowDate), GETDATE()) <= 90 THEN 3 ELSE 0 END
        - COUNT(p.Id)
        - COUNT(CASE WHEN r.StatusId = s.ReservationReserved THEN 1 END)
    ) AS Score
FROM [dbo].[Books] b 
LEFT JOIN [dbo].[Transection] t ON b.Id = t.BookId AND t.IsActive = 1
LEFT JOIN [dbo].[Reservation] r ON b.Id = r.BookId AND r.IsActive = 1
LEFT JOIN [dbo].[Penalty] p ON t.Id = p.TransectionId AND p.IsActive = 1
JOIN [dbo].[Genre] g ON b.GenreId = g.Id AND g.IsActive = 1
CROSS JOIN StatusIds s
WHERE b.IsActive = 1
GROUP BY 
    b.Id, b.Title, b.Author, b.GenreId, g.Name, b.AvailableCopies, b.TotalCopies;
GO

-- UserBookActivity View
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Purpose: Books based on the user’s own past activity.

CREATE OR ALTER VIEW [dbo].[UserBookActivity] AS
WITH StatusIds AS (
    SELECT 
        [dbo].[udfGetStatusNumber]('Transaction','Cancelled')   AS TransectionCancelled,
        [dbo].[udfGetStatusNumber]('Transaction','Borrowed')    AS TransectionBorrowed,
        [dbo].[udfGetStatusNumber]('Transaction','Returned')    AS TransectionReturned,
        [dbo].[udfGetStatusNumber]('Transaction','Overdue')     AS TransectionOverdue,
        [dbo].[udfGetStatusNumber]('Transaction','Renewed')     AS TransectionRenewed,
        [dbo].[udfGetStatusNumber]('Transaction','ClaimedLost') AS TransectionClaimedLost,

        [dbo].[udfGetStatusNumber]('Reservations','Cancelled')   AS ReservationCancelled,
        [dbo].[udfGetStatusNumber]('Reservations','Reserved')    AS ReservationReserved,
        [dbo].[udfGetStatusNumber]('Reservations','Allocated')   AS ReservationAllocated,
        [dbo].[udfGetStatusNumber]('Reservations','Fulfilled')   AS ReservationFulfilled
),
BorrowStats AS (
    SELECT 
        t.UserId,
        t.BookId,
        COUNT(CASE WHEN t.StatusId IN (s.TransectionBorrowed,s.TransectionReturned,s.TransectionOverdue,s.TransectionRenewed,s.TransectionClaimedLost) THEN 1 END) AS BorrowCount,
        COUNT(p.Id) AS PenaltyCount
    FROM [dbo].[Transection] t
    LEFT JOIN [dbo].[Penalty] p ON t.Id = p.TransectionId AND p.IsActive = 1
    CROSS JOIN StatusIds s
    WHERE t.IsActive = 1 AND t.StatusId <> s.TransectionCancelled
    GROUP BY t.UserId, t.BookId
)
-- Borrow activity
SELECT 
    t.UserId,
    b.Id AS BookId,
    b.Title,
    b.Author AS AuthorName,
    b.GenreId,
    g.Name AS GenreName,
    t.BorrowDate, 
    t.RenewDate, 
    t.ReturnDate,
    t.RenewCount,
    DATEDIFF(DAY, t.BorrowDate, ISNULL(t.ReturnDate, GETDATE())) AS BorrowDuration,

	-- Penalty signals
	bs.PenaltyCount,
    SUM(ISNULL(p.Amount,0)) AS TotalPenaltyAmount,
    CAST(bs.PenaltyCount AS FLOAT) / NULLIF(bs.BorrowCount,0) AS PenaltyRate,

	-- Availability signals
    b.AvailableCopies,
    CAST(b.AvailableCopies AS FLOAT) / NULLIF(b.TotalCopies,0) AS AvailabilityRatio,

	-- Queue signals
	0 AS QueueLength, -- queue only applies to reservations

    'Borrow' AS ActivityType,

	-- Composite score
    (
        1 
        + (t.RenewCount * 2) 
        + CASE WHEN t.ReturnDate IS NULL THEN 1 ELSE 0 END 
        + CASE WHEN DATEDIFF(DAY, t.BorrowDate, GETDATE()) <= 90 THEN 3 ELSE 0 END
        - COUNT(p.Id)
    ) AS Score
FROM [dbo].[Transection] t
JOIN BorrowStats bs ON t.UserId = bs.UserId AND t.BookId = bs.BookId
JOIN [dbo].[Books] b ON t.BookId = b.Id AND b.IsActive = 1
JOIN [dbo].[Genre] g ON b.GenreId = g.Id AND g.IsActive = 1
LEFT JOIN [dbo].[Penalty] p ON t.Id = p.TransectionId AND p.IsActive = 1
CROSS JOIN StatusIds s
WHERE t.IsActive = 1 
  AND t.StatusId <> s.TransectionCancelled
GROUP BY 
    t.UserId, b.Id, b.Title, b.Author, b.GenreId, g.Name,
    t.BorrowDate, t.RenewDate, t.ReturnDate, t.RenewCount,
    b.AvailableCopies, b.TotalCopies, bs.PenaltyCount, bs.BorrowCount

UNION

-- Reservation activity
SELECT 
    r.UserId,
    b.Id AS BookId,
    b.Title,
    b.Author AS AuthorName,
    b.GenreId,
    g.Name AS GenreName,
    r.ReservationDate AS BorrowDate,
    r.AllocatedAt AS RenewDate,
    NULL AS ReturnDate,
    0 AS RenewCount,
    NULL AS BorrowDuration,

	-- Penalty signals (not applicable for reservations)
    0 AS PenaltyCount,
    0 AS TotalPenaltyAmount,
    0 AS PenaltyRate,

	-- Availability signals
    b.AvailableCopies,
    CAST(b.AvailableCopies AS FLOAT) / NULLIF(b.TotalCopies,0) AS AvailabilityRatio,

	-- Queue signals
	COUNT(CASE WHEN r.StatusId = s.ReservationReserved THEN 1 END) AS QueueLength,

    'Reservation' AS ActivityType,

	-- Composite score
    (
        1 
        + CASE WHEN r.AllocatedAt IS NOT NULL THEN 2 ELSE 0 END 
        + CASE WHEN DATEDIFF(DAY, r.ReservationDate, GETDATE()) <= 90 THEN 3 ELSE 0 END
		- COUNT(CASE WHEN r.StatusId = s.ReservationReserved THEN 1 END)
    ) AS Score
FROM [dbo].[Reservation] r
JOIN [dbo].[Books] b ON r.BookId = b.Id AND b.IsActive = 1
JOIN [dbo].[Genre] g ON b.GenreId = g.Id AND g.IsActive = 1
CROSS JOIN StatusIds s
WHERE r.IsActive = 1 
  AND r.StatusId <> s.ReservationCancelled
GROUP BY 
    r.UserId, b.Id, b.Title, b.Author, b.GenreId, g.Name,
    r.ReservationDate, r.AllocatedAt,
    b.AvailableCopies, b.TotalCopies;
GO

-- UserMembershipActivity View
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Purpose: Books borrowed/reserved by users with the same membership type.

CREATE OR ALTER VIEW [dbo].[UserMembershipActivity] AS
WITH StatusIds AS (
    SELECT 
        [dbo].[udfGetStatusNumber]('Transaction','Cancelled')   AS TransectionCancelled,
        [dbo].[udfGetStatusNumber]('Transaction','Borrowed')    AS TransectionBorrowed,
        [dbo].[udfGetStatusNumber]('Transaction','Returned')    AS TransectionReturned,
        [dbo].[udfGetStatusNumber]('Transaction','Overdue')     AS TransectionOverdue,
        [dbo].[udfGetStatusNumber]('Transaction','Renewed')     AS TransectionRenewed,
        [dbo].[udfGetStatusNumber]('Transaction','ClaimedLost') AS TransectionClaimedLost,

        [dbo].[udfGetStatusNumber]('Reservations','Cancelled')   AS ReservationCancelled,
        [dbo].[udfGetStatusNumber]('Reservations','Reserved')    AS ReservationReserved,
        [dbo].[udfGetStatusNumber]('Reservations','Allocated')   AS ReservationAllocated,
        [dbo].[udfGetStatusNumber]('Reservations','Fulfilled')   AS ReservationFulfilled
),
BorrowStats AS (
    SELECT 
        t.UserId,
        um.MembershipId,
        COUNT(CASE WHEN t.StatusId IN (s.TransectionBorrowed,s.TransectionReturned,s.TransectionOverdue,s.TransectionRenewed,s.TransectionClaimedLost) THEN 1 END) AS BorrowCount,
        COUNT(p.Id) AS PenaltyCount
    FROM [dbo].[User] u
    JOIN [dbo].[UserMembershipMapping] um ON u.Id = um.UserId AND um.IsActive = 1
    JOIN [dbo].[Transection] t ON u.Id = t.UserId AND t.IsActive = 1
    LEFT JOIN [dbo].[Penalty] p ON t.Id = p.TransectionId AND p.IsActive = 1
    CROSS JOIN StatusIds s
    WHERE u.IsActive = 1 AND t.StatusId <> s.TransectionCancelled
    GROUP BY t.UserId, um.MembershipId
)
-- Borrow activity
SELECT
    u.Id AS UserId,
    um.MembershipId,
    m.Type AS MembershipTypeName,
    b.Id AS BookId,
    b.Title,
    b.Author AS AuthorName,
    b.GenreId,
    g.Name AS GenreName,
    t.BorrowDate, 
    t.RenewDate, 
    t.ReturnDate,
    t.RenewCount,
    DATEDIFF(DAY, t.BorrowDate, ISNULL(t.ReturnDate, GETDATE())) AS BorrowDuration,

	-- Penalty signals
	bs.PenaltyCount,
    SUM(ISNULL(p.Amount,0)) AS TotalPenaltyAmount,
    CAST(bs.PenaltyCount AS FLOAT) / NULLIF(bs.BorrowCount,0) AS PenaltyRate,

	-- Availability signals
    b.AvailableCopies,
    CAST(b.AvailableCopies AS FLOAT) / NULLIF(b.TotalCopies,0) AS AvailabilityRatio,

	-- Queue signals
    0 AS QueueLength, -- queue only applies to reservations

    'Borrow' AS ActivityType,

	-- Composite score
    (
        1 
        + (t.RenewCount * 2) 
        + CASE WHEN DATEDIFF(DAY, t.BorrowDate, GETDATE()) <= 90 THEN 3 ELSE 0 END
        - COUNT(p.Id)
    ) AS Score
FROM [dbo].[User] u
JOIN [dbo].[UserMembershipMapping] um ON u.Id = um.UserId AND um.IsActive = 1
JOIN [dbo].[Membership] m ON um.MembershipId = m.Id AND m.IsActive = 1
JOIN [dbo].[Transection] t ON u.Id = t.UserId AND t.IsActive = 1
JOIN BorrowStats bs ON u.Id = bs.UserId AND um.MembershipId = bs.MembershipId
JOIN [dbo].[Books] b ON t.BookId = b.Id AND b.IsActive = 1
JOIN [dbo].[Genre] g ON b.GenreId = g.Id AND g.IsActive = 1
LEFT JOIN [dbo].[Penalty] p ON t.Id = p.TransectionId AND p.IsActive = 1
CROSS JOIN StatusIds s
WHERE u.IsActive = 1
  AND t.StatusId <> s.TransectionCancelled
GROUP BY 
    u.Id, um.MembershipId, m.Type, b.Id, b.Title, b.Author, b.GenreId, g.Name,
    t.BorrowDate, t.RenewDate, t.ReturnDate, t.RenewCount,
    b.AvailableCopies, b.TotalCopies, bs.PenaltyCount, bs.BorrowCount

UNION

-- Reservation activity
SELECT 
    u.Id AS UserId,
    um.MembershipId,
    m.Type AS MembershipTypeName,
    b.Id AS BookId,
    b.Title,
    b.Author AS AuthorName,
    b.GenreId,
    g.Name AS GenreName,
    r.ReservationDate AS BorrowDate, 
    r.AllocatedAt AS RenewDate, 
    NULL AS ReturnDate,
    0 AS RenewCount,
    NULL AS BorrowDuration,

	-- Penalty signals (not applicable for reservations)
    0 AS PenaltyCount,
    0 AS TotalPenaltyAmount,
    0 AS PenaltyRate,

	-- Availability signals
    b.AvailableCopies,
    CAST(b.AvailableCopies AS FLOAT) / NULLIF(b.TotalCopies,0) AS AvailabilityRatio,

    COUNT(CASE WHEN r.StatusId = s.ReservationReserved THEN 1 END) AS QueueLength,

    'Reservation' AS ActivityType,

	-- Composite score
    (
        1 
        + CASE WHEN r.AllocatedAt IS NOT NULL THEN 2 ELSE 0 END 
        + CASE WHEN DATEDIFF(DAY, r.ReservationDate, GETDATE()) <= 90 THEN 3 ELSE 0 END
        - COUNT(CASE WHEN r.StatusId = s.ReservationReserved THEN 1 END)
    ) AS Score
FROM [dbo].[User] u
JOIN [dbo].[UserMembershipMapping] um ON u.Id = um.UserId AND um.IsActive = 1
JOIN [dbo].[Membership] m ON um.MembershipId = m.Id AND m.IsActive = 1
JOIN [dbo].[Reservation] r ON u.Id = r.UserId AND r.IsActive = 1
JOIN [dbo].[Books] b ON r.BookId = b.Id AND b.IsActive = 1
JOIN [dbo].[Genre] g ON b.GenreId = g.Id AND g.IsActive = 1
CROSS JOIN StatusIds s
WHERE u.IsActive = 1
  AND r.StatusId <> s.ReservationCancelled
GROUP BY 
    u.Id, um.MembershipId, m.Type, b.Id, b.Title, b.Author, b.GenreId, g.Name,
    r.ReservationDate, r.AllocatedAt,
    b.AvailableCopies, b.TotalCopies;
GO

USE [master] GO ALTER DATABASE [LibraryManagementSys] SET READ_WRITE GO