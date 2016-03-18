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
    public class ExtractShells : GH_Component
    {

        //set robot.dll file to false for embed interop
        private Bitmap _icon;

        public ExtractShells() : base("Extract Shells", "Shells", "Extracts all the shells from the ETABS model and converts them to Rhino geometry", "Test", "Extract") 
        {
            _icon = Properties.Resources.Panels;
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
            pManager.AddNumberParameter("Shell Identities", "ID:s", "Shell Identities from ETABS", GH_ParamAccess.list);
            pManager.AddBrepParameter("Shells", "Shells", "Shells from the ETABS model", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            ETABS2013.cOAPI ETABS = null;
            if (!DA.GetData(0, ref ETABS)) { return; }


            //Gets the ETABS geometry
            int numberNames = 0;
            string [] shellList = null;
            ETABS.SapModel.AreaObj.GetNameList(ref numberNames, ref shellList);

            //if there are selcted objects only export these
            List<string> selectedShellsList = new List<string>();
            bool selected = false;
            for (int i = 0; i < shellList.Count(); i++)
            {
                ETABS.SapModel.AreaObj.GetSelected(shellList[i], ref selected);
                if (selected)
                {
                    selectedShellsList.Add(shellList[i]);
                }
            }

            //if no objects are selected export all
            if (selectedShellsList.Count == null)
            {
                selectedShellsList.AddRange(shellList);
            }


            List<Brep> outBreps = new List<Brep>();
            List<int> IDs = new List<int>();
            for (int i = 0; i < selectedShellsList.Count(); i++)
            {                         
                int numberPoints = 0;
                string [] points = null;
                ETABS.SapModel.AreaObj.GetPoints(selectedShellsList[i], ref numberPoints, ref points);

                //Creates the Rhino Geometry
                List<Point3d> cornerPoints = new List<Point3d>();
                Point3d cornerPoint;
                foreach (string point in points)
                {
                    double x = 0;
                    double y = 0;
                    double z = 0;
                    ETABS.SapModel.PointObj.GetCoordCartesian(point, ref x, ref y, ref z);
                    cornerPoint = new Point3d(x, y, z);
                    cornerPoints.Add(cornerPoint);                                        
                }
                Brep outBrep;
                if (cornerPoints.Count == 3)
	            {
                    outBrep = Brep.CreateFromCornerPoints(cornerPoints[0], cornerPoints[1], cornerPoints[2], 0.01);
                    outBreps.Add(outBrep);
	            }
                else if (cornerPoints.Count == 4)
                {
                    outBrep = Brep.CreateFromCornerPoints(cornerPoints[0], cornerPoints[1], cornerPoints[2], cornerPoints[3], 0.01);
                    outBreps.Add(outBrep);
                }
                else
                {
                    //TODO: Deal with area objects with more than 4 corner points
                    //For now add null items so lists match up
                    outBrep = null;
                    outBreps.Add(null);
                }

                int ID = Convert.ToInt32(shellList[i]);
                IDs.Add(ID);
            }


            DA.SetDataList(0, IDs);
            DA.SetDataList(1, outBreps);
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("d3372b07-6eda-4802-97f7-66710d0a66e7"); }
        }
    }
}
