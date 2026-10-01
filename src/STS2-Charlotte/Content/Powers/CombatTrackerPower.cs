using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;

namespace CharlotteMod.Content.Powers;

/// <summary>
/// 战斗计数器(隐藏):本回合打出的攻击牌/总牌数、本回合消耗的牌数,回合开始清零。
/// 由初始遗物(温亨廷先生/千织屋纪念版)在战斗开始时挂载——初始遗物每局常驻且换装后仍在,
/// 双变体都接(幂等)。不攻自破/可靠信源/掷地有声/时效核验等卡经
/// Owner.Creature.GetPower&lt;CombatTrackerPower&gt;() 查询。无需本地化(永不可见)。
/// </summary>
[RegisterPower]
public sealed class CombatTrackerPower : CharlottePowerBase
{
    private sealed class Data
    {
        public int attacksThisTurn;

        public int cardsThisTurn;

        public int exhaustsThisTurn;
    }

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override bool IsVisibleInternal => false;

    protected override object InitInternalData()
    {
        return new Data();
    }

    /// <summary>本回合已打出的攻击牌数(结算中的这一张不计——AfterCardPlayed 在打完后才 +1)。</summary>
    public int AttacksThisTurn => GetInternalData<Data>().attacksThisTurn;

    /// <summary>本回合已打出的牌数(不含结算中的这一张)。</summary>
    public int CardsThisTurn => GetInternalData<Data>().cardsThisTurn;

    /// <summary>本回合已消耗的牌数。</summary>
    public int ExhaustsThisTurn => GetInternalData<Data>().exhaustsThisTurn;

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (participants.Contains(base.Owner))
        {
            Data data = GetInternalData<Data>();
            data.attacksThisTurn = 0;
            data.cardsThisTurn = 0;
            data.exhaustsThisTurn = 0;
        }
        return Task.CompletedTask;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature == base.Owner)
        {
            Data data = GetInternalData<Data>();
            data.cardsThisTurn++;
            if (cardPlay.Card.Type == CardType.Attack)
            {
                data.attacksThisTurn++;
            }
        }
        return Task.CompletedTask;
    }

    public override Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        if (card.Owner.Creature == base.Owner)
        {
            GetInternalData<Data>().exhaustsThisTurn++;
        }
        return Task.CompletedTask;
    }
}
