using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
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
            if (StudyDateStartPicker.SelectedDate != null)
                query.StudyDateMin = StudyDateStartPicker.SelectedDate.Value;
            if (StudyDateEndPicker.SelectedDate != null)
                query.StudyDateMax = StudyDateEndPicker.SelectedDate.Value;
            query.StudyDateMax = query.StudyDateMax.AddSeconds(86399);
         //   s.PatientBirthDate = dateRange(PatientBirthDateStartPicker, PatientBirthDateEndPicker);
            query.PatientName = patientFullName(PatientNameBox, PatientSurnameBox);
            query.Modality = ModalityBox.Text.ToString();
            //   q.StudyDescription = StudyDescriptionBox.Text.ToString();

            QueryRetrieve q = new QueryRetrieve();
            q.Event += showQueryResults;
            q.find(query);

        }

        private void showQueryResults(StudyResponseQuery s)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                listView.Items.Add(s);
            }), DispatcherPriority.ContextIdle);
        }

        private string patientFullName(TextBox patientNameBox, TextBox patientSurnameBox)
        {
            string name = patientNameBox.Text.ToString();
            string surname = patientSurnameBox.Text.ToString();

            string patientFullName="";
            if (name != "") patientFullName += "*";
            if (surname != "")  patientFullName += "*";

            return patientFullName;
        }


        public void getQueryFields()
        {
           // StudyLevelQuery querySettings = new StudyLevelQuery();

           // querySettings.SetField("PatientName", PatientNameBox.Text);
           // querySettings.SetField("PatientBirthDate", PatientBirthDatePicker.Text);
          //  querySettings.SetField("StudyDate", StudyDatePicker.Text);
           // querySettings.SetField("StudyDescription", StudyDescriptionBox.Text);
           // querySettings.SetField("Modality", ModalityBox.Text);

           // return querySettings;

        }
        
    }
}
