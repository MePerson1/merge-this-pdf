﻿using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.Collections.ObjectModel;
namespace MergeThisPdf
{
    public partial class MainPage : ContentPage
    {
        private ObservableCollection<string> pdfFiles;

        public MainPage()
        {
            InitializeComponent();
            pdfFiles = new ObservableCollection<string>();
            pdfFilesListView.ItemsSource = pdfFiles;
        }

        private async void OnAddClicked(object sender, EventArgs e)
        {
            var result = await FilePicker.PickMultipleAsync(new PickOptions
            {
                PickerTitle = "Pick Pdf",
                FileTypes = FilePickerFileType.Pdf,
            });

            if (result is null)
            {
                await DisplayAlert("Error", "Wrong file picked.", "Ok");
                return;
            }

            foreach (var file in result)
            {
                if (file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase) && !pdfFiles.Contains(file.FullPath))
                {
                    pdfFiles.Add(file.FullPath);
                }
            }

        }

        private void OnClearClicked(object sender, EventArgs e)
        {
            pdfFiles.Clear();
        }

        private async void OnMergeClicked(object sender, EventArgs e)
        {
            if (pdfFiles.Count == 0)
            {
                await DisplayAlert("Error", "No PDF Filest selected.", "Ok");
            }

            PdfDocument mergedDocument = new PdfDocument();

            foreach (var file in pdfFiles)
            {
                PdfDocument inputDocument = PdfReader.Open(file, PdfDocumentOpenMode.Import);

            }
        }

    }

}