/*
==================================================================================
Library Management System - Complete Function setup Script
==================================================================================
This script creates:
- All scalar-valued functions
==================================================================================
*/

-- Function to get config keyValue using keyName
IF OBJECT_ID('dbo.udfGetConfigValue', 'FN') IS NOT NULL
    DROP FUNCTION [dbo].[udfGetConfigValue];
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER FUNCTION [dbo].[udfGetConfigValue] (@KeyName NVARCHAR(50))
RETURNS NVARCHAR(1000)
AS
BEGIN
    DECLARE @KeyValue NVARCHAR(1000);
    SELECT @KeyValue = KeyValue FROM [dbo].[Configs] 
	WHERE KeyName = @KeyName;

    RETURN @KeyValue;
END;
GO

-- Function to get statusId from using statusType and statusLabel
IF OBJECT_ID('dbo.udfGetStatusNumber', 'FN') IS NOT NULL
    DROP FUNCTION [dbo].[udfGetStatusNumber];
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER FUNCTION [dbo].[udfGetStatusNumber] (@StatusType NVARCHAR(50), @StatueLabel NVARCHAR(50))
RETURNS BIGINT
AS
BEGIN
    DECLARE @Status BIGINT;
    SELECT @Status = s.Id FROM [dbo].[Status] s
	JOIN [dbo].[StatusType] st ON s.StatusTypeId = st.Id
	WHERE st.Label = @StatusType AND s.Label = @StatueLabel;

    RETURN @Status;
END;
GO
