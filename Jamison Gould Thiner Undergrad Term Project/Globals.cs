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
        // Environment Variables
        public const int TEST_DATA_SET = 0;
        public const bool SHOW_ERROR = true;
        public const bool SHOW_DEBUG = true;
        public const int DEBUG_LEVEL = int.MaxValue;
        public bool showAsDebug = false;
        public int showDebugAsLevel = 0;
        

        // Lists
        List<TextBox> variableBoxes = new List<TextBox>();
        List<TextBox> productionBoxes = new List<TextBox>();
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        List<Production> grammar = new List<Production>();

        // Counters
        public int productionCtr = 2;
        public int step = 0;
    }

    /// <summary>
    /// Holds all the data for each line in a grammar
    /// </summary>
    class Production
    {
        public static int creationCounter = 0;

        // Flags and info
        public int creationNum = 0;
        public bool markedForDeletion = false;
        public string replacementVariable = "";
        public bool visited = false;
        public bool isTerminal = false;

        // Data
        public string variable = "";
        // the right hand side has a list for each or block, and a list of strings inside each or block where each string contains exactly one element
        public List<List<string>> productionList = new List<List<string>>();

        /// <summary>
        /// Default constructor for a production
        /// </summary>
        /// <param name="variable">The left hand side of the production, or a terminal character</param>
        public Production(string variable, bool isTerminal = false)
        {
            this.variable = variable;
            this.isTerminal = isTerminal;
            this.creationNum = creationCounter++;
        }
    }
}
