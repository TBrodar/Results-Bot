using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ResultsBot
{
   public class TabItemAndEngineClass : TabItem, INotifyPropertyChanged
    {
        public TabItemAndEngineClass()
            : base()
        {
            IsAlive = false;
            IsEdited = false;
            IsPaused = false;
            FilePath = "";
            TaskInstance = null;

        }
        
        public string DataSavePath = Path.Combine(MainWindow.WorkingDirectory, "Untitled");

        private string _FilePath = "";
        public string FilePath { get { return _FilePath; }
            set {
                _FilePath = value;
                string _DataSavePath;
                if (value == "")
                {
                    _DataSavePath = Path.Combine(MainWindow.WorkingDirectory, "Untitled");
                } else
                {
                    _DataSavePath = Path.Combine(Path.GetDirectoryName(value), Path.GetFileNameWithoutExtension(value));
                }
                if (Directory.Exists(_DataSavePath) == false)
                {
                    Directory.CreateDirectory(_DataSavePath);
                }
                
                if (Directory.Exists(DataSavePath) == true && _DataSavePath != DataSavePath)
                {
                   foreach (string subfolder in MainWindow.getSubFolders(DataSavePath))
                    {
                        if (Directory.Exists(subfolder) == false) Directory.CreateDirectory(subfolder);
                        foreach (string file in Directory.GetFiles(subfolder))
                        File.Copy(DataSavePath, _DataSavePath);
                        File.Delete(DataSavePath);
                    }
                }
                DataSavePath = _DataSavePath;
            } }
        public Task TaskInstance { get; set; }
        public string RunningCode { get; set; }

        public Dictionary<string, Dictionary<string, string>> AutoCompleteValues { get; set; }

        public ScriptEngine Engine { get; set; }
        public dynamic Scope { get; set; }

        private bool _IsPaused;
        public bool IsPaused { get { return _IsPaused; } set { _IsPaused = value; OnPropertyChanged("IsPaused"); } }

        private bool _IsAlive; 
        public bool IsAlive { get { return _IsAlive; }
            set {
                if (_IsAlive == false && value == true)
                {
                    ErrorCatched = "";
                }
                _IsAlive = value;
                OnPropertyChanged("IsAlive"); } }

        private bool _IsEdited; 
        public bool IsEdited { get { return _IsEdited; } set { _IsEdited = value; OnPropertyChanged("IsEdited"); } }

        private string _ErrorCatched = "";
        public string ErrorCatched { get { return _ErrorCatched; }
            set { _ErrorCatched = value;
                if (_ErrorCatched != "")
                MainWindow.MainWindowInstance.StatusBarTextBlock.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
                    value);
            } }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }


        public object CreateAppProxy()
        {
            dynamic App = new ExpandoObject();
            App.SetAliveState = new SetBoolDelegate(x => { IsAlive = x; return true; });
            App.IsAlive   = new Func<bool>(() => { return IsAlive;  });
            App.GetErrorsString = new Func<string>(() => { return ErrorCatched; });
            App.SetPausedState = new SetBoolDelegate(x => { IsPaused = x; return true; });
            App.IsPaused = new Func<bool>(() => { return IsPaused; });
            App.SetStatus = new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus);
            App.SetClipboardText = new StringDelegate(AppClass.SetClipboardText);
            App.GetClipboardText = new ReturnStringDelegate(AppClass.GetClipboardText);
            App.ClearConsoleOutput = new VoidDelegate(() => {
                MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new VoidDelegate(() => { MainWindow.MainWindowInstance.ConsoleTextBox.Clear(); }) ); });
            return App;
        }
        
        public object CreateKeyboardProxy()
        {
            dynamic Keyboard = new ExpandoObject();
            Keyboard.GetKeys = new ReturnListDelegate(() => { return KeyboardClass.GetKeys(); });
            Keyboard.Write = new StringDelegate(x => { return KeyboardClass.Write(x); });
            Keyboard.KeyDown = new StringDelegate(x => { return KeyboardClass.KeyDown(x); });
            Keyboard.KeyUp = new StringDelegate(x => { return KeyboardClass.KeyUp(x); });
            Keyboard.KeyPress = new StringDelegate(x => { return KeyboardClass.KeyPress(x); });
            return Keyboard;
        }

		delegate IronPython.Runtime.PythonDictionary OpenDeviceAndSetLineTerminatorDelegate(int BoardID, int PrimaryAddress, int SecondaryAddress, string terminator);
		delegate IronPython.Runtime.PythonDictionary OpenDeviceDelegate(int BoardID, int PrimaryAddress, int SecondaryAddress);
        delegate bool CloseDeviceDelegate(int BoardID, int PrimaryAddress, int SecondaryAddress);
        delegate string StringReturnDeviceDelegate(int BoardID, int PrimaryAddress, int SecondaryAddress);
		delegate string StringReturnDictionaryDelegate(IronPython.Runtime.PythonDictionary dict);
		delegate bool WriteLineToDeviceInDictionaryDelegate(IronPython.Runtime.PythonDictionary Device, string text);
		delegate bool WriteLineToDeviceDelegate(int BoardID, int PrimaryAddress, int SecondaryAddress, string text);
        delegate IronPython.Runtime.PythonDictionary DictionaryDelegateDictionary(IronPython.Runtime.PythonDictionary dictionary);

        public object CreateGPIBProxy()
		{
			dynamic GPIB = new ExpandoObject();
			GPIB.OpenDeviceFromDictionaryValues = new DictionaryDelegateDictionary(GPIBClass.OpenDeviceFromDictionaryValues);
			GPIB.OpenDeviceAndSetLineTerminator = new OpenDeviceAndSetLineTerminatorDelegate(GPIBClass.OpenDeviceAndSetLineTerminator);
			GPIB.OpenDevice = new OpenDeviceDelegate(GPIBClass.OpenDevice);
			GPIB.CloseDeviceFromDictionary = new DictionaryDelegate(GPIBClass.CloseDeviceFromDictionary);
			GPIB.CloseDevice = new CloseDeviceDelegate(GPIBClass.CloseDevice);
			GPIB.SetTerminatorForDeviceInDictionary = new DictionaryDelegateDictionary(GPIBClass.SetTerminatorForDeviceInDictionary);
			GPIB.SetTerminatorForDevice = new OpenDeviceAndSetLineTerminatorDelegate(GPIBClass.SetTerminatorForDevice);
			GPIB.GetTerminatorForDeviceInDictionary = new StringReturnDictionaryDelegate(GPIBClass.GetTerminatorForDeviceInDictionary);
			GPIB.GetTerminatorForDevice = new StringReturnDeviceDelegate(GPIBClass.GetTerminatorForDevice);
			GPIB.WriteLineToDeviceInDictionary = new WriteLineToDeviceInDictionaryDelegate(GPIBClass.WriteLineToDeviceInDictionary);
			GPIB.WriteLineToDevice = new WriteLineToDeviceDelegate(GPIBClass.WriteLineToDevice);
			GPIB.ReadLineFromDeviceInDictionary = new StringReturnDictionaryDelegate(GPIBClass.ReadLineFromDeviceInDictionary);
			GPIB.ReadLineFromDevice = new StringReturnDeviceDelegate(GPIBClass.ReadLineFromDevice);
			GPIB.ReadCharFromDeviceInDictionary = new StringReturnDictionaryDelegate(GPIBClass.ReadCharFromDeviceInDictionary);
			GPIB.ReadCharFromDevice = new StringReturnDeviceDelegate(GPIBClass.ReadCharFromDevice);
			return GPIB;
		}

		public delegate void VoidDelegate();
		public delegate bool StringDictionaryDelegate(string s, IronPython.Runtime.PythonDictionary d);
		public delegate IronPython.Runtime.PythonDictionary DirectoryDictionaryDelegate(IronPython.Runtime.PythonDictionary d);
		public delegate bool SetBoolDelegate(bool value);
		public delegate bool IntDelegate(int i);
		public delegate IronPython.Runtime.List ReturnListDelegate();
		public delegate bool StringDelegate(string s);
		public delegate bool DictionaryDelegate(IronPython.Runtime.PythonDictionary Cordinates);
		public delegate bool DictionaryStringDelegate(IronPython.Runtime.List Cordinates);
		public delegate string ReturnStringDelegate();
		public delegate IronPython.Runtime.PythonDictionary GetDictonaryDelegate();
		public delegate string SaveCaptureScreenPortionToDelegate(IronPython.Runtime.List position1, IronPython.Runtime.List position2, string name);
		public delegate IronPython.Runtime.List ReturnIntArrayIntDelegate(IronPython.Runtime.List position);
		public delegate string ReturnStringStringDelegate(string name);
		public object CreateMouseProxy()
        {
            dynamic Mouse = new ExpandoObject();
            Mouse.Wheel = new IntDelegate(x => MouseClass.Wheel(x));
            Mouse.SetCursorOnDictionaryPosition = new DictionaryDelegate((x) => { MouseClass.Instance = this; return MouseClass.SetCursorOnDictionaryPosition(x); });
            Mouse.SetCursorOnPosition = new DictionaryStringDelegate((x) => { MouseClass.Instance = this; return MouseClass.SetCursorOnPosition(x); });
            Mouse.GetCursorPosition = new ReturnListDelegate(() => { return MouseClass.GetCursorPosition(); });
            Mouse.Click = new StringDelegate((x) => { MouseClass.Instance = this; return MouseClass.Click(x); });
            Mouse.DoubleClick = new StringDelegate((x) => { MouseClass.Instance = this; return MouseClass.DoubleClick(x); });

            return Mouse;
        }
        public delegate IronPython.Runtime.List StringStringDelegateList(string s1, string s2);
        public object CreateScreenProxy()
        {
            dynamic Screen = new ExpandoObject();
            //Screen.SaveCaptureScreen = new ReturnStringDelegate(() => { ScreenClass.SaveLoadDirectory = DataSavePath;return ScreenClass.SaveCaptureScreen(); });
            Screen.SaveCaptureScreenTo = new ReturnStringStringDelegate((x) => { ScreenClass.SaveLoadDirectory = DataSavePath; return ScreenClass.SaveCaptureScreenTo(x); });
            Screen.SaveCaptureScreenPortionTo = new SaveCaptureScreenPortionToDelegate( (x,y,z) => { ScreenClass.SaveLoadDirectory = DataSavePath; return ScreenClass.SaveCaptureScreenPortionTo(x,y,z); });
            Screen.GetPixelColorAt = new ReturnIntArrayIntDelegate((x) => { return ScreenClass.GetPixelColorAt(x); });
            Screen.GetScreenDimensions = new ReturnListDelegate( () => { return ScreenClass.GetScreenDimensions(); });
            Screen.Find = new StringStringDelegateList((x,y) => {
                ScreenClass.SaveLoadDirectory = DataSavePath;
                return ScreenClass.Find(x,y); });
            return Screen;
        }
       
        public object CreateOCRProxy()
        {
            dynamic OCR = new ExpandoObject();
            OCR.GetTextFromImage = new ReturnStringStringDelegate((x) => { return OCRClass.GetTextFromImage(x); });
            OCR.GetNumberFromImage = new ReturnStringStringDelegate((x) => { return OCRClass.GetNumberFromImage(x); });
            return OCR;
        }

        public delegate string GetTerminatorDelegate(string s);
        public delegate bool SetTerminatorDelegate(string s, string s2);
        public delegate IronPython.Runtime.List IListReturnDelegate();
        public delegate bool StringStringDelegate(string s1, string s2);
        public delegate bool OpenPortDelegate(string PortName, int BaudRate, string Parity_None_Even_Mark_Odd_or_Space,
            string StopBits_None_One_OnePointFive_or_Two, int DataBits, string Handshake_None_RequestToSend_RequestToSendXOnXOff_or_XOnXOff,
            bool RtsEnable);
        public object CreateUSBProxy()
        {
            dynamic USB = new ExpandoObject();
            USB.GetPortNames = new IListReturnDelegate(() => { return USBClass.GetPortNames(); });
            USB.GetOpenedPortNames = new IListReturnDelegate(() => { return USBClass.GetOpenedPortNames(); });
            USB.OpenPort = new OpenPortDelegate((x,y,z,q,w,e,r) => USBClass.OpenPort(x, y, z, q, w, e, r));
            USB.ClosePort = new StringDelegate((x) => USBClass.ClosePort(x)); //ReadLine
            USB.ReadLine = new ReturnStringStringDelegate((x) => { return USBClass.ReadLine(x); });
            USB.ReadChar = new ReturnStringStringDelegate((x) => { return USBClass.ReadChar(x); });
            USB.WriteLine = new StringStringDelegate((x,y) => { return USBClass.WriteLine(x,y); });
            USB.Write = new StringStringDelegate((x, y) => { return USBClass.Write(x, y); });
            USB.SetTerminator = new SetTerminatorDelegate((x,y) => { return USBClass.SetTerminator(x, y); });
            USB.GetTerminator = new GetTerminatorDelegate((x) => { return USBClass.GetTerminator(x); });
            return USB;
        }
    }
}
