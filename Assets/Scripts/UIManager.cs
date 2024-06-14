using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject SettingPannel;

    // Start is called before the first frame update
    void Start()
    {
        ResetPannels();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SelectSettingPannel()
    {
        // Toggle Controle
        if (SettingPannel != null)
        {
            SettingPannel.SetActive(!SettingPannel.activeSelf);
        }
    }
    public void ResetPannels()
    {
        SettingPannel.SetActive(false);
    }
}