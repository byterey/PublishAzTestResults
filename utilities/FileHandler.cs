using AzDoTestPlanSDK.Model.JUnit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace PublishAzDoTestResults.utilities
{
    internal class FileHandler
    {
        private static readonly Lazy<FileHandler> instance = new Lazy<FileHandler>(() => new FileHandler());
        private string filePath;

        // Private constructor
        private FileHandler()
        {
        }

        // Singleton Instance Property
        public static FileHandler Instance => instance.Value;

        // Initialize method to set the file path
        public void Initialize(string filePath)
        {
            this.filePath = filePath;
        }

        public string GetCreationTime()
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new InvalidOperationException("File path has not been initialized. Call Initialize() first.");
            }

            FileInfo fileInfo = new FileInfo(filePath);
            string time = fileInfo.CreationTime.ToString("yyyy-MM-ddTHH:mm:ss.fZ");
            return fileInfo.CreationTime.ToString("yyyy-MM-ddTHH:mm:ss.fZ");
        }

        public JTestSuite ReadXML()
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new InvalidOperationException("File path has not been initialized. Call Initialize() first.");
            }

            XmlSerializer serializer = new XmlSerializer(typeof(JTestSuite));
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
            {
                return (JTestSuite)serializer.Deserialize(fileStream);
            }
        }
    }
}
