using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;

namespace _DToursEditor
{
    public partial class frmRenderMethod : Form
    {
        public frmRenderMethod()
        {
            InitializeComponent();
        }

        private void frmRenderMethod_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.DriverType = comboRenderers.SelectedIndex;
            Properties.Settings.Default.Save();
            if (MessageBox.Show("Bạn cần phải khởi động lại 3DTours Editor để các thay đổi có tác dụng. Bạn có muốn khởi động lại ngay bây giờ?", "Chọn trình điều khiển mới", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
            {
                try
                {
                    System.Threading.Thread.Sleep(3000);
                    Process proc = new Process();
                    proc.StartInfo.FileName = @"3DToursEditor.exe";
                    //proc.StartInfo.Arguments = Current Opened Map;
                    proc.Start();
                    Application.Exit();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            
        }

        private void frmRenderMethod_Load(object sender, EventArgs e)
        {
            comboRenderers.SelectedIndex = Properties.Settings.Default.DriverType;
        }
    }
}