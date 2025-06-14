using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinCodeUI : MonoBehaviour
{
    private static JoinCodeUI singleton;
    public static JoinCodeUI Singleton => singleton;

    [SerializeField] TMPro.TMP_Text joinCodeText;

    public TMPro.TMP_Text JoinCodeText => joinCodeText;

    private void Awake()
    {
        if(singleton == null)
        {
            singleton = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this);
    }
}