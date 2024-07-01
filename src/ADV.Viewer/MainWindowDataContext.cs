// <copyright file="MainWindowDataContext.cs" company="Andreas Sundström">
// Copyright (c) Andreas Sundström. All rights reserved.
// </copyright>

using System.ComponentModel;
using System.Runtime.CompilerServices;
using ADV.Viewer.Models;

namespace ADV.Viewer;

/// <summary>
/// Main windows data context.
/// </summary>
public class MainWindowDataContext : INotifyPropertyChanged
{
    private List<DicomTagVM> dicomTags = [];

    private string? fileName;

    private int frame;

    private bool playing = false;

    private int maxFrames;

    /// <summary>
    /// Notify re-rendering on property change.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets or sets tags for currently loaded DICOM file.
    /// </summary>
    public List<DicomTagVM> DicomTags
    {
        get
        {
            return dicomTags;
        }

        set
        {
            dicomTags = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets the filename for current dicom file.
    /// </summary>
    public string? FileName
    {
        get
        {
            return fileName;
        }

        set
        {
            fileName = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets current frame for dicom file.
    /// </summary>
    public int Frame
    {
        get
        {
            return frame;
        }

        set
        {
            frame = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(FrameDisplay));
        }
    }

    /// <summary>
    /// Gets a display containing the frame/maxFrames.
    /// </summary>
    public string FrameDisplay
    {
        get { return $"Frame {frame}/{maxFrames}"; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the current state of playing DICOM file.
    /// </summary>
    public bool Playing
    {
        get
        {
            return playing;
        }

        set
        {
            playing = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(PlayEnabled));
            OnPropertyChanged(nameof(PauseEnabled));
        }
    }

    /// <summary>
    /// Gets a value indicating whether the play button is enabled.
    /// </summary>
    public bool PlayEnabled { get => !Playing; }

    /// <summary>
    /// Gets a value indicating whether the pause button is enabled.
    /// </summary>
    public bool PauseEnabled { get => Playing; }

    /// <summary>
    /// Gets or sets the max number of frames for the current dicom.
    /// </summary>
    public int MaxFrames
    {
        get
        {
            return maxFrames;
        }

        set
        {
            maxFrames = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(FrameDisplay));
        }
    }

    /// <summary>
    /// Method invoked when parameter is changed to trigger re-render.
    /// </summary>
    /// <param name="name">Name of the parameter.</param>
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}