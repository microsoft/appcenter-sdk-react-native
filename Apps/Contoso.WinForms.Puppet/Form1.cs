using System.Windows.Forms;

namespace Contoso.WinForms.Puppet
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, System.EventArgs e)
        {

        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            var x = 42 / int.Parse("0");
            button1.Text = x.ToString();
        }
    }
}
