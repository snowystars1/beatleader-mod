﻿namespace BeatLeader.Utils {
    internal static class BLConstants {
        #region HTTP Status codes

        public const int MaintenanceStatus = 503;
        public const int OutdatedModStatus = 418;

        #endregion

        #region Basic links

        public const string REPLAY_UPLOAD_URL = "https://api.beatleader.xyz/replay";

        public const string BEATLEADER_API_URL = "https://api.beatleader.xyz";
        
        public const string BEATLEADER_WEBSITE_URL = "https://www.beatleader.xyz";

        #endregion

        #region Authentication
        
        public const string SIGNIN_WITH_TICKET = //  /signin
            BEATLEADER_API_URL + "/signin?ticket={0}";

        #endregion

        #region ExMachina
        
        public const string EX_MACHINA_API_URL = "https://bs-replays-ai.azurewebsites.net";

        public const string EX_MACHINA_BASIC = //  json/{hash}/{diffId}/basic
            EX_MACHINA_API_URL + "/json/{0}/{1}/basic";

        #endregion

        #region Leaderboard

        public const string MASS_LEADERBOARDS = // /leaderboards?page={pageIndex}&count={itemsPerPage}&type={nominated/qualified/ranked}
            BEATLEADER_API_URL + "/leaderboards?page={0}&count={1}&type={2}";

        public const string LEADERBOARDS_HASH = // /leaderboards/hash/{hash}
            BEATLEADER_API_URL + "/leaderboards/hash/{0}";

        public static string LeaderboardPage(string leaderboardId) {
            return $"{BEATLEADER_WEBSITE_URL}/leaderboard/global/{leaderboardId}";
        }
        
        #endregion

        #region Voting

        public const string VOTE = // /vote/{hash}/{diff}/{mode}?rankability={rankability}&stars={stars}&type={type}
            BEATLEADER_API_URL + "/vote/{0}/{1}/{2}?{3}";

        public const string VOTE_STATUS = // /votestatus/{hash}/{diff}/{mode}?player={playerId}
            BEATLEADER_API_URL + "/votestatus/{0}/{1}/{2}?player={3}";

        #endregion

        #region Notifications

        public const string LATEST_RELEASES = BEATLEADER_API_URL + "/mod/lastVersions";

        public static string PlaylistLink(string name) {
            return $"{BEATLEADER_API_URL}/playlist/{name}";
        }

        #endregion

        #region Leaderboard requests

        public const string SCORES_BY_HASH_PAGED = // /v3/scores/{hash}/{diff}/{mode}/{context}/{scope}/page?player={playerId}&page={page}&count={count}
            BEATLEADER_API_URL + "/v3/scores/{0}/{1}/{2}/{3}/{4}/page?{5}";

        public const string SCORES_BY_HASH_SEEK = // /v3/scores/{hash}/{diff}/{mode}/{context}/{scope}/around?player={playerId}&count={count}
            BEATLEADER_API_URL + "/v3/scores/{0}/{1}/{2}/{3}/{4}/around?{5}";

        public const int SCORE_PAGE_SIZE = 10;

        #endregion

        #region Profile
        
        public const string PROFILE_BY_ID = // /player/{user_id}
            BEATLEADER_API_URL + "/player/{0}";

        #endregion

        #region OculusPC

        public const string OCULUS_USER_INFO = // /oculususer?token={user_id}
            BEATLEADER_API_URL + "/oculususer?token={0}";

        public const string OCULUS_PC_SIGNIN = // /signin?action=oculuspc&token={user_id}
            BEATLEADER_WEBSITE_URL + "/signin/oculuspc?token={0}";

        #endregion

        #region Score stats

        public const string SCORE_STATS_BY_ID = // score/statistic/{scoreId}
            BEATLEADER_API_URL + "/score/statistic/{0}";

        #endregion

        #region Modifiers

        public const string MODIFIERS_URL = BEATLEADER_API_URL + "/modifiers";

        #endregion

        internal class Param {
            public static readonly string PLAYER = "player";
            public static readonly string PAGE = "page";
            public static readonly string COUNT = "count";
        }
    }
}
