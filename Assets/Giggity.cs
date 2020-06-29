using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Giggity : MonoBehaviour
{
    public static Giggity Instance { get; private set; }
    public Text ConnectionState { get; private set; }
    public Button FindMatch { get; private set; }

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this);
        }

        ConnectionState = GameObject.Find("ConnectionState").GetComponent<Text>();
        FindMatch = GameObject.Find("FindMatch").GetComponent<Button>();
    }
}
