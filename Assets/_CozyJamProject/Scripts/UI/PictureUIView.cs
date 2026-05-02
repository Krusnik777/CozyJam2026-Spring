using UnityEngine;
using UnityEngine.UI;

namespace CozySpringJam.UI
{
    public class PictureUIView : MovableRectView
    {
        [field: SerializeField] public Image PictureImage { get; private set; }
    }
}