using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using UnityEditor;

#if UNITY_EDITOR
public static class EnumGenerator
{
    public static void Generate(string enumName, string path, IReadOnlyList<string> entries)
    {
        if (string.IsNullOrWhiteSpace(enumName))
        {
            Debug.LogError("Enum name cannot be null, empty or contain white spaces");
            return;
        }

        if (entries == null)
        {
            Debug.LogError("Entries are null");
            return;
        }

        string filePathAndName = $"{path}/{enumName}.cs";

        StringBuilder contents = new StringBuilder();
        contents.AppendLine("//Auto-generated code");
        contents.AppendLine();
        contents.AppendLine("using System;");
        contents.AppendLine();
        contents.AppendLine("[Flags]");
        contents.AppendLine($"public enum {string.Concat(enumName.FirstCharToUpper().Where(x => !char.IsWhiteSpace(x)))} : uint");
        contents.AppendLine("{");
        if (entries.Count > 0)  // For the first entry.
            contents.AppendLine($"\t{string.Concat(entries[0].FirstCharToUpper().Where(x => !char.IsWhiteSpace(x)))} = 0,");
        for (int i = 1; i < entries.Count; i++)
            contents.AppendLine($"\t{string.Concat(entries[i].FirstCharToUpper().Where(x => !char.IsWhiteSpace(x)))} = 1 << {i - 1},");
        contents.AppendLine("}");

        File.WriteAllText(filePathAndName, contents.ToString());
        
        AssetDatabase.Refresh();
    }
}
#endif
