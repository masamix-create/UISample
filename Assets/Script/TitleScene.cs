using System;
using Script.UI;
using Script.Util;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script
{
    public class TitleScene : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        private AddressableManager _addressableManager;
        private const String AddressableGroup = "default";

        private CustomButton _nextButton;
        private async void Start()
        {
            _addressableManager = new AddressableManager(AddressableGroup);
            await _addressableManager.LoadAssetAsync<GameObject>("Assets/Prefabs/NextButton.prefab", obj =>
                {
                    Debug.Log($"Scene {obj} loaded successfully.");
                    // ロード成功時の処理
                    _nextButton = Instantiate(obj, canvas.transform).GetComponent<CustomButton>();
                    
                    //取得したボタン押下時に次のシーン遷移を設定
                    _nextButton.OnClickAsObservable
                        .Subscribe(_ => { LoadScene(); })
                        .AddTo(this.gameObject);
                },
                error =>
                {
                    // ロード失敗時の処理
                    Debug.LogError($"Error loading scene: {error.Message}");
                });
        }

        async void LoadScene()
        {
            await _addressableManager.LoadSceneAsync("Assets/Scenes/MainScene.unity", LoadSceneMode.Single,
                sceneInstance =>
                {
                    // ロード成功時の処理
                    Debug.Log($"Scene {sceneInstance.Scene.name} loaded successfully.");
                },
                error =>
                {
                    // ロード失敗時の処理
                    Debug.LogError($"Error loading scene: {error.Message}");
                });
        }

        private void OnDestroy()
        {
            _addressableManager.Dispose();
        }
    }
}