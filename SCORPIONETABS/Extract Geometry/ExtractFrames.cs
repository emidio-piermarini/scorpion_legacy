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
    public class ExtractFrames : GH_Component
    {

        //set robot.dll file to false for embed interop
        private Bitmap _icon;

        public ExtractFrames()
            : base("Extract Frames", "Frames", "Extracts all the frames from the ETABS model and converts them to Rhino geometry", "Test", "Extract")
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
            pManager.AddNumberParameter("Frame Identities", "ID:s", "Frame Identities from ETABS", GH_ParamAccess.list);
            pManager.AddLineParameter("Frames", "Frames", "Frames from the ETABS model", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            ETABS2013.cOAPI ETABS = null;
            if (!DA.GetData(0, ref ETABS)) { return; }

            //Gets the ETABS geometry
            int numberNames = 0;
            string[] frameList = null;
            ETABS.SapModel.FrameObj.GetNameList(ref numberNames, ref frameList);

            //if there are selcted objects only export these
            List<string> selectedFramesList = new List<string>();
            bool selected = false;
            for (int i = 0; i < frameList.Count(); i++)
			{
                ETABS.SapModel.FrameObj.GetSelected(frameList[i], ref selected);
                if (selected)
                {
                    selectedFramesList.Add(frameList[i]);
                }
			}

            //if no objects are selected export all
            if (selectedFramesList.Count == null)
            {
                selectedFramesList.AddRange(frameList);
            }

            List<Line> outLines = new List<Line>();
            List<int> IDs = new List<int>();

            string point1 = null;
            string point2 = null;
            double x1 = 0;
            double x2 = 0;
            double y1 = 0;
            double y2 = 0;
            double z1 = 0;
            double z2 = 0;
            Point3d pt1 = new Point3d();
            Point3d pt2 = new Point3d();
            for (int i = 0; i < selectedFramesList.Count(); i++)
            {
                ETABS.SapModel.FrameObj.GetPoints(selectedFramesList[i], ref point1, ref point2);
                ETABS.SapModel.PointObj.GetCoordCartesian(point1, ref x1, ref y1, ref z1);
                ETABS.SapModel.PointObj.GetCoordCartesian(point2, ref x2, ref y2, ref z2);
                pt1 = new Point3d(x1, y1, z1);
                pt2 = new Point3d(x2, y2, z2);
                Line columnLine = new Line(pt1, pt2);
                outLines.Add(columnLine);

                int ID = Convert.ToInt32(selectedFramesList[i]);
                IDs.Add(ID);
            }
            

            

            DA.SetDataList(0, IDs);
            DA.SetDataList(1, outLines);
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("f2f76d60-9b2d-4be8-aec2-cb72d231bd38"); }
        }
    }
}