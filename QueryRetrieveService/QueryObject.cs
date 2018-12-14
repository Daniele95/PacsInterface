using Dicom;
using System;

namespace QueryRetrieveService
{

    public class StudyResponseQuery : QueryObject
    {
        public string StudyInstanceUID { get; set; } = "";
        public string PatientName { get; set; } = "";
        public string StudyDate { get; set; } = "";
      //  public string Modality { get; set; } = "";
    }
    public class StudyLevelQuery : QueryObject
    {
        public string PatientId { get; set; } = "";
        public string PatientName { get; set; } = "";
        public DicomDateRange StudyDate { get; set; } = new DicomDateRange();
        public string AccessionNumber { get; set; } = "";
        public string StudyID { get; set; } = "";
        public string Modality { get; set; } = "";
        public string StudyInstanceUID { get; set; } = "";
    }

    public abstract class QueryObject
    {
    }
}
