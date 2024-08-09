using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Script.UI
{
    [RequireComponent(typeof(CustomButton))]
    public class CommonButtonViewScale : MonoBehaviour
    {
        /// <summary>
        /// スケールを変更する対象のゲームオブジェクト。
        /// </summary>
        [SerializeField] private GameObject scaleTarget;
        
        /// <summary>
        /// ボタンが通常状態のときのスケール係数
        /// </summary>
        [SerializeField] private float defaultScaleFactor = 1.0f;
        
        /// <summary>
        /// ボタンが押下状態のときのスケール係数
        /// </summary>
        [SerializeField] private float pressedScaleFactor = 0.8f;
        
        /// <summary>
        /// アクティブ状態に応じてアルファ値を変更する対象の画像。
        /// </summary>
        [SerializeField] private Image activeTarget;
        
        /// <summary>
        /// ボタンがアクティブ状態のときの画像のアルファ値
        /// </summary>
        [SerializeField] private float activeImageAlpha = 1.0f;
        
        /// <summary>
        /// ボタンが非アクティブ状態のときの画像のアルファ値
        /// </summary>
        [SerializeField] private float inactiveImageAlpha = 0.5f;

        private void Start()
        {
            if (scaleTarget == null || activeTarget == null)
            {
                Debug.LogError("scaleTarget または activeTarget が設定されていません。");
                return;
            }

            var button = GetComponent<CustomButton>();
            button.OnDownAsObservable
                .Subscribe(_ => scaleTarget.transform.localScale = Vector3.one * pressedScaleFactor)
                .AddTo(this.gameObject);

            button.OnUpAsObservable
                .Subscribe(_ => scaleTarget.transform.localScale = Vector3.one * defaultScaleFactor)
                .AddTo(this.gameObject);
            
            button.IsActive
                .Subscribe(SetButtonActive)
                .AddTo(this.gameObject);
        }

        /// <summary>
        /// ボタンのアクティブ状態に応じて対象画像のアルファ値を設定します。
        /// </summary>
        /// <param name="isActive">ボタンがアクティブかどうかを示すフラグ。</param>
        private void SetButtonActive(bool isActive)
        {
            activeTarget.color = new Color(1, 1, 1, isActive ? activeImageAlpha : inactiveImageAlpha);
        }
    }
}