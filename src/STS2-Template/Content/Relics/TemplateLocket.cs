using MegaCrit.Sts2.Core.Entities.Relics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using TemplateMod.Content.RelicPools;

namespace TemplateMod.Content.Relics;

/// <summary>
/// 示例坠饰(初始遗物):结构演示件——初始遗物挂 StartingRelics,无战斗钩子。
/// 补战斗效果时参考源工程遗物的钩子组合(回合开始 + AfterCombatEnd 重置)。
/// </summary>
[RegisterRelic(typeof(TemplateRelicPool))]
public sealed class TemplateLocket : TemplateRelicBase
{
    public override RelicRarity Rarity => RelicRarity.Common;
}
