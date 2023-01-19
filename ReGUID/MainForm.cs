using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MySqlConnector;
using System.Threading;

namespace ReGUID
{
    public partial class MainForm : Form
    {
        int maxCreatureCount, maxGUID;
        int progressCount;
        decimal percentComplete;
        MySqlConnection worldDBConnection, charDBConnection;
        public MainForm(MySqlConnection worldDbConn, MySqlConnection charDbConn)
        {
            worldDBConnection = worldDbConn;
            charDBConnection = charDbConn;
            percentComplete = 0;
            maxCreatureCount = 0;
            maxGUID = 0;
            progressCount = 0;
            backgroundWorker1 = new BackgroundWorker();
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
            InitializeComponent();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            buttonStart.Enabled = false;
            checkBoxHaltOnError.Enabled = false;
            progressBar1.Value = progressBar1.Minimum;
            labelFixedCount.Text = "0";
            if (backgroundWorker1.IsBusy != true)
            {
                // Start the asynchronous operation.
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void UpdateListBox(string text)
        {
            if (listBox1.InvokeRequired)
            {
                listBox1.Invoke((MethodInvoker)delegate { UpdateListBox(text); });
            }
            else
            {
                listBox1.Items.Add(text);
                if (listBox1.Items.Count % 1000 == 0) // Prevents stuttering the control
                    listBox1.SetSelected(listBox1.Items.Count - 1, true);
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            // Create background worker object
            BackgroundWorker worker = sender as BackgroundWorker;
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;

            // pick guids from table
            MySqlCommand selectGUIDsCommand = new MySqlCommand("SELECT guid FROM `creature` ORDER BY guid ASC;", worldDBConnection);
            MySqlDataReader dataReader;
            dataReader = selectGUIDsCommand.ExecuteReader();
            List<int> guids = new List<int>();
            while (dataReader.Read())
            {
                guids.Add(dataReader.GetInt32("guid"));
            }

            dataReader.Close();

            /*
             * Handle indexation to speed up table lookups.
             * Gotta thank MySQL for not supporting DROP INDEX IF EXISTS....
             */
            string[] dropIndexes = { "ReGUIDCondition", "ReGUIDSourceType", "ReGUIDSAI", "ReGUIDLinked", "ReGUIDCreature", "ReGUIDVehicle" };
            foreach (string index in dropIndexes)
            {
                string tableName = "";
                MySqlCommand IndexVerifyCommand = new MySqlCommand("SELECT COUNT(1) 'IndexPresentCount' FROM INFORMATION_SCHEMA.STATISTICS " +
                "WHERE table_schema = DATABASE() AND index_name LIKE '%" + index + "%';", worldDBConnection);
                dataReader = IndexVerifyCommand.ExecuteReader();
                if (dataReader.Read())
                {
                    if (dataReader.GetInt32("IndexPresentCount") > 0)
                    {
                        switch (index)
                        {
                            case "ReGUIDCondition":
                            case "ReGUIDSourceType":
                                tableName = "conditions";
                                break;
                            case "ReGUIDSAI":
                                tableName = "smart_scripts";
                                break;
                            case "ReGUIDLinked":
                                tableName = "creature_linked_respawn";
                                break;
                            case "ReGUIDCreature":
                                tableName = "creature";
                                break;
                            case "ReGUIDVehicle":
                                tableName = "vehicle_accessory";
                                break;
                        }

                        dataReader.Close();
                        MySqlCommand DeleteIndexCommand = new MySqlCommand("DROP INDEX " + index + " ON " + tableName + ";", worldDBConnection);
                        try
                        {
                            DeleteIndexCommand.ExecuteNonQuery();
                        }
                        catch (MySqlException ex)
                        {
                            MessageBox.Show("Could not delete previously Added WorldDB ReGUID indexes (schema issue)? Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            worker.CancelAsync();
                            e.Cancel = true;
                            return;
                        }
                    }
                }
                dataReader.Close();
            }


            // Any previously created indexes should be gone now
            MySqlCommand CreateIndexCommand = new MySqlCommand(
                "CREATE INDEX ReGUIDCondition ON conditions (ConditionTypeOrReference,ConditionValue3);" +
                "CREATE INDEX ReGUIDSourceType ON conditions(SourceTypeOrReferenceId, SourceEntry);" +
                "CREATE INDEX ReGUIDSAI ON smart_scripts(target_type, target_param1);" +
                "CREATE INDEX REGUIDLinked ON creature_linked_respawn(linkedGuid);" +
                "CREATE INDEX REGUIDCreature ON creature(id);" +
                "CREATE INDEX REGUIDVehicle ON vehicle_accessory(guid);", worldDBConnection);

            try
            {
                CreateIndexCommand.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error creating ReGUID index on World DB. Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                worker.CancelAsync();
                return;
            }

            // Character database index removal
            MySqlCommand IndexVerifyCommandCharacter = new MySqlCommand("SELECT COUNT(1) 'IndexPresentCount' FROM INFORMATION_SCHEMA.STATISTICS " +
            "WHERE table_schema = DATABASE() AND index_name LIKE '%ReGUID%';", charDBConnection);

            MySqlDataReader readerChar =  IndexVerifyCommandCharacter.ExecuteReader();
            if (readerChar.Read())
            {
                // Delete the Indexes
                if (readerChar.GetInt32("IndexPresentCount") > 0)
                {
                    readerChar.Close();
                    MySqlCommand DeleteIndexCommand = new MySqlCommand("DROP INDEX ReGUIDAuction ON auctionhouse;", charDBConnection);
                    try
                    {
                        DeleteIndexCommand.ExecuteNonQuery();
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show("Could not delete previously Added ReGUID indexes (schema issue)? Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        worker.CancelAsync();
                        e.Cancel = true;
                        return;
                    }
                }
            }

            MySqlCommand charCommand = new MySqlCommand("CREATE INDEX ReGUIDAuction ON auctionhouse (auctioneerGuid);", charDBConnection);

            try
            {
                charCommand.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error creating ReGUID index on Char DB. Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                worker.CancelAsync();
                e.Cancel = true;
                return;
            }

            MySqlCommand UpdateCommandWorld = new MySqlCommand("", worldDBConnection);
            MySqlCommand UpdateCommandChar = new MySqlCommand("", charDBConnection);

            int newGuid = 1;
            bool HaltOnError = checkBoxHaltOnError.Checked;

            foreach (int guid in guids)
            {
                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    return;
                }

                object[] arguments = { newGuid, guid };
                if (newGuid == guid)
                {
                    UpdateListBox("Skipping GUID alteration for " + guid.ToString() + ". Value is ok. \n");
                    newGuid++;
                    if (++progressCount % 100 == 0)
                        worker.ReportProgress(100);
                    continue;
                }
                string updateString = string.Format(
                    "UPDATE `creature` SET `guid` = '{0}' WHERE `guid` = {1}; \n" +
                    "UPDATE `creature_addon` SET `guid` = '{0}' WHERE `guid` = {1}; \n" +
                    "UPDATE `game_event_creature` SET `guid` = '{0}' WHERE `guid` = {1}; \n" +
                    "UPDATE `pool_creature` SET `guid` = '{0}' WHERE `guid` = {1}; \n" +
                    "UPDATE `creature_formations` SET `leaderGUID` = '{0}' WHERE `leaderGUID` = {1}; \n" +
                    "UPDATE `creature_formations` SET `memberGUID` = '{0}' WHERE `memberGUID` = {1}; \n" +
                    "UPDATE `conditions` SET `conditionValue3` = '{0}', `conditionValue2` = (SELECT DISTINCT id from `creature` WHERE guid = {0}) WHERE `ConditionTypeOrReference` = 31 AND `conditionValue3` = {1}; \n" +
                    "UPDATE `conditions` SET `SourceEntry` = '{0}' WHERE `SourceTypeOrReferenceId` = 22 AND `SourceEntry` = -{1}; \n" +
                    "UPDATE `smart_scripts` SET `entryorguid` = '-{0}' WHERE `entryorguid` = -{1}; \n" +
                    "UPDATE `smart_scripts` SET `target_param1` = '{0}' WHERE `target_param1` = {1} AND target_type = 10; \n" +
                    "UPDATE `creature_linked_respawn` SET `guid` = '{0}' WHERE `guid` = {1}; \n" +
                    "UPDATE `creature_linked_respawn` SET `linkedGuid` = {0} WHERE `linkedGuid` = {1}; \n" +
                    "UPDATE `vehicle_accessory` SET `guid` = {0} WHERE `guid` = {1}; \n"
                    , arguments);

                string charUpdateString = string.Format(
                    "UPDATE `auctionhouse` SET `auctioneerGuid` = {0} WHERE `auctioneerGuid` = {1}; \n", arguments);

                UpdateCommandWorld.CommandText = updateString;
                UpdateCommandChar.CommandText = charUpdateString;

                UpdateListBox("Updated GUID " + guid.ToString() + " value is now " + newGuid.ToString());

                try
                {
                    UpdateCommandWorld.ExecuteNonQuery();
                }
                catch (MySqlException ex)
                {
                    if (HaltOnError)
                    {
                        DialogResult result = MessageBox.Show("World database update error. \n Error: " + ex.Message + "\n Continue?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (result == DialogResult.Yes)
                            continue;
                        else
                        {
                            worker.CancelAsync();
                            e.Cancel = true;
                            return;
                        }
                    }
                    UpdateListBox(ex.Message + "\n");
                }

                UpdateListBox(charUpdateString);

                try
                {
                    charCommand.ExecuteNonQuery();
                }
                catch (MySqlException ex)
                {
                    if (HaltOnError)
                    {
                        DialogResult result = MessageBox.Show("Character database update error. \n Error: " + ex.Message + "\n Continue?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (result == DialogResult.Yes)
                            continue;
                        else
                        {
                            worker.CancelAsync();
                            e.Cancel = true;
                            return;
                        }
                    }
                    UpdateListBox(ex.Message + "\n");
                }

                // Should be done for better error message handling?
                #region SplitQueries
                //updateString = string.Format("UPDATE `creature_addon` SET `guid` = '{0}' WHERE `guid` = {1};", arguments);
                //command = new MySqlCommand(updateString, localConnection);

                //try
                //{
                //    command.ExecuteNonQuery();
                //}
                //catch (MySqlException ex)
                //{
                //    MessageBox.Show("Creature addon error" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    continue;
                //}

                //updateString = string.Format("UPDATE `game_event_creature` SET `guid` = '{0}' WHERE `guid` = {1};", arguments);
                //command = new MySqlCommand(updateString, localConnection);

                //try
                //{
                //    command.ExecuteNonQuery();
                //}
                //catch (MySqlException ex)
                //{
                //    MessageBox.Show("Game event error. " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    continue;
                //}

                //updateString = string.Format("UPDATE `pool_creature` SET `guid` = '{0}' WHERE `guid` = {1};", arguments);
                //command = new MySqlCommand(updateString, localConnection);

                //try
                //{
                //    command.ExecuteNonQuery();
                //}
                //catch (MySqlException ex)
                //{
                //    MessageBox.Show("Pool creature error. " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    continue;
                //}

                //updateString = string.Format("UPDATE `creature_formations` SET `leaderGUID` = '{0}' WHERE `leaderGUID` = {1};", arguments);
                //command = new MySqlCommand(updateString, localConnection);

                //try
                //{
                //    command.ExecuteNonQuery();
                //}
                //catch (MySqlException ex)
                //{
                //    MessageBox.Show("Creature formations first error. " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    continue;
                //}

                //updateString = string.Format("UPDATE `creature_formations` SET `memberGUID` = '{0}' WHERE `memberGUID` = {1};", arguments);
                //command = new MySqlCommand(updateString, localConnection);

                //try
                //{
                //    command.ExecuteNonQuery();
                //}
                //catch (MySqlException ex)
                //{
                //    MessageBox.Show("Creature formations second error " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    continue;
                //}

                //updateString = string.Format("UPDATE `conditions` SET `conditionValue3` = '{0}' WHERE `conditionValue3` = {1};", arguments);
                //command = new MySqlCommand(updateString, localConnection);

                //try
                //{
                //    command.ExecuteNonQuery();
                //}
                //catch (MySqlException ex)
                //{
                //    MessageBox.Show("Conditions error " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    continue;
                //}

                //updateString = string.Format("UPDATE `smart_scripts` SET `entryorguid` = '-{0}' WHERE `entryorguid` = -{1};", arguments);
                //command = new MySqlCommand(updateString, localConnection);

                //try
                //{
                //    command.ExecuteNonQuery();
                //}
                //catch (MySqlException ex)
                //{
                //    MessageBox.Show("smart script first execute error " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    continue;
                //}

                //updateString = string.Format("UPDATE `smart_scripts` SET `target_param1` = '{0}' WHERE `target_param1` = {1} AND target_type = 10;", arguments);
                //command = new MySqlCommand(updateString, localConnection);

                //try
                //{
                //    command.ExecuteNonQuery();
                //}
                //catch (MySqlException ex)
                //{
                //    MessageBox.Show("smart script second execute error " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    continue;
                //}

                //updateString = string.Format("UPDATE `creature_linked_respawn` SET `guid` = '{0}' WHERE `guid` = {1}", arguments);
                //command = new MySqlCommand(updateString, localConnection);

                //try
                //{
                //    command.ExecuteNonQuery();
                //}
                //catch (MySqlException ex)
                //{
                //    MessageBox.Show("Linked respawn first execute error. " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    continue;
                //}

                //updateString = string.Format("UPDATE `creature_linked_respawn` SET `linkedGuid` = '{0}' WHERE `linkedGuid` = {1}", arguments);
                //command = new MySqlCommand(updateString, localConnection);

                //try
                //{
                //    command.ExecuteNonQuery();
                //}
                //catch (MySqlException ex)
                //{
                //    MessageBox.Show("Linked respawn second execute error. " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    continue;
                //}
                #endregion SplitQueries

                newGuid++;

                if (++progressCount % 100 == 0)
                    worker.ReportProgress(100);
            }

            UpdateCommandWorld = new MySqlCommand("SET @newAutoIncrement = " + (maxCreatureCount+1).ToString() + ";" +
                "SET @sql = CONCAT('ALTER TABLE `creature` AUTO_INCREMENT = ', @newAutoIncrement);" +
                "PREPARE st FROM @sql;" +
                "EXECUTE st;", worldDBConnection);
            try
            {
                UpdateCommandWorld.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                if (HaltOnError)
                {
                    MessageBox.Show("Error setting new AUTO_INCREMENT value on creature table. \nShould be MAX(guid) + 1 \nError: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                UpdateListBox(ex.Message + "\n");
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            percentComplete += (decimal)(100 * 100) / maxCreatureCount;
            progressBar1.Value = Math.Min((int)percentComplete, progressBar1.Maximum);
            labelFixedCount.Text = (int.Parse(labelFixedCount.Text) + 100).ToString();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                MessageBox.Show("Task Completed Successfully!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                labelFixedCount.Text = maxCreatureCount.ToString();
                progressBar1.Value = progressBar1.Maximum;
            }
            else
            {
                labelFixedCount.Text = "0";
                progressBar1.Value = progressBar1.Minimum;
            }
            FillLabelData();
            buttonStart.Enabled = true;
            checkBoxHaltOnError.Enabled = true;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.WorkerSupportsCancellation && !backgroundWorker1.CancellationPending)
                backgroundWorker1.CancelAsync();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form form = Application.OpenForms["ConnectionForm"];
            if (form != null)
                form.Show();
        }

        void FillLabelData()
        {
            MySqlCommand selectNomeCommand = new MySqlCommand("SELECT COUNT(*) as CreatureCount FROM `creature`;", worldDBConnection);
            MySqlCommand selectMaxGUIDCommand = new MySqlCommand("SELECT MAX(guid) as MaxGUID FROM `creature`;", worldDBConnection);
            MySqlDataReader dataReader;

            dataReader = selectNomeCommand.ExecuteReader();

            if (dataReader.Read())
                maxCreatureCount = dataReader.GetInt32("CreatureCount");

            dataReader.Close();

            dataReader = selectMaxGUIDCommand.ExecuteReader();

            if (dataReader.Read())
                maxGUID = dataReader.GetInt32("MaxGUID");

            dataReader.Close();

            labelHighestGuid.Text = maxGUID.ToString();
            labelTotalCreatures.Text = maxCreatureCount.ToString();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            FillLabelData();
        }
    }
}
