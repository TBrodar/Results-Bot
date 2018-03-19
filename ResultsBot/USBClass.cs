using System;
using System.Collections.Generic;
using System.IO.Ports;

namespace ResultsBot
{
    public static class USBClass
    {
        private static List<SerialPort> OpenSerialPorts = new List<SerialPort>();

        public static IronPython.Runtime.List GetPortNames()
        {
            IronPython.Runtime.List l = new IronPython.Runtime.List();
            foreach (string s in SerialPort.GetPortNames()) l.Add(s);
            return l;
        }
        
        public static IronPython.Runtime.List GetOpenedPortNames()
        {
            IronPython.Runtime.List OpenedPortNames = new IronPython.Runtime.List();
            foreach (var sp in OpenSerialPorts)
            {
                OpenedPortNames.Add(sp.PortName);
            }
            return OpenedPortNames;
        }
		public static bool OpenPort(string PortName, int BaudRate, string Parity_None_Even_Mark_Odd_or_Space,
			string StopBits_None_One_OnePointFive_or_Two, int DataBits, string Handshake_None_RequestToSend_RequestToSendXOnXOff_or_XOnXOff,
			bool RtsEnable)
		{
			foreach (var sp in OpenSerialPorts)
			{
				if (sp.PortName == PortName)
				{
					string mesg = "Info: Port \"" + PortName + "\" is allready open. OpenPort returned false value.";
					MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
	mesg);
					MainWindow.consoleWriter.WriteLine(mesg);
					return false;
				}
			}
			SerialPort mySerialPort = new SerialPort(PortName);

			mySerialPort.BaudRate = BaudRate;
			if (Parity_None_Even_Mark_Odd_or_Space.ToLower() == "even")
			{
				mySerialPort.Parity = Parity.Even;
			} else if (Parity_None_Even_Mark_Odd_or_Space.ToLower() == "mark")
			{
				mySerialPort.Parity = Parity.Mark;
			}
			else if (Parity_None_Even_Mark_Odd_or_Space.ToLower() == "odd")
			{
				mySerialPort.Parity = Parity.Odd;
			}
			else if (Parity_None_Even_Mark_Odd_or_Space.ToLower() == "space")
			{
				mySerialPort.Parity = Parity.Space;
			} else if (Parity_None_Even_Mark_Odd_or_Space.ToLower() == "none")
			{
				mySerialPort.Parity = Parity.None;
			}

			if (StopBits_None_One_OnePointFive_or_Two.ToLower() == "one")
			{
				mySerialPort.StopBits = StopBits.One;
			} else if (StopBits_None_One_OnePointFive_or_Two.ToLower() == "onepointfive")
			{
				mySerialPort.StopBits = StopBits.OnePointFive;
			} else if (StopBits_None_One_OnePointFive_or_Two.ToLower() == "two")
			{
				mySerialPort.StopBits = StopBits.Two;
			}

			mySerialPort.DataBits = DataBits;

			if (Handshake_None_RequestToSend_RequestToSendXOnXOff_or_XOnXOff.ToLower() == "requesttosend")
			{
				mySerialPort.Handshake = Handshake.RequestToSend;
			} else if (Handshake_None_RequestToSend_RequestToSendXOnXOff_or_XOnXOff.ToLower() == "requesttosendxonxoff")
			{
				mySerialPort.Handshake = Handshake.RequestToSend;
			} else if (Handshake_None_RequestToSend_RequestToSendXOnXOff_or_XOnXOff.ToLower() == "xonxoff")
			{
				mySerialPort.Handshake = Handshake.XOnXOff;
			} else if (Handshake_None_RequestToSend_RequestToSendXOnXOff_or_XOnXOff.ToLower() == "none")
			{
				mySerialPort.Handshake = Handshake.None;
			}

			mySerialPort.RtsEnable = RtsEnable;

			// Set the read/write timeouts
			mySerialPort.ReadTimeout = 1000;
			mySerialPort.WriteTimeout = 1000;
			try
			{

				mySerialPort.Open();
			} catch (Exception)
			{
				string mesg = "Error: Can't open port \"" + PortName + "\". OpenPort returned false value.";
				MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
mesg);
				MainWindow.consoleWriter.WriteLine(mesg);
				return false;
			}
            OpenSerialPorts.Add(mySerialPort);
			return true;
        }

        public static bool SetTerminator(string PortName, string Terminator)
        {
            foreach (var sport in OpenSerialPorts)
            {
                if (sport.PortName == PortName)
                {
                    sport.NewLine = Terminator;
					return true;
                }
            }
			return false;
        }

        public static string GetTerminator(string PortName)
        {
            foreach (var sport in OpenSerialPorts)
            {
                if (sport.PortName == PortName)
                {
                    return sport.NewLine;
                }
            }
			string mesg = "Info: Port \"" + PortName + "\" is not opened. GetTerminator returned null value.";
			MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
mesg);
			MainWindow.consoleWriter.WriteLine(mesg);
			return null;
        }

        public static bool ClosePort(string PortName)
        {
            foreach(var sport in OpenSerialPorts)
            {
                if (sport.PortName == PortName)
                {
                    sport.Close();
                    OpenSerialPorts.Remove(sport);
					return true;
                }
            }
			string mesg = "Info: Port \"" + PortName + "\" is not opened. ClosePort returned null value.";
			MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
mesg);
			MainWindow.consoleWriter.WriteLine(mesg);
			return false;
        }

        public static bool Write(string PortName, string text)
        {
            SerialPort sp = null;

            foreach (var serialport in OpenSerialPorts)
            {
                if (serialport.PortName == PortName)
                {
                    sp = serialport;
                    break;
                }
            }
            if (sp == null)
            {
				string mesg = "Error: Port \"" + PortName + "\" is not open. Write function returned value false.";
				MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
	mesg);
				MainWindow.consoleWriter.WriteLine(mesg);
				return false;
            }
            try
            {
                sp.Write(text);
                return true;
            }
            catch (TimeoutException)
            {
                return false;
            } catch (Exception)
			{
				string mesg = "Error: SerialPort.Write function throwed exception. USB.Write function returned value false.";
				MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
	mesg);
				MainWindow.consoleWriter.WriteLine(mesg);
				return false;
			}
        }


        public static bool WriteLine(string PortName, string text)
        {
            SerialPort sp = null;

            foreach(var serialport in OpenSerialPorts)
            {
                if (serialport.PortName == PortName)
                {
                    sp = serialport;
                    break;
                }
            }
            if (sp == null)
            {
                MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
                    "Error: Port \"" + PortName + "\" is not open.");
				string mesg = "Error: Port \"" + PortName + "\" is not open. USB.WriteLine function returned false value.";
				MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
	mesg);
				MainWindow.consoleWriter.WriteLine(mesg);
				return false;
            }
            try
            {
                sp.WriteLine(text);
                return true;
            } catch (TimeoutException)
            {
                return false;
            } catch (Exception)
			{
				string mesg = "Error: SerialPort.WriteLine throwed exception. USB.WriteLine function returned false value.";
				MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
	mesg);
				MainWindow.consoleWriter.WriteLine(mesg);
				return false;
			}
        }
    
        public static string ReadLine(string PortName)
        {
            SerialPort sp = null;
            foreach (var serialport in OpenSerialPorts)
            {
                if (serialport.PortName == PortName)
                {
                    sp = serialport;
                    break;
                }
            }
            if (sp == null)
            {
				string mesg = "Error: Port \"" + PortName + "\" is not open. USB.ReadLine function returned null value.";
				MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
	mesg);
				MainWindow.consoleWriter.WriteLine(mesg);
				return null;
            }
            string output = "";
            try
            {
                output = sp.ReadLine();
            } catch (TimeoutException)
            {
            }
            return output;
        }


        public static string ReadChar(string PortName)
            {
                SerialPort sp = null;
                foreach (var serialport in OpenSerialPorts)
                {
                    if (serialport.PortName == PortName)
                    {
                        sp = serialport;
                        break;
                    }
                }
                if (sp == null)
                {
				string mesg = "Error: Port \"" + PortName + "\" is not open. USB.ReadChar function returned null value.";
				MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
	mesg);
				MainWindow.consoleWriter.WriteLine(mesg);
				return null;
                }
                string output = "";
                try
                {
                    output += sp.ReadChar();
                }
                catch (TimeoutException)
                {
                } 
                return output;
            }
    }
}
