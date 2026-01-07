using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Suriyun.MCS.Example
{
    public class SceneControl : MonoBehaviour
    {

        public void ResetScene(string sceneName) {
            SceneManager.LoadScene(sceneName);
        }

        public void OpenAssetStore() {
            Application.OpenURL("https://assetstore.unity.com/packages/templates/systems/mobile-controller-system-161533?aid=1100lGeN&pubref=indemo");
        }
    }
}