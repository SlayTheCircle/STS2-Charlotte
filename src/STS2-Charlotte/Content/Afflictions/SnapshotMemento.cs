using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;

namespace CharlotteMod.Content.Afflictions;

/// <summary>
/// [留影纪念]标记:被附上此标记的卡是[留影]生成的克隆——效果/类型/费用与原牌完全相同,
/// 带有消耗。留影系卡牌与遗物以 <see cref="Mechanics.Snapshot.IsMemento"/> 为唯一判据,
/// 不要用「克隆 + 消耗」之类的组合条件判断(原版 Terraforming 类克隆会误伤)。
/// 文本在 localization/{lang}/afflictions.json:extraCardText 追加显示在被标记卡的描述下方。
/// </summary>
[RegisterAffliction]
public sealed class SnapshotMemento : AfflictionModel
{
    public override bool HasExtraCardText => true;
}
