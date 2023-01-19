using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using MySqlConnector;

namespace ReGUID
{
    public partial class ConnectionForm : Form
    {
        private MySqlConnection WorldDBConnection, CharDBConnection;

        public ConnectionForm()
        {
            InitializeComponent();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            try
            {
                Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                configuration.AppSettings.Settings["User"].Value = textBoxUsername.Text;
                configuration.AppSettings.Settings["Password"].Value = textBoxPassword.Text;
                configuration.AppSettings.Settings["ServerHost"].Value = textBoxHost.Text;
                configuration.AppSettings.Settings["WorldDatabase"].Value = textBoxWorldDatabase.Text;
                configuration.AppSettings.Settings["CharDatabase"].Value = textBoxCharacterDatabase.Text;
                configuration.AppSettings.Settings["Port"].Value = textBoxPort.Text;
                configuration.Save();
                ConfigurationManager.RefreshSection("appSettings");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while saving configuration. Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            finally
            {
                MessageBox.Show("Configuration file updated successfully", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ConnectionForm_Load(object sender, EventArgs e)
        {
            textBoxUsername.Text = ConfigurationManager.AppSettings["User"];
            textBoxPassword.Text = ConfigurationManager.AppSettings["Password"];
            textBoxHost.Text = ConfigurationManager.AppSettings["ServerHost"];
            textBoxWorldDatabase.Text = ConfigurationManager.AppSettings["WorldDatabase"];
            textBoxCharacterDatabase.Text = ConfigurationManager.AppSettings["CharDatabase"];
            textBoxPort.Text = ConfigurationManager.AppSettings["Port"].Length == 0 ? textBoxPort.Text : ConfigurationManager.AppSettings["Port"];
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxHost.Text))
            {
                MessageBox.Show("Please specify the host", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(textBoxWorldDatabase.Text))
            {
                MessageBox.Show("Please specify the host database", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (WorldDBConnection != null)
                WorldDBConnection.Close();

            if (CharDBConnection != null)
                CharDBConnection.Close();

            string server = textBoxHost.Text;
            string user = textBoxUsername.Text;
            string password = textBoxPassword.Text;
            string WorldDatabase = textBoxWorldDatabase.Text;
            string CharacterDatabase = textBoxCharacterDatabase.Text;
            string Port = textBoxPort.Text;
            // Create Connection Objects
            string worldDBConnectionString = string.Format(
                "server=" + server +
                ";user=" + user +
                "; Convert Zero Datetime=True; Persist Security Info=True; Allow User Variables=True; " +
                "password=" + password +
                "; database=" + WorldDatabase + "; pooling=true; port=" + Port + ";");

            string charDBConnectionString = string.Format(
                "server=" + server +
                ";user=" + user +
                "; Convert Zero Datetime=True; Persist Security Info=True; Allow User Variables=True;" +
                "password=" + password +
                "; database=" + CharacterDatabase + "; pooling=true; port=" + Port + ";");

            try
            {
                WorldDBConnection = new MySqlConnection(worldDBConnectionString);
                WorldDBConnection.Open();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Could not connect to World MySQL database. \nError: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                CharDBConnection = new MySqlConnection(charDBConnectionString);
                CharDBConnection.Open();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Could not connect to World MySQL database. \nError: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            MainForm mainForm = new MainForm(WorldDBConnection, CharDBConnection);
            mainForm.Show();
            Hide();
        }
    }
}
