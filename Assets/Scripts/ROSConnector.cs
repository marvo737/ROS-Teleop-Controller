using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Robotics.ROSTCPConnector;

public class ROSConnector : MonoBehaviour
{
    [SerializeField] InputField IPAdressTextField;
    [SerializeField] InputField PortTextField;
    [SerializeField] GameObject ConnectionStatusRedObj;
    [SerializeField] GameObject ConnectionStatusGreenObj;
    [SerializeField] GameObject ConnectBtnObj;
    [SerializeField] GameObject DisconnectBtnObj;

    [System.NonSerialized] public bool _isRosConnect = false;

    private ROSConnection ros;

    private const string key_IP = "key_IP";
    private const string key_Port = "key_Port";

    // Start is called before the first frame update
    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();

        if (PlayerPrefs.HasKey(key_IP))
        {
            IPAdressTextField.text = PlayerPrefs.GetString(key_IP, "192.168.128.0");
        }

        if (PlayerPrefs.HasKey(key_Port))
        {
            PortTextField.text = PlayerPrefs.GetInt(key_Port, 10000).ToString();
        }
    }

    public void StartConnection()
    {
        ros.RosIPAddress = IPAdressTextField.text;
        ros.RosPort = int.Parse(PortTextField.text);

        PlayerPrefs.SetString(key_IP, ros.RosIPAddress);
        PlayerPrefs.SetInt(key_Port, ros.RosPort);

        TFSystem.GetOrCreateInstance();
        ros.Connect();

        _isRosConnect = true;
        ChangeUI(_isRosConnect);
    }

    public void EndOfConnection()
    {
        ros.Disconnect();

        _isRosConnect = false;
        ChangeUI(_isRosConnect);
    }

    private void ChangeUI(bool ConnectStatus)
    {
        ConnectionStatusRedObj.SetActive(!ConnectStatus);
        ConnectionStatusGreenObj.SetActive(ConnectStatus);

        ConnectBtnObj.SetActive(!ConnectStatus);
        DisconnectBtnObj.SetActive(ConnectStatus);
    }
}
