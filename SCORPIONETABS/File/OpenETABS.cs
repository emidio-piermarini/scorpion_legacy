using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Reflection;
using System.Runtime.Remoting.Lifetime;
using Grasshopper.Kernel;
using Grasshopper;
using Grasshopper.Getters;
using Grasshopper.Plugin;
using Grasshopper.GUI;
using RobotOM;
using Rhino.Geometry;
using Microsoft.VisualBasic;


namespace SCORPIONETABS
{
    public class OpenETABS : GH_Component
    {
        private Bitmap _icon;
        public OpenETABS() : base("Open Etabs", "Open", "Opens an instance of Etabs", "Test", "File") 
        {
            _icon = Properties.Resources.Folder;
        }

        protected override System.Drawing.Bitmap Icon
        {
            get { return _icon; }
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("New file", "New/Open", "If true, opens a new file, if false opens file specified in the second input", GH_ParamAccess.item);
            pManager.AddTextParameter("File path and name", "Open", "Specifies what file to open", GH_ParamAccess.item);
            pManager[1].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("ETABS Instance", "ETABS", "ETABS", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool newDocument = new bool();
            string docName = "empty";

            if (!DA.GetData(0, ref newDocument)) { return; }
            if (!DA.GetData(1, ref docName) && newDocument == false) { return; }

            //System.Runtime.Remoting.Lifetime.LifetimeServices.LeaseTime = System.TimeSpan.FromDays(1);
            //System.Runtime.Remoting.Lifetime.LifetimeServices.RenewOnCallTime = System.TimeSpan.FromDays(1);
            
            //Create ETABS Object
            string pathtoetabs = @"C:\Program Files\Computers and Structures\ETABS 2013\ETABS.exe";
            Assembly ETABSassembly = Assembly.LoadFrom(pathtoetabs);
            //ETABS2013.cOAPI test = (ETABS2013.cOAPI)ETABSassembly.GetModule("CSI.ETABS.API.ETABSObject");
            
            
            ETABS2013.cOAPI ETABS = (ETABS2013.cOAPI)ETABSassembly.CreateInstance("CSI.ETABS.API.ETABSObject"); //if developer settings checkobx is left checked it stops here (1. Solution exception:Unable to cast object of type 'CSI.ETABS.API.ETABSObject' to type 'ETABS2013.cOAPI'.)
            //Interaction.GetObject(
            //ETABS2013.cOAPI ETABS2 = (ETABS2013.cOAPI)Microsoft.VisualBasic.Interaction.GetObject(@"H:\Entisar Iterations\140409_iteration_model.EDB");
            
            
            //Type anExtractedType = ETABSassembly.GetType("CSI.ETABS.API.ETABSObject");
            //object testis = (ETABS2013.cOAPI)Activator.CreateInstance(anExtractedType);
            
            //object testis = (ETABS2013.cOAPI)Activator.GetObject(anExtractedType, "C:/Program Files/Computers and Structures/ETABS 2013/ETABS.exe");
            
            //ETABS2013.cOAPI newETABS = (ETABS2013.cOAPI)testis;
            //newETABS.SapModel.InitializeNewModel();
            
            //Start ETABS and Create New Model
            ETABS.ApplicationStart();

            if (newDocument == true)
	        {
                ETABS.SapModel.InitializeNewModel(ETABS2013.eUnits.kN_m_C);
               ETABS.SapModel.File.NewBlank();
             //   ETABS.SapModel.File.NewGridOnly(4,3,3,1,1,24,24);
	        }
            else
	        {
                ETABS.SapModel.File.OpenFile(docName);
	        }           

            DA.SetData(0, ETABS);
            //GC.KeepAlive(ETABS);
        }
        
        public override Guid ComponentGuid
        {
            get { return new Guid("{f6946d54-d4be-46ed-9fc8-98e4664df315}"); }
        }
    }

}
