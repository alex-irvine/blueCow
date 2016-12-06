BULK
INSERT Distances
FROM 'c:\Users\s14003221\Desktop\bluecow\capdist.csv'
WITH
(
FIELDTERMINATOR = ',',
ROWTERMINATOR = '0x0a'
)

SELECT * FROM Distances