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
using CharlotteMod.Content.Keywords;
using CharlotteMod.Content.Mechanics;

namespace CharlotteMod.Content.Cards;

/// <summary>
/// 精彩纷呈(普通,0 费攻击):固有。造成 3 点伤害,然后将这张牌[留影]。
/// 自我留影:打出中的牌在 Play 堆,去向经 GetResultLocationForCardPlay 覆写路由进消耗堆
/// (引擎在 OnPlay 之前预计算去向,OnPlay 内补 ExhaustOnNextPlay 赶不上本次路由——原牌曾
/// 被误送弃牌堆,2026-10-03 业主问询坐实后改走 vanilla ShiningStrike 同款覆写缝);
/// 纪念克隆走 Snapshot.Memento(只生成不消耗)。升级:5 伤。
/// 平衡裁定(Mirror 2026-10-02):纪念版**不再自我留影**——纪念克隆继承本卡 OnPlay,
/// 会自我复制;配剪贴相册(打纪念抽牌)构成 0 费固有无限。仅原版留影一次,循环到纪念版终止。
/// (纪念版卡面沿用共用描述,后半句对其失真——文案待 Mirror 定夺。)
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class PhotoOp : CharlotteCardBase
{
    public override IEnumerable<CardKeyword> CanonicalKeywords
    {
        get
        {
            var set = new HashSet<CardKeyword> { CardKeyword.Innate };
            CharlotteKeywords.AddTo(set, CharlotteKeywords.Snapshot);
            return set;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new DamageVar(3m, ValueProp.Move) };

    public PhotoOp()
        : base(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
    }

    // 自我留影的原牌随打出入消耗堆:去向在 OnPlay 之前由引擎预计算,必须在此覆写;
    // 纪念版自带消耗关键词,base 路由天然进消耗堆,不劫持。两个游戏目标同缝异形:
    // 0.111.0 返回 CardLocation 记录,0.107.1 只有 PileType 版本(与 CharlottePowerBase
    // 的 ModifyDamageAdditive 垫片同一对差异)。
#if !MOD_GAME_0107_1
    protected override CardLocation GetResultLocationForCardPlay()
    {
        CardLocation result = base.GetResultLocationForCardPlay();
        if (!Snapshot.IsMemento(this))
        {
            result.pileType = PileType.Exhaust;
            result.position = CardPilePosition.Bottom;
        }
        return result;
    }
#else
    protected override PileType GetResultPileTypeForCardPlay()
    {
        return Snapshot.IsMemento(this) ? base.GetResultPileTypeForCardPlay() : PileType.Exhaust;
    }
#endif

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        if (!Snapshot.IsMemento(this))
        {
            await Snapshot.Memento(choiceContext, this, base.Owner);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(2m);
    }
}
