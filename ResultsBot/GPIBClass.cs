using NationalInstruments.NI4882;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ResultsBot
{
    class GPIBClass
    {
		
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>

		private class OpenDeviceClass
		{
			public int boardID = -1;
			public Device device;
			public string terminator = "\n";
		}

		private static List<OpenDeviceClass> OpenedDevices = new List<OpenDeviceClass>();

		public static IronPython.Runtime.PythonDictionary OpenDeviceFromDictionaryValues(IronPython.Runtime.PythonDictionary Device)
		{
			try
			{
				List<string> arguments = (List<string>)(Device.Keys);
				for (int i = 0; i < arguments.Count; i++)
				{
					arguments[i] = arguments[i].ToLower();
				}
				if (arguments.Contains("BoardID".ToLower()) == false || arguments.Contains("SecondaryAddress".ToLower()) == false || arguments.Contains("PrimaryAddress".ToLower()) == false)
				{
					throw (new Exception());
				}
				if (arguments.Contains("terminator") == true)
				{
					int BoardID = (int)Device.values()[arguments.IndexOf("BoardID".ToLower())];
					int PrimaryAddress = (int)Device.values()[arguments.IndexOf("PrimaryAddress".ToLower())];
					int SecondaryAddress = (int)Device.values()[arguments.IndexOf("SecondaryAddress".ToLower())];
					string terminator = (string)Device.values()[arguments.IndexOf("Terminator".ToLower())];
					return OpenDeviceAndSetLineTerminator(BoardID, PrimaryAddress, SecondaryAddress, terminator);
				}
				else
				{
					int BoardID = (int)Device.values()[arguments.IndexOf("BoardID".ToLower())];
					int PrimaryAddress = (int)Device.values()[arguments.IndexOf("PrimaryAddress".ToLower())];
					int SecondaryAddress = (int)Device.values()[arguments.IndexOf("SecondaryAddress".ToLower())];
					return OpenDevice(BoardID, PrimaryAddress, SecondaryAddress);
				}
			} catch (Exception e)
			{
				//MessageBox.Show("Error occured while opening GPIB device. Are used Device dictionary values ok? (int BoardID, int PrimaryAddress, int SecondaryAddress, string terminator)", "Error: GPIB Open function with dictionary argument");
				string mesg = "Error: GPIB Open function with dictionary argument. Error occured while opening GPIB device. Are used Device dictionary values ok? (int BoardID, int PrimaryAddress, int SecondaryAddress, string terminator)";
				MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
mesg);
				MainWindow.consoleWriter.WriteLine(mesg);
                throw new Exception(mesg + "\n\nInternal message:\n" + e.Message + "\n" + e.StackTrace);
                return new IronPython.Runtime.PythonDictionary();
			}
		}
		public static IronPython.Runtime.PythonDictionary OpenDeviceAndSetLineTerminator(int BoardID, int PrimaryAddress, int SecondaryAddress, string terminator)
		{
            IronPython.Runtime.PythonDictionary s = OpenDevice(BoardID, PrimaryAddress, SecondaryAddress);
			if (s.Count == 0) return s;
			s = SetTerminatorForDevice(BoardID, PrimaryAddress, SecondaryAddress, terminator);
			return s;
		}
		public static IronPython.Runtime.PythonDictionary OpenDevice(int BoardID, int PrimaryAddress, int SecondaryAddress)
		{
			
                IronPython.Runtime.PythonDictionary deviceList = new IronPython.Runtime.PythonDictionary();
            try
            {
                if (PrimaryAddress < 0 || PrimaryAddress > 256)
				{
					MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
					"Error: Set PrimaryAddress in range 0,1,2,...,256");
                    
					return deviceList;
				}
				if (SecondaryAddress != 0 && (SecondaryAddress < 96 || SecondaryAddress > 126))
				{
					string mesg = "Error: For SecondaryAddress set value 0 or value in range 96,97,...,126.";
					MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
	                mesg);
					MainWindow.consoleWriter.WriteLine(mesg);
					return deviceList;
				}
				foreach (OpenDeviceClass d in OpenedDevices)
			{
				if (d.boardID == BoardID && d.device.PrimaryAddress == PrimaryAddress && d.device.SecondaryAddress == SecondaryAddress)
				{
						string mesg = "Info: GPIB device is allready open.";
						MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
		                mesg);
						MainWindow.consoleWriter.WriteLine(mesg);
						return deviceList;
					}
			}
            
                var device = new Device(BoardID, (byte)PrimaryAddress, (byte)SecondaryAddress);
			//device.SynchronizeCallbacks = true;
			device.IOTimeout = TimeoutValue.T1000s;
			OpenedDevices.Add(new OpenDeviceClass() { boardID = BoardID, device = device });
            deviceList.Add(new KeyValuePair<object, object>(BoardIDString, BoardID));
            deviceList.Add(new KeyValuePair<object, object>(PrimaryAddressString, PrimaryAddress));
            deviceList.Add(new KeyValuePair<object, object>(SecondaryAddressString, SecondaryAddress));
            } catch (Exception ex)
			{
				string mesg = "Error: Exception heppend while opening GPIB device.";
				MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
                mesg);
				MainWindow.consoleWriter.WriteLine(mesg);
                throw new Exception(mesg + "\n\nInternal message:\n" + ex.Message + "\n" + ex.StackTrace);
                return deviceList;
			}
			return deviceList;
		}
        private static string BoardIDString = "BoardID";
        private static string PrimaryAddressString = "PrimaryAddress";
        private static string SecondaryAddressString = "SecondaryAddress";
        private static string TerminatorString = "Terminator";

        public static bool CloseDeviceFromDictionary(IronPython.Runtime.PythonDictionary Device)
		{
			try
			{
				List<string> arguments = (List<string>)(Device.Keys);
				if (arguments.Contains("BoardID".ToLower()) == false || arguments.Contains("SecondaryAddress".ToLower()) == false || arguments.Contains("PrimaryAddress".ToLower()) == false)
				{
					throw (new Exception());
				}
				int BoardID = (int)Device.values()[arguments.IndexOf("BoardID".ToLower())];
				int PrimaryAddress = (int)Device.values()[arguments.IndexOf("PrimaryAddress".ToLower())];
				int SecondaryAddress = (int)Device.values()[arguments.IndexOf("SecondaryAddress".ToLower())];
				return CloseDevice(BoardID, PrimaryAddress, SecondaryAddress);
			} catch (Exception e)
			{
				//MessageBox.Show("Exception occured while closing GPIB device. Check if Device python dictionary values are ok. Check if there are other reasons why exception could be thrown.", "Error: GPIB close function with dictionary argument");
				string mesg = "Error: GPIB close function with dictionary argument. Exception occured while closing GPIB device. Check if Device python dictionary values are ok. Check if there are other reasons why exception could be thrown.";
				MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
mesg);
				MainWindow.consoleWriter.WriteLine(mesg);
                throw new Exception(mesg + "\n\nInternal message:\n" + e.Message + "\n" + e.StackTrace);
                return false;
			}
		}
		public static bool CloseDevice(int BoardID, int PrimaryAddress, int SecondaryAddress)
		{
			try
			{
				foreach (OpenDeviceClass d in OpenedDevices)
				{
					if (d.boardID == BoardID && d.device.PrimaryAddress == PrimaryAddress && d.device.SecondaryAddress == SecondaryAddress)
					{
						d.device.Dispose();
						OpenedDevices.Remove(d);
						return true;
					}
				}
			} catch (Exception e)
			{
				string mesg = "Error: GPIB close function bool Close(int BoardID, int PrimaryAddress, int SecondaryAddress). Exception occured while closing GPIB device.";
				MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
mesg);
				MainWindow.consoleWriter.WriteLine(mesg);
                throw new Exception(mesg + "\n\nInternal message:\n" + e.Message + "\n" + e.StackTrace);
                return false;
			}
			return false;
		}

		public static IronPython.Runtime.PythonDictionary SetTerminatorForDeviceInDictionary(IronPython.Runtime.PythonDictionary Device)
		{
            try
			{
				List<string> arguments = (List<string>)(Device.Keys);
				if (arguments.Contains("BoardID".ToLower()) == false || arguments.Contains("SecondaryAddress".ToLower()) == false || arguments.Contains("PrimaryAddress".ToLower()) == false)
				{
					throw (new Exception());
				}
				int BoardID = (int)Device.values()[arguments.IndexOf("BoardID".ToLower())];
				int PrimaryAddress = (int)Device.values()[arguments.IndexOf("PrimaryAddress".ToLower())];
				int SecondaryAddress = (int)Device.values()[arguments.IndexOf("SecondaryAddress".ToLower())];
				string terminator = (string)Device.values()[arguments.IndexOf("terminator".ToLower())];
				return SetTerminatorForDevice(BoardID, PrimaryAddress, SecondaryAddress, terminator);
			}
			catch (Exception e)
			{
				string mesg = "Error: GPIB SetTerminator function with dictionary argument. Exception occured while setting terminator for GPIB device. Check if Device python dictionary values are ok. Check if there are other reasons why exception could be thrown.";
				MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
mesg);
				MainWindow.consoleWriter.WriteLine(mesg);
                throw new Exception(mesg + "\n\nInternal message:\n" + e.Message + "\n" + e.StackTrace);
                return new IronPython.Runtime.PythonDictionary();
			}
		}
		public static IronPython.Runtime.PythonDictionary SetTerminatorForDevice(int BoardID, int PrimaryAddress, int SecondaryAddress, string Terminator)
		{
            IronPython.Runtime.PythonDictionary deviceList = new IronPython.Runtime.PythonDictionary();

        try {
			foreach (OpenDeviceClass d in OpenedDevices)
			{
				if (d.boardID == BoardID && d.device.PrimaryAddress == PrimaryAddress && d.device.SecondaryAddress == SecondaryAddress)
				{
					d.terminator = Terminator;
                    deviceList.Add(new KeyValuePair<object, object>(BoardIDString, BoardID));
                    deviceList.Add(new KeyValuePair<object, object>(PrimaryAddressString, PrimaryAddress));
                    deviceList.Add(new KeyValuePair<object, object>(SecondaryAddressString, SecondaryAddress));
                    deviceList.Add(new KeyValuePair<object, object>(TerminatorString, Terminator));
                    return deviceList;
                }
			}
		} catch (Exception e)
			{
				string mesg = "Error: GPIB SetTerminator function bool SetTerminator(int BoardID, int PrimaryAddress, int SecondaryAddress, string terminator).";
				MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
mesg);
				MainWindow.consoleWriter.WriteLine(mesg);
                throw new Exception(mesg + "\n\nInternal message:\n" + e.Message + "\n" + e.StackTrace);
                return deviceList;
			}
			return deviceList;
		}

		public static string GetTerminatorForDeviceInDictionary(IronPython.Runtime.PythonDictionary Device)
		{
			try
			{
				List<string> arguments = (List<string>)(Device.Keys);
				if (arguments.Contains("BoardID".ToLower()) == false || arguments.Contains("SecondaryAddress".ToLower()) == false || arguments.Contains("PrimaryAddress".ToLower()) == false)
				{
					throw (new Exception());
				}
				int BoardID = (int)Device.values()[arguments.IndexOf("BoardID".ToLower())];
				int PrimaryAddress = (int)Device.values()[arguments.IndexOf("PrimaryAddress".ToLower())];
				int SecondaryAddress = (int)Device.values()[arguments.IndexOf("SecondaryAddress".ToLower())];
				return GetTerminatorForDevice(BoardID, PrimaryAddress, SecondaryAddress);
			}
			catch (Exception e)
			{
				string mesg = "Error: GPIB SetTerminator function with dictionary argument. Exception occured while setting terminator for GPIB device. Check if Device python dictionary keys/values are ok. If dictionary keys/values are ok, check if there is another reason why exception could be thrown.\nInfo: GetTerminator returned null value.";
				MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
mesg);
				MainWindow.consoleWriter.WriteLine(mesg);
                throw new Exception(mesg + "\n\nInternal message:\n" + e.Message + "\n" + e.StackTrace);
                return null;
			}
		}
		public static string GetTerminatorForDevice(int BoardID, int PrimaryAddress, int SecondaryAddress)
		{
			try
			{
				foreach (OpenDeviceClass d in OpenedDevices)
				{
					if (d.boardID == BoardID && d.device.PrimaryAddress == PrimaryAddress && d.device.SecondaryAddress == SecondaryAddress)
					{
						return d.terminator;
					}
				}
			} catch (Exception e)
			{
				string mesg = "Error: GPIB GetTerminator function string GetTerminator(int BoardID, int PrimaryAddress, int SecondaryAddress).\nInfo: GetTerminator returned null value.";
				MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
mesg);
				MainWindow.consoleWriter.WriteLine(mesg);
                throw new Exception(mesg + "\n\nInternal message:\n" + e.Message + "\n" + e.StackTrace);
                return null;
			}
			return "";
		}

		public static bool WriteLineToDeviceInDictionary(IronPython.Runtime.PythonDictionary Device, string text)
		{
			try
			{
				List<string> arguments = (List<string>)(Device.Keys);
				if (arguments.Contains("BoardID".ToLower()) == false || arguments.Contains("SecondaryAddress".ToLower()) == false || arguments.Contains("PrimaryAddress".ToLower()) == false)
				{
					throw (new Exception());
				}
				int BoardID = (int)Device.values()[arguments.IndexOf("BoardID".ToLower())];
				int PrimaryAddress = (int)Device.values()[arguments.IndexOf("PrimaryAddress".ToLower())];
				int SecondaryAddress = (int)Device.values()[arguments.IndexOf("SecondaryAddress".ToLower())];
				return WriteLineToDevice(BoardID, PrimaryAddress, SecondaryAddress, text);
			}
			catch (Exception e)
			{
				string mesg = "Error: GPIB WriteLine function with dictionary argument. Exception occured while writing text to GPIB device. Check if Device python dictionary keys/values are ok. If dictionary keys/values are ok, check if there is another reason why exception could be thrown.\nInfo: WriteLine returned false value.";
				MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
mesg);
				MainWindow.consoleWriter.WriteLine(mesg);
                throw new Exception(mesg + "\n\nInternal message:\n" + e.Message + "\n" + e.StackTrace);
                return false;
			}
		}
		public static bool WriteLineToDevice(int BoardID, int PrimaryAddress, int SecondaryAddress, string text)
		{
			try
			{
				foreach (OpenDeviceClass d in OpenedDevices)
				{
					if (d.boardID == BoardID && d.device.PrimaryAddress == PrimaryAddress && d.device.SecondaryAddress == SecondaryAddress)
					{
						d.device.Write(text + d.terminator);
                        return true;
					}
				}
			} catch (Exception e)
			{
				string mesg = "Error: GPIB WriteLine function bool WriteLine(int BoardID, int PrimaryAddress, int SecondaryAddress, string text). Check what is the reason why exception could be thrown.\nInfo: WriteLine returned false value.";
				MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus), mesg);
				MainWindow.consoleWriter.WriteLine(mesg);
                throw new Exception(mesg + "\n\nInternal message:\n" + e.Message + "\n" + e.StackTrace);
                return false;
			}
            return false;
		}

		public static string ReadLineFromDeviceInDictionary(IronPython.Runtime.PythonDictionary Device)
		{
			try
			{
				List<string> arguments = (List<string>)(Device.Keys);
				if (arguments.Contains("BoardID".ToLower()) == false || arguments.Contains("SecondaryAddress".ToLower()) == false || arguments.Contains("PrimaryAddress".ToLower()) == false)
				{
					throw (new Exception());
				}
				int BoardID = (int)Device.values()[arguments.IndexOf("BoardID".ToLower())];
				int PrimaryAddress = (int)Device.values()[arguments.IndexOf("PrimaryAddress".ToLower())];
				int SecondaryAddress = (int)Device.values()[arguments.IndexOf("SecondaryAddress".ToLower())];
				return ReadLineFromDevice(BoardID, PrimaryAddress, SecondaryAddress);
			}
			catch (Exception e)
			{
				string mesg = "Error: GPIB ReadLine function with dictionary argument string ReadLine(IronPython.Runtime.PythonDictionary Device). Exception occured while reading string from GPIB device. Check if Device python dictionary keys/values are ok. If dictionary keys/values are ok, check if there is another reason why exception could be thrown.\nInfo: ReadLine returned null value.";
				MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
mesg);
				MainWindow.consoleWriter.WriteLine(mesg);
                throw new Exception(mesg + "\n\nInternal message:\n" + e.Message + "\n" + e.StackTrace);
                return null;
			}
		}
		public static string ReadLineFromDevice(int BoardID, int PrimaryAddress, int SecondaryAddress)
		{
			try
			{ 
				foreach (OpenDeviceClass d in OpenedDevices)
				{
					if (d.boardID == BoardID && d.device.PrimaryAddress == PrimaryAddress && d.device.SecondaryAddress == SecondaryAddress)
					{
                        return d.device.ReadString();
					}
				}
			}
			catch (TimeoutException)
			{
				return "";
			} catch (Exception e)
			{
				string mesg = "Error: GPIB ReadLine function string ReadLine(int BoardID, int PrimaryAddress, int SecondaryAddress). Exception occured while reading string from GPIB device. Check if there is reason why exception could be thrown.\nInfo: ReadLine returned null value.";
				MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
mesg);
				MainWindow.consoleWriter.WriteLine(mesg);
                throw new Exception(mesg + "\n\nInternal message:\n" + e.Message + "\n" + e.StackTrace);
                return null;
			}
			return "";
		}

		public static string ReadCharFromDeviceInDictionary(IronPython.Runtime.PythonDictionary Device)
		{
			try
			{
				List<string> arguments = (List<string>)(Device.Keys);
				if (arguments.Contains("BoardID".ToLower()) == false || arguments.Contains("SecondaryAddress".ToLower()) == false || arguments.Contains("PrimaryAddress".ToLower()) == false)
				{
					throw (new Exception());
				}
				int BoardID = (int)Device.values()[arguments.IndexOf("BoardID".ToLower())];
				int PrimaryAddress = (int)Device.values()[arguments.IndexOf("PrimaryAddress".ToLower())];
				int SecondaryAddress = (int)Device.values()[arguments.IndexOf("SecondaryAddress".ToLower())];
				return ReadCharFromDevice(BoardID, PrimaryAddress, SecondaryAddress);
			}
			catch (Exception e)
			{
				string mesg = "Error: GPIB ReadChar function with dictionary argument string ReadChar(IronPython.Runtime.PythonDictionary Device). Exception occured while reading char from GPIB device. Check if Device python dictionary keys/values are ok. If dictionary keys/values are ok, check if there is another reason why exception could be thrown.\nInfo: ReadChar returned null value.";
				MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
mesg);
				MainWindow.consoleWriter.WriteLine(mesg);
                throw new Exception(mesg + "\n\nInternal message:\n" + e.Message + "\n" + e.StackTrace);
                return null;
			}
		}
		public static string ReadCharFromDevice(int BoardID, int PrimaryAddress, int SecondaryAddress)
		{
			try
			{
				foreach (OpenDeviceClass d in OpenedDevices)
				{
					if (d.boardID == BoardID && d.device.PrimaryAddress == PrimaryAddress && d.device.SecondaryAddress == SecondaryAddress)
					{
						return d.device.ReadString(1);
					}
				}
			}
			catch (TimeoutException)
			{
				return "";
			} catch (Exception e)
			{
				string mesg = "Error: GPIB ReadChar function string ReadChar(int BoardID, int PrimaryAddress, int SecondaryAddress). Exception occured while reading char from GPIB device. Check if there is reason why exception could be thrown.\nInfo: ReadChar returned null value.";
				MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
mesg);
				MainWindow.consoleWriter.WriteLine(mesg);
                throw new Exception(mesg + "\n\nInternal message:\n" + e.Message + "\n" + e.StackTrace);
                return null;
			}
			return "";
		}
	}
}
