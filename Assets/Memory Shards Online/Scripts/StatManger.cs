using UnityEngine;
using UnityEngine.UI;

public class StatManger : MonoBehaviour
{
  public static StatManger instance;
  void Awake()
  {
    instance = this;
   
  } 


    public StatsBar[] statsBars;
    public Button statsButton;  

}
