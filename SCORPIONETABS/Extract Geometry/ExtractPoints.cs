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
    public class ExtractPoints : GH_Component
    {

        //set robot.dll file to false for embed interop
        private Bitmap _icon;

        public ExtractPoints()
            : base("Extract Points", "Points", "Extracts all the points from the ETABS model and converts them to Rhino geometry", "Test", "Extract")
        {
            _icon = Properties.Resources.Columns;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("ETABS Instance", "ETABS", "ETABS", GH_ParamAccess.item);
        }
        protected override System.Drawing.Bitmap Icon
        {
            get { return _icon; }
        }
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Point Identities", "ID:s", "Point Identities from ETABS", GH_ParamAccess.list);
            pManager.AddPointParameter("Points", "Points", "Points from the ETABS model", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            ETABS2013.cOAPI ETABS = null;
            if (!DA.GetData(0, ref ETABS)) { return; }

            //Gets the ETABS geometry
            int numberNames = 0;
            string[] pointList = null;
            ETABS.SapModel.PointObj.GetNameList(ref numberNames, ref pointList);

            List<Point3d> outPoints = new List<Point3d>();
            List<int> IDs = new List<int>();

            for (int i = 0; i < pointList.Count(); i++)
            {
                double x1 = 0;
                double y1 = 0;
                double z1 = 0;
                ETABS.SapModel.PointObj.GetCoordCartesian(pointList[i], ref x1, ref y1, ref z1);

                Point3d pt = new Point3d(x1, y1, z1);
                outPoints.Add(pt);

                int ID = Convert.ToInt32(pointList[i]);
                IDs.Add(ID);
            }




            DA.SetDataList(0, IDs);
            DA.SetDataList(1, outPoints);
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("134b75f3-c5f3-42e7-98ba-43a586b34732"); }
        }
    }
}
