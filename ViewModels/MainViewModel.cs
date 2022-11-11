using ImageToPDFConverter.Commands;
using ImageToPDFConverter.Models;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace ImageToPDFConverter.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private string tempPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);


        private ObservableCollection<ImagesModel> images;
        public ObservableCollection<ImagesModel> Images
        {
            get => images;
            set => SetProperty(ref images, value);
        }

        private string outputPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public string OutputPath
        {
            get => outputPath;
            set => SetProperty(ref outputPath, value);
        }

        private string outputName = "New_PDF";
        public string OutputName
        {
            get => outputName;
            set => SetProperty(ref outputName, value);
        }

     


    

     
        public ICommand CreatePDFCommand { get; private set; }
        public ICommand BrowseImagesCommand { get; private set; }
        public ICommand SetOutputPathCommand { get; private set; }

        public MainViewModel()
        {
            images = new ObservableCollection<ImagesModel>();
            CreatePDFCommand = new ButtonCommand<MainViewModel>(() => OnExecuteCreatePDF());
            BrowseImagesCommand = new ButtonCommand<MainViewModel>(() => OnExecuteLoadImage());
            SetOutputPathCommand = new ButtonCommand<MainViewModel>(() => OnExecuteSetPDFOutputPath());
        }


        public void OnExecuteLoadImage()
        {
            OpenImageDialog();
        }
        public void OnExecuteCreatePDF()
        {
            CreatePDFFromImage();
        }
        public void OnExecuteSetPDFOutputPath()
        {
            SetOutPutPath();
        }

        private void SetOutPutPath()
        {
            var dialog = new FolderBrowserDialog();
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                OutputPath = dialog.SelectedPath;

            }
        }

        private void OpenImageDialog()
        {

            OpenFileDialog fileDialog = new OpenFileDialog
            {
                FileName = "Select images",
                Filter = "Image Files|*.jpg;*.jpeg;*.Bmp;*.png;*.gif;*.tif;...",
                Multiselect = true,
                InitialDirectory = tempPath

            };



            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                for (int i = 0; i < fileDialog.FileNames.Length; i++)
                {
                    string file = fileDialog.FileNames[i];
                    ImagesModel img = new ImagesModel
                    {
                        FileName = $"{Path.GetFileName($"{file}")}{Path.GetExtension($"{file}")}",
                        Uri = file,
                        Image = Path.GetFullPath(file),
                        Page = images.Count + 1
                    };

                    images.Add(img);

                }

                tempPath = Path.GetFullPath(Path.GetFullPath(fileDialog.FileName));
            }
        }

        private void CreatePDFFromImage()
        {
            PdfDocument pdfDocument = new PdfDocument();

            pdfDocument.Info.Title = outputName;
            int i = 0;

            if (images.Count < 1)
            {
                System.Windows.MessageBox.Show("Please add image(s) \nNo Images added!");
                return;
            }
            foreach (ImagesModel? img in Images)
            {
                //PdfPage page = pdfDocument.AddPage();
                //XGraphics gfx = XGraphics.FromPdfPage(page);
                //DrawImage(gfx, img.Uri, 0, 0, (int)page.Width, (int)page.Height);

                PdfPage page = pdfDocument.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);

                try
                {
                    XImage image = XImage.FromFile(img.Uri);
                    gfx.DrawImage(image, 0, 0, page.Width, page.Height);
                    i++;
                }
                catch (Exception e)
                {

                    if (System.Windows.MessageBox.Show(e.Message,
                     "Cancel process",
                     MessageBoxButton.YesNo,
                     MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        images.Clear();
                        break;
                    }
                }
               

               
            }

            if (pdfDocument.PageCount > 0)
            {
                try
                {
                    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                    string c = Path.Join(outputPath, OutputName);

                    SaveFileDialog saveFileDialog = new SaveFileDialog 
                    {
                        Filter = "PDF file (*.pdf)|*.pdf",
                    };
                        
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {

                        pdfDocument.Save($"{saveFileDialog.FileName}");
                        pdfDocument.Close();
                        System.Windows.MessageBox.Show($"{Path.GetFileName(saveFileDialog.FileName)}\ncreated successfully");
                    }
                      
                    
                }
                catch (Exception e)
                {

                    System.Windows.MessageBox.Show(e.Message);

                }

            }
        }

        //private static void DrawImage(XGraphics gfx, string imagePath, int x, int y, int width, int height)
        //{
        //    XImage image = XImage.FromFile(imagePath);
        //    gfx.DrawImage(image, x, y, width, height);
        //    gfx.DrawImage(image, new XRect(image.Size));
        //}
    }
}