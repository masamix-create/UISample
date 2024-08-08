using System;
using Cysharp.Threading.Tasks;
using Script.UI;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Script
{
    public class MainScene : MonoBehaviour
    {
        [SerializeField] private CustomButton dialogButton;
        [SerializeField] private CustomButton listButton;
        [SerializeField] private GameObject dialogPrefabObj;
        [SerializeField] private GameObject canvas;

        // Start is called before the first frame update
        private void Start()
        {
            dialogButton.OnClickAsObservable
                .Subscribe(_ =>
                {
                    GameObject dialog = Instantiate(dialogPrefabObj, Vector3.zero, Quaternion.identity);
                    dialog.GetComponent<AnimatedDialog>()?.OnClosed.Subscribe(d =>
                    {
                        Debug.Log(d);
                        Destroy(d.gameObject);
                    });
                    dialog.transform.SetParent(canvas.transform, false);
                })
                .AddTo(this.gameObject);
        }
        
        
        // シーンをロードして遷移するメソッド
        public async UniTaskVoid LoadSceneAsync(string nextScene)
        {
            try
            {
                // シーンのロード
                var handle = Addressables.LoadSceneAsync(nextScene);
            
                // ロードが完了するまで待機
                await handle.Task;
            
                // 必要に応じて追加の処理を実行
                Debug.Log("シーンロード完了");
            }
            catch (Exception e)
            {
                // エラーログを出力
                Debug.LogError($"シーンのロード中にエラーが発生しました: {e.Message}");
            }
        }
        
    }
}