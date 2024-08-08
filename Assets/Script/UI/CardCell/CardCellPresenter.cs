using System;
using UniRx;
using UnityEngine;

namespace Script.UI.CardCell
{
    public class presenter : MonoBehaviour
    {
        [SerializeField] private CardCellView cardCellView;

        private CardCellModel _cardCellModel;

        private void Awake()
        {
            _cardCellModel = new CardCellModel(1, 1, 1);
            _cardCellModel.UnitId.Subscribe(value => { cardCellView?.SetUnitId(value); }
            ).AddTo(gameObject);
            _cardCellModel.Reality.Subscribe(value => { cardCellView?.SetRealty(value); }
            ).AddTo(gameObject);
            _cardCellModel.Level.Subscribe(value => { cardCellView?.SetLevel(value); }
            ).AddTo(gameObject);

            cardCellView?.SetUnitId(1);
            cardCellView?.SetRealty(1);
            cardCellView?.SetLevel(1);
        }

        private void OnDestroy()
        {
            _cardCellModel.Dispose();
        }

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}