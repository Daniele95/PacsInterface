using System.Collections.Generic;
using System.Threading;
using System.Windows;

namespace QueryRetrieveService
{
    public class GetQueryResponsesList : Publisher
    {
        static ManualResetEvent manualResetEvent = new ManualResetEvent(false);
        List<QueryObject> queryResponses = new List<QueryObject>();

        string Level;
        public List<QueryObject> getResponsesList(QueryObject query, string level)
        {
            Level = level;
            QueryRetrieve retrieveSeries = new QueryRetrieve();

            retrieveSeries.OnDatasetArrived += gotADataset;
            retrieveSeries.OnConnectionClosed += (seriesList) => { gotAllDatasets(); };

            // Launch Query Command
            retrieveSeries.find(query, level);

            bool a = manualResetEvent.WaitOne(1000);
            if (a == false) MessageBox.Show("timeout");

            // now wait...

            return queryResponses;
        }

        private void gotADataset(QueryObject s)
        {
            queryResponses.Add(s);
        }
        private void gotAllDatasets()
        {
            manualResetEvent.Set();
        }
    }
}
