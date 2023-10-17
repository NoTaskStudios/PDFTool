using System;
using UnityEngine;
using System.Runtime.InteropServices;
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;

namespace Pdf.Utils
{
	public abstract class JsHook : MonoBehaviour
	{
		[DllImport("__Internal")]
		private static extern void GeneratePDFWebGL(string fileName, byte[] data, long size);
		public static void DownloadHandlerPDF(string fileName, byte[] data, long size)
		{
			GeneratePDFWebGL(fileName, data, size);
		}
	}
}