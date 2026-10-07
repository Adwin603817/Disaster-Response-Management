using System;
using System.IO;
using System.Collections.Generic;

namespace Disaster_Response_Coordination.Models
{
    public class SystemLogger
    {
        private List<string> _logs;
        private readonly string _logFilePath;
        private readonly string _logsDirectory = "Logs";

        public SystemLogger()
        {
            _logs = new List<string>();

            // Create Logs directory if it doesn't exist
            if (!Directory.Exists(_logsDirectory))
            {
                Directory.CreateDirectory(_logsDirectory);
            }

            // Create log file with timestamp
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            _logFilePath = Path.Combine(_logsDirectory, $"SystemLog_{timestamp}.txt");
        }

        public void Log(string message)
        {
            string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
            _logs.Add(logEntry);
            Console.WriteLine(logEntry);
            WriteToFile(logEntry);
        }

        private void WriteToFile(string message)
        {
            try
            {
                File.AppendAllText(_logFilePath, message + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to write to log file: {ex.Message}");
            }
        }

        public void DisplayLogs()
        {
            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("SYSTEM LOGS");
            Console.WriteLine(new string('=', 60));
            foreach (var log in _logs)
            {
                Console.WriteLine(log);
            }
        }

        public void SaveLogsToFile()
        {
            try
            {
                Console.WriteLine($"[SUCCESS] Logs saved to: {_logFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to save logs: {ex.Message}");
            }
        }
    }
}