using UnityEngine;
using UnityEditor;
using System.IO;

public class LanguagePackImporter
{
    [MenuItem("Tools/Import Endings To Selected LanguagePack")]
    static void ImportEndings()
    {
        LanguagePackSO pack = Selection.activeObject as LanguagePackSO;

        if (pack == null)
        {
            Debug.LogError("Select LanguagePackSO first!");
            return;
        }

        string path = EditorUtility.OpenFilePanel("Select endings txt", "", "txt");

        if (string.IsNullOrEmpty(path))
            return;

        string[] lines = File.ReadAllLines(path);

        pack.endings.Clear();

        foreach (string line in lines)
        {
            if (!string.IsNullOrWhiteSpace(line))
                pack.endings.Add(line.Trim());
        }

        EditorUtility.SetDirty(pack);
        AssetDatabase.SaveAssets();

        Debug.Log("Imported endings: " + pack.endings.Count);
    }

    [MenuItem("Tools/Import Placements To Selected LanguagePack")]
    static void ImportPlacements()
    {
        LanguagePackSO pack = Selection.activeObject as LanguagePackSO;

        if (pack == null)
        {
            Debug.LogError("Select LanguagePackSO first!");
            return;
        }

        string path = EditorUtility.OpenFilePanel("Select placements txt", "", "txt");

        if (string.IsNullOrEmpty(path))
            return;

        string[] lines = File.ReadAllLines(path);

        pack.placements.Clear();

        foreach (string line in lines)
        {
            if (!string.IsNullOrWhiteSpace(line))
                pack.placements.Add(line.Trim());
        }

        EditorUtility.SetDirty(pack);
        AssetDatabase.SaveAssets();

        Debug.Log("Imported placements: " + pack.placements.Count);
    }
}