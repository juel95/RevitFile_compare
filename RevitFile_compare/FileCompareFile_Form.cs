using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RVDB=Autodesk.Revit.DB;

namespace RevitFile_compare
{
    public partial class FileCompareFile_Form : Form
    {
        int FileCount;
        int SameCount;

        
        public FileCompareFile_Form()
        {
            InitializeComponent();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFile(textBox1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFile(textBox2);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            label_tip.Visible = true;
            label_tip.Text = "文件后台解析中......";
            if (string.IsNullOrEmpty(textBox1.Text)||string.IsNullOrEmpty(textBox2.Text))
            {
                MessageBox.Show("文件一或文件二为空，请重新选择!");
            }
            else
            {
                if (!MD5.VerifyMD5Hash(textBox1.Text, textBox2.Text))
                {
                    try
                    {
                        var doc1 = RevitCoreContext.Instance.Application.OpenDocumentFile(textBox1.Text);
                        var doc2 = RevitCoreContext.Instance.Application.OpenDocumentFile(textBox2.Text);
                        if (doc1 == null || doc2 == null)
                            throw new InvalidOperationException();
                        label_tip.Visible = false;
                        showProgressBar(true);
                        List<Data> datas;
                        compareFile(doc1, doc2, out datas);
                        dataGridView1.DataSource = datas;
                        showResult();
                        doc1.Close(false);
                        doc2.Close(false);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    
                }
                
            }
        }

        private void showResult()
        {
            double pecent = (SameCount / FileCount) * 100;
            lbl_result.Text = string.Format("结果：抽样样本为{0}个，相同元素数量{1}个，相似度为{2}%", FileCount, SameCount, pecent);
        }

        /// <summary>
        /// 打开revit文件
        /// </summary>
        /// <param name="textBox"></param>
        private void openFile(TextBox textBox)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Revit文件|*.rvt";
            if(ofd.ShowDialog()==DialogResult.OK)
            {
                textBox.Text = ofd.FileName;
            }
        }
        /// <summary>
        /// 初始化进度条
        /// </summary>
        private void initialProgressBar()
        {
            //设置进度条基础属性
            this.progressBar1.Value = 0;
            this.progressBar1.Style = ProgressBarStyle.Blocks;
            this.progressBar1.Maximum = FileCount;
            this.progressBar1.Minimum = 0;
            this.progressBar1.MarqueeAnimationSpeed = 100;
            this.progressBar1.Step = 1;
            this.lbl_progress.Text = "0%";
            this.lbl_progress.Refresh();
            showProgressBar(false);
        }
        /// <summary>
        /// 是否显示进度条
        /// </summary>
        /// <param name="bl"></param>
        private void showProgressBar(bool bl)
        {
            progressBar1.Visible = bl;
            label3.Visible = bl;
            //lbl_progress.Visible = bl;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 哈希对比两个文件
        /// </summary>
        /// <param name="path1"></param>
        /// <param name="path2"></param>
        /// <returns></returns>
        private bool isSameFile(string path1,string path2)
        {
            //计算第一个文件的哈希值
            var hash = System.Security.Cryptography.HashAlgorithm.Create();
            var stream_1 = new System.IO.FileStream(path1, System.IO.FileMode.Open);
            byte[] hashByte_1 = hash.ComputeHash(stream_1);
            stream_1.Close();
            //计算第二个文件的哈希值
            var stream_2 = new System.IO.FileStream(path2, System.IO.FileMode.Open);
            byte[] hashByte_2 = hash.ComputeHash(stream_2);
            stream_2.Close();

            //比较两个哈希值
            if (BitConverter.ToString(hashByte_1) == BitConverter.ToString(hashByte_2))
                return true;
            else
                return false;

        }
        /// <summary>
        /// 文件对比
        /// </summary>
        /// <param name="doc1">主文件</param>
        /// <param name="doc2">对比文件</param>
        /// <param name="datas">返回的数据列表</param>
        private void compareFile(RVDB.Document doc1,RVDB.Document doc2, out List<Data> datas)
        {
            progressBar1.Value = 0;
            progressBar1.Maximum = 0;
            SameCount = 0;
            FileCount = 0;
            datas = new List<Data>();
            compareElemType(doc1, doc2, datas,typeof(RVDB.Wall));
            compareElemType(doc1, doc2, datas, typeof(RVDB.Floor));
            compareElemType(doc1, doc2, datas, typeof(RVDB.FamilyInstance));
            compareElemType(doc1, doc2, datas, typeof(RVDB.ModelText));
            compareElemType(doc1, doc2, datas, typeof(RVDB.Dimension));
            compareElemType(doc1, doc2, datas, typeof(RVDB.Material));
            compareElemType(doc1, doc2, datas, typeof(RVDB.MEPCurve));
        }

        private void compareElemType(RVDB.Document doc1, RVDB.Document doc2, List<Data> datas, Type type)
        {
            var elems1 = new RVDB.FilteredElementCollector(doc1).OfClass(type).WhereElementIsNotElementType().ToList();
            int elemCount = (elems1.Count > 20) ? 20 : elems1.Count;
            progressBar1.Maximum += elemCount;
            FileCount = progressBar1.Maximum;
            for (int i = 0; i < elemCount; i++)
            {
                try
                {
                    var elem1 = elems1[i];
                    var elem2 = doc2.GetElement(elem1.UniqueId);
                    string isExist = "否";
                    if(elem2!=null&&elem1.LevelId==elem2.LevelId)
                    {
                        isExist = "是";
                        SameCount++;
                        datas.Add(new Data()
                        {
                            主文件 = "文件一",
                            元素名称 = elem1.Name,
                            元素UniqueID = elem1.UniqueId.ToString(),
                            标高名称 = doc1.GetElement(elem1.LevelId).Name,
                            标高ID = elem1.LevelId.ToString(),
                            对比文件是否存在 = isExist
                        });
                    }
                    
                }
                catch
                {
                }

                //设置进度条
                progressBar1.Value++;
                //lbl_progress.Text = (progressBar1.Value / progressBar1.Value * 100).ToString() + "%";
                progressBar1.Refresh();
                //lbl_progress.Refresh();
                
            }
        }
    }
    /// <summary>
    /// 填充的数据类
    /// </summary>
    public class Data
    {
        public string 主文件 { get; set; }
        public string 元素名称 { get; set; }
        public string  元素UniqueID { get; set; }
        public string 标高名称 { get; set; }
        public string 标高ID { get; set; }
        public string 对比文件是否存在 { get; set; }
    } 
    
}
