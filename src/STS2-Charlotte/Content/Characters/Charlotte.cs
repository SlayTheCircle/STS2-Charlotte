using System.Collections.Generic;
using Godot;
using MegaCrit.Sts2.Core.Animation;
using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;
using CharlotteMod.Content.CardPools;
using CharlotteMod.Content.Cards;
using CharlotteMod.Content.PotionPools;
using CharlotteMod.Content.RelicPools;
using CharlotteMod.Content.Relics;
using CharlotteMod.Content.Timeline;

namespace CharlotteMod.Content.Characters;

/// <summary>
/// 夏洛蒂:来自枫丹的蒸汽鸟报社记者,带着留影机前来尖塔考察新闻的踪迹。初始生命 70。
/// 核心机制:留影/聚焦/新闻(见 docs/history/design/card-roster.txt 与设计案)。
/// </summary>
[RegisterCharacter]
[UnlockEpochAfterRunAs(typeof(Charlotte1Epoch))]        // 第一章·新的报道:完成一局夏洛蒂
[UnlockEpochAfterWinAs(typeof(Charlotte2Epoch))]       // 第二章·全新视角:首次通关 → 解锁 追踪调查/摄影形态/广角镜头
[UnlockEpochAfterBossVictories(typeof(Charlotte3Epoch), 3)] // 第三章·独家爆料:累计击败 3 首领 → 解锁三遗物
[UnlockEpochAfterAscensionOneWin(typeof(Charlotte4Epoch))] // 第四章·独家专访:进阶 1 通关 → 解锁三药水
public sealed class Charlotte : CharacterModel
{
    // 主题色 #8CCEEA(Mirror 定稿);其余色值同相位派生,进游戏目视后微调。
    public override Color NameColor => new Color("8CCEEAFF");

    public override CharacterGender Gender => CharacterGender.Feminine;

    // 角色不锁定——UnlocksAfterRunAs 维持 null。
    protected override CharacterModel? UnlocksAfterRunAs => null;

    public override int StartingHp => 70;

    public override int StartingGold => 99;

    public override CardPoolModel CardPool => ModelDb.CardPool<CharlotteCardPool>();

    public override RelicPoolModel RelicPool => ModelDb.RelicPool<CharlotteRelicPool>();

    public override PotionPoolModel PotionPool => ModelDb.PotionPool<CharlottePotionPool>();

    public override IEnumerable<CardModel> StartingDeck => new CardModel[]
    {
        ModelDb.Card<CharlotteStrike>(),
        ModelDb.Card<CharlotteStrike>(),
        ModelDb.Card<CharlotteStrike>(),
        ModelDb.Card<CharlotteStrike>(),
        ModelDb.Card<CharlotteDefend>(),
        ModelDb.Card<CharlotteDefend>(),
        ModelDb.Card<CharlotteDefend>(),
        ModelDb.Card<CharlotteDefend>(),
        ModelDb.Card<CharlotteKacha>(),
        ModelDb.Card<CharlotteSayCheese>(),
    };

    public override IReadOnlyList<RelicModel> StartingRelics => new RelicModel[] { ModelDb.Relic<MonsieurVerite>() };

    public override float AttackAnimDelay => 0.15f;

    public override float CastAnimDelay => 0.4f;

    public override Color EnergyLabelOutlineColor => new Color("356D85");

    public override Color DialogueColor => new Color("57899E");

    public override Color MapDrawingColor => new Color("5AA6C7");

    public override Color RemoteTargetingLineColor => new Color("8CCEEAFF");

    public override Color RemoteTargetingLineOutline => new Color("356D85");

    // 占位:复用铁甲的切场音效,待配音接入后替换。
    public override string CharacterTransitionSfx => "event:/sfx/ui/wipe_ironclad";

#if !MOD_GAME_0107_1
    // 0.107.1 无此虚属性,其 GenerateAnimator 硬编码的默认映射与本覆写逐项相同,省略即等价。
    protected override List<(AnimState, string)> AnimationStates => new List<(AnimState, string)>
    {
        (new AnimState("attack"), "Attack"),
        (new AnimState("hurt"), "Hit"),
        (new AnimState("cast"), "Cast"),
    };
#endif

    public override List<string> GetArchitectAttackVfx()
    {
        return new List<string> { "vfx/vfx_attack_slash", "vfx/vfx_heavy_blunt", "vfx/vfx_bloody_impact" };
    }
}
