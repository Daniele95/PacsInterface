using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using Dicom;
using QueryRetrieveService;

namespace GUI
{
    public partial class QueryPage : Page
    {
        public MainWindow mainWindow;

        public QueryPage(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            InitializeComponent();
        }

        private void onLocalSearchButtonClicked(object sender, RoutedEventArgs e)
        {
          //  mainWindow.onSearchButtonClicked("local");
        }

        private void onRemoteSearchButtonClicked(object sender, RoutedEventArgs e)
        {
            StudyLevelQuery query = new StudyLevelQuery();
            DateTime start = DateTime.Today.AddYears(-100), end = DateTime.Today;
            if (StudyDateStartPicker.SelectedDate != null)
                start = StudyDateStartPicker.SelectedDate.Value;
            if (StudyDateEndPicker.SelectedDate != null)
                end =  StudyDateEndPicker.SelectedDate.Value;
            end = end.AddSeconds(86399);

            query.StudyDate = new DicomDateRange(start, end);
            query.PatientName = patientFullName(PatientNameBox, PatientSurnameBox);
            query.Modality = ModalityBox.Text.ToString();

            QueryRetrieve q = new QueryRetrieve();
            q.Event += showQueryResults;
            listView.Items.Clear();
            q.find(query,"Study");
            
        }

        private void showQueryResults(QueryObject response)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {

                // Add columns
                var gridView = new GridView();
                listView.View = gridView;

                PropertyInfo[] properties = response.GetType().GetProperties();
                foreach(PropertyInfo property in properties)
                    gridView.Columns.Add(new GridViewColumn
                    {
                        Header = property.Name,
                        DisplayMemberBinding = new Binding(property.Name)
                    });


                listView.Items.Add(response);
            }), DispatcherPriority.ContextIdle);
        }

        private void onMouseDown(object sender, MouseButtonEventArgs e)
        {
            ListViewItem item = sender as ListViewItem;

            if (item != null && item.IsSelected)
            {               
                SeriesLevelQuery query = new SeriesLevelQuery((StudyResponseQuery)item.Content);

                QueryRetrieve q = new QueryRetrieve();
                q.Event += mainWindow.downloadPage.showQueryResults;
                mainWindow.frame.Navigate(mainWindow.downloadPage);
                mainWindow.downloadPage.listView.Items.Clear();
                q.find(query,"Series");

            }
        }


        private string patientFullName(TextBox patientNameBox, TextBox patientSurnameBox)
        {
            string name = patientNameBox.Text.ToString();
            string surname = patientSurnameBox.Text.ToString();

            string patientFullName="";
            if (name != "" && surname != "") patientFullName += surname + "^"+ name;
            if (name == "") patientFullName += "*" + surname +"*";
            if (surname == "") patientFullName += "*"+name + "*";

            return patientFullName;
        }

        
        
    }
}
