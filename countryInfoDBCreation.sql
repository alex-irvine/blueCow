CREATE TABLE countryInfo (ISO3166_1_Alpha_2 varchar(150), ISO3166_1_Alpha_3 varchar(150),
ITU varchar(150), MARC varchar(150), WMO varchar(150), DS varchar(150), FIFA varchar(150), FIPS varchar(150), IOC varchar(150), 
ISO4217_currency_alphabetic_code varchar(150), Capital varchar(150), Continent varchar(150))


BULK
INSERT countryInfo
FROM 'E:\Dropbox\glyndwr\optimisation\bluecow\country-codes.csv'
WITH
(
FIELDTERMINATOR = ',',
ROWTERMINATOR = '0x0a'
)