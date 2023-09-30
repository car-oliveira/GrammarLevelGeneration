using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Generator))]
public class GeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Generator generatorScript = (Generator)target;
        if (GUILayout.Button("Extract Level Grammar"))
        {
            generatorScript.grammarManager.SetGrammarFile(true, generatorScript.levelGrammarFile);
            generatorScript.grammarManager.ExtractGrammarFromFile(true, generatorScript.probabilityAdjustment, generatorScript.minimumProbability, generatorScript.maximumProbability);
            generatorScript.CreateRoomAssets(generatorScript.grammarManager.levelGrammar.grammar);
        }
        if (GUILayout.Button("Extract Content Grammar"))
        {
            generatorScript.grammarManager.SetGrammarFile(false, generatorScript.contentGrammarFile);
            generatorScript.grammarManager.ExtractGrammarFromFile(false,generatorScript.probabilityAdjustment, generatorScript.minimumProbability, generatorScript.maximumProbability);
            generatorScript.CreateContentAssets(generatorScript.grammarManager.contentGrammar.grammar);
        }
    }
}
