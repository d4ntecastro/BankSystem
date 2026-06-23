using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

namespace BankSystem
{
    /// <summary>
    /// Handles persistent data storage using JSON files.
    /// Provides methods to save and load account data without using a database.
    /// Data is stored in a JSON file in the application's data directory.
    /// </summary>
    public class DataPersistence
    {
        /// <summary>
        /// Default data directory name.
        /// </summary>
        private const string DataDirectory = "BankData";

        /// <summary>
        /// Default data file name.
        /// </summary>
        private const string DataFileName = "accounts.json";

        /// <summary>
        /// Full path to the data file.
        /// </summary>
        private readonly string _filePath;

        /// <summary>
        /// JsonSerializerOptions configured for readable output.
        /// </summary>
        private readonly JsonSerializerOptions _jsonOptions;

        /// <summary>
        /// Initializes a new instance of the DataPersistence class.
        /// Creates the data directory if it doesn't exist.
        /// </summary>
        public DataPersistence()
        {
            // Ensure the data directory exists
            if (!Directory.Exists(DataDirectory))
            {
                Directory.CreateDirectory(DataDirectory);
            }

            _filePath = Path.Combine(DataDirectory, DataFileName);

            // Configure JSON serialization options for readable output
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        /// <summary>
        /// Saves a list of accounts to the JSON data file.
        /// Overwrites existing file. Creates file if it doesn't exist.
        /// </summary>
        /// <param name="accounts">The list of accounts to save</param>
        /// <returns>True if save was successful; false otherwise</returns>
        public bool SaveAccounts(List<Account> accounts)
        {
            try
            {
                // Serialize accounts to JSON
                string json = JsonSerializer.Serialize(accounts, _jsonOptions);

                // Write to file
                File.WriteAllText(_filePath, json);

                Console.WriteLine($"✓ Data saved successfully to {_filePath}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Error saving data: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Loads accounts from the JSON data file.
        /// Returns an empty list if the file doesn't exist.
        /// </summary>
        /// <returns>A list of loaded accounts, or empty list if file doesn't exist</returns>
        public List<Account> LoadAccounts()
        {
            try
            {
                // Return empty list if file doesn't exist
                if (!File.Exists(_filePath))
                {
                    Console.WriteLine("No existing data file found. Starting with empty account list.");
                    return new List<Account>();
                }

                // Read and deserialize JSON from file
                string json = File.ReadAllText(_filePath);
                List<Account> accounts = JsonSerializer.Deserialize<List<Account>>(json);

                Console.WriteLine($"✓ Data loaded successfully from {_filePath}");
                return accounts ?? new List<Account>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Error loading data: {ex.Message}");
                return new List<Account>();
            }
        }

        /// <summary>
        /// Checks if the data file exists.
        /// </summary>
        /// <returns>True if the data file exists; false otherwise</returns>
        public bool DataFileExists()
        {
            return File.Exists(_filePath);
        }

        /// <summary>
        /// Deletes the data file if it exists.
        /// </summary>
        /// <returns>True if file was deleted or didn't exist; false if deletion failed</returns>
        public bool DeleteDataFile()
        {
            try
            {
                if (File.Exists(_filePath))
                {
                    File.Delete(_filePath);
                    Console.WriteLine("Data file deleted.");
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting data file: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Gets the full path to the data file.
        /// </summary>
        /// <returns>The file path</returns>
        public string GetDataFilePath()
        {
            return _filePath;
        }
    }
}
