using UnityEngine;

[CreateAssetMenu(fileName = "CharacterDatabase", menuName = "Jump Game/Character Database")]
public class CharacterDatabase : ScriptableObject
{
    public CharacterData[] characters;

    public CharacterData GetSelectedCharacter()
    {
        int selectedIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);
        if (selectedIndex >= 0 && selectedIndex < characters.Length)
            return characters[selectedIndex];
        return characters.Length > 0 ? characters[0] : null;
    }

    public bool IsCharacterUnlocked(int index)
    {
        if (index < 0 || index >= characters.Length)
            return false;

        CharacterData character = characters[index];

        if (character.isUnlockedByDefault)
            return true;

        // Oyuncunun en yüksek skoru bu karakterin kilit skorunu geçtiyse açık
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        return highScore >= character.unlockScore;
    }
}
