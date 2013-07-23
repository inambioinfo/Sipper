﻿using System;
using System.Linq;
using System.Threading;
using DeconTools.UnitTesting2;
using DeconTools.Workflows.Backend.Core;
using NUnit.Framework;
using Sipper.Model;
using Sipper.ViewModel;

namespace Sipper.UnitTesting.ViewModelTests
{
    [TestFixture]
    public class ManualViewingTests
    {
        [Test]
        public void loadResultsTest()
        {
            string testParameterFile =
                @"\\protoapps\UserData\Slysz\Standard_Testing\Targeted_FeatureFinding\SIPPER_standard_testing\SipperTargetedWorkflowParameters1.xml";


            string testResultFile =
                @"\\protoapps\UserData\Slysz\Standard_Testing\Targeted_FeatureFinding\SIPPER_standard_testing\Yellow_C13_070_23Mar10_Griffin_10-01-28_testing_results.txt";

            


            FileInputsInfo fileInputs = new FileInputsInfo();
            fileInputs.ParameterFilePath = testParameterFile;
            fileInputs.TargetsFilePath = testResultFile;

            ViewAndAnnotateViewModel viewModel = new ViewAndAnnotateViewModel(fileInputs);


            viewModel.LoadResults(testResultFile);
            //Assert.IsNotEmpty(viewModel.Results);
        }



        [Test]
        public void loadParametersTest1()
        {
            string testParameterFile =
                @"\\protoapps\UserData\Slysz\Standard_Testing\Targeted_FeatureFinding\SIPPER_standard_testing\SipperTargetedWorkflowParameters1.xml";

            FileInputsInfo fileInputs = new FileInputsInfo();
            fileInputs.ParameterFilePath = testParameterFile;

            ViewAndAnnotateViewModel viewModel = new ViewAndAnnotateViewModel(fileInputs);

            Assert.IsNotNull(viewModel.Workflow.WorkflowParameters);
            Assert.AreEqual(2,((TargetedWorkflowParameters)(viewModel.Workflow.WorkflowParameters)).MSPeakDetectorPeakBR);

        }


        [Test]
        public void executeWorkflowTest1()
        {
            string testDatafile =
                @"\\protoapps\UserData\Slysz\Standard_Testing\Targeted_FeatureFinding\SIPPER_standard_testing\Yellow_C13_070_23Mar10_Griffin_10-01-28.raw";

            string testResultFile =
                @"\\protoapps\UserData\Slysz\Standard_Testing\Targeted_FeatureFinding\SIPPER_standard_testing\Results\Yellow_C13_070_23Mar10_Griffin_10-01-28_temp_results.txt";

            string testParameterFile =
                @"\\protoapps\UserData\Slysz\Standard_Testing\Targeted_FeatureFinding\SIPPER_standard_testing\SipperTargetedWorkflowParameters1.xml";

            ViewAndAnnotateViewModel viewModel = new ViewAndAnnotateViewModel();

            viewModel.FileInputs.ParameterFilePath = testParameterFile;
            viewModel.FileInputs.TargetsFilePath = testResultFile;
            viewModel.FileInputs.DatasetPath = testDatafile;

            viewModel.LoadResults(testResultFile);

            viewModel.CurrentResult = viewModel.Results.First(p => p.TargetID == 5555);
            
            Thread.Sleep(5000);  //need to wait for peaks to load.... 
            viewModel.ExecuteWorkflow();

            Assert.IsNotNull(viewModel.MassSpecXyData);
            Assert.IsNotNull(viewModel.ChromXyData);
            Assert.IsNotNull(viewModel.TheorProfileXyData);
            Assert.IsNotNull(viewModel.LabelDistributionXyData);


            //TestUtilities.DisplayXYValues(viewModel.MassSpecXYData);

            Console.WriteLine("CurrentScanSet= " + viewModel.CurrentLcScan);

            viewModel.NavigateToNextMs1MassSpectrum();

            Console.WriteLine("After manual navigating... CurrentScanSet= " + viewModel.CurrentLcScan);

            viewModel.NavigateToNextMs1MassSpectrum();

            Console.WriteLine("After manual navigating... CurrentScanSet= " + viewModel.CurrentLcScan);

        }


        [Test]
        public void executeWorkflowTest2_crashTest()
        {
            string testDatafile =
                @"F:\Yellowstone\RawData\Yellow_C13_086_23Mar10_Griffin_10-01-28.raw";

            string testResultFile =
                @"C:\Users\d3x720\Documents\PNNL\My_DataAnalysis\2012\C12C13YellowStone\2012_05_07_Data for Laurey Steinke paper\_Yellow_C13_F2_Enriched_NR_LS_GWS_validated.txt";

            string testParameterFile =
                @"\\protoapps\UserData\Slysz\Data\Yellowstone\SIPPER\SipperTargetedWorkflowParameters_Sum5.xml";

            ViewAndAnnotateViewModel viewModel = new ViewAndAnnotateViewModel();

            viewModel.FileInputs.ParameterFilePath = testParameterFile;
            viewModel.FileInputs.TargetsFilePath = testResultFile;
            viewModel.FileInputs.DatasetPath = testDatafile;

            //viewModel.CurrentResultInfo = viewModel.Results.First(p => p.TargetID == 15922);

            viewModel.ExecuteWorkflow();

            //viewModel.CurrentResultInfo = (from n in viewModel.Results
            //                           where
            //                               n.DatasetName == "Yellow_C13_085_23Mar10_Griffin_10-03-01" &&
            //                               n.TargetID == 15937
            //                           select n).First();

            viewModel.ExecuteWorkflow();

            Assert.IsNotNull(viewModel.MassSpecXyData);
            Assert.IsNotNull(viewModel.ChromXyData);
            Assert.IsNotNull(viewModel.TheorProfileXyData);
        }

    }
}
