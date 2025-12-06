--create database Pharma;
--go
--use Pharma;
--go

/****************************************
    DROP TABLES SAFELY
****************************************/
IF OBJECT_ID('invoice_items') IS NOT NULL DROP TABLE invoice_items;
IF OBJECT_ID('drug_tags') IS NOT NULL DROP TABLE drug_tags;
IF OBJECT_ID('invoices') IS NOT NULL DROP TABLE invoices;
IF OBJECT_ID('tags') IS NOT NULL DROP TABLE tags;
IF OBJECT_ID('admin_logs') IS NOT NULL DROP TABLE admin_logs;
IF OBJECT_ID('drugs') IS NOT NULL DROP TABLE drugs;
IF OBJECT_ID('admins') IS NOT NULL DROP TABLE admins;
GO

/****************************************
    Admin Users
****************************************/
CREATE TABLE admins (
    admin_id        INT IDENTITY(1,1) PRIMARY KEY,
    username        NVARCHAR(255) NOT NULL UNIQUE,
    email           NVARCHAR(255) NOT NULL UNIQUE,
    password_hash   NVARCHAR(512) NOT NULL,
    role            NVARCHAR(50) NOT NULL CHECK (role IN ('super_admin', 'cashier')),
    created_at      DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);
GO

/****************************************
    Admin Logs (Login / Logout)
****************************************/
CREATE TABLE admin_logs (
    log_id          INT IDENTITY(1,1) PRIMARY KEY,
    admin_id        INT NOT NULL,
    action_type     NVARCHAR(50) NOT NULL,      -- login | logout
    action_time     DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT fk_admin_logs_admin FOREIGN KEY (admin_id)
        REFERENCES admins(admin_id)
        ON DELETE CASCADE
);
GO

/****************************************
    Drugs Table (UPDATED)
****************************************/
CREATE TABLE drugs (
    drug_id             INT IDENTITY(1,1) PRIMARY KEY,
    name                NVARCHAR(255) NOT NULL,
    selling_price       DECIMAL(18,2) NOT NULL,
    purchasing_price    DECIMAL(18,2) NOT NULL,
    barcode             NVARCHAR(255) NULL UNIQUE,
    image_url           NVARCHAR(1000) NULL,
    description_before_use   NVARCHAR(MAX) NULL,
    description_how_to_use   NVARCHAR(MAX) NULL,
    description_side_effects NVARCHAR(MAX) NULL,
    requires_prescription BIT NOT NULL DEFAULT 0,
    drug_type NVARCHAR(50) NOT NULL CHECK (drug_type IN 
        ('tablet', 'syrup', 'injection', 'capsule', 'cream', 'gel', 'spray', 'drops')),
    manufacturer        NVARCHAR(255) NULL,
    expiration_date     DATE NULL,
    shelf_amount        INT NOT NULL DEFAULT 0,
    stored_amount       INT NOT NULL DEFAULT 0,
    low_amount          INT NOT NULL DEFAULT 10,
    -- NEW FIELD YOU REQUESTED
    sub_amount_quantity INT NOT NULL DEFAULT 0,

    created_at          DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);
GO

/****************************************
    Tags
****************************************/
CREATE TABLE tags (
    tag_id     INT IDENTITY(1,1) PRIMARY KEY,
    name       NVARCHAR(100) NOT NULL UNIQUE
);
GO

/****************************************
    Drug <-> Tag Many-to-Many
****************************************/
CREATE TABLE drug_tags (
    drug_id  INT NOT NULL,
    tag_id   INT NOT NULL,
    PRIMARY KEY (drug_id, tag_id),
    CONSTRAINT fk_dt_drug FOREIGN KEY (drug_id)
        REFERENCES drugs(drug_id)
        ON DELETE CASCADE,
    CONSTRAINT fk_dt_tag FOREIGN KEY (tag_id)
        REFERENCES tags(tag_id)
        ON DELETE CASCADE
);
GO

/****************************************
    Invoices
****************************************/
CREATE TABLE invoices (
    invoice_id       INT IDENTITY(1,1) PRIMARY KEY,
    admin_id         INT NULL,
    invoice_time     DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    total_amount     DECIMAL(18,2) NOT NULL DEFAULT 0,
    tax_amount       DECIMAL(18,2) NOT NULL DEFAULT 0,
    discount_amount  DECIMAL(18,2) NOT NULL DEFAULT 0,
    change_amount    DECIMAL(18,2) NOT NULL DEFAULT 0,
    CONSTRAINT fk_invoice_admin FOREIGN KEY (admin_id)
        REFERENCES admins(admin_id)
        ON DELETE SET NULL
);
GO

/****************************************
    Invoice Items (UPDATED)
****************************************/
CREATE TABLE invoice_items (
    item_id         INT IDENTITY(1,1) PRIMARY KEY,
    invoice_id      INT NOT NULL,
    drug_id         INT NOT NULL,
    quantity        INT NOT NULL CHECK (quantity > 0),

    -- You will calculate total_price as: drugs.selling_price * quantity

    CONSTRAINT fk_ii_invoice FOREIGN KEY (invoice_id)
        REFERENCES invoices(invoice_id)
        ON DELETE CASCADE,
    CONSTRAINT fk_ii_drug FOREIGN KEY (drug_id)
        REFERENCES drugs(drug_id)
        ON DELETE CASCADE
);
GO
