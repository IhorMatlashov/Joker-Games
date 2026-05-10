using Data;
using Menu;
using UnityEngine;

namespace Roulette
{
    public class RouletteVariantSwitcher : MonoBehaviour
    {
        [Header("Wheel rigs (toggled active per variant)")]
        [SerializeField] private GameObject americanRig;
        [SerializeField] private GameObject europeanRig;

        [Header("Shared spinner + ball")]
        [SerializeField] private RouletteSpinner spinner;
        [SerializeField] private RouletteBall ball;

        public RouletteSpinner Spinner { get => spinner; set => spinner = value; }

        [Header("Active wheel/disc per rig")]
        [SerializeField] private RouletteWheel americanWheel;
        [SerializeField] private RouletteWheel europeanWheel;

        [Header("Optional: table material swap per variant")]
        [SerializeField] private Renderer tableRenderer;
        [SerializeField] private int tableMaterialIndex = 0;
        [SerializeField] private Material americanTableMaterial;
        [SerializeField] private Material europeanTableMaterial;

        private void Awake()
        {
            Apply(MainMenuController.PickedVariant ?? RouletteVariant.European);
        }

        public void Apply(RouletteVariant variant)
        {
            bool useAmerican = variant == RouletteVariant.American;

            if (americanRig != null) americanRig.SetActive(useAmerican);
            if (europeanRig != null) europeanRig.SetActive(!useAmerican);

            if (spinner != null)
            {
                spinner.Variant = variant;
                spinner.Wheel = useAmerican ? americanWheel : europeanWheel;
                if (ball != null) spinner.Ball = ball;
            }

            ApplyTableMaterial(useAmerican);
        }

        private void ApplyTableMaterial(bool useAmerican)
        {
            if (tableRenderer == null) return;
            var target = useAmerican ? americanTableMaterial : europeanTableMaterial;
            if (target == null) return;

            var mats = tableRenderer.sharedMaterials;
            if (mats == null || mats.Length == 0) return;
            var idx = Mathf.Clamp(tableMaterialIndex, 0, mats.Length - 1);
            if (mats[idx] == target) return;
            mats[idx] = target;
            tableRenderer.sharedMaterials = mats;
        }
    }
}
