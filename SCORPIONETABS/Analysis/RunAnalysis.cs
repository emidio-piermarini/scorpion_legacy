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
    public class RunAnalysis : GH_Component
    {
        private Bitmap _icon;
        public RunAnalysis() : base("Run Analysis", "Run", "Runs analysis in ETABS", "Test", "Analysis") 
        {
            _icon = Properties.Resources.Go;
        }
        protected override System.Drawing.Bitmap Icon
        {
            get { return _icon; }
        }
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("ETABS Instance", "ETABS", "ETABS", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("ETABS Instance", "ETABS", "ETABS", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            
            ETABS2013.cOAPI ETABS = null;
            if (!DA.GetData(0, ref ETABS)) { return; }         
            ETABS.SapModel.Analyze.RunAnalysis();
            
            //try
            //{
            //    int NumberNames = 0;
            //    string[] MyName = string[0];
            //    ETABS.SapModel.PointObj.GetNameList(ref NumberNames, ref MyName);
            //}
            //catch (Exception)
            //{
                
            //    System.Reflection.Assembly ETABSassembly = System.Reflection.Assembly.GetAssembly(ETABS2013.cOAPI);
            //    ETABS = (ETABS2013.cOAPI)ETABSassembly.CreateInstance("CSI.ETABS.API.ETABSObject");
                
            //}
            DA.SetData(0, ETABS);
            System.GC.KeepAlive(ETABS);
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("6ef60eac-879b-4bbf-9cfb-457fa8d14e53"); }
        }
    }
}
