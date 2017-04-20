# XamarinCognitiveVisionApi
Microsoft Cognitive Services/Vision API Xamarin Implementation for Native Android, IOS and UWP Windows Apps

This Project for most cases is the exact same implementation that is described and published in an [MSDN article Issued Back in Nov 2016](https://msdn.microsoft.com/en-us/magazine/mt788620.aspx) with tweaks, enhancements, compatibility and bug fixes. 
This project combines all three cases that has been described in the article(Image Analysis, OCR and InModelAnalysis) for Image description, text detection and celebrity face detection. Also, project has been recompiled in VisualStudio 2017 and packages are updated.

## Description
This Project is using Xamarin Forms technology in Form of Portable Class Library or PCL. Its a Cross platform project with Cross Platform UI and Code written in C# Language that compiles natively to Android, iOS and UWP Windows Platform. It Uses Native Camera and Library Component of mobile device to take a photo or upload a photo, then uses Project Oxford Vision Api Nuget Package that Utilizes Microsoft Cognitive Services [Computer Vision API](https://www.microsoft.com/cognitive-services/en-us/computer-vision-api)


## Getting Started
1. First you need to create Microsoft Azure Account And Deploy Cognitive Service Vision Api to get started. Its Free. for more information refer to this [link](https://www.microsoft.com/cognitive-services/en-us/computer-vision-api)
2. After Deployment You need to obtain your api key and place it where it instantiates VisionServiceClient in the code
3. Compile your project and deploy it to any device

**Don't forget to put your Vision Api key before debugging**
