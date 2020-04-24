using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RVDB=Autodesk.Revit.DB;

namespace RevitFile_compare
{
    public partial class Marked_Form : Form
    {
        public Marked_Form()
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
                var doc = RevitCoreContext.Instance.Application.OpenDocumentFile(textBox1.Text);
                if (doc == null)
                    throw new InvalidOperationException();
                var elems = new RVDB.FilteredElementCollector(doc).WhereElementIsElementType().ToList();

                progressBar1.Maximum = elems.Count;
                progressBar1.Step = 1;
                progressBar1.Visible = true; ;
                label3.Visible = true;

                var length = elems.Count;
                int count = 0,n=0;
                for(int i=0;i<length;i++)
                {
                    try
                    {
                        var elem = elems[i];
                        elem.AddEntityData(doc);
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
                label2.Text = string.Format("搜索元素（包括不可见）共{0}个，成功标记个数共{1}", length, count);
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "revit文件|*.rvt";
                sfd.FileName = "标记文件.rvt";
                if(sfd.ShowDialog()==DialogResult.OK)
                {
                    var workPath = sfd.FileName;
                    doc.SaveAs(workPath, new RVDB.SaveAsOptions() { OverwriteExistingFile = true });
                    MessageBox.Show("成功");
                    doc.Close(false);
                }
                
            }
        }
    }
}
