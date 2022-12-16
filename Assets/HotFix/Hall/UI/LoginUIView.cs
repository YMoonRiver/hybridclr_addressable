using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Wanderer.GameFramework
{
    public class LoginUIView : UGUIView
    {
        Button btnGuest;
        Button btnRegister;
        Button btnLogin;

        GameObject objRegister;
        GameObject objLogin;
        public override void OnInit(IUIContext uiContext)
        {
            base.OnInit(uiContext);
            Debug.Log("LoginUIView OnInit ###");

            btnGuest = transform.Find("Buttons/ButtonGuest").GetComponent<Button>();
            btnRegister = transform.Find("Buttons/ButtonRegister").GetComponent<Button>();
            btnLogin = transform.Find("Buttons/ButtonLogin").GetComponent<Button>();
            
            objRegister = transform.Find("Register").gameObject;
            objLogin = transform.Find("Login").gameObject;

            btnGuest.onClick.AddListener(OnGuest);
            btnRegister.onClick.AddListener(OnRegister);
            btnLogin.onClick.AddListener(OnLogin);

            transform.Find("Login/Button_Close").GetComponent<Button>().onClick.AddListener(() => {
                objLogin.SetActive(false);
            });
            transform.Find("Register/Button_Close").GetComponent<Button>().onClick.AddListener(() => {
                objRegister.SetActive(false);
            });
        }

        public override void OnFree(IUIContext uiContext)
        {
            base.OnFree(uiContext);
        }


        void OnGuest()
        {
            Debug.Log("OnGuest");
            GameMode.Event.Trigger(this, new LoginToHallEventArgs());
        }

        void OnRegister()
        {
            Debug.Log("OnRegister");
            objRegister.SetActive(true);
        }

        void OnLogin()
        {
            Debug.Log("OnLogin");
            objLogin.SetActive(true);
        }
    }
}