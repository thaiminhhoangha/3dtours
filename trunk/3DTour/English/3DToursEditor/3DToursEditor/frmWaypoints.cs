using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace _DToursEditor
{
    public partial class frmWaypoints : Form
    {
        
        public frmWaypoints()
        {
            InitializeComponent();
        }

        private void frmWaypoints_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (labelCamPosition.Text != "")
                listWaypoints.Items.Add(labelCamPosition.Text.Substring(1, labelCamPosition.Text.Length - 2));
        }

        private void frmWaypoints_Load(object sender, EventArgs e)
        {
            labelCamPosition.Text = "";
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (listWaypoints.SelectedIndex >= 0)
                listWaypoints.Items.RemoveAt(listWaypoints.SelectedIndex);
            //else
                //MessageBox.Show(listWaypoints.SelectedIndex.ToString());
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            int selectedIndex = listWaypoints.SelectedIndex;

            if (selectedIndex > 0)
            {
                string previous = listWaypoints.Items[selectedIndex - 1].ToString();
                listWaypoints.Items[selectedIndex - 1] = listWaypoints.Items[selectedIndex];
                listWaypoints.Items[selectedIndex] = previous;
                listWaypoints.SelectedIndex--;
            }
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            int selectedIndex = listWaypoints.SelectedIndex;

            if (selectedIndex < listWaypoints.Items.Count - 1)
            {
                string next = listWaypoints.Items[selectedIndex + 1].ToString();
                listWaypoints.Items[selectedIndex + 1] = listWaypoints.Items[selectedIndex];
                listWaypoints.Items[selectedIndex] = next;
                listWaypoints.SelectedIndex++;
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to clear waypoint list?", "Confirm clear waypoint list", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                listWaypoints.Items.Clear();
        }

        private void btnSetStart_Click(object sender, EventArgs e)
        {
            if (labelCamPosition.Text != "")
                txtStart.Text = labelCamPosition.Text.Substring(1, labelCamPosition.Text.Length - 2);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                if (listWaypoints.Items.Count > 0)
                {
                    txtStart.Text = listWaypoints.Items[0].ToString() ;
                }
            }
        }

        private void listWaypoints_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                if (listWaypoints.Items.Count > 0)
                {
                    txtStart.Text = listWaypoints.Items[0].ToString();
                }
            }
        }
    }
}