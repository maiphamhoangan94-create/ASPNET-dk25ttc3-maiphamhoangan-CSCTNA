-- Chạy bằng SSMS hoặc sqlcmd trên SQL Server 2025 (17.x) trở lên.
-- Sửa đường dẫn tuyệt đối bên dưới theo máy đích. Tài khoản dịch vụ SQL
-- Server phải có quyền đọc .bak và ghi thư mục dữ liệu đã tồn tại.
USE master;
GO
RESTORE HEADERONLY FROM DISK = N'C:\CSCTNA\setup\du-lieu\anmph_db.bak';
RESTORE VERIFYONLY FROM DISK = N'C:\CSCTNA\setup\du-lieu\anmph_db.bak';
RESTORE FILELISTONLY FROM DISK = N'C:\CSCTNA\setup\du-lieu\anmph_db.bak';
GO
IF DB_ID(N'anmph_db') IS NOT NULL
    THROW 50001, N'Database anmph_db đã tồn tại. Dừng để tránh ghi đè dữ liệu.', 1;
ELSE
    RESTORE DATABASE [anmph_db]
    FROM DISK = N'C:\CSCTNA\setup\du-lieu\anmph_db.bak'
    WITH MOVE N'anmph_db' TO N'C:\CSCTNA\sql-data\anmph_db.mdf',
         MOVE N'anmph_db_log' TO N'C:\CSCTNA\sql-data\anmph_db_log.ldf',
         RECOVERY, STATS = 10;
GO
