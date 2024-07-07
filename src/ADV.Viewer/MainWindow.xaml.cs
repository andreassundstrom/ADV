// <copyright file="MainWindow.xaml.cs" company="Andreas Sundström">
// Copyright (c) Andreas Sundström. All rights reserved.
// </copyright>

using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using ADV.Viewer.Models;
using FellowOakDicom;
using FellowOakDicom.Imaging;
using FellowOakDicom.Imaging.Codec;
using Microsoft.Win32;

namespace ADV.Viewer;

/// <summary>
/// Interaction logic for MainWindow.xaml.
/// </summary>
public partial class MainWindow : Window
{
    private readonly DispatcherTimer playTime;

    private DicomImage? dicomImage;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
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

    /// <summary>
    /// Gets the data context for the window.
    /// </summary>
    public MainWindowDataContext MainWindowDataContext { get; }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        SlideToNextFrame();
    }

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
        // Reset from previous file
        MainWindowDataContext.Frame = 0;
        MainWindowDataContext.MaxFrames = 0;

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
        List<DicomTagVM> tags = [];
        int i = 0;
        foreach (DicomItem? tag in dataset)
        {
            bool value = dataset.TryGetString(tag.Tag, out string stringValue);
            tags.Add(new DicomTagVM(i, tag.ToString(), value ? stringValue : "# No string representation"));
            i++;
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
        MainWindowDataContext.Playing = true;
    }

    private void PauseButton_Click(object sender, RoutedEventArgs e)
    {
        playTime.Stop();
        MainWindowDataContext.Playing = false;
    }

    private void File_Close_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    private void Dicom_AddTag_Click(object sender, RoutedEventArgs e)
    {
        AddTagWindow addTagWindow = new AddTagWindow();
        bool? result = addTagWindow.ShowDialog();
        if (result == true)
        {
            string tagString = addTagWindow.AddTagWindowDataContext.DicomTag;
            string valueString = addTagWindow.AddTagWindowDataContext.TagValue;
            MatchCollection matches = Regex.Matches(tagString, @"\d{4}");
            if (matches.Count != 2)
            {
                MessageBox.Show("Failed to parse the dicom tag, ensure its format is (XXXX,XXXX)", "Error parsing result");
                return;
            }

            string group = matches.First().Value;
            string element = matches.Last().Value;
            ushort groupShort = Convert.ToUInt16(group, 16);
            ushort elementShort = Convert.ToUInt16(element, 16);

            DicomTag tag = new DicomTag(groupShort, elementShort);

            // Create new by value
            List<DicomTagVM> newTagList = new List<DicomTagVM>(MainWindowDataContext.DicomTags);
            newTagList.Add(new DicomTagVM(MainWindowDataContext.DicomTags.Count, tag.ToString(), valueString));
            MainWindowDataContext.DicomTags = newTagList;
        }
    }
}
