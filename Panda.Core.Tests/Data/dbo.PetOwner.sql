CREATE TABLE [dbo].[PetOwner]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	Name varchar(100) not null,
	AddressLine1 varchar(200) not null,
	AddressLine2 varchar(200) null,
	City varchar(100) not null,
	StateCode varchar(5) not null,
	PostalCode varchar(10) not null,
	CreateDate datetime not null default(GetDate()),
	LastUpdateDate datetime not null default(GetDate())
)
