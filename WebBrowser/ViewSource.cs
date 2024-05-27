using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WebBrowser
{
    public partial class ViewSource : Form
    {
        public ViewSource()
        {
            InitializeComponent();
        }
        private TextBox txtSource;

        public ViewSource(string htmlSource)
        {
            InitializeComponent();

            txtSource = new TextBox();
            txtSource.Multiline = true;
            txtSource.Dock = DockStyle.Fill;
            txtSource.ScrollBars = ScrollBars.Both;
            txtSource.ReadOnly = true;
            txtSource.Text = htmlSource;

            this.Controls.Add(txtSource);
            this.Text = "Source View";
            this.Size = new Size(800, 600);
        }
    }
}
