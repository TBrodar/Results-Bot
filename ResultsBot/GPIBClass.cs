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

		public static bool OpenDeviceFromDictionaryValues(IronPython.Runtime.PythonDictionary Device)
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
					string terminator = (string)Device.values()[arguments.IndexOf("terminator".ToLower())];
					return OpenDeviceAndSetLineTerminator(BoardID, PrimaryAddress, SecondaryAddress, terminator);
				}
				else
				{
					int BoardID = (int)Device.values()[arguments.IndexOf("BoardID".ToLower())];
					int PrimaryAddress = (int)Device.values()[arguments.IndexOf("PrimaryAddress".ToLower())];
					int SecondaryAddress = (int)Device.values()[arguments.IndexOf("SecondaryAddress".ToLower())];
					return OpenDevice(BoardID, PrimaryAddress, SecondaryAddress);
				}
			} catch (Exception)
			{
				//MessageBox.Show("Error occured while opening GPIB device. Are used Device dictionary values ok? (int BoardID, int PrimaryAddress, int SecondaryAddress, string terminator)", "Error: GPIB Open function with dictionary argument");
				string mesg = "Error: GPIB Open function with dictionary argument. Error occured while opening GPIB device. Are used Device dictionary values ok? (int BoardID, int PrimaryAddress, int SecondaryAddress, string terminator)";
				MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
mesg);
				MainWindow.consoleWriter.WriteLine(mesg);
				return false;
			}
		}
		public static bool OpenDeviceAndSetLineTerminator(int BoardID, int PrimaryAddress, int SecondaryAddress, string terminator)
		{
			bool s = OpenDevice(BoardID, PrimaryAddress, SecondaryAddress);
			if (s == false) return false;
			s = SetTerminatorForDevice(BoardID, PrimaryAddress, SecondaryAddress, terminator);
			if (s == false) return false;
			return true;
		}
		public static bool OpenDevice(int BoardID, int PrimaryAddress, int SecondaryAddress)
		{
			try {
				if (PrimaryAddress < 0 || PrimaryAddress > 256)
				{
					MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
					"Error: Set PrimaryAddress in range 0,1,2,...,256");
					return false;
				}
				if (SecondaryAddress != 0 && (SecondaryAddress < 96 || SecondaryAddress > 126))
				{
					string mesg = "Error: For SecondaryAddress set value 0 or value in range 96,97,...,126.";
					MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
	mesg);
					MainWindow.consoleWriter.WriteLine(mesg);
					return false;
				}
				foreach (OpenDeviceClass d in OpenedDevices)
			{
				if (d.boardID == BoardID && d.device.PrimaryAddress == PrimaryAddress && d.device.SecondaryAddress == SecondaryAddress)
				{
						string mesg = "Info: GPIB device is allready open.";
						MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
		mesg);
						MainWindow.consoleWriter.WriteLine(mesg);
						return false;
					}
			}
			var device = new Device(BoardID, (byte)PrimaryAddress, (byte)SecondaryAddress);
			//device.SynchronizeCallbacks = true;
			device.IOTimeout = TimeoutValue.T10ms;
			OpenedDevices.Add(new OpenDeviceClass() { boardID = BoardID, device = device });
			} catch (Exception ex)
			{
				string mesg = "Error: Exception heppend while opening GPIB device.";
				MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
mesg);
				MainWindow.consoleWriter.WriteLine(mesg);
				return false;
			}
			return true;
		}

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
			} catch (Exception)
			{
				//MessageBox.Show("Exception occured while closing GPIB device. Check if Device python dictionary values are ok. Check if there are other reasons why exception could be thrown.", "Error: GPIB close function with dictionary argument");
				string mesg = "Error: GPIB close function with dictionary argument. Exception occured while closing GPIB device. Check if Device python dictionary values are ok. Check if there are other reasons why exception could be thrown.";
				MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
mesg);
				MainWindow.consoleWriter.WriteLine(mesg);
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
			} catch (Exception)
			{
				string mesg = "Error: GPIB close function bool Close(int BoardID, int PrimaryAddress, int SecondaryAddress). Exception occured while closing GPIB device.";
				MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
mesg);
				MainWindow.consoleWriter.WriteLine(mesg);
				return false;
			}
			return false;
		}

		public static bool SetTerminatorForDeviceInDictionary(IronPython.Runtime.PythonDictionary Device)
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
			catch (Exception)
			{
				string mesg = "Error: GPIB SetTerminator function with dictionary argument. Exception occured while setting terminator for GPIB device. Check if Device python dictionary values are ok. Check if there are other reasons why exception could be thrown.";
				MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
mesg);
				MainWindow.consoleWriter.WriteLine(mesg);
				return false;
			}
		}
		public static bool SetTerminatorForDevice(int BoardID, int PrimaryAddress, int SecondaryAddress, string terminator)
		{
		try {
			foreach (OpenDeviceClass d in OpenedDevices)
			{
				if (d.boardID == BoardID && d.device.PrimaryAddress == PrimaryAddress && d.device.SecondaryAddress == SecondaryAddress)
				{
					d.terminator = terminator;
					return true;
				}
			}
		} catch (Exception)
			{
				string mesg = "Error: GPIB SetTerminator function bool SetTerminator(int BoardID, int PrimaryAddress, int SecondaryAddress, string terminator).";
				MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
mesg);
				MainWindow.consoleWriter.WriteLine(mesg);
				return false;
			}
			return false;
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
			catch (Exception)
			{
				string mesg = "Error: GPIB SetTerminator function with dictionary argument. Exception occured while setting terminator for GPIB device. Check if Device python dictionary keys/values are ok. If dictionary keys/values are ok, check if there is another reason why exception could be thrown.\nInfo: GetTerminator returned null value.";
				MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
mesg);
				MainWindow.consoleWriter.WriteLine(mesg);
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
			} catch (Exception)
			{
				string mesg = "Error: GPIB GetTerminator function string GetTerminator(int BoardID, int PrimaryAddress, int SecondaryAddress).\nInfo: GetTerminator returned null value.";
				MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
mesg);
				MainWindow.consoleWriter.WriteLine(mesg);
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
			catch (Exception)
			{
				string mesg = "Error: GPIB WriteLine function with dictionary argument. Exception occured while writing text to GPIB device. Check if Device python dictionary keys/values are ok. If dictionary keys/values are ok, check if there is another reason why exception could be thrown.\nInfo: WriteLine returned false value.";
				MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
mesg);
				MainWindow.consoleWriter.WriteLine(mesg);
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
					}
				}
			} catch (Exception)
			{
				string mesg = "Error: GPIB WriteLine function bool WriteLine(int BoardID, int PrimaryAddress, int SecondaryAddress, string text). Check what is the reason why exception could be thrown.\nInfo: WriteLine returned false value.";
				MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
mesg);
				MainWindow.consoleWriter.WriteLine(mesg);
				return false;
			}
			return true;
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
			catch (Exception)
			{
				string mesg = "Error: GPIB ReadLine function with dictionary argument string ReadLine(IronPython.Runtime.PythonDictionary Device). Exception occured while reading string from GPIB device. Check if Device python dictionary keys/values are ok. If dictionary keys/values are ok, check if there is another reason why exception could be thrown.\nInfo: ReadLine returned null value.";
				MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
mesg);
				MainWindow.consoleWriter.WriteLine(mesg);
				return null;
			}
		}
		public static string ReadLineFromDevice(int BoardID, int PrimaryAddress, int SecondaryAddress)
		{
			try
			{
				string output = "";
				string s = "";
				foreach (OpenDeviceClass d in OpenedDevices)
				{
					if (d.boardID == BoardID && d.device.PrimaryAddress == PrimaryAddress && d.device.SecondaryAddress == SecondaryAddress)
					{
						s = d.device.ReadString(1);
						if (d.terminator != "")
						{
							if (s == d.terminator)
							{
								return output;
							}
						} else
						{
							string mesg = "Info: GPIB device terminator is \"\", ReadLine() will return only one char like ReadChar()";
							MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
			mesg);
							MainWindow.consoleWriter.WriteLine(mesg);
							return s;
						}
						output += s;
					}
				}
			}
			catch (TimeoutException)
			{
				return "";
			} catch (Exception)
			{
				string mesg = "Error: GPIB ReadLine function string ReadLine(int BoardID, int PrimaryAddress, int SecondaryAddress). Exception occured while reading string from GPIB device. Check if there is reason why exception could be thrown.\nInfo: ReadLine returned null value.";
				MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
mesg);
				MainWindow.consoleWriter.WriteLine(mesg);
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
			catch (Exception)
			{
				string mesg = "Error: GPIB ReadChar function with dictionary argument string ReadChar(IronPython.Runtime.PythonDictionary Device). Exception occured while reading char from GPIB device. Check if Device python dictionary keys/values are ok. If dictionary keys/values are ok, check if there is another reason why exception could be thrown.\nInfo: ReadChar returned null value.";
				MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
mesg);
				MainWindow.consoleWriter.WriteLine(mesg);
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
			} catch (Exception)
			{
				string mesg = "Error: GPIB ReadChar function string ReadChar(int BoardID, int PrimaryAddress, int SecondaryAddress). Exception occured while reading char from GPIB device. Check if there is reason why exception could be thrown.\nInfo: ReadChar returned null value.";
				MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
mesg);
				MainWindow.consoleWriter.WriteLine(mesg);
				return null;
			}
			return "";
		}
	}
}
