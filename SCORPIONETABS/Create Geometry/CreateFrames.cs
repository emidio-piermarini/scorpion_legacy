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
    public class CreateFrames : GH_Component
    {
        //set robot.dll file to false for embed interop
        private Bitmap _icon;

        public CreateFrames()
            : base("Create Frames", "Frames", "Creates frames from Rhino geometry and exports to ETABS", "Test", "Create")
        {
            _icon = Properties.Resources.Columns;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("ETABS Instance", "ETABS", "ETABS", GH_ParamAccess.item);
            pManager.AddLineParameter("Lines", "Lines", "Lines from Rhino", GH_ParamAccess.list);
        }
        protected override System.Drawing.Bitmap Icon
        {
            get { return _icon; }
        }
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("ETABS Instance", "ETABS", "ETABS", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            ETABS2013.cOAPI ETABS = null;
            List<Line> lines = new List<Line>();
            if (!DA.GetData(0, ref ETABS)) { return; }
            if (!DA.GetDataList(1, lines)) { return; }


            for (int i = 0; i < lines.Count(); i++)
            {

                double x1 = lines[i].PointAt(0).X;
                double y1 = lines[i].PointAt(0).Y;
                double z1 = lines[i].PointAt(0).Z;
                double x2 = lines[i].PointAt(1).X;
                double y2 = lines[i].PointAt(1).Y;
                double z2 = lines[i].PointAt(1).Z;
                string Name = "";

                ETABS.SapModel.FrameObj.AddByCoord(x1, y1, z1, x2, y2, z2, ref Name);
            }
            DA.SetData(0, ETABS);
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("213b9ffa-09f9-48b5-aeab-023e36caaf5b"); }
        }
    }
}
