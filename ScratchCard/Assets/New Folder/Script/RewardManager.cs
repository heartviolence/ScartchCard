using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RewardManager : MonoBehaviour
{

    public int TotalPolyChrome { get; private set; }
    public int TotalDenny { get; private set; }
    // Start is called before the first frame update

    [SerializeField]
    TMP_Text polyChromeText;

    [SerializeField]
    TMP_Text DennyText;

    public void AddPolyChrome(int count)
    {
        TotalPolyChrome += count;
        polyChromeText.text = TotalPolyChrome.ToString();
    }

    public void AddDenny(int count)
    {
        TotalDenny += count;
        DennyText.text = TotalDenny.ToString();
    }
}
