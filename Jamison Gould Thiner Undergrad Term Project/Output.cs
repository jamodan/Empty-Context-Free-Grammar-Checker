using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EmptyGrammarAlg
{
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Prints all the productions currently in the grammar
        /// </summary>
        void PrintGrammar()
        {
            foreach (Production production in grammar)
            {
                // Terminals are saved as productions but only displayed if SHOW_DEBUG flag is set
                if (!production.isTerminal || SHOW_DEBUG)
                {
                    string left = production.variable + " --> ";
                    string right = "";
                    // Access each or block
                    foreach (var innerList in production.productionList)
                    {
                        // Read each element of the or block
                        foreach (var str in innerList)
                        {
                            right += str;
                        }
                        right += "|";
                    }
                    // remove the last or symbol from right
                    if (right.Length > 0)
                    {
                        right = right.Remove(right.Length - 1, 1);
                    }
                    Output(left + right);
                }
            }
        }

        /// <summary>
        /// Determins whether or not to display the output and color codes it depending on the message
        /// </summary>
        /// <param name="line">String to display</param>
        /// <param name="writeLine">SHould the output be an entire line default is true</param>
        /// <param name="level">Level of debug message, higher level more important default is zero</param>
        void Output(string line, bool writeLine = true, int debugLevel = int.MaxValue)
        {
            bool showMsg = false;
            if (!line.StartsWith("ERROR") && !line.StartsWith("DEBUG") && !showAsDebug)
            {
                showMsg = true;
            }
            else if (((SHOW_ERROR && line.StartsWith("DEBUG")) || (showAsDebug)) && debugLevel >= DEBUG_LEVEL)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                showMsg = true;
            }
            else if (SHOW_ERROR && line.StartsWith("ERROR"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                showMsg = true;
            }


            if (showMsg)
            {
                if (writeLine)
                {

                    WriteLine(line);
                }
                else
                {
                    Write(line);
                }

            }
            Console.ResetColor();
        }

        /// <summary>
        /// Writes a partial line
        /// </summary>
        /// <param name="line">String to write</param>
        public void Write(string line)
        {
            if (output.Text.Length == 0)
            {
                output.Text = string.Concat(output.Text, line);
            }
            else
            {
                output.Text = string.Concat(output.Text, line);
            }
        }

        /// <summary>
        /// Writes an entire line
        /// </summary>
        /// <param name="line">String to write</param>
        public void WriteLine(string line)
        {
            if (output.Text.Length == 0)
            {
                // Dont increment the line if there is currently no text to be displayed
                output.Text = string.Concat(output.Text, line);
            }
            else
            {
                output.Text = string.Concat(output.Text, "\n", line);
            }
        }
    }
}
