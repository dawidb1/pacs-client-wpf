using pacs_client.Model;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace pacs_client
{

    public partial class MainWindow : Window
    {
        Patients patients;
        Images images;
        List<string> patientList = new List<string>();
        List<string> imagesList = new List<string>();

        string selectedPatient = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
            IPacsConfiguration pacsConfiguration = new PacsConfiguration();
            this.patients = new Patients(pacsConfiguration);
            this.images = new Images(pacsConfiguration);
            this.patientList = patients.GetPatients();

            patientDataGrid.ItemsSource = this.patientList;
        }

        private void PatientDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            this.selectedPatient = patientDataGrid.SelectedValue.ToString();
            this.imagesList = this.images.GetImages(this.selectedPatient);
            imageDataGrid.ItemsSource = this.imagesList;
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            this.patientList = this.patients.GetPatients();
            patientDataGrid.ItemsSource = this.patientList;
        }


        private void ImageDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (imageDataGrid.SelectedValue != null)
            {
                var filename = imageDataGrid.SelectedValue.ToString();
                var imageBrowserWindow = new ShowImageWindow();
                var sourceName = new ImageSourceConverter().ConvertFromString(filename) as ImageSource;

                imageBrowserWindow.imageContainer.Source = sourceName;
                imageBrowserWindow.Show();
            }

        }
    }
}
