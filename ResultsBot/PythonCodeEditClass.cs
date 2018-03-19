using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Controls;

namespace ResultsBot
{
    public static class PythonCodeEditClass
    {
        public static string ValueString = "";
        public static IronPython.Runtime.List ValueIntArray;

        private static bool IsDictionaryDefinitionSynax(string line)
        {
            if (line.Contains("="))
            { string [] s = line.Split('=');
                if (s.Length == 2)
                { if (s[1].Contains("{") && s[1].Contains("}"))
                    for (int i = 0; i < s[1].Length; i++)
                    {   if (char.IsWhiteSpace(s[1][i])) {  continue; }
                        if (s[1][i] != '{')             { return false; } else
                            {
                                for(int j = s[1].Length-1; j>= 0; j--)
                                {
                                    if (char.IsWhiteSpace(s[1][j]))
                                    {
                                        continue;
                                    }
                                    if (s[1][j] != '}')
                                    {
                                        return false;
                                    }
                                    return true;
                                }
                            }
                    }
                }
            }
            return false;
        }
        public static bool IsDictionaryDefeinition(string line)
        {
            if (line.Contains('#')) { line = line.Substring(0, line.IndexOf('#')); }
            if (IsDictionaryDefinitionSynax(line))
            {

                string code = "proxy.SendValueString(str(type({0})))".Replace("{0}", line.Split('=')[1]);
                var source = MainWindow.MainWindowInstance.AutoCompleteEngine.CreateScriptSourceFromString(code);
                try
                {
                    source.Execute(MainWindow.MainWindowInstance.AutoCompleteScope);
                    if (ValueString.Contains("'dict'"))
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                }
            }
            return false;
        }
        public static string Dictionary_SetIntValues(string line, string key, IronPython.Runtime.List values)
        {
            string comment = "";
            if (line.Contains('#'))
            {
                int index = line.IndexOf('#');
                comment = line.Substring(index);
                line = line.Substring(0, index);
            }
            if (values.Count == 0)
            {
                return line;
            }

            string code = "a = " + line.Split('=')[1] + @"
a[" + "\"" + key + "\"" + "] = [" + ((int)values[0]).ToString("F0");

for(int i = 1; i < values.Count; i++)
            {
                code += ", " + ((int)values[i]).ToString("F0");
            }
code += @"]
proxy.SendValueString(str(a))";
            var source = MainWindow.MainWindowInstance.AutoCompleteEngine.CreateScriptSourceFromString(code, SourceCodeKind.Statements);
            try
            {
                source.Execute(MainWindow.MainWindowInstance.AutoCompleteScope);
            }
            catch (Exception ex)
            {
                var eo = MainWindow.MainWindowInstance.AutoCompleteEngine.GetService<ExceptionOperations>();
                var error = eo.FormatException(ex);

                System.Windows.MessageBox.Show(error, "There was an Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);

            }

            return line.Split('=')[0] + "=" + ValueString + comment;
        }
        public static string Dictionary_SetStringValue(string line, string key, string value)
        {
            string comment = "";
            if (line.Contains('#'))
            {
                int index = line.IndexOf('#');
                comment = line.Substring(index);
                line = line.Substring(0, index);
            }

            string code = "a = " + line.Split('=')[1] + @"
a[" + "\"" + key + "\"" + "] = \"" + value + @"""
proxy.SendValueString(str(a))";
            var source = MainWindow.MainWindowInstance.AutoCompleteEngine.CreateScriptSourceFromString(code, SourceCodeKind.Statements);
            try
            {
                source.Execute(MainWindow.MainWindowInstance.AutoCompleteScope);
            }
            catch (Exception ex)
            {
                var eo = MainWindow.MainWindowInstance.AutoCompleteEngine.GetService<ExceptionOperations>();
                var error = eo.FormatException(ex);

                System.Windows.MessageBox.Show(error, "There was an Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);

            }

            return line.Split('=')[0] + "=" + ValueString + comment;
        }


        public static IronPython.Runtime.List GetIntValues(string line, string key)
        {
            string comment = "";
            if (line.Contains('#'))
            {
                int index = line.IndexOf('#');
                comment = line.Substring(index);
                line = line.Substring(0, index);
            }

            string code = "a = " + line.Split('=')[1] + @"
proxy.SendValueInt(a[" + "\""+ key+"\"" + "])";
            var source = MainWindow.MainWindowInstance.AutoCompleteEngine.CreateScriptSourceFromString(code, SourceCodeKind.Statements);
            try
            {
                source.Execute(MainWindow.MainWindowInstance.AutoCompleteScope);
            }
            catch (Exception ex)
            {
                var eo = MainWindow.MainWindowInstance.AutoCompleteEngine.GetService<ExceptionOperations>();
                var error = eo.FormatException(ex);

                System.Windows.MessageBox.Show(error, "There was an Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            return ValueIntArray;
        }
    }

    public class TextBoxWriter : TextWriter
    {
        private TextBox _textBox;

        public TextBoxWriter(TextBox textbox)
        {
            _textBox = textbox;
        }
		
		public override void Write(char value)
        {
            base.Write(value);
            // When character data is written, append it to the text box.
            MainWindow.MainWindowInstance.ConsoleTextBox.Dispatcher.BeginInvoke(new AppendTextBoxDelegate(AppendConsole), value.ToString());
           // _textBox.AppendText(value.ToString());
        }

        public override System.Text.Encoding Encoding
        {
            get { return System.Text.Encoding.UTF8; }
        }
        private delegate void AppendTextBoxDelegate(string str);
        private void AppendConsole(string str)
        {
            if (MainWindow.MainWindowInstance.LogFileCheckBox.IsChecked == true)
            {
                MainWindow.MainWindowInstance.LogFile.Write(str);
                MainWindow.MainWindowInstance.LogFile.Flush();
            }
            _textBox.AppendText(str);
        }
    }

}