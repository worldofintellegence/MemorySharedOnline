using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Suriyun.MCS.Example
{
    public class SceneNavigator : MonoBehaviour
    {

        public void ChangeScene(string sceneName) {
            SceneManager.LoadScene(sceneName);
        }
    }
}