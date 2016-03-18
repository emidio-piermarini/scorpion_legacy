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
    public class CreateShells : GH_Component
    {
        //set robot.dll file to false for embed interop
        private Bitmap _icon;

        public CreateShells()
            : base("Create Shells", "Shells", "Creates shells from Rhino geometry and exports to ETABS", "Test", "Create")
        {
            _icon = Properties.Resources.Panels;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("ETABS Instance", "ETABS", "ETABS", GH_ParamAccess.item);
            pManager.AddMeshParameter("Mesh Faces", "Mesh Faces", "Mesh faces from Rhino", GH_ParamAccess.list);
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
            ETABS2013.cOAPI activate = null;
            List<GeometryBase> surfaces = new List<GeometryBase>();
            if (!DA.GetData(0, ref activate)) { return; }
            if (!DA.GetDataList(1, surfaces)) { return; }


            /*   for (int i = 0; i < meshes.Count(); i++)
              {



                  //Get the contour polyline and its points (this is done to get the points in the right order
                  Rhino.Geometry.Plane mshPlane = new Rhino.Geometry.Plane(Plane.WorldXY.Origin, new Vector3d(0, 0, 1));
                  Polyline[] mshOutlines = meshes[i].GetOutlines(mshPlane);
                  List<Point3d> pts = mshOutlines[0].GetRange(0, mshOutlines[0].Count - 1).ToList(); // Last value in the range is the first point again
                
                  //I uset arrays cus the AddByCoord function can only handle arrays
                  double[] xValues = new double[pts.Count];
                  double[] yValues = new double[pts.Count];
                  double[] zValues = new double[pts.Count];
                  for(int j = 0; j < pts.Count; j++)
                  {
                      xValues[j] =pts[j].X;
                      yValues[j] = pts[j].Y;
                      zValues[j] = pts[j].Z;
                  }



                  string Name = "Slab" + i;
                  ETABS.SapModel.AreaObj.AddByCoord(pts.Count(), ref xValues, ref yValues, ref zValues, ref Name);
                  ETABS.SapModel.View.RefreshView(0, false);
              } */


            //Connect from the input ETABS file
            ETABS2013.cOAPI ETABS = (ETABS2013.cOAPI)activate;
            Mesh mesh = new Mesh();

            for (int i = 0; i < surfaces.Count(); i++)
            {

                //If input is brep convert to mesh (easiest way to get the points)
                if (surfaces[i].GetType() == typeof(Rhino.Geometry.Brep))
                {
                    MeshingParameters mshParam = new MeshingParameters();
                    mshParam.MinimumTolerance = 0.01; //we can get meshing params from a standard GH component as it is a generic type

                    Mesh[] mshArray = Mesh.CreateFromBrep((Brep)surfaces[i], mshParam);
                    mesh = mshArray[0];
                }
                //If input is a mesh no convertion is needed
                else if (surfaces[i].GetType() == typeof(Rhino.Geometry.Mesh))
                    mesh = (Mesh)surfaces[i];
                //If not mesh or brep do nothing (return)
                else
                    return;

                //Get the contour polyline and its points (this is done to get the points in the right order
                BoundingBox bMesh = new BoundingBox();
                Rhino.Geometry.Plane mshPlane = new Rhino.Geometry.Plane(bMesh.Center, mesh.FaceNormals[0]); //Just take one normal on the surface since it's planar
                Polyline[] mshOutlines = mesh.GetOutlines(mshPlane);
                List<Point3d> pts = mshOutlines[0].GetRange(0, mshOutlines[0].Count - 1).ToList(); // Last value in the range is the first point again

                //I use arrays because the AddByCoord function can only handle arrays
                double[] xValues = new double[pts.Count];
                double[] yValues = new double[pts.Count];
                double[] zValues = new double[pts.Count];
                for (int j = 0; j < pts.Count; j++)
                {
                    //Add all the x,y,z values from the mesh into arrays, used in AddByCoord function
                    xValues[j] = pts[j].X;
                    yValues[j] = pts[j].Y;
                    zValues[j] = pts[j].Z;
                }
                string Name = "Slab" + i;
                //Add the mesh in ETABS and refresh the view
                ETABS.SapModel.AreaObj.AddByCoord(pts.Count(), ref xValues, ref yValues, ref zValues, ref Name);
                ETABS.SapModel.View.RefreshView(0, false);



                DA.SetData(0, ETABS);
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("d530e872-7adb-447b-9d7f-79269778787e"); }
        }
    }
}
