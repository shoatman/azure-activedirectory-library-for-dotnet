using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace WinFormsTestApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            //http://localhost/testappshane
            //client id: 2d50ea8a-850f-4351-8068-4927f92f65fd
            //resource: 00000002-0000-0000-c000-000000000000

            AuthenticationContext ac = new AuthenticationContext("https://login.microsoftonline.com/common");
            AuthenticationResult result = await ac.AcquireTokenAsync("00000002-0000-0000-c000-000000000000", "2d50ea8a-850f-4351-8068-4927f92f65fd", new Uri(@"http://localhost/testappshane"), new PlatformParameters(PromptBehavior.Auto, this));
        }
    }
}
