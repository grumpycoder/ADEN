

CREATE PROCEDURE [Aden].[p_createWorkItemReport]
	 @DataYear int
	,@ReportLevel varchar(3) 
AS
	
	 --DECLARE @DataYear int = 2018
	 --DECLARE @ReportLevel varchar(3) = 'SEA'

	--SELECT '' [MyField1], '' [MyField2], ''
	--UNION
	SELECT
		--@DataYear, @ReportLevel,		
		@DataYear, @ReportLevel, *		
	FROM Aden.FileSpecifications

	--return 1

