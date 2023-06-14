using Photon.Pun;
using UnityEngine.UI;

namespace Paroxe.PdfRenderer
{
    public class SharePDF : MonoBehaviourPun
    {
        //Örnek Web PDF'i: https://www.dropbox.com/s/tssavtnvaym2t6b/DocumentationEN.pdf?raw=1
        public PhotonView photonView;
        public PDFViewer PDFViewer;
        public bool isPresenter = false;
        public string userPdfUrl;

        public InputField userPDFurlInput;
        // Button nesnesine eklediğimiz scriptte bu değişkeni tanımladık
        public Button presenterButton;

        // Scriptte bu metodu oluşturduk
        public void OnClickPresenterTrue()
        {
            isPresenter = true;
        }

        public void OnClickPresenterFalse()
        {
            userPdfUrl = "";
            SharePDFURL();
            isPresenter = false;
        }

        public void OnInputChangePDFURL()
        {
            // Burada değişiklik yaptık
            userPdfUrl = userPDFurlInput.text;
        }

        //////////////////////////////////////////////////////////////////////// Once presenter wants to share the link of PDF among all users
        public void SharePDFURL()
        {
            if (isPresenter)
            {
                photonView.RPC("RPC_SharePDFURL", RpcTarget.AllBuffered, userPdfUrl);
            }
        }

        [PunRPC]
        private void RPC_SharePDFURL(string PDFURL)
        {
            PDFViewer.FileURL = PDFURL;
            PDFViewer.LoadDocumentFromWeb(PDFURL, "", 0);
        }

        //////////////////////////////////////////////////////////////////// Once presenter changed the page (It gets orders from PDFViewer script)
        public void LoadPDFData()
        {
            photonView.RPC("RPC_PDFData", RpcTarget.AllBuffered, PDFViewer.GetCurrentPageNumber());
        }

        [PunRPC]
        private void RPC_PDFData(int currentpagenumber)
        {
            if (!isPresenter)
            {
                PDFViewer.GoToPage(currentpagenumber);
            }
        }

        /// ////////////////////////////////////////////////////////////// When the presenter zoom in or zoom out (It gets orders from PDFViewer script)
        public void ZoomPDF(float zoomnumber, bool zoomin)
        {
            photonView.RPC("RPC_ZoomPDF", RpcTarget.AllBuffered, zoomnumber, zoomin);
        }

        [PunRPC]
        private void RPC_ZoomPDF(float zoomnumber, bool zoomin)
        {
            if (!isPresenter)
            {
                PDFViewer.NetworkZoom(zoomnumber, zoomin);
            }
        }

        // Bu metodu PhotonView componentinin OnPhotonSerializeView metoduna ekledik
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // We own this player: send the others our data
                stream.SendNext(isPresenter);
            }
            else
            {
                // Network player, receive data
                isPresenter = (bool)stream.ReceiveNext();
            }
        }
    }
}
