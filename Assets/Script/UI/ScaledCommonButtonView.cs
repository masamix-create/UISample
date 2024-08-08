using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Script.UI
{
    [RequireComponent(typeof(CustomButton))]
    public class ScaledCommonButtonView : MonoBehaviour
    {
        private const float DefaultScale = 1f;
        private const float PressedScale = 0.9f;
        
        private const float ActiveImageAlpha = 1f;
        private const float InactiveImageAlpha = 0.5f;
        
        private CustomButton _button;
        [SerializeField] private Image image;

        private void Start()
        {
            _button = GetComponent<CustomButton>();
            
            _button.OnPointerDownAsObservable
                .Subscribe(_ => SetScale(PressedScale))
                .AddTo(this.gameObject);

            _button.OnPointerUpAsObservable
                .Subscribe(_ => SetScale(DefaultScale))
                .AddTo(this.gameObject);
            
            _button.IsActiveRP
                .Subscribe(SetButtonActive)
                .AddTo(this.gameObject);
        }

        private void SetScale(float scale)
        {
            image.rectTransform.localScale = Vector3.one * scale;
        }

        private void SetButtonActive(bool isActive)
        {
            float alpha = isActive ? ActiveImageAlpha : InactiveImageAlpha;
            image.color = new Color(1, 1, 1, alpha);
        }
    }
}
