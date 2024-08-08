using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Script.Util
{
    /// <summary>
    /// Addressableアセットを管理し、アセットグループのダウンロード、ロード、およびキャッシュのクリア機能を提供するクラス。
    /// IDisposableを実装して、リソースが適切に解放されるようにする。
    /// このクラスを返してロードしたアセットはこのクラスのDisposeで解放を行います。ただしLoadSceneは活用時にシーンを跨ぐことが想定されるため解放は含めていません。
    /// Singleでの読み込みの場合は考慮不要ですが、マルチシーンの活用の場合は自分で管理してください。
    /// </summary>
    public class AddressableManager : IDisposable
    {
        /// <summary>
        /// コンストラクタ
        /// 扱うAddressableのグループごとにインスタンスを作成して利用する想定。
        /// </summary>
        /// <param name="targetGroup">このクラスで扱うアドレッサブルのグループ</param>
        public AddressableManager(string targetGroup)
        {
            TargetGroup = targetGroup;
            _handles = new List<AsyncOperationHandle>();
        }
        /// <summary>
        /// このクラスで扱うアドレッサブルのグループ
        /// </summary>
        public string TargetGroup { get; }
        private readonly List<AsyncOperationHandle> _handles;
        private readonly Subject<float> _downloadProgressSubject = new Subject<float>();
        private readonly Subject<Unit> _downloadCompleteSubject = new Subject<Unit>();
        private readonly Subject<Exception> _downloadErrorSubject = new Subject<Exception>();

        /// <summary>
        /// ダウンロード処理中の進捗を報告するイベント。
        /// </summary>
        public IObservable<float> OnDownloadProgress => _downloadProgressSubject;

        /// <summary>
        /// ダウンロード処理が正常に完了したときにトリガーされるイベント。
        /// </summary>
        public IObservable<Unit> OnDownloadComplete => _downloadCompleteSubject;

        /// <summary>
        /// ダウンロード処理中にエラーが発生したときにトリガーされるイベント。
        /// </summary>
        public IObservable<Exception> OnDownloadError => _downloadErrorSubject;

        /// <summary>
        /// 指定されたAddressableグループラベルのダウンロードサイズを取得する。
        /// </summary>
        /// <param name="groupLabel">確認するAddressableグループのラベル。</param>
        /// <returns>ダウンロードサイズ（バイト単位）を表すUniTask。</returns>
        public async UniTask<long> GetDownloadSizeAsync(string groupLabel)
        {
            var sizeHandle = Addressables.GetDownloadSizeAsync(groupLabel);
            long downloadSize = await sizeHandle.ToUniTask();
            return downloadSize;
        }

        /// <summary>
        /// 指定されたAddressableグループラベルの依存関係をダウンロードする。
        /// 進捗を報告し、完了およびエラーのイベントをトリガーする。
        /// </summary>
        /// <returns>非同期操作を管理するためのUniTaskVoid。</returns>
        public async UniTaskVoid DownloadGroupAsync()
        {
            try
            {
                var downloadHandle = Addressables.DownloadDependenciesAsync(TargetGroup);
                _handles.Add(downloadHandle);

                while (!downloadHandle.IsDone)
                {
                    float progress = downloadHandle.PercentComplete;
                    _downloadProgressSubject.OnNext(progress);
                    await UniTask.Yield();
                }

                if (downloadHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    _downloadCompleteSubject.OnNext(Unit.Default);
                }
                else
                {
                    throw new Exception("Failed to download updated assets.");
                }
            }
            catch (Exception ex)
            {
                _downloadErrorSubject.OnNext(ex);
            }
        }

        /// <summary>
        /// 指定されたAddressableグループラベルのキャッシュをクリアする。
        /// Addressables.ClearDependencyCacheAsyncをラップしているだけ。
        /// </summary>
        public void ClearCache()
        {
            // Asyncと名前がついているのに戻り値がvoidになっている。
            Addressables.ClearDependencyCacheAsync(TargetGroup);
        }

        /// <summary>
        /// アセットを非同期にロードし、成功とエラー処理のコールバックを提供する。
        /// </summary>
        /// <typeparam name="T">ロードするアセットの型。</typeparam>
        /// <param name="assetAddress">ロードするアセットのアドレス。</param>
        /// <param name="onSuccess">アセットが正常にロードされたときに呼び出されるコールバック。</param>
        /// <param name="onError">ロード中にエラーが発生したときに呼び出されるコールバック。</param>
        /// <returns>非同期操作を表すUniTask。</returns>
        public async UniTask LoadAssetAsync<T>(string assetAddress, Action<T> onSuccess, Action<Exception> onError)
        {
            try
            {
                var handle = Addressables.LoadAssetAsync<T>(assetAddress);
                _handles.Add(handle);
                T asset = await handle.ToUniTask();

                if (handle.Status != AsyncOperationStatus.Succeeded)
                {
                    throw new Exception($"Failed to load asset at address: {assetAddress}");
                }

                onSuccess?.Invoke(asset);
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex);
            }
        }

        /// <summary>
        /// シーンを非同期にロードし、成功とエラー処理のコールバックを提供する。
        /// シーン遷移後に管理しているオブジェクトが破棄される場合に備え、ハンドルは管理リストに追加しない。
        /// </summary>
        /// <param name="sceneAddress">ロードするシーンのアドレス。</param>
        /// <param name="loadMode">シーンのロードモード。</param>
        /// <param name="onSuccess">シーンが正常にロードされたときに呼び出されるコールバック。</param>
        /// <param name="onError">ロード中にエラーが発生したときに呼び出されるコールバック。</param>
        /// <returns>非同期操作を表すUniTask。</returns>
        public async UniTask LoadSceneAsync(string sceneAddress, LoadSceneMode loadMode, Action<SceneInstance> onSuccess, Action<Exception> onError)
        {
            try
            {
                var handle = Addressables.LoadSceneAsync(sceneAddress, loadMode);
                SceneInstance sceneInstance = await handle.ToUniTask();

                if (handle.Status != AsyncOperationStatus.Succeeded)
                {
                    throw new Exception($"Failed to load scene at address: {sceneAddress}");
                }

                onSuccess?.Invoke(sceneInstance);
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex);
            }
        }

        /// <summary>
        /// このAddressableManagerインスタンスが保持するすべてのリソースを解放する。
        /// </summary>
        public void Dispose()
        {
            foreach (var handle in _handles)
            {
                Addressables.Release(handle);
            }
            _handles.Clear();
            _downloadProgressSubject.Dispose();
            _downloadCompleteSubject.Dispose();
            _downloadErrorSubject.Dispose();
        }
    }
}