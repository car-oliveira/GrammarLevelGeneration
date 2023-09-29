using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ProductionRule
{
    [SerializeField]
    string nonTerminal;
    [SerializeField]
    string result;
    [SerializeField]
    float probability;

    public ProductionRule(string nonTerminal, string result, float probability)
    {
        this.nonTerminal = nonTerminal;
        this.result = result;
        this.probability = probability;
    }

    public ProductionRule(ProductionRule rule)
    {
        this.nonTerminal = rule.nonTerminal;
        this.result = rule.result;
        this.probability = rule.probability;
    }

    public string GetNonTerminal()
    {
        return this.nonTerminal;
    }

    public string GetResult()
    {
        return this.result;
    }

    public float GetProbability()
    {
        return this.probability;
    }

    public void SetProbability(float probability)
    {
        this.probability = probability;
    }
}
