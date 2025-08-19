using System;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Messenger
{
    public class SenderIconView : MonoBehaviour
    {
        [SerializeField] private Image _image;

        public void SetIcon(Sprite icon)
        {
            _image.sprite = icon;
        }
    }
}