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
    public class UnlockModel : GH_Component
    {
        private Bitmap _icon;
        public UnlockModel()
            : base("Unlock model", "Unlock", "Unlocks ETABS model", "Test", "Analysis")
        {
            _icon = Properties.Resources.Unlock;
        }
        protected override System.Drawing.Bitmap Icon
        {
            get { return _icon; }
        }
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("ETABS Instance", "ETABS", "ETABS", GH_ParamAccess.item);
            pManager.AddNumberParameter("Refresh", "Refresh", "When input is change component is triggered again", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("ETABS Instance", "ETABS", "ETABS", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            ETABS2013.cOAPI ETABS = null;
            if (!DA.GetData(0, ref ETABS)) { return; }
            ETABS2013.cSapModel sapModel = ETABS.SapModel;
            ETABS.SapModel.SetModelIsLocked(false);
            DA.SetData(0, ETABS);
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("85f7085f-8ef5-4ffb-852e-fca6f7dce72d"); }
        }
    }
}
