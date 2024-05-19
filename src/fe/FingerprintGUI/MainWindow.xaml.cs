using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32; // Needed for OpenFileDialog
using System.Windows.Media.Imaging; // Needed for BitmapImage

namespace FingerprintGUI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select an image file";
            openFileDialog.Filter = "Image files (*.bmp, *.jpg, *.jpeg, *.png)|*.bmp;*.jpg;*.jpeg;*.png"; // Filter to only show image files

            // Show open file dialog box
            bool? result = openFileDialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = openFileDialog.FileName;
                try
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(filename);
                    bitmap.EndInit();

                    // Set the image source
                    imageUpload.Source = bitmap;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading image: " + ex.Message);
                }
            }
        }
    }
}
