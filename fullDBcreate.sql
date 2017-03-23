CREATE TABLE Distances (numa int, ida nvarchar(5), numb int, idb nvarchar(5), kmdist int, midist int)

BULK
INSERT Distances
FROM 'G:\Dropbox\glyndwr\optimisation\bluecow\capdist.csv'
WITH
(
FIELDTERMINATOR = ',',
ROWTERMINATOR = '0x0a'
)

CReATE TABLE continents (cc nvarchar(2), a_2 nvarchar(2), a_3 nvarchar(3), num int, name nvarchar(max))

BULK
INSERT continents
FROM 'G:\Dropbox\glyndwr\optimisation\bluecow\continents.csv'
WITH
(
FIELDTERMINATOR = ',',
ROWTERMINATOR = '0x0a'
)


CREATE TABLE countryInfo (ISO3166_1_Alpha_2 varchar(150), ISO3166_1_Alpha_3 varchar(150),
ITU varchar(150), MARC varchar(150), WMO varchar(150), DS varchar(150), FIFA varchar(150), FIPS varchar(150), IOC varchar(150), 
ISO4217_currency_alphabetic_code varchar(150), Capital varchar(150), Continent varchar(150))


BULK
INSERT countryInfo
FROM 'G:\Dropbox\glyndwr\optimisation\bluecow\country-codes.csv'
WITH
(
FIELDTERMINATOR = ',',
ROWTERMINATOR = '0x0a'
)

CREATE TABLE Bids (id nvarchar(5), bid decimal)

INSERT INTO Bids (id)
SELECT DISTINCT ida FROM Distances

SELECT * FROM Bids

INSERT INTO continents (a_3)
SELECT DISTINCT ida FROM Distances WHERE ida NOT IN (SELECT a_3 FROM continents)

update continents set cc = 'EU' where a_3 in ( 'DEN','FRN','IRE','SER','SPN','YUG')
update continents set cc = 'NA' where a_3 in ( 'BHM')
update continents set cc = 'SA' where a_3 in ( 'CAM','SUD','TRI')
update continents set cc = 'AF' where a_3 in ( 'LEB','LIB','NIG','NIR','SWA','TAJ','TAW','TAZ','ZAM','ZAN','ZIM')
update continents set cc = 'AS' where a_3 in ( 'KUW','KZK','LAT','MAL','UAE')