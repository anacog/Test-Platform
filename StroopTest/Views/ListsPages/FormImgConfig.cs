﻿using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TestPlatform.Models;
using TestPlatform.Views;
using TestPlatform.Controllers;

namespace TestPlatform
{
    public partial class FormImgConfig : UserControl
    {
        private ImageList imgsList = new ImageList();
        private string instructionsText = HelpData.ImageConfigInstructions;

        public FormImgConfig(string imgListEdit)
        {
            this.Dock = DockStyle.Fill;
            InitializeComponent();
            imgPathDataGridView.AllowDrop = true;
            imgPathDataGridView.RowTemplate.MinimumHeight = 120;
            
            labelEmpty.Visible = false;
            if (imgListEdit != "false")
            {
                openImgList();
            }
        }

        private void openImgList()
        {
            try
            {
                FormDefine defineFilePath = defineFilePath = new FormDefine("Lista de Imagens: ", Global.testFilesPath + Global.listFolderName, "lst","_image",true);
                var result = defineFilePath.ShowDialog();
                
                if (result == DialogResult.OK)
                {
                    string dir = defineFilePath.ReturnValue;
                    imgListNameTextBox.Text = dir.Remove(dir.Length - 6); // removes the _img identification from file while editing (when its saved it is always added again)

                    string[] filePaths = StroopProgram.readDirListFile(Global.testFilesPath + Global.listFolderName + "/" + dir + ".lst");
                    readImagesIntoDGV(filePaths, imgPathDataGridView);
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            openImagesDirectory();
        }

        // opens directory with images to be choosen by the list creator
        private void openImagesDirectory()
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Multiselect = true;
                openFileDialog.Filter = "Images (*.BMP;*.JPG;*.JPEG;*.GIF;*.PNG)|*.BMP;*.JPG;*.JPeG;*.GIF;*.PNG|" + "All files (*.*)|*.*";
                
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string[] filePaths = openFileDialog.FileNames;
                    readImagesIntoDGV(filePaths, imgPathDataGridView);
                    selectedImageIntoPictureBox();
                }
            }
            catch (NullReferenceException)
            {
            }
            catch (FileLoadException ex)
            {
                throw new Exception("Não pode apresentar a imagem. Você pode não ter permissão para ler este arquivo ou ele pode estar corrompido.\n" + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void readImagesIntoDGV(string[] directory, DataGridView imagesDataGridView)
        {
            DataGridView dgv = imagesDataGridView;
            try
            {
                foreach (string file in directory)
                {
                    Image image = Image.FromFile(file);
                    dgv.Rows.Add(Path.GetFileNameWithoutExtension(file), image, file);
                    ((DataGridViewImageColumn)dgv.Columns[1]).ImageLayout = DataGridViewImageCellLayout.Stretch;
                    numberFiles.Text = imgPathDataGridView.RowCount.ToString();
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        // delete button click - deletes row from not empty dgv; refresh view
        private void deleteRow_Click(object sender, EventArgs e)
        {
            DataGridView dgv = imgPathDataGridView;
            DGVManipulation.DeleteDGVRow(dgv);
            numberFiles.Text = imgPathDataGridView.RowCount.ToString();
            if(imgPathDataGridView.RowCount == 0)
            {
                pictureBox.Image = null;
                pictureBox.Refresh();
            }
        }

        private void imgPathDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            selectedImageIntoPictureBox();
        }

        private void imgPathDataGridView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            selectedImageIntoPictureBox();
        }

        private void imgPathDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            selectedImageIntoPictureBox();
        }

        // pictureBox receives clicked rows img
        private void selectedImageIntoPictureBox()
        {
            if(imgPathDataGridView.CurrentRow != null)
            {
                pictureBox.Image = Image.FromFile(imgPathDataGridView.CurrentRow.Cells[2].Value.ToString());
            }
            
        }
        
        // button up click - moves selected row Upper
        private void btnUp_Click(object sender, EventArgs e)
        {
            DataGridView dgv = imgPathDataGridView;
            try
            {
                DGVManipulation.MoveDGVRowUp(dgv);
                if (imgPathDataGridView.CurrentRow.Cells.Count > 0)
                {
                    pictureBox.Image = Image.FromFile(dgv.CurrentRow.Cells[2].Value.ToString());
                }
            }
            catch { }
        }

        // button up click - moves selected row Down
        private void btnDown_Click(object sender, EventArgs e)
        {
            DataGridView dgv = imgPathDataGridView;
            try
            {
                DGVManipulation.MoveDGVRowDown(dgv);
                pictureBox.Image = Image.FromFile(dgv.CurrentRow.Cells[2].Value.ToString());
            }
            catch { }
        }

        // saves list into .lst file inside lst directory
        private void saveButton_Click(object sender, EventArgs e)
        {
            if (!this.ValidateChildren(ValidationConstraints.Enabled))
                MessageBox.Show("Algum campo não foi preenchido de forma correta.");
            else
            {
                try
                {
                    DataGridView dgv = imgPathDataGridView;
                    DGVManipulation.SaveColumnToListFile(dgv, 2, Global.testFilesPath + Global.listFolderName, imgListNameTextBox.Text + "_image");
                    this.Parent.Controls.Remove(this);
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DataGridView dgv = imgPathDataGridView;
            AutoValidate = AutoValidate.Disable;
            try
            {
                DGVManipulation.CloseFormListNotEmpty(dgv);
                this.Parent.Controls.Remove(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.Parent.Controls.Remove(this);
            }
        }

        private void imgPathDataGridView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Left)
            {
                DataGridView dgv = imgPathDataGridView;
                try
                {
                    DGVManipulation.MoveDGVRowUp(dgv);
                }
                catch { }
            }
            if (e.Control && e.KeyCode == Keys.Right)
            {
                DataGridView dgv = imgPathDataGridView;
                try
                {
                    DGVManipulation.MoveDGVRowDown(dgv);
                }
                catch { }
            }
        }

        private void imgPathDataGridView_CellStateChanged(object sender, DataGridViewCellStateChangedEventArgs e)
        {
            DataGridView dgv = imgPathDataGridView;
            if (dgv.RowCount <= 1)
            {
                pictureBox.Image = null;
                pictureBox.Refresh();
            }
        }

        private void listName_Validating(object sender,
                             System.ComponentModel.CancelEventArgs e)
        {
            string errorMsg;
            if (!ValidListName(imgListNameTextBox.Text, out errorMsg))
            {
                e.Cancel = true;
                this.errorProvider1.SetError(this.imgListNameTextBox, errorMsg);
            }
        }

        private void listName_Validated(object sender, System.EventArgs e)
        {
            errorProvider1.SetError(this.imgListNameTextBox, "");
        }

        public bool ValidListName(string name, out string errorMessage)
        {
            if (Validations.isEmpty(name))
            {
                errorMessage = "O nome da lista deve ser preenchido";
                return false;
            }

            errorMessage = "";
            return true;
        }

        private void listLength_Validated(object sender, System.EventArgs e)
        {
            labelEmpty.Visible = false;
        }

        public bool ValidListLength(int number, out string errorMessage)
        {
            if (number == 0)
            {
                errorMessage = "A lista não possui \n nenhum item!";
                return false;
            }

            errorMessage = "";
            return true;
        }

        private void listLength_Validating(object sender,
                             System.ComponentModel.CancelEventArgs e)
        {
            string errorMsg;
            if (!ValidListLength(imgPathDataGridView.RowCount, out errorMsg))
            {
                e.Cancel = true;
                labelEmpty.Text = errorMsg;
                labelEmpty.Visible = true;
            }
        }

        private void helpButton_Click(object sender, EventArgs e)
        {
            FormInstructions infoBox = new FormInstructions(instructionsText);
            try { infoBox.Show(); }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
    }
}
