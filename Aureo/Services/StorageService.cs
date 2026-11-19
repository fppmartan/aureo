using System.IO;
using System.Collections.Generic;
using System.Text.Json;
using Aureo.Models;


namespace Aureo.Services
{
    /// <summary>
    /// Provides storage operations for application data and JSON backup files.
    /// </summary>
    public class StorageService
    {
        private readonly string _filePath; // Path to the JSON file for storing application data.


        // Options for JSON serialization, including indentation for readability.
        private readonly JsonSerializerOptions _jsonOptions = new() 
        { 
            WriteIndented = true
        };

        public StorageService()
        {
            // Determine the path to the local application data folder and create a subfolder for Aureo if it doesn't exist.
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string folder = Path.Combine(localAppData, "Aureo");
            Directory.CreateDirectory(folder);
            _filePath = Path.Combine(folder, "aureo_data.json");
        }

        public void Save(AppData data) 
        {
            string json = JsonSerializer.Serialize(data, _jsonOptions); // Serialize the AppData object to a JSON string using the specified options.
            File.WriteAllText(_filePath, json); // Write the JSON string to the specified file path.
        }

        public AppData Load() 
        {
            if (!File.Exists(_filePath)) 
            {
                return new AppData(); // If the file doesn't exist, return a new instance of AppData.
            }

            string json = File.ReadAllText(_filePath); // Read the JSON string from the specified file path.
            return JsonSerializer.Deserialize<AppData>(json) ?? new AppData(); // Deserialize the JSON string back into an AppData object, or return a new instance if deserialization fails.
        }

        public void ExportBackup(string destinationPath, AppData data) 
        {
            string json = JsonSerializer.Serialize(data, _jsonOptions);
            File.WriteAllText(destinationPath, json); // Write the JSON string to the specified destination path for backup purposes.)
        }

        public AppData ImportBackup(string sourcePath) 
        { 
            if (!File.Exists(sourcePath)) 
            {
                return new AppData();
            }

            string json = File.ReadAllText(sourcePath);
            return JsonSerializer.Deserialize<AppData>(json) ?? new AppData();
        }

    }

}
