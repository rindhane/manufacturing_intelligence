-- sql file to create the database
-- command: Sqlcmd –S . –i .\create_pdf_table.sql
DECLARE @qdas_db VARCHAR(100);
declare @query nvarchar(max); 

SET @qdas_db='QDAS_VALUE_DATABASE'; --replace with the data database of qdas

--help https://stackoverflow.com/questions/2838490/a-table-name-as-a-variable
set @query= 'CREATE TABLE'+ QUOTENAME(@qdas_db)+'.dbo.LABREPORT(
  id              INT           NOT NULL    IDENTITY    PRIMARY KEY,
  serialNum       VARCHAR(100)  NOT NULL,
  stationName     VARCHAR(50),
  pdfData  VARCHAR(max),
)';
EXEC sp_executesql @query;
GO