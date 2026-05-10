using Roulette;
using UnityEditor;
using UnityEngine;

namespace JokerEditor
{
    /// <summary>
    /// Inspector extension for <see cref="RouletteWheel"/> — adds inline
    /// calibration buttons that let designers nudge the pocket-0 offset by
    /// exactly one pocket per click while watching the wheel react in
    /// scene view. Editor-only, never compiled into runtime builds.
    /// </summary>
    [CustomEditor(typeof(RouletteWheel))]
    public class RouletteWheelEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Calibration", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Run a deterministic spin to '0' and use these buttons to align " +
                "pocket 0 with the ball's landing position. Save the scene when done.",
                MessageType.Info);

            var offset = serializedObject.FindProperty("pocket0OffsetDegrees");
            var clockwise = serializedObject.FindProperty("clockwiseLayout");

            EditorGUILayout.LabelField($"Current offset: {offset.floatValue:F2}°");

            EditorGUILayout.LabelField("European wheel (37 pockets)", EditorStyles.miniBoldLabel);
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("− 1 pocket")) Nudge(offset, -360f / 37f);
                if (GUILayout.Button("+ 1 pocket")) Nudge(offset, +360f / 37f);
            }

            EditorGUILayout.LabelField("American wheel (38 pockets)", EditorStyles.miniBoldLabel);
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("− 1 pocket")) Nudge(offset, -360f / 38f);
                if (GUILayout.Button("+ 1 pocket")) Nudge(offset, +360f / 38f);
            }

            EditorGUILayout.Space();
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Flip layout direction")) clockwise.boolValue = !clockwise.boolValue;
                if (GUILayout.Button("Reset offset to 0")) offset.floatValue = 0f;
            }

            // Live preview — rotate the disc so pocket 0 sits at +Z right now.
            if (GUILayout.Button("Snap mesh preview to 0"))
            {
                var wheel = (RouletteWheel)target;
                wheel.DiscTransform.localRotation = Quaternion.AngleAxis(-offset.floatValue, Vector3.up);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private static void Nudge(SerializedProperty offset, float delta)
        {
            float v = (offset.floatValue + delta) % 360f;
            if (v < 0f) v += 360f;
            offset.floatValue = v;
        }
    }
}
