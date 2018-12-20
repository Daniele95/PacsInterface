using Dicom;
using Dicom.Network;
using System;
using System.Collections.Generic;
using System.IO;
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

        public void move (string callingAE, QueryObject query, string level,GUILogic guiLogic)
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
                Console.WriteLine(studyId + " " + seriesId + " " + imageId);
            }

            var client = new DicomClient();

            bool? moveSuccessfully = null;
            cmove.OnResponseReceived += (DicomCMoveRequest requ, DicomCMoveResponse response) =>
            {
                if (response.Status.State == DicomState.Pending)
                {
                    Console.WriteLine("Sending is in progress. please wait: " + response.Remaining.ToString());
                }
                else if (response.Status.State == DicomState.Success)
                {
                    Console.WriteLine("Sending successfully finished");
                    moveSuccessfully = true;
                }
                else if (response.Status.State == DicomState.Failure)
                {
                    Console.WriteLine("Error sending datasets: " + response.Status.Description);
                    moveSuccessfully = false;
                }
                Console.WriteLine(response.Status);
            };
            var pcs = DicomPresentationContext.GetScpRolePresentationContextsFromStorageUids(
                DicomStorageCategory.Image,
                DicomTransferSyntax.ExplicitVRLittleEndian,
                DicomTransferSyntax.ImplicitVRLittleEndian,
                DicomTransferSyntax.ImplicitVRBigEndian);
            client.AdditionalPresentationContexts.AddRange(pcs);
            
            client.AddRequest(cmove);
            // cicle to kill listener and restart it (thus ending associatio) if move takes too much time
            bool sendSuccess = false;
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                Thread.Sleep(5000);
                if(!sendSuccess)
                {
                    guiLogic.listenerProcess.Kill();
                    guiLogic.newProcess();
                }
            }).Start();
            client.Send(GUILogic.readFromFile("server"), Int32.Parse(GUILogic.readFromFile("serverPort")), false, GUILogic.readFromFile("thisMachineAE"), GUILogic.readFromFile("serverAE"),1000);
            sendSuccess = true;
        }

        public static void SaveImage(DicomDataset dataset)
        {
            var studyUid = dataset.GetSingleValue<string>(DicomTag.StudyInstanceUID);
            var instUid = dataset.GetSingleValue<string>(DicomTag.SOPInstanceUID);

            var path = Path.GetFullPath(@"C:\Users\daniele\Desktop\");
            path = Path.Combine(path, studyUid);

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            path = Path.Combine(path, instUid) + ".dcm";

            new DicomFile(dataset).Save(path);
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
                        DicomTag myTag = DicomTag.Parse(tag.ToString());
                        try { 
                            property.SetValue(response, rp.Dataset.GetValues<string>(myTag)[0]);
                        } catch(Exception e)
                        {
           //                 MessageBox.Show("tag " + myTag.ToString() + " not found in this dataset");
                        }
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
            try
            {
                client.Send(GUILogic.readFromFile("server"), Int32.Parse(GUILogic.readFromFile("serverPort")), false, GUILogic.readFromFile("thisMachineAE"), GUILogic.readFromFile("serverAE"),1000);
            } catch(Exception e) { MessageBox.Show("impossible connect to server"); }

        }
        
    }
}
