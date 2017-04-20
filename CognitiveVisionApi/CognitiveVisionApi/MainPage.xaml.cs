using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using Newtonsoft.Json.Linq;
using Plugin.Connectivity;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CognitiveVisionApi
{
    public partial class MainPage : ContentPage
    {
        private VisionServiceClient visionClient;

        public MainPage()
        {
            InitializeComponent();
            visionClient = new VisionServiceClient("Your Vision API Key Goes Here");
        }

        private async void TakePictureButton_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", "No camera available.", "OK");
                return;
            }
            var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                SaveToAlbum = false,
                CompressionQuality = 50,
                Name = "test.jpg"
            });
            if (file == null)
                return;
            await AnalyzeAndProcessPhoto(file);
        }
        

        private async void UploadPictureButton_Clicked(object sender, EventArgs e)
        {
            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await DisplayAlert("No upload", "Picking a photo is not supported.", "OK");
                return;
            }
            var file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
            {
                CompressionQuality = 50
            });
            if (file == null)
                return;

            await AnalyzeAndProcessPhoto(file);
        }


        private async Task AnalyzeAndProcessPhoto(MediaFile file)
        {
            Indicator1.IsVisible = true;
            Indicator1.IsRunning = true;
            Image1.Source = ImageSource.FromStream(() => file.GetStream());
            var analysisResult = await AnalyzePictureAsync(file.GetStream());
            BindingContext = analysisResult;
            tags.IsVisible = true;

            //Text analysis
            if (analysisResult.Tags.Any(t => t.Name.ToLower() == "text"))
            {
                OcrResults ocrResult = await AnalyzePictureTextAsync(file.GetStream());
                pnlOcr.IsVisible = true;
                lblAngel.Text = $"Text Angel: {ocrResult.TextAngle.ToString()}";
                lblLanguage.Text = $"Language: {ocrResult.Language}";
                PopulateUIWithRegions(ocrResult);
            }
            else
            {
                pnlOcr.IsVisible = false;
            }

            //Detect Celebrity
            AnalysisInDomainResult analysisDomain = await AnalyzePictureDomainAsync(file.GetStream());
            CelebrityName.Text = ParseCelebrityName(analysisDomain.Result);

            Indicator1.IsRunning = false;
            Indicator1.IsVisible = false;
        }

        private async Task<OcrResults> AnalyzePictureTextAsync(Stream inputFile)
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                await DisplayAlert("Network error",
                  "Please check your network connection and retry.", "OK");
                return null;
            }
            OcrResults ocrResult = await visionClient.RecognizeTextAsync(inputFile);
            return ocrResult;
        }

        private async Task<AnalysisResult> AnalyzePictureAsync(Stream inputFile)
        {
            // Use the connectivity plug-in to detect
            // if a network connection is available
            // Remember using Plugin.Connectivity directive
            if (!CrossConnectivity.Current.IsConnected)
            {
                await DisplayAlert("Network error",
                  "Please check your network connection and retry.", "OK");
                return null;
            }
            VisualFeature[] visualFeatures = new VisualFeature[] { VisualFeature.Adult,
                    VisualFeature.Categories, VisualFeature.Color, VisualFeature.Description,
                    VisualFeature.Faces, VisualFeature.ImageType, VisualFeature.Tags };
            AnalysisResult analysisResult =
              await visionClient.AnalyzeImageAsync(inputFile,
              visualFeatures);

            
            return analysisResult;
        }

        private void PopulateUIWithRegions(OcrResults ocrResult)
        {
            for (int i = 0; i < DetectedText.Children.Count; i++)
            {
                DetectedText.Children.RemoveAt(i);
            }
            // Iterate the regions
            foreach (var region in ocrResult.Regions)
            {
                // Iterate lines per region
                foreach (var line in region.Lines)
                {
                    // For each line, add a panel
                    // to present words horizontally
                    var lineStack = new StackLayout
                    { Orientation = StackOrientation.Horizontal };
                    // Iterate words per line and add the word
                    // to the StackLayout
                    foreach (var word in line.Words)
                    {
                        var textLabel = new Label { Text = word.Text };
                        lineStack.Children.Add(textLabel);
                    }
                    // Add the StackLayout to the UI
                    DetectedText.Children.Add(lineStack);
                }
            }
        }

        private string ParseCelebrityName(object analysisResult)
        {
            JObject parsedJSONresult = JObject.Parse(analysisResult.ToString());
            var celebrities = from celebrity in parsedJSONresult["celebrities"]
                              select (string)celebrity["name"];
            return celebrities.FirstOrDefault();
        }

        private async Task<Model> GetDomainModel()
        {
            ModelResult modelResult = await visionClient.ListModelsAsync();
            // At this writing, only celebrity recognition
            // is available. It is the first model in the list
            return modelResult.Models.First();
        }


        private async Task<AnalysisInDomainResult> AnalyzePictureDomainAsync(Stream inputFile)
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                await DisplayAlert("Network error",
                  "Please check your network connection and retry.", "OK");
                return null;
            }
            AnalysisInDomainResult analysisResult =
              await visionClient.AnalyzeImageInDomainAsync(inputFile, await GetDomainModel());
            return analysisResult;
        }


        
    }
}
