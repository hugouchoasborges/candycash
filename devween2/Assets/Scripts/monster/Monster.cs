/*
 * Created by Hugo Uchoas Borges <hugouchoas@outlook.com>
 */

using core;

namespace monster
{
    public class Monster : ClickableComponent
    {
        public MonsterSO monsterSO;
        public bool correct;

        public void SetMonster(MonsterSO monsterSO)
        {
            this.monsterSO = monsterSO;
            mImage.sprite = monsterSO.sprite;
            name = monsterSO.name;
        }
    }
}