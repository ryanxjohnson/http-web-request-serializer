using System;
using System.Windows.Forms;
using HttpWebRequestSerializer;
using HttpWebRequestSerializer.Extensions;

namespace RequestMaker
{
    public partial class R : Form
    {
        public R()
        {
            InitializeComponent();
        }

        private void btnMakeRequest_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.Text))
                webBrowser1.DocumentText = textBox1.Text.ParseHeaders().BuildBaseHttpWebRequest().GetResponseString();
            else
            {
                MessageBox.Show("Paste in headers separated by new line");
            }
        }
    }
}
