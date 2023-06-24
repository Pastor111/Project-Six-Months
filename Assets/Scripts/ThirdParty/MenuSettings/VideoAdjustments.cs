using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MainMenuUtils 
{
    [System.Serializable]
    public class VideoSettingsSave
    {
        public int Width;
        public int Height;
        public bool FullScreen;
        public bool Vsync;
        public int TargetFPS;
    }

    public class VideoAdjustments : MonoBehaviour
    {
        public VideoSettingsSave settings;

        // Start is called before the first frame update
        void Start()
        {
            Resize();
        }

        // Update is called once per frame
        void Update()
        {

            if (settings.Vsync)
                QualitySettings.vSyncCount = 1;
            else
                QualitySettings.vSyncCount = 0;

            Application.targetFrameRate = settings.TargetFPS;
        }

        public void Resize()
        {
            Screen.SetResolution(settings.Width, settings.Height, settings.FullScreen);

        }
    }
}
