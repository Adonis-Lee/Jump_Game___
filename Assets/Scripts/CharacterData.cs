using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacter", menuName = "Jump Game/Character Data")]
public class CharacterData : ScriptableObject
{
    public string characterName;
    public Sprite characterSprite;
    public Color characterColor = Color.white;

    [TextArea]
    public string description;

    [Header("Kilit Durumu")]
    public bool isUnlockedByDefault = false;
    public int unlockScore = 0; // Bu skora ulaşınca açılır (0 = baştan açık)
}
