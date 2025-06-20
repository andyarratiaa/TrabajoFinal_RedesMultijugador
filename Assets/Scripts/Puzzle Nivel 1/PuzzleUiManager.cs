using TMPro;
using UnityEngine;

public class PuzzleUIManager : MonoBehaviour
{
    public static PuzzleUIManager Instance;

    [SerializeField] private TMP_Text collectedText;

    private int totalRequired = 0;

    private void Awake()
    {
        Instance = this;
    }

    public void SetTotalRequired(int total)
    {
        totalRequired = total;
        UpdateText(0);
    }

    public void SetCollected(int count)
    {
        UpdateText(count);
    }

    private void UpdateText(int collected)
    {
        if (collectedText != null)
        {
            collectedText.text = $"Objetos recogidos: {collected} / {totalRequired}";
        }
    }
}


