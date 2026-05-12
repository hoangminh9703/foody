-- Insert admin user if not exists
-- password = Admin123! (bcrypt hash)
IF NOT EXISTS (
    SELECT 1 FROM Users WHERE Email = 'admin@medicare.local'
)
BEGIN
    INSERT INTO Users (
        Email,
        FullName,
        PasswordHash
    )
    VALUES (
        'admin@medicare.local',
        'System Administrator',
        '$2a$10$wH8KQ9Q7ZQv9zZk1Yw1j9eZp6YH1l7Y6KjQ0u1m8pG3V7z8xYk2bK'
    );
END