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
        /// Loads the grammar with the input defined by the user
        /// </summary>
        private void InitializeGramar()
        {
            // Load input from user into grammar list
            for (int i = 0; i < dictionary.Count; i++)
            {
                var production = dictionary.ElementAt(i);

                // Do not allow duplicate left hand side variable to be entered
                if (!ProductionExists(production.Key))
                {
                    Production temp = new Production(production.Key);
                    InitializeProductions(temp, production.Value);
                    grammar.Add(temp);
                }
                else
                {
                    Output("ERROR: Duplicate variables found and ignored");
                    dictionary.Remove(production.Key);
                    i--;
                }
            }

            // Start the step process
            step = 1;
            PrintGrammar();
        }

        /// <summary>
        /// Change the right hand side of a production to the new list. *Treats all production variables as one char in length*
        /// </summary>
        /// <param name="production">Production to have right side modified</param>
        /// <param name="productionList">String of new right hand side, each entry must be one char in length</param>
        private void InitializeProductions(Production production, string productionList)
        {
            // Seperate right hand side into or blocks
            string[] parts = productionList.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string str in parts)
            {
                // Enter each char in the or block as a seperate string list element
                List<string> orBlock = new List<string>();
                foreach (char ch in str)
                {
                    orBlock.Add(ch + "");
                }
                production.productionList.Add(orBlock);
            }
        }

        /// <summary>
        /// Change the right hand side of a production to the new list
        /// </summary>
        /// <param name="production">Production to have right side modified</param>
        /// <param name="productionList">String of new right hand side</param>
        private void ChangeProductions(Production production, List<List<string>> productionList)
        {
            // Erases all data from the right hand side of the passed in production
            production.productionList.Clear();

            // Seperate right hand data into or blocks
            foreach (var outerList in productionList)
            {
                // Enter each string in the or block as a seperate string list element
                List<string> orBlock = new List<string>();
                foreach (string str in outerList)
                {
                    orBlock.Add(str);
                }
                production.productionList.Add(orBlock);
            }
        }

        /// <summary>
        /// Finds productions that can have the terminal values replaced by a variable and replaces them accordingly
        /// </summary>
        private void ReplaceTerminalsWithVariables()
        {
            // Step through each production in the grammar
            for (int i = 0; i < grammar.Count; i++)
            {
                // Step through each or block in the production
                Production production = grammar.ElementAt(i);
                for (int j = 0; j < production.productionList.Count; j++)
                {
                    // Ensure that there is more than one element in the or block
                    var orBlock = production.productionList.ElementAt(j);
                    if (orBlock.Count > 1)
                    {
                        // Step through each element in the or block
                        for (int k = 0; k < orBlock.Count; k++)
                        {
                            // Test if length is one *terminal variables are all one char in length*
                            var orElement = orBlock.ElementAt(k);
                            if (orElement.Length == 1)
                            {
                                // Test for terminal *terminals are allways a lowerchase character*
                                if (!Char.IsUpper(orElement[0]))
                                {
                                    orBlock.RemoveAt(k);
                                    orBlock.Insert(k, GetTerminalProductionVariable(orElement));
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// If the right hand side of a production is empty then mark for deletion
        /// </summary>
        private void MarkEmptyForDeletion()
        {
            // Step through each production in the grammar
            foreach (var production in grammar)
            {
                // Test if the left hand side is empty
                if (production.productionList.Count == 0)
                {
                    production.markedForDeletion = true;
                }
            }
        }

        /// <summary>
        /// Finds terminal productions that have identical left and right sides and flags them for deletion
        /// </summary>
        private void MarkDuplicateTerminalDefinitionsForDeletion()
        {
            // Stepper 1 to step through each production in the grammar and compare it to every other production
            foreach (var production1 in grammar)
            {
                // Test for possible explicitly terminal production *explicitly terminal productions have one or block*
                if (production1.productionList.Count == 1)
                {
                    // Test that or block contains only one element *explicitly terminal productions have one element in the or block"
                    var orBlock1 = production1.productionList.ElementAt(0);
                    if (orBlock1.Count == 1)
                    {
                        // Test for terminal variable
                        string element1 = orBlock1.ElementAt(0);
                        if (element1.Length == 1 && !Char.IsUpper(element1[0]))
                        {
                            // Stepper 2 to compare production to stepper 1
                            foreach (var production2 in grammar)
                            {
                                // Ensure the two productions are not actually the same production in memory
                                if (production1.creationNum != production2.creationNum)
                                {
                                    // Ensure that neither production is marked for deletion
                                    if (!production1.markedForDeletion && !production2.markedForDeletion)
                                    {
                                        // Test for possible terminal or block
                                        if (production2.productionList.Count == 1)
                                        {
                                            // Test for possible terminal variable
                                            var orBlock2 = production2.productionList.ElementAt(0);
                                            if (orBlock2.Count == 1)
                                            {
                                                // Test if char is a terminal variable
                                                string element2 = orBlock2.ElementAt(0);
                                                if (element2.Length == 1 && !Char.IsUpper(element2[0]))
                                                {
                                                    // Compare terminal variables
                                                    if (element1.Equals(element2))
                                                    {
                                                        production2.markedForDeletion = true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Finds productions that have identical left and right sides and flags them for deletion
        /// </summary>
        private void MarkExactDuplicatesForDeletion()
        {
            // Stepper 1 to compare all other productions too
            foreach (var production1 in grammar)
            {
                // Stepper 2 to compare against stepper 1
                foreach (var production2 in grammar)
                {
                    // Make sure that we dont compare a production to itself
                    if (production1.creationNum != production2.creationNum)
                    {
                        // Make sure that neither production has been marked for deletion
                        if (!production1.markedForDeletion && !production2.markedForDeletion)
                        {
                            // Make sure that the left hand side of the productions are identical
                            if (production1.variable.Equals(production2.variable))
                            {
                                // Make sure the right hand sides of the production contain the same number of elements
                                if (production1.productionList.Count == production2.productionList.Count)
                                {
                                    // Compare each element on the right side
                                    for (int i = 0; i < production1.productionList.Count; i++)
                                    {
                                        List<string> tempList1 = production1.productionList.ElementAt(i);
                                        List<string> tempList2 = production2.productionList.ElementAt(i);
                                        // Make sure that there are the same number of emements in each or block
                                        if (tempList1.Count == tempList2.Count)
                                        {
                                            // Compare each or block
                                            for (int j = 0; j < tempList1.Count; j++)
                                            {
                                                // Test to make see if the elements in the or blocks are the same
                                                if (!(tempList1.ElementAt(j).Equals(tempList2.ElementAt(j))))
                                                {
                                                    production2.markedForDeletion = true;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Searches through the grammar for all productions marked for deletion, then replaces each instance of it with the specified replacement variable.
        /// If the replacement variable has been left blank it deletes instance withour replacement. 
        /// </summary>
        private void PrepareForDeletion()
        {
            // Step through each production in the grammar
            for (int n = 0; n < grammar.Count; n++)
            {
                // Test if grammar has been marked for deletion
                if (grammar.ElementAt(n).markedForDeletion)
                {
                    // Step through each production in the grammar
                    string variableToReplace = grammar.ElementAt(n).variable;
                    string replacementVariable = grammar.ElementAt(n).replacementVariable;
                    for (int i = 0; i < grammar.Count; i++)
                    {
                        // Step through each or block of the production
                        Production production = grammar.ElementAt(i);
                        for (int j = 0; j < production.productionList.Count; j++)
                        {
                            // Step through each element of the or block
                            var orBlock = production.productionList.ElementAt(j);
                            for (int k = 0; k < orBlock.Count; k++)
                            {
                                // Test if variable to remove is at location
                                if (orBlock.ElementAt(k).Equals(variableToReplace))
                                {
                                    // If no replacement variable has been specified delete the element
                                    if (replacementVariable == "")
                                    {
                                        orBlock.RemoveAt(k);
                                        k--;
                                    }
                                    else
                                    {
                                        orBlock.RemoveAt(k);
                                        orBlock.Insert(k, replacementVariable);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Moves through the grammar and removes productions that have been marked for deletion
        /// </summary>
        private void DeleteProductions()
        {
            // Step through each production in the grammar
            for (int i = 0; i < grammar.Count; i++)
            {
                // Test if production has been marked for deletion
                var production = grammar.ElementAt(i);
                if (production.markedForDeletion)
                {
                    grammar.RemoveAt(i);
                    i--;
                }
            }
        }

        

        /// <summary>
        /// Creates new productions for every instance of a terminal character found. If the terminal char was found to be not replacable by a variable,
        /// that is the variable in that OR block only points to the terminal then the production created will have the form "a-->S" otherwise if it was
        /// movable a production was created of the form "A0-->a"
        /// </summary>
        private void InitialTerminalRemovalStep1()
        {
            // Step through each production in the grammar
            foreach (var production in dictionary)
            {
                string strShort = production.Value.Trim();
                if (strShort.Length == 0)
                {
                    Output("ERROR: Blank value in right");
                }
                else
                {
                    // Test for explicit terminal production
                    if (strShort.Length == 1 && !Char.IsUpper(strShort[0]))
                    {
                        //Production temp = FindProduction(production.Key);
                        //temp.isTerminal = true;
                        Production temp2 = new Production(strShort[0] + "", true);
                        List<string> tempList = new List<string>();
                        tempList.Add(production.Key);
                        temp2.productionList.Add(tempList);
                        grammar.Add(temp2);

                    }
                    else
                    {
                        // Step through each or block
                        string[] parts = strShort.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string onePiece in parts)
                        {
                            // Test for a single terminal variable in the or block
                            if (onePiece.Length == 1 && !Char.IsUpper(onePiece[0]))
                            {
                                // Production not movable insert a new production in the style a-->a
                                Production temp = new Production(onePiece[0] + "", true);
                                List<string> tempList = new List<string>();
                                tempList.Add(production.Key);
                                temp.productionList.Add(tempList);
                                grammar.Add(temp);
                            }
                            else
                            {
                                // Step through each element in the or block
                                foreach (char ch in onePiece)
                                {
                                    // Test for terminal
                                    if (!Char.IsUpper(ch))
                                    {
                                        // Make sure production of form a-->a  or a-->A  does not exist in the grammar
                                        if (!ProductionExists(ch + ""))
                                        {
                                            // Adds production in the style A0-->a  defineing a new explicitly terminal variable to the grammar
                                            Production temp = new Production(Char.ToUpper(ch) + "0", true);
                                            List<string> tempList = new List<string>();
                                            tempList.Add(ch + "");
                                            temp.productionList.Add(tempList);
                                            grammar.Add(temp);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Determins weather or not a Production exists in the grammar
        /// </summary>
        /// <param name="variable">Production to search for</param>
        /// <returns>True if the Production exits in the grammar, false otherwise</returns>
        private bool ProductionExists(string variable)
        {
            // Test if function returns a reference to a production
            Production production = FindProduction(variable);
            if (production != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the Production that has the given variable name in the grammar
        /// </summary>
        /// <param name="variable">The variable that we are looking for</param>
        /// <returns>A reference to the requested Production, NULL if not found</returns>
        private Production FindProduction(string variable)
        {
            // Step through each production in the grammar
            foreach (Production production in grammar)
            {
                // Test if production has been found
                if (production.variable == variable)
                {
                    return production;
                }
            }
            return null;
        }

        /// <summary>
        /// Finds the left hand side of the production that defines a terminal
        /// </summary>
        /// <param name="terminal">The variable we are looking for to be explicitly defined</param>
        /// <returns>The left hand side of the production that explcitly defines a terminal variable</returns>
        private string GetTerminalProductionVariable(string terminal)
        {
            // *a-->B   in the grammar states that a is defined at B*
            // Step through the productions in the grammar
            foreach (Production production in grammar)
            {
                // Test if production is explicitly terminal
                if (production.isTerminal && production.productionList.Count == 1)
                {
                    // Get the first or block
                    var temp1 = production.productionList.ElementAt(0);
                    if (temp1.Count == 1)
                    {
                        // Get the first element of the or block
                        string temp2 = temp1.ElementAt(0);
                        if (temp2.Equals(terminal))
                        {
                            return production.variable;
                        }
                    }
                }
            }
            Output("ERROR: Did not find the terminal variable production for " + terminal);
            return "NANA NANA BOOBOO";
        }
    }
}
