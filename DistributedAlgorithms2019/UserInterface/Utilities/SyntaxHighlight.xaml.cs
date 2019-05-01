////////////////////////////////////////////////////////////////////////////////////////////////////
/// \file UserInterface\SyntaxHighlight.xaml.cs
///
/// \brief Implements the syntax highlight.xaml class.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DistributedAlgorithms
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class SyntaxHighlight
    ///
    /// \brief The syntaxHighlight is a control which contains a text which is syntax highlighted  
    ///
    /// \par Description
    ///      The following is the way that this class works:
    ///      -#     The class gets a text to highlight and a list of class names
    ///      -#     The class divide the text into text fragments in the following way
    ///             -#  Find all the text fragments that begins with # and end with eol (the regions)
    ///             -#  Find all the text fragments which begins with " and end with "
    ///             -#  Find all the text fragments that are c# reserved words
    ///             -#  Find all the text fragments which contain a primitive type names
    ///             -#  find all the rest of the textFragments
    ///       -#    Create an ordered list according to the text begin index while resolving coalitions
    ///       -#    Put the text fragments one after the other in the text block
    ///       
    /// \par Usage Notes.
    ///
    /// \author Ilanh
    /// \date 18/10/2017
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public partial class SyntaxHighlight : UserControl
    {
        #region /// \name Class TextFragment
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \class TextFragment
        ///
        /// \brief A text fragment.
        ///
        /// \par Description.
        ///      Contains the data needed to insert a text fragment to the TextBlock
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 18/10/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public class TextFragment
        {
            /// \brief  (int) - Zero-based index of the.
            public int index;
            /// \brief  (int) - The length.
            public int length;
            /// \brief  (Brush) - The brush.
            public Brush brush;
            /// \brief  (string) - The text.
            public string text;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// \fn public TextFragment(Match m, Brush brush, string text)
            ///
            /// \brief Constructor.
            ///
            /// \par Description.
            ///
            /// \par Algorithm.
            ///
            /// \par Usage Notes.
            ///
            /// \author Ilanh
            /// \date 18/10/2017
            ///
            /// \param m      (Match) - The Match to process.
            /// \param brush  (Brush) - The brush.
            /// \param text   (string) - The text.
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public TextFragment(Match m, Brush brush, string text)
            {
                index = m.Index;
                length = m.Length;
                this.brush = brush;
                this.text = text.Substring(index, length);
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// \fn public TextFragment(int index, int length, string text, Brush brush)
            ///
            /// \brief Constructor.
            ///
            /// \par Description.
            ///
            /// \par Algorithm.
            ///
            /// \par Usage Notes.
            ///
            /// \author Ilanh
            /// \date 18/10/2017
            ///
            /// \param index   (int) - Zero-based index of the.
            /// \param length  (int) - The length.
            /// \param text    (string) - The text.
            /// \param brush   (Brush) - The brush.
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public TextFragment(int index, int length, string text, Brush brush)
            {
                this.index = index;
                this.length = length;
                this.text = text.Substring(index, length);
                this.brush = brush;
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// \fn public static List<TextFragment> ToList(List<List<TextFragment>> allLists)
            ///
            /// \brief Create a list of text fragment from a list of list of text fragments
            ///
            /// \par Description.
            ///
            /// \par Algorithm.
            ///
            /// \par Usage Notes.
            ///
            /// \author Ilanh
            /// \date 18/10/2017
            ///
            /// \param allLists  (List&lt;List&lt;TextFragment&gt;&gt;) - all lists.
            ///
            /// \return AllLists as a List&lt;TextFragment&gt;
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static List<TextFragment> ToList(List<List<TextFragment>> allLists)
            {
                List<TextFragment> result = new List<TextFragment>();
                for (int idx = 0; idx < allLists.Count; idx++)
                {
                    foreach (TextFragment tf in allLists[idx])
                    {
                        HandleCollitions(result, tf);
                    }
                }
                result = result.OrderBy(f => f.index).ToList();
                return result;
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// \fn public static void HandleCollitions(List<TextFragment> list, TextFragment item)
            ///
            /// \brief Handles the collisions.
            ///
            /// \par Description.
            ///      -  When inserting a new TextFragment a collisions with all the other text fragments has to be resolved  
            ///      -  The following are the options  
            ///         -#  If the new Text has no collisions with the existing texts - insert it
            ///         -#  If the list has a text that contains the new text - throw the new text
            ///         -#  If the new text contains texts in the list - remove the texts from the list
            ///         -#  In all the other options (partly collisions) throw the new text
            ///
            /// \par Algorithm.
            ///
            /// \par Usage Notes.
            ///
            /// \author Ilanh
            /// \date 18/10/2017
            ///
            /// \param list  (List&lt;TextFragment&gt;) - The list.
            /// \param item  (TextFragment) - The item.
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static void HandleCollitions(List<TextFragment> list, TextFragment item)
            {
                int idx = 0;
                while (idx < list.Count)
                {

                    int listItemStart = list[idx].index;
                    int listItemEnd = list[idx].index + list[idx].length - 1;
                    int itemStart = item.index;
                    int itemEnd = item.index + item.length - 1;

                    // If the item is contained in the list item - discard the item and return
                    if (listItemStart <= itemStart && itemEnd <= listItemEnd)
                    {
                        return;
                    }

                    // If the listItem is contained in the item - remove the listItem and continue
                    // (because there might be other listItem contained in item)
                    if (itemStart <= listItemStart && listItemEnd <= itemEnd)
                    {
                        list.RemoveAt(idx);
                        continue;
                    }

                    // If they are foreign - continue to check the next item in the list
                    if (itemEnd < listItemStart || listItemEnd < itemStart)
                    {
                        idx++;
                        continue;
                    }

                    // Else (partly collision) - discard the new item
                    return;
                }

                // If we reached this point - the item is not contained or partly contained
                // of any element in the list so we can add it to the list
                list.Add(item);
            }
        }
        #endregion
        #region /// \name Members

        /// \brief  (string) - The EOL.
        string eol = Environment.NewLine;

        #endregion
        #region /// \name Attributes Accessing

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \property public string Text
        ///
        /// \brief Gets the text.
        ///
        /// \return The text.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public string Text { get { return TextBlock_content.Text; } }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \property public new FontFamily FontFamily
        ///
        /// \brief Sets the font family.
        ///
        /// \return The font family.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public new FontFamily FontFamily { set { TextBlock_content.FontFamily = value; } }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \property public new int FontSize
        ///
        /// \brief Sets the size of the font.
        ///
        /// \return The size of the font.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public new int FontSize { set { TextBlock_content.FontSize = value; } }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \property public new FontStyle FontStyle
        ///
        /// \brief Sets the font style.
        ///
        /// \return The font style.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public new FontStyle FontStyle { set { TextBlock_content.FontStyle = value; } }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \property public new FontWeight FontWeight
        ///
        /// \brief Sets the font weight.
        ///
        /// \return The font weight.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public new FontWeight FontWeight { set { TextBlock_content.FontWeight = value; } }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \property public new Thickness Margin
        ///
        /// \brief Sets the margin.
        ///
        /// \return The margin.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public new Thickness Margin { set { TextBlock_content.Margin = value; } }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \property public new HorizontalAlignment HorizontalAlignment
        ///
        /// \brief Sets the horizontal alignment.
        ///
        /// \return The horizontal alignment.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public new HorizontalAlignment HorizontalAlignment { set { TextBlock_content.HorizontalAlignment = value; } }
        #endregion
        #region /// \name Constructor

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public SyntaxHighlight(string text, List<string> classes)
        ///
        /// \brief Constructor.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 18/10/2017
        ///
        /// \param text     (string) - The text.
        /// \param classes  (List&lt;string&gt;) - The classes.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public SyntaxHighlight(string text, HashSet<string> classes)
        {
            InitializeComponent();
            text = text.Replace(eol, "\n");
            text = RemoveHeadingTab(text);
            List<List<TextFragment>> list = new List<List<TextFragment>>();
            list.Add(FromToFragments("#", "\n", Brushes.DarkMagenta, text));
            list.Add(FromToFragments("\"", "\"", Brushes.Brown, text));
            list.Add(CreateTokens(text));
            list.Add(CreateClassNames(text, classes));
            list.Add(CreatePrimitiveTypes(text));
            list.Add(CreateNoHighlight(text, list));

            List<TextFragment> matchList = TextFragment.ToList(list);
            int start = 0;
            foreach (TextFragment match in matchList)
            {
                string s = text.Substring(start, match.index - start);
                TextBlock_content.Inlines.Add(new Run(s));
                TextBlock_content.Inlines.Add(new Run(match.text) { Foreground = match.brush });
                start = match.index + match.length;
            }
        }
        #endregion
        #region /// \name Utility Methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private string RemoveHeadingTab(string text)
        ///
        /// \brief Removes the heading tab described by text.
        ///
        /// \par Description.
        ///      The text given is part of a class therefor it contains a heading tab.
        ///      This method removes the heading tab from each line 
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 18/10/2017
        ///
        /// \param text  (string) - The text.
        ///
        /// \return A string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private string RemoveHeadingTab(string text)
        {
            string[] lines = text.Split(new string[] { "\n" }, 1000, StringSplitOptions.None);
            bool removeTab = lines.All(line => line == "" || char.IsWhiteSpace(line[0]));
            string result = "";
            foreach (string line in lines)
            {
                if (line.Length > 0)
                {
                    if (removeTab)
                    {
                        result += line.Substring(1) + "\n";
                    }
                    else
                    {
                        result += line + "\n";
                    }
                }
            }
            return result;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private List<TextFragment> FromToFragments(string startString, string endString, Brush brush, string text)
        ///
        /// \brief From to fragments.
        ///
        /// \par Description.
        ///      This method extract all the text fragments that start string to end string
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 18/10/2017
        ///
        /// \param startString  (string) -   The start string.
        /// \param endString    (string) -   The end string.
        /// \param brush        (Brush) - The brush.
        /// \param text         (string) - The text.
        ///
        /// \return A List&lt;TextFragment&gt;
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private List<TextFragment> FromToFragments(string startString, string endString, Brush brush, string text)
        {
            List<TextFragment> list = new List<TextFragment>();
            int end = -1;
            int start = 0;
            while ((start = text.IndexOf(startString, end + 1)) != -1)
            {
                end = text.IndexOf(endString, start + 1);
                list.Add(new TextFragment(start, end - start + 1, text, brush));
            }
            return list;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private List<TextFragment> CreateMatchList(string text, string tokens, Brush brush)
        ///
        /// \brief Creates match list.
        ///
        /// \par Description.
        ///      This method gets a string with the texts to extract, extract them from the text 
        ///      and return a list of TextFragments
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 18/10/2017
        ///
        /// \param text    (string) - The text.
        /// \param tokens  (string) - The tokens.
        /// \param brush   (Brush) - The brush.
        ///
        /// \return The new match list.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private List<TextFragment> CreateMatchList(string text, string tokens, Brush brush)
        {
            Regex regEx = new Regex(tokens);
            MatchCollection mc = regEx.Matches(text);
            List<TextFragment> list = new List<TextFragment>();
            foreach (Match m in mc)
            {
                list.Add(new TextFragment(m, brush, text));
            }
            return list;

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private string CreateTokensString(List<string> tokens)
        ///
        /// \brief Creates tokens string.
        ///
        /// \par Description.
        ///      This class gets a list of strings and generates from them a string of tokens to be 
        ///      used as an input to the regular expression
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 18/10/2017
        ///
        /// \param tokens  (List&lt;string&gt;) - The tokens.
        ///
        /// \return The new tokens string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private string CreateTokensString(HashSet<string> tokens)
        {
            string tokensString = "(";
            bool first = true;
            foreach (string token in tokens)
            {
                if (!first)
                {
                    tokensString += "|";
                }
                first = false;
                tokensString += "(" + token +")";
            }
            tokensString += ")";
            return tokensString;
        }
        #endregion
        #region /// \name Methods that create the TextFragments lists

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private List<TextFragment> CreateTokens(string text)
        ///
        /// \brief Creates the tokens.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 18/10/2017
        ///
        /// \param text  (string) - The text.
        ///
        /// \return The new tokens.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private List<TextFragment> CreateTokens(string text)
        {
            string tokens = "(auto|double|struct|break|else|long|switch|case|" +
                                         "enum|register|typedef|char|extern|return|union|const|" +
                                        "float|short|unsigned|continue|for|signed|void|default|" +
                                         "goto|sizeof|volatile|do|if|static|while|private|protected|public|" +
                                         "partial|class|true|false|List<int>|null|new|override|dynamic|base|try|catch|namespace)";
            return CreateMatchList(text, tokens, Brushes.Blue);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private List<TextFragment> CreateClassNames(string text, List<string> additionals)
        ///
        /// \brief Creates class names.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 18/10/2017
        ///
        /// \param text         (string) - The text.
        /// \param additionals  (List&lt;string&gt;) - The additionals.
        ///
        /// \return The new class names.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private List<TextFragment> CreateClassNames(string text, HashSet<string> additionals)
        {
            HashSet<string> types = TypesUtility.GetClassesForSyntaxHighlight();
            types.UnionWith(additionals);
            List<TextFragment> result = new List<TextFragment>();
            foreach (string type in types)
            {
                HashSet<string> hs = new HashSet<string> { type };
                string tokensString = CreateTokensString(hs);
                foreach (TextFragment tf in CreateMatchList(text, tokensString, Brushes.Green))
                {
                    if (types.Any(s => s == tf.text))
                    {
                        result.Add(tf);
                    }
                }
            }
            return result;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private List<TextFragment> CreatePrimitiveTypes(string text)
        ///
        /// \brief Creates primitive types.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 18/10/2017
        ///
        /// \param text  (string) - The text.
        ///
        /// \return The new primitive types.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private List<TextFragment> CreatePrimitiveTypes(string text)
        {
            List<Type> types = TypesUtility.GetPrimitiveTypes();
            HashSet<string> typesStrings = new HashSet<string>();
            types.ForEach(t => typesStrings.Add(t.ToString()));
            string tokensString = CreateTokensString(typesStrings);
            return CreateMatchList(text, tokensString, Brushes.Red);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private List<TextFragment> CreateNoHighlight(string text, List<List<TextFragment>> list)
        ///
        /// \brief Creates no highlight.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 18/10/2017
        ///
        /// \param text  (string) - The text.
        /// \param list  (List&lt;List&lt;TextFragment&gt;&gt;) - The list.
        ///
        /// \return The new no highlight.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private List<TextFragment> CreateNoHighlight(string text, List<List<TextFragment>> list)
        {
            string eol = Environment.NewLine;
            string[] tokens = text.Split(new string[] { " ", ".", ":", "\n", ",", "(", ")", "\t", ";", "[", "]" }, 1000, StringSplitOptions.RemoveEmptyEntries);
            HashSet<string> noHighlightTokens = new HashSet<string>();
            for (int idx = 0; idx < tokens.Length; idx++)
            {
                if (!list.Any(l => l.Any(e => e.text == tokens[idx])))
                {
                    if (tokens[idx][tokens[idx].Length - 1] == '"' && tokens[idx][0] != '"')
                    {
                        tokens[idx] = '"' + tokens[idx];
                    }
                    if (tokens[idx][0] == '"' && tokens[idx][tokens[idx].Length - 1] != '"')
                    {
                        tokens[idx] = tokens[idx] + '"';
                    }

                    noHighlightTokens.Add(tokens[idx]);
                }
            }

            string tokensString = CreateTokensString(noHighlightTokens);
            return CreateMatchList(text, tokensString, Brushes.Black);
        }
        #endregion
    }
}
