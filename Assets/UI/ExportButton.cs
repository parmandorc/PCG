using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.IO;

// Class responsible for launching the exporting process of the map to a JSON file.
public class ExportButton : MonoBehaviour {

	public void Export() {

		TerrainCharacteristicsManager TCM = TerrainCharacteristicsManager.Instance;

		if (TCM != null) {

			string path = EditorUtility.SaveFilePanel("Save map data as JSON", "", "map.json", "json");

			if (path.Length != 0) {

				try {
					File.WriteAllText (path, TCM.SaveDataToJson());
				} catch (Exception e) {
					EditorUtility.DisplayDialog ("Exporting error", "An error has occurred while trying to export the map data to a JSON file.", "OK");
				}

			}
		}
	}
}
