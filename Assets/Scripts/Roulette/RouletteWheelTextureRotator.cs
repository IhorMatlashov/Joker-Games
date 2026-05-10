using UnityEngine;

namespace Roulette
{
    [ExecuteAlways]
    public class RouletteWheelTextureRotator : MonoBehaviour
    {
        [SerializeField] private Renderer targetRenderer;

        [SerializeField] private string rotationProperty = "_Rotation";

        [SerializeField, Range(0f, 360f)] private float rotationDegrees = 0f;

        private static readonly int RotationId = Shader.PropertyToID("_Rotation");
        private MaterialPropertyBlock _mpb;
        private int _cachedPropertyId;
        private string _cachedPropertyName;

        private void Reset() { targetRenderer = GetComponent<Renderer>(); }
        private void OnEnable() => Apply();

        public void SetRotation(float degrees)
        {
            rotationDegrees = degrees;
            Apply();
        }

        public void AddRotation(float deltaDegrees)
        {
            rotationDegrees = Mathf.Repeat(rotationDegrees + deltaDegrees, 360f);
            Apply();
        }

        private int PropertyId
        {
            get
            {
                if (_cachedPropertyName == rotationProperty) return _cachedPropertyId;
                _cachedPropertyName = rotationProperty;
                _cachedPropertyId = string.IsNullOrEmpty(rotationProperty)
                    ? RotationId
                    : Shader.PropertyToID(rotationProperty);
                return _cachedPropertyId;
            }
        }

        private void Apply()
        {
            if (targetRenderer == null) return;

            if (_mpb == null) _mpb = new MaterialPropertyBlock();
            targetRenderer.GetPropertyBlock(_mpb);
            _mpb.SetFloat(PropertyId, rotationDegrees);
            targetRenderer.SetPropertyBlock(_mpb);
        }
    }
}
