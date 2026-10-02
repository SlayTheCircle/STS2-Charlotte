using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using CharlotteMod.Content.CardPools;

namespace CharlotteMod.Content.Cards;

/// <summary>
/// 鞭辟入里(普通,0 费技能):去除敌人所有的格挡与人工制品,并给予 1 层虚弱。消耗。
/// 升级:2 层虚弱。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class IncisiveAnalysis : CharlotteCardBase
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => new HashSet<CardKeyword> { CardKeyword.Exhaust };

    // 文案三要素悬停(vanilla Expose 同款,2026-10-03 审阅 #58;ExtraHoverTips 在 ModCardTemplate 已封死,走 AdditionalHoverTips)。
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<VulnerablePower>(),
        HoverTipFactory.FromPower<ArtifactPower>(),
        HoverTipFactory.Static(StaticHoverTip.Block),
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new PowerVar<WeakPower>(1m) };

    public IncisiveAnalysis()
        : base(0, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target is not Creature target)
        {
            return;
        }
        if (target.Block > 0)
        {
            // 走 CreatureCmd 保留 AfterBlockBroken 钩子与碎甲音效(原版 Expose 同款;
            // LoseBlockInternal 是引擎内部原语,2026-10-03 审阅 #22)。
            // 双目标签名差异:0.111.0 带 ctx/remover,0.107.1 只有 (creature, amount)。
#if MOD_GAME_0107_1
            await CreatureCmd.LoseBlock(target, target.Block);
#else
            await CreatureCmd.LoseBlock(choiceContext, target, target.Block, base.Owner.Creature);
#endif
        }
        if (target.GetPower<ArtifactPower>() is { } artifact)
        {
            await PowerCmd.Remove(artifact);
        }
        await PowerCmd.Apply<WeakPower>(choiceContext, target, base.DynamicVars["WeakPower"].IntValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["WeakPower"].UpgradeValueBy(1m);
    }
}
