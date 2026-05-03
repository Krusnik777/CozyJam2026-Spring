using UnityEditor;
using UnityEngine;

public class RandomizeSelectedTransformsWindow : EditorWindow
{
    private bool useX = true;
    private bool useY = true;
    private bool useZ = true;

    private bool randomizePosition = true;
    private bool randomizeRotation = false;

    private bool useLocalPosition = false;
    private bool useLocalRotation = false;

    private Vector3 positionMin = new Vector3(-1f, 0f, -1f);
    private Vector3 positionMax = new Vector3(1f, 0f, 1f);

    private bool usePositionStep = false;
    private Vector3 positionStep = new Vector3(1f, 1f, 1f);

    private Vector3 rotationMin = new Vector3(0f, 0f, 0f);
    private Vector3 rotationMax = new Vector3(0f, 360f, 0f);

    private bool useRotationStep = true;
    private Vector3 rotationStep = new Vector3(90f, 90f, 90f);

    [MenuItem("Tools/Randomize Selected Transforms")]
    public static void ShowWindow()
    {
        GetWindow<RandomizeSelectedTransformsWindow>("Randomize Transforms");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Randomize Selected Transforms", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Axes", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        useX = EditorGUILayout.ToggleLeft("X", useX, GUILayout.Width(60));
        useY = EditorGUILayout.ToggleLeft("Y", useY, GUILayout.Width(60));
        useZ = EditorGUILayout.ToggleLeft("Z", useZ, GUILayout.Width(60));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(10);

        randomizePosition = EditorGUILayout.ToggleLeft("Randomize Position", randomizePosition);

        if (randomizePosition)
        {
            useLocalPosition = EditorGUILayout.ToggleLeft("Use Local Position", useLocalPosition);

            positionMin = EditorGUILayout.Vector3Field("Position Min", positionMin);
            positionMax = EditorGUILayout.Vector3Field("Position Max", positionMax);

            usePositionStep = EditorGUILayout.ToggleLeft("Use Position Step", usePositionStep);

            if (usePositionStep)
            {
                positionStep = EditorGUILayout.Vector3Field("Position Step", positionStep);
            }
        }

        EditorGUILayout.Space(10);

        randomizeRotation = EditorGUILayout.ToggleLeft("Randomize Rotation", randomizeRotation);

        if (randomizeRotation)
        {
            useLocalRotation = EditorGUILayout.ToggleLeft("Use Local Rotation", useLocalRotation);

            rotationMin = EditorGUILayout.Vector3Field("Rotation Min", rotationMin);
            rotationMax = EditorGUILayout.Vector3Field("Rotation Max", rotationMax);

            useRotationStep = EditorGUILayout.ToggleLeft("Use Rotation Step", useRotationStep);

            if (useRotationStep)
            {
                rotationStep = EditorGUILayout.Vector3Field("Rotation Step", rotationStep);
            }
        }

        EditorGUILayout.Space(15);

        using (new EditorGUI.DisabledScope(Selection.transforms.Length == 0))
        {
            if (GUILayout.Button($"Randomize Selected ({Selection.transforms.Length})", GUILayout.Height(35)))
            {
                RandomizeSelected();
            }
        }

        if (Selection.transforms.Length == 0)
        {
            EditorGUILayout.HelpBox("Select one or more objects in the scene.", MessageType.Info);
        }
    }

    private void RandomizeSelected()
    {
        Transform[] selectedTransforms = Selection.transforms;

        Undo.SetCurrentGroupName("Randomize Selected Transforms");
        int undoGroup = Undo.GetCurrentGroup();

        foreach (Transform target in selectedTransforms)
        {
            Undo.RecordObject(target, "Randomize Transform");

            if (randomizePosition)
            {
                Vector3 currentPosition = useLocalPosition ? target.localPosition : target.position;
                Vector3 newPosition = currentPosition;

                if (useX)
                    newPosition.x = GetRandomValue(positionMin.x, positionMax.x, positionStep.x, usePositionStep);

                if (useY)
                    newPosition.y = GetRandomValue(positionMin.y, positionMax.y, positionStep.y, usePositionStep);

                if (useZ)
                    newPosition.z = GetRandomValue(positionMin.z, positionMax.z, positionStep.z, usePositionStep);

                if (useLocalPosition)
                    target.localPosition = newPosition;
                else
                    target.position = newPosition;
            }

            if (randomizeRotation)
            {
                Vector3 currentRotation = useLocalRotation
                    ? target.localEulerAngles
                    : target.eulerAngles;

                Vector3 newRotation = currentRotation;

                if (useX)
                    newRotation.x = GetRandomValue(rotationMin.x, rotationMax.x, rotationStep.x, useRotationStep);

                if (useY)
                    newRotation.y = GetRandomValue(rotationMin.y, rotationMax.y, rotationStep.y, useRotationStep);

                if (useZ)
                    newRotation.z = GetRandomValue(rotationMin.z, rotationMax.z, rotationStep.z, useRotationStep);

                if (useLocalRotation)
                    target.localRotation = Quaternion.Euler(newRotation);
                else
                    target.rotation = Quaternion.Euler(newRotation);
            }

            EditorUtility.SetDirty(target);
        }

        Undo.CollapseUndoOperations(undoGroup);
    }

    private float GetRandomValue(float min, float max, float step, bool useStep)
    {
        if (!useStep)
        {
            return Random.Range(min, max);
        }

        step = Mathf.Abs(step);

        if (step <= 0.0001f)
        {
            return Random.Range(min, max);
        }

        if (max < min)
        {
            float temp = min;
            min = max;
            max = temp;
        }

        int stepCount = Mathf.FloorToInt((max - min) / step);

        if (stepCount <= 0)
        {
            return min;
        }

        int randomStepIndex = Random.Range(0, stepCount + 1);

        return min + randomStepIndex * step;
    }
}