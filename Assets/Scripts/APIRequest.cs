using System;

namespace Abandoned.Models
{
    /// <summary>
    /// ログインリクエスト
    /// </summary>
    [Serializable]
    public class LoginRequest
    {
        public string device_id;
    }

    /// <summary>
    /// 放置開始リクエスト
    /// </summary>
    [Serializable]
    public class IdleStartRequest
    {
        public int user_id;
    }

    /// <summary>
    /// 放置終了リクエスト
    /// </summary>
    [Serializable]
    public class IdleFinishRequest
    {
        public int user_id;
    }

    /// <summary>
    /// レベルアップリクエスト
    /// </summary>
    [Serializable]
    public class LevelUpRequest
    {
        public int user_id;
    }

    /// <summary>
    /// 能力強化リクエスト
    /// </summary>
    [Serializable]
    public class UpgradeRequest
    {
        public int user_id;
        public string upgrade_type; // "attack", "speed", "hp_regain"
    }

    /// <summary>
    /// 進化リクエスト
    /// </summary>
    [Serializable]
    public class EvolveRequest
    {
        public int user_id;
    }

    /// <summary>
    /// 能力強化タイプ
    /// </summary>
    public enum UpgradeType
    {
        Attack,
        Speed,
        HPRegain
    }
}