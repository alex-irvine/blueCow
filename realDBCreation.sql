CREATE TABLE Distances (numa int, ida nvarchar(5), numb int, idb nvarchar(5), kmdist int, midist int)

BULK
INSERT Distances
FROM 'E:\Dropbox\glyndwr\optimisation\bluecow\capdist.csv'
WITH
(
FIELDTERMINATOR = ',',
ROWTERMINATOR = '0x0a'
)

SELECT * FROM Distances