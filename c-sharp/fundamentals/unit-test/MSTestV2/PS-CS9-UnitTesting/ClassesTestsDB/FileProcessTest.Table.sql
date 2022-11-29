CREATE TABLE [tests].[FileProcessTest]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [FileName] VARCHAR(255) NULL, 
    [ExpectedValue] BIT NOT NULL, 
    [CausesException] BIT NOT NULL
)
