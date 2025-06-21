using TMPro;
using UnityEngine;

public class Puzzle2UIManager : MonoBehaviour
{
    public static Puzzle2UIManager Instance;

    [SerializeField] private TMP_Text collectedTextA;
    [SerializeField] private TMP_Text collectedTextB;
    [SerializeField] private string labelA = "Objeto A";
    [SerializeField] private string labelB = "Objeto B";

    private int collectedA = 0, totalA = 0;
    private int collectedB = 0, totalB = 0;

    private void Awake() => Instance = this;

    public void SetTotalRequiredA(int total) { totalA = total; UpdateA(); }
    public void SetCollectedA(int value) { collectedA = value; UpdateA(); }
    public void SetTotalRequiredB(int total) { totalB = total; UpdateB(); }
    public void SetCollectedB(int value) { collectedB = value; UpdateB(); }

    private void UpdateA()
    {
        if (collectedTextA != null)
            collectedTextA.text = $"{labelA}: {collectedA} / {totalA}";
    }

    private void UpdateB()
    {
        if (collectedTextB != null)
            collectedTextB.text = $"{labelB}: {collectedB} / {totalB}";
    }
}
