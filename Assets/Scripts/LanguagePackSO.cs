using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LanguagePack", menuName = "Game/Language Pack")]
public class LanguagePackSO : ScriptableObject
{
    public string languageCode; // np "EN", "PL"
    public List<string> phrases;
}