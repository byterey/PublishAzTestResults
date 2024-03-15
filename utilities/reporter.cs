using System;
using System.Collections.Generic;
using System.Text;

namespace PublishAzDoTestResults.utilities
{
    public class TestResultsCollector
    {
        private int passedCount;
        private int failedCount;
        private string _URL;

        private List<Dictionary<string, string>> TestCaseNotFound = new List<Dictionary<string, string>>();

        public TestResultsCollector()
        {
            passedCount = 0;
            failedCount = 0;
        }

        public void RecordResult(bool passed)
        {
            if (passed)
            {
                passedCount++;
            }
            else
            {
                failedCount++;
            }
        }

        public void PrintResults()
        {
            Console.WriteLine("Test Results:");
            Console.WriteLine($"    Passed: {passedCount}");
            Console.WriteLine($"    Failed: {failedCount}");
        }

        public void NotProcessedTestCases(string TestCaseName, String Reason)
        {
            Dictionary<string, string> map1 = new Dictionary<string, string>();

            map1.Add("TestCase", "Value1");
            map1.Add("Reason", "Value2");
            TestCaseNotFound.Add(map1);
        }

        public void PrintNotFoundTestCase()
        {
            if (TestCaseNotFound.Count == 0)
            {
                Console.WriteLine("All TestCases are processed");
            }
            else
            {
                Console.WriteLine("List of testcases that was not processed");
                // Print all entries
                foreach (var map in TestCaseNotFound)
                {
                    foreach (var entry in map)
                    {
                        Console.WriteLine($"TestCase Name: {entry.Key}, Reason: {entry.Value}");
                    }
                }
            }
        }

    }
}
