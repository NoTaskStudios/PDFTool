using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using UnityEngine;

namespace Pdf.Core
{
        public abstract class PDFHandler : MonoBehaviour
        {
                private string _fileName = "Comprovante";
                private const string Extension = ".pdf";
        
                private const float JumpLine = 20;
                private const int TableWidth = 400;
        
                private int _cellSize;
                private Sprite[,] _board;
                
                [SerializeField] private Texture2D _mine;
                [SerializeField] private Texture2D _hay;
                
                private int[][] matrizExemplo = 
                {
                        new [] {0, 0, 0, 0, 0},
                        new [] {0, 1, 0, 0, 0},
                        new [] {0, 0, 0, 1, 0},
                        new [] {0, 0, 1, 0, 0},
                        new [] {0, 0, 0, 0, 0},
                };
        
                private readonly PdfFont _h1Font = new PdfStandardFont(PdfFontFamily.Helvetica, 20);
                private readonly PdfFont _font10 = new PdfStandardFont(PdfFontFamily.Helvetica, 10);
                
                public void GenerateProof()
                {
                        PdfDocument document = new PdfDocument();
                        PdfPage page = document.Pages.Add();
                        PdfGraphics graphics = page.Graphics;
            
                        float y = 0;
                        float x = 0;
            
                        var columnWidth = TableWidth / 4;
            
                        graphics.DrawString("Comprovante Opa Games", _h1Font, PdfBrushes.Black, new PointF(x, y));
                        graphics.DrawLine(PdfPens.Black, new PointF(x, y + 20), new PointF(x + TableWidth, y + 20));
            
                        y += JumpLine;
                        y += JumpLine;
            
                        PdfBrush headerBackgroundBrush = new PdfSolidBrush(new PdfColor(0, 51, 102));
                        graphics.DrawRectangle(headerBackgroundBrush, new RectangleF(x, y, TableWidth, JumpLine));
            
                        graphics.DrawString("Data", _font10, PdfBrushes.White, new PointF(x, y));
                        graphics.DrawString("Aposta", _font10, PdfBrushes.White, new PointF(x + columnWidth, y));
                        graphics.DrawString("Saque", _font10, PdfBrushes.White, new PointF(x + columnWidth * 2, y));
                        graphics.DrawString("Multiplicador", _font10, PdfBrushes.White, new PointF(x + columnWidth * 3, y));
            
                        y += JumpLine;
            
                        DateTime[] times = {RandomDateTime(), RandomDateTime(), RandomDateTime(), RandomDateTime()};
                        string[] bets = {"12.50", "260.20", "500.00", "5.00"};
                        string[] cashOuts = {"23.00", "50.4", "1.234", "0"};
                        string[] multipliers = {"x3", "x45", "x16", "x1"};

                        for (int i = 0; i < times.Length; i++)
                        {
                                PdfBrush cellBackgroundBrush = (i % 2 == 0) ? PdfBrushes.LightGray : PdfBrushes.White;
                    
                                graphics.DrawRectangle(cellBackgroundBrush, new RectangleF(x, y, TableWidth, JumpLine));
                    
                                graphics.DrawString(times[i].ToString(CultureInfo.InvariantCulture), _font10, PdfBrushes.Black, new PointF(x, y));
                                graphics.DrawString(bets[i], _font10, PdfBrushes.Black, new PointF(x + columnWidth, y));
                                graphics.DrawString(cashOuts[i], _font10, float.Parse(cashOuts[i]) > 0f  ? PdfBrushes.Green : PdfBrushes.Black, new PointF(x + columnWidth * 2, y));
                                graphics.DrawString(multipliers[i], _font10, PdfBrushes.Black, new PointF(x + columnWidth * 3, y));
                                y += JumpLine;
                        }

                        DrawBoard(graphics, matrizExemplo, x, y);
            
                        MemoryStream stream = new MemoryStream();
                        stream.Seek(0, SeekOrigin.Begin);
                        document.Save(stream);
                        stream.Position = 0;
            
                        document.Close(true);
                        byte[] pdfBytes = stream.ToArray();
#if UNITY_WEBGL && !UNITY_EDITOR
                    using (MD5 md5 = MD5.Create())
                    {
                            byte[] checksumBytes = md5.ComputeHash(pdfBytes);
                            string checksum = BitConverter.ToString(checksumBytes).Replace("-", String.Empty);
                            Debug.Log(checksum);
                            JsHook.DownloadHandlerPDF(_fileName + CollectDateNow() + Extension, stream.ToArray(), stream.Length);
                    }
#endif
#if UNITY_EDITOR
                        //Application.ExternalCall("GeneratePDFWebGL", pdfBytes, _fileName + CollectDateNow() + Extension);
                        using (MD5 md5 = MD5.Create())
                        {
                                byte[] checksumBytes = md5.ComputeHash(pdfBytes);
                                string checksum = BitConverter.ToString(checksumBytes).Replace("-", String.Empty);
                                Debug.Log(checksum);
                                File.WriteAllBytes(Application.persistentDataPath + "/" + _fileName + CollectDateNow() + Extension, stream.ToArray());
                                System.Diagnostics.Process.Start(Application.persistentDataPath);
                        }
#endif
                }

                private string CollectDateNow() => DateTime.Now.ToString("_dd_MM_yyyy_HH_mm_ss");
                //DEPOIS DELETAR
                private DateTime RandomDateTime()
                {
                        DateTime start = new DateTime(2022, 1, 1);
                        int range = (DateTime.Today - start).Days;
                        DateTime randomDate = start.AddDays(UnityEngine.Random.Range(0, range));
                        return randomDate;
                }
                //============
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
}