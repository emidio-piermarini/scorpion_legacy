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
    public class AnalysisResultsStoryDrifts : GH_Component
    {
        private Bitmap _icon;
        public AnalysisResultsStoryDrifts()
            : base("Analysis results Story Drifts", "Story Drifts", "Extracts the analysis results for story drifts", "Test", "Analysis")
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
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {

            ETABS2013.cOAPI ETABS = null;
            string loadcase = null;
            if (!DA.GetData(0, ref ETABS)) { return; }
            if (!DA.GetData(1, ref loadcase)) { return; }

            Rhino.RhinoDoc RhinoDoc = Rhino.RhinoDoc.ActiveDoc;

            Rhino.Display.RhinoView view = RhinoDoc.Views.ActiveView;
            System.Drawing.Bitmap picture = view.CaptureToBitmap();
            picture.Save("H:\\printad.bmp");
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("921bd494-a5ad-4af6-9455-73fef79ea977"); }
        }
    }
}

