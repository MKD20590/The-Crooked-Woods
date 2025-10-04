#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;

public class TreeToGameObject : MonoBehaviour
{
    [MenuItem("Tools/Convert Selected Terrain Trees To GameObjects")]
    static void ConvertSelected()
    {
        foreach (GameObject go in Selection.gameObjects)
        {
            Terrain t = go.GetComponent<Terrain>();
            if (t != null)
            {
                ConvertTerrain(t);
            }
        }
        Debug.Log("[ConvertTreesEditor] Done converting selected terrains.");
    }

    static void ConvertTerrain(Terrain terrain)
    {
        var data = terrain.terrainData;
        var instances = data.treeInstances;
        if (instances == null || instances.Length == 0)
        {
            Debug.LogWarning("[ConvertTreesEditor] No tree instances found for " + terrain.name);
            return;
        }

        GameObject parent = new GameObject(terrain.name + "_ConvertedTrees");
        Undo.RegisterCreatedObjectUndo(parent, "Create ConvertedTrees Parent");

        for (int i = 0; i < instances.Length; i++)
        {
            var ti = instances[i];
            if (ti.prototypeIndex < 0 || ti.prototypeIndex >= data.treePrototypes.Length) continue;
            GameObject prefab = data.treePrototypes[ti.prototypeIndex].prefab;
            if (prefab == null) continue;

            Vector3 worldPos = Vector3.Scale(ti.position, data.size) + terrain.transform.position;
            Quaternion rot = Quaternion.Euler(0f, ti.rotation * Mathf.Rad2Deg, 0f);
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            instance.transform.position = worldPos;
            instance.transform.rotation = rot;
            instance.transform.localScale = new Vector3(ti.widthScale, ti.heightScale, ti.widthScale);
            Undo.RegisterCreatedObjectUndo(instance, "Instantiate tree prefab");
            instance.transform.SetParent(parent.transform);
        }

        // set terrain treeDistance 0
        Undo.RecordObject(terrain, "Set treeDistance to 0");
        terrain.treeDistance = 0;
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(terrain.gameObject.scene);
        Debug.Log($"[ConvertTreesEditor] Converted {parent.transform.childCount} trees for terrain {terrain.name}");
    }
}
#endif
