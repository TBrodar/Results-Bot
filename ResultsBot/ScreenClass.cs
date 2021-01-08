using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
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
                string savePath = "";
                if (Path.HasExtension(Name) == false || Path.GetExtension(Name).Contains("bmp"))
                {
                    Name += ".bmp";
                    savePath = Path.Combine(SaveLoadDirectory, Name);
                    bmp.Save(savePath);
                }
                else if (Path.GetExtension(Name).Contains("jpg") || Path.GetExtension(Name).Contains("jpeg"))
                {
                    savePath = Path.Combine(SaveLoadDirectory, Name);
                    bmp.Save(savePath, ImageFormat.Jpeg);
                }
                else if (Path.GetExtension(Name).Contains("png"))
                {
                    savePath = Path.Combine(SaveLoadDirectory, Name);
                    bmp.Save(savePath, ImageFormat.Png);
                }
                bmp.Dispose();
                return savePath;
			} catch (Exception e)
			{
                
				string mesg = "Error: Exception occured while execution SaveCaptureScreenTo(string Name) function. \nInfo: SaveCaptureScreenTo function returned null value.";
				MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
mesg);
				MainWindow.consoleWriter.WriteLine(mesg);
                throw new Exception(mesg + "\n\nInternal message:\n" + e.Message + "\n" + e.StackTrace);
                
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
                string savePath = "";
                if (Path.HasExtension(name) == false || Path.GetExtension(name).Contains("bmp"))
                {
                    name += ".bmp";
                    savePath = Path.Combine(SaveLoadDirectory, name);
                    bmp.Save(savePath);
                }
                else if (Path.GetExtension(name).Contains("jpg") || Path.GetExtension(name).Contains("jpeg"))
                {
                    savePath = Path.Combine(SaveLoadDirectory, name);
                    bmp.Save(savePath, ImageFormat.Jpeg);
                } else if (Path.GetExtension(name).Contains("png"))
                {
                    savePath = Path.Combine(SaveLoadDirectory, name);
                    bmp.Save(savePath, ImageFormat.Png);
                }

                bmp.Dispose();
				return savePath;
			}
			catch (Exception e)
			{
				string mesg = "Error: Exception occured while execution string SaveCaptureScreenPortionTo(IronPython.Runtime.List Position1, IronPython.Runtime.List Position2, string name) function. \nInfo: SaveCaptureScreenPortionTo function returned null value.";
				MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
mesg);
				MainWindow.consoleWriter.WriteLine(mesg);
                throw new Exception(mesg + "\n\nInternal message:\n" + e.Message + "\n" + e.StackTrace);
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
			catch (Exception e)
			{
				string mesg = "Error: Exception occured while execution IronPython.Runtime.List GetPixelColorAt(IronPython.Runtime.List position) function. \nInfo: GetPixelColorAt function returned null value.";
				MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
mesg);
				MainWindow.consoleWriter.WriteLine(mesg);

                throw new Exception(mesg + "\n\nInternal message:\n" + e.Message + "\n" + e.StackTrace);
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

        public static IronPython.Runtime.List Find(string BmpHaystackName, string BmpNeedleName)
        {
            Bitmap haystack = null;
            Bitmap needle = null;
            IronPython.Runtime.List Points = new IronPython.Runtime.List();
            if (Path.HasExtension(BmpHaystackName) == false)
            {
                BmpHaystackName += ".bmp";
            } else { return Points; }
            if (Path.HasExtension(BmpNeedleName) == false)
            {
                BmpNeedleName += ".bmp";
            } else { return Points; }

            using (var image = new Bitmap(Path.Combine(SaveLoadDirectory, BmpHaystackName)))
            {
                haystack = new Bitmap(image);
            }
            using (var image = new Bitmap(Path.Combine(SaveLoadDirectory, BmpNeedleName)))
            {
                needle = new Bitmap(image);
            }
            
            
            if (null == haystack || null == needle)
            {
                return Points;
            }
            if (haystack.Width < needle.Width || haystack.Height < needle.Height)
            {
                return Points;
            }

            var haystackArray = GetPixelArray(haystack);
            var needleArray = GetPixelArray(needle);
            
            foreach (var firstLineMatchPoint in FindMatch(haystackArray.Take(haystack.Height - needle.Height), needleArray[0]))
            {
                if (IsNeedlePresentAtLocation(haystackArray, needleArray, firstLineMatchPoint, 1))
                {
                    Points.Add(new IronPython.Runtime.List() { firstLineMatchPoint.X, firstLineMatchPoint.Y });
                }
            }

            return Points;
        }

        private static int[][] GetPixelArray(Bitmap bitmap)
        {
            var result = new int[bitmap.Height][];
            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);

            for (int y = 0; y < bitmap.Height; ++y)
            {
                result[y] = new int[bitmap.Width];
                Marshal.Copy(bitmapData.Scan0 + y * bitmapData.Stride, result[y], 0, result[y].Length);
            }

            bitmap.UnlockBits(bitmapData);

            return result;
        }

        private static IEnumerable<Point> FindMatch(IEnumerable<int[]> haystackLines, int[] needleLine)
        {
            var y = 0;
            foreach (var haystackLine in haystackLines)
            {
                for (int x = 0, n = haystackLine.Length - needleLine.Length; x < n; ++x)
                {
                    if (ContainSameElements(haystackLine, x, needleLine, 0, needleLine.Length))
                    {
                        yield return new Point(x, y);
                    }
                }
                y += 1;
            }
        }

        private static bool ContainSameElements(int[] first, int firstStart, int[] second, int secondStart, int length)
        {
            for (int i = 0; i < length; ++i)
            {
                if (first[i + firstStart] != second[i + secondStart])
                {
                    return false;
                }
            }
            return true;
        }

        private static bool IsNeedlePresentAtLocation(int[][] haystack, int[][] needle, Point point, int alreadyVerified)
        {
            //we already know that "alreadyVerified" lines already match, so skip them
            for (int y = alreadyVerified; y < needle.Length; ++y)
            {
                if (!ContainSameElements(haystack[y + point.Y], point.X, needle[y], 0, needle.Length))
                {
                    return false;
                }
            }
            return true;
        }

    }
}
