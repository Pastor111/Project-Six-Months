using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MainMenuUtils;

public class OptionsMenu : MonoBehaviour
{
    public GraphicsAdjustMents currentGraphicsCongig;
    public VideoAdjustments VideoSettings;
    public SoundAdjustments soundAdjustments;

    public GraphicsConfiguration[] g_Settings;

    [Header("VIDEO")]
    public TMP_Dropdown Resolution_DropDown;
    public TMP_InputField TargetFPS_Input;
    public Slider TargetFPS_Slider;
    public Toggle Vsync;
    public Toggle FullScreen;
    [Header("Graphics")]
    public TMP_Dropdown GraphicsQuality_DropDown;
    public TMP_InputField ResolutionScale_Input;
    public Slider ResolutionScale_Slider;
    public Toggle EnableShadows;
    public TMP_Dropdown ShadowQuality_DropDown;
    public TMP_InputField ShadowDistance_Input;
    public Slider ShadowDistance_Slider;
    public TMP_Dropdown TextureQuality_DropDown;
    public Toggle EnablePP;
    public Toggle EnableAO;
    public Toggle EnableFXAA;
    [Header("Audio")]
    public TMP_InputField GeneralVolume_Input;
    public Slider GeneralVolume_Slider;
    public TMP_InputField MusicVolume_Input;
    public Slider MusicVolume_Slider;
    public TMP_InputField SFXVolume_Input;
    public Slider SFXVolume_Slider;
    [Header("GamePlay")]
    public TMP_InputField MouseSensitivity_Input;
    public Slider MouseSensitivity_Slider;
    public Toggle InvertX;
    public Toggle InvertY;

    // Start is called before the first frame update
    void Start()
    {
        SetUI();
        DontDestroyOnLoad(gameObject.transform.parent);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetUI()
    {
        #region Video

        Resolution_DropDown.ClearOptions();
        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            var res = Screen.resolutions[i];

            Resolution_DropDown.options.Add(new TMP_Dropdown.OptionData(res.ToString()));
        }

        SetDefaultVideoSettings();

        Resolution_DropDown.onValueChanged.AddListener((i) => { SetInt(out int x, i); VideoSettings.settings.Width = Screen.resolutions[x].width; VideoSettings.settings.Height = Screen.resolutions[x].height; VideoSettings.Resize(); });

        TargetFPS_Input.onEndEdit.AddListener((s) => { SetInt(out VideoSettings.settings.TargetFPS, int.Parse(s)); TargetFPS_Slider.value = int.Parse(s); });
        TargetFPS_Slider.onValueChanged.AddListener((s) => { SetInt(out VideoSettings.settings.TargetFPS, (int)s); TargetFPS_Input.text = s.ToString(); });

        TargetFPS_Slider.minValue = 24;
        TargetFPS_Slider.wholeNumbers = true;
        TargetFPS_Slider.maxValue = 240;

        Vsync.onValueChanged.AddListener((b) => { SetBool(out VideoSettings.settings.Vsync, b); });

        FullScreen.onValueChanged.AddListener((b) => { SetBool(out VideoSettings.settings.FullScreen, b); VideoSettings.Resize(); });

        SetDefaultVideoSettings();

        #endregion

        #region Graphics


        TextureQuality_DropDown.ClearOptions();

        TextureQuality_DropDown.options.Add(new TMP_Dropdown.OptionData("Very High"));
        TextureQuality_DropDown.options.Add(new TMP_Dropdown.OptionData("High"));
        TextureQuality_DropDown.options.Add(new TMP_Dropdown.OptionData("Medium"));
        TextureQuality_DropDown.options.Add(new TMP_Dropdown.OptionData("Low"));


        ShadowQuality_DropDown.ClearOptions();
        ShadowQuality_DropDown.options.Add(new TMP_Dropdown.OptionData("Very Low"));
        ShadowQuality_DropDown.options.Add(new TMP_Dropdown.OptionData("Low"));
        ShadowQuality_DropDown.options.Add(new TMP_Dropdown.OptionData("Medium"));
        ShadowQuality_DropDown.options.Add(new TMP_Dropdown.OptionData("High"));
        ShadowQuality_DropDown.options.Add(new TMP_Dropdown.OptionData("Very High"));




        GraphicsQuality_DropDown.ClearOptions();

        for (int i = 0; i < g_Settings.Length; i++)
        {
            GraphicsQuality_DropDown.options.Add(new TMP_Dropdown.OptionData(g_Settings[i].Name));
        }





        SetGraphicsDefaultValue();




        GraphicsQuality_DropDown.onValueChanged.AddListener((i) => { SetInt(out int x, i); currentGraphicsCongig.GC = g_Settings[x]; SetGraphicsDefaultValue(); });

        GraphicsQuality_DropDown.value = 1;
        GraphicsQuality_DropDown.value = 0;

        ResolutionScale_Input.onValueChanged.AddListener((s) => { SetFloat(out currentGraphicsCongig.GC.ResolutionScale, float.Parse(s)); ResolutionScale_Slider.value = float.Parse(s); });
        ResolutionScale_Slider.onValueChanged.AddListener((s) => { SetFloat(out currentGraphicsCongig.GC.ResolutionScale, s); ResolutionScale_Input.text = s.ToString(); });

        ResolutionScale_Slider.minValue = 0.25f;
        ResolutionScale_Slider.maxValue = 3f;
        ResolutionScale_Input.contentType = TMP_InputField.ContentType.DecimalNumber;

        EnableShadows.onValueChanged.AddListener((b) => { SetBool(out currentGraphicsCongig.GC.EnableShadows, b); });

        ShadowQuality_DropDown.onValueChanged.AddListener((i) => { SetInt(out int x, i); currentGraphicsCongig.GC.shadowQuality = (MainMenuUtils.ShadowQuality)x; });

        //ShadowDistance_Slider.onValueChanged.AddListener((s) => { SetFloat(out currentGraphicsCongig.GC.ShadowDistance, s); ShadowDistance_Input.text = s.ToString(); });
        //ShadowDistance_Input.onValueChanged.AddListener((s) => { SetFloat(out currentGraphicsCongig.GC.ShadowDistance, float.Parse(s)); ShadowDistance_Slider.value = float.Parse(s); });

        ShadowDistance_Slider.minValue = 10f;
        ShadowDistance_Slider.wholeNumbers = true;
        ShadowDistance_Slider.maxValue = 200f;

        ShadowDistance_Input.contentType = TMP_InputField.ContentType.IntegerNumber;


        TextureQuality_DropDown.onValueChanged.AddListener((i) => { SetInt(out int x, i); currentGraphicsCongig.GC.textureQuality = (MainMenuUtils.TextureQuality)x; });

        EnablePP.onValueChanged.AddListener((b) => { SetBool(out currentGraphicsCongig.GC.PostProcessing, b); });
        EnableFXAA.onValueChanged.AddListener((b) => { SetBool(out currentGraphicsCongig.GC.FXAA, b); });
        EnableAO.onValueChanged.AddListener((b) => { SetBool(out currentGraphicsCongig.GC.AO, b); });

        SetGraphicsDefaultValue();


        #endregion

        #region Audio

        SetDefaultAudio();

        GeneralVolume_Slider.onValueChanged.AddListener((f) => { SetFloat(out soundAdjustments.GeneralVolume, f); GeneralVolume_Input.text = f.ToString(); });
        GeneralVolume_Input.onValueChanged.AddListener((f) => { SetFloat(out soundAdjustments.GeneralVolume, float.Parse(f)); GeneralVolume_Slider.value = float.Parse(f); });

        MusicVolume_Slider.onValueChanged.AddListener((f) => { SetFloat(out soundAdjustments.AmbientSound, f); MusicVolume_Input.text = f.ToString(); });
        MusicVolume_Input.onValueChanged.AddListener((f) => { SetFloat(out soundAdjustments.AmbientSound, float.Parse(f)); MusicVolume_Slider.value = float.Parse(f); });

        SFXVolume_Slider.onValueChanged.AddListener((f) => { SetFloat(out soundAdjustments.VFXSounds, f); SFXVolume_Input.text = f.ToString(); });
        SFXVolume_Input.onValueChanged.AddListener((f) => { SetFloat(out soundAdjustments.VFXSounds, float.Parse(f)); SFXVolume_Slider.value = float.Parse(f); });

        #endregion

    }

    void SetGraphicsDefaultValue()
    {

                for (int i = 0; i < g_Settings.Length; i++)
        {
            if (g_Settings[i] == currentGraphicsCongig.GC)
            {
                GraphicsQuality_DropDown.value = i;
                break;
            }
        }

        ResolutionScale_Slider.value = currentGraphicsCongig.GC.ResolutionScale;
        ResolutionScale_Input.text = currentGraphicsCongig.GC.ResolutionScale.ToString();

        EnableShadows.isOn = currentGraphicsCongig.GC.EnableShadows;

        ShadowQuality_DropDown.value = (int)currentGraphicsCongig.GC.shadowQuality;

        ShadowDistance_Input.text = currentGraphicsCongig.GC.ShadowDistance.ToString();
        ShadowDistance_Slider.value = currentGraphicsCongig.GC.ShadowDistance;

        TextureQuality_DropDown.value = (int)currentGraphicsCongig.GC.textureQuality;

        EnableAO.isOn = currentGraphicsCongig.GC.AO;
        EnablePP.isOn = currentGraphicsCongig.GC.PostProcessing;
        EnableFXAA.isOn = currentGraphicsCongig.GC.FXAA;




    }

    void SetDefaultVideoSettings()
    {
        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            if(Screen.resolutions[i].width == VideoSettings.settings.Width && Screen.resolutions[i].height == VideoSettings.settings.Height)
            {
                Resolution_DropDown.value = i;
                break;
            }
        }

        TargetFPS_Slider.value = VideoSettings.settings.TargetFPS;
        TargetFPS_Input.text = VideoSettings.settings.TargetFPS.ToString();

        Vsync.isOn = VideoSettings.settings.Vsync;

        FullScreen.isOn = VideoSettings.settings.FullScreen;
    }

    void SetDefaultAudio()
    {
        GeneralVolume_Slider.value = soundAdjustments.GeneralVolume;
        GeneralVolume_Input.text = soundAdjustments.GeneralVolume.ToString();

        MusicVolume_Slider.value = soundAdjustments.AmbientSound;
        MusicVolume_Input.text = soundAdjustments.AmbientSound.ToString();

        SFXVolume_Slider.value = soundAdjustments.VFXSounds;
        SFXVolume_Input.text = soundAdjustments.VFXSounds.ToString();
    }

    public void SetGraphicsConfig(GraphicsConfiguration c)
    {
        currentGraphicsCongig.GC = c;
    }

    public void SetFloat(out float s, float value)
    {
        s = value;
    }

    public void SetBool(out bool s, bool value)
    {
        s = value;
    }

    public void SetInt(out int s, int value)
    {
        s = value;
    }

}
