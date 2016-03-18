using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Grasshopper.Kernel;
using Grasshopper;
using Grasshopper.Getters;
using Grasshopper.Plugin;
using Grasshopper.GUI;
using RobotOM;
using Rhino.Geometry;

namespace SCORPIONETABS
{
    public class TestCustomUI : IGH_VariableParameterComponent
    {
        public bool CanInsertParameter(GH_ParameterSide side, int index)
        {
            throw new NotImplementedException();
        }

        public bool CanRemoveParameter(GH_ParameterSide side, int index)
        {
            throw new NotImplementedException();
        }

        public IGH_Param CreateParameter(GH_ParameterSide side, int index)
        {
            throw new NotImplementedException();
        }

        public bool DestroyParameter(GH_ParameterSide side, int index)
        {
            throw new NotImplementedException();
        }

        public void VariableParameterMaintenance()
        {
            throw new NotImplementedException();
        }
    }
}
