using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterSelectUI : MonoBehaviour
{
    [Header("Veritabanı")]
    public CharacterDatabase characterDatabase;

    [Header("UI Elemanları")]
    public Image characterPreview;          // Karakterin büyük önizlemesi
    public TextMeshProUGUI characterNameText;
    public TextMeshProUGUI characterDescriptionText;
    public TextMeshProUGUI pageIndicatorText; // "2 / 5" gibi
    public Button selectButton;
    public TextMeshProUGUI selectButtonText;
    public Button leftArrowButton;
    public Button rightArrowButton;

    [Header("Kilit UI")]
    public GameObject lockOverlay;           // Kilitli karakterde gösterilecek kilit simgesi
    public TextMeshProUGUI unlockRequirementText; // "500 Skor Gerekli" gibi

    private int currentIndex = 0;
    private int selectedIndex = 0;

    void OnEnable()
    {
        selectedIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);
        currentIndex = selectedIndex;
        UpdateUI();
    }

    public void NextCharacter()
    {
        if (characterDatabase.characters.Length == 0) return;
        currentIndex = (currentIndex + 1) % characterDatabase.characters.Length;
        UpdateUI();
    }

    public void PreviousCharacter()
    {
        if (characterDatabase.characters.Length == 0) return;
        currentIndex--;
        if (currentIndex < 0)
            currentIndex = characterDatabase.characters.Length - 1;
        UpdateUI();
    }

    public void SelectCharacter()
    {
        if (!characterDatabase.IsCharacterUnlocked(currentIndex))
            return;

        selectedIndex = currentIndex;
        PlayerPrefs.SetInt("SelectedCharacter", selectedIndex);
        PlayerPrefs.Save();
        UpdateUI();
    }

    void UpdateUI()
    {
        if (characterDatabase.characters.Length == 0) return;

        CharacterData character = characterDatabase.characters[currentIndex];
        bool isUnlocked = characterDatabase.IsCharacterUnlocked(currentIndex);

        // Önizleme
        if (characterPreview != null)
        {
            if (character.characterSprite != null)
            {
                characterPreview.sprite = character.characterSprite;
                characterPreview.color = isUnlocked ? character.characterColor : new Color(0.3f, 0.3f, 0.3f, 1f);
            }
            else
            {
                characterPreview.color = isUnlocked ? character.characterColor : new Color(0.3f, 0.3f, 0.3f, 1f);
            }
        }

        // İsim
        if (characterNameText != null)
            characterNameText.text = character.characterName;

        // Açıklama
        if (characterDescriptionText != null)
            characterDescriptionText.text = isUnlocked ? character.description : "???";

        // Sayfa göstergesi
        if (pageIndicatorText != null)
            pageIndicatorText.text = $"{currentIndex + 1} / {characterDatabase.characters.Length}";

        // Kilit durumu
        if (lockOverlay != null)
            lockOverlay.SetActive(!isUnlocked);

        if (unlockRequirementText != null)
        {
            if (!isUnlocked)
            {
                unlockRequirementText.gameObject.SetActive(true);
                unlockRequirementText.text = $"{character.unlockScore} Score Required";
            }
            else
            {
                unlockRequirementText.gameObject.SetActive(false);
            }
        }

        // Seç butonu
        if (selectButton != null)
        {
            if (!isUnlocked)
            {
                selectButton.interactable = false;
                if (selectButtonText != null)
                    selectButtonText.text = "Locked";
            }
            else if (currentIndex == selectedIndex)
            {
                selectButton.interactable = false;
                if (selectButtonText != null)
                    selectButtonText.text = "Selected";
            }
            else
            {
                selectButton.interactable = true;
                if (selectButtonText != null)
                    selectButtonText.text = "Select";
            }
        }

        // Ok butonları
        if (leftArrowButton != null)
            leftArrowButton.interactable = characterDatabase.characters.Length > 1;
        if (rightArrowButton != null)
            rightArrowButton.interactable = characterDatabase.characters.Length > 1;
    }
}
