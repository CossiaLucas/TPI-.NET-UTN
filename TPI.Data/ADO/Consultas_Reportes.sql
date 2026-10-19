-- =====================================================================
--  Consultas de reportes (las mismas que ejecuta TPI.Data/ADO/ReporteRepository.cs)
--  Sirven para probarlas a mano contra la base TPIG6Db (Visual Studio > Explorador de objetos
--  de SQL Server > clic derecho en la base > Nueva consulta) antes de usar la aplicación.
--  Cambiá los DECLARE de cada bloque para probar otros filtros.
-- =====================================================================
USE TPIG6Db;
GO

-- ---------------------------------------------------------------------
-- 1) VENTAS POR MES
--    @Estado = NULL  -> cuentan Pagado y NoPagado (se excluyen las Canceladas)
--    @Estado = 'Pagado' | 'NoPagado' | 'Cancelado' -> solo ese estado
-- ---------------------------------------------------------------------
DECLARE @Desde          DATETIME      = DATEADD(MONTH, -11, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1));
DECLARE @HastaExclusivo DATETIME      = DATEADD(DAY, 1, CAST(CAST(GETDATE() AS DATE) AS DATETIME));
DECLARE @Estado         NVARCHAR(50)  = NULL;
DECLARE @Cancelado      NVARCHAR(50)  = N'Cancelado';

SELECT YEAR(v.Fecha)  AS Anio,
       MONTH(v.Fecha) AS Mes,
       COUNT(*)       AS CantidadVentas,
       SUM(v.Total)   AS Total
FROM venta v
WHERE v.Fecha >= @Desde
  AND v.Fecha <  @HastaExclusivo
  AND ((@Estado IS NULL AND v.Estado <> @Cancelado) OR v.Estado = @Estado)
GROUP BY YEAR(v.Fecha), MONTH(v.Fecha)
ORDER BY Anio, Mes;
GO

-- ---------------------------------------------------------------------
-- 2) PRODUCTOS MÁS VENDIDOS  (@IdCategoria = NULL -> todas las categorías)
-- ---------------------------------------------------------------------
DECLARE @Top            INT           = 10;
DECLARE @Desde          DATETIME      = DATEADD(MONTH, -11, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1));
DECLARE @HastaExclusivo DATETIME      = DATEADD(DAY, 1, CAST(CAST(GETDATE() AS DATE) AS DATETIME));
DECLARE @Estado         NVARCHAR(50)  = NULL;
DECLARE @Cancelado      NVARCHAR(50)  = N'Cancelado';
DECLARE @IdCategoria    INT           = NULL;

SELECT TOP (@Top)
       p.Id            AS IdProducto,
       p.Nombre        AS Nombre,
       c.Nombre        AS Categoria,
       SUM(d.Cantidad) AS Unidades,
       SUM(d.Subtotal) AS Monto
FROM detalleVenta d
INNER JOIN venta     v ON v.NroVenta = d.IdVenta
INNER JOIN producto  p ON p.Id       = d.IdProducto
INNER JOIN categoria c ON c.Id       = p.IdCategoria
WHERE v.Fecha >= @Desde
  AND v.Fecha <  @HastaExclusivo
  AND ((@Estado IS NULL AND v.Estado <> @Cancelado) OR v.Estado = @Estado)
  AND (@IdCategoria IS NULL OR p.IdCategoria = @IdCategoria)
GROUP BY p.Id, p.Nombre, c.Nombre
ORDER BY Unidades DESC, Monto DESC, p.Nombre;
GO

-- ---------------------------------------------------------------------
-- 3) PRODUCTOS CON STOCK BAJO
-- ---------------------------------------------------------------------
DECLARE @Umbral      INT = 5;
DECLARE @IdCategoria INT = NULL;

SELECT p.Id     AS IdProducto,
       p.Nombre AS Nombre,
       c.Nombre AS Categoria,
       p.Stock  AS Stock
FROM producto p
INNER JOIN categoria c ON c.Id = p.IdCategoria
WHERE p.Stock <= @Umbral
  AND (@IdCategoria IS NULL OR p.IdCategoria = @IdCategoria)
ORDER BY p.Stock ASC, p.Nombre;
GO
