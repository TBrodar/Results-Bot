using System;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using System.Collections.Generic;

namespace ResultsBot
{
    public class PairOfStrings
    {
        public PairOfStrings(string name, string doc)
        {
            this.name = name;
            this.doc = doc;
        }
        public string name;
        public string doc;
    }

    public class MyCompletionData : ICompletionData
    {
        public MyCompletionData(string text)
        {
            this.Text = text;
        }

        public string DescriptionText2 = "";
        public MyCompletionData(string text, string description)
        {
            this.Text = text;
            this.DescriptionText2 = description;
        }

        public System.Windows.Media.ImageSource Image
        {
            get { return null; }
        }

        public string Text { get; private set; }

        // Use this property if you want to show a fancy UIElement in the list.
        public object Content
        {
            get { return this.Text; }
        }

        public object Description
        {
            get
            { if (this.DescriptionText2 != "")
                {return this.DescriptionText2;
                } else {  return null; }
            }
        }

        public void Complete(TextArea textArea, ISegment completionSegment,
       EventArgs insertionRequestEventArgs)
        {
            textArea.Document.Replace(completionSegment, this.Text);
        }

        public double Priority
        {
            get { return  0; }
        }

        public static List<PairOfStrings> GetMethods(Type type)
        {
            List<PairOfStrings> s = new List<PairOfStrings>();
            foreach (var method in type.GetMethods())
            {
                string parameterDescriptions = "";
                var parameters = method.GetParameters();
                if (parameters.Length > 0) parameterDescriptions = parameters[0].ParameterType + " " + parameters[0].Name;
                for (int i = 1; i < parameters.Length; i++)
                {
                    parameterDescriptions += ", " + parameters[i].ParameterType + " " + parameters[i].Name;
                }
                s.Add(new PairOfStrings( method.Name, string.Format("{0} {1} ({2})",
                                  method.ReturnType,
                                  method.Name,
                                  parameterDescriptions)));
            }
            return s;
        }

    }
}