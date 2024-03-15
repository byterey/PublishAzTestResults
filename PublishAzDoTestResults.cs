using System;
using System.Management.Automation;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Generic;
using AzDoTestPlanSDK.Model;
using AzDoTestPlanSDK.Model.JUnit;
using PublishAzDoTestResults.core;
using PublishAzDoTestResults.utilities;
using PublishAzDoTestResults.model;

namespace PublishAzDoTestResults
{
    [Cmdlet(VerbsData.Import, "AzDoTestRunJUnit")]
    public class GetXmlCommand : PSCmdlet
    {

        [Parameter(Position = 0, ValueFromPipeline = true, HelpMessage = "The azure devops project url. example: https://dev.azure.com/<organization>/<project>")]
        public string Uri { get; set; }

        [Parameter(Position = 1, Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The personal access token (PAT) from devops project")]
        public string Token { get; set; }

        [Parameter(Position = 2, Mandatory = true, HelpMessage = "The file path of the JUnit xml test result")]
        [ValidateNotNullOrEmpty]
        public string FilePath { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true, HelpMessage = "The query request, an object output from the command Get-AzDoTestPlan")]
        public GetAzDoTestRun TestPlanObject { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true, HelpMessage = "The test configuration of the testcase in the azure test plan, to where the results will be published")]
        public string TestConfiguration { get; set; }

        public string CreationTime { get; set; }

        // Create an instance of Random class

        Random random = new Random();
        Logic LogicProcess = new Logic();


        private static async Task<Value> SendAPIRequest(string Token, string Uri, string payload, string Path, string method)
        {
            APIHandler ApiHandler = new APIHandler();
            ApiHandler.SetRequestHeaders(Token);
            ApiHandler.SetURLEndPoint(Uri + Path);

            if (method == "Post")
            {
                return await ApiHandler.PostAPIRequest(Uri + Path, ApiHandler.SetRequestBody(payload));
            }
            else if (method == "Patch")
            {
                return await ApiHandler.PatchAPIRequest(Uri + Path, ApiHandler.SetRequestBody(payload));
            }
            else
            {
                throw new ArgumentException("Invalid HTTP method specified. Only 'Get', 'Post', or 'Patch' are supported.");
            }
        }

        private static async Task<GetAzDoTestRun> SendAPIRequest(string Token, string Uri, string Path, string method)
        {
            APIHandler ApiHandler = new APIHandler();
            ApiHandler.SetRequestHeaders(Token);
            ApiHandler.SetURLEndPoint(Uri + Path);

            return await ApiHandler.GetAPIRequest();

        }
        private static Value CreateNewTestRun(string Token, string Uri, string Content)
        {
            string NewTestRunPath = "/_apis/test/runs?api-version=7.0";
            var task = SendAPIRequest(Token, Uri, Content, NewTestRunPath, "Post");
            task.Wait();
            var result = task.Result;
            return result;
        }

        private static Value PatchNewTestRun(string Token, string Uri, string[] Content, string NewTestRunID)
        {
            string bodyRequest = "[" + string.Join(",", Content) + "]";
            string NewTestRunPath = $"/_apis/test/Runs/{NewTestRunID}/results?api-version=7.0";
            var task = SendAPIRequest(Token, Uri, bodyRequest, NewTestRunPath, "Patch");
            task.Wait();
            var result = task.Result;
            return result;
        }

        private static GetAzDoTestRun GetTestRun(string Token, string Uri, string NewTestRunID)
        {
            string GetTestRunPath = $"/_apis/test/Runs/{NewTestRunID}/results?api-version=7.2-preview.6";
            var task = SendAPIRequest(Token, Uri, GetTestRunPath, "Get");
            task.Wait();
            var result = task.Result;
            return result;
        }


        protected override void ProcessRecord()
        {
            this.Token = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"`:{Token}"));
            string BodyRequest = "";
            var testplanID = TestPlanObject.value[0].testPlan.id;
            var TestSuiteID = TestPlanObject.value[0].testSuite.id;

            FileHandler.Instance.Initialize(FilePath);
            JTestSuite JUnitResults = FileHandler.Instance.ReadXML();
            //WriteObject(JUnitResults);
            //GetFileMetaData(FilePath);

            BodyRequest = ProcessTestRun(JUnitResults);
            //Console.Write("ProcessTestRun BodyRequest: " + BodyRequest);

            var list = CreateNewTestRun(Token, Uri, BodyRequest);

            var GetResult = GetTestRun(Token, Uri, list.id.ToString());
            //WriteObject(GetResult);
            string[] arrayBodyRequest = LogicProcess.PrepareResultsToPatch(GetResult, JUnitResults);

            //Console.Write("PrepareResultsToPatch BodyRequest: " + arrayBodyRequest);

            var list2 = PatchNewTestRun(Token, Uri, arrayBodyRequest, list.id.ToString());

            Console.WriteLine($"Result Published to {Uri}/_testPlans/execute?planId={testplanID}&suiteId={TestSuiteID}");
            Console.WriteLine($"Test Configuration: {TestConfiguration}");
            PrintResults();
            PrintNotProcessTestCases();

        } // End of class ProcessRecord

        private void PrintResults()
        {
            TestResultsCollector results = LogicProcess.GetTestResultsCollector();
            results.PrintResults();
        }

        private void PrintNotProcessTestCases()
        {
            TestResultsCollector results = LogicProcess.GetTestResultsCollector();
            results.PrintNotFoundTestCase();
        }


        private GetAzDoTestRun SetToAzTestplanModel(Value list)
        {
            GetAzDoTestRun getAzDoTestRun = new GetAzDoTestRun
            {
                count = 1, // Set count property if needed
                value = new List<Value> { list }
            };

            return getAzDoTestRun;
        }

        private string ProcessTestRun(JTestSuite JUnitResults)
        {
            DateTime now = DateTime.Now;
            string formattedDate = now.ToString("yyyy-MM-dd.HHmm");
            JsonElement TestPlanIDObj = (JsonElement)TestPlanObject.value[0].testPlan.id;
            int TestPlanID = TestPlanIDObj.GetInt32();
            var ObjBuilder = new ObjectBuilder();
            List<int> TestPoints = null;

            if (TestPlanObject != null)
            {

                LogicProcess.ReadEachTestCasesArray(JUnitResults, TestPlanObject, TestConfiguration);
                TestPoints = LogicProcess.GetTestPoints();
                //WriteObject($"List of Testpoint: {TestPoints}");

            } // End of if statement

            return ObjBuilder.BuildNewTestRun(formattedDate, TestPlanID, TestPoints);
        }
    } // End of class GetXmlCommand
} // End of namespace