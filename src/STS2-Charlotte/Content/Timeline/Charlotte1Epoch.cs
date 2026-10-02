using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Screens.Timeline;
using MegaCrit.Sts2.Core.Timeline;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Timeline.Scaffolding;

namespace CharlotteMod.Content.Timeline;

/// <summary>
/// 第一章·新的报道(杂项纪元,Navia1Epoch/DarvEpoch 同款):夏洛蒂的冒险起点。
/// 角色**不锁**(组织惯例,同 Navia 2026-09-29 裁定),本章仅作剧情节点,
/// 「解锁夏洛蒂成为一名可玩角色」为时间线 unlockText 文案形态(epochs.json),非角色锁定开关;
/// 揭示条件=完成一局夏洛蒂(Charlotte 类上的 [UnlockEpochAfterRunAs])。
/// 基类须为 ModEpochTemplate:时间线槽位合并补丁按 `is ModEpochTemplate` 过滤,
/// 且 Era/EraPosition 由布局注册表解析([AutoTimelineSlot] 注册,勿手工 override——模板已密封)。
/// 世界线立绘待 Mirror 交付,AssetProfile 暂不覆盖(缩略图走原版图集回退显示 NOPE,不炸)。
/// </summary>
[RegisterEpoch]
[RegisterStoryEpoch(typeof(CharlotteStory))]
[AutoTimelineSlot(EpochEra.Blight2)]
public sealed class Charlotte1Epoch : ModEpochTemplate
{
    public override string Id => "STS2_CHARLOTTE_EPOCH_1";

    public override string StoryId => "Charlotte";

    public override void QueueUnlocks()
    {
        LocString locString = new LocString("epochs", Id + ".unlock");
        NTimelineScreen.Instance.QueueMiscUnlock(locString.GetFormattedText() ?? "");
    }
}
