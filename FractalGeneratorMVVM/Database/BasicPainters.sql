CREATE TABLE [dbo].[BasicPainters]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [InSetColour] BINARY(24) NOT NULL, 
    [MainColour] BINARY(24) NOT NULL, 
    [Type] BIT NOT NULL, 
    [Name] VARCHAR(50) NOT NULL
)
