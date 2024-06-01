using AzDoTestPlanSDK.Model;
using AzDoTestPlanSDK.Model.JUnit;
using PublishAzDoTestResults.model;
using PublishAzDoTestResults.utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Xml.Serialization;

namespace PublishAzDoTestResults.core
{
    internal class Logic
    {
        private List<int> TestPoints = new List<int>();
        private Dictionary<string, string> NotFoundInTestPlan;

        ObjectBuilder ObjectBuilder = new ObjectBuilder();

        TestResultsCollector collector = new TestResultsCollector();

        // Assuming some test results are recorded here
        public void ReadEachTestCasesArray(JUnitTest JUnitResults, GetAzDoTestRun TestPlan, string TestConfiguration)
        {
            int localTestPoint = 0;
            if (JUnitResults is JTestSuites)
            {
                JUnitResults.TestSuite.ForEach(Testsuite =>
                {
                    Testsuite.TestCases.ForEach(testCase =>
                    {
                        //Console.WriteLine($"Test Case name from xml: {testCase.Name}");

                        var PointsAssignments = TestPlan.value
                            .Where(nested => string.Equals(nested.workItem.name, testCase.Name, StringComparison.OrdinalIgnoreCase))
                            .Select(nested => nested.pointAssignments);

                        string ReasonForTestCaseNotFound = "";

                        if (PointsAssignments.Count() == 0)
                        {
                            ReasonForTestCaseNotFound = "not found in the testplan";
                            collector.NotProcessed();
                            CollectNotFoundInTestPlan(testCase.Name, ReasonForTestCaseNotFound);
                        }

                        else
                        {
                            foreach (var points in PointsAssignments)
                            {
                                foreach (var pointslist in points)
                                {
                                    if (pointslist.configurationName == TestConfiguration)
                                    {
                                        //Console.WriteLine($"Testpoint of TestCase {testCase.Name} is {pointslist.id}");
                                        TestPoints.Add(pointslist.id);
                                    }
                                }
                            }
                        }

                        localTestPoint = 0;
                    }); //End of forEach
                });

            }
            if (JUnitResults is JTestSuite)
            {
                JUnitResults.TestCases.ForEach(testCase =>
                {
                    //Console.WriteLine($"Test Case name from xml: {testCase.Name}");

                    var PointsAssignments = TestPlan.value
                        .Where(nested => string.Equals(nested.workItem.name, testCase.Name, StringComparison.OrdinalIgnoreCase))
                        .Select(nested => nested.pointAssignments);

                    string ReasonForTestCaseNotFound = "";

                    if (PointsAssignments.Count() == 0)
                    {
                        ReasonForTestCaseNotFound = "not found in the testplan";
                        collector.NotProcessed();
                        CollectNotFoundInTestPlan(testCase.Name, ReasonForTestCaseNotFound);
                    }

                    else
                    {
                        foreach (var points in PointsAssignments)
                        {
                            foreach (var pointslist in points)
                            {
                                if (pointslist.configurationName == TestConfiguration)
                                {
                                    //Console.WriteLine($"Testpoint of TestCase {testCase.Name} is {pointslist.id}");
                                    TestPoints.Add(pointslist.id);
                                }
                            }
                        }
                    }

                    localTestPoint = 0;
                }); //End of forEach
            }
        }

        private void CollectNotFoundInTestPlan(string TestCaseNotFoundInTestPlan, string reason)
        {
            collector.NotProcessedTestCases(TestCaseNotFoundInTestPlan, reason);
        }

        private ObjectBuilder CollectResults(Value testCase, JUnitTest JUnitResults, ObjectBuilder obj)
        {
            collector.RecordResult(CheckFailure(JUnitResults, testCase.testCase.name) == "Passed" ? true : false);

            string CreationDate = FileHandler.Instance.GetCreationTime();
            obj.TestRunValueID = testCase.id;
            obj.State = "Completed";
            obj.Comment = "Automated update";
            obj.Outcome = CheckFailure(JUnitResults, testCase.testCase.name);
            obj.Testpoint = testCase.testPoint.id;
            obj.Errormessage = GetErrorMessage(JUnitResults, testCase.testCase.name);
            obj.StackTrace = GetStackTraceMessage(JUnitResults, testCase.testCase.name);
            obj.CreatedDate = CreationDate;
            obj.TestCaseTitle = testCase.testCase.name;
            return obj;
        }


        private string CheckFailure(JUnitTest testCases, string testCaseName)
        {
            Failure failureObject = GetFailureObject(testCases, testCaseName);

            string outcome = "";
            if (failureObject?.Message == null && failureObject?.StackTrace == null)
            {
                outcome = "Passed";
            }
            else
            {
                outcome = "Failed";
            }
            return outcome;
        }

        private string GetErrorMessage(JUnitTest testCases, string testCaseName)
        {
            Failure failureObject = GetFailureObject(testCases, testCaseName);
            string value = failureObject?.Message;
            return value;
        }

        private string GetStackTraceMessage(JUnitTest testCases, string testCaseName)
        {
            Failure failureObject = GetFailureObject(testCases, testCaseName);
            string value = failureObject?.StackTrace;
            return value;

        }

        private Failure GetFailureObject(JUnitTest testCases, string testCaseName)
        {
            Failure failureObject = new Failure();
            failureObject = testCases.TestCases
                .Where(nested => string.Equals(nested.Name, testCaseName, StringComparison.OrdinalIgnoreCase))
                .Select(nested => nested.Failure)
                .FirstOrDefault();

            return failureObject;
        }

        public string GetTestCaseID(Value testCase)
        {
            Console.WriteLine($"Processing TestID: {testCase.testCase.id}");
            return testCase.testCase.id;
        }

        public string GetTestPointID(Value testCase)
        {
            Console.WriteLine($"Processing TestPoint: {testCase.testPoint.id}");
            return testCase.testPoint.id;
        }

        internal List<int> GetTestPoints()
        {
            return TestPoints;
        }

        internal string[] PrepareResultsToPatch(GetAzDoTestRun getResult, JUnitTest JUnitResults)
        {
            List<string> PatchBodyRequestList = new List<string>();
            var ObjectBuilder = new ObjectBuilder();
            getResult.value.ForEach(GetTestRunValue =>
            {
                ObjectBuilder = CollectResults(GetTestRunValue, JUnitResults, ObjectBuilder);
                PatchBodyRequestList.Add(ObjectBuilder.BuildPatchTestRun());
            });

            return PatchBodyRequestList.ToArray();
        }

        internal TestResultsCollector GetTestResultsCollector()
        {
            return collector;
        }

    }
}