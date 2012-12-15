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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Starting point for execution "Main"
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            SetUp();
        }

        /// <summary>
        /// When the step button is pressed progresses to the next state of the grammar and displays current state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (step > 0)
            {
                output.Text = "State after step: " + step;
                switch (step)
                {
                    case 1: // Make sure that terminal variables are decalred in a production as one element of an or block
                        InitialTerminalRemovalStep1();
                        MarkExactDuplicatesForDeletion();
                        // Do not prepare for deletions as it will remove both the duplicate and non duplicate productions from the right hand side
                        DeleteProductions();
                        break;
                        
                    case 2:
                        MarkDuplicateTerminalDefinitionsForDeletion();
                        PrepareForDeletion();
                        DeleteProductions();
                        break;

                    case 3: // Replace instances of terminal variables that are not defined alone in an or block with the corresponding variable that was created in step 0
                                // replace    S-->Aa   where   A-->a   to   S-->AA
                        ReplaceTerminalsWithVariables();
                        break;

                    case 4: // Remove lamda transitions. if we can just cut them out do this first, also would then have to test for empty productions after removal
                        //RemoveLambda();
                        MarkEmptyForDeletion();
                        PrepareForDeletion();
                        DeleteProductions();
                        break;

                    case 5: // Remove useless transitions recursivley but still allow stepping by decrementing step in the function if one is found and removed
                        // MarkUselessTransitionsForDeletion();
                        MarkVariableToSingleVariableForReplacement();
                        PrepareForDeletion();
                        DeleteProductions();
                        break;
                    default:
                        Output("Done processing");
                        step--;
                        break;
                }
                PrintGrammar();
                step++;
            }
        }

        /// <summary>
        /// Sets up the forum
        /// </summary>
        private void SetUp()
        {
            //lists of text boxes
            variableBoxes.Add(Variable1); variableBoxes.Add(Variable2); variableBoxes.Add(Variable3); variableBoxes.Add(Variable4); variableBoxes.Add(Variable5);
            variableBoxes.Add(Variable6); variableBoxes.Add(Variable7); variableBoxes.Add(Variable8); variableBoxes.Add(Variable9); variableBoxes.Add(Variable10);
            variableBoxes.Add(Variable11); variableBoxes.Add(Variable12);

            productionBoxes.Add(Production1); productionBoxes.Add(Production2); productionBoxes.Add(Production3); productionBoxes.Add(Production4);
            productionBoxes.Add(Production5); productionBoxes.Add(Production6); productionBoxes.Add(Production7); productionBoxes.Add(Production8);
            productionBoxes.Add(Production9); productionBoxes.Add(Production10); productionBoxes.Add(Production11); productionBoxes.Add(Production12);

            DisplayInst();
        }


        /// <summary>
        /// Reads in grammar from the forum
        /// </summary>
        private void RecieveValues()
        {
            bool success = true;
            int count = 0;
            foreach (TextBox box in variableBoxes)
            {
                if (!string.IsNullOrEmpty(box.Text))
                {
                    if (box.Text.Length > 1 || !char.IsLetter(box.Text[0]) || !char.IsUpper(box.Text[0]))
                    {
                        MessageBox.Show("Only one capital letter in the variable columns");
                    }

                    var tempToEnter = productionBoxes[count].Text.Trim();
                    tempToEnter = tempToEnter.Replace(" ", "");

                    dictionary.Add(box.Text.Trim(), tempToEnter);
                    count++;
                }
            }

            foreach (var item in dictionary)
            {
                if (string.IsNullOrEmpty(item.Key) || string.IsNullOrEmpty(item.Value))
                {
                    MessageBox.Show("Variables to Production counts do not match!");
                    ClearAll();
                    success = false;
                    break;
                }
            }

            if (success)
            {
                output.Text = "Successful input.";
            }
        }

        /// <summary>
        /// Starts the process of seeing if the grammar is empty
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Go_Click(object sender, RoutedEventArgs e)
        {
            ClearAll();
            if (string.IsNullOrEmpty(Variable1.Text) || string.IsNullOrEmpty(Production1.Text))
            {
                MessageBox.Show("You must have at least one full production");
                ClearAll();
            }
            else
            {
                RecieveValues();
            }

            InitializeGramar();
        }

        /// <summary>
        /// Adds additional entry points to the screen for inputing additional productions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddBoxes_Click(object sender, RoutedEventArgs e)
        {
            if (productionCtr < 11)
            {
                var variable = variableBoxes.ElementAt(productionCtr);
                variable.Visibility = Visibility.Visible;
                var production = productionBoxes.ElementAt(productionCtr);
                production.Visibility = Visibility.Visible;

                productionCtr++;
            }

            else
            {
                MessageBox.Show("Maximum allowed number of productions reached");
            }
        }

        /// <summary>
        /// Clears the grammar and dictionary from memory
        /// </summary>
        private void ClearAll()
        {
            dictionary.Clear();
            grammar.Clear();
        }

        /// <summary>
        /// Displays the initial instruictions on the screen
        /// </summary>
        private void DisplayInst()
        {
            Output("This application will check to see if a given context free grammar will result in an empty Language.\n" +
                           "The starting Variable will always be S, you must have at least one production to continue.\n" +
                           "Use the '~' symbol when you are inputting a lambda value.");
        }

        /// <summary>
        /// Loads test data on button press depending on TEST_DATA_SET value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Load_Test_Data_Click(object sender, RoutedEventArgs e)
        {
            switch (TEST_DATA_SET)
            {
                case 0:
                    Production1.Text = "A";

                    Variable2.Text = "A";
                    Production2.Text = "Ba";

                    Variable3.Visibility = Visibility.Visible;
                    Production3.Visibility = Visibility.Visible;
                    productionCtr++;

                    Variable3.Text = "B";
                    Production3.Text = "c|A";
                    break;
            }
        }
    }
}