using Common.Pool;
using Logging;
using UnityEngine;
using Zenject;

namespace ZooWorld.UI
{
    public class WorldSpaceFloatingTextPresenter : MonoBehaviour
    {
        [SerializeField] private RectTransform textsRoot;
        [SerializeField] private Canvas targetCanvas;
        [SerializeField] private FloatingTextView[] pooledViews;
        [SerializeField] private FloatingTextAnimationSettings animationSettings;

        private ILogsWriter logsWriter;
        private Pool<FloatingTextView> pool;

        [Inject]
        private void Construct(ILogsWriter logsWriter)
        {
            this.logsWriter = logsWriter;
        }

        private void Awake()
        {
            if (pooledViews == null || pooledViews.Length == 0)
            {
                pool = null;
                return;
            }

            pool = new Pool<FloatingTextView>(pooledViews);
        }

        public void ShowFloatingText(string text, Vector3 worldPosition, Camera worldCamera = null)
        {
            if (pool == null)
            {
                return;
            }

            var resolvedCamera = worldCamera != null ? worldCamera : Camera.main;
            if (resolvedCamera == null)
            {
                logsWriter.LogError("WorldSpaceFloatingTextPresenter requires a world camera.");
                return;
            }

            var screenPoint = resolvedCamera.WorldToScreenPoint(worldPosition);
            if (screenPoint.z < 0f)
            {
                return;
            }

            var uiCamera = targetCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : targetCanvas.worldCamera;

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(textsRoot, screenPoint, uiCamera, out var anchoredPosition))
            {
                return;
            }

            var view = pool.GetObject();
            if (!view)
            {
                return;
            }

            view.transform.SetParent(textsRoot, false);
            view.Play(text, anchoredPosition, animationSettings, ReturnToPool);
        }

        private void ReturnToPool(FloatingTextView view)
        {
            if (!view || pool == null)
            {
                return;
            }

            pool.ReturnObject(view);
        }
    }
}