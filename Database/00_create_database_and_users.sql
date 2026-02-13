/*
==================================================================================
This script creates:
- DB, DB user / roles, Grant privileges
==================================================================================
*/

-- ============================================================================
-- SECTION: DATABASE CREATION (Optional - comment out if database exists)
-- ============================================================================

USE master;
GO

IF DB_ID('LibraryManagementSys') IS NOT NULL
BEGIN
    ALTER DATABASE LibraryManagementSys SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE LibraryManagementSys;
END
GO

CREATE DATABASE LibraryManagementSys;
GO

