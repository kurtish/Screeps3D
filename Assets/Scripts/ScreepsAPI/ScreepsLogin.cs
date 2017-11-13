﻿using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Screeps3D {
    public class ScreepsLogin : MonoBehaviour {
        [SerializeField] private ScreepsAPI api;
        [SerializeField] private Toggle save;
        [SerializeField] private Toggle ssl;
        [SerializeField] private TMP_InputField server;
        [SerializeField] private TMP_InputField port;
        [SerializeField] private TMP_InputField email;
        [SerializeField] private TMP_InputField password;
        [SerializeField] private Button connect;
        [SerializeField] public FadePanel panel;
        public Action<Credentials, Address> OnSubmit;
        public string secret = "abc123";

        private void Start() {
            connect.onClick.AddListener(OnClick);
            this.save.onValueChanged.AddListener(UpdateSaveSetting);
            RefreshSavedSettings();
            api.OnConnectionStatusChange += OnConnectionStatusChange;
            panel.Show(false, true);
            panel.Show(); // fade in
        }

        private void OnConnectionStatusChange(bool isConnected) {
            panel.Show(!isConnected);
        }

        private void RefreshSavedSettings() {
            var save = PlayerPrefs.GetInt("saveCredentials");
            Debug.Log($"save value: {save}");
            if (save == 1) {
                this.save.isOn = true;
                var port = PlayerPrefs.GetString("port");
                if (port != null) {
                    this.server.text = port;
                }
                var server = PlayerPrefs.GetString("server");
                if (server != null) {
                    this.server.text = server;
                }
                var email = PlayerPrefs.GetString("email");
                if (email != null) {
                    this.email.text = email;
                }
                var encryptedPassword = PlayerPrefs.GetString("password");
                var password = Crypto.DecryptStringAES(encryptedPassword, secret);
                this.password.text = password;
            }
        }

        private void UpdateSaveSetting(bool value) {
            PlayerPrefs.SetInt("saveCredentials", value ? 1 : 0);
            if (!value) {
                PlayerPrefs.SetString("email", "");
                PlayerPrefs.SetString("password", "");
            }
        }

        private void OnClick() {
            if (save.isOn) {
                PlayerPrefs.SetString("port", port.text);
                PlayerPrefs.SetString("server", server.text);
                PlayerPrefs.SetString("email", email.text);
                var password = this.password.text;
                var encryptedPassword = Crypto.EncryptStringAES(password, secret);
                PlayerPrefs.SetString("password", encryptedPassword);
            }
            
            var credentials = new Credentials {
                email = email.text, 
                password = password.text
            };
            var address = new Address {
                hostName = server.text, 
                path = "/", 
                ssl = this.ssl.isOn, 
                port = port.text
            };
            api.Connect(credentials, address);
        }
    }
	
    public struct Credentials {
        public string email;
        public string password;
    }
	
    public struct Address {
        public bool ssl;
        public string hostName;
        public string port;
        public string path;
	
        public string Http(string path = "") {
            if (path.StartsWith("/") && this.path.EndsWith("/")) {
                path = path.Substring(1);
            } 
            var protocol = ssl ? "https" : "http";
            var port = hostName.ToLowerInvariant() == "screeps.com" ? "" : $":{this.port}";
            Debug.Log($"{protocol}://{hostName}{port}{this.path}{path}");
            return $"{protocol}://{hostName}{port}{this.path}{path}";
        }
    }
}