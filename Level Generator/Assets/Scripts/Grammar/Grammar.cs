using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Grammar
{
    public List<string> terminalSymbols;
    public List<string> nonTerminalSymbols;
    public List<ProductionRule> productionRules;
    public float probabilityAdjustment;
    public float minimumProbability;
    public float maximumProbability;


    public Grammar(List<string> terminalSymbols, List<string> nonTerminalSymbols, List<ProductionRule> productionRules)
    {
        this.terminalSymbols = terminalSymbols;
        this.nonTerminalSymbols = nonTerminalSymbols;
        this.productionRules = productionRules;
        this.probabilityAdjustment = 10f;
        this.minimumProbability = 15f;
        this.maximumProbability = 60f;
    }

    public void SetProbabilityAdjustment(float adjustment)
    {
        this.probabilityAdjustment = adjustment;
    }

    public void SetMinimumProbability(float minimum)
    {
        this.minimumProbability = minimum;
    }

    public void SetMaximumProbability(float maximum)
    {
        this.maximumProbability = maximum;
    }

    public void UpdateAdjustments(float minimumProbability, float maximumProbability, float probabilityAdjustment)
    {
        this.minimumProbability = minimumProbability;
        this.maximumProbability = maximumProbability;
        this.probabilityAdjustment = probabilityAdjustment;
    }

    public string GenerateSentence()
    {
        int i;
        string startingSentence = this.productionRules[0].GetResult();
        string[] splitString = startingSentence.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        while (HasNonTerminal(splitString))
        {
            for (i = 0; i < splitString.Length; i++)
            {
                if (IsNonTerminal(splitString[i]))
                {
                    List<ProductionRule> matchingRules = GetMatchingRulesFromNonTerminal(splitString[i]);
                    string chosenResult = ApplyRandomRule(matchingRules);
                    if(chosenResult!= ".")
                    {
                        splitString[i] = chosenResult;
                    }
                    else
                    {
                        splitString[i] = " ";
                    }
                }
                
            }
            string tempString = string.Join(" ", splitString);
            //Debug.Log(tempString);
            splitString = tempString.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);


        }
        string result= string.Join(" ", splitString);
        return result;
    }

    public string GenerateSentenceFromString(string nonTerminal)
    {
        int i;
        string startingSentence = nonTerminal;
        string[] splitString = startingSentence.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        while (HasNonTerminal(splitString))
        {
            for (i = 0; i < splitString.Length; i++)
            {
                if (IsNonTerminal(splitString[i]))
                {
                    List<ProductionRule> matchingRules = GetMatchingRulesFromNonTerminal(splitString[i]);
                    string chosenResult = ApplyRandomRule(matchingRules);
                    if (chosenResult != ".")
                    {
                        splitString[i] = chosenResult;
                    }
                    else
                    {
                        splitString[i] = " ";
                    }
                }

            }
            string tempString = string.Join(" ", splitString);
            //Debug.Log(tempString);
            splitString = tempString.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);


        }
        string result = string.Join(" ", splitString);
        return result;
    }

    public float GenerateRandomNumber(float totalProbability)
    {
        float number = UnityEngine.Random.Range(0f, totalProbability);
        return number;
    }

    public List<ProductionRule> GetMatchingRulesFromNonTerminal(string nonTerminal)
    {
        List<ProductionRule> matchingRules = new List<ProductionRule>();
        foreach(ProductionRule rule in this.productionRules)
        {
            if (rule.GetNonTerminal() == nonTerminal)
            {
                matchingRules.Add(rule);
            }
        }
        return matchingRules;
    }

    public bool HasNonTerminal(string[] sentence)
    {
        foreach(string symbol in sentence)
        {
            if (IsNonTerminal(symbol))
            {
                return true;
            }
        }
        return false;
    }

    public bool IsNonTerminal(string symbol)
    {

        if (this.nonTerminalSymbols.Contains(symbol))
        {
            
            return true;
        }
        return false;
    }

    public float MatchingRulesTotalOdds(List<ProductionRule> matchingRules)
    {
        float totalOdds = 0f;
        foreach(ProductionRule rule in matchingRules)
        {
            totalOdds += rule.GetProbability();
        }
        return totalOdds;
    }

    public string ApplyRandomRule(List<ProductionRule> productionRules)
    {
        float totalOdds = MatchingRulesTotalOdds(productionRules);
        float numberRoll = GenerateRandomNumber(totalOdds);
        float accumulatedOdds = 0f;
        string result = null;
        foreach(ProductionRule rule in productionRules)
        {
            if(numberRoll>accumulatedOdds && numberRoll <= (accumulatedOdds + rule.GetProbability()))
            {
                result = rule.GetResult();
                AdjustUsedRuleProbability(rule);
                //Debug.Log("Adjustment:" + rule.GetProbability() + ":" + rule.GetResult());
                AdjustTerminalProbability(productionRules);
                return result;
            }
            accumulatedOdds += rule.GetProbability();
        }
        return result;
    }

    List<ProductionRule> GetTerminalRules(List<ProductionRule> matchingRules)
    {
        List<ProductionRule> terminalRules = new List<ProductionRule>();
        foreach(ProductionRule rule in matchingRules)
        {
            if (HasNonTerminal(rule.GetResult().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)) == false)
            {
                terminalRules.Add(rule);
            }
        }
        return terminalRules;
    }

    ProductionRule GetLeastProbableRule(List<ProductionRule> productionRules)
    {
        float minimum = float.MaxValue;
        ProductionRule leastProbableRule = null;
        foreach(ProductionRule rule in productionRules)
        {
            if (rule.GetProbability() < minimum)
            {
                minimum = rule.GetProbability();
                leastProbableRule = rule;
            }
        }
        return leastProbableRule;
    }

    void AdjustUsedRuleProbability(ProductionRule rule)
    {
        if (rule.GetProbability() - this.probabilityAdjustment < this.minimumProbability)
        {
            rule.SetProbability(this.minimumProbability);
        }
        else
        {
            rule.SetProbability(rule.GetProbability() - this.probabilityAdjustment);
        }
    }

    void AdjustTerminalProbability(List<ProductionRule> matchingRules)
    {
        List<ProductionRule> terminalRules = GetTerminalRules(matchingRules);
        ProductionRule ruleToAdjust = GetLeastProbableRule(terminalRules);
        if (ruleToAdjust == null)
        {
            return;
        }
        //Debug.Log("AYYYY " + ruleToAdjust);
        if (ruleToAdjust.GetProbability() + this.probabilityAdjustment > this.maximumProbability)
        {
            ruleToAdjust.SetProbability(this.maximumProbability);
        }
        else
        {
            ruleToAdjust.SetProbability(ruleToAdjust.GetProbability() + this.probabilityAdjustment);
        }
    }


}
