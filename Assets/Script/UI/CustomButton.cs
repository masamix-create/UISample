using System;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Script.UI
{
    /// <summary>
    /// カスタムボタンクラス。ボタンのクリック、押下、リリースイベントをObservableとして提供します。
    /// また、ボタンはアクティブかどうかをフラグで管理しており、状態の変化もObservableとして提供します。
    /// </summary>
    public class CustomButton : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        /// <summary>
        /// ボタンのアクティブ状態を保持するReactiveProperty
        /// </summary>
        public IReadOnlyReactiveProperty<bool> IsActive => _isActiveReactiveProperty;

        private readonly ReactiveProperty<bool> _isActiveReactiveProperty = new(true);

        /// <summary>
        /// ボタンがクリックされたときのイベントをObservableとして公開
        /// </summary>
        public IObservable<PointerEventData> OnClickAsObservable => _onClickSubject.AsObservable();

        /// <summary>
        /// ボタンが押されたときのイベントをObservableとして公開
        /// </summary>
        public IObservable<PointerEventData> OnDownAsObservable => _onPointerDownSubject.AsObservable();

        /// <summary>
        /// ボタンが放されたときのイベントをObservableとして公開
        /// </summary>
        public IObservable<PointerEventData> OnUpAsObservable => _onPointerUpSubject.AsObservable();

        private readonly Subject<PointerEventData> _onClickSubject = new Subject<PointerEventData>();
        private readonly Subject<PointerEventData> _onPointerDownSubject = new Subject<PointerEventData>();
        private readonly Subject<PointerEventData> _onPointerUpSubject = new Subject<PointerEventData>();

        /// <summary>
        /// ボタンがクリックされたときに呼び出されるメソッド
        /// </summary>
        /// <param name="eventData">クリックイベントのデータ</param>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (IsActive.Value)
            {
                _onClickSubject.OnNext(eventData);
            }
        }

        /// <summary>
        /// ボタンが押されたときに呼び出されるメソッド
        /// </summary>
        /// <param name="eventData">押下イベントのデータ</param>
        public void OnPointerDown(PointerEventData eventData)
        {
            if (IsActive.Value)
            {
                _onPointerDownSubject.OnNext(eventData);
            }
        }

        /// <summary>
        /// ボタンが放されたときに呼び出されるメソッド
        /// </summary>
        /// <param name="eventData">放したイベントのデータ</param>
        public void OnPointerUp(PointerEventData eventData)
        {
            if (IsActive.Value)
            {
                _onPointerUpSubject.OnNext(eventData);
            }
        }

        private void OnDestroy()
        {
            _onClickSubject.OnCompleted();
            _onPointerDownSubject.OnCompleted();
            _onPointerUpSubject.OnCompleted();
            
            _isActiveReactiveProperty.Dispose();
        }

        /// <summary>
        /// ボタンのアクティブ状態を変更するメソッド
        /// </summary>
        /// <param name="isActive">新しいアクティブ状態</param>
        public void SetActive(bool isActive)
        {
            _isActiveReactiveProperty.Value = isActive;
        }
    }
}