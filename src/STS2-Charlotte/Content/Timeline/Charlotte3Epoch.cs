using System;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.Timeline;
using CharlotteMod.Content.Relics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Timeline.Scaffolding;

namespace CharlotteMod.Content.Timeline;

/// <summary>
/// 第三章·独家爆料(遗物纪元,Navia2Epoch/Defect3Epoch 同款):解锁 特殊分析变焦镜头/镜头盖/采访稿记录本。
/// 揭示条件=用夏洛蒂累计击败 3 个首领([UnlockEpochAfterBossVictories(3)] 挂 Charlotte 类);
/// 池过滤见 CharlotteRelicPool.GetUnlockedRelics。
/// 基类 RelicUnlockEpochTemplate 自动提供 UnlockText/QueueUnlocks。
/// 大图使用纪元 ID 推导的全局路径，缩略图显式覆盖 Mod 资源槽。
/// </summary>
[RegisterEpoch]
[RegisterStoryEpoch(typeof(CharlotteStory))]
[AutoTimelineSlot(EpochEra.Flourish3)]
public sealed class Charlotte3Epoch : RelicUnlockEpochTemplate
{
    /// <summary>解锁物类型(单一来源):模板 QueueUnlocks/UnlockText 与池门控共用。</summary>
    public static readonly Type[] RelicUnlockTypes =
    {
        typeof(SpecialAnalysisZoomLens), typeof(LensCap), typeof(InterviewNotebook),
    };

    public override string Id => "STS2_CHARLOTTE_EPOCH_3";

    public override string StoryId => "Charlotte";

    public override EpochAssetProfile AssetProfile => new(
        PackedPortraitPath: "res://STS2-Charlotte/images/timeline/sts2_charlotte_epoch_3_thumb.png");

    protected override IEnumerable<Type> RelicTypes => RelicUnlockTypes;
}
