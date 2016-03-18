using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper;
using Grasshopper.Getters;
using Grasshopper.Plugin;
using Grasshopper.GUI;
using RobotOM;
using Rhino.Geometry;

namespace SCORPIONETABS
{
    public class AnalysisResultsShells : GH_Component, IGH_VariableParameterComponent
    {
        private Bitmap _icon;

        //decides if we show output params or not
        private bool _showsSVMtop;
        private bool _showsSVMbot;
        private bool _shows11top;
        private bool _shows12top;
        private bool _shows22top;
        private bool _shows11bot;
        private bool _shows12bot;
        private bool _shows22bot;

        

        public AnalysisResultsShells() : base("Analysis results shells", "Shell Results", "Extracts all the shells from the ETABS model and gets the results", "Test", "Analysis") 
        {
            _icon = Properties.Resources.TablePanels;

            _showsSVMtop = true;
            _showsSVMbot = true;
            _shows11top = false;
            _shows12top = false;
            _shows22top = false;
            _shows11bot = false;
            _shows12bot = false;
            _shows22bot = false;
        }
        public override void AppendAdditionalMenuItems(ToolStripDropDown menu)
        {
            base.AppendAdditionalMenuItems(menu);
            
            Menu_AppendItem(menu, "SVM Top", SVMtopclicked, true, _showsSVMtop);
            Menu_AppendItem(menu, "SVM Bottom", SVMbottomclicked, true, _showsSVMbot);

            Menu_AppendSeparator(menu);

            Menu_AppendItem(menu, "S11 Top", s11topclicked, true, _shows11top);
            Menu_AppendItem(menu, "S12 Top", s12topclicked, true, _shows12top);
            Menu_AppendItem(menu, "S22 Top", s22topclicked, true, _shows22top);

            Menu_AppendItem(menu, "S11 Bottom", s11botclicked, true, _shows11bot);
            Menu_AppendItem(menu, "S12 Bottom", s12botclicked, true, _shows12bot);
            Menu_AppendItem(menu, "S22 Bottom", s22botclicked, true, _shows22bot);
        }

        private void AddOutput(string name, string nickname, string explanation)
        {
            Grasshopper.Kernel.Parameters.Param_Number param = new Grasshopper.Kernel.Parameters.Param_Number();
            param.NickName = nickname;
            param.Name = name;
            param.Description = explanation;
            param.Access = GH_ParamAccess.list;
            Params.RegisterOutputParam(param);

            Params.OnParametersChanged();
            ExpireSolution(true);
        }

        private void RemoveOutput(string name)
        {
            for (int i = 0; i < Params.Output.Count; i++)
            {
                if (Params.Output[i].Name == name)
	            {
                    Params.UnregisterOutputParameter(Params.Output[i]);
	            }
            }
            Params.OnParametersChanged();
            ExpireSolution(true);
        }

        private void SVMtopclicked(Object sender, EventArgs e)
        {
            _showsSVMtop = !_showsSVMtop;
            if (_showsSVMtop)
            {
                AddOutput("svmtop", "SVM Top", "explanation");
            }
            else
            {
                RemoveOutput("svmtop");
            }
            ExpireSolution(true);
        }

        private void SVMbottomclicked(Object sender, EventArgs e)
        {
            _showsSVMbot = !_showsSVMbot;
            if (_showsSVMbot)
            {
                AddOutput("svmbot", "SVM Bottom", "explanation");
            }
            else
            {
                RemoveOutput("svmbot");
            }
            ExpireSolution(true);
        }

        private void s11topclicked(Object sender, EventArgs e)
        {
            _shows11top = !_shows11top;
            if (_shows11top)
            {
                AddOutput("s11top", "S11 Top", "explanation");
            }
            else
            {
                RemoveOutput("s11top");
            }
            ExpireSolution(true);
        }

        private void s12topclicked(Object sender, EventArgs e)
        {
            _shows12top = !_shows12top;
            if (_shows12top)
            {
                AddOutput("s12top", "S12 Top", "explanation");
            }
            else
            {
                RemoveOutput("s12top");
            }
            ExpireSolution(true);
        }

        private void s22topclicked(Object sender, EventArgs e)
        {
            _shows22top = !_shows22top;
            if (_shows22top)
            {
                AddOutput("s22top", "S22 Top", "explanation");
            }
            else
            {
                RemoveOutput("s22top");
            }
            ExpireSolution(true);
        }

        private void s11botclicked(Object sender, EventArgs e)
        {
            _shows11bot = !_shows11bot;
            if (_shows11bot)
            {
                AddOutput("s11bot", "S11 Bottom", "explanation");
            }
            else
            {
                RemoveOutput("s11bot");
            }
            ExpireSolution(true);
        }

        private void s12botclicked(Object sender, EventArgs e)
        {
            _shows12bot = !_shows12bot;
            if (_shows12bot)
            {
                AddOutput("s12bot", "S12 Bottom", "explanation");
            }
            else
            {
                RemoveOutput("s12bot");
            }
            ExpireSolution(true);
        }

        private void s22botclicked(Object sender, EventArgs e)
        {
            _shows22bot = !_shows22bot;
            if (_shows22bot)
            {
                AddOutput("s22bot", "S22 Bottom", "explanation");
            }
            else
            {
                RemoveOutput("s22bot");
            }
            ExpireSolution(true);
        }

        

        protected override System.Drawing.Bitmap Icon
        {
            get { return _icon; }
        }
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("ETABS Instance", "ETABS", "ETABS", GH_ParamAccess.item);
            pManager.AddTextParameter("Loadcase/Combo", "Loadcase", "Loadcase or load combo input as a string", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Shell Identities", "ID:s", "Shell Identities from ETABS", GH_ParamAccess.list);
            pManager.AddGenericParameter("svmtop", "SVM Top", "TODO: find out what this is exactly", GH_ParamAccess.tree);
            pManager.AddGenericParameter("svmbot", "SVM Bottom", "TODO: find out what this is exactly", GH_ParamAccess.tree);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {

            ETABS2013.cOAPI ETABS = null;
            string loadcase = null;
            if (!DA.GetData(0, ref ETABS)) { return; }
            if (!DA.GetData(1, ref loadcase)) { return; }

            //Gets the ETABS geometry
            int numberNames = 0;
            string[] shellList = null;
            ETABS.SapModel.AreaObj.GetNameList(ref numberNames, ref shellList);

            List<int> IDs = new List<int>();
            List<double> svmtopList = new List<double>();
            DataTree<double> svmtopTree = new DataTree<double>();
            List<double> svmbotList = new List<double>();

            List<double> s11topList = new List<double>();
            List<double> s22topList = new List<double>();
            List<double> s12topList = new List<double>();

            List<double> s11botList = new List<double>();
            List<double> s12botList = new List<double>();
            List<double> s22botList = new List<double>();
            int ret;
            ret = ETABS.SapModel.Results.Setup.SetCaseSelectedForOutput(loadcase);

            for (int i = 0; i < shellList.Count(); i++)
            {

                //Define all output arrays
                ETABS2013.eItemTypeElm ItemTypeElm = new ETABS2013.eItemTypeElm();
                int NumberResults = 20;
                string[] obj = new string[100];
                string[] elm = new string[100];
                string[] PointElm = new string[100];
                string[] LoadCase = new string[100];
                string[] StepType = new string[100];
                double[] StepNum = new double[100];
                double[] s11top = new double[100];
                double[] s22top = new double[100];
                double[] s12top = new double[100];
                double[] smaxtop = new double[100];
                double[] smintop = new double[100];
                double[] sangletop = new double[100];
                double[] svmtop = new double[100];
                double[] s11bot = new double[100];
                double[] s22bot = new double[100];
                double[] s12bot = new double[100];
                double[] smaxbot = new double[100];
                double[] sminbot = new double[100];
                double[] sanglebot = new double[100];
                double[] svmbot = new double[100];
                double[] s13avg = new double[100];
                double[] s23avg = new double[100];
                double[] smaxavg = new double[100];
                double[] sangleavg = new double[100];
                double[] U1 = new double[100];
                double[] U2 = new double[100];
                double[] U3 = new double[100];
                double[] R1 = new double[100];
                double[] R2 = new double[100];
                double[] R3 = new double[100];
                //Gets the analysis results
                ret = ETABS.SapModel.Results.AreaStressShell(shellList[i], ItemTypeElm, ref NumberResults, ref obj, ref elm, ref PointElm, ref LoadCase, ref StepType, ref StepNum, ref s11top,
                ref s22top, ref s12top, ref smaxtop, ref smintop, ref sangletop, ref svmtop, ref s11bot, ref s22bot, ref s12bot, ref smaxbot,
                ref sminbot, ref sanglebot, ref svmbot, ref s13avg, ref s23avg, ref smaxavg, ref sangleavg);


                for (int j = 0; j < svmtop.Count(); j++)
                {
                    int[] pathInts = new int[2];
                    pathInts[0] = i;
                    pathInts[1] = j;
                    GH_Path path = new GH_Path(pathInts);
                    svmtopTree.Add(svmtop[j], path);
                }
                int ID = Convert.ToInt32(shellList[i]);
                IDs.Add(ID);
                svmtopList.Add(svmtop.Max());
                svmbotList.Add(svmbot.Max());
                s11topList.Add(s11top.Max());
                s22topList.Add(s22top.Max());
                s12topList.Add(s12top.Max());
            }

            //TODO: Find out which ones of these are of interest
            DA.SetDataList("Shell Identities", IDs);
            if (_showsSVMtop)
            {
                DA.SetDataList("svmtop", svmtopList);
            }
            if (_showsSVMbot)
            {
                DA.SetDataList("svmbot", svmbotList);
            }
            if (_shows11top)
            {
                DA.SetDataList("s11top", s11topList);
            }
            if (_shows12top)
            {
                DA.SetDataList("s12top", s12topList);
            }
            if (_shows22top)
            {
                DA.SetDataList("s22top", s22topList); 
            }
                       
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("372bca20-6ad2-4fab-8d42-f3de34ef76ee"); }
        }


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

