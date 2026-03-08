using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LanguagePack", menuName = "Game/Language Pack")]

public class PhraseElement
{
    public string word;
    public string placement;

    public PhraseElement(string word, string placement)
    {
        this.word = word;
        this.placement = placement;
    }
}
public class LanguagePackSO : ScriptableObject
{
    public string languageCode;

    [Header("Word Parts")]
    public List<string> endings;

    [Header("Placement Rules")]
    public List<string> placements;
}