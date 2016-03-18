using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper;
using Grasshopper.Getters;
using Grasshopper.Plugin;
using Grasshopper.GUI;
using RobotOM;
using Rhino.Geometry;

namespace SCORPIONETABS
{
    public class AnalysisResultsColumns : GH_Component
    {
        private Bitmap _icon;
        public AnalysisResultsColumns() : base("Analysis results columns", "Column Results", "Extracts all the columns from the ETABS model and gets the results", "Test", "Analysis") 
        {
            _icon = Properties.Resources.TableColumns;
        }
        protected override System.Drawing.Bitmap Icon
        {
            get { return _icon; }
        }
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("ETABS Instance", "ETABS", "ETABS", GH_ParamAccess.item);
            pManager.AddTextParameter("Loadcase/Combo", "Loadcase", "Loadcase or load combo input as a string", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Shell Identities", "ID:s", "Shell Identities from ETABS", GH_ParamAccess.list);
            pManager.AddNumberParameter("P", "P", "TODO: find out what this is exactly", GH_ParamAccess.list);
            pManager.AddNumberParameter("V2", "V2", "TODO: find out what this is exactly", GH_ParamAccess.list);
            pManager.AddNumberParameter("V3", "V3", "TODO: find out what this is exactly", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {

            ETABS2013.cOAPI ETABS = null;
            string loadcase = null;
            if (!DA.GetData(0, ref ETABS)) { return; }
            if (!DA.GetData(1, ref loadcase)) { return; }

            //Gets the ETABS geometry
            int numberNames = 0;
            string[] frameList = null;
            ETABS.SapModel.FrameObj.GetNameList(ref numberNames, ref frameList);

            List<int> IDs = new List<int>();
            List<double> PList = new List<double>();
            List<double> V2List = new List<double>();
            List<double> V3List = new List<double>();
            int ret;
            ret = ETABS.SapModel.Results.Setup.SetCaseSelectedForOutput(loadcase);

            for (int i = 0; i < frameList.Count(); i++)
            {

                //Define all output arrays
                ETABS2013.eItemTypeElm ItemTypeElm = new ETABS2013.eItemTypeElm();
                int NumberResults = 20;
                string[] obj = new string[100];
                double[] objSta = new double[100];
                string[] elm = new string[100];
                double[] elmSta = new double[100];
                string[] PointElm = new string[100];
                string[] LoadCase = new string[100];
                string[] StepType = new string[100];
                double[] StepNum = new double[100];
                double[] P = new double[100];
                double[] V2 = new double[100];
                double[] V3 = new double[100];
                double[] T = new double[100];
                double[] M2 = new double[100];
                double[] M3 = new double[100];
                //Gets the analysis results
                ret = ETABS.SapModel.Results.FrameForce(frameList[i], ItemTypeElm, ref NumberResults, ref obj, ref objSta, ref elm, ref elmSta, ref LoadCase, ref StepType, ref StepNum, ref P, ref V2, ref V3, ref T, ref M2, ref M3);

                //TODO: find out what's interesting
                int ID = Convert.ToInt32(frameList[i]);
                IDs.Add(ID);
                PList.Add(P.Max());
                V2List.Add(V2.Max());
                V3List.Add(V3.Max());
            }
            DA.SetDataList(0, IDs);
            DA.SetDataList(1, PList);
            DA.SetDataList(2, V2List);
            DA.SetDataList(3, V3List);
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("2851dcf5-8194-44ae-911d-c4abc884526d"); }
        }
    }
}


