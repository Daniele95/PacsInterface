using Dicom.Network;
using Listener;
using System;
using System.IO;
using System.Windows;

namespace QueryRetrieveService
{
    public class GUILogic
    {

        public IDicomServer imageListener = DicomServer.Create<CStoreSCP>(11117);
        

    }
}
