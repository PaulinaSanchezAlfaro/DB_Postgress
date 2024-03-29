﻿using Npgsql;
using System;
using System.Data;
using System.Diagnostics;

public class DatabaseManager
{
    private readonly string _connectionString;

    public DatabaseManager(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void CreateDatabase(string databaseName)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        using var command = new NpgsqlCommand($"CREATE DATABASE {databaseName}", connection);
        command.ExecuteNonQuery();
    }
    public void DropDatabase(string databaseName)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        using var command = new NpgsqlCommand($"DROP DATABASE IF EXISTS {databaseName}", connection);
        command.ExecuteNonQuery();
    }
    public void CreateUserToDB(string username, string password, string database)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        using var command = new NpgsqlCommand($"CREATE USER {username} WITH PASSWORD '{password}'", connection);
        command.ExecuteNonQuery();

        using var grantCommand = new NpgsqlCommand($"GRANT ALL PRIVILEGES ON DATABASE {database} TO {username}", connection);
        grantCommand.ExecuteNonQuery();
    }
    public void CreateUser(string username, string password)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        using var command = new NpgsqlCommand($"CREATE USER {username} WITH PASSWORD '{password}'", connection);
        command.ExecuteNonQuery();
    }
    public void DeleteUser(string username)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        using var command = new NpgsqlCommand($"DROP USER IF EXISTS {username}", connection);
        command.ExecuteNonQuery();
    }
    public List<string> GetUsers()
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        var userList = new List<string>();

        using var command = new NpgsqlCommand("SELECT usename FROM pg_user;", connection);
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            userList.Add(reader.GetString(0));
        }

        return userList;
    }
    public List<string> GetDataBase()
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        var DataBaseList = new List<string>();

        using var command = new NpgsqlCommand("SELECT datname FROM pg_database;", connection);
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            DataBaseList.Add(reader.GetString(0));
        }

        return DataBaseList;
    }
    public void CreateSchema(string schemaName)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        using var command = new NpgsqlCommand($"CREATE SCHEMA {schemaName}", connection);
        command.ExecuteNonQuery();
    }
    public void DropSchema(string schemaName)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        using (var checkSchemaCommand = new NpgsqlCommand($"SELECT schema_name FROM information_schema.schemata WHERE schema_name = '{schemaName}'", connection))
        {
            var existingSchemaName = checkSchemaCommand.ExecuteScalar();

            if (existingSchemaName == null || existingSchemaName.ToString() != schemaName)
            {
                Console.WriteLine($"El esquema '{schemaName}' no existe.");
                return;
            }
        }
        using var command = new NpgsqlCommand($"DROP SCHEMA {schemaName} CASCADE;", connection);
        command.ExecuteNonQuery();

        Console.WriteLine($"Esquema '{schemaName}' eliminado exitosamente.");
    }


    public List<string> GetSchemas()
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        var schemaList = new List<string>();

        using (var command = new NpgsqlCommand($"SELECT schema_name FROM information_schema.schemata;", connection))
        {
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                schemaList.Add(reader.GetString(0));
            }
        }

        return schemaList;
    }
    public void ActivateAuditALll(string databaseName, string type )
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        using (var checkDatabaseCommand = new NpgsqlCommand($"SELECT datname FROM pg_database WHERE datname = '{databaseName}'", connection))
        {
            var existingDatabaseName = checkDatabaseCommand.ExecuteScalar();

            if (existingDatabaseName == null || existingDatabaseName.ToString() != databaseName)
            {
                throw new Exception($"La base de datos '{databaseName}' no existe.");
            }
        }

        using (var switchDatabaseCommand = new NpgsqlCommand($"SET search_path TO {databaseName}", connection))
        {
            switchDatabaseCommand.ExecuteNonQuery();
        }

        using (var auditCommand = new NpgsqlCommand($"ALTER DATABASE {databaseName} SET audit.config = '{type}'", connection))
        {
            auditCommand.ExecuteNonQuery();
        }
    }
    public string GetAuditConfiguration(string databaseName)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();
        using (var checkDatabaseCommand = new NpgsqlCommand($"SELECT datname FROM pg_database WHERE datname = '{databaseName}'", connection))
        {
            var existingDatabaseName = checkDatabaseCommand.ExecuteScalar();

            if (existingDatabaseName == null || existingDatabaseName.ToString() != databaseName)
            {
                throw new Exception($"La base de datos '{databaseName}' no existe.");
            }
        }

        using (var switchDatabaseCommand = new NpgsqlCommand($"SET search_path TO {databaseName}", connection))
        {
            switchDatabaseCommand.ExecuteNonQuery();
        }
        using (var auditCommand = new NpgsqlCommand("SHOW audit.config", connection))
        {
            var auditConfig = auditCommand.ExecuteScalar();
            return auditConfig?.ToString();
        }
    }
    public void BackupDatabase(string databaseName, string backupFolderPath,string user)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        using (var checkDatabaseCommand = new NpgsqlCommand($"SELECT datname FROM pg_database WHERE datname = '{databaseName}'", connection))
        {
            var existingDatabaseName = checkDatabaseCommand.ExecuteScalar();

            if (existingDatabaseName == null || existingDatabaseName.ToString() != databaseName)
            {
                throw new Exception($"La base de datos '{databaseName}' no existe.");
            }
        }

        var backupFileName = $"{databaseName}_backup_{DateTime.Now:yyyyMMddHHmmss}.backup";
        var backupFilePath = System.IO.Path.Combine(backupFolderPath, backupFileName);

        var processInfo = new ProcessStartInfo
        {
            FileName = "pg_dump",
            Arguments = $"-h localhost -U {user} -d {databaseName} -F c -b -v -f \"{backupFilePath}\"",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (var process = new Process { StartInfo = processInfo })
        {
            process.Start();
            process.WaitForExit();
        }
    }
    public void RestoreBackup( string backupFilePath, string newDatabaseName,string user)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        // Utiliza el comando pg_restore para restaurar el respaldo
        var processInfo = new ProcessStartInfo
        {
            FileName = "pg_restore",
            Arguments = $"-h localhost -U {user} -d {newDatabaseName} -v \"{backupFilePath}\"",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (var process = new Process { StartInfo = processInfo })
        {
            process.Start();
            process.WaitForExit();
        }
    }


}
