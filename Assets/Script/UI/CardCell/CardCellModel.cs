using System;
using UniRx;
namespace Script.UI.CardCell
{
    public class CardCellModel : IDisposable
    {
        private ReactiveProperty<int> _unitId = new ReactiveProperty<int>();
        /// <summary>
        /// ユニットの種類を表す
        /// </summary>
        public IReadOnlyReactiveProperty<int> UnitId => _unitId;
        
        private ReactiveProperty<int> _level = new ReactiveProperty<int>();
        /// <summary>
        /// ユニットのレベル
        /// </summary>
        public IReadOnlyReactiveProperty<int> Level => _level;
        
        private ReactiveProperty<int> _reality = new ReactiveProperty<int>();
        /// <summary>
        /// ユニットのレアリティ
        /// </summary>
        public IReadOnlyReactiveProperty<int> Reality => _reality;
        
        public CardCellModel( int unitId, int level, int reality)
        {
            _unitId.Value = unitId;
            _level.Value = level;
            _reality.Value = reality;
        }

        public void Dispose()
        {
            _unitId.Dispose();
            _level.Dispose();
            _reality.Dispose();
        }
    }
}
