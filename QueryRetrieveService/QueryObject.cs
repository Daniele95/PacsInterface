﻿using Dicom;
using System;
using System.Windows;

namespace QueryRetrieveService
{
    public class SeriesResponseQuery : QueryObject
    {
        public string SeriesInstanceUID { get; set; } = "";
    }
    public class SeriesLevelQuery : QueryObject
    {
        public string StudyInstanceUID { get; set; } = "";
        public string Modality { get; set; } = "";

        public SeriesLevelQuery(string studyInstanceUID)
        {
            StudyInstanceUID = studyInstanceUID;
        }
    }
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
