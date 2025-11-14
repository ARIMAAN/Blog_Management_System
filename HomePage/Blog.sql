create Database Blog_Management

use Blog_Management

CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(100) UNIQUE NOT NULL,
    Email NVARCHAR(255) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    IsActive BIT DEFAULT 1,  -- 1 means active, 0 means inactive
    CreatedAt DATETIME DEFAULT GETDATE()
);
select * from Users

delete from Users