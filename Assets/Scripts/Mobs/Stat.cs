using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperDungeons
{
    public class Stat
    {
        public enum Type
        {
            // Increases the damage dealt with strength-based weapons and skills, as well as the carry weight and the ability to move heavier objects. This is also a prerequisite for Heavy armors that focus more on physical armor than magical armor.
            // Skills: Athletics
            Strength,

            // Increases damage dealt with finesse-based weapons and skills, and certain amounts of it are necessary for higher grade Leather armors that have balanced defenses.
            // Skills: Acrobatics
            Dexterity,

            // Increases damage dealt with intelligence-based weapons and skills. Also a prerequisite for Magical armors.
            Intelligence,

            // Determines the amount of health the character has.
            Constitution,

            // Determines the ability to detect traps and find hidden treasures (perception and insight)
            // also Critical Chance, Initiative?
            Wits,

            // Personality, Deception, Persuasion
            // Charisma

            // Determines the number of or strength of godly effects
            // Skills: Religion
            // Attunement
        }
    }
}
