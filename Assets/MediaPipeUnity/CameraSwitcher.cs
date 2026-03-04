using UnityEngine;
using Mediapipe.Unity;
using System.Linq;

public class CameraSwitcher : MonoBehaviour
{
    // Inspector mein Solution ya Game Manager drag karein jisme WebCamSource hai
    public WebCamSource webCamSource; 
    private int currentDeviceIndex = 0;
    void Start() {
        
        
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length > 0) {
            Debug.Log("Camera found: " + devices[0].name);
        } else {
            Debug.LogError("No Camera found on this device!");
        }
    }
    public void ToggleCamera()
    {
        var devices = WebCamTexture.devices;
        if (devices.Length <= 1) return;

        // 1. Current stream ko stop karein
        webCamSource.Stop();

        // 2. Next device index par jayein
        currentDeviceIndex = (currentDeviceIndex + 1) % devices.Length;

        // 3. MediaPipe Source ko batayein ke kounsa index use karna hai
        // Homuler plugin mein hum direct index set kar sakte hain
        webCamSource.SelectSource(currentDeviceIndex);

        // 4. Restart karein
        webCamSource.Play();

        Debug.Log("Switched to camera: " + devices[currentDeviceIndex].name);
    }
}