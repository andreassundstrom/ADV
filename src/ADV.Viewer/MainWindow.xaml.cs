using FellowOakDicom;
using FellowOakDicom.Imaging;
using FellowOakDicom.Imaging.Codec;
using Microsoft.Win32;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace ADV.Viewer;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        new DicomSetupBuilder()
            .RegisterServices(s =>
            {
                s.AddFellowOakDicom().AddImageManager<WPFImageManager>();
                s.AddTranscoderManager<FellowOakDicom.Imaging.NativeCodec.NativeTranscoderManager>();
            })
            .SkipValidation()
            .Build();

        InitializeComponent();
        MainWindowDataContext = new MainWindowDataContext();
        DataContext = MainWindowDataContext;
        playTime = new DispatcherTimer();
        playTime.Tick += Timer_Tick;
        playTime.Interval = new TimeSpan(0, 0, 0, 0, 100);
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        SlideToNextFrame();
    }

    public MainWindowDataContext MainWindowDataContext { get; }
    private DicomImage? dicomImage;
    private readonly DispatcherTimer playTime;

    private void File_Open_Click(object sender, RoutedEventArgs e)
    {
        FileDialog fileDialog = new OpenFileDialog();

#if DEBUG
        fileDialog.InitialDirectory = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.Parent?.Parent?.Parent?.FullName + "\\sample-dicoms";
#endif

        if (fileDialog.ShowDialog() is bool filePicked && filePicked)
        {
            MainWindowDataContext.FileName = fileDialog.FileName;
            ReadDicom(fileDialog.FileName);
        }
    }

    private void ReadDicom(string file)
    {
        try
        {
            DicomFile dicomFile = DicomFile.Open(file);

            if (dicomFile.Dataset.TryGetValue(DicomTag.FrameTime, 0, out string frameTimeDecimalString))
            {
                int frameTimeInt = int.Parse(frameTimeDecimalString);
                playTime.Interval = new TimeSpan(0, 0, 0, 0, frameTimeInt);
            }

            SetDicomTags(dicomFile.Dataset);
            dicomImage = new DicomImage(file);
            WriteableBitmap bitmap = dicomImage.RenderImage().AsWriteableBitmap();
            DicomImageSurface.Source = bitmap;
            MainWindowDataContext.MaxFrames = dicomImage.NumberOfFrames - 1;

        }
        catch (DicomFileException exception)
        {
            MessageBox.Show(exception.Message, "Error when reading dicom-file", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (DicomCodecException exception)
        {
            MessageBox.Show(exception.Message, "Error when loading image data", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (Exception exception)
        {
            MessageBox.Show(exception.Message, "Unhandled exception");
        }
    }

    private void SlideToNextFrame()
    {
        if (MainWindowDataContext.Frame == MainWindowDataContext.MaxFrames)
        {
            MainWindowDataContext.Frame = 0;
            return;
        }

        MainWindowDataContext.Frame += 1;
        SlideToFrame(MainWindowDataContext.Frame);
    }

    private void SlideToFrame(int frame)
    {
        try
        {
            WriteableBitmap? bitmap = dicomImage?.RenderImage(frame).AsWriteableBitmap();
            DicomImageSurface.Source = bitmap;
        }
        catch (Exception exception)
        {
            MessageBox.Show(exception.Message, "Unhandled exception");
        }
    }

    private void SetDicomTags(DicomDataset dataset)
    {
        Dictionary<string, string> tags = new System.Collections.Generic.Dictionary<string, string>();
        foreach (DicomItem? tag in dataset)
        {
            bool value = dataset.TryGetString(tag.Tag, out string stringValue);
            tags.Add(tag.ToString(), value ? stringValue : "# No string representation");
        }
        MainWindowDataContext.DicomTags = tags;
    }

    private void FrameSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        SlideToFrame((int)e.NewValue);
    }

    private void PlayButton_Click(object sender, RoutedEventArgs e)
    {
        playTime.Start();
    }

    private void PauseButton_Click(object sender, RoutedEventArgs e)
    {
        playTime.Stop();
    }

    private void File_Close_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }
}

public class MainWindowDataContext : INotifyPropertyChanged
{
    private Dictionary<string, string> dicomTags = [];

    public Dictionary<string, string> DicomTags
    {
        get { return dicomTags; }
        set
        {
            dicomTags = value;
            OnPropertyChanged();
        }
    }


    private string? _fileName;
    public string? FileName
    {
        get { return _fileName; }
        set
        {
            _fileName = value;
            OnPropertyChanged();
        }
    }

    private int frame;

    public int Frame
    {
        get { return frame; }
        set
        {
            frame = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(FrameDisplay));
        }
    }

    public string FrameDisplay
    {
        get { return $"Frame {frame}/{maxFrames}"; }
    }

    private int maxFrames;

    public int MaxFrames
    {
        get { return maxFrames; }
        set
        {
            maxFrames = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(FrameDisplay));
        }
    }



    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}