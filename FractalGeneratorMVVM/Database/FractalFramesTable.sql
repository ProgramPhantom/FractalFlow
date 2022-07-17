CREATE TABLE [dbo].[FractalFrames]
(
	[Id] INT NOT NULL PRIMARY KEY,
	[Name] NVARCHAR(40) NULL,
    [Left] FLOAT NOT NULL, 
    [Right] FLOAT NOT NULL, 
    [Bottom] FLOAT NOT NULL, 
    [Top] FLOAT NOT NULL, 
    [Iterations] INT NOT NULL, 
    [Bail] INT NOT NULL
)
