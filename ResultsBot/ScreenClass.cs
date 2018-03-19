using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ResultsBot
{
    public static class ScreenClass
    {
        public static string SaveLoadDirectory;
        public static string SaveCaptureScreenTo(string Name)
        {
			try
			{
				Size sz = Screen.PrimaryScreen.Bounds.Size;
				IntPtr hDesk = GetDesktopWindow();
				IntPtr hSrce = GetWindowDC(hDesk);
				IntPtr hDest = CreateCompatibleDC(hSrce);
				IntPtr hBmp = CreateCompatibleBitmap(hSrce, sz.Width, sz.Height);
				IntPtr hOldBmp = SelectObject(hDest, hBmp);
				bool b = BitBlt(hDest, 0, 0, sz.Width, sz.Height, hSrce, 0, 0, CopyPixelOperation.SourceCopy | CopyPixelOperation.CaptureBlt);
				Bitmap bmp = Bitmap.FromHbitmap(hBmp);
				SelectObject(hDest, hOldBmp);
				DeleteObject(hBmp);
				DeleteDC(hDest);
				ReleaseDC(hDesk, hSrce);
				bmp.Save(Path.Combine(SaveLoadDirectory, Name) + ".bmp");
				// dictionary.Add(key, Path.Combine(SaveLoadDirectory, key) + ".bmp");
				bmp.Dispose();
				return Path.Combine(SaveLoadDirectory, Name) + ".bmp";
			} catch (Exception)
			{
				string mesg = "Error: Exception occured while execution SaveCaptureScreenTo(string Name) function. \nInfo: SaveCaptureScreenTo function returned null value.";
				MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
mesg);
				MainWindow.consoleWriter.WriteLine(mesg);
				return null;
			}
        }
        //public static string SaveCaptureScreen()
        //{
        //    return SaveCaptureScreenTo("CaptureScreenImage");
        //}
        public static string SaveCaptureScreenPortionTo(IronPython.Runtime.List Position1, IronPython.Runtime.List Position2, string name)
        {
			try
			{
				int x = (int)Position1[0];
				int y = (int)Position1[1];
				int w = ((int)Position2[0]) - x;
				int h = ((int)Position2[1]) - y;
				if (w < 1 || h < 1)
				{
					return "";
				}
				IntPtr hDesk = GetDesktopWindow();
				IntPtr hSrce = GetWindowDC(hDesk);
				IntPtr hDest = CreateCompatibleDC(hSrce);
				IntPtr hBmp = CreateCompatibleBitmap(hSrce, w, h);
				IntPtr hOldBmp = SelectObject(hDest, hBmp);
				bool b = BitBlt(hDest, 0, 0, w, h, hSrce, x, y, CopyPixelOperation.SourceCopy | CopyPixelOperation.CaptureBlt);
				Bitmap bmp = Bitmap.FromHbitmap(hBmp);
				SelectObject(hDest, hOldBmp);
				DeleteObject(hBmp);
				DeleteDC(hDest);
				ReleaseDC(hDesk, hSrce);
				string savePath = Path.Combine(SaveLoadDirectory, name);
				bmp.Save(savePath + ".bmp");
				bmp.Dispose();
				return savePath + ".bmp";
			}
			catch (Exception)
			{
				string mesg = "Error: Exception occured while execution string SaveCaptureScreenPortionTo(IronPython.Runtime.List Position1, IronPython.Runtime.List Position2, string name) function. \nInfo: SaveCaptureScreenPortionTo function returned null value.";
				MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
mesg);
				MainWindow.consoleWriter.WriteLine(mesg);
				return null;
			}
		}
        public static IronPython.Runtime.List GetPixelColorAt(IronPython.Runtime.List position)
        {
			try
			{
				int x = (int)position[0];
				int y = (int)position[1];

				IntPtr hDesk = GetDesktopWindow();
				IntPtr hSrce = GetWindowDC(hDesk);
				IntPtr hDest = CreateCompatibleDC(hSrce);
				IntPtr hBmp = CreateCompatibleBitmap(hSrce, 1, 1);
				IntPtr hOldBmp = SelectObject(hDest, hBmp);
				bool b = BitBlt(hDest, 0, 0, 1, 1, hSrce, x, y, CopyPixelOperation.SourceCopy | CopyPixelOperation.CaptureBlt);
				Bitmap bmp = Bitmap.FromHbitmap(hBmp);
				SelectObject(hDest, hOldBmp);
				DeleteObject(hBmp);
				DeleteDC(hDest);
				ReleaseDC(hDesk, hSrce);
				Color c = bmp.GetPixel(0, 0);
				bmp.Dispose();
				return new IronPython.Runtime.List
			{
				(int)c.R,
				(int)c.G,
				(int)c.B
			};
			}
			catch (Exception)
			{
				string mesg = "Error: Exception occured while execution IronPython.Runtime.List GetPixelColorAt(IronPython.Runtime.List position) function. \nInfo: GetPixelColorAt function returned null value.";
				MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
mesg);
				MainWindow.consoleWriter.WriteLine(mesg);
				return null;
			}
		}
        public static IronPython.Runtime.List GetScreenDimensions()
        {
            Size sz = Screen.PrimaryScreen.Bounds.Size;
            return new IronPython.Runtime.List
            {
                sz.Width,
                sz.Height
            };
        }

        [DllImport("gdi32.dll")]
        static extern bool BitBlt(IntPtr hdcDest, int xDest, int yDest, int
        wDest, int hDest, IntPtr hdcSource, int xSrc, int ySrc, CopyPixelOperation rop);
        [DllImport("user32.dll")]
        static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDc);
        [DllImport("gdi32.dll")]
        static extern IntPtr DeleteDC(IntPtr hDc);
        [DllImport("gdi32.dll")]
        static extern IntPtr DeleteObject(IntPtr hDc);
        [DllImport("gdi32.dll")]
        static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);
        [DllImport("gdi32.dll")]
        static extern IntPtr CreateCompatibleDC(IntPtr hdc);
        [DllImport("gdi32.dll")]
        static extern IntPtr SelectObject(IntPtr hdc, IntPtr bmp);
        [DllImport("user32.dll")]
        private static extern IntPtr GetDesktopWindow();
        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowDC(IntPtr ptr);

    }
}
