﻿#region copyright
//--------------------------------------------------------------------
// Copyright (C) 2015 Dmitriy Yukhanov - focus [http://codestage.net].
//--------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using CodeStage.Maintainer.Tools;
using UnityEditor;
using UnityEngine;

namespace CodeStage.Maintainer.Core
{
	[Serializable]
	public enum AssetSettingsKind
	{
		NotSettings = 0,
		AudioManager = 100,
		ClusterInputManager = 200,
		DynamicsManager = 300,
		EditorBuildSettings = 400,
		EditorSettings = 500,
		GraphicsSettings = 600,
		InputManager = 700,
		NavMeshAreas = 800,
		NavMeshLayers = 900,
		NavMeshProjectSettings = 1000,
		NetworkManager = 1100,
		Physics2DSettings = 1200,
		ProjectSettings = 1300,
		PresetManager = 1400,
		QualitySettings = 1500,
		TagManager = 1600,
		TimeManager = 1700,
		UnityAdsSettings = 1800,
		UnityConnectSettings = 1900,
		Unknown = 100000
	}

	public class RawAssetInfo
	{
		public string path;
		public string guid;
		public bool isSettingsAsset;
	}

	[Serializable]
	public class AssetInfo
	{
		public string AssetGUID { get; private set; }
		public string AssetPath { get; private set; }
		public Type Type { get; private set; }
		public long Size { get; private set; }
		public AssetSettingsKind SettingsKind { get; private set; }

		public readonly List<string> referenceGuids = new List<string>();
		public readonly List<ReferenceInfo> referencesInfo = new List<ReferenceInfo>();
		public readonly List<ReferencedAtInfo> referencedAtInfoList = new List<ReferencedAtInfo>();

		public bool needToRebuildReferences = true;

		private long lastTimestamp;
		private long lastSize;
		private System.IO.FileInfo fileInfo;

		public static AssetInfo Create(RawAssetInfo rawAssetInfo, Type type, AssetSettingsKind settingsKind)
		{
			if (string.IsNullOrEmpty(rawAssetInfo.guid))
			{
				Debug.LogError("Can't create AssetInfo since guid is invalid!");
				return null;
			}

			var newAsset = new AssetInfo
			{
				AssetGUID = rawAssetInfo.guid,
				AssetPath = rawAssetInfo.path,
				Type = type,
				SettingsKind = settingsKind,
				fileInfo = new System.IO.FileInfo(rawAssetInfo.path)
			};

			newAsset.UpdateIfNeeded();

			return newAsset;
		}

		private AssetInfo() { }

		public void ActualizePath()
		{
			var actualPath = AssetDatabase.GUIDToAssetPath(AssetGUID);
			if (!string.IsNullOrEmpty(actualPath) && actualPath != AssetPath)
			{
				fileInfo = new System.IO.FileInfo(actualPath);
				AssetPath = actualPath;
			}
		}

		public void UpdateIfNeeded()
		{
			if (string.IsNullOrEmpty(AssetPath))
			{
				Debug.LogWarning(Maintainer.LogPrefix + "Can't update Asset since path is not set!");
				return;
			}

			/*if (AssetPath.Contains("Button.prefab"))
			{
				Debug.Log(AssetPath);
			}*/

			fileInfo.Refresh();

			if (!fileInfo.Exists)
			{
				Debug.LogWarning(Maintainer.LogPrefix + "Can't update asset since file at path is not found!");
				return;
			}

			var currentTimestamp = fileInfo.LastWriteTimeUtc.Ticks;
			var currentSize = fileInfo.Length;

			if (lastTimestamp == currentTimestamp && lastSize == currentSize)
			{
				for (var i = referenceGuids.Count - 1; i > -1; i--)
				{
					var guid = referenceGuids[i];
					var path = AssetDatabase.GUIDToAssetPath(guid);
					if (string.IsNullOrEmpty(path) || !System.IO.File.Exists(path))
					{
						referenceGuids.RemoveAt(i);

						foreach (var referenceInfo in referencesInfo)
						{
							if (referenceInfo.assetInfo.AssetGUID == guid)
							{
								referencesInfo.Remove(referenceInfo);
								break;
							}
						}
					}
				}

				if (!needToRebuildReferences) return;
			}

			foreach (var referenceInfo in referencesInfo)
			{
				foreach (var info in referenceInfo.assetInfo.referencedAtInfoList)
				{
					if (info.assetInfo == this)
					{
						referenceInfo.assetInfo.referencedAtInfoList.Remove(info);
						break;
					}
				}
			}

			lastTimestamp = currentTimestamp;
			lastSize = currentSize;
			needToRebuildReferences = true;
			Size = fileInfo.Length;

			referencesInfo.Clear();
			referenceGuids.Clear();

			var dependencies = new List<string>();

			if (SettingsKind == AssetSettingsKind.NotSettings)
			{
#if UNITY_2017_3_OR_NEWER
				if (Type == typeof(UnityEditorInternal.AssemblyDefinitionAsset))
				{
					//TODO: check if bug 1020737 is fixed and this can be removed
					dependencies.AddRange(GetAssetsReferencedFromAssemblyDefinition(AssetPath));
				}
				else
				{
					dependencies.AddRange(AssetDatabase.GetDependencies(AssetPath, false));
				}
#else
				dependencies.AddRange(AssetDatabase.GetDependencies(AssetPath, false));
#endif
			}
			else
			{
				dependencies.AddRange(GetAssetsReferencedInPlayerSettingsAsset(AssetPath, SettingsKind));
			}

			// kept for debugging purposes
			/*if (AssetPath.Contains("1.unity"))
			{
				Debug.Log("1.unity non-recursive dependencies:");
				foreach (var reference in references)
				{
					Debug.Log(reference);
				}
			}*/

			if (Type == typeof(UnityEngine.Shader))
			{
				// below is a workaround for the shader fallbacks issue 902729
				// we just manually check shader for fallbacks recursively to make sure all fallback shaders are included to the references list
				ScanShaderForNestedFallbacks(dependencies, AssetPath);

				// below is an another workaround for dependencies not include #include-ed files, like *.cginc
				ScanFileForIncludes(dependencies, AssetPath);
			}

			if (Type == typeof(UnityEngine.TextAsset) && AssetPath.EndsWith(".cginc"))
			{
				// below is an another workaround for dependencies not include #include-ed files, like *.cginc
				ScanFileForIncludes(dependencies, AssetPath);
			}

			var guids = new string[dependencies.Count];

			for (var i = 0; i < dependencies.Count; i++)
			{
				guids[i] = AssetDatabase.AssetPathToGUID(dependencies[i]);
			}

			referenceGuids.AddRange(guids);
		}

		public List<AssetInfo> GetReferencesRecursive()
		{
			var result = new List<AssetInfo>();

			WalkReferencesRecursive(result, referencesInfo);

			return result;
		}

		public List<AssetInfo> GetReferencedAtRecursive()
		{
			var result = new List<AssetInfo>();

			WalkReferencedAtRecursive(result, referencedAtInfoList);

			return result;
		}

		public void Clean()
		{
			foreach (var referenceInfo in referencesInfo)
			{
				foreach (var info in referenceInfo.assetInfo.referencedAtInfoList)
				{
					if (info.assetInfo == this)
					{
						referenceInfo.assetInfo.referencedAtInfoList.Remove(info);
						break;
					}
				}
			}

			foreach (var referencedAtInfo in referencedAtInfoList)
			{
				foreach (var info in referencedAtInfo.assetInfo.referencesInfo)
				{
					if (info.assetInfo == this)
					{
						referencedAtInfo.assetInfo.referencesInfo.Remove(info);
						referencedAtInfo.assetInfo.needToRebuildReferences = true;
						break;
					}
				}
			}
		}

		public static void ScanShaderForNestedFallbacks(List<string> referencePaths, string shaderPath)
		{
			var shaderCode = new StringBuilder();

			using (var sr = System.IO.File.OpenText(shaderPath))
			{
				string s;
				while ((s = sr.ReadLine()) != null)
				{
					shaderCode.AppendLine(s);
				}
			}

			var shaderCodeString = shaderCode.ToString();

			var lastIndex = 0;

			while (lastIndex != -1)
			{
				lastIndex = shaderCodeString.IndexOf("fallback", lastIndex + 1, StringComparison.CurrentCultureIgnoreCase);
				if (lastIndex != -1)
				{
					var firstQuoteIndex = shaderCodeString.IndexOf('"', lastIndex);
					if (firstQuoteIndex == -1) continue;

					var whiteSpace = shaderCodeString.Substring(lastIndex + 8, firstQuoteIndex - (lastIndex + 8));
					whiteSpace = whiteSpace.Trim();
					if (whiteSpace.Length != 0) continue;

					var lastQuoteIndex = shaderCodeString.IndexOf('"', firstQuoteIndex + 1);
					if (lastQuoteIndex == -1) continue;

					var fallbackName = shaderCodeString.Substring(firstQuoteIndex + 1, lastQuoteIndex - firstQuoteIndex - 1);
					var fallbackShader = UnityEngine.Shader.Find(fallbackName);
					var fallbackShaderPath = AssetDatabase.GetAssetPath(fallbackShader);

					if (!fallbackShaderPath.StartsWith("Assets", StringComparison.Ordinal)) continue;

					if (referencePaths.IndexOf(fallbackShaderPath) == -1)
					{
						referencePaths.Add(fallbackShaderPath);
						ScanFileForIncludes(referencePaths, fallbackShaderPath);
					}

					ScanShaderForNestedFallbacks(referencePaths, fallbackShaderPath);
				}
			}
		}

		private static void ScanFileForIncludes(List<string> referencePaths, string filePath)
		{
			var shaderLines = System.IO.File.ReadAllLines(filePath);
			foreach (var line in shaderLines)
			{
				var includeIndex = line.IndexOf("include", StringComparison.Ordinal);
				if (includeIndex == -1) continue;

				var noSharp = line.IndexOf('#', 0, includeIndex) == -1;
				if (noSharp) continue;

				var indexOfFirstQuote = line.IndexOf('"', includeIndex);
				if (indexOfFirstQuote == -1) continue;

				var indexOfLastQuote = line.IndexOf('"', indexOfFirstQuote + 1);
				if (indexOfLastQuote == -1) continue;

				var path = line.Substring(indexOfFirstQuote + 1, indexOfLastQuote - indexOfFirstQuote - 1);
				path = path.Replace('\\', '/');

				string assetPath;

				if (path.StartsWith("Assets/"))
				{
					assetPath = path;
				}
				else if (path.IndexOf('/') != -1)
				{
					var folder = System.IO.Path.GetDirectoryName(filePath);
					if (folder == null) continue;

					var combinedPath = System.IO.Path.Combine(folder, path);
					var fullPath = System.IO.Path.GetFullPath(combinedPath).Replace('\\', '/');
					var assetsIndex = fullPath.IndexOf("Assets/", StringComparison.Ordinal);
					if (assetsIndex == -1) continue;

					assetPath = fullPath.Substring(assetsIndex, fullPath.Length - assetsIndex);
				}
				else
				{
					var folder = System.IO.Path.GetDirectoryName(filePath);
					if (folder == null) continue;

					assetPath = System.IO.Path.Combine(folder, path).Replace('\\', '/');
				}

				if (!System.IO.File.Exists(assetPath)) continue;

				if (referencePaths.IndexOf(assetPath) != -1) continue;
				{
					referencePaths.Add(assetPath);
				}
			}
		}

		private void WalkReferencesRecursive(List<AssetInfo> result, List<ReferenceInfo> referenceInfos)
		{
			foreach (var referenceInfo in referenceInfos)
			{
				if (result.IndexOf(referenceInfo.assetInfo) == -1)
				{
					result.Add(referenceInfo.assetInfo);
					WalkReferencesRecursive(result, referenceInfo.assetInfo.referencesInfo);
				}
			}
		}

		private void WalkReferencedAtRecursive(List<AssetInfo> result, List<ReferencedAtInfo> referencedAtInfos)
		{
			foreach (var referencedAtInfo in referencedAtInfos)
			{
				if (result.IndexOf(referencedAtInfo.assetInfo) == -1)
				{
					result.Add(referencedAtInfo.assetInfo);
					WalkReferencedAtRecursive(result, referencedAtInfo.assetInfo.referencedAtInfoList);
				}
			}
		}

		private static string[] GetAssetsReferencedInPlayerSettingsAsset(string assetPath, AssetSettingsKind settingsKind)
		{
			var referencedAssets = new List<string>();

			if (settingsKind == AssetSettingsKind.EditorBuildSettings)
			{
				referencedAssets.AddRange(CSSceneTools.GetScenesInBuild(true));
			}
			else
			{
				var settingsAsset = AssetDatabase.LoadAllAssetsAtPath(assetPath);
				if (settingsAsset != null && settingsAsset.Length > 0)
				{
					var settingsAssetSerialized = new SerializedObject(settingsAsset[0]);

					var sp = settingsAssetSerialized.GetIterator();
					while (sp.Next(true))
					{
						if (sp.propertyType == SerializedPropertyType.ObjectReference)
						{
							var instanceId = sp.objectReferenceInstanceIDValue;
							if (instanceId != 0)
							{
								var path = AssetDatabase.GetAssetPath(instanceId);
								if (!string.IsNullOrEmpty(path) && path.StartsWith("Assets"))
								{
									if (referencedAssets.IndexOf(path) == -1)
										referencedAssets.Add(path);
								}
							}
						}
					}
				}
			}

			return referencedAssets.ToArray();
		}

#if UNITY_2017_3_OR_NEWER
		private List<string> GetAssetsReferencedFromAssemblyDefinition(string assetPath)
		{
			var result = new List<string>();

			var asset = AssetDatabase.LoadAssetAtPath<UnityEditorInternal.AssemblyDefinitionAsset>(assetPath);
			var data = JsonUtility.FromJson<AssemblyDefinitionData>(asset.text);

			foreach (var reference in data.references)
			{
				var assemblyDefinitionFilePathFromAssemblyName = UnityEditor.Compilation.CompilationPipeline.GetAssemblyDefinitionFilePathFromAssemblyName(reference);
				if (!string.IsNullOrEmpty(assemblyDefinitionFilePathFromAssemblyName))
				{
					result.Add(assemblyDefinitionFilePathFromAssemblyName);
				}
			}

			data.references = null;

			return result;
		}

		private class AssemblyDefinitionData
		{
			public string[] references;
		}
#endif
	}
}