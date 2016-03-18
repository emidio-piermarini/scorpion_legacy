using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;
using Grasshopper;
using Grasshopper.Getters;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Grasshopper.Plugin;
using Grasshopper.GUI;
using RobotOM;
using Rhino.Geometry;

namespace SCORPIONETABS
{
    //not yet implemented
    class SelectItems : GH_Component
    {

        public SelectItems() : base("Select items", "Select", "Selects all elements close to certain XYZ:s", "Test", "Special") { }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("ETABS Instance", "ETABS", "ETABS", GH_ParamAccess.item);
            pManager.AddPointParameter("X,Y,Z coordinates", "XYZ:s", "X, Y and Z coordinates for points to select everything close to", GH_ParamAccess.list);
            pManager.AddNumberParameter("Tolerance", "tol", "Tolerance when selecting", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("ETABS Instance", "ETABS", "ETABS", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            ETABS2013.cOAPI ETABS = null;
            List<Point3d> pts = new List<Point3d>();
            double tol = 0;

            int ret;

            if (!DA.GetData(0, ref ETABS)) { return; }
            if (!DA.GetDataList(1, pts)) { return; }
            if (!DA.GetData(2, ref tol)) {return;}

            for (int i = 0; i < pts.Count; i++)
            {
                ret = ETABS.SapModel.SelectObj.CoordinateRange(pts[i].X - tol, pts[i].X + tol, pts[i].Y - tol, pts[i].Y + tol, pts[i].Z - tol, pts[i].Z + tol);
            }
            
            DA.SetData(0, ETABS);
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("9f573c44-73c6-44b8-bad6-67fbacf729e3"); }
        }
    }
}
