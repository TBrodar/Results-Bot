using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System;
using System.Windows;
using System.IO;
using Microsoft.Scripting;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System.Xml;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using System.Globalization;
using System.Threading.Tasks;
using ICSharpCode.AvalonEdit.CodeCompletion;
using System.Windows.Input;
using Mono.Options;

namespace ResultsBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool Alive = false;
        static public MainWindow MainWindowInstance;
        static public string WorkingDirectory { get; set; }

        public static string DefaultCode = "";

        public ObservableCollection<TabItemAndEngineClass> TabItems { get; set; }
        private TabItemAndEngineClass _tabAdd;
		public static TextBoxWriter consoleWriter { get; set; }

		public MainWindow()
        {

            WorkingDirectory = string.Copy(Environment.CurrentDirectory);
            WorkingDirectory = WorkingDirectory.Replace("\\", "\\\\");
            MainWindowInstance = this;
            InitializeComponent();

            TabItems = new ObservableCollection<TabItemAndEngineClass>();
            _tabAdd = new TabItemAndEngineClass();
            _tabAdd.Header = "+";
            TabItems.Add(_tabAdd);

            this.AddTabItem("Untitled.py", "", DefaultCode);
            TabItems[0].IsEdited = true;

            tabDynamic.DataContext = TabItems;
            tabDynamic.SelectedIndex = 0;
            Alive = true;
            SetAutoCompleteIronPythonEngine();
			consoleWriter = new TextBoxWriter(ConsoleTextBox);
			Console.SetOut(TextWriter.Synchronized(consoleWriter));


            bool help = false;
            string file = "";
            OptionSet option_set = new OptionSet()
            .Add("?|help|h", "Prints out the help.", option => help = true)
            .Add("f=|file=",
               "Opens python code with specified path.",
               option => file = option)
            ;
            string[] args = Environment.GetCommandLineArgs();
            try
            {
                option_set.Parse(args);
            }
            catch (OptionException)
            {
            }

            if (help)
            {
                consoleWriter.WriteLine("f=PATH or file=PATH");
                consoleWriter.WriteLine("Opens python code with specified path. ");
            }

            if (file != "")
            {
                try
                {
                    using (StreamReader reader = new StreamReader(file))
                    {
                        this.AddTabItem(Path.GetFileName(file), file, reader.ReadToEnd());
                    }
                    consoleWriter.WriteLine("ResultsBot.exe -file="+file);
                }
                catch (Exception e7) { MessageBox.Show(e7.Message); }
            }
        }

        private TabItemAndEngineClass AddTabItem(string Name, string FilePath, string Text)
        {
            int count = TabItems.Count;
            
            // create new tab item
            TabItemAndEngineClass tab = new TabItemAndEngineClass();
            tab.Header = Name;
            tab.HeaderTemplate = tabDynamic.FindResource("TabHeader") as DataTemplate;
            tab.FilePath = FilePath;
            // add controls to tab item, this case I added just a textbox
            ICSharpCode.AvalonEdit.TextEditor s = new ICSharpCode.AvalonEdit.TextEditor();
            s.Name = "CodeTextEditor";
            s.FontFamily = new System.Windows.Media.FontFamily("Consolas");
            
            s.Text = Text;
            s.ShowLineNumbers = true;
            s.SyntaxHighlighting =
                    HighlightingLoader.Load(new XmlTextReader(Path.Combine(WorkingDirectory,"ICSharpCode.PythonBinding.Resources.Python.xshd")),
                       HighlightingManager.Instance);
            s.TextChanged += new EventHandler(OnTextChanged);
            s.TextArea.TextEntering += textEditor_TextArea_TextEntering;
            //s.MouseDown += S_MouseDown;
            s.TextArea.TextEntered += textEditor_TextArea_TextEntered;
            tab.Content = s;
            TabItems.Insert(count - 1, tab);
            
            return tab;
        }

        //private void S_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    var editor = (tabDynamic.SelectedContent as ICSharpCode.AvalonEdit.TextEditor);
        //    lastSelectedLine = editor.Document.Lines[editor.TextArea.Caret.Line - 1];
        //}

        private void OnTextChanged(object sender, EventArgs e)
        {
            var editor = (tabDynamic.SelectedContent as ICSharpCode.AvalonEdit.TextEditor);
            if (editor.Text.Contains("\t"))
            {
                editor.Document.Replace(editor.Text.IndexOf('\t'), 1, "    ");
            }


            if ((tabDynamic.SelectedItem as TabItemAndEngineClass).IsEdited == false)
            {
                (tabDynamic.SelectedItem as TabItemAndEngineClass).IsEdited = true;
            }
        }

        private void FillCompletionData(string AutoCompleteObject, IList<ICompletionData> data)
        {

            var editor = (tabDynamic.SelectedContent as ICSharpCode.AvalonEdit.TextEditor);
            var textAPom = (tabDynamic.SelectedContent as ICSharpCode.AvalonEdit.TextEditor).TextArea;

            // Search proxy ::

            if (AutoCompleteObject == "Mouse")
            {
                foreach( PairOfStrings p in MyCompletionData.GetMethods(typeof(MouseClass))) 
                    if (p.name != "ToString" && p.name != "GetType" && p.name != "GetHashCode" && p.name != "Equals" )
                        data.Add(new MyCompletionData(p.name, p.doc));
                return;
            } if (AutoCompleteObject == "Keyboard") 
            {
                foreach (PairOfStrings p in MyCompletionData.GetMethods(typeof(KeyboardClass)))
                    if (p.name != "ToString" && p.name != "GetType" && p.name != "GetHashCode" && p.name != "Equals")
                        data.Add(new MyCompletionData(p.name, p.doc));
                return;
            } if (AutoCompleteObject == "Screen")
            {
                foreach (PairOfStrings p in MyCompletionData.GetMethods(typeof(ScreenClass)))
                    if (p.name != "ToString" && p.name != "GetType" && p.name != "GetHashCode" && p.name != "Equals")
                        data.Add(new MyCompletionData(p.name, p.doc));
                return;
            } if (AutoCompleteObject == "App")
            {
                foreach (PairOfStrings p in MyCompletionData.GetMethods(typeof(AppClass)))
                    if (p.name != "ToString" && p.name != "GetType" && p.name != "GetHashCode" && p.name != "Equals")
                        data.Add(new MyCompletionData(p.name, p.doc));
                return;
            }
            if (AutoCompleteObject == "USB")
            {
                foreach (PairOfStrings p in MyCompletionData.GetMethods(typeof(USBClass)))
                    if (p.name != "ToString" && p.name != "GetType" && p.name != "GetHashCode" && p.name != "Equals")
                        data.Add(new MyCompletionData(p.name, p.doc));
                return;
            }
            if (AutoCompleteObject == "OCR")
            {
                foreach (PairOfStrings p in MyCompletionData.GetMethods(typeof(OCRClass)))
                    if (p.name != "ToString" && p.name != "GetType" && p.name != "GetHashCode" && p.name != "Equals")
                        data.Add(new MyCompletionData(p.name, p.doc));
                return;
            }
			if (AutoCompleteObject == "GPIB")
			{
				foreach (PairOfStrings p in MyCompletionData.GetMethods(typeof(GPIBClass)))
					if (p.name != "ToString" && p.name != "GetType" && p.name != "GetHashCode" && p.name != "Equals")
						data.Add(new MyCompletionData(p.name, p.doc));
				return;
			}
            if (AutoCompleteObject == "VISA")
            {
                foreach (PairOfStrings p in MyCompletionData.GetMethods(typeof(VISAClass)))
                    if (p.name != "ToString" && p.name != "GetType" && p.name != "GetHashCode" && p.name != "Equals")
                        data.Add(new MyCompletionData(p.name, p.doc));
                return;
            }

            // Search imports
            int index = 0;
            int start = 0;
            string line = "";
            while (index != -1)
            {
                index = editor.Text.Substring(start).IndexOf("import") + start;
                if (index == start - 1) break;
                var Docline = textAPom.Document.GetLineByOffset(index);
                line = editor.Text.Substring(Docline.Offset, Docline.Length);
                if (line.Contains("#") == true)
                {
                    line = line.Substring(0, line.IndexOf("#"));
                }
                start = Docline.Offset + Docline.Length;
                if (line.Contains(AutoCompleteObject) == false) continue;

                if        (line.Contains("as") == true && line.Substring(line.IndexOf("as") + "as".Length).Contains(AutoCompleteObject) == true)
                { }
                else if (line.Contains("from") == true && line.Substring(line.IndexOf("import") + "import".Length).Contains(AutoCompleteObject) == true)
                { }
                else if (line.Contains("from") == false && line.Contains("as") == false && line.Substring(line.IndexOf("import") + "import".Length).Contains(AutoCompleteObject) == true)
                { }
                else  { continue; }

                foreach(PairOfStrings p in GetAutoCompleteData(AutoCompleteObject, line))
                {
                        data.Add(new MyCompletionData(p.name,p.doc));
                }
                if (data.Count != 0)
                {
                        break;
                }
            }
        }
        public delegate void SetAutoCompleteDelegate(string key, string value1);
        public delegate void IListDelegate(IronPython.Runtime.List values);
        private void SetAutoCompleteIronPythonEngine()
        {
            AutoCompleteEngine = Python.CreateEngine();
            AutoCompleteScope = AutoCompleteEngine.CreateScope();
            dynamic proxy = new System.Dynamic.ExpandoObject();

            proxy.SetAutoCompleteData = new SetAutoCompleteDelegate((x, y) => AutoCompletevalues.Add(new PairOfStrings(x, y)));
            proxy.SendValueString = new SetStatusDelegate((x) => { PythonCodeEditClass.ValueString = x; });
            proxy.SendValueInt = new IListDelegate((x) => {
                PythonCodeEditClass.ValueIntArray = x;
            });
            AutoCompleteScope.proxy = proxy;
        }
        public ScriptEngine AutoCompleteEngine;
        public dynamic AutoCompleteScope;
        private List<PairOfStrings> AutoCompletevalues = new List<PairOfStrings>();

        private List<PairOfStrings> GetAutoCompleteData(string AutoCompleteObject, string line)
        {
            AutoCompletevalues.Clear();
            ICollection<string> paths = AutoCompleteEngine.GetSearchPaths();
            paths.Clear();
            paths.Add(Path.Combine(MainWindow.WorkingDirectory, "Lib"));
            paths.Add(Path.Combine(MainWindow.WorkingDirectory, "DLLs"));
            if ((tabDynamic.SelectedItem as TabItemAndEngineClass).FilePath != "")
            {
                paths.Add(Path.GetDirectoryName((tabDynamic.SelectedItem as TabItemAndEngineClass).FilePath));
            }
            AutoCompleteEngine.SetSearchPaths(paths);
            string code = line + @"
for a in dir({0}):
    if (a.__doc__ is None == False) :
        proxy.SetAutoCompleteData(str(a),str(a.__doc__))
    else :
        proxy.SetAutoCompleteData(str(a),"""")";
            code = code.Replace("{0}", AutoCompleteObject);
            var source = AutoCompleteEngine.CreateScriptSourceFromString(code);
            try
            {
                source.Execute(AutoCompleteScope);
            }
            catch (Exception ex)
            {
            }
            
            return AutoCompletevalues;
        }

        CompletionWindow completionWindow;
        void textEditor_TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == "." )
            {
                // Open code completion after the user has pressed dot:
                completionWindow = new CompletionWindow((tabDynamic.SelectedContent as ICSharpCode.AvalonEdit.TextEditor).TextArea);

                var editor = (tabDynamic.SelectedContent as ICSharpCode.AvalonEdit.TextEditor);
                var textAPom = (tabDynamic.SelectedContent as ICSharpCode.AvalonEdit.TextEditor).TextArea;

                string AutoCompleteObjectLineUpToObject = editor.Text.Substring(textAPom.Caret.Offset - (textAPom.Caret.Location.Column-1), (textAPom.Caret.Location.Column-2));
                int i;
                if (AutoCompleteObjectLineUpToObject.Length == 0) return; // Empty line with dot
                for (i = AutoCompleteObjectLineUpToObject.Length - 1; i >= 0; i--)
                {
                    if (char.IsLetterOrDigit(AutoCompleteObjectLineUpToObject[i]) == false)
                    {
                        break;
                    }
                }
                string AutoCompleteObject = AutoCompleteObjectLineUpToObject.Substring(i+1);


                FillCompletionData(AutoCompleteObject,completionWindow.CompletionList.CompletionData);
                if (completionWindow.CompletionList.CompletionData.Count < 1)
                {
                    completionWindow = null;
                    return;
                }
                completionWindow.Show();
                completionWindow.Closed += delegate {
                    completionWindow = null;
                };
            }
        }
        void textEditor_TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    // Whenever a non-letter is typed while the completion window is open,
                    // insert the currently selected element.
                    completionWindow.CompletionList.RequestInsertion(e);
                }
            }
            // Do not set e.Handled=true.
            // We still want to insert the character that was typed.
        }


        private bool _disableAddTab = false;
        private TabItemAndEngineClass _PreviusSelectedTab = null;
        private void tabDynamic_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            TabItemAndEngineClass tab = tabDynamic.SelectedItem as TabItemAndEngineClass;
            if (tab != null && tab.Header != null)
            {
                if (_disableAddTab == true) return;
                if (tab.Equals(_tabAdd))
                {
                    AddNewFile();
                }
            } else
            {
                if (tab == null) return;
                _PreviusSelectedTab = tab;

            }
        }

        public static List<string> getSubFolders(string Folderpath)
        {
            List<string> s = new List<string>();
            foreach (string path in Directory.GetDirectories(Folderpath))
            {
                s.Add(path);
                s.AddRange(getSubFolders(path));
            }
            return s;
        }

        private void StartPause_Click(object sender, RoutedEventArgs e)
        {
            if ((tabDynamic.SelectedItem as TabItemAndEngineClass) == null) return;       

            try
            {
                TabItemAndEngineClass RunningTab = tabDynamic.SelectedItem as TabItemAndEngineClass;
                if (RunningTab.IsAlive == true) {
                    if (RunningTab.IsPaused == true)
                    {
                        RunningTab.IsPaused = false;
                        MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
                        "Info: Continued.");
                    }
                    else {
                        RunningTab.IsPaused = true;
                        MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
                        "Info: Paused.");
                    return;
                    }
                } else
                {
                    RunningTab.RunningCode = (RunningTab.Content as ICSharpCode.AvalonEdit.TextEditor).Text;
                    SetStatus("Info : Started");
                    // RunningTab.TaskInstance = StartSTATask(ExecuteRunningCode(RunningTab));
                    RunningTab.TaskInstance = StartSTATask(RunningTab);
                    RunningTab.IsAlive = true;
                }
            }
            catch (MissingMemberException e4)
            {
                SetStatus("Error : " + e4.ToString());
                return;
            }           
        }

        public Task StartSTATask(TabItemAndEngineClass RunningTab)
        {
            var tcs = new TaskCompletionSource<object>();
            var thread = new System.Threading.Thread(() =>
            {
                try
                {
                    ExecuteRunningCode(RunningTab);
                    tcs.SetResult(null);
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
            });
            thread.SetApartmentState(System.Threading.ApartmentState.STA);
            thread.Start();
            return tcs.Task;
        }

        private void _SetStatus(string text) { StatusBarTextBlock.Text = text; }
        public void SetStatus(string text)
        {
            StatusBarTextBlock.Dispatcher.BeginInvoke(new SetStatusDelegate(_SetStatus), text);
        }
        public delegate void SetStatusDelegate(string text);

        public delegate void VoidDelegate();


        //delegate object ImportDelegate(IronPython.Runtime.CodeContext context, string moduleName, object globals = null, object locals = null, object fromlist = null, int level = -1);

        //protected object ImportOverride(IronPython.Runtime.CodeContext context, string moduleName, object globals = null, object locals = null, object fromlist = null, int level = -1)
        //{
        //    // do custom import logic here
        //    SetStatus("Ok");
        //    return IronPython.Modules.Builtin.__import__(context, moduleName, globals, locals, fromlist, level);
        //}
        private void ExecuteRunningCode(TabItemAndEngineClass RunningTabInstance)
        {

            RunningTabInstance.Engine = Python.CreateEngine();

            ICollection<string> paths = RunningTabInstance.Engine.GetSearchPaths();
            string LibPath = Path.Combine(MainWindow.WorkingDirectory, "Lib");
            //paths.Add(LibPath);
            foreach(string s in getSubFolders(LibPath))
            {
                paths.Add(s);
            }
            if (RunningTabInstance.FilePath != "")
            {
                paths.Add(Path.GetDirectoryName(RunningTabInstance.FilePath));
            }
            RunningTabInstance.Engine.SetSearchPaths(paths);

            var runtime = RunningTabInstance.Engine.Runtime;
            runtime.LoadAssembly(typeof(String).Assembly);
            runtime.LoadAssembly(typeof(Uri).Assembly);
            runtime.LoadAssembly(typeof(Decimal).Assembly);
            dynamic builtinscope = Python.GetBuiltinModule(RunningTabInstance.Engine);
            //builtinscope.SetVariable("__import__", new ImportDelegate(ImportOverride));
            builtinscope.App = RunningTabInstance.CreateAppProxy();
            builtinscope.Mouse = RunningTabInstance.CreateMouseProxy();
            builtinscope.Keyboard = RunningTabInstance.CreateKeyboardProxy();
            builtinscope.Screen = RunningTabInstance.CreateScreenProxy();
            builtinscope.USB = RunningTabInstance.CreateUSBProxy();
            builtinscope.OCR = RunningTabInstance.CreateOCRProxy();
			builtinscope.GPIB = RunningTabInstance.CreateGPIBProxy();
            builtinscope.VISA = RunningTabInstance.CreateVISAProxy();

            RunningTabInstance.Engine.Runtime.IO.RedirectToConsole();

            //RunningTabInstance.Scope = RunningTabInstance.Engine.CreateScope();
            //RunningTabInstance.Scope.App = RunningTabInstance.CreateAppProxy();
            //RunningTabInstance.Scope.Mouse = RunningTabInstance.CreateMouseProxy();
            //RunningTabInstance.Scope.Keyboard = RunningTabInstance.CreateKeyboardProxy();
            //RunningTabInstance.Scope.Screen = RunningTabInstance.CreateScreenProxy();
            //RunPom = RunningTabInstance;
            //RunningTabInstance.Scope.__import__ =  new ImportDelegate(ImportOverride);
            string[] files = System.IO.Directory.GetFiles(Path.Combine(Path.Combine(LibPath, "site-packages"), "ExecuteBeforeStart"));
            foreach(string f in files)
            {
                if (Path.GetExtension(f) != ".py") continue;
                //RunningTabInstance.Scope = RunningTabInstance.Engine.ExecuteFile(f, RunningTabInstance.Scope);
                RunningTabInstance.Scope = RunningTabInstance.Engine.ExecuteFile(f, builtinscope);
            }
            var source = RunningTabInstance.Engine.CreateScriptSourceFromString(RunningTabInstance.RunningCode, SourceCodeKind.Statements);
                RunningTabInstance.IsAlive = true;
           try
            {
                //source.Execute(RunningTabInstance.Scope);
                source.Execute(builtinscope);
            }
            catch (Exception ex)
            {
                var eo = RunningTabInstance.Engine.GetService<ExceptionOperations>();
                var error = eo.FormatException(ex);
                if (error.Contains("System") == true && error.Substring(error.IndexOf("System")).Contains("Exit") == true)
                {
                    MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
                         "Info: Exited (Stoped).");
                } else
                {
                    if (ex.InnerException != null) error += "\n" + ex.InnerException;
                    System.Windows.MessageBox.Show(error, "There was an Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            RunningTabInstance.IsAlive = false;
            RunningTabInstance.IsPaused = false;
            MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
                 "Info: Exited.");
        }

        
        private void CloseTab_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (TabItems.Count <= 2) return;

            if (Directory.Exists((tabDynamic.SelectedItem as TabItemAndEngineClass).DataSavePath) == true)
                foreach (var file in Directory.GetFiles((tabDynamic.SelectedItem as TabItemAndEngineClass).DataSavePath))
                {
                    if (Path.GetFileName(file).Contains("Temp") == true)
                    {
                        File.Delete(file);
                    }
                }
            try
            {
                TabItemAndEngineClass pom = tabDynamic.SelectedItem as TabItemAndEngineClass;
                 if (TabItems.IndexOf(pom) == 0)
                    {
                        pom.IsAlive = false;
                        pom.IsPaused = false;
                        TabItems.Remove(pom);
                        tabDynamic.Items.Refresh();
                    if (_PreviusSelectedTab == null)
                    {
                        tabDynamic.SelectedIndex = 0;
                    }
                    else
                    {
                        tabDynamic.SelectedItem = _PreviusSelectedTab;
                    }
                }
                    else
                    {
                        _disableAddTab = true;
                        pom.IsAlive = false;
                        pom.IsPaused = false;
                        TabItems.Remove(pom);
                        tabDynamic.Items.Refresh();
                        if (_PreviusSelectedTab == null)
                        {
                            tabDynamic.SelectedIndex = TabItems.Count - 2;
                        }else
                        {
                            tabDynamic.SelectedItem = _PreviusSelectedTab;
                        }
                        
                        _disableAddTab = false;
                    }
            }
            catch (Exception e5)
            {
                MessageBox.Show(e5.Message);
            }

        }

        private void AddNewFile()
        {
            this.AddTabItem("Untitled.py", "", DefaultCode);
            TabItems[TabItems.Count - 2].IsEdited = true;
            tabDynamic.SelectedIndex = TabItems.Count - 2;
            RenameHeaders();
        }
        private void NewFileMenuItem_Click(object sender, RoutedEventArgs e)
        {
            AddNewFile();
        }

        private void RenameHeaders()
        {
            int count = TabItems.Count;
            for (int i = 0; i < count-1; i++)
            {
                // Add number if two tabs have same header
                string shortName;
                string extension;
                if (TabItems[i].FilePath == "")
                {
                     shortName = "Untitled";
                     extension = ".py";
                } else
                {
                    shortName = Path.GetFileNameWithoutExtension(TabItems[i].FilePath);
                    extension = Path.GetExtension(TabItems[i].FilePath);
                }
                int number = -1;

                for (int j = 0; j < i; j++)
                {
                        TabItemAndEngineClass f = TabItems[j];
                        string exte;
                        string shortHeader;
                        if (f.FilePath != "")
                        {
                            exte = "Untitled";
                            shortHeader = ".py";
                        }
                        else
                        {
                            exte = Path.GetExtension(f.Header as string);
                            shortHeader = Path.GetFileNameWithoutExtension(f.Header as string);
                        }
                        if (extension != exte) continue;


                        if (shortHeader == shortName && number == -1)
                        {
                            number = 0;
                        }
                        if (shortHeader.Contains("(") && shortHeader.Contains(")"))
                        {

                            string poms = shortHeader.Substring(0, shortHeader.LastIndexOf('('));
                            if (poms == shortName)
                            {
                                int pomInt = -1;
                                string numstr = shortHeader.Split('(')[shortHeader.Split('(').Length - 1];
                                if (numstr[numstr.Length - 1].Equals(')'))
                                {
                                    if (int.TryParse(numstr.Substring(0, numstr.Length - 1), System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out pomInt))
                                    {
                                        if (pomInt > number)
                                        {
                                            number = pomInt;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (number != -1)
                    {
                        TabItems[i].Header = shortName + "(" + (number + 1).ToString("N0", CultureInfo.InvariantCulture) + ")" + extension;
                    } else
                    {
                        TabItems[i].Header = Path.GetFileName(shortName + extension);
                    }
            }
            
        }

        private void OpenFileMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Filter = "Python files (*.py)|*.py|Text files (*.txt)|*.txt|All files (*)|*";
            dialog.FilterIndex = 0;
            dialog.Multiselect = true;

            if (dialog.ShowDialog() == true)
            {
                foreach (string FilePath in dialog.FileNames)
                {
                    bool AllreadyExists = false;
                    foreach(TabItemAndEngineClass p in TabItems)
                    {
                        if (p.FilePath == FilePath)
                        {
                            AllreadyExists = true;
                        }
                    }
                    if (AllreadyExists == true)
                    {
                        SetStatus("Info : File " + FilePath + " is allready open.");
                        continue;
                    }
                    try { 
                    using(StreamReader reader = new StreamReader(FilePath))
                    {
                          this.AddTabItem(Path.GetFileName(FilePath),FilePath, reader.ReadToEnd());
                    }
                    } catch (Exception e7) { MessageBox.Show(e7.Message); }
                }
            }
            if (TabItems.Count == 3)
            {
                if (TabItems[0].FilePath == "" && (TabItems[0].Content as ICSharpCode.AvalonEdit.TextEditor).Text == "")
                {
                    TabItems.RemoveAt(0);
                    tabDynamic.Items.Refresh();
                }
            }
            tabDynamic.SelectedIndex = TabItems.Count - 2;
            RenameHeaders();
        }

        private void SaveFileMenuItem_Click(object sender, RoutedEventArgs e)
        {
            string FilePath;
            if ((tabDynamic.SelectedItem as TabItemAndEngineClass).FilePath == "")
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Python files (*.py)|*.py|Text files (*.txt)|*.txt|All files (*)|*";
                saveFileDialog.FilterIndex = 0;
                if (saveFileDialog.ShowDialog() == true)
                {
                    FilePath = saveFileDialog.FileName;
                } else { return;  }
            } else
            {
                FilePath = (tabDynamic.SelectedItem as TabItemAndEngineClass).FilePath;
            }
            
            File.WriteAllText(FilePath, ((tabDynamic.SelectedItem as TabItemAndEngineClass).Content as ICSharpCode.AvalonEdit.TextEditor).Text);
            (tabDynamic.SelectedItem as TabItemAndEngineClass).FilePath = FilePath;
            (tabDynamic.SelectedItem as TabItemAndEngineClass).IsEdited = false;
            (tabDynamic.SelectedItem as TabItemAndEngineClass).Header = Path.GetFileName(FilePath);
        }

        private void SaveFileAsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = Path.GetFileName((tabDynamic.SelectedItem as TabItemAndEngineClass).FilePath);
            saveFileDialog.Filter = "Python files (*.py)|*.py|Text files (*.txt)|*.txt|All files (*)|*";
            saveFileDialog.FilterIndex = 0;
            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, ((tabDynamic.SelectedItem as TabItemAndEngineClass).Content as ICSharpCode.AvalonEdit.TextEditor).Text);
                //string oldFilePath = (tabDynamic.SelectedItem as TabItemAndEngineClass).FilePath;
                (tabDynamic.SelectedItem as TabItemAndEngineClass).FilePath = saveFileDialog.FileName;
                (tabDynamic.SelectedItem as TabItemAndEngineClass).IsEdited = false;
                (tabDynamic.SelectedItem as TabItemAndEngineClass).Header = Path.GetFileName(saveFileDialog.FileName);
               // copyFiles(oldFilePath, (tabDynamic.SelectedItem as TabItemAndEngineClass).FilePath);
                
            }           
        }
        private void copyFiles(string source, string destination)
        {
            string[] Files = System.IO.Directory.GetFiles(source);
            foreach (var file in Files)
            {
                System.IO.File.Copy(file, Path.Combine(destination, Path.GetFileName(file)), true);
            }
            string[] Folders = System.IO.Directory.GetDirectories(source);
            foreach (var folder in Folders)
            {
                copyFiles(folder, destination);
            }
        }
        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result =
                  MessageBox.Show(
                    "Exit?",
                    "Exit ResultsBot?",
                    MessageBoxButton.YesNo);
                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            Alive = false;
            if (LogFile != null) LogFile.Close();
            // Add abort Iron Python call if necessary (IronPython exec Threads are deamon, so they exit)

            foreach (TabItemAndEngineClass s in TabItems)
            {
                s.IsAlive = false;
                if (Directory.Exists(s.DataSavePath) == true)
                    foreach (var file in Directory.GetFiles(s.DataSavePath))
                    {
                        if (Path.GetFileName(file).Contains("Temp") == true)
                        {
                            File.Delete(file);
                        }
                    }
            }

            foreach (string USBport in USBClass.GetOpenedPortNames())
            {
                USBClass.ClosePort(USBport);
            }
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Alive = false;
            // Add abort Iron Python call if necessary (IronPython exec Threads are deamon, so they exit)

            foreach (TabItemAndEngineClass s in TabItems)
            {
                s.IsAlive = false;
                if (Directory.Exists(s.DataSavePath) == true)
                    foreach (var file in Directory.GetFiles(s.DataSavePath))
                    {
                        if (Path.GetFileName(file).Contains("Temp") == true)
                        {
                            File.Delete(file);
                        }
                    }
            }
            MainWindow.MainWindowInstance.Close();
        }
        
        private void StopMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if ((tabDynamic.SelectedItem as TabItemAndEngineClass) == null) return;
            (tabDynamic.SelectedItem as TabItemAndEngineClass).IsAlive = false;
        }

        public void ShowMousePositionAndPixelColorCheckBox_Change(object sender, RoutedEventArgs e)
        {
            if (ShowMousePositionAndPixelColorCheckBox.IsChecked == true)
            {
                ShowMousePositionAndPixelColorCheckBox.IsChecked = false;
                ShowMousePositionAndPixelColorCheckBoxComboBoxIsChecked = false;
            } else
            {
                ShowMousePositionAndPixelColorCheckBox.IsChecked = true;
                ShowMousePositionAndPixelColorCheckBoxComboBoxIsChecked = true;
            }
        }
        public void ShowMousePositionAndPixelColor(object sender, RoutedEventArgs e)
        {
            if (ShowMousePositionAndPixelColorCheckBox.IsChecked == true)
            {
                ShowMousePositionAndPixelColorCheckBoxComboBoxIsChecked = true;
                Task a = new Task(ShowMousePositionAndPixelColorLoop);
                a.Start();
            } else
            {
                ShowMousePositionAndPixelColorCheckBoxComboBoxIsChecked = false;
            }
            
        }

        public bool ShowMousePositionAndPixelColorCheckBoxComboBoxIsChecked = false;
        private void ShowMousePositionAndPixelColorLoop()
        {
            while (ShowMousePositionAndPixelColorCheckBoxComboBoxIsChecked == true)
            {
                string s = string.Format("Position [{0:F0},{1:F0}]",MouseKeyboardLibrary.MouseSimulator.X, MouseKeyboardLibrary.MouseSimulator.Y);
                var list = ScreenClass.GetPixelColorAt(new IronPython.Runtime.List() { MouseKeyboardLibrary.MouseSimulator.X, MouseKeyboardLibrary.MouseSimulator.Y });
                s += string.Format(", Color [{0:F0},{1:F0},{2:F0}]",(int)list[0], (int)list[1], (int)list[2]);
                StatusBarTextBlock.Dispatcher.BeginInvoke(new SetStatusDelegate(SetStatus), s);
                System.Threading.Thread.Sleep(100);
            }
        }

            public void HelpButton(object sender, RoutedEventArgs e)
        {
            string path = Path.Combine(WorkingDirectory, "Readme.txt");
            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    this.AddTabItem("Readme.txt", path, reader.ReadToEnd());
                }
            (tabDynamic.SelectedContent as ICSharpCode.AvalonEdit.TextEditor).IsReadOnly = true;
            (tabDynamic.SelectedContent as ICSharpCode.AvalonEdit.TextEditor).SyntaxHighlighting = null;
            }
            catch (Exception e7) { MessageBox.Show(e7.Message); }
        }

        //ICSharpCode.AvalonEdit.Document.DocumentLine lastSelectedLine;

        public void SetF1Position(object sender, RoutedEventArgs e)
        {
        try { 
            var editor = (tabDynamic.SelectedContent as ICSharpCode.AvalonEdit.TextEditor);

            var line = editor.Document.Lines[editor.TextArea.Caret.Line - 1];
            string text = editor.Text.Substring(line.Offset, line.Length);
            if (PythonCodeEditClass.IsDictionaryDefeinition(text))
            {
                editor.Document.Replace(line.Offset, line.Length, PythonCodeEditClass.Dictionary_SetIntValues(text,F1TextBox.Text,new IronPython.Runtime.List {MouseKeyboardLibrary.MouseSimulator.X, MouseKeyboardLibrary.MouseSimulator.Y }));
            }
        } catch (Exception)
            {
                MessageBox.Show("Error occured at setup action.", "Error: Setup");
            }
}
        public void SetF2Position(object sender, RoutedEventArgs e)
        {
        try { 
            var editor = (tabDynamic.SelectedContent as ICSharpCode.AvalonEdit.TextEditor);

            var line = editor.Document.Lines[editor.TextArea.Caret.Line - 1];
            string text = editor.Text.Substring(line.Offset,line.Length);
            if (PythonCodeEditClass.IsDictionaryDefeinition(text))
            {
                editor.Document.Replace(line.Offset, line.Length, PythonCodeEditClass.Dictionary_SetIntValues(text, F2TextBox.Text, new IronPython.Runtime.List { MouseKeyboardLibrary.MouseSimulator.X, MouseKeyboardLibrary.MouseSimulator.Y }));
            }
        } catch (Exception)
            {
                MessageBox.Show("Error occured at setup action.", "Error: Setup");
            }
}
        public void SetF3Position(object sender, RoutedEventArgs e)
        {
        try { 
            var editor = (tabDynamic.SelectedContent as ICSharpCode.AvalonEdit.TextEditor);

            var line = editor.Document.Lines[editor.TextArea.Caret.Line - 1];
            string text = editor.Text.Substring(line.Offset,  line.Length);
            if (PythonCodeEditClass.IsDictionaryDefeinition(text))
            {
                editor.Document.Replace(line.Offset, line.Length, PythonCodeEditClass.Dictionary_SetIntValues(text, F3TextBox.Text, new IronPython.Runtime.List { MouseKeyboardLibrary.MouseSimulator.X, MouseKeyboardLibrary.MouseSimulator.Y }));
            }
        } catch (Exception)
            {
                MessageBox.Show("Error occured at setup action.", "Error: Setup");
            }
}
        public void CapturePixelColor(object sender, RoutedEventArgs e)
        {
            try { 
            var editor = (tabDynamic.SelectedContent as ICSharpCode.AvalonEdit.TextEditor);
            
            var line = editor.Document.Lines[editor.TextArea.Caret.Line - 1];
            string text = editor.Text.Substring(line.Offset, line.Length);
            if (PythonCodeEditClass.IsDictionaryDefeinition(text))
            {
                IronPython.Runtime.List position = new IronPython.Runtime.List();
                position.Add(MouseKeyboardLibrary.MouseSimulator.X);
                position.Add(MouseKeyboardLibrary.MouseSimulator.Y);
                editor.Document.Replace(line.Offset, line.Length, PythonCodeEditClass.Dictionary_SetIntValues(text, PixelColorName.Text, ScreenClass.GetPixelColorAt(position)));
            }
        } catch (Exception)
            {
                MessageBox.Show("Error occured at setup action.", "Error: Setup");
            }
}


            public void CaptureImageFromX1Y1X2Y2Positions(object sender, RoutedEventArgs e)
        {
            try {
            var editor = (tabDynamic.SelectedContent as ICSharpCode.AvalonEdit.TextEditor);

            var line = editor.Document.Lines[editor.TextArea.Caret.Line - 1];
            string text = editor.Text.Substring(line.Offset, line.Length);
            if (PythonCodeEditClass.IsDictionaryDefeinition(text))
            {
                ScreenClass.SaveLoadDirectory = (tabDynamic.SelectedItem as TabItemAndEngineClass).DataSavePath;
                string path = ScreenClass.SaveCaptureScreenPortionTo(PythonCodeEditClass.GetIntValues(text,Position1TextBox.Text), PythonCodeEditClass.GetIntValues(text, Position2TextBox.Text), CapturedScreenName.Text);
                editor.Document.Replace(line.Offset, line.Length, PythonCodeEditClass.Dictionary_SetStringValue(text, CapturedScreenName.Text, path));
            }
            } catch (Exception)
            {
                MessageBox.Show("Error occured at setup action.", "Error: Setup");
            }
        }

        private void ShowConsoleToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if ( ShowConsoleToggleButton.IsChecked == true)
            {
                ConsoleTextBox.Visibility = Visibility.Visible;
                ConsoleHeightDefinition.Height = (GridLength)((new GridLengthConverter())).ConvertFromString("*");
            } else
            {
                ConsoleTextBox.Visibility = Visibility.Collapsed;
                ConsoleHeightDefinition.Height = (GridLength)((new GridLengthConverter())).ConvertFromString("0");
            }
        }

        public StreamWriter LogFile;
        private void LogFileCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (LogFileCheckBox.IsChecked == true)
            {
                if (LogFileTextBox.Text == "")
                {
                    LogFileCheckBox.IsChecked = false;
                    StatusBarTextBlock.Text = "Info : Enter name of log file.";
                    return;
                }
                try { 
                if (File.Exists(Path.Combine(WorkingDirectory, LogFileTextBox.Text)) == false)
                {
                    LogFile = new StreamWriter(Path.Combine(WorkingDirectory, LogFileTextBox.Text), false, System.Text.Encoding.UTF8);
                } else
                {
                    LogFile = new StreamWriter(Path.Combine(WorkingDirectory, LogFileTextBox.Text), true, System.Text.Encoding.UTF8);
                }
                LogFileTextBox.IsEnabled = false;
                } catch (Exception)
                {
                    MessageBox.Show("Error occured while opening log file.", "IO Error");
                }
            } else
            {
                if (LogFile == null) return;
                LogFileTextBox.IsEnabled = true;
                LogFile.Flush();
                LogFile.Close();
            }
        }

        //public void SetFocusOnMainWindow()
        //{
        //    if (!IsVisible)
        //    {
        //        Show();
        //    }

        //    if (WindowState == WindowState.Minimized)
        //    {
        //        WindowState = WindowState.Normal;
        //    }

        //    Activate();
        //    Topmost = true;  // important
        //    Topmost = false; // important
        //    Focus();         // important
        //}


    }
}
