using System;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.Timeline;
using CharlotteMod.Content.Cards;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Timeline.Scaffolding;

namespace CharlotteMod.Content.Timeline;

/// <summary>
/// 第二章·全新视角(卡牌纪元,Navia4Epoch/Ironclad2Epoch 同款):解锁 追踪调查/摄影形态/广角镜头。
/// 揭示条件=用夏洛蒂通关一次([UnlockEpochAfterWinAs] 挂 Charlotte 类);池过滤见 CharlotteCardPool.FilterThroughEpochs。
/// 基类 CardUnlockEpochTemplate 自动提供 UnlockText/QueueUnlocks(解锁展示文案读前三项,故每章恰好 3 件)。
/// 世界线立绘待 Mirror 交付,AssetProfile 暂不覆盖。
/// </summary>
[RegisterEpoch]
[RegisterStoryEpoch(typeof(CharlotteStory))]
[AutoTimelineSlot(EpochEra.Flourish2)]
public sealed class Charlotte2Epoch : CardUnlockEpochTemplate
{
    /// <summary>解锁物类型(单一来源):模板 QueueUnlocks/UnlockText 与池门控共用。</summary>
    public static readonly Type[] CardUnlockTypes =
    {
        typeof(TrackingInvestigation), typeof(PhotographyForm), typeof(WideAngleLens),
    };

    public override string Id => "STS2_CHARLOTTE_EPOCH_2";

    public override string StoryId => "Charlotte";

    protected override IEnumerable<Type> CardTypes => CardUnlockTypes;
}
