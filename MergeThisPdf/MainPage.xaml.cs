using CommunityToolkit.Maui.Storage;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.Collections.ObjectModel;

namespace MergeThisPdf
{
    public partial class MainPage : ContentPage
    {
        private ObservableCollection<string> pdfFiles;
        public readonly IFileSaver fileSaver;

        public MainPage(IFileSaver fileSaver)
        {
            InitializeComponent();

            this.fileSaver = fileSaver;
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
            if (pdfFiles.Count < 2)
            {
                await DisplayAlert("Error", "Not enough PDF Files selected (at least 2).", "Ok");
                return;
            }

            try
            {
                PdfDocument mergedDocument = MergeDocuments(pdfFiles);

                using var stream = new MemoryStream();
                mergedDocument.Save(stream);

                stream.Position = 0;

                var result = await fileSaver.SaveAsync("MergedDocuments.pdf", stream);

                if (result.IsSuccessful)
                {
                    pdfFiles.Clear();
                    await DisplayAlert("Success", "The PDF has been saved successfully.", "Ok");
                }
                else
                {
                    await DisplayAlert("Error", "Failed to save the PDF file.", "Ok");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Something went wrong: {ex.Message}", "Ok");
                System.Diagnostics.Debug.WriteLine($"Exception: {ex}");
            }
        }

        private PdfDocument MergeDocuments(ObservableCollection<string> pdfFiles)
        {
            PdfDocument mergedDocument = new PdfDocument();

            foreach (var file in pdfFiles)
            {
                PdfDocument inputDocument = PdfReader.Open(file, PdfDocumentOpenMode.Import);
                for (int i = 0; i < inputDocument.PageCount; i++)
                {
                    PdfPage page = inputDocument.Pages[i];
                    mergedDocument.Pages.Add(page);
                }
            }

            return mergedDocument;
        }
    }
}
