using System;
using TMPro;
using UnityEngine;

public class ScreenLogging : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    public static ScreenLogging Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            _text.text = "Cleared\n";
        }
    }

    public void HandleLog(string sender, string log)
    {
        _text.text += $"{sender}\n";
        _text.text += $"{log}\n\n";
    }
}
