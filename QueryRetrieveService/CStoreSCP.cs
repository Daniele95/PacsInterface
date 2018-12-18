using System;using Dicom; using Dicom.Network;
using System.Text;
using System.Threading.Tasks;
using Dicom.Log;
using System.IO;
using System.Windows;
using QueryRetrieveService;

namespace Listener
{
    public class CStoreSCP : DicomService, IDicomServiceProvider, IDicomCStoreProvider, IDicomCEchoProvider
    {
        private static DicomTransferSyntax[] AcceptedTransferSyntaxes = new DicomTransferSyntax[]
                                                                            {
                                                                                    DicomTransferSyntax
                                                                                        .ExplicitVRLittleEndian,
                                                                                    DicomTransferSyntax
                                                                                        .ExplicitVRBigEndian,
                                                                                    DicomTransferSyntax
                                                                                        .ImplicitVRLittleEndian
                                                                            };

        private static DicomTransferSyntax[] AcceptedImageTransferSyntaxes = new DicomTransferSyntax[]
                                                                                 {
                                                                                         // Lossless
                                                                                         DicomTransferSyntax
                                                                                             .JPEGLSLossless,
                                                                                         DicomTransferSyntax
                                                                                             .JPEG2000Lossless,
                                                                                         DicomTransferSyntax
                                                                                             .JPEGProcess14SV1,
                                                                                         DicomTransferSyntax
                                                                                             .JPEGProcess14,
                                                                                         DicomTransferSyntax
                                                                                             .RLELossless,

                                                                                         // Lossy
                                                                                         DicomTransferSyntax
                                                                                             .JPEGLSNearLossless,
                                                                                         DicomTransferSyntax
                                                                                             .JPEG2000Lossy,
                                                                                         DicomTransferSyntax
                                                                                             .JPEGProcess1,
                                                                                         DicomTransferSyntax
                                                                                             .JPEGProcess2_4,

                                                                                         // Uncompressed
                                                                                         DicomTransferSyntax
                                                                                             .ExplicitVRLittleEndian,
                                                                                         DicomTransferSyntax
                                                                                             .ExplicitVRBigEndian,
                                                                                         DicomTransferSyntax
                                                                                             .ImplicitVRLittleEndian
                                                                                 };

        public CStoreSCP(INetworkStream stream, Encoding fallbackEncoding, Logger log)
            : base(stream, fallbackEncoding, log)
        {
        }


        public void OnReceiveAssociationRequest(DicomAssociation association)
        {
            foreach (var pc in association.PresentationContexts)
            {
                if (pc.AbstractSyntax == DicomUID.Verification) pc.AcceptTransferSyntaxes(AcceptedTransferSyntaxes);
                else if (pc.AbstractSyntax.StorageCategory != DicomStorageCategory.None) pc.AcceptTransferSyntaxes(AcceptedImageTransferSyntaxes);
            }

            SendAssociationAccept(association);
        }

        public DicomCStoreResponse OnCStoreRequest(DicomCStoreRequest request)
        {
            Console.WriteLine("request");
            var patientName = request.Dataset.GetValues<string>(DicomTag.PatientName)[0].Replace(' ', '_');
        //    var studyDescription = request.Dataset.GetValues<string>(DicomTag.StudyDescription)[0].Replace(' ','_');
        //    var seriesDescription = request.Dataset.GetValues<string>(DicomTag.SeriesDescription)[0].Replace(' ','_');
        //    var instanceNumber = request.Dataset.GetValues<string>(DicomTag.InstanceNumber)[0];
            

            var path = Path.GetFullPath(Constants.imageThumbsFolder);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            path = path+"/"+Constants.tempFileName;

            request.File.Save(path);

            return new DicomCStoreResponse(request, DicomStatus.Success);

        }



        private void SendAssociationAccept(DicomAssociation association)
        {
            SendAssociationAcceptAsync(association);
        }

        public void OnReceiveAssociationReleaseRequest()
        {
            SendAssociationReleaseResponseAsync();
        }

        public void OnReceiveAbort(DicomAbortSource source, DicomAbortReason reason)
        {
        }

        public void OnConnectionClosed(Exception exception)
        {
        }

        public void OnCStoreRequestException(string tempFileName, Exception e)
        {
            // let library handle logging and error response
        }

        public DicomCEchoResponse OnCEchoRequest(DicomCEchoRequest request)
        {
            return new DicomCEchoResponse(request, DicomStatus.Success);
        }

        public Task OnReceiveAssociationRequestAsync(DicomAssociation association)
        {
            return Task.Run(()=> { OnReceiveAssociationRequest(association); });
        }

        public Task OnReceiveAssociationReleaseRequestAsync()
        {
            return Task.Run(() => { OnReceiveAssociationReleaseRequest(); });
        }
    }
}
