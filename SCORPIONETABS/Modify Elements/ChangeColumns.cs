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
    public class ChangeColumns : GH_Component
    {

        //set robot.dll file to false for embed interop
        private Bitmap _icon;

        public ChangeColumns()
            : base("Modify Shells", "Modify Shells", "Modifies shells in the selected group to the new wall section defined", "Test", "Modify")
        {
            _icon = Properties.Resources.ModifyColumns;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("ETABS Instance", "ETABS", "ETABS", GH_ParamAccess.item);
            pManager.AddTextParameter("Group Name", "Group", "Name of the group of elements that are to be modified", GH_ParamAccess.list);
            pManager.AddTextParameter("New Section", "New Section", "Name of the new column type to be applied", GH_ParamAccess.item);
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
            string newColumnName = "";
            if (!DA.GetData(0, ref ETABS)) { return; }
            if (!DA.GetDataList(1, groupNames)) { return; }
            if (!DA.GetData(2, ref newColumnName)) { return; }

            Tools tools = new Tools();

            for (int i = 0; i < groupNames.Count; i++)
            {
                string[] shells = tools.GetGroupInformation(ETABS, groupNames[i]);
                for (int j = 0; j < shells.Count(); j++)
                {
                    ETABS.SapModel.FrameObj.SetSection(shells[j], newColumnName);
                }
            }

            DA.SetData(0, ETABS);
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("ff4bd054-ff91-4e5a-974f-1f1f0df77582"); }
        }
    }
}
