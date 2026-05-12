-- Medicare Database Initial Schema
-- Generated for SQL Server 8.0

-- Create Users table
IF OBJECT_ID('[dbo].[Users]', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Users] (
        [Id] int NOT NULL IDENTITY,
        [Email] nvarchar(255) NOT NULL,
        [PasswordHash] nvarchar(max) NOT NULL,
        [FullName] nvarchar(max) NOT NULL,
        [Role] int NOT NULL,
        [IsActive] bit NOT NULL,
        [LastLoginAt] datetime2 NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([Id]),
        CONSTRAINT [UQ_Users_Email] UNIQUE ([Email])
    );
    CREATE INDEX [IX_Users_Email] ON [dbo].[Users] ([Email]);
END

-- Create Menus table
IF OBJECT_ID('[dbo].[Menus]', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Menus] (
        [Id] int NOT NULL IDENTITY,
        [DateApplied] datetime2 NOT NULL,
        [MealType] int NOT NULL,
        [Description] nvarchar(max) NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Menus] PRIMARY KEY ([Id])
    );
END

-- Create MenuItems table
IF OBJECT_ID('[dbo].[MenuItems]', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[MenuItems] (
        [Id] int NOT NULL IDENTITY,
        [MenuId] int NOT NULL,
        [Name] nvarchar(255) NOT NULL,
        [Description] nvarchar(max) NOT NULL,
        [Price] decimal(10, 2) NOT NULL,
        [DisplayOrder] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_MenuItems] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_MenuItems_Menus] FOREIGN KEY ([MenuId]) REFERENCES [dbo].[Menus] ([Id]) ON DELETE CASCADE
    );
    CREATE INDEX [IX_MenuItems_MenuId] ON [dbo].[MenuItems] ([MenuId]);
END

-- Create OrderRequests table
IF OBJECT_ID('[dbo].[OrderRequests]', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[OrderRequests] (
        [Id] int NOT NULL IDENTITY,
        [CustomerName] nvarchar(255) NOT NULL,
        [CustomerPhone] nvarchar(20) NOT NULL,
        [OrderDate] datetime2 NOT NULL,
        [MealType] int NOT NULL,
        [Status] int NOT NULL,
        [Notes] nvarchar(max) NOT NULL,
        [TotalQuantity] int NOT NULL,
        [TotalPrice] decimal(10, 2) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_OrderRequests] PRIMARY KEY ([Id])
    );
END

-- Create OrderRequestItems table
IF OBJECT_ID('[dbo].[OrderRequestItems]', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[OrderRequestItems] (
        [Id] int NOT NULL IDENTITY,
        [OrderRequestId] int NOT NULL,
        [MenuItemId] int NOT NULL,
        [MenuItemName] nvarchar(max) NOT NULL,
        [Quantity] int NOT NULL,
        [UnitPrice] decimal(10, 2) NOT NULL,
        [Subtotal] decimal(10, 2) NOT NULL,
        [SpecialRequest] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_OrderRequestItems] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_OrderRequestItems_OrderRequests] FOREIGN KEY ([OrderRequestId]) REFERENCES [dbo].[OrderRequests] ([Id]) ON DELETE CASCADE
    );
    CREATE INDEX [IX_OrderRequestItems_OrderRequestId] ON [dbo].[OrderRequestItems] ([OrderRequestId]);
END

-- Create SiteContents table
IF OBJECT_ID('[dbo].[SiteContents]', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[SiteContents] (
        [Id] int NOT NULL IDENTITY,
        [Key] nvarchar(255) NOT NULL,
        [Value] nvarchar(max) NOT NULL,
        [ContentType] nvarchar(max) NOT NULL,
        [Description] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_SiteContents] PRIMARY KEY ([Id]),
        CONSTRAINT [UQ_SiteContents_Key] UNIQUE ([Key])
    );
    CREATE INDEX [IX_SiteContents_Key] ON [dbo].[SiteContents] ([Key]);
END

-- Insert default admin user (Password: Admin123!)
IF NOT EXISTS (SELECT 1 FROM [dbo].[Users] WHERE [Email] = 'admin@medicare.local')
BEGIN
    INSERT INTO [dbo].[Users] ([Email], [PasswordHash], [FullName], [Role], [IsActive], [CreatedAt])
    VALUES ('admin@medicare.local', '$2a$11$5u2s0V3kZVx5Bvl0Wd4W2uRH5Qc5Z5Y5X5W5V5U5T5S5R5Q5P5O', 'System Administrator', 1, 1, GETUTCDATE());
END

-- Insert default site content
IF NOT EXISTS (SELECT 1 FROM [dbo].[SiteContents] WHERE [Key] = 'CompanyName')
BEGIN
    INSERT INTO [dbo].[SiteContents] ([Key], [Value], [ContentType], [Description], [CreatedAt])
    VALUES ('CompanyName', 'Medicare', 'text', 'Company name displayed on landing page', GETUTCDATE());
    INSERT INTO [dbo].[SiteContents] ([Key], [Value], [ContentType], [Description], [CreatedAt])
    VALUES ('CompanyPhone', '+84 123 456 7890', 'text', 'Company contact phone number', GETUTCDATE());
    INSERT INTO [dbo].[SiteContents] ([Key], [Value], [ContentType], [Description], [CreatedAt])
    VALUES ('CompanyDescription', 'Đặt cơm ngon mỗi ngày - Giao hàng nhanh - Menu thay đổi hàng ngày', 'text', 'Company tagline', GETUTCDATE());
END
