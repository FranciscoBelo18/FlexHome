using UnityEngine;

public class ChangeLoginRegister : MonoBehaviour
{
    public GameObject loginPanel;
    public GameObject registerPanel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
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
