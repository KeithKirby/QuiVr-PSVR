#region copyright
//--------------------------------------------------------------------
// Copyright (C) 2015 Dmitriy Yukhanov - focus [http://codestage.net].
//--------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CodeStage.Maintainer.Settings;
using CodeStage.Maintainer.Tools;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace CodeStage.Maintainer.Core
{
	[Serializable]
	public class AssetsMap
	{
		private const string MapPath = "Library/MaintainerMap.dat";

		private static AssetsMap cachedMap;

		public readonly List<AssetInfo> assets = new List<AssetInfo>();
		public string version;

		public static AssetsMap CreateNew()
		{
			Delete();
			return GetUpdated();
		}

		public static void Delete()
		{
			cachedMap = null;
			if (File.Exists(MapPath))
			{
				CSEditorTools.RemoveReadOnlyAttribute(MapPath);
				File.Delete(MapPath);
			}
		}

		public static AssetsMap GetUpdated()
		{
			if (cachedMap == null)
			{
				cachedMap = LoadMap(MapPath);
			}

			if (cachedMap == null)
			{
				cachedMap = new AssetsMap {version = Maintainer.Version};
			}

			try
			{
				if (UpdateMap(cachedMap))
				{
					SaveMap(MapPath, cachedMap);
				}
				else
				{
					cachedMap.assets.Clear();
					cachedMap = null;
				}
			}
			catch (Exception e)
			{
				Debug.LogError(e);
			}

			EditorUtility.ClearProgressBar();

			return cachedMap;
		}

		public static void Save()
		{
			if (cachedMap != null)
			{
				SaveMap(MapPath, cachedMap);
			}
			else
			{
				Debug.LogError(Maintainer.ConstructError("Can't save AssetsMap, no cache found!"));
			}
		}

		public int GetAssetInfoIndex(AssetInfo assetInfo)
		{
			return assets.IndexOf(assetInfo);
		}

		public AssetInfo GetAssetInfoAtIndex(int index)
		{
			var items = GetUpdated().assets;
			return index > items.Count - 1 ? null : items[index];
		}

		private static bool UpdateMap(AssetsMap map)
		{
			// ----------------------------------------
			// getting all valid assets within project 
			// ----------------------------------------
			if (EditorUtility.DisplayCancelableProgressBar("Updating Assets Map, phase 1 of 4", "Getting all valid assets...", 0))
			{
				Debug.LogError(Maintainer.LogPrefix + "Assets Map update was canceled by user.");
				return false;
			}

			var allAssetPaths = AssetDatabase.GetAllAssetPaths();
			var validNewAssets = new List<RawAssetInfo>(allAssetPaths.Length);

			foreach (var assetPath in allAssetPaths)
			{
				var settingsAsset = false;

				/*if (assetPath.Contains(@"NewAssembly"))
				{
					Debug.Log(assetPath);
				}*/

				var projectAsset = assetPath.IndexOf("Assets/", StringComparison.Ordinal) == 0;
				if (!projectAsset)
				{
					settingsAsset = assetPath.IndexOf("ProjectSettings/", StringComparison.Ordinal) == 0;
					if (!settingsAsset) continue;
				}

				if (!File.Exists(assetPath)) continue;
				if (AssetDatabase.IsValidFolder(assetPath)) continue;

				var guid = AssetDatabase.AssetPathToGUID(assetPath);
				var rawInfo = new RawAssetInfo
				{
					path = assetPath,
					guid = guid,
					isSettingsAsset = settingsAsset
				};

				validNewAssets.Add(rawInfo);
			}

			// -----------------------------
			// checking existing map assets 
			// -----------------------------

			if (EditorUtility.DisplayCancelableProgressBar("Updating Assets Map, phase 2 of 4", "Checking existing assets in map...", 0))
			{
				Debug.LogError(Maintainer.LogPrefix + "Assets Map update was canceled by user.");
				return false;
			}

			var count = map.assets.Count;
			var updateStep = Math.Max(count / MaintainerSettings.UpdateProgressStep, 1);
			for (var i = count - 1; i > -1; i--)
			{
				if (i % updateStep == 0 && i != 0)
				{
					var index = count - i;
					if (EditorUtility.DisplayCancelableProgressBar("Updating Assets Map, phase 2 of 4", "Checking existing assets in map..." + index + "/" + count, (float) index / count))
					{
						EditorUtility.ClearProgressBar();
						Debug.LogError(Maintainer.LogPrefix + "Assets Map update was canceled by user.");
						return false;
					}
				}

				var assetInMap = map.assets[i];
				assetInMap.ActualizePath();

				if (File.Exists(assetInMap.AssetPath))
				{
					validNewAssets.RemoveAll(a => a.guid == assetInMap.AssetGUID);
					assetInMap.UpdateIfNeeded();
				}
				else
				{
					assetInMap.Clean();
					map.assets.RemoveAt(i);
				}
			}

			// ------------------------
			// dealing with new assets
			// ------------------------

			if (EditorUtility.DisplayCancelableProgressBar("Updating Assets Map, phase 3 of 4", "Looking for new assets...", 0))
			{
				Debug.LogError(Maintainer.LogPrefix + "Assets Map update was canceled by user.");
				return false;
			}

			count = validNewAssets.Count;
			updateStep = Math.Max(count / MaintainerSettings.UpdateProgressStep, 1);

			for (var i = 0; i < count; i++)
			{
				if (i % updateStep == 0 && i != 0)
				{
					if (EditorUtility.DisplayCancelableProgressBar("Updating Assets Map, phase 3 of 4",
						"Looking for new assets..." + (i + 1) + "/" + count, (float)i / count))
					{
						Debug.LogError(Maintainer.LogPrefix + "Assets Map update was canceled by user.");
						return false;
					}
				}

				var rawAssetInfo = validNewAssets[i];

				/*if (assetPath.Contains(@"789789"))
				{
					Debug.Log(AssetDatabase.AssetPathToGUID(assetPath));
				}*/

				var type = AssetDatabase.GetMainAssetTypeAtPath(rawAssetInfo.path);
				if (type == null)
				{
					var loadedAsset = AssetDatabase.LoadMainAssetAtPath(rawAssetInfo.path);
					if (loadedAsset == null)
					{
						Debug.LogWarning(Maintainer.LogPrefix + "Can't retrieve type of the asset:\n" + rawAssetInfo.path);
						continue;
					}

					type = loadedAsset.GetType();
				}
				
				var settingsKind = rawAssetInfo.isSettingsAsset ? GetSettingsKind(rawAssetInfo.path) : AssetSettingsKind.NotSettings;

				var asset = AssetInfo.Create(rawAssetInfo, type, settingsKind);
				map.assets.Add(asset);
			}

			if (EditorUtility.DisplayCancelableProgressBar("Updating Assets Map, phase 4 of 4", "Generating links...", 0))
			{
				Debug.LogError(Maintainer.LogPrefix + "Assets Map update was canceled by user.");
				return false;
			}

			count = map.assets.Count;
			updateStep = Math.Max(count / MaintainerSettings.UpdateProgressStep, 1);

			for (var i = 0; i < count; i++)
			{
				if (i % updateStep == 0 && i != 0)
				{
					if (EditorUtility.DisplayCancelableProgressBar("Updating Assets Map, phase 4 of 4", "Generating links..." + (i + 1) + "/" + count, (float)i / count))
					{
						Debug.LogError(Maintainer.LogPrefix + "Assets Map update was canceled by user.");
						return false;
					}
				}

				var asset = map.assets[i];

				if (!asset.needToRebuildReferences) continue;

				var references = asset.referenceGuids;
				foreach (var reference in references)
				{
					foreach (var mapAsset in map.assets)
					{
						if (mapAsset.AssetGUID == reference)
						{
							if (mapAsset.Type == typeof(Font) && asset.Type == typeof(Font)) continue;

							var referencedAtInfo = new ReferencedAtInfo()
							{
								assetInfo = asset
							};

							var referenceInfo = new ReferenceInfo()
							{
								assetInfo = mapAsset
							};

							mapAsset.referencedAtInfoList.Add(referencedAtInfo);
							asset.referencesInfo.Add(referenceInfo);
						}
					}
				}

				asset.needToRebuildReferences = false;
			}

			/*Debug.Log("Total assets in map: " + map.assets.Count);
			foreach (var mapAsset in map.assets)
			{
				//if (!(mapAsset.path.Contains("frag_ab") || mapAsset.path.Contains("frag_ac"))) continue;
				if (!mapAsset.AssetPath.Contains("NewAssembly")) continue;

				Debug.Log("==================================================\n" + mapAsset.AssetPath + "\n" + mapAsset.AssetPath);
				Debug.Log("[REFERENCED BY]");
				foreach (var reference in mapAsset.referencedAtInfoList)
				{
					Debug.Log(reference.assetInfo.AssetPath);
				}

				Debug.Log("[REFERENCES]");
				foreach (var reference in mapAsset.referencesInfo)
				{
					Debug.Log(reference.assetInfo.AssetPath);
				} 
			}*/

			return true;
		}

		private static AssetsMap LoadMap(string path)
		{
			if (!File.Exists(path)) return null;

			var fileSize = new FileInfo(path).Length;

			if (fileSize > 500000)
			{
				EditorUtility.DisplayProgressBar("Loading Assets Map", "Please wait...", 0);
			}

			AssetsMap result = null;
			var bf = new BinaryFormatter();
			var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

			try
			{
				result = bf.Deserialize(stream) as AssetsMap;

				if (result != null && result.version != Maintainer.Version)
				{
					result = null;
				}
			}
			catch (Exception)
			{
				Debug.Log(Maintainer.LogPrefix + "Couldn't read assets map (more likely you've updated Maintainer recently).\nThis message is harmless unless repeating on every Maintainer run."); //"\n" + ex);
			}
			finally
			{
				stream.Close();
			}

			EditorUtility.ClearProgressBar();

			return result;
		}

		private static void SaveMap(string path, AssetsMap map)
		{
			if (map.assets.Count > 10000)
			{
				EditorUtility.DisplayProgressBar("Saving Assets Map", "Please wait...", 0);
			}

			var bf = new BinaryFormatter();
			var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
			bf.Serialize(stream, map);
			stream.Close();

			EditorUtility.ClearProgressBar();
		}
		
		private static AssetSettingsKind GetSettingsKind(string assetPath)
		{
			var result = AssetSettingsKind.Unknown;

			var fileName = Path.GetFileNameWithoutExtension(assetPath);
			if (!string.IsNullOrEmpty(fileName))
			{
				try
				{
					result = (AssetSettingsKind)Enum.Parse(typeof(AssetSettingsKind), fileName);
				}
				catch (Exception)
				{
					// ignored
				}
			}

			return result;
		}
	}
}