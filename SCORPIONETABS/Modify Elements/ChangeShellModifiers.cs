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
    public class ChangeShellModifiers : GH_Component
    {

        //set robot.dll file to false for embed interop
        private Bitmap _icon;

        public ChangeShellModifiers()
            : base("Modify Shells", "Modify Shells", "Modifies shells in the selected group to the new modifyer defined", "Test", "Modify")
        {
            _icon = Properties.Resources.ModifyPanels;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("ETABS Instance", "ETABS", "ETABS", GH_ParamAccess.item);
            pManager.AddTextParameter("Group Name", "Group", "Name of the group of elements that are to be modified", GH_ParamAccess.list);
            pManager.AddNumberParameter("Modifyer", "Modifyer", "Modifyer (ex 0.8 or 1.2)", GH_ParamAccess.list);
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
            List<string> groupNames = new List<string>();
            List<double> modifiers = new List<double>();
            if (!DA.GetData(0, ref ETABS)) { return; }
            if (!DA.GetDataList(1, groupNames)) { return; }
            if (!DA.GetDataList(2, modifiers)) { return; }

            if (groupNames.Count != modifiers.Count)
            {
                throw new Exception("Group and wall type lists must match in numbers");
            }
           
            Tools tools = new Tools();

            for (int i = 0; i < groupNames.Count; i++)
            {
                string[] shells = tools.GetGroupInformation(ETABS, groupNames[i]);
                for (int j = 0; j < shells.Count(); j++)
                {
                    tools.ModifyWall(ref ETABS, modifiers[i], shells[j]);
                }
            }

            DA.SetData(0, ETABS);
        }
        
        public override Guid ComponentGuid
        {
            get { return new Guid("bfb5ba79-93c4-4666-92e5-06ee91977b9d"); }
        }
    }
}
