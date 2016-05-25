using FelicidApp.Model;
using FelicidApp.Services;
using FelicidApp.Utils.Messages;
using FelicidApp.View.Base;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.ProjectOxford.Emotion;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace FelicidApp.View
{
    public sealed partial class MainPage : BasePage
    {
        private static int _playingIndex = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        public override void ShowErrorToUser(ErrorMessage error)
        {
            ErrorMessage.Text = (error != null) ? $"{error.Title}: {error.Message}" : "OK";
        }

        static string EmotionAPIKey = "1c08a603043a49e58303f90d5570e4df";

        MediaCapture mediaCapture;
        DispatcherTimer dispatcherTimer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(2) };
        EmotionServiceClient emotionClient = new EmotionServiceClient(EmotionAPIKey);
    

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (string.IsNullOrWhiteSpace(ConfigurationService.UserName)
                || string.IsNullOrWhiteSpace(ConfigurationService.DeviceId)
                || string.IsNullOrWhiteSpace(ConfigurationService.DeviceKey))
            {
                var dialog = new UserNameDialog();
                ContentDialogResult result;
                do
                {
                    result = await dialog.ShowAsync();

                } while (result != ContentDialogResult.Primary);
            }

            MotionConfig.Text = ConfigurationService.Motion;
            ConfigurationService.Playlist = await YoutubeService.GetYoutubeURI();
            NavigateToList(0);

            UserName.Text = ConfigurationService.UserName;
            DeviceId.Text = ConfigurationService.DeviceId;

            await Init();
            GetEmotions(null, null);
            dispatcherTimer.Tick += GetEmotions;
            dispatcherTimer.Start();
        }

        private void NavigateToList(int i)
        {
            var list = ConfigurationService.Playlist.Skip(i).FirstOrDefault();

            if (string.IsNullOrWhiteSpace(list)) return;

            PlayInfo.Text = $"{i + 1} of {ConfigurationService.Playlist.Count}";
            _playingIndex = i;
            WebView.Navigate(new Uri(list));
        }

        uint width, height;

        private async Task Init()
        {
            log("finding devices");
            mediaCapture = new MediaCapture();
            var cameras = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
            var camera = cameras.First();
            log("initializing");
            var settings = new MediaCaptureInitializationSettings() { VideoDeviceId = camera.Id };
            await mediaCapture.InitializeAsync(settings);
            log("initialized");
            var streamprops = mediaCapture.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.Photo) as VideoEncodingProperties;
            width = streamprops.Width;
            height = streamprops.Height;
            ViewFinder.Source = mediaCapture;
            await mediaCapture.StartPreviewAsync();
            log("preview started");
        }

        async void GetEmotions(object sender, object e)
        {
            dispatcherTimer.Stop();
            double ratio = 1;
            double leftMargin = 0;
            double topMargin = 0;
            if (width > 0)
            {
                var hratio = ViewFinder.ActualHeight / height;
                var wratio = ViewFinder.ActualWidth / width;
                if (hratio < wratio)
                {
                    ratio = hratio;
                    leftMargin = (ViewFinder.ActualWidth - (width * ratio)) / 2;
                }
                else
                {
                    ratio = wratio;
                    topMargin = (ViewFinder.ActualHeight - (height * ratio)) / 2;

                }
            }
            try
            {
                log("Capturing photo");
                using (var mediaStream = new MemoryStream())
                {
                    await mediaCapture.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), mediaStream.AsRandomAccessStream());
                    var streamCopy = new MemoryStream();
                    try
                    {
                        mediaStream.Position = 0L;
                        mediaStream.CopyTo(streamCopy);
                        mediaStream.Position = 0L;
                        streamCopy.Position = 0L;
                        log("Getting emotion");

                        var emotions = await emotionClient.RecognizeAsync(mediaStream);

                        if (emotions != null && emotions.Length > 0)
                        {
                            log("Emotion recognized");
                            RectangleCanvas.Children.Clear();
                            foreach (var emotion in emotions)
                            {
                                var r = new Windows.UI.Xaml.Shapes.Rectangle();
                                RectangleCanvas.Children.Add(r);
                                r.Stroke = new SolidColorBrush(Windows.UI.Colors.Yellow);
                                r.StrokeThickness = 1;
                                r.Width = emotion.FaceRectangle.Width * ratio;
                                r.Height = emotion.FaceRectangle.Height * ratio;
                                Canvas.SetLeft(r, (emotion.FaceRectangle.Left * ratio) + leftMargin);
                                Canvas.SetTop(r, (emotion.FaceRectangle.Top * ratio) + topMargin);
                                var t = new TextBlock();
                                RectangleCanvas.Children.Add(t);
                                t.Width = r.Width;
                                t.FontSize = 6;
                                t.Foreground = new SolidColorBrush(Windows.UI.Colors.Yellow);
                                Canvas.SetLeft(t, (emotion.FaceRectangle.Left * ratio) + leftMargin);
                                Canvas.SetTop(t, (emotion.FaceRectangle.Top * ratio) + topMargin + r.Height);

                                var list = new[] { new { Emotion="Anger", Score= emotion.Scores.Anger },
                                    new { Emotion="Contempt", Score= emotion.Scores.Contempt},
                                    new { Emotion="Disgust", Score= emotion.Scores.Disgust },
                                    new { Emotion="Fear", Score= emotion.Scores.Fear },
                                    new { Emotion="Happiness", Score= emotion.Scores.Happiness },
                                    new { Emotion="Neutral", Score= emotion.Scores.Neutral },
                                    new { Emotion="Sadness", Score= emotion.Scores.Sadness },
                                    new { Emotion="Surprise", Score= emotion.Scores.Surprise }
                                };

                                var max = list.ToList().OrderByDescending(a => a.Score);
                                if (max.First().Score < 0.8)
                                {
                                    t.Text = string.Format("{0}:{1:P}\n{2}:{3:P}", max.First().Emotion, max.First().Score,
                                        max.Skip(1).First().Emotion, max.Skip(1).First().Score);
                                }
                                else
                                {
                                    t.Text = string.Format("{0}:{1:P}", max.First().Emotion, max.First().Score);
                                }

                                Messenger.Default.Send(
                                    new EmotionData(
                                        ConfigurationService.UserName, 
                                        DateTime.Now, 
                                        max.First().Emotion));
                            }
                        }
                        else
                        {
                            log("emotion not recognized");
                        }
                    }
                    finally
                    {
                        streamCopy.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            finally
            {
                dispatcherTimer.Start();
            }
        }

        private void log(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        private void Next_OnClick(object sender, RoutedEventArgs e)
        {
            NavigateToList(++_playingIndex);
        }

        private void Previous_OnClick(object sender, RoutedEventArgs e)
        {
            NavigateToList(--_playingIndex);
        }

        private void Change_OnClick(object sender, RoutedEventArgs e)
        {
            var rootFrame = Window.Current.Content as Frame;
            rootFrame?.Navigate(typeof(Home));
        }
    }
}
