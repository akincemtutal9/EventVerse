using UnityEngine;
using System.Collections;
using Paroxe.PdfRenderer.WebGL;
using UnityEngine.UI;

namespace Paroxe.PdfRenderer.Examples
{
    public class WebGL_API_Usage : MonoBehaviour
    {
        public RawImage m_RawImage;
        public int page;
        PDFDocument document;
        public Button nextButton;
        public IEnumerator Start()
        {
            page = 0;

            Debug.Log("WebGLTest: ");

            PDFJS_Promise<PDFDocument> documentPromise = PDFDocument.LoadDocumentFromUrlAsync("https://gptverse.art/docs/whitepaper.pdf");

            while (!documentPromise.HasFinished)
                yield return null;

            if (!documentPromise.HasSucceeded)
            {
                Debug.Log("Fail: documentPromise");

                yield break;
            }

            Debug.Log("Success: documentPromise");

            document = documentPromise.Result;
            nextButton.onClick.AddListener(() => StartCoroutine(RenderPage()));

            //nextButton.onClick.AddListener(StartCoroutine(RenderPage()));

        }

        public IEnumerator RenderPage()
        {
            if (this.page == document.GetPageCount())
                this.page = 0;

            PDFJS_Promise<PDFPage> pagePromise = document.GetPageAsync(this.page);

            while (!pagePromise.HasFinished)
                yield return null;

            if (!pagePromise.HasSucceeded)
            {
                Debug.Log("Fail: pagePromise");

                yield break;
            }

            Debug.Log("Success: pagePromise");

            PDFPage page = pagePromise.Result;

            PDFJS_Promise<Texture2D> renderPromise = PDFRenderer.RenderPageToTextureAsync(page, (int)page.GetPageSize().x, (int)page.GetPageSize().y);

            while (!renderPromise.HasFinished)
                yield return null;

            if (!renderPromise.HasSucceeded)
            {
                Debug.Log("Fail: pagePromise");

                yield break;
            }

            this.page++;

            Texture2D renderedPageTexture = renderPromise.Result;

            ((RectTransform)m_RawImage.transform).sizeDelta = new Vector2(renderedPageTexture.width, renderedPageTexture.height);
            m_RawImage.texture = renderedPageTexture;
        }
    }
}
