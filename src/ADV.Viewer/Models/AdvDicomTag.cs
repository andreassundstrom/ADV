// <copyright file="AdvDicomTag.cs" company="Andreas Sundström">
// Copyright (c) Andreas Sundström. All rights reserved.
// </copyright>

namespace ADV.Viewer.Models
{
    /// <summary>
    /// A Dicom tag representation.
    /// </summary>
    public class AdvDicomTag
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AdvDicomTag"/> class.
        /// </summary>
        /// <param name="id">The Id of the DicomTag.</param>
        /// <param name="tag">The dicom tag.</param>
        /// <param name="value">The value of the dicom tag.</param>
        public AdvDicomTag(int id, string tag, string value)
        {
            Id = id;
            Tag = tag;
            Value = value;
        }

        /// <summary>
        /// Gets or sets id of the dicom-tag.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the dicom tag.
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Gets or sets the value of the current tag.
        /// </summary>
        public string Value { get; set; }
    }
}
