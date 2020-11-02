/*
 * Created by Hugo Uchoas Borges <hugouchoas@outlook.com>
 */

using System;
using UnityEngine;

namespace monster
{
    [Serializable]
    [CreateAssetMenu(fileName = "Monster_0", menuName = "Monsters/New Monster")]
    public class MonsterSO : ScriptableObject
    {
        public string name;
        public Sprite sprite;
    }
}