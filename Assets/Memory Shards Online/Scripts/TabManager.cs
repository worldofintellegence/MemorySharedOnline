using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TabManager : MonoBehaviour
{
    [System.Serializable]
    public struct Tab
    {
        public string name;
        public GameObject panel;
        public Button button;
        public bool isRestricted; // Agar true hai toh sirf private location mein khulega
    }

    public List<Tab> tabs;
    // public StrengthStats strengthStats; // Reference to our Strength child class
    public Color activeColor = Color.white;
    public Color inactiveColor = Color.gray;

    void Start()
    {
        // Shuru mein sab panels band kar dein, pehla tab chorr kar
        for (int i = 0; i < tabs.Count; i++)
        {
            int index = i; // Closure handle karne ke liye
            tabs[i].button.onClick.AddListener(() => OpenTab(index));
        }

        OpenTab(0); // Default first tab open
    }

    public void OpenTab(int index)
    {
        // // 1. Restriction Check (For Stats Tab)
        // if (tabs[index].isRestricted && strengthStats != null)
        // {
        //     if (!strengthStats.isInPrivateLocation)
        //     {
        //         Debug.Log("Cannot open Stats: You are not in a private location!");
        //         // Yahan aap koi UI popup ya message bhi dikha sakte hain
        //         return; 
        //     }
        // }

        // 2. Open/Close Logic
        for (int i = 0; i < tabs.Count; i++)
        {
            bool isActive = (i == index);
            tabs[i].panel.SetActive(isActive);
            
            // UI Button ka color change karein taake pata chale kaunsa tab active hai
            if (tabs[i].button.GetComponent<Image>() != null)
            {
                tabs[i].button.GetComponent<Image>().color = isActive ? activeColor : inactiveColor;
            }
        }
    }
}