using UnityEditor;
using UnityEngine;
using System;

[InitializeOnLoad]
public static class HierarchyDoubleClickFocus
{
    private static int lastClickedInstanceID;
    private static double lastClickTime;
    private const double doubleClickTime = 0.3;

    static HierarchyDoubleClickFocus()
    {
        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
    }

    private static void OnHierarchyGUI(int instanceID, Rect selectionRect)
    {
        Event e = Event.current;

        if (e.type != EventType.MouseDown || e.button != 0)
            return;

        // Clique dentro do item da Hierarchy
        if (!selectionRect.Contains(e.mousePosition))
            return;

        double time = EditorApplication.timeSinceStartup;

        if (instanceID == lastClickedInstanceID &&
            time - lastClickTime < doubleClickTime)
        {
            FocusAllSceneViews();
            e.Use();
        }

        lastClickedInstanceID = instanceID;
        lastClickTime = time;
    }

    private static void FocusAllSceneViews()
    {
        foreach (SceneView sceneView in SceneView.sceneViews)
        {
            if (sceneView != null)
            {
                sceneView.FrameSelected();
            }
        }
    }
}
