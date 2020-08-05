using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace PaperDungeons
{
    public class CharacterCreationPanel : MonoBehaviour
    {
        [Header("Child components")]

        [SerializeField]
        private TMPro.TMP_InputField _nameTextInput = null;

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

            _nameTextInput.onEndEdit.AddListener(HandleNameChange);
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
            _nameTextInput.text = PhotonNetwork.NickName;

            if (LocalCharacterStats.UnspentStatPoints > 0)
                _unspentStatPointText.text = $"<color=red>You have {LocalCharacterStats.UnspentStatPoints} unspent stat points available.</color>";
            else
                _unspentStatPointText.text = "";

            foreach (CharacterCreationStatRow statRow in statRows.Values)
            {
                statRow.RefreshView();
            }
        }

        private void HandleNameChange(string newName)
        {
            PhotonNetwork.NickName = newName;
            PlayerPrefsExt.NickName = newName;
        }
    }
}
