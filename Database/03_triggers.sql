/*
==================================================================================
Library Management System - Complete Trigger setup Script
==================================================================================
This script creates:
- All Triggers
==================================================================================
*/

-- Trigger: BookFileMapping Audit for Insert
IF OBJECT_ID('dbo.BookFileMapping_Audit_Insert', 'TR') IS NOT NULL
    DROP TRIGGER [dbo].[BookFileMapping_Audit_Insert];
GO

CREATE OR ALTER TRIGGER [dbo].[BookFileMapping_Audit_Insert]
ON [dbo].[BookFileMapping]
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO [logging].[BookFileMappingLog]
        SELECT 
            'Insert' AS Operation,
            'Information' AS LogLevel,
            SYSDATETIMEOFFSET() AS LoggedAt,
            i.*
        FROM INSERTED i;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRANSACTION;
    
        INSERT INTO [logging].[ExceptionLog] (UserId, Type, Message, ErrorNumber, ErrorProcedure, ErrorLine, ErrorTime)
        VALUES (SYSTEM_USER, '[dbo].[BookFileMapping_Audit_Insert]', ERROR_MESSAGE(), ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), SYSDATETIME());
    
        THROW;
    END CATCH
END
GO

-- Trigger: BookFileMapping Audit for Update
IF OBJECT_ID('dbo.BookFileMapping_Audit_Update', 'TR') IS NOT NULL
    DROP TRIGGER [dbo].[BookFileMapping_Audit_Update];
GO

CREATE OR ALTER TRIGGER [dbo].[BookFileMapping_Audit_Update]
ON [dbo].[BookFileMapping]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        DECLARE @IsActiveExists BIT = 0;

        IF COL_LENGTH('dbo.BookFileMapping', 'IsActive') IS NOT NULL
            SET @IsActiveExists = 1;
    
        IF @IsActiveExists = 1
        BEGIN
    		-- Soft Delete: IsActive 1 → 0
            INSERT INTO [logging].[BookFileMappingLog]
            SELECT 
                'SoftDelete', 'Information', SYSDATETIMEOFFSET(), i.*
            FROM INSERTED i
            JOIN DELETED d ON i.Id = d.Id
            WHERE d.IsActive = 1 AND i.IsActive = 0;
    
            -- Restore: IsActive 0 → 1
            INSERT INTO [logging].[BookFileMappingLog]
            SELECT 
                'Restore', 'Information', SYSDATETIMEOFFSET(), i.*
            FROM INSERTED i
            JOIN DELETED d ON i.Id = d.Id
            WHERE d.IsActive = 0 AND i.IsActive = 1;
        END
    
        -- General update (excluding soft delete/restore)
        INSERT INTO [logging].[BookFileMappingLog]
        SELECT 
            'Update', 'Information', SYSDATETIMEOFFSET(), i.*
        FROM INSERTED i
        JOIN DELETED d ON i.Id = d.Id
        WHERE 
            (@IsActiveExists = 0 OR i.IsActive = d.IsActive) -- IsActive didn't change
            AND NOT (d.IsActive = 1 AND i.IsActive = 0)       -- Not soft delete
            AND NOT (d.IsActive = 0 AND i.IsActive = 1)       -- Not restore
            AND EXISTS (SELECT i.* EXCEPT SELECT d.*);        -- Other columns changed
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRANSACTION;
    
        INSERT INTO [logging].[ExceptionLog] (UserId, Type, Message, ErrorNumber, ErrorProcedure, ErrorLine, ErrorTime)
        VALUES (SYSTEM_USER, '[dbo].[BookFileMapping_Audit_Update]', ERROR_MESSAGE(), ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), SYSDATETIME());
    
        THROW;
    END CATCH
END
GO

-- Trigger: BookFileMapping Audit for Delete
IF OBJECT_ID('dbo.BookFileMapping_Audit_Delete', 'TR') IS NOT NULL
    DROP TRIGGER [dbo].[BookFileMapping_Audit_Delete];
GO

CREATE OR ALTER TRIGGER [dbo].[BookFileMapping_Audit_Delete]
ON [dbo].[BookFileMapping]
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO [logging].[BookFileMappingLog]
       SELECT 
           'Delete' AS Operation,
           'Information',
           SYSDATETIMEOFFSET(),
           d.*
       FROM DELETED d;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRANSACTION;
    
        INSERT INTO [logging].[ExceptionLog] (UserId, Type, Message, ErrorNumber, ErrorProcedure, ErrorLine, ErrorTime)
        VALUES (SYSTEM_USER, '[dbo].[BookFileMapping_Audit_Delete]', ERROR_MESSAGE(), ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), SYSDATETIME());
    
        THROW;
    END CATCH
END

-- Trigger: Book Audit for Insert
IF OBJECT_ID('dbo.Books_Audit_Insert', 'TR') IS NOT NULL
    DROP TRIGGER [dbo].[Books_Audit_Insert];
GO

CREATE OR ALTER TRIGGER [dbo].[Books_Audit_Insert]
ON [dbo].[Books]
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO [logging].[BookSLog]
        SELECT 
            'Insert' AS Operation,
            'Information' AS LogLevel,
            SYSDATETIMEOFFSET() AS LoggedAt,
            *
        FROM INSERTED;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRANSACTION;
    
        INSERT INTO [logging].[ExceptionLog] (UserId, Type, Message, ErrorNumber, ErrorProcedure, ErrorLine, ErrorTime)
        VALUES (SYSTEM_USER, '[dbo].[Books_Audit_Insert]', ERROR_MESSAGE(), ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), SYSDATETIME());
    
        THROW;
    END CATCH
END
GO

-- Trigger: Book Audit for Update
IF OBJECT_ID('dbo.Books_Audit_Update', 'TR') IS NOT NULL
    DROP TRIGGER [dbo].[Books_Audit_Update];
GO

CREATE OR ALTER TRIGGER [dbo].[Books_Audit_Update]
ON [dbo].[Books]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        DECLARE @IsActiveExists BIT = 0;

        IF COL_LENGTH('dbo.Books', 'IsActive') IS NOT NULL
            SET @IsActiveExists = 1;

        IF @IsActiveExists = 1
        BEGIN
     		-- Soft Delete (IsActive: 1 → 0)
            INSERT INTO [logging].[BookSLog]
            SELECT 
                'SoftDelete', 'Information', SYSDATETIMEOFFSET(), i.*
            FROM INSERTED i
            JOIN DELETED d ON i.Id = d.Id
            WHERE d.IsActive = 1 AND i.IsActive = 0;

            -- Restore (IsActive: 0 → 1)
            INSERT INTO [logging].[BookSLog]
            SELECT 
                'Restore', 'Information', SYSDATETIMEOFFSET(), i.*
            FROM INSERTED i
            JOIN DELETED d ON i.Id = d.Id
            WHERE d.IsActive = 0 AND i.IsActive = 1;
        END

        -- General Update (excluding soft delete/restore)
        INSERT INTO [logging].[BookSLog]
        SELECT 
            'Update', 'Information', SYSDATETIMEOFFSET(), i.*
        FROM INSERTED i
        JOIN DELETED d ON i.Id = d.Id
        WHERE 
            (@IsActiveExists = 0 OR i.IsActive = d.IsActive) -- IsActive didn't change
            AND NOT (d.IsActive = 1 AND i.IsActive = 0)       -- Not soft delete
            AND NOT (d.IsActive = 0 AND i.IsActive = 1)       -- Not restore
            AND EXISTS (SELECT i.* EXCEPT SELECT d.*);        -- Other columns changed

      END TRY
      BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRANSACTION;
    
        INSERT INTO [logging].[ExceptionLog] (UserId, Type, Message, ErrorNumber, ErrorProcedure, ErrorLine, ErrorTime)
        VALUES (SYSTEM_USER, '[dbo].[Books_Audit_Update]', ERROR_MESSAGE(), ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), SYSDATETIME());
    
        THROW;
    END CATCH
END
GO

-- Trigger: Book Audit for Delete
IF OBJECT_ID('dbo.Books_Audit_Delete', 'TR') IS NOT NULL
    DROP TRIGGER [dbo].[Books_Audit_Delete];
GO

CREATE OR ALTER TRIGGER [dbo].[Books_Audit_Delete]
ON [dbo].[Books]
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO [logging].[BookSLog]
        SELECT 
            'Delete' AS Operation,
            'Information',
            SYSDATETIMEOFFSET(),
            *
        FROM DELETED;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRANSACTION;
    
        INSERT INTO [logging].[ExceptionLog] (UserId, Type, Message, ErrorNumber, ErrorProcedure, ErrorLine, ErrorTime)
        VALUES (SYSTEM_USER, '[dbo].[Books_Audit_Delete]', ERROR_MESSAGE(), ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), SYSDATETIME());
    
        THROW;
    END CATCH
END
GO

-- Trigger: Config Audit for Insert
IF OBJECT_ID('dbo.Configs_Audit_Insert', 'TR') IS NOT NULL
    DROP TRIGGER [dbo].[Configs_Audit_Insert];
GO

CREATE OR ALTER TRIGGER [dbo].[Configs_Audit_Insert]
ON [dbo].[Configs]
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO [logging].[ConfigsLog]
        SELECT 
            'Insert' AS Operation,
            'Information' AS LogLevel,
            SYSDATETIMEOFFSET() AS LoggedAt,
            i.*
        FROM INSERTED i;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRANSACTION;

        INSERT INTO [logging].[ExceptionLog] (UserId, Type, Message, ErrorNumber, ErrorProcedure, ErrorLine, ErrorTime)
        VALUES (SYSTEM_USER, '[dbo].[Configs_Audit_Insert]', ERROR_MESSAGE(), ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), SYSDATETIME());

        THROW;
    END CATCH
END
GO

-- Trigger: Config Audit for Update
IF OBJECT_ID('dbo.Configs_Audit_Update', 'TR') IS NOT NULL
    DROP TRIGGER [dbo].[Configs_Audit_Update];
GO

CREATE OR ALTER TRIGGER [dbo].[Configs_Audit_Update]
ON [dbo].[Configs]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        DECLARE @IsActiveExists BIT = 0;

        IF COL_LENGTH('dbo.Configs', 'IsActive') IS NOT NULL
            SET @IsActiveExists = 1;

        IF @IsActiveExists = 1
        BEGIN
            -- Soft Delete: IsActive 1 → 0
            INSERT INTO [logging].[ConfigsLog]
            SELECT 
                'SoftDelete', 'Information', SYSDATETIMEOFFSET(), i.*
            FROM INSERTED i
            JOIN DELETED d ON i.Id = d.Id
            WHERE d.IsActive = 1 AND i.IsActive = 0;

            -- Restore: IsActive 0 → 1
            INSERT INTO [logging].[ConfigsLog]
            SELECT 
                'Restore', 'Information', SYSDATETIMEOFFSET(), i.*
            FROM INSERTED i
            JOIN DELETED d ON i.Id = d.Id
            WHERE d.IsActive = 0 AND i.IsActive = 1;
        END

        -- General Update (not a soft delete/restore)
        INSERT INTO [logging].[ConfigsLog]
        SELECT 
            'Update', 'Information', SYSDATETIMEOFFSET(), i.*
        FROM INSERTED i
        JOIN DELETED d ON i.Id = d.Id
        WHERE 
            (@IsActiveExists = 0 OR i.IsActive = d.IsActive) -- IsActive didn't change
            AND NOT (d.IsActive = 1 AND i.IsActive = 0)       -- Not soft delete
            AND NOT (d.IsActive = 0 AND i.IsActive = 1)       -- Not restore
            AND EXISTS (SELECT i.* EXCEPT SELECT d.*);        -- Other columns changed
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRANSACTION;

        INSERT INTO [logging].[ExceptionLog] (UserId, Type, Message, ErrorNumber, ErrorProcedure, ErrorLine, ErrorTime)
        VALUES (SYSTEM_USER, '[dbo].[Configs_Audit_Update]', ERROR_MESSAGE(), ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), SYSDATETIME());

        THROW;
    END CATCH
END
GO

-- Trigger: Config Audit for Delete
IF OBJECT_ID('dbo.Configs_Audit_Delete', 'TR') IS NOT NULL
    DROP TRIGGER [dbo].[Configs_Audit_Delete];
GO

CREATE OR ALTER TRIGGER [dbo].[Configs_Audit_Delete]
ON [dbo].[Configs]
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO [logging].[ConfigsLog]
		SELECT 
		    'Delete' AS Operation,
		    'Information',
		    SYSDATETIMEOFFSET(),
		    d.*
		FROM DELETED d;
    END TRY
    BEGIN CATCH
	    IF XACT_STATE() <> 0
	        ROLLBACK TRANSACTION;
	
	    INSERT INTO [logging].[ExceptionLog] (UserId, Type, Message, ErrorNumber, ErrorProcedure, ErrorLine, ErrorTime)
	    VALUES (SYSTEM_USER, '[dbo].[Configs_Audit_Delete]', ERROR_MESSAGE(), ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), SYSDATETIME());
	
	    THROW;
	END CATCH
END
GO

-- Trigger: Genre Audit for Insert
IF OBJECT_ID('dbo.Genre_Audit_Insert', 'TR') IS NOT NULL
    DROP TRIGGER [dbo].[Genre_Audit_Insert];
GO

CREATE OR ALTER TRIGGER [dbo].[Genre_Audit_Insert]
ON [dbo].[Genre]
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO [logging].[GenreLog]
        SELECT 
            'Insert', 
            'Information', 
            SYSDATETIMEOFFSET(), 
            i.*
        FROM INSERTED i;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRANSACTION;

        INSERT INTO [logging].[ExceptionLog] (UserId, Type, Message, ErrorNumber, ErrorProcedure, ErrorLine, ErrorTime)
        VALUES (SYSTEM_USER, '[dbo].[Genre_Audit_Insert]', ERROR_MESSAGE(), ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), SYSDATETIME());

        THROW;
    END CATCH
END
GO

-- Trigger: Genre Audit for Update
IF OBJECT_ID('dbo.Genre_Audit_Update', 'TR') IS NOT NULL
    DROP TRIGGER [dbo].[Genre_Audit_Update];
GO

CREATE OR ALTER TRIGGER [dbo].[Genre_Audit_Update]
ON [dbo].[Genre]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        DECLARE @IsActiveExists BIT = 0;
        IF COL_LENGTH('dbo.Genre', 'IsActive') IS NOT NULL SET @IsActiveExists = 1;

        IF @IsActiveExists = 1
        BEGIN
			-- Soft Delete: IsActive 1 → 0
            INSERT INTO [logging].[GenreLog]
            SELECT 
                'SoftDelete', 'Information', SYSDATETIMEOFFSET(), i.*
            FROM INSERTED i
            JOIN DELETED d ON i.Id = d.Id
            WHERE d.IsActive = 1 AND i.IsActive = 0;

			-- Restore: IsActive 0 → 1
            INSERT INTO [logging].[GenreLog]
            SELECT 
                'Restore', 'Information', SYSDATETIMEOFFSET(), i.*
            FROM INSERTED i
            JOIN DELETED d ON i.Id = d.Id
            WHERE d.IsActive = 0 AND i.IsActive = 1;
        END

		-- General update (excluding soft delete/restore)
        INSERT INTO [logging].[GenreLog]
        SELECT 
            'Update', 'Information', SYSDATETIMEOFFSET(), i.*
        FROM INSERTED i
        JOIN DELETED d ON i.Id = d.Id
        WHERE 
            (@IsActiveExists = 0 OR i.IsActive = d.IsActive) -- IsActive didn't change
            AND NOT (d.IsActive = 1 AND i.IsActive = 0)       -- Not soft delete
            AND NOT (d.IsActive = 0 AND i.IsActive = 1)       -- Not restore
            AND EXISTS (SELECT i.* EXCEPT SELECT d.*);        -- Other columns changed
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 
			ROLLBACK TRANSACTION;

        INSERT INTO [logging].[ExceptionLog] (UserId, Type, Message, ErrorNumber, ErrorProcedure, ErrorLine, ErrorTime) 
		VALUES (SYSTEM_USER, '[dbo].[Genre_Audit_Update]', ERROR_MESSAGE(), ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), SYSDATETIME());

        THROW;
    END CATCH
END
GO

-- Trigger: Genre Audit for Delete
IF OBJECT_ID('dbo.Genre_Audit_Delete', 'TR') IS NOT NULL
    DROP TRIGGER [dbo].[Genre_Audit_Delete];
GO

CREATE OR ALTER TRIGGER [dbo].[Genre_Audit_Delete]
ON [dbo].[Genre]
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO [logging].[GenreLog]
        SELECT 
			'Delete', 
			'Information', 
			SYSDATETIMEOFFSET(), 
			d.*
        FROM DELETED d;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 
			ROLLBACK TRANSACTION;

        INSERT INTO [logging].[ExceptionLog] (UserId, Type, Message, ErrorNumber, ErrorProcedure, ErrorLine, ErrorTime) 
		VALUES (SYSTEM_USER, '[dbo].[Genre_Audit_Delete]', ERROR_MESSAGE(), ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), SYSDATETIME());

        THROW;
    END CATCH
END
GO

-- Trigger: Membership Audit for Insert
IF OBJECT_ID('dbo.Membership_Audit_Insert', 'TR') IS NOT NULL
    DROP TRIGGER [dbo].[Membership_Audit_Insert];
GO

CREATE OR ALTER TRIGGER [dbo].[Membership_Audit_Insert]
ON [dbo].[Membership]
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO [logging].[MembershipLog]
        SELECT 
			'Insert', 
			'Information', 
			SYSDATETIMEOFFSET(), 
			i.*
        FROM INSERTED i;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 
			ROLLBACK TRANSACTION;

        INSERT INTO [logging].[ExceptionLog] (UserId, Type, Message, ErrorNumber, ErrorProcedure, ErrorLine, ErrorTime) 
		VALUES (SYSTEM_USER, '[dbo].[Membership_Audit_Insert]', ERROR_MESSAGE(), ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), SYSDATETIME());

        THROW;
    END CATCH
END
GO

-- Trigger: Membership Audit for Update
IF OBJECT_ID('dbo.Membership_Audit_Update', 'TR') IS NOT NULL
    DROP TRIGGER [dbo].[Membership_Audit_Update];
GO

CREATE OR ALTER TRIGGER [dbo].[Membership_Audit_Update]
ON [dbo].[Membership]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        DECLARE @IsActiveExists BIT = 0;
        IF COL_LENGTH('dbo.Membership', 'IsActive') IS NOT NULL SET @IsActiveExists = 1;

        IF @IsActiveExists = 1
        BEGIN
			-- Soft Delete: IsActive 1 → 0
            INSERT INTO [logging].[MembershipLog]
            SELECT 
				'SoftDelete', 'Information', SYSDATETIMEOFFSET(), i.*
            FROM INSERTED i
            JOIN DELETED d ON i.Id = d.Id
            WHERE d.IsActive = 1 AND i.IsActive = 0;

			-- Restore: IsActive 0 → 1
            INSERT INTO [logging].[MembershipLog]
            SELECT 
				'Restore', 'Information', SYSDATETIMEOFFSET(), i.*
            FROM INSERTED i
            JOIN DELETED d ON i.Id = d.Id
            WHERE d.IsActive = 0 AND i.IsActive = 1;
        END

		-- General update (excluding soft delete/restore)
        INSERT INTO [logging].[MembershipLog]
        SELECT 
			'Update', 'Information', SYSDATETIMEOFFSET(), i.*
        FROM INSERTED i
        JOIN DELETED d ON i.Id = d.Id
        WHERE 
            (@IsActiveExists = 0 OR i.IsActive = d.IsActive) -- IsActive didn't change
            AND NOT (d.IsActive = 1 AND i.IsActive = 0)       -- Not soft delete
            AND NOT (d.IsActive = 0 AND i.IsActive = 1)       -- Not restore
            AND EXISTS (SELECT i.* EXCEPT SELECT d.*);        -- Other columns changed
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 
			ROLLBACK TRANSACTION;

        INSERT INTO [logging].[ExceptionLog] (UserId, Type, Message, ErrorNumber, ErrorProcedure, ErrorLine, ErrorTime) 
		VALUES (SYSTEM_USER, '[dbo].[Membership_Audit_Update]', ERROR_MESSAGE(), ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), SYSDATETIME());
        
		THROW;
    END CATCH
END
GO

-- Trigger: Membership Audit for Delete
IF OBJECT_ID('dbo.Membership_Audit_Delete', 'TR') IS NOT NULL
    DROP TRIGGER [dbo].[Membership_Audit_Delete];
GO

CREATE OR ALTER TRIGGER [dbo].[Membership_Audit_Delete]
ON [dbo].[Membership]
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO [logging].[MembershipLog]
        SELECT 
			'Delete', 
			'Information', 
			SYSDATETIMEOFFSET(), 
			d.*
        FROM DELETED d;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 
			ROLLBACK TRANSACTION;

        INSERT INTO [logging].[ExceptionLog] (UserId, Type, Message, ErrorNumber, ErrorProcedure, ErrorLine, ErrorTime) 
		VALUES (SYSTEM_USER, '[dbo].[Membership_Audit_Delete]', ERROR_MESSAGE(), ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), SYSDATETIME());
        
		THROW;
    END CATCH
END
GO

-- Trigger: OutboxMessages Audit for Insert
IF OBJECT_ID('dbo.OutboxMessages_Audit_Insert', 'TR') IS NOT NULL
    DROP TRIGGER [dbo].[OutboxMessages_Audit_Insert];
GO

CREATE OR ALTER TRIGGER [dbo].[OutboxMessages_Audit_Insert]
ON [dbo].[OutboxMessages]
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO [logging].[OutboxMessagesLog]
        SELECT 
			'Insert', 
			'Information', 
			SYSDATETIMEOFFSET(), 
			i.*
        FROM INSERTED i;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 
			ROLLBACK TRANSACTION;

        INSERT INTO [logging].[ExceptionLog] (UserId, Type, Message, ErrorNumber, ErrorProcedure, ErrorLine, ErrorTime) 
		VALUES (SYSTEM_USER, '[dbo].[OutboxMessages_Audit_Insert]', ERROR_MESSAGE(), ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), SYSDATETIME());
        
		THROW;
    END CATCH
END
GO

-- Trigger: OutboxMessages Audit for Update
IF OBJECT_ID('dbo.OutboxMessages_Audit_Update', 'TR') IS NOT NULL
    DROP TRIGGER [dbo].[OutboxMessages_Audit_Update];
GO

CREATE OR ALTER TRIGGER [dbo].[OutboxMessages_Audit_Update]
ON [dbo].[OutboxMessages]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        DECLARE @IsActiveExists BIT = 0;
        IF COL_LENGTH('dbo.OutboxMessages', 'IsActive') IS NOT NULL SET @IsActiveExists = 1;

        IF @IsActiveExists = 1
        BEGIN
			-- Soft Delete: IsActive 1 → 0
            INSERT INTO [logging].[OutboxMessagesLog]
            SELECT 
				'SoftDelete', 'Information', SYSDATETIMEOFFSET(), i.*
            FROM INSERTED i
            JOIN DELETED d ON i.Id = d.Id
            WHERE d.IsActive = 1 AND i.IsActive = 0;

			-- Restore: IsActive 0 → 1
            INSERT INTO [logging].[OutboxMessagesLog]
            SELECT 
				'Restore', 'Information', SYSDATETIMEOFFSET(), i.*
            FROM INSERTED i
            JOIN DELETED d ON i.Id = d.Id
            WHERE d.IsActive = 0 AND i.IsActive = 1;
        END

		-- General update (excluding soft delete/restore)
        INSERT INTO [logging].[OutboxMessagesLog]
        SELECT 
			'Update', 'Information', SYSDATETIMEOFFSET(), i.*
        FROM INSERTED i
        JOIN DELETED d ON i.Id = d.Id
        WHERE 
            (@IsActiveExists = 0 OR i.IsActive = d.IsActive) -- IsActive didn't change
            AND NOT (d.IsActive = 1 AND i.IsActive = 0)       -- Not soft delete
            AND NOT (d.IsActive = 0 AND i.IsActive = 1)       -- Not restore
            AND EXISTS (SELECT i.* EXCEPT SELECT d.*);        -- Other columns changed
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 
			ROLLBACK TRANSACTION;

        INSERT INTO [logging].[ExceptionLog] (UserId, Type, Message, ErrorNumber, ErrorProcedure, ErrorLine, ErrorTime) 
		VALUES (SYSTEM_USER, '[dbo].[OutboxMessages_Audit_Update]', ERROR_MESSAGE(), ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), SYSDATETIME());
        
		THROW;
    END CATCH
END

-- Trigger: OutboxMessages Audit for Delete
IF OBJECT_ID('dbo.OutboxMessages_Audit_Delete', 'TR') IS NOT NULL
    DROP TRIGGER [dbo].[OutboxMessages_Audit_Delete];
GO

CREATE OR ALTER TRIGGER [dbo].[OutboxMessages_Audit_Delete]
ON [dbo].[OutboxMessages]
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO [logging].[OutboxMessagesLog]
        SELECT 
			'Delete', 
			'Information', 
			SYSDATETIMEOFFSET(), 
			d.*
        FROM DELETED d;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 
			ROLLBACK TRANSACTION;

        INSERT INTO [logging].[ExceptionLog] (UserId, Type, Message, ErrorNumber, ErrorProcedure, ErrorLine, ErrorTime) 
		VALUES (SYSTEM_USER, '[dbo].[OutboxMessages_Audit_Delete]', ERROR_MESSAGE(), ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), SYSDATETIME());
        
		THROW;
    END CATCH
END
GO

-- Trigger: Penalty Audit for Insert
IF OBJECT_ID('dbo.Penalty_Audit_Insert', 'TR') IS NOT NULL
    DROP TRIGGER [dbo].[Penalty_Audit_Insert];
GO

CREATE OR ALTER TRIGGER [dbo].[Penalty_Audit_Insert]
ON [dbo].[Penalty]
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO [logging].[PenaltyLog]
        SELECT 
			'Insert', 
			'Information', 
			SYSDATETIMEOFFSET(), 
			i.*
        FROM INSERTED i;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 
			ROLLBACK TRANSACTION;

        INSERT INTO [logging].[ExceptionLog] (UserId, Type, Message, ErrorNumber, ErrorProcedure, ErrorLine, ErrorTime) 
		VALUES (SYSTEM_USER, '[dbo].[Penalty_Audit_Insert]', ERROR_MESSAGE(), ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), SYSDATETIME());
        
		THROW;
    END CATCH
END
GO

-- Trigger: Penalty Audit for Update
IF OBJECT_ID('dbo.Penalty_Audit_Update', 'TR') IS NOT NULL
    DROP TRIGGER [dbo].[Penalty_Audit_Update];
GO

CREATE OR ALTER TRIGGER [dbo].[Penalty_Audit_Update]
ON [dbo].[Penalty]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        DECLARE @IsActiveExists BIT = 0;
        IF COL_LENGTH('dbo.Penalty', 'IsActive') IS NOT NULL SET @IsActiveExists = 1;

        -- Soft Delete: IsActive 1 → 0
        IF @IsActiveExists = 1
        BEGIN
            INSERT INTO [logging].[PenaltyLog]
            SELECT 
				'SoftDelete', 'Information', SYSDATETIMEOFFSET(), i.*
            FROM INSERTED i
            JOIN DELETED d ON i.Id = d.Id
            WHERE d.IsActive = 1 AND i.IsActive = 0;

            -- Restore: IsActive 0 → 1
            INSERT INTO [logging].[PenaltyLog]
            SELECT 
				'Restore', 'Information', SYSDATETIMEOFFSET(), i.*
            FROM INSERTED i
            JOIN DELETED d ON i.Id = d.Id
            WHERE d.IsActive = 0 AND i.IsActive = 1;
        END

        -- General update (excluding soft delete/restore)
        INSERT INTO [logging].[PenaltyLog]
        SELECT 
			'Update', 'Information', SYSDATETIMEOFFSET(), i.*
        FROM INSERTED i
        JOIN DELETED d ON i.Id = d.Id
        WHERE 
            (@IsActiveExists = 0 OR i.IsActive = d.IsActive) -- IsActive didn't change
            AND NOT (d.IsActive = 1 AND i.IsActive = 0)       -- Not soft delete
            AND NOT (d.IsActive = 0 AND i.IsActive = 1)       -- Not restore
            AND EXISTS (SELECT i.* EXCEPT SELECT d.*);        -- Other columns changed
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 
			ROLLBACK TRANSACTION;

        INSERT INTO [logging].[ExceptionLog] (UserId, Type, Message, ErrorNumber, ErrorProcedure, ErrorLine, ErrorTime) 
		VALUES (SYSTEM_USER, '[dbo].[Penalty_Audit_Update]', ERROR_MESSAGE(), ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), SYSDATETIME());
        
		THROW;
    END CATCH
END
GO

-- Trigger: Penalty Audit for Delete
IF OBJECT_ID('dbo.Penalty_Audit_Delete', 'TR') IS NOT NULL
    DROP TRIGGER [dbo].[Penalty_Audit_Delete];
GO

CREATE OR ALTER TRIGGER [dbo].[Penalty_Audit_Delete]
ON [dbo].[Penalty]
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO [logging].[PenaltyLog]
        SELECT 
			'Delete', 
			'Information', 
			SYSDATETIMEOFFSET(), 
			d.*
        FROM DELETED d;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 
			ROLLBACK TRANSACTION;

        INSERT INTO [logging].[ExceptionLog] (UserId, Type, Message, ErrorNumber, ErrorProcedure, ErrorLine, ErrorTime) 
		VALUES (SYSTEM_USER, '[dbo].[Penalty_Audit_Delete]', ERROR_MESSAGE(), ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), SYSDATETIME());
        
		THROW;
    END CATCH
END
GO

-- Trigger: PenaltyType Audit for Insert
IF OBJECT_ID('dbo.PenaltyType_Audit_Insert', 'TR') IS NOT NULL
    DROP TRIGGER [dbo].[PenaltyType_Audit_Insert];
GO

CREATE OR ALTER TRIGGER [dbo].[PenaltyType_Audit_Insert]
ON [dbo].[PenaltyType]
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO [logging].[PenaltyTypeLog]
        SELECT 
			'Insert', 
			'Information', 
			SYSDATETIMEOFFSET(), 
			i.*
        FROM INSERTED i;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 
			ROLLBACK TRANSACTION;

        INSERT INTO [logging].[ExceptionLog] (UserId, Type, Message, ErrorNumber, ErrorProcedure, ErrorLine, ErrorTime) 
		VALUES (SYSTEM_USER, '[dbo].[PenaltyType_Audit_Insert]', ERROR_MESSAGE(), ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), SYSDATETIME());
        
		THROW;
    END CATCH
END
GO

-- Trigger: PenaltyType Audit for Update
IF OBJECT_ID('dbo.PenaltyType_Audit_Update', 'TR') IS NOT NULL
    DROP TRIGGER [dbo].[PenaltyType_Audit_Update];
GO

CREATE OR ALTER TRIGGER [dbo].[PenaltyType_Audit_Update]
ON [dbo].[PenaltyType]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        DECLARE @IsActiveExists BIT = 0;
        IF COL_LENGTH('dbo.PenaltyType', 'IsActive') IS NOT NULL SET @IsActiveExists = 1;

        -- Soft Delete: IsActive 1 → 0
        IF @IsActiveExists = 1
        BEGIN
            INSERT INTO [logging].[PenaltyTypeLog]
            SELECT 
				'SoftDelete', 'Information', SYSDATETIMEOFFSET(), i.*
            FROM INSERTED i
            JOIN DELETED d ON i.Id = d.Id
            WHERE d.IsActive = 1 AND i.IsActive = 0;

            -- Restore: IsActive 0 → 1
            INSERT INTO [logging].[PenaltyTypeLog]
            SELECT 
				'Restore', 'Information', SYSDATETIMEOFFSET(), i.*
            FROM INSERTED i
            JOIN DELETED d ON i.Id = d.Id
            WHERE d.IsActive = 0 AND i.IsActive = 1;
        END

        -- General update (excluding soft delete/restore)
        INSERT INTO [logging].[PenaltyTypeLog]
        SELECT 
			'Update', 'Information', SYSDATETIMEOFFSET(), i.*
        FROM INSERTED i
        JOIN DELETED d ON i.Id = d.Id
        WHERE 
            (@IsActiveExists = 0 OR i.IsActive = d.IsActive) -- IsActive didn't change
            AND NOT (d.IsActive = 1 AND i.IsActive = 0)       -- Not soft delete
            AND NOT (d.IsActive = 0 AND i.IsActive = 1)       -- Not restore
            AND EXISTS (SELECT i.* EXCEPT SELECT d.*);        -- Other columns changed
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 
			ROLLBACK TRANSACTION;

        INSERT INTO [logging].[ExceptionLog] (UserId, Type, Message, ErrorNumber, ErrorProcedure, ErrorLine, ErrorTime) 
		VALUES (SYSTEM_USER, '[dbo].[PenaltyType_Audit_Update]', ERROR_MESSAGE(), ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), SYSDATETIME());
        
		THROW;
    END CATCH
END
GO

-- Trigger: PenaltyType Audit for Delete
IF OBJECT_ID('dbo.PenaltyType_Audit_Delete', 'TR') IS NOT NULL
    DROP TRIGGER [dbo].[PenaltyType_Audit_Delete];
GO

CREATE OR ALTER TRIGGER [dbo].[PenaltyType_Audit_Delete]
ON [dbo].[PenaltyType]
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO [logging].[PenaltyTypeLog]
        SELECT 
			'Delete', 
			'Information', 
			SYSDATETIMEOFFSET(), 
			d.*
        FROM DELETED d;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 
			ROLLBACK TRANSACTION;

        INSERT INTO [logging].[ExceptionLog] (UserId, Type, Message, ErrorNumber, ErrorProcedure, ErrorLine, ErrorTime) 
		VALUES (SYSTEM_USER, '[dbo].[PenaltyType_Audit_Delete]', ERROR_MESSAGE(), ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), SYSDATETIME());
        
		THROW;
    END CATCH
END
GO

-- Trigger: Reservation Audit for Insert
IF OBJECT_ID('dbo.Reservation_Audit_Insert', 'TR') IS NOT NULL
    DROP TRIGGER [dbo].[Reservation_Audit_Insert];
GO

CREATE OR ALTER TRIGGER [dbo].[Reservation_Audit_Insert]
ON [dbo].[Reservation]
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO [logging].[ReservationLog]
        SELECT 
			'Insert', 
			'Information', 
			SYSDATETIMEOFFSET(), 
			i.*
        FROM INSERTED i;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 
			ROLLBACK TRANSACTION;

        INSERT INTO [logging].[ExceptionLog] (UserId, Type, Message, ErrorNumber, ErrorProcedure, ErrorLine, ErrorTime) 
		VALUES (SYSTEM_USER, '[dbo].[Reservation_Audit_Insert]', ERROR_MESSAGE(), ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), SYSDATETIME());
        
		THROW;
    END CATCH
END
GO

-- Trigger: Reservation Audit for Update
IF OBJECT_ID('dbo.Reservation_Audit_Udpate', 'TR') IS NOT NULL
    DROP TRIGGER [dbo].[Reservation_Audit_Update];
GO

CREATE OR ALTER TRIGGER [dbo].[Reservation_Audit_Update]
ON [dbo].[Reservation]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        DECLARE @IsActiveExists BIT = 0;
        IF COL_LENGTH('dbo.Reservation', 'IsActive') IS NOT NULL SET @IsActiveExists = 1;

        -- Soft Delete: IsActive 1 → 0
        IF @IsActiveExists = 1
        BEGIN
            INSERT INTO [logging].[ReservationLog]
            SELECT 
				'SoftDelete', 'Information', SYSDATETIMEOFFSET(), i.*
            FROM INSERTED i
            JOIN DELETED d ON i.Id = d.Id
            WHERE d.IsActive = 1 AND i.IsActive = 0;

            -- Restore: IsActive 0 → 1
            INSERT INTO [logging].[ReservationLog]
            SELECT 
				'Restore', 'Information', SYSDATETIMEOFFSET(), i.*
            FROM INSERTED i
            JOIN DELETED d ON i.Id = d.Id
            WHERE d.IsActive = 0 AND i.IsActive = 1;
        END

        -- General update (excluding soft delete/restore)
        INSERT INTO [logging].[ReservationLog]
        SELECT 
			'Update', 'Information', SYSDATETIMEOFFSET(), i.*
        FROM INSERTED i
        JOIN DELETED d ON i.Id = d.Id
        WHERE 
            (@IsActiveExists = 0 OR i.IsActive = d.IsActive) -- IsActive didn't change
            AND NOT (d.IsActive = 1 AND i.IsActive = 0)       -- Not soft delete
            AND NOT (d.IsActive = 0 AND i.IsActive = 1)       -- Not restore
            AND EXISTS (SELECT i.* EXCEPT SELECT d.*);        -- Other columns changed
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 
			ROLLBACK TRANSACTION;

        INSERT INTO [logging].[ExceptionLog] (UserId, Type, Message, ErrorNumber, ErrorProcedure, ErrorLine, ErrorTime) 
		VALUES (SYSTEM_USER, '[dbo].[Reservation_Audit_Update]', ERROR_MESSAGE(), ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), SYSDATETIME());
        
		THROW;
    END CATCH
END
GO

-- Trigger: Reservation Audit for Delete
IF OBJECT_ID('dbo.Reservation_Audit_Delete', 'TR') IS NOT NULL
    DROP TRIGGER [dbo].[Reservation_Audit_Delete];
GO

CREATE OR ALTER TRIGGER [dbo].[Reservation_Audit_Delete]
ON [dbo].[Reservation]
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO [logging].[ReservationLog]
        SELECT 
			'Delete', 
			'Information', 
			SYSDATETIMEOFFSET(), 
			d.*
        FROM DELETED d;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 
		ROLLBACK TRANSACTION;
        
		INSERT INTO [logging].[ExceptionLog] (UserId, Type, Message, ErrorNumber, ErrorProcedure, ErrorLine, ErrorTime) 
		VALUES (SYSTEM_USER, '[dbo].[Reservation_Audit_Delete]', ERROR_MESSAGE(), ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), SYSDATETIME());
        
		THROW;
    END CATCH
END
GO

-- Trigger: RoleList Audit for Insert
IF OBJECT_ID('dbo.RoleList_Audit_Insert', 'TR') IS NOT NULL
    DROP TRIGGER [dbo].[RoleList_Audit_Insert];
GO

CREATE OR ALTER TRIGGER [dbo].[RoleList_Audit_Insert]
ON [dbo].[RoleList]
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO [logging].[RoleListLog]
        SELECT 
			'Insert', 
			'Information', 
			SYSDATETIMEOFFSET(), 
			i.*
        FROM INSERTED i;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 
			ROLLBACK TRANSACTION;

        INSERT INTO [logging].[ExceptionLog] (UserId, Type, Message, ErrorNumber, ErrorProcedure, ErrorLine, ErrorTime) 
		VALUES (SYSTEM_USER, '[dbo].[RoleList_Audit_Insert]', ERROR_MESSAGE(), ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), SYSDATETIME());
        
		THROW;
    END CATCH
END
GO

-- Trigger: RoleList Audit for Update
IF OBJECT_ID('dbo.RoleList_Audit_Update', 'TR') IS NOT NULL
    DROP TRIGGER [dbo].[RoleList_Audit_Update];
GO

CREATE OR ALTER TRIGGER [dbo].[RoleList_Audit_Update]
ON [dbo].[RoleList]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        DECLARE @IsActiveExists BIT = 0;
        IF COL_LENGTH('dbo.RoleList', 'IsActive') IS NOT NULL SET @IsActiveExists = 1;

        -- Soft Delete: IsActive 1 → 0
        IF @IsActiveExists = 1
        BEGIN
            INSERT INTO [logging].[RoleListLog]
            SELECT 
				'SoftDelete', 'Information', SYSDATETIMEOFFSET(), i.*
            FROM INSERTED i
            JOIN DELETED d ON i.Id = d.Id
            WHERE d.IsActive = 1 AND i.IsActive = 0;

            -- Restore: IsActive 0 → 1
            INSERT INTO [logging].[RoleListLog]
            SELECT 
				'Restore', 'Information', SYSDATETIMEOFFSET(), i.*
            FROM INSERTED i
            JOIN DELETED d ON i.Id = d.Id
            WHERE d.IsActive = 0 AND i.IsActive = 1;
        END

        -- General update (excluding soft delete/restore)
        INSERT INTO [logging].[RoleListLog]
        SELECT 
			'Update', 'Information', SYSDATETIMEOFFSET(), i.*
        FROM INSERTED i
        JOIN DELETED d ON i.Id = d.Id
        WHERE 
            (@IsActiveExists = 0 OR i.IsActive = d.IsActive) -- IsActive didn't change
            AND NOT (d.IsActive = 1 AND i.IsActive = 0)       -- Not soft delete
            AND NOT (d.IsActive = 0 AND i.IsActive = 1)       -- Not restore
            AND EXISTS (SELECT i.* EXCEPT SELECT d.*);        -- Other columns changed
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 
			ROLLBACK TRANSACTION;

        INSERT INTO [logging].[ExceptionLog] (UserId, Type, Message, ErrorNumber, ErrorProcedure, ErrorLine, ErrorTime) 
		VALUES (SYSTEM_USER, '[dbo].[RoleList_Audit_Update]', ERROR_MESSAGE(), ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), SYSDATETIME());

        THROW;
    END CATCH
END
GO

-- Trigger: RoleList Audit for Delete
IF OBJECT_ID('dbo.RoleList_Audit_Delete', 'TR') IS NOT NULL
    DROP TRIGGER [dbo].[RoleList_Audit_Delete];
GO

CREATE OR ALTER TRIGGER [dbo].[RoleList_Audit_Delete]
ON [dbo].[RoleList]
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO [logging].[RoleListLog]
        SELECT 
			'Delete', 
			'Information', 
			SYSDATETIMEOFFSET(), 
			d.*
        FROM DELETED d;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 
			ROLLBACK TRANSACTION;

        INSERT INTO [logging].[ExceptionLog] (UserId, Type, Message, ErrorNumber, ErrorProcedure, ErrorLine, ErrorTime) 
		VALUES (SYSTEM_USER, '[dbo].[RoleList_Audit_Delete]', ERROR_MESSAGE(), ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), SYSDATETIME());
        
		THROW;
    END CATCH
END
GO

-- Trigger: Status Audit for Insert
IF OBJECT_ID('dbo.Status_Audit_Insert', 'TR') IS NOT NULL
    DROP TRIGGER [dbo].[Status_Audit_Insert];
GO

CREATE OR ALTER TRIGGER [dbo].[Status_Audit_Insert]
ON [dbo].[Status]
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO [logging].[StatusLog]
        SELECT 
			'Insert', 
			'Information', 
			SYSDATETIMEOFFSET(), 
			i.*
        FROM INSERTED i;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 
            ROLLBACK TRANSACTION;

        INSERT INTO [logging].[ExceptionLog] (UserId, Type, Message, ErrorNumber, ErrorProcedure, ErrorLine, ErrorTime) 
        VALUES (SYSTEM_USER, '[dbo].[Status_Audit_Insert]', ERROR_MESSAGE(), ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), SYSDATETIME());
        
		THROW;
    END CATCH
END
GO

-- Trigger: Status Audit for Update
IF OBJECT_ID('dbo.Status_Audit_Update', 'TR') IS NOT NULL
    DROP TRIGGER [dbo].[Status_Audit_Update];
GO

CREATE OR ALTER TRIGGER [dbo].[Status_Audit_Update]
ON [dbo].[Status]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        DECLARE @IsActiveExists BIT = 0;
        IF COL_LENGTH('dbo.Status', 'IsActive') IS NOT NULL SET @IsActiveExists = 1;

        -- Soft Delete: IsActive 1 → 0
        IF @IsActiveExists = 1
        BEGIN
            INSERT INTO [logging].[StatusLog]
            SELECT 
				'SoftDelete', 'Information', SYSDATETIMEOFFSET(), i.*
            FROM INSERTED i
            JOIN DELETED d ON i.Id = d.Id
            WHERE d.IsActive = 1 AND i.IsActive = 0;

            -- Restore: IsActive 0 → 1
            INSERT INTO [logging].[StatusLog]
            SELECT 
				'Restore', 'Information', SYSDATETIMEOFFSET(), i.*
            FROM INSERTED i
            JOIN DELETED d ON i.Id = d.Id
            WHERE d.IsActive = 0 AND i.IsActive = 1;
        END

        -- General update (excluding soft delete/restore)
        INSERT INTO [logging].[StatusLog]
        SELECT 
			'Update', 'Information', SYSDATETIMEOFFSET(), i.*
        FROM INSERTED i
        JOIN DELETED d ON i.Id = d.Id
        WHERE 
            (@IsActiveExists = 0 OR i.IsActive = d.IsActive) -- IsActive didn't change
            AND NOT (d.IsActive = 1 AND i.IsActive = 0)       -- Not soft delete
            AND NOT (d.IsActive = 0 AND i.IsActive = 1)       -- Not restore
            AND EXISTS (SELECT i.* EXCEPT SELECT d.*);        -- Other columns changed
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 
            ROLLBACK TRANSACTION;
        
		INSERT INTO [logging].[ExceptionLog] (UserId, Type, Message, ErrorNumber, ErrorProcedure, ErrorLine, ErrorTime) 
        VALUES (SYSTEM_USER, '[dbo].[Status_Audit_Update]', ERROR_MESSAGE(), ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), SYSDATETIME());
        
		THROW;
    END CATCH
END
GO

-- Trigger: Status Audit for Delete
IF OBJECT_ID('dbo.Status_Audit_Delete', 'TR') IS NOT NULL
    DROP TRIGGER [dbo].[Status_Audit_Delete];
GO

CREATE OR ALTER TRIGGER [dbo].[Status_Audit_Delete]
ON [dbo].[Status]
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO [logging].[StatusLog]
        SELECT 
			'Delete', 
			'Information', 
			SYSDATETIMEOFFSET(), 
			d.*
        FROM DELETED d;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 
            ROLLBACK TRANSACTION;
        
		INSERT INTO [logging].[ExceptionLog] (UserId, Type, Message, ErrorNumber, ErrorProcedure, ErrorLine, ErrorTime) 
        VALUES (SYSTEM_USER, '[dbo].[Status_Audit_Delete]', ERROR_MESSAGE(), ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), SYSDATETIME());
        
		THROW;
    END CATCH
END
GO

-- Trigger: Status Type Audit for Insert
IF OBJECT_ID('dbo.StatusType_Audit_Insert', 'TR') IS NOT NULL
    DROP TRIGGER [dbo].[StatusType_Audit_Insert];
GO

CREATE OR ALTER TRIGGER [dbo].[StatusType_Audit_Insert]
ON [dbo].[StatusType]
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO [logging].[StatusTypeLog]
        SELECT 
			'Insert', 
			'Information', 
			SYSDATETIMEOFFSET(), 
			i.*
        FROM INSERTED i;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 
            ROLLBACK TRANSACTION;
        
		INSERT INTO [logging].[ExceptionLog] (UserId, Type, Message, ErrorNumber, ErrorProcedure, ErrorLine, ErrorTime) 
        VALUES (SYSTEM_USER, '[dbo].[StatusType_Audit_Insert]', ERROR_MESSAGE(), ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), SYSDATETIME());
        
		THROW;
    END CATCH
END
GO

-- Trigger: Status Type Audit for Update
IF OBJECT_ID('dbo.StatusType_Audit_Update', 'TR') IS NOT NULL
    DROP TRIGGER [dbo].[StatusType_Audit_Update];
GO

CREATE OR ALTER TRIGGER [dbo].[StatusType_Audit_Update]
ON [dbo].[StatusType]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        DECLARE @IsActiveExists BIT = 0;
        IF COL_LENGTH('dbo.StatusType', 'IsActive') IS NOT NULL SET @IsActiveExists = 1;

        -- Soft Delete: IsActive 1 → 0
        IF @IsActiveExists = 1
        BEGIN
            INSERT INTO [logging].[StatusTypeLog]
            SELECT 
				'SoftDelete', 'Information', SYSDATETIMEOFFSET(), i.*
            FROM INSERTED i
            JOIN DELETED d ON i.Id = d.Id
            WHERE d.IsActive = 1 AND i.IsActive = 0;

            -- Restore: IsActive 0 → 1
            INSERT INTO [logging].[StatusTypeLog]
            SELECT 
				'Restore', 'Information', SYSDATETIMEOFFSET(), i.*
            FROM INSERTED i
            JOIN DELETED d ON i.Id = d.Id
            WHERE d.IsActive = 0 AND i.IsActive = 1;
        END

        -- General update (excluding soft delete/restore)
        INSERT INTO [logging].[StatusTypeLog]
        SELECT 
			'Update', 'Information', SYSDATETIMEOFFSET(), i.*
        FROM INSERTED i
        JOIN DELETED d ON i.Id = d.Id
        WHERE 
            (@IsActiveExists = 0 OR i.IsActive = d.IsActive) -- IsActive didn't change
            AND NOT (d.IsActive = 1 AND i.IsActive = 0)       -- Not soft delete
            AND NOT (d.IsActive = 0 AND i.IsActive = 1)       -- Not restore
            AND EXISTS (SELECT i.* EXCEPT SELECT d.*);        -- Other columns changed
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 
            ROLLBACK TRANSACTION;
        
		INSERT INTO [logging].[ExceptionLog] (UserId, Type, Message, ErrorNumber, ErrorProcedure, ErrorLine, ErrorTime) 
        VALUES (SYSTEM_USER, '[dbo].[StatusType_Audit_Update]', ERROR_MESSAGE(), ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), SYSDATETIME());
        
		THROW;
    END CATCH
END
GO

-- Trigger: Status Type Audit for Delete
IF OBJECT_ID('dbo.StatusType_Audit_Delete', 'TR') IS NOT NULL
    DROP TRIGGER [dbo].[StatusType_Audit_Delete];
GO

CREATE OR ALTER TRIGGER [dbo].[StatusType_Audit_Delete]
ON [dbo].[StatusType]
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO [logging].[StatusTypeLog]
        SELECT 
			'Delete', 
			'Information', 
			SYSDATETIMEOFFSET(), 
			d.*
        FROM DELETED d;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 
            ROLLBACK TRANSACTION;
        
		INSERT INTO [logging].[ExceptionLog] (UserId, Type, Message, ErrorNumber, ErrorProcedure, ErrorLine, ErrorTime) 
        VALUES (SYSTEM_USER, '[dbo].[StatusType_Audit_Delete]', ERROR_MESSAGE(), ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), SYSDATETIME());
        
		THROW;
    END CATCH
END
GO

-- Trigger: Transection Audit for Insert
IF OBJECT_ID('dbo.[Transection_Audit_Insert]', 'TR') IS NOT NULL
    DROP TRIGGER [dbo].[Transection_Audit_Insert];
GO

CREATE OR ALTER TRIGGER [dbo].[Transection_Audit_Insert]
ON [dbo].[Transection]
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO [logging].[TransectionLog]
        SELECT 
			'Insert', 
			'Information', 
			SYSDATETIMEOFFSET(), 
			i.*
        FROM INSERTED i;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 
            ROLLBACK TRANSACTION;
        
		INSERT INTO [logging].[ExceptionLog] (UserId, Type, Message, ErrorNumber, ErrorProcedure, ErrorLine, ErrorTime) 
        VALUES (SYSTEM_USER, '[dbo].[Transection_Audit_Insert]', ERROR_MESSAGE(), ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), SYSDATETIME());
        
		THROW;
    END CATCH
END
GO

-- Trigger: Transection Audit for Update
IF OBJECT_ID('dbo.[Transection_Audit_Update]', 'TR') IS NOT NULL
    DROP TRIGGER [dbo].[Transection_Audit_Update];
GO

CREATE OR ALTER TRIGGER [dbo].[Transection_Audit_Update]
ON [dbo].[Transection]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        DECLARE @IsActiveExists BIT = 0;
        IF COL_LENGTH('dbo.Transection', 'IsActive') IS NOT NULL SET @IsActiveExists = 1;

        -- Soft Delete: IsActive 1 → 0
        IF @IsActiveExists = 1
        BEGIN
            INSERT INTO [logging].[TransectionLog]
            SELECT 
				'SoftDelete', 'Information', SYSDATETIMEOFFSET(), i.*
            FROM INSERTED i
            JOIN DELETED d ON i.Id = d.Id
            WHERE d.IsActive = 1 AND i.IsActive = 0;

            -- Restore: IsActive 0 → 1
            INSERT INTO [logging].[TransectionLog]
            SELECT 
				'Restore', 'Information', SYSDATETIMEOFFSET(), i.*
            FROM INSERTED i
            JOIN DELETED d ON i.Id = d.Id
            WHERE d.IsActive = 0 AND i.IsActive = 1;
        END

        -- General update (excluding soft delete/restore)
        INSERT INTO [logging].[TransectionLog]
        SELECT 
			'Update', 'Information', SYSDATETIMEOFFSET(), i.*
        FROM INSERTED i
        JOIN DELETED d ON i.Id = d.Id
        WHERE 
            (@IsActiveExists = 0 OR i.IsActive = d.IsActive) -- IsActive didn't change
            AND NOT (d.IsActive = 1 AND i.IsActive = 0)       -- Not soft delete
            AND NOT (d.IsActive = 0 AND i.IsActive = 1)       -- Not restore
            AND EXISTS (SELECT i.* EXCEPT SELECT d.*);        -- Other columns changed
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 
            ROLLBACK TRANSACTION;
        
		INSERT INTO [logging].[ExceptionLog] (UserId, Type, Message, ErrorNumber, ErrorProcedure, ErrorLine, ErrorTime) 
        VALUES (SYSTEM_USER, '[dbo].[Transection_Audit_Update]', ERROR_MESSAGE(), ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), SYSDATETIME());
        
		THROW;
    END CATCH
END
GO

-- Trigger: Transection Audit for Delete
IF OBJECT_ID('dbo.[Transection_Audit_Delete]', 'TR') IS NOT NULL
    DROP TRIGGER [dbo].[Transection_Audit_Delete];
GO

CREATE OR ALTER TRIGGER [dbo].[Transection_Audit_Delete]
ON [dbo].[Transection]
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO [logging].[TransectionLog]
        SELECT 
			'Delete', 
			'Information', 
			SYSDATETIMEOFFSET(), 
			d.*
        FROM DELETED d;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 
            ROLLBACK TRANSACTION;
        
		INSERT INTO [logging].[ExceptionLog] (UserId, Type, Message, ErrorNumber, ErrorProcedure, ErrorLine, ErrorTime) 
        VALUES (SYSTEM_USER, '[dbo].[Transection_Audit_Delete]', ERROR_MESSAGE(), ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), SYSDATETIME());
        
		THROW;
    END CATCH
END
GO

-- Trigger: Transection trigger
IF OBJECT_ID('dbo.[Transection_Trigger]', 'TR') IS NOT NULL
    DROP TRIGGER [dbo].[Transection_Trigger];
GO

CREATE OR ALTER TRIGGER [dbo].[Transection_Trigger]
ON [dbo].[Transection]
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION;
    BEGIN TRY
        DECLARE @BookId BIGINT, @UserId BIGINT, @BookStatusId BIGINT, @IsBorrowReservedBook BIT = 0, @ShouldDeactivate BIT;

        -- Retrieve values from INSERTED
        SELECT @BookId = BookId, @UserId = UserId FROM INSERTED;

		-- Retrive current book status with lock to prevent concurrency issues
		SELECT @BookStatusId = StatusId FROM [dbo].[Books] WITH (UPDLOCK, ROWLOCK) WHERE Id = @BookId AND IsActive = 1;
		
        -- Handle INSERT Operations if inserted is not cancle, returned or claimedLost
        IF EXISTS (SELECT 1 FROM INSERTED WHERE NOT EXISTS (SELECT 1 FROM DELETED WHERE INSERTED.Id = DELETED.Id))
			AND
			EXISTS (SELECT * FROM INSERTED WHERE StatusId NOT IN ([dbo].[udfGetStatusNumber]('Transaction', 'Cancelled'), 
                                                                 [dbo].[udfGetStatusNumber]('Transaction', 'Returned'),
																 [dbo].[udfGetStatusNumber]('Transaction', 'ClaimedLost')))
        BEGIN
			-- Full-Fill reservation if done for this book by given user
            IF EXISTS (SELECT 1 FROM [dbo].[Reservation]
                       WHERE IsActive = 1 
                         AND BookId = @BookId 
                         AND UserId = @UserId 
                         AND StatusId NOT IN ([dbo].[udfGetStatusNumber]('Reservations', 'Cancelled'), 
                                              [dbo].[udfGetStatusNumber]('Reservations', 'Fulfilled')))
            BEGIN
				-- Set flag to define transaction is done for allocated reserve book
				IF EXISTS (
				    SELECT 1 FROM [dbo].[Reservation]
				    WHERE IsActive = 1 
				      AND BookId = @BookId 
				      AND UserId = @UserId 
				      AND IsAllocated = 1
				      AND StatusId = [dbo].[udfGetStatusNumber]('Reservations', 'Allocated')
				)
				BEGIN
				    SELECT @IsBorrowReservedBook = 1;
				END
								
                UPDATE [dbo].[Reservation]
                SET StatusId = [dbo].[udfGetStatusNumber]('Reservations', 'Fulfilled'), ModifiedBy = @UserId, ModifiedAt = GETDATE()
                WHERE IsActive = 1 
                  AND BookId = @BookId 
                  AND UserId = @UserId 
                  AND StatusId NOT IN ([dbo].[udfGetStatusNumber]('Reservations', 'Cancelled'), 
                                       [dbo].[udfGetStatusNumber]('Reservations', 'Fulfilled'));
            END
			
			-- Flag for reservation existence
			DECLARE @HasActiveReservations BIT = (
			    SELECT CASE 
			        WHEN EXISTS (
			            SELECT 1 FROM [dbo].[Reservation] 
			            WHERE IsActive = 1 
			              AND BookId = @BookId 
			              AND IsAllocated = 1 
			              AND StatusId NOT IN ([dbo].[udfGetStatusNumber]('Reservations', 'Cancelled'), [dbo].[udfGetStatusNumber]('Reservations',	'Fulfilled'))
			        ) THEN 1 ELSE 0 
			    END
			);

			-- Update book status based on reservation existence
			If (@BookStatusId = [dbo].[udfGetStatusNumber]('Book', 'Reserved') AND @HasActiveReservations = 0)
			BEGIN
				UPDATE [dbo].[Books]
				SET StatusId = [dbo].[udfGetStatusNumber]('Book', 'CheckedOut'), ModifiedBy = @UserId, ModifiedAt = GETDATE()
				WHERE Id = @BookId AND IsActive = 1;
			END
			ELSE IF (@BookStatusId = [dbo].[udfGetStatusNumber]('Book', 'Available'))
			BEGIN
				IF (@IsBorrowReservedBook = 0)
				BEGIN
					-- Adjust available copies safely
					IF EXISTS (SELECT TOP 1 Id FROM [dbo].[Books] Where Id = @BookId AND IsActive = 1 AND AvailableCopies - 1 > 0)
					BEGIN
						UPDATE [dbo].[Books]
						SET AvailableCopies = AvailableCopies - 1, ModifiedBy = @UserId, ModifiedAt = GETDATE()
						WHERE Id = @BookId AND IsActive = 1;
					END
					ELSE
					BEGIN
						-- Manage book status based on reservation presence
						IF (@HasActiveReservations = 0)
						BEGIN
							UPDATE [dbo].[Books]
							SET StatusId = [dbo].[udfGetStatusNumber]('Book', 'CheckedOut'), AvailableCopies = AvailableCopies - 1, ModifiedBy = @UserId, ModifiedAt = GETDATE()
							WHERE Id = @BookId AND IsActive = 1;
						END
						ELSE
						BEGIN
							UPDATE [dbo].[Books]
							SET StatusId = [dbo].[udfGetStatusNumber]('Book', 'Reserved'), AvailableCopies = AvailableCopies - 1, ModifiedBy = @UserId, ModifiedAt = GETDATE()
							WHERE Id = @BookId AND IsActive = 1;
						END
					END
				END
			END
        END

		-- Handle INSERT Operations if inserted is claimedLost
		IF EXISTS (SELECT 1 FROM INSERTED WHERE NOT EXISTS (SELECT 1 FROM DELETED WHERE INSERTED.Id = DELETED.Id))
			AND
			EXISTS (SELECT * FROM INSERTED WHERE StatusId = [dbo].[udfGetStatusNumber]('Transaction', 'ClaimedLost'))
		BEGIN
			UPDATE [dbo].[Books]
			SET AvailableCopies = AvailableCopies - 1, ModifiedBy = @UserId, ModifiedAt = GETDATE()
			WHERE Id = @BookId AND IsActive = 1;
		END

        -- Handle UPDATE Operations
		IF EXISTS (SELECT 1 FROM INSERTED WHERE EXISTS (SELECT 1 FROM DELETED WHERE INSERTED.Id = DELETED.Id))
        BEGIN
			-- Handle SOFT DELETE Operation
			IF (COL_LENGTH('[dbo].[Transection]', 'IsActive') IS NOT NULL 
				AND EXISTS (SELECT 1 FROM INSERTED WHERE EXISTS (SELECT 1 FROM DELETED WHERE INSERTED.IsActive = 0 AND DELETED.IsActive = 1)))
		    BEGIN
				IF EXISTS (SELECT * FROM DELETED WHERE StatusId NOT IN ([dbo].[udfGetStatusNumber]('Transaction', 'Cancelled'), 
				                                                  [dbo].[udfGetStatusNumber]('Transaction', 'Returned'),
																  [dbo].[udfGetStatusNumber]('Transaction', 'ClaimedLost')))
				BEGIN
					-- Check condition for deactivation
					SELECT @ShouldDeactivate = CASE 
					    WHEN NOT EXISTS (
					        SELECT 1 FROM [dbo].[Transection]
					        WHERE BookId = @BookId 
					        AND StatusId NOT IN ([dbo].[udfGetStatusNumber]('Transaction', 'Cancelled'), 
					                             [dbo].[udfGetStatusNumber]('Transaction', 'Returned'),
												 [dbo].[udfGetStatusNumber]('Transaction', 'ClaimedLost'))
					    ) 
					    AND @BookStatusId = [dbo].[udfGetStatusNumber]('Book', 'Removed')
					    THEN 1 ELSE 0
					END;
					
					-- Update Books table based on condition
					UPDATE [dbo].[Books]
					SET StatusId = CASE 
					    WHEN @BookStatusId IN ([dbo].[udfGetStatusNumber]('Book', 'CheckedOut'), 
					                            [dbo].[udfGetStatusNumber]('Book', 'Reserved')) 
					    THEN [dbo].[udfGetStatusNumber]('Book', 'Available')
					    ELSE StatusId 
					END,
					    AvailableCopies = AvailableCopies + 1, 
					    ModifiedBy = CASE WHEN @ShouldDeactivate = 1 THEN ModifiedBy ELSE @UserId END,
					    ModifiedAt = CASE WHEN @ShouldDeactivate = 1 THEN ModifiedAt ELSE GETDATE() END,
					    IsActive = CASE WHEN @ShouldDeactivate = 1 THEN 0 ELSE 1 END,
					    DeletedAt = CASE WHEN @ShouldDeactivate = 1 THEN GETDATE() ELSE NULL END,
					    DeletedBy = CASE WHEN @ShouldDeactivate = 1 THEN @UserId ELSE NULL END
					WHERE Id = @BookId AND IsActive = 1;
				END
		    END
			-- Handle book return and cancel
            ELSE IF EXISTS (SELECT * FROM INSERTED WHERE StatusId IN ([dbo].[udfGetStatusNumber]('Transaction', 'Cancelled'), 
                                                                 [dbo].[udfGetStatusNumber]('Transaction', 'Returned')))
			   AND
			   EXISTS (SELECT * FROM DELETED WHERE StatusId NOT IN ([dbo].[udfGetStatusNumber]('Transaction', 'Cancelled'), 
                                                                 [dbo].[udfGetStatusNumber]('Transaction', 'Returned'),
																 [dbo].[udfGetStatusNumber]('Transaction', 'ClaimedLost')))
            BEGIN
				-- Check condition for deactivation
				SELECT @ShouldDeactivate = CASE 
				    WHEN NOT EXISTS (
				        SELECT 1 FROM [dbo].[Transection]
				        WHERE BookId = @BookId 
				        AND StatusId NOT IN ([dbo].[udfGetStatusNumber]('Transaction', 'Cancelled'), 
				                             [dbo].[udfGetStatusNumber]('Transaction', 'Returned'),
											 [dbo].[udfGetStatusNumber]('Transaction', 'ClaimedLost'))
				    ) 
				    AND @BookStatusId = [dbo].[udfGetStatusNumber]('Book', 'Removed')
				    THEN 1 ELSE 0
				END;
				
				-- Update Books table based on condition
				UPDATE [dbo].[Books]
				SET StatusId = CASE 
				    WHEN @BookStatusId IN ([dbo].[udfGetStatusNumber]('Book', 'CheckedOut'), 
				                            [dbo].[udfGetStatusNumber]('Book', 'Reserved')) 
				    THEN [dbo].[udfGetStatusNumber]('Book', 'Available')
				    ELSE StatusId 
				END,
				    AvailableCopies = AvailableCopies + 1, 
				    ModifiedBy = CASE WHEN @ShouldDeactivate = 1 THEN ModifiedBy ELSE @UserId END,
				    ModifiedAt = CASE WHEN @ShouldDeactivate = 1 THEN ModifiedAt ELSE GETDATE() END,
				    IsActive = CASE WHEN @ShouldDeactivate = 1 THEN 0 ELSE 1 END,
				    DeletedAt = CASE WHEN @ShouldDeactivate = 1 THEN GETDATE() ELSE NULL END,
				    DeletedBy = CASE WHEN @ShouldDeactivate = 1 THEN @UserId ELSE NULL END
				WHERE Id = @BookId AND IsActive = 1;
            END
			-- Handle claimed to lost for the holding book
			ELSE IF EXISTS (SELECT * FROM INSERTED WHERE StatusId = [dbo].[udfGetStatusNumber]('Transaction', 'ClaimedLost'))
			   AND
			   EXISTS (SELECT * FROM DELETED WHERE StatusId IN ([dbo].[udfGetStatusNumber]('Transaction', 'Cancelled'), 
                                                                 [dbo].[udfGetStatusNumber]('Transaction', 'Returned'),
																 [dbo].[udfGetStatusNumber]('Transaction', 'ClaimedLost')))
            BEGIN
				-- Check condition for deactivation
				SELECT @ShouldDeactivate = CASE 
				    WHEN NOT EXISTS (
				        SELECT 1 FROM [dbo].[Transection]
				        WHERE BookId = @BookId 
				        AND StatusId NOT IN ([dbo].[udfGetStatusNumber]('Transaction', 'Cancelled'), 
				                             [dbo].[udfGetStatusNumber]('Transaction', 'Returned'),
											 [dbo].[udfGetStatusNumber]('Transaction', 'ClaimedLost'))
				    ) 
				    AND @BookStatusId = [dbo].[udfGetStatusNumber]('Book', 'Removed')
				    THEN 1 ELSE 0
				END;
				
				-- Update Books table based on condition
				UPDATE [dbo].[Books]
				SET StatusId = CASE 
				    WHEN @BookStatusId IN ([dbo].[udfGetStatusNumber]('Book', 'CheckedOut'), 
				                            [dbo].[udfGetStatusNumber]('Book', 'Reserved')) 
				    THEN [dbo].[udfGetStatusNumber]('Book', 'Available')
				    ELSE StatusId 
				END,
				    AvailableCopies = AvailableCopies - 1, 
				    ModifiedBy = CASE WHEN @ShouldDeactivate = 1 THEN ModifiedBy ELSE @UserId END,
				    ModifiedAt = CASE WHEN @ShouldDeactivate = 1 THEN ModifiedAt ELSE GETDATE() END,
				    IsActive = CASE WHEN @ShouldDeactivate = 1 THEN 0 ELSE 1 END,
				    DeletedAt = CASE WHEN @ShouldDeactivate = 1 THEN GETDATE() ELSE NULL END,
				    DeletedBy = CASE WHEN @ShouldDeactivate = 1 THEN @UserId ELSE NULL END
				WHERE Id = @BookId AND IsActive = 1;
            END
        END
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;

		DECLARE @ErrorMessage NVARCHAR(4000);
        DECLARE @ErrorSeverity INT;
        DECLARE @ErrorState INT;
		
		INSERT INTO [logging].[ExceptionLog] (UserId, Type, Message, ErrorNumber, ErrorProcedure, ErrorLine, ErrorTime) values (@userId, '[dbo].[Transection_Trigger]', ERROR_MESSAGE(), ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), GETDATE())
		
		SELECT 
			@ErrorMessage = ERROR_MESSAGE(),
			@ErrorSeverity = ERROR_SEVERITY(),
			@ErrorState = ERROR_STATE();

		RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH;
END;

-- Trigger: User Audit for Insert
IF OBJECT_ID('dbo.User_Audit_Insert', 'TR') IS NOT NULL
    DROP TRIGGER [dbo].[User_Audit_Insert];
GO

CREATE OR ALTER TRIGGER [dbo].[User_Audit_Insert]
ON [dbo].[User]
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO [logging].[UserLog]
        SELECT 
			'Insert', 
			'Information', 
			SYSDATETIMEOFFSET(), 
			i.*
        FROM INSERTED i;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 
            ROLLBACK TRANSACTION;
        
		INSERT INTO [logging].[ExceptionLog] (UserId, Type, Message, ErrorNumber, ErrorProcedure, ErrorLine, ErrorTime)
        VALUES (SYSTEM_USER, '[dbo].[User_Audit_Insert]', ERROR_MESSAGE(), ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), SYSDATETIME());
        
		THROW;
    END CATCH
END
GO

-- Trigger: User Audit for Update
IF OBJECT_ID('dbo.User_Audit_Update', 'TR') IS NOT NULL
    DROP TRIGGER [dbo].[User_Audit_Update];
GO

CREATE OR ALTER TRIGGER [dbo].[User_Audit_Update]
ON [dbo].[User]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        DECLARE @IsActiveExists BIT = 0;
        IF COL_LENGTH('dbo.User', 'IsActive') IS NOT NULL SET @IsActiveExists = 1;

        -- Soft Delete: IsActive 1 → 0
        IF @IsActiveExists = 1
        BEGIN
            INSERT INTO [logging].[UserLog]
            SELECT 
				'SoftDelete', 'Information', SYSDATETIMEOFFSET(), i.*
            FROM INSERTED i
            JOIN DELETED d ON i.Id = d.Id
            WHERE d.IsActive = 1 AND i.IsActive = 0;

            -- Restore: IsActive 0 → 1
            INSERT INTO [logging].[UserLog]
            SELECT 
				'Restore', 'Information', SYSDATETIMEOFFSET(), i.*
            FROM INSERTED i
            JOIN DELETED d ON i.Id = d.Id
            WHERE d.IsActive = 0 AND i.IsActive = 1;
        END

        -- General update (excluding soft delete/restore)
        INSERT INTO [logging].[UserLog]
        SELECT 
			'Update', 'Information', SYSDATETIMEOFFSET(), i.*
        FROM INSERTED i
        JOIN DELETED d ON i.Id = d.Id
        WHERE 
            (@IsActiveExists = 0 OR i.IsActive = d.IsActive) -- IsActive didn't change
            AND NOT (d.IsActive = 1 AND i.IsActive = 0)       -- Not soft delete
            AND NOT (d.IsActive = 0 AND i.IsActive = 1)       -- Not restore
            AND EXISTS (SELECT i.* EXCEPT SELECT d.*);        -- Other columns changed
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 
            ROLLBACK TRANSACTION;
        
		INSERT INTO [logging].[ExceptionLog] (UserId, Type, Message, ErrorNumber, ErrorProcedure, ErrorLine, ErrorTime)
        VALUES (SYSTEM_USER, '[dbo].[User_Audit_Update]', ERROR_MESSAGE(), ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), SYSDATETIME());
        
		THROW;
    END CATCH
END
GO

-- Trigger: User Audit for Delete
IF OBJECT_ID('dbo.User_Audit_Delete', 'TR') IS NOT NULL
    DROP TRIGGER [dbo].[User_Audit_Delete];
GO

CREATE OR ALTER TRIGGER [dbo].[User_Audit_Delete]
ON [dbo].[User]
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO [logging].[UserLog]
        SELECT 
			'Delete', 
			'Information', 
			SYSDATETIMEOFFSET(), 
			d.*
        FROM DELETED d;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 
            ROLLBACK TRANSACTION;
        
		INSERT INTO [logging].[ExceptionLog] (UserId, Type, Message, ErrorNumber, ErrorProcedure, ErrorLine, ErrorTime)
        VALUES (SYSTEM_USER, '[dbo].[User_Audit_Delete]', ERROR_MESSAGE(), ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), SYSDATETIME());
        
		THROW;
    END CATCH
END
GO

-- Trigger: UserMembershipMapping Audit for Insert
IF OBJECT_ID('dbo.UserMembershipMapping_Audit_Insert', 'TR') IS NOT NULL
    DROP TRIGGER [dbo].[UserMembershipMapping_Audit_Insert];
GO

CREATE OR ALTER TRIGGER [dbo].[UserMembershipMapping_Audit_Insert]
ON [dbo].[UserMembershipMapping]
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO [logging].[UserMembershipMappingLog]
        SELECT 
			'Insert', 
			'Information', 
			SYSDATETIMEOFFSET(), 
			i.*
        FROM INSERTED i;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 
            ROLLBACK TRANSACTION;
        
		INSERT INTO [logging].[ExceptionLog] (UserId, Type, Message, ErrorNumber, ErrorProcedure, ErrorLine, ErrorTime)
        VALUES (SYSTEM_USER, '[dbo].[UserMembershipMapping_Audit_Insert]', ERROR_MESSAGE(), ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), SYSDATETIME());
        
		THROW;
    END CATCH
END
GO

-- Trigger: UserMembershipMapping Audit for Update
IF OBJECT_ID('dbo.UserMembershipMapping_Audit_Update', 'TR') IS NOT NULL
    DROP TRIGGER [dbo].[UserMembershipMapping_Audit_Upate];
GO

CREATE OR ALTER TRIGGER [dbo].[UserMembershipMapping_Audit_Update]
ON [dbo].[UserMembershipMapping]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        DECLARE @IsActiveExists BIT = 0;
        IF COL_LENGTH('dbo.UserMembershipMapping', 'IsActive') IS NOT NULL SET @IsActiveExists = 1;

        -- Soft Delete: IsActive 1 → 0
        IF @IsActiveExists = 1
        BEGIN
            INSERT INTO [logging].[UserMembershipMappingLog]
            SELECT 
				'SoftDelete', 'Information', SYSDATETIMEOFFSET(), i.*
            FROM INSERTED i
            JOIN DELETED d ON i.Id = d.Id
            WHERE d.IsActive = 1 AND i.IsActive = 0;

            -- Restore: IsActive 0 → 1
            INSERT INTO [logging].[UserMembershipMappingLog]
            SELECT 
				'Restore', 'Information', SYSDATETIMEOFFSET(), i.*
            FROM INSERTED i
            JOIN DELETED d ON i.Id = d.Id
            WHERE d.IsActive = 0 AND i.IsActive = 1;
        END

		-- General update (excluding soft delete/restore)
        INSERT INTO [logging].[UserMembershipMappingLog]
        SELECT 
            'Update', 'Information', SYSDATETIMEOFFSET(), i.*
        FROM INSERTED i
        JOIN DELETED d ON i.Id = d.Id
        WHERE 
            (@IsActiveExists = 0 OR i.IsActive = d.IsActive) -- IsActive didn't change
            AND NOT (d.IsActive = 1 AND i.IsActive = 0)       -- Not soft delete
            AND NOT (d.IsActive = 0 AND i.IsActive = 1)       -- Not restore
            AND EXISTS (SELECT i.* EXCEPT SELECT d.*);        -- Other columns changed
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 
            ROLLBACK TRANSACTION;
        
		INSERT INTO [logging].[ExceptionLog] (UserId, Type, Message, ErrorNumber, ErrorProcedure, ErrorLine, ErrorTime)
        VALUES (SYSTEM_USER, '[dbo].[UserMembershipMapping_Audit_Update]', ERROR_MESSAGE(), ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), SYSDATETIME());
        
		THROW;
    END CATCH
END
GO

-- Trigger: UserMembershipMapping Audit for Delete
IF OBJECT_ID('dbo.UserMembershipMapping_Audit_Delete', 'TR') IS NOT NULL
    DROP TRIGGER [dbo].[UserMembershipMapping_Audit_Delete];
GO

CREATE OR ALTER TRIGGER [dbo].[UserMembershipMapping_Audit_Delete]
ON [dbo].[UserMembershipMapping]
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO [logging].[UserMembershipMappingLog]
        SELECT 
			'Delete', 
			'Information', 
			SYSDATETIMEOFFSET(), 
			d.*
        FROM DELETED d;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 
            ROLLBACK TRANSACTION;
        
		INSERT INTO [logging].[ExceptionLog] (UserId, Type, Message, ErrorNumber, ErrorProcedure, ErrorLine, ErrorTime)
        VALUES (SYSTEM_USER, '[dbo].[UserMembershipMapping_Audit_Delete]', ERROR_MESSAGE(), ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), SYSDATETIME());
        
		THROW;
    END CATCH
END
GO