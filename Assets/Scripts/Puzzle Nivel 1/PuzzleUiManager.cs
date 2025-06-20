using TMPro;
using UnityEngine;

public class PuzzleUIManager : MonoBehaviour
{
    public static PuzzleUIManager Instance;

    [SerializeField] private TMP_Text collectedText;

    private int currentCollected = 0;
    private int totalRequired = 0;

    private void Awake() => Instance = this;

    public void SetTotalRequired(int total)
    {
        totalRequired = total;
        UpdateText();
    }

    public void SetCollected(int count)
    {
        currentCollected = count;
        UpdateText();
    }

    private void UpdateText()
    {
        if (collectedText != null)
            collectedText.text = $"Objetos recogidos: {currentCollected} / {totalRequired}";
    }
}



