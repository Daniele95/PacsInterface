using Dicom;
using Dicom.Network;
using System;

namespace QueryRetrieveService
{
    public abstract class Publisher
    {
        public delegate void EventHandler(StudyResponseQuery s);
        public event EventHandler Event;

        public void RaiseEvent(StudyResponseQuery s)
        {
            Event(s);
        }
    }
    public class QueryRetrieve : Publisher
    {

        public void move ()
        {
            var cmove = new DicomCMoveRequest("USER", "1.3.6.1.4.1.5962.1.1.0.0.0.1196527414.5534.0.1");

            var client = new DicomClient();
            client.AddRequest(cmove);
            client.Send("localhost", 11112, false, "USER", "MIOSERVER");
        }

        public void find(StudyLevelQuery query)
        {
            var cfind = DicomCFindRequest.CreateStudyQuery(
             //   patientId: query.PatientID,
                patientName: query.PatientName,
                studyDateTime: new DicomDateRange(query.StudyDateMin, query.StudyDateMax),
             //   accession: query.AccessionNumber,
                // studyid:
                modalitiesInStudy: query.Modality
           //     studyInstanceUid: query.StudyInstanceUID
              );
            cfind.OnResponseReceived = (DicomCFindRequest rq, DicomCFindResponse rp) => {
                if (rp.HasDataset)
                {
                    var response = new StudyResponseQuery();
                    response.PatientName = rp.Dataset.GetValues<string>(DicomTag.PatientName)[0];
                    response.StudyDate = rp.Dataset.GetValues<string>(DicomTag.StudyDate)[0];
                  //  response.Modality = rp.Dataset.GetValues<string>(DicomTag.Modality)[0];

                    RaiseEvent(response);
                }
            
            };


            var client = new DicomClient();
            client.AddRequest(cfind);
            client.Send("localhost", 11112, false, "USER", "MIOSERVER");

        }




    }
}
