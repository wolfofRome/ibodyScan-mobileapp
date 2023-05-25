namespace FitAndShape
{
    public enum PostureCondition
    {
        /// <summary>
        /// 正常.
        /// </summary>
        Normal = 0,

        /// <summary>
        /// 前傾（頭部）.
        /// </summary>
        ForwardTiltHead = 1,

        /// <summary>
        /// 後傾（頭部）.
        /// </summary>
        BackwardTiltHead = 2,

        /// <summary>
        /// 前傾（膝）.
        /// </summary>
        ForwardTiltKnee = 3,

        /// <summary>
        /// 後傾（膝）.
        /// </summary>
        BackwardTiltKnee = 4,

        /// <summary>
        /// 前傾（骨盤）.
        /// </summary>
        ForwardTiltPelvis = 5,

        /// <summary>
        /// 後傾（骨盤）.
        /// </summary>
        BackwardTiltPelvis = 6,

        /// <summary>
        /// 猫背（胸椎後弯）.
        /// </summary>
        Stoop = 7,

        /// <summary>
        /// 平背.
        /// </summary>
        FlatBack = 8,

        /// <summary>
        /// 腰椎平坦.
        /// </summary>
        LumbarFlat = 9,

        /// <summary>
        /// おなか突出.
        /// </summary>
        StomachProtruding = 10,

        /// <summary>
        /// ストレートネック.
        /// </summary>
        StraightNeck = 11,

        /// <summary>
        /// のけぞり（頸部後屈）.
        /// </summary>
        NeckBackFlexion = 12,

        /// <summary>
        /// 左傾き（体）.
        /// </summary>
        LeftTiltBody = 13,

        /// <summary>
        /// 右傾き（体）.
        /// </summary>
        RightTiltBody = 14,

        /// <summary>
        /// 左寛骨前傾 or 右寛骨後傾 or 両方.
        /// </summary>
        LeftThighShort = 15,

        /// <summary>
        /// 右寛骨前傾 or 左寛骨後傾 or 両方.
        /// </summary>
        RightThighShort = 16,

        /// <summary>
        /// 膝下長（左が短い）.
        /// </summary>
        LeftLowerLegLengthShort = 17,

        /// <summary>
        /// 膝下長（右が短い）.
        /// </summary>
        RightLowerLegLengthShort = 18,

        /// <summary>
        /// 肩峰左傾.
        /// </summary>
        LeftTiltAcromion = 19,

        /// <summary>
        /// 肩峰右傾.
        /// </summary>
        RightTiltAcromion = 20,

        /// <summary>
        /// 骨盤左傾.
        /// </summary>
        LeftTiltPelvis = 21,

        /// <summary>
        /// 骨盤右傾.
        /// </summary>
        RightTiltPelvis = 22,

        /// <summary>
        /// X脚.
        /// </summary>
        KnockKnees = 23,

        /// <summary>
        /// O脚.
        /// </summary>
        BowLegs = 24,

        /// <summary>
        /// 左ねじれ（肩）.
        /// </summary>
        LeftTwistShoulder = 25,

        /// <summary>
        /// 右ねじれ（肩）.
        /// </summary>
        RightTwistShoulder = 26,

        /// <summary>
        /// 左ねじれ（首）.
        /// </summary>
        LeftTwistNeck = 27,

        /// <summary>
        /// 右ねじれ（首）.
        /// </summary>
        RightTwistNeck = 28,

        /// <summary>
        /// 左ねじれ（腰）.
        /// </summary>
        LeftTwistWaist = 29,

        /// <summary>
        /// 右ねじれ（腰）.
        /// </summary>
        RightTwistWaist = 30,

        /// <summary>
        /// 右膝が前に出ている.
        /// </summary>
        RightKneeProtruding = 31,

        /// <summary>
        /// 左膝が前に出ている.
        /// </summary>
        LeftKneeProtruding = 32,
    }
}
