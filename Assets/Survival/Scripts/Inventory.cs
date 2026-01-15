using UnityEngine;

public class Inventory : MonoBehaviour
{
    public Animator anim;
    public CanvasGroup canvasGroup;
    public void OnClick()
    {
        anim.SetBool("isOpen", !anim.GetBool("isOpen"));
        // if (anim.GetBool("isOpen"))
        // {
        //    PlayerC
        // }
        // else
        // {
        //     canvasGroup.blocksRaycasts = false;
        // }
    }
}
