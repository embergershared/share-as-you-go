INSERT INTO tests.FileProcessTest ( [FileName], [ExpectedValue], [CausesException])
VALUES ('C:\Windows\explorer.exe', 1, 0);

INSERT INTO tests.FileProcessTest ( [FileName], [ExpectedValue], [CausesException])
VALUES ('C:\BadFile.txt', 0, 0);

INSERT INTO tests.FileProcessTest ( [FileName], [ExpectedValue], [CausesException])
VALUES (null, 0, 1);
