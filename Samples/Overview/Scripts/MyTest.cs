using System.IO;
using Pdf.Utils;
using Syncfusion.Pdf;
using UnityEngine;
using Syncfusion.Pdf.Graphics;
	public class MyTest : MonoBehaviour
	{
		private Texture2D _mine;
		private Texture2D _hay;
		
		private readonly PdfFont _h1Font = new PdfStandardFont(PdfFontFamily.Helvetica, 20);
		private readonly PdfFont _font10 = new PdfStandardFont(PdfFontFamily.Helvetica, 10);
		
		float y = 0;
		float x = 0;
		
		int[][] matrizExemplo = 
		{
			new [] {0, 0, 0, 0, 0},
			new [] {0, 1, 0, 0, 0},
			new [] {0, 0, 0, 1, 0},
			new [] {0, 0, 1, 0, 0},
			new [] {0, 0, 0, 0, 0},
		};

		private void ClickButton()
		{
			JsHook.DownloadHandlerPDF("OKOK", new byte[0], 0);

		}
		private void Create()
		{
			PdfDocument document = new PdfDocument();
			PdfPage page = document.Pages.Add();
			PdfGraphics graphics = page.Graphics;
			
			MemoryStream stream = new MemoryStream();
			stream.Seek(0, SeekOrigin.Begin);
			document.Save(stream);
			stream.Position = 0;
            
			document.Close(true);
			byte[] pdfBytes = stream.ToArray();
			
			DrawBoard(graphics, matrizExemplo, x, y);
		}
		
		private void DrawBoard(PdfGraphics graphics, int[][] matriz, float startX, float startY)
		{
			float retangleSize = 80f;
			float padding = 2f;
			for (int i = 0; i < matriz.Length; i++)
			{
				for (int j = 0; j < matriz.Length; j++)
				{
					float x = startX + j * retangleSize;
					float y = startY + i * retangleSize;
                            
					if (matriz[i][j] == 1)
					{
						graphics.DrawRectangle(PdfBrushes.Red , x, y, retangleSize - padding, retangleSize - padding);
						graphics.DrawImage(EncodePng(_mine), x, y, retangleSize, retangleSize);
					}
					else
					{
						graphics.DrawRectangle(PdfBrushes.LightGray, x, y, retangleSize - padding, retangleSize - padding);
						graphics.DrawImage(EncodePng(_hay), x, y, retangleSize, retangleSize);
					}
				} 
			}
		}
		private PdfBitmap EncodePng(Texture2D currentImage)
		{
			byte[] imagemBytes = currentImage.EncodeToPNG();

			using (MemoryStream imageStream = new MemoryStream(imagemBytes))
			{
				return new PdfBitmap(imageStream);
			}
		}
	}