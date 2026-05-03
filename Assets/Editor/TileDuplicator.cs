using UnityEditor;
using UnityEngine;

public class TileDuplicatorWindow : EditorWindow
{
    private bool useX = true;
    private bool useY = false;
    private bool useZ = false;

    private int countX = 5;
    private int countY = 1;
    private int countZ = 5;

    private bool includeOriginal = true;
    private bool useLocalDirection = false;
    private bool keepSameParent = true;

    [MenuItem("Tools/Tile Duplicator")]
    public static void ShowWindow()
    {
        GetWindow<TileDuplicatorWindow>("Tile Duplicator");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Tile Duplicator", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Directions", EditorStyles.boldLabel);

        useX = EditorGUILayout.ToggleLeft("Use X", useX);
        if (useX)
            countX = Mathf.Max(1, EditorGUILayout.IntField("Count X", countX));

        useY = EditorGUILayout.ToggleLeft("Use Y", useY);
        if (useY)
            countY = Mathf.Max(1, EditorGUILayout.IntField("Count Y", countY));

        useZ = EditorGUILayout.ToggleLeft("Use Z", useZ);
        if (useZ)
            countZ = Mathf.Max(1, EditorGUILayout.IntField("Count Z", countZ));

        EditorGUILayout.Space();

        includeOriginal = EditorGUILayout.ToggleLeft("Include Original", includeOriginal);
        useLocalDirection = EditorGUILayout.ToggleLeft("Use Local Direction", useLocalDirection);
        keepSameParent = EditorGUILayout.ToggleLeft("Keep Same Parent", keepSameParent);

        EditorGUILayout.Space(12);

        bool hasSelection = Selection.activeTransform != null;
        bool hasAtLeastOneAxis = useX || useY || useZ;

        using (new EditorGUI.DisabledScope(!hasSelection || !hasAtLeastOneAxis))
        {
            if (GUILayout.Button("Duplicate Tiles", GUILayout.Height(35)))
            {
                DuplicateTiles();
            }
        }

        if (!hasSelection)
        {
            EditorGUILayout.HelpBox("Select one tile object in the scene.", MessageType.Info);
        }
        else if (!hasAtLeastOneAxis)
        {
            EditorGUILayout.HelpBox("Enable at least one direction: X, Y or Z.", MessageType.Warning);
        }
    }

    private void DuplicateTiles()
    {
        Transform source = Selection.activeTransform;
        Bounds bounds = GetObjectBounds(source);

        float stepX = bounds.size.x;
        float stepY = bounds.size.y;
        float stepZ = bounds.size.z;

        Vector3 dirX = useLocalDirection ? source.right : Vector3.right;
        Vector3 dirY = useLocalDirection ? source.up : Vector3.up;
        Vector3 dirZ = useLocalDirection ? source.forward : Vector3.forward;

        int maxX = useX ? Mathf.Max(1, countX) : 1;
        int maxY = useY ? Mathf.Max(1, countY) : 1;
        int maxZ = useZ ? Mathf.Max(1, countZ) : 1;

        Undo.SetCurrentGroupName("Duplicate Tiles Grid");
        int undoGroup = Undo.GetCurrentGroup();

        for (int x = 0; x < maxX; x++)
        {
            for (int y = 0; y < maxY; y++)
            {
                for (int z = 0; z < maxZ; z++)
                {
                    // Пропускаем оригинал, если он уже должен остаться в сетке
                    if (includeOriginal && x == 0 && y == 0 && z == 0)
                        continue;

                    Vector3 offset =
                        dirX * (x * stepX) +
                        dirY * (y * stepY) +
                        dirZ * (z * stepZ);

                    GameObject duplicate = CreateDuplicate(source.gameObject);
                    if (duplicate == null)
                        continue;

                    Undo.RegisterCreatedObjectUndo(duplicate, "Duplicate Tile");

                    Transform t = duplicate.transform;

                    if (keepSameParent)
                        t.SetParent(source.parent);

                    t.position = source.position + offset;
                    t.rotation = source.rotation;
                    t.localScale = source.localScale;

                    duplicate.name = $"{source.name}_{x}_{y}_{z}";
                }
            }
        }

        Undo.CollapseUndoOperations(undoGroup);
    }

    private GameObject CreateDuplicate(GameObject source)
    {
        GameObject duplicate = null;

        if (PrefabUtility.IsPartOfAnyPrefab(source))
        {
            duplicate = (GameObject)PrefabUtility.InstantiatePrefab(source);
        }

        if (duplicate == null)
        {
            duplicate = Instantiate(source);
        }

        return duplicate;
    }

    private Bounds GetObjectBounds(Transform target)
    {
        Renderer[] renderers = target.GetComponentsInChildren<Renderer>();

        if (renderers == null || renderers.Length == 0)
        {
            return new Bounds(target.position, Vector3.one);
        }

        Bounds bounds = renderers[0].bounds;

        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        return bounds;
    }
}