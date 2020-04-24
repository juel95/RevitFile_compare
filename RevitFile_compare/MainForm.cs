using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RevitFile_compare
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            radioButton1.Checked = true;
            try
            {
                RevitCoreContext.Instance?.Run();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(radioButton1.Checked)
            {
                Marked_Form form2 = new Marked_Form();
                form2.ShowDialog();
            }
            if (radioButton2.Checked)
            {
                Verify_Form form3 = new Verify_Form();
                form3.ShowDialog();
            }
            if (radioButton3.Checked)
            {
                FileCompareFile_Form form1 = new FileCompareFile_Form();
                form1.ShowDialog();
            }

        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}
