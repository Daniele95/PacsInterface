using System;
using System.Windows;
using System.Windows.Navigation;
using QueryRetrieveService;

namespace GUI
{
    public partial class MainWindow : Window
    {
        public QueryPage queryPage;
        public DownloadPage downloadPage;
        public GUILogic guiLogic;

        public MainWindow()
        {
            InitializeComponent();
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
            queryPage = new QueryPage(this);
            downloadPage = new DownloadPage();
            guiLogic = new GUILogic();
            frame.NavigationService.Navigate(queryPage);
        }
        void OnProcessExit(object sender, EventArgs e)
        {
            guiLogic.listenerProcess.Kill();
        }

        void queryClick(object o, RoutedEventArgs e)
        {
            frame.NavigationService.Navigate(queryPage);
        }
        void downloadClick(object o, RoutedEventArgs e)
        {
            frame.NavigationService.Navigate(downloadPage);
        }
        
        private void frame_Navigated(object sender, NavigationEventArgs e)
        {
        }
    }
}