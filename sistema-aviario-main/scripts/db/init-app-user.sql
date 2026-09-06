-- ---------------------------------------------------------------------------
-- Cria o banco e um usuario de aplicacao dedicado, para que o backend nao
-- precise se conectar como 'sa'.
--
-- Idempotente: pode rodar em toda subida do compose sem efeito colateral.
-- Recebe as variaveis DbName / AppUser / AppPassword via sqlcmd -v.
-- ---------------------------------------------------------------------------

:on error exit
SET NOCOUNT ON;
GO

DECLARE @dbName  sysname       = N'$(DbName)';
DECLARE @appUser sysname       = N'$(AppUser)';
DECLARE @appPwd  nvarchar(256) = N'$(AppPassword)';
DECLARE @sql     nvarchar(max);

IF @appPwd IS NULL OR LEN(@appPwd) < 12
BEGIN
    RAISERROR('DB_APP_PASSWORD ausente ou com menos de 12 caracteres.', 16, 1);
    RETURN;
END

-- 1) Banco de dados
IF DB_ID(@dbName) IS NULL
BEGIN
    SET @sql = N'CREATE DATABASE ' + QUOTENAME(@dbName) + N';';
    EXEC sp_executesql @sql;
    PRINT 'Banco criado: ' + @dbName;
END

-- 2) Login no servidor (cria ou apenas sincroniza a senha vinda do .env)
IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = @appUser)
BEGIN
    SET @sql = N'CREATE LOGIN ' + QUOTENAME(@appUser) +
               N' WITH PASSWORD = ' + QUOTENAME(@appPwd, '''') +
               N', CHECK_POLICY = ON, DEFAULT_DATABASE = ' + QUOTENAME(@dbName) + N';';
    EXEC sp_executesql @sql;
    PRINT 'Login criado: ' + @appUser;
END
ELSE
BEGIN
    SET @sql = N'ALTER LOGIN ' + QUOTENAME(@appUser) +
               N' WITH PASSWORD = ' + QUOTENAME(@appPwd, '''') + N';';
    EXEC sp_executesql @sql;
    PRINT 'Senha do login sincronizada: ' + @appUser;
END

-- 3) Usuario dentro do banco.
--    db_owner porque o EF Core aplica as migrations (CREATE/ALTER TABLE) no
--    startup. E um escopo restrito a este banco: o login nao tem nenhuma role
--    de servidor, entao nao enxerga nem administra as outras bases.
SET @sql = N'USE ' + QUOTENAME(@dbName) + N';
IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = ' + QUOTENAME(@appUser, '''') + N')
    CREATE USER ' + QUOTENAME(@appUser) + N' FOR LOGIN ' + QUOTENAME(@appUser) + N';
IF IS_ROLEMEMBER(''db_owner'', ' + QUOTENAME(@appUser, '''') + N') = 0
    ALTER ROLE db_owner ADD MEMBER ' + QUOTENAME(@appUser) + N';';
EXEC sp_executesql @sql;

PRINT 'Usuario de aplicacao pronto.';
GO
