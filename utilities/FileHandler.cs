using AzDoTestPlanSDK.Model.JUnit;
using System;
using System.IO;
using System.Xml;
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

        public JUnitTest ReadXML()
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new InvalidOperationException("File path has not been initialized. Call Initialize() first.");
            }
            

            string xml = File.ReadAllText(filePath);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            XmlSerializer serializer;

            if (doc.DocumentElement.Name == "testsuite")
            {
                serializer = new XmlSerializer(typeof(JTestSuite));
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
                {
                    return (JTestSuite)serializer.Deserialize(fileStream);
                }

            }
            else if (doc.DocumentElement.Name == "testsuites")
            {

                serializer = new XmlSerializer(typeof(JTestSuites));
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
                {
                    return (JTestSuites)serializer.Deserialize(fileStream);
                }

            }

            else
            {
                return null;
            }
        }
    }
}
