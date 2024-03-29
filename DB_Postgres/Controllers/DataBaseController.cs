﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

[ApiController]
[Route("api/")]
public class DatabaseController : ControllerBase
{
    private readonly DatabaseManager _databaseManager;

    public DatabaseController(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        _databaseManager = new DatabaseManager(connectionString);
    }

    [HttpPost("create-database")]
    public IActionResult CreateDatabase(string databaseName)
    {
        _databaseManager.CreateDatabase(databaseName);
        return Ok($"Database '{databaseName}' created successfully.");
    }

    [HttpDelete("drop-database")]
    public IActionResult DropDatabase(string databaseName)
    {
        _databaseManager.DropDatabase(databaseName);
        return Ok($"Database '{databaseName}' dropped successfully.");
    }

    [HttpPost("create-user")]
    public IActionResult CreateUser(string username, string password)
    {
        _databaseManager.CreateUser(username, password);
        return Ok($"User '{username}' created successfully.");
    }

    [HttpDelete("delete-user")]
    public IActionResult DeleteUser(string username)
    {
        _databaseManager.DeleteUser(username);
        return Ok($"User '{username}' deleted successfully.");
    }

    [HttpPost("create-user-DB")]
    public IActionResult CreateUser(string username, string password, string database)
    {
        _databaseManager.CreateUserToDB(username, password, database);
        return Ok($"User '{username}' created successfully in database '{database}'.");
    }

    [HttpGet("get-users")]
    public IActionResult GetUsers()
    {
        var userList = _databaseManager.GetUsers();
        return Ok(userList);
    }

    [HttpGet("get-DataBase")]
    public IActionResult GetDataBase()
    {
        var dataBaseList = _databaseManager.GetDataBase();
        return Ok(dataBaseList);
    }

    [HttpPost("create-schema")]
    public IActionResult CreateSchema(string schemaName)
    {
        try
        {
            _databaseManager.CreateSchema(schemaName);
            return Ok($"Esquema '{schemaName}' creado exitosamente'.");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al crear el esquema: {ex.Message}");
        }
    }

    [HttpGet("get-schemas")]
    public IActionResult GetSchemas()
    {
        try
        {
            var schemaList = _databaseManager.GetSchemas();
            return Ok(schemaList);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al obtener los esquemas: {ex.Message}");
        }
    }

    [HttpDelete("delete-schema")]
    public IActionResult DeleteSchema(string schema)
    {
        _databaseManager.DropSchema(schema);
        return Ok($"Schema '{schema}' deleted successfully.");
    }

    [HttpPost("activate-audit")]
    public IActionResult ActivateAudit(string databaseName, string type)
    {
        try
        {
            _databaseManager.ActivateAuditALll(databaseName,type);
            return Ok($"Auditoría activada exitosamente en la base de datos '{databaseName}'.");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al activar la auditoría: {ex.Message}");
        }
    }
    //revisar funciona la primera vez y luego no 
    [HttpGet("audit-configuration")]
    public IActionResult GetAuditConfiguration(string databaseName)
    {
        try
        {
            var auditConfig = _databaseManager.GetAuditConfiguration(databaseName);
            return Ok($"Configuración de auditoría para la base de datos '{databaseName}': {auditConfig}");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al obtener la configuración de auditoría: {ex.Message}");
        }
    }

    [HttpPost("backup")]
    public IActionResult BackupDatabase(string databaseName, string backupFolderPath, string user)
    {
        try
        {
            _databaseManager.BackupDatabase(databaseName, backupFolderPath, user);
            return Ok($"Respaldo de la base de datos '{databaseName}' creado en '{backupFolderPath}'.");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al crear el respaldo: {ex.Message}");
        }
    }
    [HttpPost("restore")]
    public IActionResult RestoreBackup(string backupFilePath, string newDatabaseName, string user)
    {
        try
        {
            _databaseManager.RestoreBackup(backupFilePath, newDatabaseName,user);
            return Ok($"Respaldo restaurado en la nueva base de datos '{newDatabaseName}'.");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al restaurar el respaldo: {ex.Message}");
        }
    }
}
