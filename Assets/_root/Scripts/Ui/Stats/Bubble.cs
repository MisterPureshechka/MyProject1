using UnityEngine;
using UnityEngine.UI;

public class Bubble : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private Sprite _fullSprite;
    [SerializeField] private Sprite _halfSprite;

    public void SetSpriteHalf(bool isHalf)
    {
        _image.sprite = isHalf ? _halfSprite : _fullSprite;
    }
    
    public void SetEmpty()
    {
        _image.enabled = false;              
    }
}