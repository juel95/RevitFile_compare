using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RVDB = Autodesk.Revit.DB;

namespace RevitFile_compare
{
    public partial class Verify_Form : Form
    {
        public Verify_Form()
        {
            InitializeComponent();
            progressBar1.Visible = false;
            label3.Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Revit文件|*.rvt";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = ofd.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            label2.Text = "";
            if (string.IsNullOrEmpty(textBox1.Text))
                MessageBox.Show("请选择文件");
            else
            {
                var doc =RevitCoreContext.Instance.Application.OpenDocumentFile(textBox1.Text);
                if (doc == null)
                    throw new InvalidOperationException();
                var elems = new RVDB.FilteredElementCollector(doc).WhereElementIsElementType().ToList();

                progressBar1.Maximum = elems.Count;
                progressBar1.Step = 1;
                progressBar1.Visible = true; ;
                label3.Visible = true;

                var length = elems.Count;
                int count = 0,n=0;
                for (int i = 0; i < length; i++)
                {
                    try
                    {
                        if(elems[i].IsExistEntityData())
                        count++;
                    }
                    catch
                    {

                    }
                    //进度条
                    n++;
                    progressBar1.Value = n;
                    double percent = (n / length) * 100;
                    label3.Text = string.Format(percent.ToString() + "%");
                    progressBar1.Refresh();
                    label3.Refresh();

                }
                label2.Text = string.Format("扫描结果：文件元素总个数为{0}个，持有标记的元素个数为{1}个",length ,count);
                doc.Close(false);
            }
        }
    }
}
