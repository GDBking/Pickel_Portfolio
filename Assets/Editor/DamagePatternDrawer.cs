using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DamagePattern))]
public class DamagePatternDrawer : PropertyDrawer
{
    const float SIZE = 25f;
    const float PAD = 2f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var rangeProp = property.FindPropertyRelative("range");
        var list = property.FindPropertyRelative("cells");
        var isDirProp = property.FindPropertyRelative("isDir");

        // =========================
        // range
        // =========================
        EditorGUI.BeginChangeCheck();

        EditorGUI.PropertyField(
            new Rect(position.x, position.y, position.width, 18),
            rangeProp
        );

        if (EditorGUI.EndChangeCheck())
        {
            rangeProp.serializedObject.ApplyModifiedProperties();
            ClampCellsToRange(list, rangeProp.intValue);
        }

        position.y += 20;

        // =========================
        // isDir
        // =========================
        EditorGUI.PropertyField(
            new Rect(position.x, position.y, position.width, 18),
            isDirProp
        );

        position.y += 22;

        int range = Mathf.Max(1, rangeProp.intValue);
        int gridSize = range * 2 + 1;

        // =========================
        // Grid
        // =========================
        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                int offsetX = x - range;
                int offsetY = (gridSize - 1 - y) - range;

                Vector3Int offset = new(offsetX, offsetY, 0);
                bool isCenter = offset == Vector3Int.zero;

                Rect rect = new(
                    position.x + x * (SIZE + PAD),
                    position.y + y * (SIZE + PAD),
                    SIZE,
                    SIZE
                );

                bool exists = Contains(list, offset);
                bool isDir = isDirProp.boolValue;

                Color prev = GUI.color;
                GUI.color = exists ? Color.green : Color.gray;

                GUI.enabled = !isCenter;

                // 중앙 셀 표시
                string labelText = "";
                if (isCenter)
                    labelText = isDir ? "↓" : "C";

                if (GUI.Button(rect, labelText))
                {
                    if (exists) Remove(list, offset);
                    else Add(list, offset);
                }

                GUI.enabled = true;
                GUI.color = prev;
            }
        }

        EditorGUI.EndProperty();
    }

    // =========================
    // range 줄었을 때 자동 정리
    // =========================
    void ClampCellsToRange(SerializedProperty list, int range)
    {
        for (int i = list.arraySize - 1; i >= 0; i--)
        {
            var elem = list.GetArrayElementAtIndex(i);
            var offset = elem.FindPropertyRelative("offset").vector3IntValue;

            if (Mathf.Abs(offset.x) > range || Mathf.Abs(offset.y) > range)
            {
                list.DeleteArrayElementAtIndex(i);
            }
        }
    }

    bool Contains(SerializedProperty list, Vector3Int offset)
    {
        for (int i = 0; i < list.arraySize; i++)
        {
            var elem = list.GetArrayElementAtIndex(i);
            if (elem.FindPropertyRelative("offset").vector3IntValue == offset)
                return true;
        }
        return false;
    }

    void Add(SerializedProperty list, Vector3Int offset)
    {
        if (Contains(list, offset))
            return;

        list.arraySize++;
        var elem = list.GetArrayElementAtIndex(list.arraySize - 1);

        elem.FindPropertyRelative("offset").vector3IntValue = offset;
        elem.FindPropertyRelative("direction").vector2Value = GetDirection(offset);
    }

    void Remove(SerializedProperty list, Vector3Int offset)
    {
        for (int i = list.arraySize - 1; i >= 0; i--)
        {
            var elem = list.GetArrayElementAtIndex(i);

            if (elem.FindPropertyRelative("offset").vector3IntValue == offset)
            {
                list.DeleteArrayElementAtIndex(i);
                break;
            }
        }
    }

    Vector2 GetDirection(Vector3Int offset)
    {
        // 대각선이면 x 기준 방향
        if (offset.x != 0 && offset.y != 0)
            return new Vector2(-Mathf.Sign(offset.x), 0);

        return new Vector2(-offset.x, -offset.y);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var rangeProp = property.FindPropertyRelative("range");
        int range = Mathf.Max(1, rangeProp.intValue);
        int gridSize = range * 2 + 1;

        // range + isDir + grid
        return 40 + (SIZE + PAD) * gridSize;
    }
}