using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ADV.Viewer
{
    /// <summary>
    /// The datacontext for the AddTagWindow.
    /// </summary>
    public class AddTagWindowDataContext : INotifyPropertyChanged
    {
        private string dicomTag = string.Empty;
        private string tagValue = string.Empty;

        /// <summary>
        /// Notify when property is changed.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets or sets the new dicomtag.
        /// </summary>
        public string DicomTag
        {
            get
            {
                return dicomTag;
            }

            set
            {
                dicomTag = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the value for the new dicomtag.
        /// </summary>
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
}
