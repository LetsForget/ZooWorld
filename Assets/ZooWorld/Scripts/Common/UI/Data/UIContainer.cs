using UnityEngine;

namespace ZooWorld.UI
{
    public class UIContainer : MonoBehaviour
    {
        [field: SerializeField] public Canvas MainCanvas { get; private set; }
        [field: SerializeField] public CanvasGroup OverlayGroup { get; private set; }
        [field: SerializeField] public CanvasGroup ScreenGroup { get; private set;}
        [field: SerializeField] public CanvasGroup WindowGroup { get; private set; }
        [field: SerializeField] public CanvasGroup PopupGroup { get; private set; }
    }
}