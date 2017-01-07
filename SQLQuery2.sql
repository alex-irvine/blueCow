select a_3 from continents 
group by a_3
having COUNT(a_3) > 1

select * from continents where a_3 = 'CYP'

SELECT DISTINCT ida FROM Distances WHERE ida NOT IN (SELECT a_3 FROM continents) ORDER BY ida SELECT DISTINCT * FROM continents WHERE a_3 NOT IN (SELECT DISTINCT ida FROM Distances) ORDER BY a_3

SELECT DISTINCT ida FROM Distances ORDER BY ida SELECT DISTINCT a_3 FROM continents ORDER BY a_3

SELECT * FROM continents ORDER BY a_3

SELECT * FROM countryInfo ORDER BY FIFA SELECT DISTINCT ida FROM Distances ORDER BY ida

SELECT * FROM countryInfo WHERE IOC = 'AAB'

SELECT * FROM Distances

INSERT INTO continents (a_3)
SELECT DISTINCT ida FROM Distances WHERE ida NOT IN (SELECT a_3 FROM continents)

select * from continents where cc IS NULL

select * from distances where ida = 'AAB'

update continents set cc = 'EU' where a_3 in ( 'DEN','FRN','IRE','SER','SPN','YUG')
update continents set cc = 'NA' where a_3 in ( 'BHM')
update continents set cc = 'SA' where a_3 in ( 'CAM','SUD','TRI')
update continents set cc = 'AF' where a_3 in ( 'LEB','LIB','NIG','NIR','SWA','TAJ','TAW','TAZ','ZAM','ZAN','ZIM')
update continents set cc = 'AS' where a_3 in ( 'KUW','KZK','LAT','MAL','UAE')

select * from continents where cc = 'EU'