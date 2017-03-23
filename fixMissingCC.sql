INSERT INTO continents (a_3)
SELECT DISTINCT ida FROM Distances WHERE ida NOT IN (SELECT a_3 FROM continents)

update continents set cc = 'EU' where a_3 in ( 'DEN','FRN','IRE','SER','SPN','YUG')
update continents set cc = 'NA' where a_3 in ( 'BHM')
update continents set cc = 'SA' where a_3 in ( 'CAM','SUD','TRI')
update continents set cc = 'AF' where a_3 in ( 'LEB','LIB','NIG','NIR','SWA','TAJ','TAW','TAZ','ZAM','ZAN','ZIM')
update continents set cc = 'AS' where a_3 in ( 'KUW','KZK','LAT','MAL','UAE')