namespace FitAndShape
{
    /// <summary>
    /// 姿勢判定個所.
    /// ※詳細は以下のエクセルを参照.
    ///   (Dropbox)\姿勢機能\姿勢判定処理_170728.xlsx
    /// </summary>
    public enum PostureVerifyPoint
    {
        /// <summary>
        /// 重心Y軸ラインと耳の乖離.
        /// 姿勢の前後の傾斜.
        /// (左右アングル)
        /// </summary>
        UpperInclinationRatio = 0,

        /// <summary>
        /// 重心Y軸ラインとヒザの乖離.
        /// 姿勢の前後の傾斜.
        /// (左右アングル)
        /// </summary>
        LowerInclinationRatio = 1,

        /// <summary>
        /// 骨盤の前後傾斜角度の正常値.
        /// 骨盤の前後の傾斜.
        /// (左右アングル)
        /// </summary>
        PelvisInclinationAngle = 2,

        /// <summary>
        /// 背骨(胸椎)角度の正常値からの誤差.
        /// 後弯の度合い.
        /// (左右アングル)
        /// </summary>
        ThoracicVertebraAngle = 3,

        /// <summary>
        /// 背骨(腰椎上部)角度の正常値からの誤差.
        /// 前弯の度合い.
        /// (左右アングル)
        /// </summary>
        LumbarVertebraAngle = 4,

        /// <summary>
        /// 首の湾曲角度の正常値からの誤差.
        /// 首の湾曲度合い.
        /// (左右アングル)
        /// </summary>
        NeckCurveAngle = 5,

        /// <summary>
        /// 重心Y軸ラインと頭頂との乖離.
        /// 体全体の左右の傾斜.
        /// (前面アングル)
        /// </summary>
        BodyInclinationRatio = 6,

        /// <summary>
        /// 大腿骨大転子から膝頭までの両足間の差（右ー左）.
        /// 長さ.
        /// (前面アングル)
        /// </summary>
        UpperLegLengthRatio = 7,

        /// <summary>
        /// 膝頭から足首までの高さの両足間の差.
        /// 長さ.
        /// (前面アングル)
        /// </summary>
        LowerLegLengthRatio = 8,

        /// <summary>
        /// 肩峰の左右の高さの差（右ー左）.
        /// 肩の傾斜.
        /// (前面アングル)
        /// </summary>
        ShoulderInclinationRatio = 9,

        /// <summary>
        /// 左右の上前腸骨棘の高さの差（右ー左）.
        /// 骨盤の傾斜.
        /// (前面アングル)
        /// </summary>
        PelvisInclinationRatio = 10,

        /// <summary>
        /// ヒザ関節の外反角度（FTA）.
        /// O脚、X脚.
        /// (前面アングル)
        /// </summary>
        KneeJointAngle = 11,

        /// <summary>
        /// 左右の鎖骨角度の正常角度からの差.
        /// 両肩それぞれの前後のねじれ.
        /// (上アングル)
        /// </summary>
        ShoulderDistortionAngle = 12,

        /// <summary>
        /// 首の左右回転角度.
        /// 首の左右のねじれ.
        /// (上アングル)
        /// </summary>
        NeckDistortionAngle = 13,

        /// <summary>
        /// 腰の左右を結ぶ線と、肩の左右を結ぶ線の角度差（腰に対する肩）.
        /// 肩と腰のねじれ.
        /// (下アングル)
        /// </summary>
        UpperBodyDistortionAngle = 14,

        /// <summary>
        /// 膝頭の前後差（右ー左）.
        /// 膝の前後差.
        /// (下アングル)
        /// </summary>
        KneeAnteroposteriorDiffRatio = 15,
    }
}
