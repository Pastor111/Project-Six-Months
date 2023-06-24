using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MainMenuUtils
{

    public enum ShadowQuality { VeryLOW_256, LOW_512, MEDIUM_1024, HIGH_2048, VERYHIGH_4096 }
    public enum TextureQuality { FullRes, HalfRes, QuaterRes, EightRes }

    //[System.Serializable]
    [CreateAssetMenu(fileName = "GraphicSetting", menuName = "New Graphics Setting")]
    public class GraphicsConfiguration : ScriptableObject
    {
        public string Name;
        [Range(0.25f, 3f)]
        public float ResolutionScale;
        public bool EnableShadows;
        public ShadowQuality shadowQuality;
        public ShadowQuality shadowQuality_additional_Lights;
        [Range(10f, 200f)]
        public float ShadowDistance;
        public TextureQuality textureQuality;

        public bool PostProcessing;
        public bool Bloom;
        public bool Vignette;
        public bool DepthOfField;
        public bool FXAA;
        public bool AO;
        public bool HighQualityFXAA;
        public bool SoftShadows;

        public GraphicsConfiguration()
        {

        }

        public GraphicsConfiguration(SavebleGraphicsConfig config)
        {
            Name = config.Name;
            ResolutionScale = config.ResolutionScale;
            EnableShadows = config.EnableShadows;
            shadowQuality = config.shadowQuality;
            ShadowDistance = config.ShadowDistance;
            textureQuality = config.textureQuality;

            PostProcessing = config.PostProcessing;
            Bloom = config.Bloom;
            Vignette = config.Vignette;
            DepthOfField = config.DepthOfField;
            FXAA = config.FXAA;
            AO = config.AO;
            HighQualityFXAA = config.HighQualityFXAA;
            SoftShadows = config.SoftShadows;
        }

    }

    [System.Serializable]
    public class SavebleGraphicsConfig
    {
        public string Name;
        [Range(0.25f, 3f)]
        public float ResolutionScale;
        public bool EnableShadows;
        public ShadowQuality shadowQuality;
        [Range(10f, 200f)]
        public float ShadowDistance;
        public TextureQuality textureQuality;

        public bool PostProcessing;
        public bool Bloom;
        public bool Vignette;
        public bool DepthOfField;
        public bool FXAA;
        public bool HighQualityFXAA;
        public bool SoftShadows;
        public bool AO;

        public SavebleGraphicsConfig(GraphicsConfiguration config)
        {
            Name = config.Name;
            ResolutionScale = config.ResolutionScale;
            EnableShadows = config.EnableShadows;
            shadowQuality = config.shadowQuality;
            ShadowDistance = config.ShadowDistance;
            textureQuality = config.textureQuality;

            PostProcessing = config.PostProcessing;
            Bloom = config.Bloom;
            Vignette = config.Vignette;
            DepthOfField = config.DepthOfField;
            FXAA = config.FXAA;
            HighQualityFXAA = config.HighQualityFXAA;
            SoftShadows = config.SoftShadows;
            AO = config.AO;
        }


    }
}
