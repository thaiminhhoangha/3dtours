/* 3DTours main program by Thai Minh Hoang Ha
 * This program uses the following engines for graphics and audio:
 * Irrlicht engine - website: http://irrlicht.sourceforge.net
 * IrrKlang engine - website: http://www.ambiera.com/irrklang
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Irrlicht;
using Irrlicht.Video;
using Irrlicht.GUI;
using Irrlicht.Core;
using Irrlicht.Scene;
using IrrKlang;


namespace _DTour
{
    class Program
    {

        static void Main(string[] args)
        {
            frmSplash splash = new frmSplash();
            splash.Show();
            splash.Focus();
            System.Threading.Thread.Sleep(5000);
            splash.Dispose();

            Render Tour3DGUI = new Render();
            Tour3DGUI.displayMenu(DriverType.OPENGL, 800, 600, 32, false, false);


            /*
            switch (Tour3DGUI.ClickedButton)
            {
                case MyMenuControls.START_AUTO_MODE:
                    //device.GUIEnvironment.AddMessageBox("debug", "test", false, MessageBoxFlag.OK, null, -1);
                    //if (Tour3DGUI.mapFilename == null)
                    //    Tour3DGUI.device.GUIEnvironment.AddMessageBox("Error", "Please select 3D scene from list", true, MessageBoxFlag.OK, null, -1);
                    break;
                case MyMenuControls.START_WALKTHROUGH_MODE:
                    if (Tour3DGUI.mapFilename == null || Tour3DGUI.mapFilename == "")
                        Tour3DGUI.device.GUIEnvironment.AddMessageBox("Error", "Please select 3D scene from list", true, MessageBoxFlag.OK, null, -1);
                    else
                    {
                        Tour3DGUI.runWalkthrough();
                    }
                    //device.GUIEnvironment.AddMessageBox("debug", "test", false, MessageBoxFlag.OK, null, -1);
                    break;
                case MyMenuControls.EXIT_BUTTON:
                    Environment.Exit(0);
                    //device.GUIEnvironment.AddMessageBox("debug", "test", false, MessageBoxFlag.OK, null, -1);
                    break;
            }
            */
        }

    }
}
