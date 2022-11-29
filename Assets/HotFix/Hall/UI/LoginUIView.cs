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
        public override void OnInit(IUIContext uiContext)
        {
            base.OnInit(uiContext);
            Debug.Log("LoginUIView OnInit ###");

            btnGuest = transform.Find("Buttons/ButtonGuest").GetComponent<Button>();
            btnRegister = transform.Find("Buttons/ButtonRegister").GetComponent<Button>();
            btnLogin = transform.Find("Buttons/ButtonLogin").GetComponent<Button>();

            btnGuest.onClick.AddListener(OnGuest);
            btnRegister.onClick.AddListener(OnRegister);
            btnLogin.onClick.AddListener(OnLogin);
        }

        public override void OnFree(IUIContext uiContext)
        {
            base.OnFree(uiContext);
        }


        void OnGuest()
        {
            Debug.Log("OnGuest");
        }

        void OnRegister()
        {
            Debug.Log("OnRegister");
        }

        void OnLogin()
        {
            Debug.Log("OnLogin");
            GameMode.Event.Trigger(this, new LoginToHallEventArgs());
        }
    }
}