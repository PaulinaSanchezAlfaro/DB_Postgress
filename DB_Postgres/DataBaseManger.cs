using Npgsql;
using System;
using System.Data;

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

}
