using System;
using System.Collections;
using System.IO;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime
{
    public class ScreenCapture : MonoBehaviour
    {
        private Texture2D screenshotTexture;
    
        [SerializeField] private RectTransform targetRect;
        [SerializeField] private CanvasScaler canvas;
        private int _height;
        private int _width;

        public Button takePicture;

        private void Start()
        {
            takePicture.onClick.AddListener(() =>
            {
                TakeScreenshot("Image", "FotoTirada");
            });
        }
        public void TakeScreenshot(string fileName, string caption = "")
        {
            StartCoroutine(CutSpriteFromScreen(fileName, caption));
        }
    
        private IEnumerator CutSpriteFromScreen(string fileName, string caption = "")
        {

            canvas.referenceResolution = new Vector2(Screen.width, Screen.height);
            yield return new WaitForEndOfFrame();
            Rect rect = RectTransformUtility.PixelAdjustRect(targetRect, canvas.GetComponent<Canvas>());

            int textWidth = Convert.ToInt32(rect.width);
            int textHeight = Convert.ToInt32(rect.height);

            var startX = Convert.ToInt32(rect.x) + Screen.width / 2;
            var startY = Convert.ToInt32(rect.y) + Screen.height / 2;

            var tex = new Texture2D(textWidth, textHeight, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(startX, startY, textWidth, textHeight), 0, 0);
            tex.Apply();
            ConvertToPdf(tex.EncodeToPNG());
            canvas.referenceResolution = new Vector2(720, 1280);
        }
        public void ConvertToPdf(byte[] bytes)
        {
            PdfDocument document = new PdfDocument();
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
;
            XImage image = XImage.FromStream(new MemoryStream(bytes));

            gfx.DrawImage(image, 0, 0, page.Width, page.Height);
            
            string outputPath = Path.Combine(Application.persistentDataPath, "screenshot" + ".pdf");
            document.Save(outputPath);
            document.Close();
        }
    }
}
