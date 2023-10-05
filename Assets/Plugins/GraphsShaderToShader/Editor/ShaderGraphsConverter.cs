using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.Graphing;
using UnityEngine;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Serialization;

namespace NKStudio
{
    public static class ShaderGraphConverter
    {
        private const string TargetExtension = ".shadergraph";

        /// <summary>
        /// UnityEditor.ShaderGraph.ShaderGraphImporterEditor���κ��� �ڵ带 �����ͼ� ������.
        /// </summary>
        [MenuItem("Tools/Shader Graph to Code Generate")]
        private static void CodeGenerate()
        {
            string targetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            string fileExtension = Path.GetExtension(targetPath);

            if (!fileExtension.Equals(TargetExtension))
            {
                Debug.LogWarning("It is not a shader file.");
                return;
            }

            string selectAssetName = Path.GetFileNameWithoutExtension(targetPath);
            string directoryPath = Path.GetDirectoryName(targetPath);

            GraphData graphData = GetGraphData(targetPath);
            Generator generator = new Generator(graphData, null, GenerationMode.ForReals, selectAssetName,null,humanReadable: true);

            // AssetDatabase.CreateAsset���δ� Shader ������ ������ �� ����
            // generator.generatedShader : String, File�� �����ϱ� �� �߰����� �峭�� ����
            File.WriteAllText($"{directoryPath}/{selectAssetName}.shader", generator.generatedShader);
            AssetDatabase.ImportAsset($"{directoryPath}/{selectAssetName}.shader");
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<Shader>($"{directoryPath}/{selectAssetName}.shader");
        }

        /// <summary>
        /// UnityEditor.ShaderGraph.ShaderGraphImporterEditor.OnInspectorGUI.GetGraphData�� �����ͼ� ������.
        /// </summary>
        private static GraphData GetGraphData(string assetPath)
        {
            string textGraph = File.ReadAllText(assetPath, Encoding.UTF8);
            GraphObject graphObject = ScriptableObject.CreateInstance<GraphObject>();
            graphObject.hideFlags = HideFlags.HideAndDontSave;
            bool isSubGraph;
            string extension = Path.GetExtension(assetPath).Replace(".", "");
            switch (extension)
            {
                case ShaderGraphImporter.Extension:
                    isSubGraph = false;
                    break;
                case ShaderGraphImporter.LegacyExtension:
                    isSubGraph = false;
                    break;
                case ShaderSubGraphImporter.Extension:
                    isSubGraph = true;
                    break;
                default:
                    throw new Exception($"Invalid file extension {extension}");
            }
            string assetGuid = AssetDatabase.AssetPathToGUID(assetPath);
            graphObject.graph = new GraphData
            {
                assetGuid = assetGuid,
                isSubGraph = isSubGraph,
                messageManager = null
            };
            MultiJson.Deserialize(graphObject.graph, textGraph);
            graphObject.graph.OnEnable();
            graphObject.graph.ValidateGraph();
            return graphObject.graph;
        }
    }
}