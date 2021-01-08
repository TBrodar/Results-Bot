using MouseKeyboardLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResultsBot
{
    static public class MouseClass
    {
        public static TabItemAndEngineClass Instance;
        static public bool SetCursorOnPosition(IronPython.Runtime.List Position)
        {
			try {
				MouseSimulator.X = (int)Position[0];
				MouseSimulator.Y = (int)Position[1];
				return true;
			} catch (Exception e)
			{
				string mesg = "Error: Exception occured while execution SetCursorPosition(IronPython.Runtime.List Position) function. Check if list contains two integer values. \nAnother source of exception could be in MouseSimulator.X or MouseSimulator.Y set function. \nInfo: SetCursorPosition function returned value false";
				MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
mesg);
				MainWindow.consoleWriter.WriteLine(mesg);
                throw new Exception(mesg + "\n\nInternal message:\n" + e.Message + "\n" + e.StackTrace);
                return false;
			}
		}
		static public bool SetCursorOnDictionaryPosition(IronPython.Runtime.PythonDictionary Cordinates)
        {
			if (Cordinates.Keys.Contains("xy"))
			{
				return SetCursorOnPosition((IronPython.Runtime.List)Cordinates.get("xy"));
			} else
			{
				string mesg = "Error: Dictionary doesn't contain 'xy' key, can't get IronPython.Runtime.List position value from dictionariy with 'xy' key. \nInfo: SetCursorPosition function returned value false";
				MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
mesg);
				MainWindow.consoleWriter.WriteLine(mesg);
				return false;
			}
        }

        static public IronPython.Runtime.List GetCursorPosition()
        {
			try { 
            return new IronPython.Runtime.List { MouseSimulator.X, MouseSimulator.Y };
			}
			catch (Exception e)
			{
				string mesg = "Error: Mouse.GetCursorPosition failed at execution. Get function of MouseSimulator.X or MouseSimulator.Y throwed exception.";
				MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
mesg);
				MainWindow.consoleWriter.WriteLine(mesg);
                throw new Exception(mesg + "\n\nInternal message:\n" + e.Message + "\n" + e.StackTrace);

                return null;
			}

		}
       

        static public bool Click(string Right_left_or_middle_button)
        {
            MouseButton b;
            if (Right_left_or_middle_button.ToLower() == "left")
            {
                b = MouseButton.Left;
            } else if (Right_left_or_middle_button.ToLower() == "right")
            {
                b = MouseButton.Right;
            } else if (Right_left_or_middle_button.ToLower() == "middle")
            {
                b = MouseButton.Middle;
            } else
            {
				string mesg = "Error: Mouse.Click Posible Button values are \"left\", \"middle\" and \"right\"";
					MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
mesg);
				MainWindow.consoleWriter.WriteLine(mesg);
				return false;
			}
			try { 
            MouseSimulator.Click(b);
			}
			catch (Exception e)
			{
				string mesg = "Error: Mouse.Click failed at execution. MouseSimulator.Click throwed exception.";
				MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
mesg);
				MainWindow.consoleWriter.WriteLine(mesg);
                throw new Exception(mesg + "\n\nInternal message:\n" + e.Message + "\n" + e.StackTrace);
                return false;
			}
			return true;
        }
        static public bool DoubleClick(string Right_left_or_middle_button)
        {
            MouseButton b;
            if (Right_left_or_middle_button.ToLower() == "left")
            {
                b = MouseButton.Left;
            }
            else if (Right_left_or_middle_button.ToLower() == "right")
            {
                b = MouseButton.Right;
            }
            else if (Right_left_or_middle_button.ToLower() == "middle")
            {
                b = MouseButton.Middle;
            }
            else
            {
				string mesg = "Error: Mouse.DoubleClick Posible Button values are \"left\", \"middle\" and \"right\"\nInfo: DoubleClick returned value false.";
				MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
mesg);
				MainWindow.consoleWriter.WriteLine(mesg);
				return false;
            }
			try
			{
				MouseSimulator.DoubleClick(b);
			} catch (Exception e)
			{
				string mesg = "Error: Mouse.DoubleClick failed at execution. MouseSimulator.DoubleClick throwed exception.";
				MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
mesg);
				MainWindow.consoleWriter.WriteLine(mesg);
                throw new Exception(mesg + "\n\nInternal message:\n" + e.Message + "\n" + e.StackTrace);
                return false;
			}
			return true;
        }

        static public bool Wheel(int delta)
        {
			try { 
            MouseSimulator.MouseWheel(delta);
			} catch (Exception e)
			{
				string mesg = "Error: Mouse.Wheel failed at execution. MouseSimulator.Wheel throwed exception.";
				MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
mesg);
				MainWindow.consoleWriter.WriteLine(mesg);
                throw new Exception(mesg + "\n\nInternal message:\n" + e.Message + "\n" + e.StackTrace);
                return false;
			}
			return true;
		}
    }
}
