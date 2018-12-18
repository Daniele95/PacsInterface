using Dicom;
using Dicom.Network;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Windows;


namespace QueryRetrieveService
{
    public abstract class Publisher
    {
        public delegate void EventHandler(QueryObject s);
        public event EventHandler OnDatasetArrived;

        public void RaiseEvent(QueryObject s)
        {
            OnDatasetArrived(s);
        }

        public delegate void ConnectionClosed(List<QueryObject> l);
        public event ConnectionClosed OnConnectionClosed;

        public void RaiseConnectionClosed(List<QueryObject> l)
        {
            OnConnectionClosed(l);
        }
    }

    public class QueryRetrieve : Publisher
    {
        List<QueryObject> queryResponses = new List<QueryObject>();

        public void move (string callingAE, QueryObject query, string level)
        {
            var cmove = new DicomCMoveRequest("","");

            if (query.GetType().ToString()== "QueryRetrieveService.StudyResponseQuery")
            {
                string studyId = ((StudyResponseQuery)query).StudyInstanceUID;
                cmove = new DicomCMoveRequest(callingAE, studyId);
            }
            if (query.GetType().ToString() == "QueryRetrieveService.SeriesResponseQuery")
            {
                string studyId = ((SeriesResponseQuery)query).StudyInstanceUID;
                string seriesId = ((SeriesResponseQuery)query).SeriesInstanceUID;
                cmove = new DicomCMoveRequest(callingAE, studyId,seriesId);
            }

            if (query.GetType().ToString() == "QueryRetrieveService.ImageResponseQuery")
            {
                string studyId = ((ImageResponseQuery)query).StudyInstanceUID;
                string seriesId = ((ImageResponseQuery)query).SeriesInstanceUID;
                string imageId = ((ImageResponseQuery)query).SOPInstanceUID;
                cmove = new DicomCMoveRequest(callingAE, studyId, seriesId, imageId);
            }

            var client = new DicomClient();
            client.AddRequest(cmove);
            client.Send("localhost", 11112, false, "USER", "MIOSERVER");
        }
        
        public void find(QueryObject query, string level)
        {
            if (level != "Study" && level != "Series" && level != "Image")
            { MessageBox.Show("incorrect level"); return; }

            DicomQueryRetrieveLevel queryLevel = (DicomQueryRetrieveLevel) Enum.Parse(typeof(DicomQueryRetrieveLevel), level);
            DicomCFindRequest cfind = new DicomCFindRequest(queryLevel);
            Type tipo = query.GetType();
            PropertyInfo[] properties1 = tipo.GetProperties();

            foreach (PropertyInfo property in properties1)
            {
                var tag = typeof(DicomTag).GetField(property.Name).GetValue(null);
                DicomTag theTag = (DicomTag.Parse(tag.ToString()));

                var variabile = property.GetValue(query);
                if (variabile.GetType().ToString() == "System.String")
                {
                    String a = (String)variabile;
                    cfind.Dataset.Add(theTag, a);
                }
                if (variabile.GetType().ToString() == "Dicom.DicomDateRange")
                {
                    DicomDateRange a = (DicomDateRange)variabile;
                    cfind.Dataset.Add(theTag, a);
                }
            }

            cfind.OnResponseReceived = (DicomCFindRequest rq, DicomCFindResponse rp) => {
                if (rp.HasDataset)
                {
                    var type = Type.GetType("QueryRetrieveService."+level+"ResponseQuery");
                    var response = (QueryObject)Activator.CreateInstance(type);
                    queryResponses.Add(response);

                    PropertyInfo[] properties = response.GetType().GetProperties();
                    foreach (PropertyInfo property in properties)
                    {
                        var tag = typeof(DicomTag).GetField(property.Name).GetValue(null);
                        property.SetValue(response, rp.Dataset.GetValues<string>(DicomTag.Parse(tag.ToString()))[0]);
                    }
                    RaiseEvent(response);
                }
            };


            var client = new DicomClient();
            client.AddRequest(cfind);
            client.AssociationReleased += (sender,e)=> {
                // non può inviare la lista 'queryResponses' prima
                // che 'cfind.OnResponseReceived' abbia finito di riempirla!!
                Thread.Sleep(5);
                RaiseConnectionClosed(queryResponses);

            };
            try { 
            client.Send(GUILogic.readFromFile("server"), Int32.Parse(GUILogic.readFromFile("serverPort")), false, GUILogic.readFromFile("thisMachineAE"), GUILogic.readFromFile("serverAE"));
            } catch(Exception e) { MessageBox.Show("impossible connect to server"); }

        }
        
    }
}
