using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Script.UI.CardCell
{
    public class CardCellView : MonoBehaviour
    {
        [SerializeField] private Image unitImage;
        [SerializeField] private TextMeshProUGUI realtyTextMeshPro;
        [SerializeField] private TextMeshProUGUI levelTextMeshPro;

        private Sprite _loadImage;

        public async void SetUnitId(int unitId)
        {
            var imagePath = unitId switch
            {
                0 => "Assets/AddressableAssets/CardImage/boys_01.png",
                1 => "Assets/AddressableAssets/CardImage/boys_02.png",
                2 => "Assets/AddressableAssets/CardImage/boys_03.png",
                3 => "Assets/AddressableAssets/CardImage/boys_04.png",
                4 => "Assets/AddressableAssets/CardImage/boys_05.png",
                5 => "Assets/AddressableAssets/CardImage/girls_01.png",
                6 => "Assets/AddressableAssets/CardImage/girls_02.png",
                7 => "Assets/AddressableAssets/CardImage/girls_03.png",
                8 => "Assets/AddressableAssets/CardImage/girls_04.png",
                9 => "Assets/AddressableAssets/CardImage/girls_05.png",
                _ => ""
            };
            if (_loadImage == null)
            {
                Destroy(_loadImage);
            }
            // アセットの非同期読み込み
            _loadImage = await LoadAssetAsync(imagePath);
            unitImage.sprite = _loadImage;
        }
        
        async UniTask<Sprite>LoadAssetAsync(string address)
        {
            // AddressablesのLoadAssetAsyncメソッドをUniTaskに変換して実行
            AsyncOperationHandle<Sprite> handle = Addressables.LoadAssetAsync<Sprite>(address);
        
            // 非同期処理が完了するまで待機
            await handle.ToUniTask();
        
            // 読み込みが成功した場合は結果を返す
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                return handle.Result;
            }

            // 読み込みが失敗した場合はnullを返す
            return null;
        }

        public void SetRealty(int realty)
        {
            var text = realty switch
            {
                0 => "C",
                1 => "R",
                2 => "SR",
                3 => "UR",
                _ => ""
            };
            realtyTextMeshPro.SetText( text);
        }
    
        public void SetLevel(int level)
        {
            levelTextMeshPro.SetText( "Lv:" + level);
        }
        
    }
    
    
    
}