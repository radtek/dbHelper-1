using Geoway.ADF.GIS.Utility;
using Geoway.ADF.MIS.Utility.DevExpressEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ApplyDemo
{
    class AppRoot
    {
        private static FormMain _frmMain = null;

        internal static bool InitLicense()
        {
            if (ArcGISLicenseManger.InitializeArcObjects() == false)
            {
                DevMessageUtil.ShowErrorMessage("需要的ArcGIS组件未安装，程序不能运行");
                return false;
            }
            return true;
        }

        public static Form InitFormMain()
        {
            _frmMain = new FormMain();
            return _frmMain;
        }
    }
}
