using ImageProcessingComponent;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace ImageProcessing
{
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private string[] extensions = new string[] { ".jpg", ".jpeg", ".png", ".bmp" };

        private int thresholdLevel;

        private Array thresholdTypes;
        private ThresholdType thresholdType;

        private SoftwareBitmap inputSoftwareBitmap;

        private WriteableBitmap inputImage;
        private WriteableBitmap processedImage;

        private object ThresholdType
        {
            get { return thresholdType; }
            set
            {
                thresholdType = (ThresholdType)value;
                ThresholdImage();
            }
        }

        private double ThresholdLevel
        {
            get { return thresholdLevel; }
            set
            {
                thresholdLevel = Convert.ToInt32(value);
                ThresholdImage();
                OnPropertyChanged();
            }
        }

        private WriteableBitmap InputImage
        {
            get { return inputImage; }
            set
            {
                inputImage = value;
                OnPropertyChanged();
            }
        }

        private WriteableBitmap ProcessedImage
        {
            get { return processedImage; }
            set
            {
                processedImage = value;
                OnPropertyChanged();
            }
        }

        public SoftwareBitmap InputSoftwareBitmap
        {
            get
            {
                return inputSoftwareBitmap;
            }

            set
            {
                inputSoftwareBitmap = value;
            }
        }

        public MainPage()
        {
            InitializeComponent();

            ConfigureThresholdComboBox();
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ConfigureThresholdComboBox()
        {
            thresholdTypes = Enum.GetValues(typeof(ThresholdType));
            ThresholdType = (ThresholdType)thresholdTypes.GetValue(0);
        }

        private async void ButtonLoadImage_Click(object sender, RoutedEventArgs e)
        {
            var bitmapFile = await PickBitmap();

            if (bitmapFile != null)
            {
                inputSoftwareBitmap = await GetBitmapFromFile(bitmapFile);

                InputImage = inputSoftwareBitmap.ToWriteableBitmap();
            }
        }

        private IAsyncOperation<StorageFile> PickBitmap()
        {
            var photoPicker = new FileOpenPicker()
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };

            foreach (string extension in extensions)
            {
                photoPicker.FileTypeFilter.Add(extension);
            }

            return photoPicker.PickSingleFileAsync();
        }

        private async Task<SoftwareBitmap> GetBitmapFromFile(StorageFile bitmapFile)
        {
            using (var fileStream = await bitmapFile.OpenAsync(FileAccessMode.Read))
            {
                var bitmapDecoder = await BitmapDecoder.CreateAsync(fileStream);

                return await bitmapDecoder.GetSoftwareBitmapAsync();
            }
        }

        private void ThresholdImage()
        {
            if (inputSoftwareBitmap != null)
            {
                processedImage = inputSoftwareBitmap.ToWriteableBitmap();

                OpenCvWrapper.Threshold(processedImage, thresholdLevel, thresholdType);

                OnPropertyChanged("ProcessedImage");
            }
        }

        private void ButtonDrawContours_Click(object sender, RoutedEventArgs e)
        {
            if (inputSoftwareBitmap != null)
            {
                processedImage = inputSoftwareBitmap.ToWriteableBitmap();

                OpenCvWrapper.DetectObjects(processedImage, true);

                OnPropertyChanged("ProcessedImage");
            }
        }
    }
}
