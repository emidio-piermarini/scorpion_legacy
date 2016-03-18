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
    public class AnalysisResultsJointDisplacements : GH_Component
    {
        private Bitmap _icon;
        public AnalysisResultsJointDisplacements()
            : base("Analysis results Joint Displacements", "Joint Displacements", "Extracts the analysis results for Joint Displacements", "Test", "Analysis")
        {
            _icon = Properties.Resources.Table;
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
            pManager.AddGenericParameter("ETABS Instance", "ETABS", "ETABS", GH_ParamAccess.item);
            pManager.AddNumberParameter("Id:s", "Id:s", "ID:s", GH_ParamAccess.list);
            pManager.AddNumberParameter("U1", "U1", "U1", GH_ParamAccess.list);
            pManager.AddNumberParameter("U2", "U2", "U2", GH_ParamAccess.list);
            pManager.AddNumberParameter("U3", "U3", "U3", GH_ParamAccess.list);
            pManager.AddNumberParameter("R1", "R1", "R1", GH_ParamAccess.list);
            pManager.AddNumberParameter("R2", "R2", "R2", GH_ParamAccess.list);
            pManager.AddNumberParameter("R3", "R3", "R3", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {

            ETABS2013.cOAPI ETABS = null;
            string loadcase = null;
            if (!DA.GetData(0, ref ETABS)) { return; }
            if (!DA.GetData(1, ref loadcase)) { return; }

            int numberResults = 0;
            ETABS2013.eItemTypeElm ItemTypeElm = new ETABS2013.eItemTypeElm();
            string[] obj = new string[0];
            string[] elm = new string[0];
            string[] loadcaseArray = new string[1];
            loadcaseArray[0] = loadcase;
            string[] StepType = new string[100];
            double[] StepNum = new double[100];
            double[] U1 = new double[100];
            double[] U2 = new double[100];
            double[] U3 = new double[100];
            double[] R1 = new double[100];
            double[] R2 = new double[100];
            double[] R3 = new double[100];

            //Creates output lists
            List<int> IDs = new List<int>(); 
            List<double> U1list = new List<double>();
            List<double> U2list = new List<double>();
            List<double> U3list = new List<double>();
            List<double> R1list = new List<double>();
            List<double> R2list = new List<double>();
            List<double> R3list = new List<double>();

            //Gets point objects
            int numberNames = 0;
            string[] myName = new string[0];
            ETABS.SapModel.PointObj.GetNameList(ref numberNames, ref myName);

            int ret;
            ret = ETABS.SapModel.Results.Setup.SetCaseSelectedForOutput(loadcase);

            int commonto = 0;


            for (int i = 0; i < myName.Count(); i++)
			{
                ETABS.SapModel.PointObj.GetCommonTo(myName[i], ref commonto); 
                ETABS.SapModel.Results.JointDispl(myName[i], ItemTypeElm, ref numberResults, ref obj, ref elm, ref loadcaseArray, ref StepType, ref StepNum, ref U1, ref U2, ref U3, ref R1, ref R2, ref R3);
                int ID = Convert.ToInt32(myName[i]);
                IDs.Add(ID);
                U1list.Add(U1[0]);
                U2list.Add(U2[0]);
                U3list.Add(U3[0]);
                R1list.Add(R1[0]);
                R2list.Add(R2[0]);
                R3list.Add(R3[0]);
			}

            //outputting the etabsobj even though nothing has changed... to be able to extract other analysis results before next iteration in a loop.
            //Is there a better way to do this?
            DA.SetData(0, ETABS);
            DA.SetDataList(1, IDs);
            DA.SetDataList(2, U1list);
            DA.SetDataList(3, U2list);
            DA.SetDataList(4, U3list);
            DA.SetDataList(5, R1list);
            DA.SetDataList(6, R2list);
            DA.SetDataList(7, R3list);
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("16705d4d-c470-47f5-bedd-c60f01a103f7"); }
        }
    }
}
