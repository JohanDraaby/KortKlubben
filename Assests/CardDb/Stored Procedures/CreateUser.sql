DROP PROCEDURE CreateUser;
DROP PROCEDURE ModifyMail;
DROP PROCEDURE ModifyPassword;
DROP PROCEDURE DeleteUser;
DROP PROCEDURE ReadUser;
GO
--Create User
CREATE PROCEDURE CreateUser 
				@Username varchar(255), 
				@Password varchar(255), 
				@Mail varchar(255)
AS
INSERT INTO Users 
				(Username, Password, Mail) 
				VALUES 
				(@Username, @Password, @Mail);
GO
--Delete User
CREATE PROCEDURE DeleteUser
				@Username varchar(255),
				@Password varchar(255)
AS
UPDATE Users SET 
				Username = null,
				Password = null,
				Mail = null
WHERE Username = @Username AND Password = @Password;
GO
--Modify mail
CREATE PROCEDURE ModifyMail
				@Mail varchar(255),
				@Username varchar(255)
AS
UPDATE Users SET 
				Mail = @Mail
WHERE Username = @Username;
GO
--Modify password
CREATE PROCEDURE ModifyPassword
				@Username varchar(255),
				@Password varchar(255)
AS
UPDATE Users SET
				Password = @Password
WHERE Username = @Username;
GO
--Read User
CREATE PROCEDURE ReadUser
				@Username varchar(255),
				@Password varchar(255),
				@Mail varchar(255)
AS
SELECT * FROM Users WHERE Username = @Username;
GO