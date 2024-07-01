// <copyright file="DicomTag.cs" company="Andreas Sundström">
// Copyright (c) Andreas Sundström. All rights reserved.
// </copyright>

namespace ADV.Viewer.Models
{
    /// <summary>
    /// A Dicom tag representation.
    /// </summary>
    public class DicomTag
    {
        /// <summary>
        /// Gets or sets id of the dicom-tag.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the dicom tag.
        /// </summary>
        public required string Tag { get; set; }

        /// <summary>
        /// Gets or sets the value of the current tag.
        /// </summary>
        public required string Value { get; set; }
    }
}
