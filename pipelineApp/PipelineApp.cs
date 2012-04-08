using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using pipelineLibrary;

namespace pipelineApp
{
    public partial class PipelineApp : Form
    {
        public PipelineApp()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDlg = new OpenFileDialog())
            {
                openFileDlg.InitialDirectory = System.Environment.CurrentDirectory.ToString();
                if (openFileDlg.ShowDialog() == DialogResult.OK)
                {
                    this.textBox1.Text = openFileDlg.FileName;
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDlg = new OpenFileDialog())
            {
                if (openFileDlg.ShowDialog() == DialogResult.OK)
                {
                    this.textBox2.Text = openFileDlg.FileName;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text)) {
                label4.ForeColor=Color.Red;
                label4.Text="没有输入文件不是好孩纸";
                label4.Visible=true;
            }
            if (string.IsNullOrEmpty(textBox2.Text)) {
                label4.ForeColor=Color.Red;
                label4.Text="没有输出文件不是好孩纸";
                label4.Visible=true;
            }
            try {
                Pipeline c=new Pipeline();
                c.Init(textBox1.Text, textBox2.Text);
                c.Run();
                label4.Text="已完成 ^_^";
                label4.ForeColor=Color.Black;
                label4.Visible=true;
                System.Diagnostics.Process.Start("notepad.exe ", textBox2.Text);

            }
            catch(Exception exc) 
            {
                label4.ForeColor=Color.Red;
                label4.Text="出错了肿么办 "+exc.Message;
                label4.Visible=true;
           
            }
        }
    }
}
