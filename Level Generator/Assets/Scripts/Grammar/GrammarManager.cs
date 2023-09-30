using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName ="Grammar Manager")]
public class GrammarManager : ScriptableObject
{
    [SerializeField]
    public GenerationGrammar levelGrammar;
    [SerializeField]
    public GenerationGrammar contentGrammar;
    Grammar levelGrammarCopy;
    Grammar contentGrammarCopy;


    public void SetGrammarFile(bool isLevelGrammar, TextAsset grammarFile)
    {
        if (isLevelGrammar)
        {
            this.levelGrammar.file = grammarFile;
        }
        else
        {
            this.contentGrammar.file = grammarFile;
        }
    }

    public void ExtractGrammarFromFile(bool isLevelGrammar, float probabilityAdjustment, float minimumProbability, float maximumProbability)
    {
        if (isLevelGrammar){
            string fileText = this.levelGrammar.file.text;
            string[] rules = fileText.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            List<ProductionRule> productionRules = GetProductionRules(rules);
            HashSet<string> nonTerminals = GetNonTerminalSymbols(productionRules);
            HashSet<string> terminals = GetTerminalSymbols(productionRules, nonTerminals);
            List<string> terminalsList = terminals.ToList<string>();
            List<string> nonTerminalsList = nonTerminals.ToList<string>();
            Grammar extractedGrammar = new Grammar(terminalsList, nonTerminalsList, productionRules, probabilityAdjustment, minimumProbability, maximumProbability);
            CreateRoomAssets(extractedGrammar);
            this.levelGrammar.grammar = extractedGrammar;
        }
        else
        {
            string fileText = this.contentGrammar.file.text;
            string[] rules = fileText.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            List<ProductionRule> productionRules = GetProductionRules(rules);
            HashSet<string> nonTerminals = GetNonTerminalSymbols(productionRules);
            HashSet<string> terminals = GetTerminalSymbols(productionRules, nonTerminals);
            List<string> terminalsList = terminals.ToList<string>();
            List<string> nonTerminalsList = nonTerminals.ToList<string>();
            Grammar extractedGrammar = new Grammar(terminalsList, nonTerminalsList, productionRules, probabilityAdjustment, minimumProbability, maximumProbability);
            CreateRoomAssets(extractedGrammar);
            this.contentGrammar.grammar = extractedGrammar;
        }
    }

    //public void ExtractLevelGrammarFromFile()
    //{
    //    string fileText = this.levelGrammar.file.text;
    //    string[] rules = fileText.Split(new[] { '\n' },StringSplitOptions.RemoveEmptyEntries);
    //    List<ProductionRule> productionRules=GetProductionRules(rules);
    //    HashSet<string> nonTerminals = GetNonTerminalSymbols(productionRules);
    //    HashSet<string> terminals = GetTerminalSymbols(productionRules, nonTerminals);
    //    List<string> terminalsList = terminals.ToList<string>();
    //    List<string> nonTerminalsList = nonTerminals.ToList<string>();
    //    Grammar extractedGrammar = new Grammar(terminalsList, nonTerminalsList, productionRules);
    //    CreateRoomAssets(extractedGrammar);
    //    this.levelGrammar.grammar = extractedGrammar;
    //}

    //public void ExtractContentGrammarFromFile()
    //{
    //    string fileText = this.contentGrammar.file.text;
    //    string[] rules = fileText.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
    //    List<ProductionRule> productionRules = GetProductionRules(rules);
    //    HashSet<string> nonTerminals = GetNonTerminalSymbols(productionRules);
    //    HashSet<string> terminals = GetTerminalSymbols(productionRules, nonTerminals);
    //    List<string> terminalsList = terminals.ToList<string>();
    //    List<string> nonTerminalsList = nonTerminals.ToList<string>();
    //    Grammar extractedGrammar = new Grammar(terminalsList, nonTerminalsList, productionRules);
    //    CreateRoomAssets(extractedGrammar);
    //    this.contentGrammar.grammar = extractedGrammar;
    //}

    public void ResetCopyGrammars()
    {
        this.levelGrammarCopy = this.levelGrammar.grammar;
        this.contentGrammarCopy = this.contentGrammar.grammar;
    }

    public String GenerateLevelSentence()
    {
        string levelSentence = this.levelGrammarCopy.GenerateSentence();
        return levelSentence;
    }

    public String GenerateContentSentence()
    {
        string contentSentence = this.contentGrammarCopy.GenerateSentence();
        return contentSentence;
    }

    List<ProductionRule> GetProductionRules(string[] rules)
    {
        List<ProductionRule> productionRules = new List<ProductionRule>();
        foreach(string rule in rules)
        {
            string[] splitRule = rule.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            string[] splitResult = splitRule[1].Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            ProductionRule productionRule = new ProductionRule(splitRule[0], splitResult[0], float.Parse(splitResult[1]));
            productionRules.Add(productionRule);
        }
        return productionRules;
    }

    HashSet<string> GetTerminalSymbols(List<ProductionRule> productionRules, HashSet<string> nonTerminals)
    {
        HashSet<string> terminals = new HashSet<string>();
        foreach(ProductionRule productionRule in productionRules)
        {
            string[] resultSymbols = productionRule.GetResult().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach(string resultSymbol in resultSymbols)
            {
                if (nonTerminals.Contains(resultSymbol)==false)
                {
                    terminals.Add(resultSymbol);
                }
            }
        }
        return terminals;
    }

    HashSet<string> GetNonTerminalSymbols(List<ProductionRule> productionRules)
    {
        HashSet<string> nonTerminals = new HashSet<string>();
        foreach(ProductionRule productionRule in productionRules)
        {
            nonTerminals.Add(productionRule.GetNonTerminal());
        }
        return nonTerminals;
        
    }


    void CreateRoomAssets(Grammar grammar)
    {
       /* Generator generator = this.gameObject.GetComponent<Generator>();
        generator.ResetRoomAsset();
        foreach(string symbol in grammar.terminalSymbols)
        {
            generator.AddNewRoomAsset(symbol);
        }
        */
    }

    private void UpdateAdjustmentValues(float minimumProbability, float maximumProbability, float probabilityAdjustment, bool isLevelGrammar)
    {
        if (isLevelGrammar)
        {
            levelGrammar.grammar.UpdateAdjustments(minimumProbability, maximumProbability, probabilityAdjustment);
        }
        else
        {
            contentGrammar.grammar.UpdateAdjustments(minimumProbability, maximumProbability, probabilityAdjustment);
        }
 
    }

    [Serializable]
    public struct GenerationGrammar
    {
        public TextAsset file;
        public Grammar grammar;
    }
}
