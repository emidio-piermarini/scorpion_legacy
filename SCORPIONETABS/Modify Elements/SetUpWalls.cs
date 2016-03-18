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
    public class SetUpWalls : GH_Component
    {

        //set robot.dll file to false for embed interop
        private Bitmap _icon;

        public SetUpWalls()
            : base("Set up walls", "Set up walls", "Creates a unique section for each wall, named by it's unique name", "Test", "Modify")
        {
            _icon = Properties.Resources.Modify;
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
            pManager.AddGenericParameter("ETABS Instance", "ETABS", "ETABS", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            ETABS2013.cOAPI ETABS = null;
            if (!DA.GetData(0, ref ETABS)) { return; }

            Tools tools = new Tools();

            int numberNames = 0;
            string[] myName = new string[0];
            ETABS.SapModel.AreaObj.GetNameList(ref numberNames, ref myName);

            for (int i = 0; i < myName.Count(); i++)
            {
                tools.SetupWalls(ref ETABS, myName[i]);
            }
            DA.SetData(0, ETABS);
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("206de4ef-d488-4e38-8127-d8d69ee80546"); }
        }
    }
}

