DROP TABLE Sessions;
DROP TABLE Users;
DROP database CardDb;
CREATE database CardDb;

USE CardDB;

CREATE TABLE Users (
    ID int IDENTITY(1,1) NOT NULL,
    Username varchar(255),
    Password varchar(255),
    Mail varchar(255),
    Wins varchar(255),
	CreationDate Datetime,
	DeletionDate Datetime,
	PRIMARY KEY(ID)
);

CREATE TABLE Sessions (
    ID int,
	Login Datetime,
	Logout Datetime,
	PRIMARY KEY(ID, Login),
	FOREIGN KEY (ID) REFERENCES Users(ID)
);