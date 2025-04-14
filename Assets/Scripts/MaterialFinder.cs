using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class SelectAllMaterialsInScene : EditorWindow
{
    [MenuItem("Tools/Select All Materials Used In Scene")]
    public static void SelectMaterialsInScene()
    {
        HashSet<Material> materials = new HashSet<Material>();

        Renderer[] renderers = GameObject.FindObjectsOfType<Renderer>();

        foreach (Renderer rend in renderers)
        {
            foreach (Material mat in rend.sharedMaterials)
            {
                if (mat != null)
                    materials.Add(mat);
            }
        }

        if (materials.Count > 0)
        {
            // Select materials in Project window
            Selection.objects = new List<Material>(materials).ToArray();
            Debug.Log($"Selected {materials.Count} materials used in the scene.");
        }
        else
        {
            Debug.LogWarning("No materials found in the scene.");
        }
    }
}
