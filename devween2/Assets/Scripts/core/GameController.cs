/*
 * Created by Hugo Uchoas Borges <hugouchoas@outlook.com>
 */

using System.Collections.Generic;
using UnityEngine;

namespace core
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private monster.MonsterPoolController mMonsterPoolController;

        /// <summary>
        /// First call in the ENTIRE game
        /// </summary>
        private void Start()
        {

        }
    }
}