/*
 * Created by Hugo Uchoas Borges <hugouchoas@outlook.com>
 */

namespace leaderboard
{
    public class LeaderboardPoolController : GenericPoolController<LeaderboardItem>
    {
        public override void DestroyItem(LeaderboardItem poolObj)
        {
            base.DestroyItem(poolObj);
            poolObj.onPointerDown.RemoveAllListeners();
        }
    }
}