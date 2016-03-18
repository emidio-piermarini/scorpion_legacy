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
    public class SaveETABS : GH_Component
    {

        //set robot.dll file to false for embed interop
        private Bitmap _icon;
        public SaveETABS() : base("Save Etabs", "Save", "Saves an instance of Etabs", "Test", "File") 
        {
            _icon = Properties.Resources.Save;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("ETABS Instance", "ETABS", "ETABS", GH_ParamAccess.item);
            pManager.AddTextParameter("File path and name", "Save as", "Specifies what file to save", GH_ParamAccess.item);
        }
        protected override System.Drawing.Bitmap Icon
        {
            get { return _icon; }
        }
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {

        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string docName = "empty";
            ETABS2013.cOAPI ETABS = null;

            if (!DA.GetData(1, ref docName)) { return; }
            if (!DA.GetData(2, ref ETABS)) { return; }

            ETABS.SapModel.File.Save(docName);
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("{129b31d9-85e9-472a-98da-27663560a66d}"); }
        }
    }
}