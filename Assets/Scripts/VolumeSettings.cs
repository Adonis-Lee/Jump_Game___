using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// MainMenu'deki Options panelinde ses düzeyini kontrol eder.
/// AudioListener.volume'ü ayarlar ve PlayerPrefs ile kaydeder.
/// </summary>
public class VolumeSettings : MonoBehaviour
{
    [Header("UI Referansları")]
    public Slider volumeSlider;
    public TextMeshProUGUI volumeLabel;

    private const string VOLUME_KEY = "MasterVolume";

    void Start()
    {
        // Kaydedilmiş ses değerini yükle (varsayılan: 1.0 = %100)
        float savedVolume = PlayerPrefs.GetFloat(VOLUME_KEY, 1f);

        // Slider'ı ayarla (OnValueChanged tetiklenmeden)
        volumeSlider.SetValueWithoutNotify(savedVolume);

        // Gerçek ses seviyesini uygula
        AudioListener.volume = savedVolume;

        // Etiketi güncelle
        UpdateLabel(savedVolume);
    }

    /// <summary>
    /// Slider'ın OnValueChanged eventi buraya bağlanır.
    /// </summary>
    public void SetVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat(VOLUME_KEY, value);
        PlayerPrefs.Save();
        UpdateLabel(value);
    }

    private void UpdateLabel(float value)
    {
        if (volumeLabel != null)
            volumeLabel.text = Mathf.RoundToInt(value * 100f) + "%";
    }
}
