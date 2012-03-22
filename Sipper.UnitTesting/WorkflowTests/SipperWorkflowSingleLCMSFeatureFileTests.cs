﻿using System;
using System.Linq;
using DeconTools.Backend;
using DeconTools.Backend.Core;
using DeconTools.Backend.FileIO;
using DeconTools.Backend.Utilities;
using DeconTools.Workflows.Backend.Core;
using NUnit.Framework;
using Sipper.Backend;
using Sipper.Backend.Results;

namespace Sipper.UnitTesting.WorkflowTests
{
    [TestFixture]
    public class SipperWorkflowSingleLCMSFeatureFileTests
    {
        [Test]
        public void loadLCMSFeatures_and_massTags()
        {

            string lcmsfeaturesFile = 
                @"C:\Users\d3x720\Documents\PNNL\My_DataAnalysis\2012\C12C13YellowStone\2011_02_20_SIPPER_workflow_standards\Yellow_C13_070_23Mar10_Griffin_10-01-28_LCMSFeatures.txt";

            // load LCMSFeatures as targets
            LcmsTargetFromFeaturesFileImporter importer =
                new LcmsTargetFromFeaturesFileImporter(lcmsfeaturesFile);

            var lcmsTargetCollection = importer.Import();


            // load MassTags
            string massTagFile1 =
                @"C:\Users\d3x720\Documents\PNNL\My_DataAnalysis\2012\C12C13YellowStone\2011_02_20_SIPPER_workflow_standards\Yellow_C13_070_23Mar10_Griffin_10-01-28_MassTags.txt";

            MassTagFromTextFileImporter massTagImporter = new MassTagFromTextFileImporter(massTagFile1);
            var massTagCollection =   massTagImporter.Import();


            var masstagIDlist = (from n in massTagCollection.TargetList select n.ID).ToList();


            // Update LCMSFeatures using MassTag info




            foreach (LcmsFeatureTarget target in lcmsTargetCollection.TargetList)
            {
                
                if (masstagIDlist.Contains(target.ID))
                {
                    var mt=  massTagCollection.TargetList.Where(p => p.ID == target.FeatureToMassTagID).First();
                    target.Code = mt.Code;
                    target.EmpiricalFormula = mt.EmpiricalFormula;
                }
                else
                {
                    
                }
      
            }

            int[] testMassTags = {344540889, 344540889, 344972415, 354881152, 355157363, 355162540, 355315129};

            var filteredLcmsFeatureTargets = (from n in lcmsTargetCollection.TargetList
                                              where testMassTags.Contains(((LcmsFeatureTarget) n).FeatureToMassTagID)
                                              select n).ToList();


            foreach (LcmsFeatureTarget filteredLcmsFeatureTarget in filteredLcmsFeatureTargets)
            {
                Console.WriteLine(filteredLcmsFeatureTarget);
            }


        }


        [Test]
        public void executeWorkflowTest1()
        {
            string testFile = FileRefs.RawDataFile1;
            string peaksFile =
                @"C:\Users\d3x720\Documents\PNNL\My_DataAnalysis\2012\C12C13YellowStone\2011_02_20_SIPPER_workflow_standards\Yellow_C13_070_23Mar10_Griffin_10-01-28_peaks.txt";

            Run run= RunUtilities.CreateAndLoadPeaks(testFile, peaksFile);


            string lcmsfeaturesFile =
             @"C:\Users\d3x720\Documents\PNNL\My_DataAnalysis\2012\C12C13YellowStone\2011_02_20_SIPPER_workflow_standards\Yellow_C13_070_23Mar10_Griffin_10-01-28_LCMSFeatures.txt";

            // load LCMSFeatures as targets
            LcmsTargetFromFeaturesFileImporter importer =
                new LcmsTargetFromFeaturesFileImporter(lcmsfeaturesFile);

            var lcmsTargetCollection = importer.Import();


            // load MassTags
            string massTagFile1 =
                @"C:\Users\d3x720\Documents\PNNL\My_DataAnalysis\2012\C12C13YellowStone\2011_02_20_SIPPER_workflow_standards\Yellow_C13_070_23Mar10_Griffin_10-01-28_MassTags.txt";

            MassTagFromTextFileImporter massTagImporter = new MassTagFromTextFileImporter(massTagFile1);
            var massTagCollection = massTagImporter.Import();


            var masstagIDlist = (from n in massTagCollection.TargetList select n.ID).ToList();


            // Update LCMSFeatures using MassTag info

            //int[] testMassTags = { 344540889, 344540889, 344972415, 354881152, 355157363, 355162540, 355315129, 355054192, 355160150};



            //no enrichment peptides:
            int[] testMassTags = { 355057553, 355058671, 355084418 };

            //enriched
            testMassTags = new int[] { 355116553, 355129038, 355160150, 355162540, 355163371 };
            


            //int[] testMassTags = { 355058604 };

            var filteredLcmsFeatureTargets = (from n in lcmsTargetCollection.TargetList
                                              where testMassTags.Contains(((LcmsFeatureTarget)n).FeatureToMassTagID)
                                              select n).ToList();


            foreach (LcmsFeatureTarget target in filteredLcmsFeatureTargets)
            {

                if (masstagIDlist.Contains(target.FeatureToMassTagID))
                {
                    var mt = massTagCollection.TargetList.Where(p => p.ID == target.FeatureToMassTagID).First();
                    target.Code = mt.Code;
                    target.EmpiricalFormula = mt.EmpiricalFormula;
                }
                else
                {

                }

            }

           


            TargetedWorkflowParameters parameters = new BasicTargetedWorkflowParameters();
            parameters.ChromPeakSelectorMode  = Globals.PeakSelectorMode.ClosestToTarget;

            SipperTargetedWorkflow workflow = new SipperTargetedWorkflow(run, parameters);


            foreach (var target in filteredLcmsFeatureTargets)
            {
                run.CurrentMassTag = target;

                workflow.Execute();


            }

            foreach (var targetedResultBase in (run.ResultCollection.MassTagResultList))
            {
                var result = (SipperTargetedResult) targetedResultBase.Value;

                Console.WriteLine(result + "\t" + result.Target.IsotopicProfile.Peaklist.Count);
            }


        }


    }
}
