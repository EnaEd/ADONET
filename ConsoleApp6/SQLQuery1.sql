CREATE TABLE Authors(
Id int PRIMARY KEY IDENTITY(1,1),
FirstName nvarchar(30) NOT NULL,
LastName nvarchar(30) NOT NULL
)
CREATE TABLE Books(
Id int PRIMARY KEY IDENTITY(1,1),
AuthorsId int FOREIGN KEY REFERENCES Authors(Id),
Title nvarchar(100) NOT NULL,
Price money NOT NULL CHECK(Price>0),
Pages int NOT NULL CHECK(Pages>0)
)


