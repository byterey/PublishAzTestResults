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

    }
}
