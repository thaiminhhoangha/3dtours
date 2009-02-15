using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Irrlicht;
using Irrlicht.Core;
using Irrlicht.Scene;
using Irrlicht.Video;

namespace _DToursEditor
{
    public partial class Form1 : Form 
    {
        private frmWaypoints frmWaypts;
        private frmRenderMethod frmRender;
        private ListBox lstWaypts;
        private DriverType driver;
        public IrrlichtDevice device;
        public ICameraSceneNode cam;
        public ICameraSceneNode cam2;

        public Form1()
        {
            InitializeComponent();
        }

        //DISPLAY MAP METHOD
        public void displayMap(string mapFilename, Control c)
        {
            //switch (frmRender.comboRenderers.SelectedIndex)
            switch(Properties.Settings.Default.DriverType)
            {
                case 0:
                    driver = DriverType.SOFTWARE;
                    break;
                case 1:
                    driver = DriverType.SOFTWARE2;
                    break;
                case 2:
                    driver = DriverType.DIRECT3D8;
                    break;
                case 3:
                    driver = DriverType.DIRECT3D9;
                    break;
                case 4:
                    driver = DriverType.OPENGL;
                    break;
                default:
                    driver = DriverType.SOFTWARE;
                    frmRender.comboRenderers.SelectedIndex = 0;
                    break;
            }
            /*if (device != null)
            {
                device.CloseDevice();
                //device = null;
            }*/
            if (device == null)
            {
                device = new IrrlichtDevice(driver,
                        new Dimension2D(c.Width, c.Height),
                        32, false, false, false, true, c.Handle);
                device.ResizeAble = true;
            }

            cam2 = device.SceneManager.AddCameraSceneNodeFPS();
            cam = device.SceneManager.AddCameraSceneNodeMaya(null, -1000, 1000, 1000, -1);

            IAnimatedMesh mesh = null;
            if (mapFilename.EndsWith(".pk3", StringComparison.CurrentCultureIgnoreCase))
            {
                device.FileSystem.AddZipFileArchive(mapFilename);
                mesh = device.SceneManager.GetMesh(Path.GetFileNameWithoutExtension(mapFilename) + ".bsp");
                ISceneNode scenenode = device.SceneManager.AddOctTreeSceneNode(mesh, null, -1);
            }
            else
            {
                mesh = device.SceneManager.GetMesh(mapFilename);
                ISceneNode scenenode = device.SceneManager.AddOctTreeSceneNode(mesh, null, -1);
                scenenode.SetMaterialFlag(MaterialFlag.LIGHTING, false);
            }
            if (mesh == null)
            {
                MessageBox.Show("Cannot load scene", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //move camera to start position
            if (frmWaypts.txtStart.Text != "")
            {
                cam.Position = parseStartPosition(frmWaypts.txtStart.Text);
            }
            // render
            device.Run();
            while (device.Run() && c.Enabled)
            {
                device.VideoDriver.BeginScene(true, true, new Irrlicht.Video.Color(255, 255, 255, 255));
                device.SceneManager.DrawAll();
                device.GUIEnvironment.DrawAll();

                statusStrip1.Items[1].Text = cam.Position.ToString();
                frmWaypts.labelCamPosition.Text = cam.Position.ToString();


                device.VideoDriver.EndScene();
            }

        }

        //READ START POSITION
        public Irrlicht.Core.Vector3D parseStartPosition(string position)
        {
            string[] values = position.Split(',');
            Irrlicht.Core.Vector3D campos = new Irrlicht.Core.Vector3D(float.Parse(values[0]),
                                                                       float.Parse(values[1]),
                                                                       float.Parse(values[2]));
            return campos;

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Open 3D scene...";
            openFileDialog1.Filter = "All supported file types|*.pk3;*.b3d;*.x;*.3ds;*.my3d;*.dmf;*.obj;*.dae";
            openFileDialog1.Multiselect = false;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string[] waypoints = parseWaypointsList(openFileDialog1.FileName);

                //create waypoint list
                lstWaypts = frmWaypts.listWaypoints;
                lstWaypts.Items.Clear();
                if(waypoints!=null)
                    lstWaypts.Items.AddRange(waypoints);

                //display start position
                frmWaypts.txtStart.Text = readStartPosition(openFileDialog1.FileName);

                //display status
                statusStrip1.Items[0].Text = openFileDialog1.FileName;
                statusStrip1.Items[0].ToolTipText = openFileDialog1.FileName;

                //display scene
                displayMap(openFileDialog1.FileName, pictureBox1);

            }
            
        }

        //PARSE WAYPOINT LIST
        public string[] parseWaypointsList(string mapname)
        {

            if (File.Exists(mapname + "-waypoints.txt"))
            {
                TextReader reader = new StreamReader(mapname + "-waypoints.txt");
                string textString = reader.ReadToEnd();
                textString = textString.Replace("\r", "");
                string[] values = textString.Split("\n".ToCharArray());
                
                reader.Close();
                reader.Dispose();
                return values;
            }
            else
                return null;
        }

        //LOAD START POSITION
        public string readStartPosition(string mapname)
        {
            if (File.Exists(mapname + "-start.txt"))
            {
                TextReader reader = new StreamReader(mapname + "-start.txt");
                string textString = reader.ReadToEnd();
                reader.Close();
                reader.Dispose();
                return textString;
            }
            else
                return "";

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            frmWaypts = new frmWaypoints();
            frmWaypts.Show();
            frmWaypts.Left = this.Left - frmWaypts.Width;
            frmWaypts.Top = this.Top;
            
            frmRender = new frmRenderMethod();
            frmRender.Left = this.Left + this.Width;
            frmRender.Top = this.Top;
            frmRender.Show();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to exit?", "Confirm exit", MessageBoxButtons.YesNo) == DialogResult.Yes)
                Application.Exit();
        }

        private void changerenderMethodToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmRender.Show();
            frmRender.Focus();
        }

        private void editWaypointsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmWaypts.Show();
            frmWaypts.Focus();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (device != null)
            {
                pictureBox1.Enabled = false;
                device.CloseDevice();
                device = null;
                Application.Exit();
            }
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            frmWaypts.Left = this.Left - frmWaypts.Width;
            frmWaypts.Top = this.Top;
            
            frmRender.Left = this.Left + this.Width;
            frmRender.Top = this.Top;
        }

        private void about3DToursEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 frmAbout = new AboutBox1();
            frmAbout.ShowDialog(this);
        }

        private void changeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            device.SceneManager.ActiveCamera = cam;
            cam.Position = cam2.Position;
        }

        //DEVICE EVENT RECEIVER
        /*
        public bool OnEvent(Event p_e)
        {
            if (p_e.Type == EventType.KeyInput && !p_e.KeyPressedDown)
            {
                switch (p_e.Key)
                {
                    case KeyCode.KEY_F2:
                        device.SceneManager.ActiveCamera = cam;
                        cam.Position = cam2.Position;
                        break;
                    case KeyCode.KEY_F3:
                        device.SceneManager.ActiveCamera = cam2;
                        cam2.Position = cam.Position;
                        break;
                }
            }
            return false;
        }
        */

        private void fPSStyleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            device.SceneManager.ActiveCamera = cam2;
            cam2.Position = cam.Position;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (frmWaypts.txtStart.Text != "")
            {
                saveStartPosition();

                if (frmWaypts.listWaypoints.Items.Count >= 2)
                {
                    saveWaypointList();
                }
                else
                {
                    MessageBox.Show("Waypoint list requires at least 2 positions.", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Please set start position", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            }
        }

        //SAVE WAYPOINT LIST METHOD
        private void saveWaypointList()
        {
            TextWriter writer = new StreamWriter(openFileDialog1.FileName + "-waypoints.txt");
            for (int i = 0; i <= frmWaypts.listWaypoints.Items.Count - 1; i++)
            {
                string data = frmWaypts.listWaypoints.Items[i].ToString().Replace(" ", "");
                writer.WriteLine(data);
            }
            writer.Close();
        }

        //SAVE START POSITION METHOD
        private void saveStartPosition()
        {
            TextWriter writer = new StreamWriter(openFileDialog1.FileName + "-start.txt");
            string data = frmWaypts.txtStart.Text.Replace(" ", "");
            writer.WriteLine(data);
            writer.Close();
        }
    }
}