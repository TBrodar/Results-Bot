using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;

namespace ResultsBot
{
    class OCRClass
    {
        public static string GetTextFromImage(string ImagePath)
        {
            Bitmap imgsource = null;
            using (var image = new Bitmap(ImagePath))
            {
                imgsource = new Bitmap(image);
            }

            var ocrtext = string.Empty;
            try { 
            using (var engine = new Tesseract.TesseractEngine(System.IO.Path.Combine(MainWindow.WorkingDirectory, "tessdata\\"), "eng", Tesseract.EngineMode.Default))
            {
                Tesseract.TesseractEnviornment.CustomSearchPath = System.IO.Path.Combine(MainWindow.WorkingDirectory, "x86\\");
                using (var img = Tesseract.PixConverter.ToPix(imgsource))
                {
                    if (NumbersOnly == true)
                    {
                        engine.SetVariable("tessedit_char_whitelist", "0123456789.");
                    }
                    using (var page = engine.Process(img))
                    {
                        ocrtext = page.GetText();
                    }
                }
            }
            } catch (Exception e)
            {
                if (e.InnerException != null)
                {
                    MessageBox.Show(e.ToString() + "\n" + e.InnerException, "Tesseract error");
                } else
                {
                    MessageBox.Show(e.ToString(), "Tesseract error");
                }
                return "";
            }
            return ocrtext;
        }

        private static bool NumbersOnly = false;
        public static string GetNumberFromImage(string ImagePath)
        {
            NumbersOnly = true;
            string value = GetTextFromImage(ImagePath);
            NumbersOnly = false;
            return value;
        }
    }
}
