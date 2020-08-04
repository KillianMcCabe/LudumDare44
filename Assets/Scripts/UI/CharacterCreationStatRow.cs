using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PaperDungeons
{
    public class CharacterCreationStatRow : MonoBehaviour
    {
        public event Action<Stat.Type> OnPlusClick;
        public event Action<Stat.Type> OnMinusClick;

        public int StatValue
        {
            get
            {
                return LocalCharacterStats.stats.GetStatValue(_statType);
            }
            set
            {
                LocalCharacterStats.stats.SetStatValue(_statType, value);
                int newStatValue = LocalCharacterStats.stats.GetStatValue(_statType);
                _valueText.text = newStatValue.ToString();
            }
        }

        [SerializeField]
        private TMPro.TextMeshProUGUI _typeText = null;

        [SerializeField]
        private TMPro.TextMeshProUGUI _valueText = null;

        [SerializeField]
        private Button _plusButton = null;

        [SerializeField]
        private Button _minusButton = null;

        private Stat.Type _statType;

        private void Awake()
        {
            _plusButton.onClick.AddListener(HandlePlusClick);
            _minusButton.onClick.AddListener(HandleMinusClick);
        }

        public void Init(Stat.Type statType)
        {
            _statType = statType;
            _typeText.text = _statType.ToString();
            _valueText.text = LocalCharacterStats.stats.GetStatValue(_statType).ToString();

            RefreshView();
        }

        public void RefreshView()
        {
            int newStatValue = LocalCharacterStats.stats.GetStatValue(_statType);
            _valueText.text = newStatValue.ToString();

            _minusButton.interactable = (newStatValue > MobStats.BaseStatPoints);
            _plusButton.interactable = (LocalCharacterStats.UnspentStatPoints > 0);
        }

        private void HandlePlusClick()
        {
            OnPlusClick?.Invoke(_statType);
        }

        private void HandleMinusClick()
        {
            OnMinusClick?.Invoke(_statType);
        }
    }
}
