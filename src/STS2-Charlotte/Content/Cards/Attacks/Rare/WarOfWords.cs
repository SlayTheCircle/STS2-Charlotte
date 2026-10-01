using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using CharlotteMod.Content.CardPools;
using CharlotteMod.Content.Mechanics;

namespace CharlotteMod.Content.Cards;

/// <summary>
/// 唇枪舌剑(稀有,0 费攻击):固有。造成 3 点伤害。回合开始时,若此牌在消耗牌堆中,
/// 则在本场战斗中将此卡的伤害提升 2 点,然后将其返回你的手牌。升级:提升 3 点。
/// 回归逻辑经 IExileReturner 由 CombatTracker 在回合开始扫描触发。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class WarOfWords : CharlotteCardBase, IExileReturner
{
    private decimal _bonusDamage;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(3m, ValueProp.Move),
        new CalculationBaseVar(3m),
        new ExtraDamageVar(2m),
    };

    public WarOfWords()
        : base(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue + _bonusDamage).FromCard(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    public async Task OnTurnStartInExile(PlayerChoiceContext ctx)
    {
        _bonusDamage += base.DynamicVars.ExtraDamage.BaseValue;
        await CardPileCmd.AddGeneratedCardToCombat(this, PileType.Hand, base.Owner);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.ExtraDamage.UpgradeValueBy(1m);
    }
}
