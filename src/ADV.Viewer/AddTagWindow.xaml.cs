// <copyright file="AddTagWindow.xaml.cs" company="Andreas Sundström">
// Copyright (c) Andreas Sundström. All rights reserved.
// </copyright>

using System.Windows;

namespace ADV.Viewer
{
    /// <summary>
    /// Interaction logic for AddTagWindow.xaml.
    /// </summary>
    public partial class AddTagWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddTagWindow"/> class.
        /// </summary>
        public AddTagWindow()
        {
            InitializeComponent();
            AddTagWindowDataContext = new AddTagViewModel();
            DataContext = AddTagWindowDataContext;
        }

        /// <summary>
        /// Gets or sets the datacontext for the window.
        /// </summary>
        public AddTagViewModel AddTagWindowDataContext { get; set; }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
