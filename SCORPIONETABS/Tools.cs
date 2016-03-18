using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using ETABS2013;

namespace SCORPIONETABS
{
    public class Tools
    {

        //Generates a new GUID
        public string GenerateGUID()
        {
            string sGUID = null;
            sGUID = System.Guid.NewGuid().ToString();
            return sGUID;
        }


        //Gets all groups in model (must be done manually for now)
        public object GetGroups(ref ETABS2013.cSapModel sapModel)
        {
            //Not yet implemented
            int ret = 0;
            int NumberNames = 0;
            string[] GroupNames = new string[99];
            ret = sapModel.GroupDef.GetNameList(ref NumberNames, ref GroupNames);
            return GroupNames;

        }

        
        //Extracts information, and returns
        public string[] GetGroupInformation(ETABS2013.cOAPI ETABS, string groupName)
	    {

		    int ret = 0;
		    int NumberItems = 1;
		    int[] ObjectType = new int[99];
		    string[] ObjectName = new string[99];

		    //Gets group by name
		    ret = ETABS.SapModel.GroupDef.GetAssignments(groupName, ref NumberItems, ref ObjectType, ref ObjectName);
		    return ObjectName;

	    }


        //Extracts analysis results for object
        public object AnalysisResults(ref ETABS2013.cSapModel sapModel, ref string objectName)
        {

            int ret = 0;

            //Define all output arrays
            ETABS2013.eItemTypeElm ItemTypeElm = new ETABS2013.eItemTypeElm();
            int NumberResults = 0;
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
            ret = sapModel.Results.AreaStressShell(objectName, ItemTypeElm, ref NumberResults, ref obj, ref elm, ref PointElm, ref LoadCase, ref StepType, ref StepNum, ref s11top,
            ref s22top, ref s12top, ref smaxtop, ref smintop, ref sangletop, ref svmtop, ref s11bot, ref s22bot, ref s12bot, ref smaxbot,
            ref sminbot, ref sanglebot, ref svmbot, ref s13avg, ref s23avg, ref smaxavg, ref sangleavg);

            //What are we interested in here? Any calculations? (s22top is the stress in global z-direction, positive values indicate tension)
            return s22top;

        }


        //Sets up a wall section type for each shell, named by the shell

        public void SetupWalls(ref ETABS2013.cOAPI ETABS, string objectName)
        {
            int ret = 0;

            ETABS2013.eWallPropType wallType = default(ETABS2013.eWallPropType);
            ETABS2013.eShellType shellType = default(ETABS2013.eShellType);
            string matProp = null;
            double thickness = 0;
            int color = 0;
            string notes = null;
            string guid = null;

            string propName = null;

            ret = ETABS.SapModel.AreaObj.GetProperty(objectName, ref propName);
            ret = ETABS.SapModel.PropArea.GetWall(propName, ref wallType, ref shellType, ref matProp, ref thickness, ref color, ref notes, ref guid);
            string newGuid = GenerateGUID();
            ret = ETABS.SapModel.PropArea.SetWall(objectName, wallType, shellType, matProp, thickness, color, notes, newGuid);
            ret = ETABS.SapModel.AreaObj.SetProperty(objectName, objectName);

        }


        //Modify wall (actually creates a new wall type and assigns to object)

        public void ModifyWall(ref ETABS2013.cOAPI ETABS, double modVal, string objectName)
        {
            

            int ret = 0;

            //ETABS2013.eWallPropType wallType = default(ETABS2013.eWallPropType);
            //ETABS2013.eShellType shellType = default(ETABS2013.eShellType);
            //string matProp = null;
            //double thickness = 0;
            //int color = 0;
            //string notes = null;
            //string guid = null;
            double[] baseMod = new double[] { 99 };

            string propName = "0";

            ret = ETABS.SapModel.AreaObj.GetProperty(objectName, ref propName);
            //ret = ETABS.SapModel.PropArea.GetWall(objectName, ref wallType, ref shellType, ref matProp, ref thickness, ref color, ref notes, ref guid);

            //Input wall from ETABS to modify
            ret = ETABS.SapModel.PropArea.GetModifiers(propName, ref baseMod);
            

            //Sets new modifier with changed values
            double[] modifier = new double[10] {
			    modVal,
			    modVal,
			    modVal,
			    baseMod[3],
			    baseMod[4],
			    baseMod[5],
			    baseMod[6],
			    baseMod[7],
			    baseMod[8],
			    baseMod[9]
		    };

            ret = ETABS.SapModel.PropArea.SetModifiers(propName, ref modifier);


            //ret = SapModel.PropArea.SetWall(objectName, wallType, shellType, matProp, thickness, color, notes, newGuid)
            //ret = SapModel.AreaObj.SetProperty(objectName, objectName)

        }

    }
}