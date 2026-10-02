using System;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.Timeline;
using CharlotteMod.Content.Potions;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Timeline.Scaffolding;

namespace CharlotteMod.Content.Timeline;

/// <summary>
/// 第四章·独家专访(药水纪元,Navia3Epoch/Regent4Epoch 同款):解锁 香浓的热咖啡/镜头清洁剂/提神醒脑茶。
/// 揭示条件=用夏洛蒂通关进阶 1([UnlockEpochAfterAscensionOneWin] 挂 Charlotte 类,vanilla 第七章同型);
/// 池过滤见 CharlottePotionPool.GetUnlockedPotions。
/// 基类 PotionUnlockEpochTemplate 自动提供 UnlockText/QueueUnlocks。
/// 世界线立绘待 Mirror 交付,AssetProfile 暂不覆盖。
/// </summary>
[RegisterEpoch]
[RegisterStoryEpoch(typeof(CharlotteStory))]
[AutoTimelineSlot(EpochEra.Invitation5)]
public sealed class Charlotte4Epoch : PotionUnlockEpochTemplate
{
    /// <summary>解锁物类型(单一来源):模板 QueueUnlocks/UnlockText 与池门控共用。</summary>
    public static readonly Type[] PotionUnlockTypes =
    {
        typeof(HotCoffee), typeof(LensCleaner), typeof(RefreshingTea),
    };

    public override string Id => "STS2_CHARLOTTE_EPOCH_4";

    public override string StoryId => "Charlotte";

    protected override IEnumerable<Type> PotionTypes => PotionUnlockTypes;
}
