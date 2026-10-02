using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using CharlotteMod.Content.CardPools;
using CharlotteMod.Content.Keywords;
using CharlotteMod.Content.Powers;
using MegaCrit.Sts2.Core.HoverTips;

namespace CharlotteMod.Content.Cards;

/// <summary>
/// 谨遵事实(稀有,2 费能力):每当场上的[gold]聚焦[/gold]造成伤害时，你获得与那个伤害值相同的格挡。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class Adherence : CharlotteCardBase
{
    // 文案提及的效果词/机制动词悬停(悬浮扫描 2026-10-03;ModCardTemplate 封死 ExtraHoverTips,扩展点 AdditionalHoverTips)。
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        new IHoverTip[] { HoverTipFactory.Static(StaticHoverTip.Block) };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new PowerVar<AdherencePower>(1m) };

    public Adherence()
        : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
    }


    public override IEnumerable<CardKeyword> CanonicalKeywords
    {
        get
        {
            var set = new HashSet<CardKeyword>();
            CharlotteKeywords.AddTo(set, CharlotteKeywords.Focus);
            return set;
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<AdherencePower>(choiceContext, base.Owner.Creature, base.DynamicVars["AdherencePower"].IntValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }

}
