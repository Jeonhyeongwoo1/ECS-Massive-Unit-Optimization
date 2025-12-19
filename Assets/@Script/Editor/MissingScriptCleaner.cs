using UnityEngine;
using UnityEditor;

public class MissingScriptCleanerSelected : MonoBehaviour
{
    [MenuItem("Tools/Clean Missing Scripts From Selected")]
    private static void CleanMissingScriptsFromSelected()
    {
        GameObject[] selectedObjects = Selection.gameObjects;

        if (selectedObjects == null || selectedObjects.Length == 0)
        {
            Debug.LogWarning("[MissingScriptCleaner] No GameObjects selected!");
            return;
        }

        int removedCount = 0;

        foreach (GameObject selected in selectedObjects)
        {
            // 선택한 오브젝트와 하위 오브젝트 전부 가져오기 (비활성화된 것도 포함)
            Transform[] transforms = selected.GetComponentsInChildren<Transform>(true);

            foreach (Transform t in transforms)
            {
                GameObject go = t.gameObject;
                int count = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go);

                if (count > 0)
                {
                    Undo.RegisterCompleteObjectUndo(go, "Remove missing scripts");
                    GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
                    removedCount += count;
                }
            }
        }

        if (removedCount > 0)
        {
            Debug.Log($"[MissingScriptCleaner] Removed {removedCount} missing scripts from selected GameObjects.");
        }
        else
        {
            Debug.Log("[MissingScriptCleaner] No missing scripts found in selected GameObjects.");
        }
    }
}