using UnityEngine.UI;
using System.Net;

public class NetworkScreen : MonoBehaviourSingleton<NetworkScreen>
{
    //BUTTONS
    public Button connectButton;
    public Button startServerButton;

    //INPUT FIELDS
    public InputField portInputField;
    public InputField addressInputField;

    protected override void Initialize()
    {
        connectButton.onClick.AddListener(OnConnectBtnClick);
        
        startServerButton.onClick.AddListener(OnStartServerBtnClick);
    }

    void OnConnectBtnClick()
    {
        IPAddress ipAddress = IPAddress.Parse(addressInputField.text);
        
        int port = System.Convert.ToInt32(portInputField.text);

        NetworkManager.Instance.StartClient(ipAddress, port);
        
        SwitchToChatScreen();
    }

    void OnStartServerBtnClick()
    {
        int port = System.Convert.ToInt32(portInputField.text);
        
        NetworkManager.Instance.StartServer(port);
        
        SwitchToChatScreen();
    }

    void SwitchToChatScreen()
    {
        ChatScreen.Instance.gameObject.SetActive(true);
        
        this.gameObject.SetActive(false);
    }
}
