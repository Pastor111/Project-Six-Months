using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using ShadowResolution = UnityEngine.Rendering.Universal.ShadowResolution;

namespace MainMenuUtils
{
    public static class UnityGraphicsBullshit
    {
        private static FieldInfo MainLightCastShadows_FieldInfo;
        private static FieldInfo AdditionalLightCastShadows_FieldInfo;
        private static FieldInfo MainLightShadowmapResolution_FieldInfo;
        private static FieldInfo AdditionalLightShadowmapResolution_FieldInfo;
        private static FieldInfo Cascade2Split_FieldInfo;
        private static FieldInfo Cascade4Split_FieldInfo;
        private static FieldInfo SoftShadowsEnabled_FieldInfo;

        public static void Init()
        {
            var pipelineAssetType = typeof(UniversalRenderPipelineAsset);
            var flags = BindingFlags.Instance | BindingFlags.NonPublic;

            MainLightCastShadows_FieldInfo = pipelineAssetType.GetField("m_MainLightShadowsSupported", flags);
            AdditionalLightCastShadows_FieldInfo = pipelineAssetType.GetField("m_AdditionalLightShadowsSupported", flags);
            MainLightShadowmapResolution_FieldInfo = pipelineAssetType.GetField("m_MainLightShadowmapResolution", flags);
            AdditionalLightShadowmapResolution_FieldInfo = pipelineAssetType.GetField("m_AdditionalLightsShadowmapResolution", flags);
            Cascade2Split_FieldInfo = pipelineAssetType.GetField("m_Cascade2Split", flags);
            Cascade4Split_FieldInfo = pipelineAssetType.GetField("m_Cascade4Split", flags);
            SoftShadowsEnabled_FieldInfo = pipelineAssetType.GetField("m_SoftShadowsSupported", flags);
        }


        public static bool MainLightCastShadows
        {
            get => (bool)MainLightCastShadows_FieldInfo.GetValue(UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline);
            set => MainLightCastShadows_FieldInfo.SetValue(UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline, value);
        }

        public static bool AdditionalLightCastShadows
        {
            get => (bool)AdditionalLightCastShadows_FieldInfo.GetValue(UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline);
            set => AdditionalLightCastShadows_FieldInfo.SetValue(UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline, value);
        }

        public static UnityEngine.Rendering.Universal.ShadowResolution MainLightShadowResolution
        {
            get => (ShadowResolution)MainLightShadowmapResolution_FieldInfo.GetValue(UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline);
            set => MainLightShadowmapResolution_FieldInfo.SetValue(UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline, value);
        }

        public static ShadowResolution AdditionalLightShadowResolution
        {
            get => (ShadowResolution)AdditionalLightShadowmapResolution_FieldInfo.GetValue(UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline);
            set => AdditionalLightShadowmapResolution_FieldInfo.SetValue(UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline, value);
        }

        public static float Cascade2Split
        {
            get => (float)Cascade2Split_FieldInfo.GetValue(UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline);
            set => Cascade2Split_FieldInfo.SetValue(UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline, value);
        }

        public static Vector3 Cascade4Split
        {
            get => (Vector3)Cascade4Split_FieldInfo.GetValue(UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline);
            set => Cascade4Split_FieldInfo.SetValue(UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline, value);
        }

        public static bool SoftShadowsEnabled
        {
            get => (bool)SoftShadowsEnabled_FieldInfo.GetValue(UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline);
            set => SoftShadowsEnabled_FieldInfo.SetValue(UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline, value);
        }
    }



    public class GraphicsAdjustMents : MonoBehaviour
    {

        public GraphicsConfiguration GC;
        public bool CheckForObjectsEveryFrame = true;

        public ScriptableRendererFeature AO;

        UniversalRenderPipelineAsset asset;

        Volume[] volumes;
        Camera[] cameras;

        // Start is called before the first frame update
        void Start()
        {
            Init();

            if (!CheckForObjectsEveryFrame)
            {
                volumes = GameObject.FindObjectsOfType<Volume>();
                cameras = GameObject.FindObjectsOfType<Camera>();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (CheckForObjectsEveryFrame)
            {
                volumes = GameObject.FindObjectsOfType<Volume>();
                cameras = GameObject.FindObjectsOfType<Camera>();
            }

            asset.renderScale = GC.ResolutionScale;
            UnityGraphicsBullshit.MainLightCastShadows = GC.EnableShadows;

            AO.SetActive(GC.AO);

            if (GC.shadowQuality == ShadowQuality.VeryLOW_256)
                UnityGraphicsBullshit.MainLightShadowResolution = ShadowResolution._256;
            if (GC.shadowQuality == ShadowQuality.LOW_512)
                UnityGraphicsBullshit.MainLightShadowResolution = ShadowResolution._512;
            if (GC.shadowQuality == ShadowQuality.MEDIUM_1024)
                UnityGraphicsBullshit.MainLightShadowResolution = ShadowResolution._1024;
            if (GC.shadowQuality == ShadowQuality.HIGH_2048)
                UnityGraphicsBullshit.MainLightShadowResolution = ShadowResolution._2048;
            if (GC.shadowQuality == ShadowQuality.VERYHIGH_4096)
                UnityGraphicsBullshit.MainLightShadowResolution = ShadowResolution._4096;

            if (GC.shadowQuality_additional_Lights == ShadowQuality.VeryLOW_256)
                UnityGraphicsBullshit.AdditionalLightShadowResolution = ShadowResolution._256;
            if (GC.shadowQuality_additional_Lights == ShadowQuality.LOW_512)
                UnityGraphicsBullshit.AdditionalLightShadowResolution = ShadowResolution._512;
            if (GC.shadowQuality_additional_Lights == ShadowQuality.MEDIUM_1024)
                UnityGraphicsBullshit.AdditionalLightShadowResolution = ShadowResolution._1024;
            if (GC.shadowQuality_additional_Lights == ShadowQuality.HIGH_2048)
                UnityGraphicsBullshit.AdditionalLightShadowResolution = ShadowResolution._2048;
            if (GC.shadowQuality_additional_Lights == ShadowQuality.VERYHIGH_4096)
                UnityGraphicsBullshit.AdditionalLightShadowResolution = ShadowResolution._4096;

            asset.shadowDistance = GC.ShadowDistance;
            UnityGraphicsBullshit.SoftShadowsEnabled = GC.SoftShadows;

            if (GC.textureQuality == TextureQuality.EightRes)
                UnityEngine.QualitySettings.globalTextureMipmapLimit = 3;
            if (GC.textureQuality == TextureQuality.QuaterRes)
                UnityEngine.QualitySettings.globalTextureMipmapLimit = 2;
            if (GC.textureQuality == TextureQuality.HalfRes)
                UnityEngine.QualitySettings.globalTextureMipmapLimit = 1;
            if (GC.textureQuality == TextureQuality.FullRes)
                UnityEngine.QualitySettings.globalTextureMipmapLimit = 0;

            for (int i = 0; i < volumes.Length; i++)
            {
                if (volumes[i].profile != null)
                {
                    Vignette vignette;
                    Bloom bloom;
                    DepthOfField DOF;

                    if (volumes[i].profile.TryGet(out vignette))
                        vignette.active = GC.Vignette;
                    if (volumes[i].profile.TryGet(out bloom))
                        bloom.active = GC.Bloom;
                    if (volumes[i].profile.TryGet(out DOF))
                        DOF.active = GC.DepthOfField;
                }
            }

            for (int i = 0; i < cameras.Length; i++)
            {

                cameras[i].GetUniversalAdditionalCameraData().renderPostProcessing = GC.PostProcessing;

                if (GC.FXAA)
                    cameras[i].GetUniversalAdditionalCameraData().antialiasing = AntialiasingMode.FastApproximateAntialiasing;
                else
                    cameras[i].GetUniversalAdditionalCameraData().antialiasing = AntialiasingMode.None;

                if (GC.HighQualityFXAA)
                    cameras[i].GetUniversalAdditionalCameraData().antialiasingQuality = AntialiasingQuality.High;
                else
                    cameras[i].GetUniversalAdditionalCameraData().antialiasingQuality = AntialiasingQuality.Low;


            }
        }

        public void Init()
        {
            UnityGraphicsBullshit.Init();
            asset = (UniversalRenderPipelineAsset)UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline;

        }


    }
}
