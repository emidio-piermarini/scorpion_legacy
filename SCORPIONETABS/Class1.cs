using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;
using Grasshopper;
using Grasshopper.Getters;
using Grasshopper.Plugin;
using Grasshopper.GUI;
using RobotOM;
using Rhino.Geometry;

namespace SCORPIONETABS
{
    //turned off for demo
    public class Class1 : GH_Component
    {
        private IRobotApplication robApp;

        //set robot.dll file to false for embed interop

        public Class1() : base("Test", "Test", "Test", "Test", "test") { }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBrepParameter("Brep", "Brep", "Brep", GH_ParamAccess.list);
            pManager.AddCurveParameter("curve", "curve", "curve", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            //pManager.AddTextParameter("ID", "I", "ID", GH_ParamAccess.item);
            pManager.AddGenericParameter("ETABS Instance", "ETABS", "ETABS", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            #region Create Variables
            List<Grasshopper.Kernel.Types.GH_Curve> lines = new List<Grasshopper.Kernel.Types.GH_Curve>();
            List<Grasshopper.Kernel.Types.GH_Brep> panels = new List<Grasshopper.Kernel.Types.GH_Brep>();

            #endregion

            #region Extract Data from GH
            if (!DA.GetDataList(0, panels)) { return; }
            if (panels == null) { return; }
            if (panels.Count == 0) { return; }

            if (!DA.GetDataList(1, lines)) { return; }
            if (lines == null) { return; }
            if (lines.Count == 0) { return; }



            #endregion

            //Create ETABS Object
            string pathtoetabs = @"C:\Program Files\Computers and Structures\ETABS 2013\ETABS.exe";
            System.Reflection.Assembly ETABSassembly = System.Reflection.Assembly.LoadFrom(pathtoetabs);
            ETABS2013.cOAPI ETABS = (ETABS2013.cOAPI)ETABSassembly.CreateInstance("CSI.ETABS.API.ETABSObject"); //if developer settings checkobx is left checked it stops here (1. Solution exception:Unable to cast object of type 'CSI.ETABS.API.ETABSObject' to type 'ETABS2013.cOAPI'.)

            //Start ETABS and Create New Model
            ETABS.ApplicationStart();
            ETABS.SapModel.InitializeNewModel();
            ETABS.SapModel.File.NewBlank();
            ETABS.SapModel.SetPresentUnits(ETABS2013.eUnits.kgf_mm_C);
            ETABS.SapModel.File.Save(@"C:\Users\janderss\testingtesting.EDB");
            int counter = 0;
            //Import Lines
            foreach (Grasshopper.Kernel.Types.GH_Curve i in lines)
            {

                string label = counter.ToString();
                //Insert Nodes
                ETABS.SapModel.PointObj.AddCartesian(i.Value.PointAtStart.X, i.Value.PointAtStart.Y, i.Value.PointAtStart.Z, ref  label, "", "Global", false, 0);
                ETABS.SapModel.PointObj.AddCartesian(i.Value.PointAtEnd.X, i.Value.PointAtEnd.Y, i.Value.PointAtEnd.Z, ref  label, "", "Global", false, 0);
                //Insert Frames
                ETABS.SapModel.FrameObj.AddByCoord(i.Value.PointAtStart.X, i.Value.PointAtStart.Y, i.Value.PointAtStart.Z, i.Value.PointAtEnd.X, i.Value.PointAtEnd.Y, i.Value.PointAtEnd.Z, ref label, "Default", "", "Global");

                counter++;
            }
            //Use Robot to Mesh Surfaces

            #region create Robot app
            //      robApp = new RobotApplicationClass();
            //      // if Robot is not visible
            //      if (robApp.Visible == 0)   // robApp.Visible = -1
            //      {
            //          robApp.Interactive = 1;   // set robot visible and allow user interaction
            //          robApp.Visible = 1;
            //      }
            //      robApp.Project.New(IRobotProjectType.I_PT_SHELL);
            //      IRobotStructure str = robApp.Project.Structure;
            //      RobotLabelServer labelserv = null;

            //      robApp.Project.Structure.ResultsFreeze=false;
            //      //Delete all the panel objects In the model(don't use clear as this clears loadcases etc.)

            //    //  If Delete_Panels = True Then
            //      RobotSelection prev_panel_sel;
            //      prev_panel_sel=robApp.Project.Structure.Selections.CreateFull(IRobotObjectType.I_OT_PANEL);
            //      robApp.Project.Structure.Objects.DeleteMany(prev_panel_sel);
            //    // End If

            //      RobotPointsArray panel_pnts=new RobotPointsArray();
            //      List<int> panel_nos= new List<int>();
            //      GH_ObjectType obj_type= new GH_ObjectType();
            //      Grasshopper.Kernel.Types.GH_Brep curr_brep = new Grasshopper.Kernel.Types.GH_Brep();

            //      int brepno = 1;
            //     foreach (Grasshopper.Kernel.Types.GH_Brep b in panels)
            //      {
            //         List<Rhino.Geometry.BrepEdge> crvs =new List<Rhino.Geometry.BrepEdge>();
            //         int edges=0;
            //         int i=0;
            //         edges=b.Value.Edges.Count;
            //         for(i=0;i<=edges-1;i++)
            //         {
            //             crvs.Add(b.Value.Edges[i]);
            //         }


            //          string label = b.ToString();
            //          int p = 0;
            //          panel_pnts.SetSize(edges);

            //         counter=1;
            //         foreach (Rhino.Geometry.BrepEdge crv in crvs)
            //         {
            //             panel_pnts.Set(counter, crv.PointAtStart.X, crv.PointAtStart.Y, crv.PointAtStart.Z);
            //             counter++;
            //         }

            //          robApp.Project.Structure.Objects.CreateContour(brepno, panel_pnts);

            //          robApp.Project.Structure.Objects.BeginMultiOperation();
            //          int panel_NUM = robApp.Project.Structure.Objects.FreeNumber;

            //          RobotObjObject panel;


            //          //panel = (RobotObjObject)robApp.Project.Structure.Objects.Get(panel_NUM);
            //          panel = (RobotObjObject)robApp.Project.Structure.Objects.Get(brepno);

            //          panel.SetLabel(IRobotLabelType.I_LT_PANEL_CALC_MODEL, "Shell");
            //          panel.SetLabel(IRobotLabelType.I_LT_PANEL_THICKNESS, "TH12.00");
            //          panel.Main.Attribs.Meshed = 1;
            //          panel.Update();
            //          panel.Initialize();
            //          panel.Mesh.Generate();


            //          brepno++;
            //     }
            //     robApp.Project.Structure.Objects.EndMultiOperation();
            //     robApp.Project.ViewMngr.Refresh();
            ////For i As int32 = 0 To Brep_or_Curve.Count - 1
            ////  obj_type = Brep_or_Curve(i).ObjectType

            ////  //If obj_type = ObjectType.Brep Then
            ////  //  curr_brep = Brep_or_Curve(i)
            ////  //  crv_array = curr_brep.DuplicateEdgeCurves(False)
            ////  //End If

            ////  //If obj_type = ObjectType.Curve Then
            ////  //  curr_crv = Brep_or_Curve(i)
            ////  //  crv_array = curr_crv.DuplicateSegments
            ////  //End If



            ////  Dim kounta As int32: kounta = 1
            ////  For Each crv As curve In crv_array
            ////    panel_pnts.SetSize(kounta)
            ////    panel_pnts.Set(kounta, crv.PointAtStart.X, crv.PointAtStart.Y, crv.PointAtStart.Z)
            ////    kounta = kounta + 1
            ////  Next crv

            ////  robot.Project.Structure.Objects.BeginMultiOperation()
            ////  Dim panel As robotobjobject
            ////  Dim panel_num As int32: panel_num = robot.Project.Structure.Objects.FreeNumber + i
            ////  panel_nos.Add(panel_num)
            ////  robot.Project.Structure.Objects.CreateContour(panel_num, panel_pnts)
            ////  panel = robot.Project.Structure.Objects.Get(panel_num)
            ////  panel.SetLabel(IRobotLabelType.I_LT_PANEL_CALC_MODEL, Calculation_Model)
            ////  panel.Main.Attribs.Meshed = True

            ////  If Calculation_Model = "Shell" Then
            ////    panel.Mesh.Params.SurfaceParams.Generation.Type = IRobotMeshGenerationType.I_MGT_ELEMENT_SIZE
            ////    panel.Mesh.Params.SurfaceParams.Generation.ElementSize = FE_Element_Size
            ////    panel.Mesh.Params.SurfaceParams.Method.Method = IRobotMeshMethodType.I_MMT_DELAUNAY
            ////    panel.Mesh.Params.SurfaceParams.Delaunay.RegularMesh = False
            ////    If Not fe_emitter_size = 0 Then
            ////      panel.Mesh.Params.SurfaceParams.Delaunay.Type = IRobotMeshDelaunayType.I_MDT_DELAUNAY_AND_KANG
            ////      panel.Mesh.Params.SurfaceParams.Delaunay.EmittersSmoothing = True
            ////      panel.Mesh.Params.SurfaceParams.Delaunay.H0 = fe_emitter_size
            ////    End If
            ////  End If

            ////  If Panel_Thickness.Count >= Brep_or_Curve.Count Then
            ////    panel.SetLabel(IRobotLabelType.I_LT_PANEL_THICKNESS, Panel_Thickness(i))
            ////  Else
            ////    panel.setlabel(IRobotLabelType.I_LT_PANEL_THICKNESS, "TH30_CONCR")
            ////  End If

            ////  panel.initialize
            ////  panel_pnts = Nothing

            ////Next i
            ////robot.Project.Structure.Objects.EndMultiOperation()

            ////Panel_Numbers = panel_nos
            ////robot.Project.ViewMngr.Refresh


            //      // IRobotNodeSupportData fix;

            #endregion


            //Import Panels
            foreach (Grasshopper.Kernel.Types.GH_Brep i in panels)
            {
                int number_of_vertices = i.Value.Vertices.Count();
                double[] x = new double[number_of_vertices];
                double[] y = new double[number_of_vertices];
                double[] z = new double[number_of_vertices];
                string label = i.ToString();
                int p = 0;

                for (p = 0; p <= number_of_vertices - 1; p++)
                {
                    x[p] = i.Value.Vertices[p].Location.X;
                    y[p] = i.Value.Vertices[p].Location.Y;
                    z[p] = i.Value.Vertices[p].Location.Z;
                }

                ETABS.SapModel.AreaObj.AddByCoord(number_of_vertices, ref x, ref y, ref z, ref label, "Default", "", "Global");

            }

            //Merge Points
            int NumberPoints = 0;
            string[] PointName = new string[1];
            ETABS.SapModel.EditPoint.Merge(.01, ref NumberPoints, ref PointName);
            DA.SetData(0, ETABS);
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("{84CDAF8D-90AC-4850-ACF0-F752BF7A6A9E}"); }
        }
    }
	}
//    public class ETABSParameter
//    {
//        public ETABSParameter() 
//            : base(new GH_InstanceDescription("SGMesh", "SGM", "Maintains a collection SGMeshes", "SMART", "Params"))
//        {

//        }
//    }
//}
