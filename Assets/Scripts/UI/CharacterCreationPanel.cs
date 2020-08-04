using System.Collections.Generic;
using UnityEngine;

namespace PaperDungeons
{
    public class CharacterCreationPanel : MonoBehaviour
    {
        [Header("Child components")]

        [SerializeField]
        private Transform _statsListTransform = null;

        [SerializeField]
        private TMPro.TextMeshProUGUI _unspentStatPointText = null;


        [Header("Asset references")]

        [SerializeField]
        private CharacterCreationStatRow _statsRowPrefab = null;

        private Dictionary<Stat.Type, CharacterCreationStatRow> statRows = new Dictionary<Stat.Type, CharacterCreationStatRow>();

        private void Awake()
        {
            // TODO: initialise this somewhere else
            LocalCharacterStats.stats = new MobStats();
            LocalCharacterStats.stats.Init();

            foreach (Stat.Type statType in System.Enum.GetValues(typeof(Stat.Type)))
            {
                CharacterCreationStatRow statRow = Instantiate(_statsRowPrefab, _statsListTransform);
                statRow.Init(statType);
                statRow.OnPlusClick += HandlePlusClick;
                statRow.OnMinusClick += HandleMinusClick;

                statRows.Add(statType, statRow);
            }
        }

        private void Start()
        {
            RefreshView();
        }

        private void HandlePlusClick(Stat.Type statType)
        {
            statRows[statType].StatValue ++;

            RefreshView();
        }

        private void HandleMinusClick(Stat.Type statType)
        {
            statRows[statType].StatValue --;

            RefreshView();
        }

        private void RefreshView()
        {
            _unspentStatPointText.text = $"You have {LocalCharacterStats.UnspentStatPoints} unspent stat points available.";

            foreach (CharacterCreationStatRow statRow in statRows.Values)
            {
                statRow.RefreshView();
            }
        }
    }
}
