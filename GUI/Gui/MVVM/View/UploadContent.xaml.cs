using System;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Threading.Tasks;
using CloudinaryDotNet;
using Gui.Core;
using Microsoft.Win32;

namespace Gui.MVVM.View {
    /// <summary>
    /// Interaction logic for UploadContent.xaml
    /// </summary>
    public partial class UploadContent : UserControl {
        public UploadContent() {
            InitializeComponent();
            cloudinary = new Cloudinary();
        }

        private Cloudinary cloudinary;
        private string _targetFile;
        private bool _uploading = false;

        private void SelectFileBtn_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter =
                "Word Documents|*.doc|Excel Worksheets|*.xls,*.xlsx|PowerPoint Presentations|*.ppt,*.ppts" +
                "|Office Files|*.doc;*.xls;*.ppt;*.pdf;*.docx;" +
                "|Web files|*.html,*.xml" +
                "|Text files|*.txt";

            if (openFileDialog.ShowDialog() == true) {
                SelectFileButton.Content = "Change File";
                FileName.Text = Path.GetFileName(openFileDialog.FileName);
                _targetFile = openFileDialog.FileName;
            }
            else {
                FileName.Text = "No file selected";
            }
        }

        private async void UploadFileBtn_Click(object sender, RoutedEventArgs e) {
            if (_uploading) return;
            
            if(String.IsNullOrWhiteSpace(DocumentTitle.Text)) return;

            if (_targetFile == null) return;

            _uploading = true;
            UploadFileBtn.Content = "Uploading...";
            UploadFileBtnWrapper.Width = 200;
            var url = await UploadFile.Upload(_targetFile);
            UploadFileBtnWrapper.Width = 120;
            UploadFileBtn.Content = "Upload";
            _uploading = false;

            MessageBox.Show("Your document has been queued for indexing");
            try {
                var title = DocumentTitle.Text;
                await Task.Run(() => Engine.DbDocument.IndexDocument(title, url));
            }
            catch (Exception ss) {
               Console.WriteLine(ss.Message); 
            }
        }
    }
}