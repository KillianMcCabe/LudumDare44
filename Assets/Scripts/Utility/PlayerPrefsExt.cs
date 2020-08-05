using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace PaperDungeons
{
    public static class PlayerPrefsExt
    {
        public static string NickName
        {
            get
            {
                if (!PlayerPrefs.HasKey("NickName"))
                    return null;
                return PlayerPrefs.GetString("NickName");
            }
            set
            {
                PlayerPrefs.SetString("NickName", value);
            }
        }
    }
}