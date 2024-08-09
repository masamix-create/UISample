using System;
using Script.Util;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script
{
    public class InitScene : MonoBehaviour
    {
        private AddressableManager _addressableManager;
        private const String AddressableGroup = "default";

        // Start is called before the first frame update
        async void Start()
        {
            //マネージャのインスタンス化　今回はデフォルトGroupのアセットを利用
            _addressableManager = new AddressableManager(AddressableGroup);

            //一度読み込むとダウンロード済みになるので、以下のコメントアウトをかいじょすると初期化され最初からダウンロードされる様になる。
            //_addressableManager.ClearCache(AddressableGroup);
        
            // ダウンロード進捗を購読
            _addressableManager.OnDownloadProgress
                .Subscribe(progress => Debug.Log($"Download progress: {progress * 100:F2}%"))
                .AddTo(this);

            // ダウンロード完了を購読
            _addressableManager.OnDownloadComplete
                .Subscribe(_ =>
                {
                    Debug.Log("Download complete");
                    //次のシーンへ
                    LoadScene();
                })
                .AddTo(this);

            // ダウンロードエラーを購読
            _addressableManager.OnDownloadError
                .Subscribe(ex => Debug.LogError($"Download error: {ex.Message}"))
                .AddTo(this);

            // 特定のグループの差分ダウンロードサイズを確認
            long downloadSize = await _addressableManager.GetDownloadSizeAsync(AddressableGroup);
            Debug.Log($"Download size: {downloadSize} bytes");
            // ダウンロード開始　（downloadSizeが０だと即座に終わるが、その後の遷移を統一させるためによぶ
            _addressableManager.DownloadGroupAsync().Forget();
        }

        async void LoadScene()
        {
            await _addressableManager.LoadSceneAsync("Assets/Scenes/TitleScene.unity", LoadSceneMode.Single,
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