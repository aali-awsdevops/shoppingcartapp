-- SQL script to create tables for RegisterCustomerService
-- Adjust schema and types as needed.

CREATE TABLE IF NOT EXISTS customers (
    id SERIAL PRIMARY KEY,
    first_name TEXT NOT NULL,
    last_name TEXT NOT NULL,
    email TEXT NOT NULL,
    phone_number TEXT,
    address TEXT,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

-- You can run this script against your PostgreSQL database:
-- psql -h <host> -U <user> -d <database> -f create_tables.sql
