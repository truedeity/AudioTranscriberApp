using System;
using System.Data.SqlClient;

public class DatabaseHelper
{
    private string _connectionString;

    public DatabaseHelper()
    {
        _connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Integrated Security=True;";
    }

    public void InitializeDatabase()
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            string createDatabaseQuery = "IF DB_ID('TranscriptionDB') IS NULL CREATE DATABASE TranscriptionDB";
            SqlCommand createDbCommand = new SqlCommand(createDatabaseQuery, connection);
            createDbCommand.ExecuteNonQuery();
        }

        _connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TranscriptionDB;Integrated Security=True;";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string createTableQuery = @"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Transcriptions')
                CREATE TABLE Transcriptions (
                    Id INT PRIMARY KEY IDENTITY(1,1),
                    TranscribedText NVARCHAR(MAX),
                    TranscriptionDate DATETIME
                )";
            SqlCommand createTableCommand = new SqlCommand(createTableQuery, connection);
            createTableCommand.ExecuteNonQuery();
        }
    }

    public void SaveTranscription(string text)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string insertQuery = "INSERT INTO Transcriptions (TranscribedText, TranscriptionDate) VALUES (@text, @date)";
            SqlCommand insertCommand = new SqlCommand(insertQuery, connection);
            insertCommand.Parameters.AddWithValue("@text", text);
            insertCommand.Parameters.AddWithValue("@date", DateTime.Now);
            insertCommand.ExecuteNonQuery();
        }
    }
}
