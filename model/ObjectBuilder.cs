using AzDoTestPlanSDK.Model;
using AzDoTestPlanSDK.Model.JUnit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace PublishAzDoTestResults.model
{
    internal class ObjectBuilder
    {
        private int _ID;
        private string _State;
        private string _Comment;
        private string _TestcaseID;
        private string _Testpoint;
        private string _Outcome;
        private string _Errormessage;
        private string _StackTrace;
        private string _CreatedDate;
        private string _TestCaseTitle;

        public string TestcaseID
        {
            get { return _TestcaseID; }
            set { _TestcaseID = value; }
        }
        public string State
        {
            get { return _State; }
            set { _State = value; }
        }

        public string Comment
        {
            get { return _Comment; }
            set { _Comment = value; }
        }

        public string Testpoint
        {
            get { return _Testpoint; }
            set { _Testpoint = value; }

        }
        public string Outcome
        {
            get { return _Outcome; }
            set { _Outcome = value; }
        }
        public string Errormessage
        {
            get { return _Errormessage; }
            set { _Errormessage = value; }
        }
        public string StackTrace
        {
            get { return _StackTrace; }
            set { _StackTrace = value; }
        }
        public string CreatedDate
        {
            get { return _CreatedDate; }
            set { _CreatedDate = value; }
        }
        public string TestCaseTitle
        {
            get { return _TestCaseTitle; }
            set { _TestCaseTitle = value; }
        }
        public int TestRunValueID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        public string BuildNewTestRun(string formattedDate, int TestPlanID, List<int> TestPoints)
        {
            var obj_BodyNewTestRun = new
            {
                name = "newTestRun" + formattedDate,
                plan = new { id = TestPlanID }, // Create an anonymous object with an 'id' property
                pointIds = TestPoints.ToArray() // Convert TestPoints to an array
                                                // Add more properties as needed
            };
            return JsonSerializer.Serialize(obj_BodyNewTestRun);
        }

        public string BuildPatchTestRun()
        {
            var newObj = new
            {
                id = _ID,
                state = "Completed",
                comment = "Automated update-" + _CreatedDate,
                outcome = _Outcome,
                Errormessage = _Errormessage,
                CreatedDate = _CreatedDate,
                stackTrace = _StackTrace,
                testCaseTitle = _TestCaseTitle,
                startedDate = _CreatedDate,
                completedDate = _CreatedDate
            };

            return JsonSerializer.Serialize(newObj);
        }




    }
}
