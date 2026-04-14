-- Agregar columna Activa si no existe
IF NOT EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID('dbo.Encuestas') 
    AND name = 'Activa'
)
BEGIN
    ALTER TABLE dbo.Encuestas ADD Activa BIT NOT NULL DEFAULT 1;
    PRINT 'Columna Activa agregada correctamente';
END
ELSE
BEGIN
    PRINT 'La columna Activa ya existe';
END
