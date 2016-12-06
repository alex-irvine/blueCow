CReATE TABLE continents (cc nvarchar(2), a_2 nvarchar(2), a_3 nvarchar(3), num int, name nvarchar(max))

BULK
INSERT continents
FROM 'c:\Users\s14003221\Desktop\continents.csv'
WITH
(
FIELDTERMINATOR = ',',
ROWTERMINATOR = '0x0a'
)
