using UnityEngine;

public class ChangeLoginRegister : MonoBehaviour
{
    public GameObject loginPanel;
    public GameObject registerPanel;
    
    public void ActivateLoginPanel()
    {
        loginPanel.SetActive(true);
        registerPanel.SetActive(false);
    }

    public void ActivateRegisterPanel()
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(true);
    }
}
