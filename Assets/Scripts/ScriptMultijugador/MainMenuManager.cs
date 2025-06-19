using UnityEngine;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField playerNameInput;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OnNameEntered()
    {
        string name = playerNameInput.text;
        PlayerDataHandler.Instance.SetPlayerName(name);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
