using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using MySql.Data.MySqlClient;
using System.Windows.Input;
using System.Windows.Media;

namespace MiddleFingerprintUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _isToggled = false;
        private Bitmap inputFingerprint;

        public MainWindow()
        {
            InitializeComponent();
            this.WindowState = WindowState.Maximized; // Maximize window
            this.WindowStyle = WindowStyle.None; // Remove window border and title bar
            this.ResizeMode = ResizeMode.NoResize; // Disable resizing
        }

        private void handle_upload(object sender, RoutedEventArgs e)
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
                    this.inputFingerprint = new Bitmap(filename);

                    // Set the image source
                    imageUpload.Source = bitmap;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading image: " + ex.Message);
                }
            }
        }

        private void ToggleSwitch(object sender, MouseButtonEventArgs e)
        {
            _isToggled = !_isToggled; // Toggle the state
            AnimateToggle();
        }

        private void AnimateToggle()
        {
            double targetX = _isToggled ? 36 : 0; // Move to right if toggled on
            var animation = new DoubleAnimation(targetX, TimeSpan.FromSeconds(0.2));
            sliderTransform.BeginAnimation(TranslateTransform.XProperty, animation);

            toggleBackgroundOn.Visibility = _isToggled ? Visibility.Visible : Visibility.Collapsed;
            textKMP.Visibility = _isToggled ? Visibility.Visible : Visibility.Collapsed;
            textBM.Visibility = _isToggled ? Visibility.Collapsed : Visibility.Visible;
        }

        private void handle_search(object sender, RoutedEventArgs e)
        {
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            Bitmap result = API.SearchFingerprint(inputFingerprint, _isToggled);
            if (result != null)
            {
                imageResult.Source = BitmapToImageSource(result);
            }
            else
            {
                MessageBox.Show("No match found.");
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();  // Closes the MainWindow
        }

        private ImageSource BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }
    }
}