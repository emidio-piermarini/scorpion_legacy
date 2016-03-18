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
    public class AnalysisResultsModalPeriodsAndFrequencies : GH_Component
    {
        private Bitmap _icon;
        public AnalysisResultsModalPeriodsAndFrequencies()
            : base("Analysis results Modal Periods And Frequencies", "Modal Periods And Frequencies", "Extracts the analysis results for Modal Periods And Frequencies", "Test", "Analysis")
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
            pManager.AddNumberParameter("Period", "Period", "Period", GH_ParamAccess.list);
            pManager.AddNumberParameter("Frequency", "Frequency", "Frequency", GH_ParamAccess.list);
            pManager.AddNumberParameter("CirqFrequency", "CirqFrequency", "CirqFrequency", GH_ParamAccess.list);
            pManager.AddNumberParameter("EigenValue", "EigenValue", "EigenValue", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {

            ETABS2013.cOAPI ETABS = null;
            string loadcase = null;
            if (!DA.GetData(0, ref ETABS)) { return; }
            if (!DA.GetData(1, ref loadcase)) { return; }

            int numberResults = 0;
            string[] loadcaseArray = new string[1];
            loadcaseArray[0] = loadcase;
            string[] StepType = new string[100];
            double[] StepNum = new double[100];
            double[] Period = new double[100];
            double[] Frequency = new double[100];
            double[] CirqFrequency = new double[100];
            double[] EigenValue = new double[100];

            int ret;
            ret = ETABS.SapModel.Results.Setup.SetCaseSelectedForOutput(loadcase);

            ret = ETABS.SapModel.Results.ModalPeriod(ref numberResults, ref loadcaseArray, ref StepType, ref StepNum, ref Period, ref Frequency, ref CirqFrequency, ref EigenValue);

            //outputting the etabsobj even though nothing has changed... to be able to extract other analysis results before next iteration in a loop.
            //Is there a better way to do this?
            DA.SetData(0, ETABS);
            DA.SetDataList(1, Period.ToList());
            DA.SetDataList(2, Frequency.ToList());
            DA.SetDataList(3, CirqFrequency.ToList());
            DA.SetDataList(4, EigenValue.ToList());
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("f4ecba83-91a9-4fee-b030-75d3f289b065"); }
        }
    }
}