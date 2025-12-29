using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class PlayButtonAnimation : MonoBehaviour
{
    private Button _button;
    private bool _isScaleBack;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }
    private void Start()
    {

        _button.onClick.AddListener(() => { PlayAnim(); });

    }

        void PlayAnim()
    {
        if (!_isScaleBack)
        {
            _isScaleBack = true;
            transform.localScale = Vector3.one;
            transform.DORewind();
            transform.transform.DOPunchScale(new Vector3(1, 1, 1) * .2f, .3f, 10, 90).OnComplete(() => { ShowComplete(); });
          
        }
    }

    void ShowComplete()
    {

        transform.localScale = Vector3.one;
        _isScaleBack = false;
    }
}
