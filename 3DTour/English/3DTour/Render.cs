/* 3DTours main program by Thai Minh Hoang Ha
 * This program uses the following engines for graphics and audio:
 * Irrlicht engine - website: http://irrlicht.sourceforge.net
 * IrrKlang engine - website: http://www.ambiera.com/irrklang
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Timers;
using Irrlicht;
using Irrlicht.Video;
using Irrlicht.GUI;
using Irrlicht.Core;
using Irrlicht.Scene;
using IrrKlang;

enum MyMenuControls
{
    NONE = 0,
    LIST_RENDERERS,
    LIST_MAPS,
    START_AUTO_MODE,
    START_WALKTHROUGH_MODE,
    EXIT_BUTTON,
    LIST_RESOLUTIONS,
    LIST_BITS
};


namespace _DTour
{
    class Render : IEventReceiver
    {
        public DriverType SelectedDriverType = DriverType.DIRECT3D9;
        public MyMenuControls ClickedButton = MyMenuControls.NONE;
        public IrrlichtDevice device;
        public string mapFilename;
        public string strMapDescription;
        public IGUIStaticText mapDescription;
        public ISoundEngine soundengine = new ISoundEngine();
        public int scrnWidth = 640;
        public int scrnHeight = 480;
        public int scrnBitDepth = 32;
        public bool playRandomBGM = false;
        private bool camCollision = true;
        private bool camCollisionAdded = false;
        private IGUIStaticText labelcampos = null;
        private IGUIStaticText labelstatus = null;
        private ITexture pichelp = null;
        private IGUIElement help = null;

        private bool issoundplaying = true;
        private bool ismenudisplaying = true;
        private bool ishelpdisplaying = false;
        


        //Begin of Event handler
        //==========================
        public bool OnEvent(Event e)
        {
            if (e.Type == EventType.GUIEvent)
            {
                // an UI event

                if (e.GUIEventType == GUIEvent.LISTBOX_CHANGED ||
                     e.GUIEventType == GUIEvent.LISTBOX_SELECTED_AGAIN)
                {
                    switch (e.GUIEventCaller.ID)
                    {
                        case (int)MyMenuControls.LIST_RENDERERS:
                            int selected = ((IGUIListBox)e.GUIEventCaller).Selected;
                            SelectedDriverType = (DriverType)(selected + 1);
                            break;
                        case (int)MyMenuControls.LIST_MAPS:
                            mapFilename = ((IGUIListBox)e.GUIEventCaller).GetListItem(((IGUIListBox)e.GUIEventCaller).Selected);
                            //device.GUIEnvironment.AddMessageBox("debug", mapFilename, false, MessageBoxFlag.OK, null, -1);
                            //get a map description
                            if (File.Exists(mapFilename + ".txt"))
                            {
                                TextReader reader = new StreamReader(mapFilename + ".txt");
                                strMapDescription = reader.ReadToEnd();
                                mapDescription.Text = strMapDescription;
                                reader.Close();
                            }
                            else
                                mapDescription.Text = "Description for this scene was not found";

                            break;
                        case (int)MyMenuControls.LIST_RESOLUTIONS:
                            //device.GUIEnvironment.AddMessageBox("debug", ((IGUIListBox)e.GUIEventCaller).Selected.ToString(), false, MessageBoxFlag.OK, null, -1);
                            switch (((IGUIListBox)e.GUIEventCaller).Selected)
                            {
                                case 0:
                                    scrnWidth = 640;
                                    scrnHeight = 480;
                                    break;
                                case 1:
                                    scrnWidth = 800;
                                    scrnHeight = 600;
                                    break;
                                case 2:
                                    scrnWidth = 1024;
                                    scrnHeight = 768;
                                    break;
                            }
                            break;
                        case (int)MyMenuControls.LIST_BITS:
                            switch (((IGUIListBox)e.GUIEventCaller).Selected)
                            {
                                case 0:
                                    scrnBitDepth = 16;
                                    break;
                                case 1:
                                    scrnBitDepth = 32;
                                    break;

                            }
                            break;

                    }
                }
                else
                    if (e.GUIEventType == GUIEvent.BUTTON_CLICKED)
                    {
                        //device.GUIEnvironment.AddMessageBox("Debug", e.GUIEventCaller.ID.ToString(), false, MessageBoxFlag.OK, null, -1);
                        ClickedButton = (MyMenuControls)e.GUIEventCaller.ID;
                        //play a sound effect
                        soundengine.Play2D(@"media\sounds\CLICK18B.WAV");
                        /*
                        if (mapFilename == null)
                            device.GUIEnvironment.AddMessageBox("Error", "Please select 3D scene from list", true, MessageBoxFlag.OK, null, -1);
                        else
                            runWalkthrough();

                        if (ClickedButton == MyMenuControls.EXIT_BUTTON)
                            Environment.Exit(0);
                        */
                        
                        switch (ClickedButton)
                        {
                            case MyMenuControls.START_AUTO_MODE:
                                //device.GUIEnvironment.AddMessageBox("debug", "test", false, MessageBoxFlag.OK, null, -1);
                                if (mapFilename == null)
                                    device.GUIEnvironment.AddMessageBox("Error", "Please select 3D scene from list", true, MessageBoxFlag.OK, null, -1);
                                else
                                    runGuidedmode();
                                break;
                            case MyMenuControls.START_WALKTHROUGH_MODE:
                                if (mapFilename == null)
                                    device.GUIEnvironment.AddMessageBox("Error", "Please select 3D scene from list", true, MessageBoxFlag.OK, null, -1);
                                else
                                {
                                    runWalkthrough();
                                }
                                //device.GUIEnvironment.AddMessageBox("debug", "test", false, MessageBoxFlag.OK, null, -1);
                                //runWalkthrough(SelectedDriverType, scrnWidth, scrnHeight, 32, true, false);
                                break;
                            case MyMenuControls.EXIT_BUTTON:
                                Environment.Exit(0);
                                //device.GUIEnvironment.AddMessageBox("debug", "test", false, MessageBoxFlag.OK, null, -1);
                                break;
                        }
                        
                    }
            }
            else
                if (e.Type == EventType.KeyInput)
                {
                    //a key has been pressed

                    if (!e.KeyPressedDown && e.Key == KeyCode.KEY_ESCAPE)
                    {
                        device.CloseDevice();
                        //displayMenu(SelectedDriverType,800,600,32,false,false);
                        Environment.Exit(0);
                        return true;
                    }

                    //F1 to display help
                    if (!e.KeyPressedDown && e.Key == KeyCode.KEY_F1 && ismenudisplaying==false)
                    {
                        /* OLD CODE
                        if (File.Exists("guide.txt"))
                        {
                            TextReader reader = new StreamReader("guide.txt");
                            string message = reader.ReadToEnd();
                            device.GUIEnvironment.AddMessageBox("Quick guide",
                                message,
                                true,
                                MessageBoxFlag.OK, null, -1);
                        }
                        */ 
                        //NEW CODE
                        if (ismenudisplaying == false)
                        {
                            if (ishelpdisplaying == false)
                            {
                                pichelp = device.VideoDriver.GetTexture(@"media\help.3dt");
                                help = device.GUIEnvironment.AddImage(pichelp, new Position2D(scrnWidth/2-175,scrnHeight/2-225), true, null, -1, "");
                                ishelpdisplaying = true;
                            }
                            else
                            {
                                help.Remove();
                                device.VideoDriver.RemoveTexture(pichelp);
                                ishelpdisplaying = false;
                            }
                        }

                        return true;
                    }

                    //F2 to disable camera collision
                    if (!e.KeyPressedDown && e.Key == KeyCode.KEY_F2 && ismenudisplaying == false)
                    {
                        camCollision = !camCollision;
                        if (camCollision)
                            displayStatus("Walking mode");
                        else
                            displayStatus("Flying mode");
                        return true;
                    }

                    //F3 to display current camera position
                    if (!e.KeyPressedDown && e.Key == KeyCode.KEY_F3 && ismenudisplaying == false)
                    {
                        labelcampos.Visible = !labelcampos.Visible;
                        return true;
                    }

                    //S to toogle sound on/off
                    if (!e.KeyPressedDown && e.Key == KeyCode.KEY_KEY_S && ismenudisplaying == false)
                    {
                        issoundplaying = !issoundplaying;
                        if (issoundplaying)
                        {
                            //soundengine.StopAllSounds();
                            soundengine.SoundVolume = 0;
                            displayStatus("Sound off");
                        }
                        else
                        {
                            //playBGM();
                            soundengine.SoundVolume = 1.0f;
                            displayStatus("Sound on");
                        }

                        return true;
                    }

                    //increase/decrease camera gravity
                    //CAN NOT REALIZE YET




                }

            return false;
        }
        //END EVENT HANDLER


        //DISPLAY MAIN MENU METHOD
        //========================
        public void displayMenu(DriverType driverType, int screenWidth, int screenHeight, int bits, bool fullScreen, bool vSync)
        {
            ismenudisplaying = true;

            //create device
            device = new IrrlichtDevice(driverType, new Dimension2D(screenWidth, screenHeight), bits, fullScreen, true, vSync);
            //if device creation failed
            if (device == null)
            {
                Console.Out.WriteLine("Failed to create new device.");
                return;
            }
            //set event handler
            device.EventReceiver = this;
            device.VideoDriver.SetTextureCreationFlag(Irrlicht.Video.TextureCreationFlag.OPTIMIZED_FOR_QUALITY, true);
            //device.VideoDriver.SetFog(new Color(150, 200, 200, 200), true, 100, 110, 0.1f, true, true);

            device.WindowCaption = "3DTours main menu";

            //create sound engine
            soundengine.Play2D(@"media\sounds\bgm\bgm-01.mp3", true);

            //create GUI
            //font
            IGUIFont font = device.GUIEnvironment.GetFont(@"media\fonts\mssansserif.xml");
            if (font != null)
                device.GUIEnvironment.Skin.Font = font;

            //Rect position = new Rect(screenWidth/4, screenHeight/10, screenWidth-(screenWidth/4), (screenHeight/10)+(screenHeight/8) );
            Rect position = new Rect(screenWidth - (screenWidth/3), screenHeight / 5, screenWidth - (screenWidth / 16), (screenHeight / 5) + 10);

            //add background image
            device.GUIEnvironment.AddImage(device.VideoDriver.GetTexture("media/bg.3dt"), new Position2D(0, 0), true, null, -1, "");

            //set alpha channel for skin
            for (int i = 0; i < (int)SkinColor.COUNT; ++i)
            {
                Color cl = device.GUIEnvironment.Skin.GetColor((SkinColor)i);
                cl.Alpha = 150;
                device.GUIEnvironment.Skin.SetColor((SkinColor)i, cl);
            }
            Color cl2 = new Color(150,255,144,0);

            //renderer label
            IGUIStaticText labelRenderer = device.GUIEnvironment.AddStaticText("Select render method:", position, false, false, null, -1);
            labelRenderer.OverrideColor = new Color(100, 255, 255, 255);

            //display a list of available renderers
            position.UpperLeftCorner.Y += labelRenderer.AbsolutePosition.Height;
            position.LowerRightCorner.Y += screenHeight / 8;
            IGUIListBox listRenderers = device.GUIEnvironment.AddListBox(position, null, (int)MyMenuControls.LIST_RENDERERS, true);

            listRenderers.AddItem("Irrlicht Software Renderer");
            listRenderers.AddItem("Apfelbaum Software Renderer");
            listRenderers.AddItem("Direct3D 8.1");
            listRenderers.AddItem("Direct3D 9.0c");
            listRenderers.AddItem("OpenGL 1.5");
            listRenderers.Selected = ((int)SelectedDriverType) - 1;

            //resolutions
            position.UpperLeftCorner.Y += listRenderers.AbsolutePosition.Height+1;
            position.LowerRightCorner.Y += screenHeight / 10;
            IGUIListBox listResolutions = device.GUIEnvironment.AddListBox(position, null, (int)MyMenuControls.LIST_RESOLUTIONS, true);
            listResolutions.AddItem("640 x 480");
            listResolutions.AddItem("800 x 600");
            listResolutions.AddItem("1024 x 768");
            listResolutions.Selected = 0;

            //maps label
            position.UpperLeftCorner.Y += listResolutions.AbsolutePosition.Height;
            position.LowerRightCorner.Y += 10;
            IGUIStaticText labelMaps = device.GUIEnvironment.AddStaticText("Select 3D scene:", position, false, false, null, -1);
            labelMaps.OverrideColor = new Color(100, 255, 255, 255);

            //display a list of available maps
            position.UpperLeftCorner.Y += labelMaps.AbsolutePosition.Height;
            position.LowerRightCorner.Y += listResolutions.AbsolutePosition.Height + 10;

            IGUIListBox listMaps = device.GUIEnvironment.AddListBox(position, null, (int)MyMenuControls.LIST_MAPS, true);
            string filePattern = "*.bsp;*.b3d;*.pk3;*.x;*.3ds;*.my3d;*.dmf;*.obj;*.dae";
            string[] fileExtensions = filePattern.Split(';');
            if (Directory.Exists(@"media\maps"))
            {
                foreach (string fileExtension in fileExtensions)
                {
                    string[] maps = Directory.GetFiles(@"media\maps", fileExtension);
                    foreach (string map in maps)
                    {
                        listMaps.AddItem(map);
                    }
                }
            }

            //display a list of available bit depth
            position.UpperLeftCorner.Y += listMaps.AbsolutePosition.Height;
            position.LowerRightCorner.Y += listMaps.AbsolutePosition.Height / 2;
            IGUIListBox listBits = device.GUIEnvironment.AddListBox(position, null, (int)MyMenuControls.LIST_BITS, true);
            listBits.AddItem("16 bits");
            listBits.AddItem("32 bits");
            listBits.Selected = 1;

            //display Fullscreen checkbox
            //position.UpperLeftCorner.Y += listBits.AbsolutePosition.Height;
            //position.LowerRightCorner.Y += 15;
            //device.GUIEnvironment.AddCheckBox(true, position, null, -1, "Full screen");

            //display Vsync checkbox
            //position.UpperLeftCorner.Y += 30;
            //position.LowerRightCorner.Y += 30;
            //device.GUIEnvironment.AddCheckBox(false, position, null, -1, "Vsync");


            //display demo mode button
            position.UpperLeftCorner.Y += listBits.AbsolutePosition.Height+1;
            position.LowerRightCorner.Y += 30;
            device.GUIEnvironment.AddButton(position, null, (int)MyMenuControls.START_AUTO_MODE, "Auto mode");

            //walkthrough button
            position.UpperLeftCorner.Y += 30;
            position.LowerRightCorner.Y += 30;
            device.GUIEnvironment.AddButton(position, null, (int)MyMenuControls.START_WALKTHROUGH_MODE, "Tourist mode");

            //exit button
            position.UpperLeftCorner.Y += 30;
            position.LowerRightCorner.Y += 30;
            device.GUIEnvironment.AddButton(position, null, (int)MyMenuControls.EXIT_BUTTON, "Exit");

            //display map description
            Rect position2 = new Rect(new Position2D(screenWidth- labelRenderer.AbsolutePosition.UpperLeftCorner.X - labelRenderer.AbsolutePosition.Width-5, labelRenderer.AbsolutePosition.UpperLeftCorner.Y),
                                        new Dimension2D(2*(position.LowerRightCorner.X - labelRenderer.AbsolutePosition.UpperLeftCorner.X), position.LowerRightCorner.Y - labelRenderer.AbsolutePosition.UpperLeftCorner.Y));
            mapDescription = device.GUIEnvironment.AddStaticText("Scene description", position2, true, true, null, -1);
            mapDescription.OverrideColor = new Color(100, 255, 255, 255);

            

            //render
            device.Run();
            while (device.Run() && ismenudisplaying)// && ClickedButton == MyMenuControls.NONE)
            {
                if (device.WindowActive)
                {
                    device.VideoDriver.BeginScene(true, true, new Color(255, 0, 0, 50));

                    device.SceneManager.DrawAll();
                    device.GUIEnvironment.DrawAll();

                    device.VideoDriver.EndScene();
                }
            }
            device.CloseDevice();
            return;

        }

        //DETECT FILET TYPES FOR WALKTHROUGH
        public void runWalkthrough()
        {
            ismenudisplaying = false;
            //OLD METHOD TO GET A PATH EXTENSION
            //System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");
            //if (mapFilename.EndsWith(".pk3", true, ci))

            
            //NEW METHOD:
            //if it is compressed .pk3 file
            if (Path.GetExtension(mapFilename).Equals(".pk3", StringComparison.CurrentCultureIgnoreCase))
            {
                runWalkthroughPK3();
            }
            //or if it is .irr scene
            if (Path.GetExtension(mapFilename).Equals(".irr", StringComparison.CurrentCultureIgnoreCase))
            {
                runWalkthroughIRR();
            }
            //if it is not .pk3 nor .irr 
            if (Path.GetExtension(mapFilename).Equals(".pk3", StringComparison.CurrentCultureIgnoreCase) == false &&
                Path.GetExtension(mapFilename).Equals(".irr", StringComparison.CurrentCultureIgnoreCase) == false)
            {
                runNormalWalkthrough();
            }

            //ISceneNode scenenode = device.SceneManager.AddOctTreeSceneNode(mesh, null, -1);
            
        }

        //WALKTHROUGH FOR IRR FORMAT
        public void runWalkthroughIRR()
        {
            //close old device
            device.CloseDevice();
            //add sound
            soundengine.StopAllSounds();

            //********** add code to add sound here

            //start new device
            device = new IrrlichtDevice(SelectedDriverType, new Dimension2D(scrnWidth, scrnHeight), scrnBitDepth, true, true, false);
            if (device == null)
            {
                System.Windows.Forms.MessageBox.Show("An error encountered, the program is now closed. We are sorry.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Stop);
                Environment.Exit(1);
            }
            device.ResizeAble = true;
            device.EventReceiver = this;
            device.WindowCaption = "3DTours";

            //3DTours logo
            device.GUIEnvironment.AddImage(device.VideoDriver.GetTexture(@"media\logo.png"),
            new Position2D(0, 0), true, null, -1, "");

            device.SceneManager.LoadScene(mapFilename);
            ICameraSceneNode cam = device.SceneManager.AddCameraSceneNodeFPS(null, 100, 300, -1);

            addSkybox();

            //display camera position
            labelcampos = device.GUIEnvironment.AddStaticText("Camera position", new Rect(new Position2D(0, scrnHeight - 10), new Dimension2D(scrnWidth, 10)), false, false, null, -1);
            labelcampos.OverrideColor = new Color(150, 255, 255, 255);
            labelcampos.Visible = false; //hide by default

            device.Run();
            //render
            while (device.Run())
            {
                if (device.WindowActive)
                {
                    device.VideoDriver.BeginScene(true, true, new Color(255, 0, 0, 0));
                    device.SceneManager.DrawAll();
                    device.GUIEnvironment.DrawAll();

                    //update current camera position
                    labelcampos.Text = "Camera position:" + cam.Position.ToString();

                    device.VideoDriver.EndScene();
                }
            }
            device.CloseDevice();

        }

        //WALKTHROUGH FOR OTHER MAP FORMATS
        public void runNormalWalkthrough()
        {
            parseWaypointsList(mapFilename);
            //close old device
            device.CloseDevice();
            //add sound
            soundengine.StopAllSounds();

            //********** add code to add sound here

            //start new device
            device = new IrrlichtDevice(SelectedDriverType, new Dimension2D(scrnWidth, scrnHeight), scrnBitDepth, false, true, false);
            if (device == null)
            {
                System.Windows.Forms.MessageBox.Show("An error encountered, the program is now closed. We are sorry.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Stop);
                Environment.Exit(1);
            }
            device.ResizeAble = true;
            device.EventReceiver = this;
            device.WindowCaption = "3DTours";

            //device.VideoDriver.SetTextureCreationFlag(Irrlicht.Video.TextureCreationFlag.ALWAYS_32_BIT, true);
            //device.VideoDriver.SetFog(new Color(150, 200, 200, 200), true, 100, 110, 0.1f, true, true);

            //3DTours logo
            device.GUIEnvironment.AddImage(device.VideoDriver.GetTexture(@"media\logo.png"),
            new Position2D(0, 0), true, null, -1, "");

            IAnimatedMesh mesh = device.SceneManager.GetMesh(mapFilename);
            if (mesh == null)
                System.Windows.Forms.MessageBox.Show("mesh is null");
            //repair texture mapping for 3ds
            //device.SceneManager.MeshManipulator.MakePlanarTextureMapping(mesh.GetMesh(0), 0.003f);

            ISceneNode scenenode = device.SceneManager.AddOctTreeSceneNode(mesh, null, -1);
            //scenenode.Position = new Irrlicht.Core.Vector3D(-1370, -130, -1400);
            if (scenenode == null)
                System.Windows.Forms.MessageBox.Show("Scene node is null");
            scenenode.SetMaterialFlag(MaterialFlag.LIGHTING, false);

            //create camera
            ICameraSceneNode cam = device.SceneManager.AddCameraSceneNodeFPS(null, 100, 300, -1);
            //ICameraSceneNode cam = device.SceneManager.AddCameraSceneNode(null, new Irrlicht.Core.Vector3D(20, 300, -50), new Irrlicht.Core.Vector3D(20, 300, 50), -1);
            cam.Position = readStartPosition();

            //hide cursor
            device.CursorControl.Visible = false;

            //create collision and add to the camera

            ITriangleSelector selector = device.SceneManager.CreateOctTreeTriangleSelector(mesh.GetMesh(0), scenenode, 128);
            ISceneNodeAnimator collision = device.SceneManager.CreateCollisionResponseAnimator(
                selector, cam,
                new Irrlicht.Core.Vector3D(30, 50, 30),  // size of ellipsoid around camera
                new Irrlicht.Core.Vector3D(0, -1, 0),  // gravity
                new Irrlicht.Core.Vector3D(0, 50, 0),  // translation
                0.0005f);                // sliding value
            cam.AddAnimator(collision);
            camCollisionAdded = true;

            IGUIFont font = device.GUIEnvironment.GetFont(@"media\fonts\mssansserif.xml");

            addSkybox();

            //display camera position
            labelcampos = device.GUIEnvironment.AddStaticText("Camera position", new Rect(new Position2D(0, scrnHeight - 10), new Dimension2D(scrnWidth, 10)), false, false, null, -1);
            labelcampos.OverrideColor = new Color(150, 255, 255, 255);
            labelcampos.Visible = false; //hide by default

            device.Run();
            //render
            while (device.Run())
            {
                if (device.WindowActive)
                {
                    device.VideoDriver.BeginScene(true, true, new Color(255, 0, 0, 0));
                    device.SceneManager.DrawAll();
                    device.GUIEnvironment.DrawAll();

                    if (camCollision)
                    {
                        if (camCollisionAdded == false)
                        {
                            cam.AddAnimator(collision);
                            camCollisionAdded = true;
                        }
                    }
                    else
                    {
                        if (camCollisionAdded == true)
                        {
                            cam.RemoveAnimators();
                            camCollisionAdded = false;
                        }
                    }

                    //update current camera position
                    labelcampos.Text = "Camera position:" + cam.Position.ToString();

                    device.VideoDriver.EndScene();
                }
            }
            device.CloseDevice();
        }

        //START WALKTHROUGH (PK3) MODE
        public void runWalkthroughPK3()
        {
            //close old device
            device.CloseDevice();
            //add sound
            soundengine.StopAllSounds();

            //********** add code to add sound here

            //start new device
            device = new IrrlichtDevice(SelectedDriverType, new Dimension2D(scrnWidth, scrnHeight), scrnBitDepth, true, true, false);
            if (device == null)
            {
                System.Windows.Forms.MessageBox.Show("An error encountered, the program is now closed. We are sorry.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Stop);
                Environment.Exit(1);
            }
            device.ResizeAble = true;
            device.EventReceiver = this;
            device.WindowCaption = "3DTours";

            //3DTours logo
            device.GUIEnvironment.AddImage(device.VideoDriver.GetTexture(@"media\logo.png"),
            new Position2D(0, 0), true, null, -1, "");

            
            device.FileSystem.AddZipFileArchive(mapFilename);
            IAnimatedMesh mesh = device.SceneManager.GetMesh(Path.GetFileNameWithoutExtension(mapFilename) + ".bsp");
            if (mesh == null)
                System.Windows.Forms.MessageBox.Show("mesh is null");
            ISceneNode scenenode = device.SceneManager.AddOctTreeSceneNode(mesh, null, -1);
            //scenenode.Position = new Irrlicht.Core.Vector3D(-1370, -130, -1400);
            if (scenenode == null)
                System.Windows.Forms.MessageBox.Show("Scene node is null");

            //create camera
            ICameraSceneNode cam = device.SceneManager.AddCameraSceneNodeFPS(null, 100, 300, -1);
            //ICameraSceneNode cam = device.SceneManager.AddCameraSceneNode(null, new Irrlicht.Core.Vector3D(20, 300, -50), new Irrlicht.Core.Vector3D(20, 300, 50), -1);
            cam.Position = readStartPosition();

            //hide cursor
            device.CursorControl.Visible = false;

            //create collision and add to the camera
            ITriangleSelector selector = device.SceneManager.CreateOctTreeTriangleSelector(mesh.GetMesh(0), scenenode, 128);
            ISceneNodeAnimator collision = device.SceneManager.CreateCollisionResponseAnimator(
                selector, cam,
                new Irrlicht.Core.Vector3D(30, 50, 30),  // size of ellipsoid around camera
                new Irrlicht.Core.Vector3D(0, -3, 0),  // gravity
                new Irrlicht.Core.Vector3D(0, 50, 0),  // translation
                0.0005f);                // sliding value
            cam.AddAnimator(collision);
            camCollisionAdded = true;

            IGUIFont font = device.GUIEnvironment.GetFont(@"media\fonts\mssansserif.xml");

            addSkybox();

            //display camera position
            labelcampos = device.GUIEnvironment.AddStaticText("Camera position", new Rect(new Position2D(0, scrnHeight - 10), new Dimension2D(scrnWidth, 10)), false, false, null, -1);
            labelcampos.OverrideColor = new Color(150, 255, 255, 255);
            labelcampos.Visible = false; //hide by default

            device.Run();
            //render
            while (device.Run())
            {
                if (device.WindowActive)
                {
                    device.VideoDriver.BeginScene(true, true, new Color(255, 0, 0, 0));
                    device.SceneManager.DrawAll();
                    device.GUIEnvironment.DrawAll();

                    //remove or add collision to camera
                    if (camCollision)
                    {
                        if (camCollisionAdded == false)
                        {
                            cam.AddAnimator(collision);
                            camCollisionAdded = true;
                        }   
                    }
                    else
                    {
                        if (camCollisionAdded == true)
                        {
                            cam.RemoveAnimators();
                            camCollisionAdded = false;
                        }
                    }

                    //update current camera position
                    labelcampos.Text = "Camera position:" + cam.Position.ToString();

                    device.VideoDriver.EndScene();
                }
            }
            device.CloseDevice();
        }

        //***************************************
        //GUIDED MODE
        //***************************************
        //DETECT FILET TYPES FOR GUIDED MODE
        public void runGuidedmode()
        {
            ismenudisplaying = false;

            //OLD METHOD TO GET A PATH EXTENSION
            //System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");
            //if (mapFilename.EndsWith(".pk3", true, ci))


            //NEW METHOD:
            //if it is compressed .pk3 file
            if (Path.GetExtension(mapFilename).Equals(".pk3", StringComparison.CurrentCultureIgnoreCase))
            {
                runGuidedmodePK3();
            }
            //or if it is .irr scene
            if (Path.GetExtension(mapFilename).Equals(".irr", StringComparison.CurrentCultureIgnoreCase))
            {
                runGuidedmodeIRR();
            }
            //if it is not .pk3 nor .irr 
            if (Path.GetExtension(mapFilename).Equals(".pk3", StringComparison.CurrentCultureIgnoreCase) == false &&
                Path.GetExtension(mapFilename).Equals(".irr", StringComparison.CurrentCultureIgnoreCase) == false)
            {
                runNormalGuidedmode();
            }

        }

        public void runGuidedmodePK3()
        {
            //close old device
            device.CloseDevice();
            //add sound
            soundengine.StopAllSounds();

            //********** add code to add sound here

            //start new device
            device = new IrrlichtDevice(SelectedDriverType, new Dimension2D(scrnWidth, scrnHeight), scrnBitDepth, true, true, false);
            if (device == null)
            {
                System.Windows.Forms.MessageBox.Show("An error encountered, the program is now closed. We are sorry.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Stop);
                Environment.Exit(1);
            }
            device.ResizeAble = true;
            device.EventReceiver = this;
            device.WindowCaption = "3DTours";

            //3DTours logo
            device.GUIEnvironment.AddImage(device.VideoDriver.GetTexture(@"media\logo.png"),
            new Position2D(0, 0), true, null, -1, "");


            device.FileSystem.AddZipFileArchive(mapFilename);
            IAnimatedMesh mesh = device.SceneManager.GetMesh(Path.GetFileNameWithoutExtension(mapFilename) + ".bsp");
            if (mesh == null)
                System.Windows.Forms.MessageBox.Show("mesh is null");
            ISceneNode scenenode = device.SceneManager.AddOctTreeSceneNode(mesh, null, -1);
            //scenenode.Position = new Irrlicht.Core.Vector3D(-1370, -130, -1400);
            if (scenenode == null)
                System.Windows.Forms.MessageBox.Show("Scene node is null");

            addSkybox();
            
            Irrlicht.Core.Vector3D[] waypoints = parseWaypointsList(mapFilename);

            ICameraSceneNode cam = device.SceneManager.AddCameraSceneNode(null, waypoints[0], waypoints[waypoints.Length-1], -1);
            ISceneNodeAnimator sa = device.SceneManager.CreateFollowSplineAnimator((int)device.Timer.Time, waypoints, 0.1f, 1);
            cam.AddAnimator(sa);

            //display camera position
            labelcampos = device.GUIEnvironment.AddStaticText("Camera position", new Rect(new Position2D(0, scrnHeight - 10), new Dimension2D(scrnWidth, 10)), false, false, null, -1);
            labelcampos.OverrideColor = new Color(150, 255, 255, 255);
            labelcampos.Visible = false; //hide by default
            device.Run();

            //render
            while (device.Run())
            {
                if (device.WindowActive)
                {
                    device.VideoDriver.BeginScene(true, true, new Color(255, 0, 0, 0));
                    device.SceneManager.DrawAll();
                    device.GUIEnvironment.DrawAll();

                    //update current camera position
                    labelcampos.Text = "Camera position:" + cam.Position.ToString();

                    device.VideoDriver.EndScene();
                }
            }
            device.CloseDevice();

        }

        //GUIDED MODE IRR
        public void runGuidedmodeIRR()
        {
        }

        public void runNormalGuidedmode()
        {
            //close old device
            device.CloseDevice();
            //add sound
            soundengine.StopAllSounds();

            //********** add code to add sound here

            //start new device
            device = new IrrlichtDevice(SelectedDriverType, new Dimension2D(scrnWidth, scrnHeight), scrnBitDepth, true, true, false);
            if (device == null)
            {
                System.Windows.Forms.MessageBox.Show("An error encountered, the program is now closed. We are sorry.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Stop);
                Environment.Exit(1);
            }
            device.ResizeAble = true;
            device.EventReceiver = this;
            device.WindowCaption = "3DTours";

            //3DTours logo
            device.GUIEnvironment.AddImage(device.VideoDriver.GetTexture(@"media\logo.png"),
            new Position2D(0, 0), true, null, -1, "");

            IAnimatedMesh mesh = device.SceneManager.GetMesh(mapFilename);
            if (mesh == null)
                System.Windows.Forms.MessageBox.Show("mesh is null");
            ISceneNode scenenode = device.SceneManager.AddOctTreeSceneNode(mesh, null, -1);
            //scenenode.Position = new Irrlicht.Core.Vector3D(-1370, -130, -1400);
            if (scenenode == null)
                System.Windows.Forms.MessageBox.Show("Scene node is null");
            scenenode.SetMaterialFlag(MaterialFlag.LIGHTING, false);

            addSkybox();

            Irrlicht.Core.Vector3D[] waypoints = parseWaypointsList(mapFilename);

            ICameraSceneNode cam = device.SceneManager.AddCameraSceneNode(null, waypoints[0], waypoints[waypoints.Length - 1], -1);
            ISceneNodeAnimator sa = device.SceneManager.CreateFollowSplineAnimator((int)device.Timer.Time, waypoints, 0.1f, 1);
            cam.AddAnimator(sa);

            //display camera position
            labelcampos = device.GUIEnvironment.AddStaticText("Camera position", new Rect(new Position2D(0, scrnHeight - 10), new Dimension2D(scrnWidth, 10)), false, false, null, -1);
            labelcampos.OverrideColor = new Color(150, 255, 255, 255);
            labelcampos.Visible = false; //hide by default
            device.Run();

            //render
            while (device.Run())
            {
                if (device.WindowActive)
                {
                    device.VideoDriver.BeginScene(true, true, new Color(255, 0, 0, 0));
                    device.SceneManager.DrawAll();
                    device.GUIEnvironment.DrawAll();

                    //update current camera position
                    labelcampos.Text = "Camera position:" + cam.Position.ToString();

                    device.VideoDriver.EndScene();
                }
            }
            device.CloseDevice();
        }

        //PLAY BACKGROUND MUSIC
        public void playBGM()
        {

        }

        //PLAY CUSTOM SOUND
        public void playCustomSound()
        {

        }

        //ADD SKYBOX
        //plan for next version: allow user to add custom skybox
        public void addSkybox()
        {
            device.SceneManager.AddSkyBoxSceneNode(
            device.VideoDriver.GetTexture(@"media\textures\irrlicht2_up.jpg"),
            device.VideoDriver.GetTexture(@"media\textures\irrlicht2_dn.jpg"),
            device.VideoDriver.GetTexture(@"media\textures\irrlicht2_lf.jpg"),
            device.VideoDriver.GetTexture(@"media\textures\irrlicht2_rt.jpg"),
            device.VideoDriver.GetTexture(@"media\textures\irrlicht2_ft.jpg"),
            device.VideoDriver.GetTexture(@"media\textures\irrlicht2_bk.jpg"),
            null, -1);
        }

        //READ START POSITION
        public Irrlicht.Core.Vector3D readStartPosition()
        {
            if (File.Exists(mapFilename + "-start.txt"))
            {
                TextReader reader = new StreamReader(mapFilename + "-start.txt");
                string textString = reader.ReadToEnd();
                string[] values = textString.Split(',');
                Irrlicht.Core.Vector3D campos = new Irrlicht.Core.Vector3D(float.Parse(values[0].Trim()),
                                                                           float.Parse(values[1].Trim()),
                                                                           float.Parse(values[2].Trim()));
                reader.Close();
                reader.Dispose();
                return campos;
            }
            else
                return new Irrlicht.Core.Vector3D(0, 0, 0);

        }

        //DISPLAY STATUS
        public void displayStatus(string message)
        {
            IGUIFont font = device.GUIEnvironment.GetFont(@"media\fonts\mssansserif.xml");
            if (font != null)
                device.GUIEnvironment.Skin.Font = font;

            if (labelstatus!=null)
                labelstatus.Remove();
            Timer mytimer = new Timer(5000);
            mytimer.Elapsed+=new ElapsedEventHandler(DisplayTimeEvent);
            mytimer.Start();
            labelstatus = device.GUIEnvironment.AddStaticText(message, new Rect(new Position2D(scrnWidth/2- font.GetDimension(message).Width/2, scrnHeight/2 - 10), font.GetDimension(message)), false, false, null, -1);
            labelstatus.OverrideColor = new Color(150, 83, 218, 63);
        }

        //TIMER EVENT
        public void DisplayTimeEvent(object source, ElapsedEventArgs e)
        {
            labelstatus.Remove();
        }

        //PARSE WAYPOINTS LIST
        public Irrlicht.Core.Vector3D[] parseWaypointsList(string mapname)
        {
            
            if (File.Exists(mapFilename + "-waypoints.txt"))
            {
                TextReader reader = new StreamReader(mapFilename + "-waypoints.txt");
                string textString = reader.ReadToEnd();
                textString = textString.Replace("\r", "");
                string[] values = textString.Split("\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                Irrlicht.Core.Vector3D[] waypoints= new Irrlicht.Core.Vector3D[values.Length];
                for (int i = 0; i <= values.Length - 1; i++)
                {
                    //System.Windows.Forms.MessageBox.Show(values[i]); //debug
                    string[] subvalues = values[i].Split(',');
                    waypoints[i] = new Irrlicht.Core.Vector3D(float.Parse(subvalues[0].Trim()),
                                                              float.Parse(subvalues[1].Trim()),
                                                              float.Parse(subvalues[2].Trim()));
                }
                reader.Close();
                reader.Dispose();
                return waypoints;
            }
            else
                return new Irrlicht.Core.Vector3D[1];
        }
    }
}
