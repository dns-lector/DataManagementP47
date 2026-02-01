-- https://www.uuidgenerator.net/version4
-- https://www.random.org/calendar-dates/
-- https://gelezo.ks.ua/price-list.html
-- https://generator-online.com/uk/names/


SET NOCOUNT ON;
DECLARE @StartDate DATETIME2;
SET @StartDate = CAST( '2020-01-01' AS DATETIME2 );
DECLARE @Seconds INT;
SET @Seconds = DATEDIFF(SECOND, @StartDate, CURRENT_TIMESTAMP);
DECLARE @RandomProductId UNIQUEIDENTIFIER;
DECLARE @RandomProductPrice MONEY;
DECLARE @RandomDate DATETIME2;
DECLARE @RandomQuantity INT;
DECLARE @i INT;
SET  @i = 1000000;
WHILE @i > 0
BEGIN
	SET  @i = @i - 1;
	SET @RandomProductId = (SELECT TOP 1 Id FROM Products  ORDER BY NEWID());
	SET @RandomDate = DATEADD(SECOND, ABS( CHECKSUM( NEWID() ) ) % @Seconds, @StartDate);
	SET @RandomProductPrice = (SELECT Price FROM Products WHERE Id = @RandomProductId);
	SET @RandomQuantity = CASE
		WHEN @RandomProductPrice > 2000 THEN 1
		WHEN @RandomProductPrice > 500 THEN ABS( CHECKSUM( NEWID() ) ) % 2 + 1
		ELSE ABS( CHECKSUM( NEWID() ) ) % 4 + 1
	END

	INSERT INTO Sales (Id, ProductId, EmployeeId, Quantity, Moment)
	VALUES (
		NEWID(),
		@RandomProductId,
		(SELECT TOP 1 Id FROM Employees ORDER BY NEWID()),
		@RandomQuantity,
		@RandomDate
	);
END
GO