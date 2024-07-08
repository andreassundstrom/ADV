using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace ADV.Viewer
{
    /// <summary>
    /// The datacontext for the AddTagWindow.
    /// </summary>
    public class AddTagViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        private string dicomTag = string.Empty;
        private string tagValue = string.Empty;
        private Dictionary<string, List<string>> errors = [];

        /// <summary>
        /// Notify when property is changed.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Notifies on errors.
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        /// <summary>
        /// Gets or sets the new dicomtag.
        /// </summary>
        [Required(ErrorMessage = "Must set dicom tag")]
        public string DicomTag
        {
            get
            {
                return dicomTag;
            }

            set
            {
                dicomTag = value;
                Validate(nameof(DicomTag), value);
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the value for the new dicomtag.
        /// </summary>
        [Required(ErrorMessage = "Must set tag value")]
        public string TagValue
        {
            get
            {
                return tagValue;
            }

            set
            {
                tagValue = value;
                OnPropertyChanged();
                Validate(nameof(TagValue), value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the model has errors.
        /// </summary>
        public bool HasErrors => errors.Count > 0;

        /// <summary>
        /// Gets errors in view model.
        /// </summary>
        /// <param name="propertyName">The property to get error for.</param>
        /// <returns>List of errors.</returns>
        public IEnumerable GetErrors(string? propertyName)
        {
            if (propertyName is not null && errors.Any(p => p.Key == propertyName))
            {
                return errors[propertyName];
            }

            return Enumerable.Empty<string>();
        }

        /// <summary>
        /// Method invoked when parameter is changed to trigger re-render.
        /// </summary>
        /// <param name="name">Name of the parameter.</param>
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void Validate(string property, object value)
        {
            List<ValidationResult> results = new List<ValidationResult>();
            Validator.TryValidateProperty(value, new ValidationContext(this) { MemberName = property }, results);
            if (results.Count > 0)
            {
                errors.Remove(property);
                errors.Add(property, results.Select(r => r.ErrorMessage ?? "Unspecified error").ToList());
            }
            else
            {
                errors.Clear();
            }

            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(property));
        }
    }
}
