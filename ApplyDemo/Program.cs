using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.UserSkins;
using DevExpress.Skins;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using System.Drawing;
using Geoway.ADF.MIS.Utility.Log;
using System.IO;
using DbtuneConfigHelper;
using PartitionUtility.DataModel;

namespace ApplyDemo
{
    static class Program
    {
        //系统主窗体
        private static Form _frmMain = null;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            BonusSkins.Register();
            AppearanceObject.DefaultFont = new Font("微软雅黑", 9);
            UserLookAndFeel.Default.SetSkinStyle("Office 2013");
            SkinManager.EnableFormSkins();

            Application.ApplicationExit += new EventHandler(ApplicationExit);

            StartApp();
        }

        private static void ApplicationExit(object sender, EventArgs e)
        {
            if (File.Exists(string.Format("{0}\\{1}.xml", Environment.CurrentDirectory, UtilityConst.XML_NAME)))
            {
                File.Delete(string.Format("{0}\\{1}.xml", Environment.CurrentDirectory, UtilityConst.XML_NAME));
            }
        }

        private static void StartApp()
        {
            try
            {
                if (!AppRoot.InitLicense())
                {
                    return;
                }

                //创建主窗体
                _frmMain = AppRoot.InitFormMain();

                Application.Run(_frmMain);
            }
            catch (Exception ex)
            {
                LogHelper.Error.Append(ex);
                return;
            }
            finally
            {
                Application.Exit();
            }
        }
    }
}
