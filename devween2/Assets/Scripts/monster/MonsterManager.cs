/*
 * Created by Hugo Uchoas Borges <hugouchoas@outlook.com>
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using util;

using Random = System.Random;

namespace monster
{
    public class MonsterManager : MonoBehaviour
    {
        private const int MONSTERS_COUNT = 3;
        private MonsterSO[] _monstersSO;

        [SerializeField] private string _monstersPath;
        [SerializeField] private GameObject monsterPrefab;
        [SerializeField] private Transform monstersHolder;
        private CanvasGroup _monstersHolderCG;

        [Header("Monster Instances")]
        [Space]
        public Monster[] activeMonsters;

        //[Header("Round Info")]
        public Action onNextRound = null;
        public Action onGameOver = null;

        private void Awake()
        {
            _monstersHolderCG = monstersHolder.GetComponent<CanvasGroup>();
        }

        public void Init()
        {
            _monstersSO = Resources.LoadAll<MonsterSO>(_monstersPath);

            activeMonsters = new Monster[MONSTERS_COUNT];
            for (int i = 0; i < activeMonsters.Length; i++)
            {
                var idx = i;
                activeMonsters[idx] = Instantiate(monsterPrefab).GetComponent<Monster>();
                activeMonsters[idx].transform.SetParent(monstersHolder);
                activeMonsters[idx].onPointerDown.AddListener(() => SelectMonster(activeMonsters[idx]));
            }
        }


        // ----------------------------------------------------------------------------------
        // ========================== Round Logic ============================
        // ----------------------------------------------------------------------------------

        public void PrepareRound()
        {
            ClearAllSelected();

            Random random = new Random();

            // List copy from all monsterSO
            List<MonsterSO> monstersChoose = new List<MonsterSO>(_monstersSO);

            // Choosing a random Monster from the available ScriptableObjects
            int idx = random.Next(monstersChoose.Count);
            MonsterSO chosenMonster = monstersChoose[idx];

            // Remove the chosen monster from the Copy list
            monstersChoose.Remove(chosenMonster);

            // Select one of the activeMonster to be the correct one
            int correctIdx = random.Next(activeMonsters.Length);
            activeMonsters[correctIdx].correct = true;
            activeMonsters[correctIdx].SetMonster(chosenMonster);

            // Set random monsters for the other activeMonsters
            foreach (var activeMonster in activeMonsters)
            {
                if (activeMonster.correct) continue;
                activeMonster.SetMonster(monstersChoose[random.Next(monstersChoose.Count)]);
            }

            SetMonstersAlpha(1);
        }

        public void SetMonstersAlpha(float alpha)
        {
            _monstersHolderCG.alpha = alpha;
        }

        public void StartRound()
        {
            SetTouchActive(true);
        }

        public void SetTouchActive(bool active)
        {
            foreach (var activeMonster in activeMonsters)
                activeMonster.touchable = active;
        }

        private void SelectMonster(Monster selectedMonster)
        {
            SetTouchActive(false);

            if (selectedMonster.correct)
            {
                GameDebug.Log("CORRECT MONSTER!!!", util.LogType.Round);
                onNextRound?.Invoke();
            }
            else
            {
                GameDebug.Log("INCORRECT MONSTER!!!", util.LogType.Round);
                onGameOver?.Invoke();
            }
        }

        private void ClearAllSelected()
        {
            foreach (var activeMonster in activeMonsters)
                activeMonster.correct = false;
        }

        public Monster GetSelectedMonster()
        {
            foreach (var activeMonster in activeMonsters)
                if (activeMonster.correct) return activeMonster;
            return null;
        }
    }
}