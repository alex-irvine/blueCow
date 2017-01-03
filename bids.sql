CREATE TABLE Bids (id nvarchar(5), bid decimal)

INSERT INTO Bids (id)
SELECT DISTINCT ida FROM Distances

SELECT * FROM Bids