using UnityEngine;
using UnityEngine.UI;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;
    private Slider _musicSlider;
    private Slider _soundsSlider;
    private float _musicVolume = 0.3f;
    private float _volumeScaleFactor = 1;

    public float VolumeScaleFactor => _volumeScaleFactor;

    public void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadSettings()
    {
        _musicSlider = GameObject.Find("Settings").transform.GetChild(1).GetComponentInChildren<Slider>();
        _musicSlider.value = _musicVolume;
        _musicSlider.onValueChanged.AddListener(delegate{UpdateMusicSetting();});
        GameObject.Find("Main Camera").GetComponent<AudioSource>().volume = _musicVolume;

        _soundsSlider = GameObject.Find("Settings").transform.GetChild(3).GetComponentInChildren<Slider>();
        _soundsSlider.value = _volumeScaleFactor;
        _soundsSlider.onValueChanged.AddListener(delegate{UpdateSoundsSetting();});
    }

    public void UpdateMusicSetting()
    {
        _musicVolume = _musicSlider.value;
    }

    public void UpdateSoundsSetting()
    {
        _volumeScaleFactor = _soundsSlider.value;
    }
}
