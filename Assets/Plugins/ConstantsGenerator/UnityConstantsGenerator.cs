using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace UnityConstantsGenerator
{
    public class ConstantsGenerator : EditorWindow
    {
        private string _namespaceName = @"UnityConstants";
        private string _folderPath = @"Unity Constants";
        private string _axesFileName = @"Axes.cs";
        private string _tagsFileName = @"Tags.cs";
        private string _sortingLayersFileName = @"SortingLayers.cs";
        private string _layersFileName = @"Layers.cs";

        [MenuItem("Window/Unity Constants Generator")]
        private static void CallCreateWindow()
        {
            // Get existing open window or if none, make a new one:
            var window = (ConstantsGenerator) GetWindow(typeof(ConstantsGenerator));
            window.titleContent.text = "Unity Constants Generator";
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.Separator();

            EditorGUILayout.BeginVertical(EditorStyles.inspectorFullWidthMargins);

            EditorGUILayout.LabelField("Generates as Assets/<Folder_Path>/<File_Name>", EditorStyles.centeredGreyMiniLabel);

            EditorGUILayout.Separator();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(@"Namespace");
            _namespaceName = EditorGUILayout.TextField(_namespaceName, EditorStyles.textField);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(@"Folder Path");
            _folderPath = EditorGUILayout.TextField(_folderPath, EditorStyles.textField);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("File Names", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(@"Axes");
            _axesFileName = EditorGUILayout.TextField(_axesFileName, EditorStyles.textField);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(@"Tags");
            _tagsFileName = EditorGUILayout.TextField(_tagsFileName, EditorStyles.textField);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(@"Sorting Layers");
            _sortingLayersFileName = EditorGUILayout.TextField(_sortingLayersFileName, EditorStyles.textField);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(@"Layers");
            _layersFileName = EditorGUILayout.TextField(_layersFileName, EditorStyles.textField);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Separator();

            EditorGUILayout.BeginHorizontal();
            //GUILayout.FlexibleSpace();
            if (GUILayout.Button("Generate All"))
            {
                try
                {
                    GenerateAndImport(_namespaceName, Path.Combine("Assets", _folderPath, _axesFileName), true, GetAllAxisNames);
                    GenerateAndImport(_namespaceName, Path.Combine("Assets", _folderPath, _tagsFileName), true, GetAllTags);
                    GenerateAndImport(_namespaceName, Path.Combine("Assets", _folderPath, _sortingLayersFileName), true, GetAllSortingLayers);
                    GenerateAndImport(_namespaceName, Path.Combine("Assets", _folderPath, _layersFileName), true, GetAllLayers);
                    GC.Collect();
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
            //GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        #region code generation

        public static void GenerateAndImport(string namespaceName, string fullPath, bool staticClass, Func<IEnumerable<string>> namesProvider)
        {
            var names = namesProvider();
            if (names.Any())
            {
                GenerateNamesCodeFile(namespaceName, fullPath, staticClass, names);
                AssetDatabase.ImportAsset(fullPath, ImportAssetOptions.ForceUpdate);
            }
            else
                Debug.Log($"No names found, skipping generation of {fullPath}");
        }

        private static void GenerateNamesCodeFile(string namespaceName, string fullPath, bool staticClass, IEnumerable<string> names)
        {
            var name = Path.GetFileNameWithoutExtension(fullPath);
            var constants = names.ToDictionary(ConvertToValidIdentifier, s => s);

            var code = CreateStringConstantsClass(namespaceName, name, staticClass, constants);

            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            using (var stream = new StreamWriter(fullPath, append: false))
            {
                var tw = new IndentedTextWriter(stream);
                var codeProvider = new CSharpCodeProvider();
                codeProvider.GenerateCodeFromCompileUnit(code, tw, new CodeGeneratorOptions());
            }
        }

        private static CodeCompileUnit CreateStringConstantsClass(
            string namespaceName,
            string name,
            bool staticClass,
            IDictionary<string, string> constants)
        {
            var compileUnit = new CodeCompileUnit();
            var @namespace =  new CodeNamespace(namespaceName);

            var @class = new CodeTypeDeclaration(name);

            if (staticClass)
                ImitateStaticClass(@class);
            else
                PublicClass(@class);

            foreach (var pair in constants)
            {
                var @const = new CodeMemberField(
                    typeof(string),
                    pair.Key);
                @const.Attributes &= ~MemberAttributes.AccessMask;
                @const.Attributes &= ~MemberAttributes.ScopeMask;
                @const.Attributes |= MemberAttributes.Public;
                if (staticClass)
                    @const.Attributes |= MemberAttributes.Const;
                else
                    @const.Type = new CodeTypeReference($"readonly string");

                @const.InitExpression = new CodePrimitiveExpression(pair.Value);

                @class.Members.Add(@const);
            }

            @namespace.Types.Add(@class);
            compileUnit.Namespaces.Add(@namespace);

            return compileUnit;
        }

        /// <summary>
        /// Marks class as sealed and adds private constructor to it.
        /// </summary>
        /// <remarks>
        /// It's not possible to create static class using CodeDom.
        /// Creating abstract sealed class instead leads to compilation error.
        /// This method can be used instead to make pseudo-static class.
        /// </remarks>
        private static void ImitateStaticClass(CodeTypeDeclaration type)
        {
            type.TypeAttributes |= TypeAttributes.Sealed;

            type.Members.Add(new CodeConstructor
            {
                Attributes = MemberAttributes.Private | MemberAttributes.Final
            });
        }

        private static void PublicClass(CodeTypeDeclaration type)
        {
            type.TypeAttributes |= TypeAttributes.Public;

            type.Members.Add(new CodeConstructor
            {
                Attributes = MemberAttributes.Public
            });
        }

        private static string ConvertToValidIdentifier(string name)
        {
            var sb = new StringBuilder(name.Length + 1);

            if (!char.IsLetter(name[0]))
                sb.Append('_');

            foreach (var ch in name)
            {
                if (char.IsLetterOrDigit(ch))
                {
                    sb.Append(char.ToUpperInvariant(ch));
                }
                else if (char.IsWhiteSpace(ch))
                {

                }
                else
                {
                    sb.Append('_');
                }
            }

            return sb.ToString();
        }

        #endregion

        #region names providers

        private static IEnumerable<string> GetAllAxisNames()
        {
            var result = new StringCollection();

            var serializedObject =
                new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
            var axesProperty = serializedObject.FindProperty("m_Axes");

            axesProperty.Next(true);
            axesProperty.Next(true);

            while (axesProperty.Next(false))
            {
                var axis = axesProperty.Copy();
                axis.Next(true);
                result.Add(axis.stringValue);
            }

            return result.Cast<string>().Distinct();
        }

        private static IEnumerable<string> GetAllTags()
        {
            return new ReadOnlyCollection<string>(InternalEditorUtility.tags);
        }

        private static IEnumerable<string> GetAllSortingLayers()
        {
            var internalEditorUtilityType = typeof(InternalEditorUtility);
            var sortingLayersProperty =
                internalEditorUtilityType.GetProperty("sortingLayerNames",
                    BindingFlags.Static | BindingFlags.NonPublic);
            var sortingLayers = (string[]) sortingLayersProperty.GetValue(null, new object[0]);

            return new ReadOnlyCollection<string>(sortingLayers);
        }

        private static IEnumerable<string> GetAllLayers()
        {
            return new ReadOnlyCollection<string>(InternalEditorUtility.layers);
        }

        #endregion
    }
}
