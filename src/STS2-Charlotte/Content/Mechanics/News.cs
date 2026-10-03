using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using CharlotteMod.Content.CardPools;
using CharlotteMod.Content.Keywords;

namespace CharlotteMod.Content.Mechanics;

/// <summary>新闻打出观察者:能力/遗物实现本接口,由 News.Play 在每次新闻打出后分发。</summary>
public interface INewsObserver
{
    Task OnNewsPlayed(PlayerChoiceContext ctx, CardModel news, Player player);
}

/// <summary>
/// [新闻]家族接线:七张虚无无色 token 卡(注册于 CharlotteNewsPool,Token 稀有度,不进奖励/商店池)。
/// 不设共享基类——花名册审计按 Content/Cards 下非 Base 前缀类文件计数,多一个基类会被误计为卡;
/// 统一从 <see cref="KeywordSet"/> 取关键词、从 <see cref="Play"/> 走「先效果、后[聚焦]结算、再观察者」的固定顺序。
/// 留影纪念克隆复用原牌的同一 OnPlay,新闻克隆打出同样触发聚焦与观察者。
/// </summary>
public static class News
{
    /// <summary>[新闻]族判据:注册于 CharlotteNewsPool 的卡(留影纪念克隆与原牌同池同型,同样命中)。</summary>
    public static bool IsNews(CardModel card) => card.Pool is CharlotteNewsPool;

    /// <summary>新闻牌关键词集(虚无 + [新闻]横幅);调用方在其上追加消耗/[留影]/[聚焦]等后返回。</summary>
    public static HashSet<CardKeyword> KeywordSet()
    {
        var set = new HashSet<CardKeyword> { CardKeyword.Ethereal };
        CharlotteKeywords.AddTo(set, CharlotteKeywords.News);
        return set;
    }

    /// <summary>新闻牌打出统一入口:先执行卡牌效果,再结算[聚焦],最后分发新闻观察者。</summary>
    public static async Task Play(PlayerChoiceContext ctx, CardModel card, Func<Task> effect)
    {
        await effect();
        if (card.CombatState is { } combatState)
        {
            await Focus.TriggerAll(combatState);
        }
        foreach (PowerModel? observer in card.Owner?.Creature?.Powers)
        {
            if (observer is INewsObserver o)
            {
                await o.OnNewsPlayed(ctx, card, card.Owner!);
            }
        }
        if (card.Owner is { } owner)
        {
            foreach (RelicModel relic in owner.Relics)
            {
                if (relic is INewsObserver o)
                {
                    await o.OnNewsPlayed(ctx, card, owner);
                }
            }
        }
    }

    /// <summary>七种新闻的生成工厂(随机抽选走 CombatCardGeneration 通道,BigHat 同款)。</summary>
    private static readonly Func<Player, ICombatState, CardModel>[] _newsFactories =
    {
        (p, cs) => cs.CreateCard<Cards.CharlotteGleamBulletin>(p),
        (p, cs) => cs.CreateCard<Cards.CharlotteMissingPersonNotice>(p),
        (p, cs) => cs.CreateCard<Cards.CharlotteHazardWarning>(p),
        (p, cs) => cs.CreateCard<Cards.CharlotteJudgmentNews>(p),
        (p, cs) => cs.CreateCard<Cards.CharlotteStreetInterview>(p),
        (p, cs) => cs.CreateCard<Cards.CharlottePaidPromotion>(p),
        (p, cs) => cs.CreateCard<Cards.CharlotteExclusiveReport>(p),
    };

    /// <summary>生成 1 张随机新闻(未入堆;调用方决定去向)。</summary>
    public static CardModel CreateRandom(Player owner, ICombatState combatState)
    {
        Func<Player, ICombatState, CardModel> factory = owner.RunState.Rng.CombatCardGeneration.NextItem(_newsFactories);
        return factory(owner, combatState);
    }

    /// <summary>生成 n 张互不重复的新闻(不足 7 种时可能少于 n 张;早间特报三选一用)。</summary>
    public static List<CardModel> CreateRandomDistinct(int count, Player owner, ICombatState combatState)
    {
        var pool = new List<Func<Player, ICombatState, CardModel>>(_newsFactories);
        var result = new List<CardModel>();
        while (result.Count < count && pool.Count > 0)
        {
            Func<Player, ICombatState, CardModel> factory = owner.RunState.Rng.CombatCardGeneration.NextItem(pool);
            pool.Remove(factory);
            result.Add(factory(owner, combatState));
        }
        return result;
    }

    /// <summary>生成 1 张随机新闻并加入手牌(羽笔打击等「生成1张[新闻]」入口)。</summary>
    public static async Task<CardModel> CreateRandomInHand(PlayerChoiceContext ctx, Player owner)
    {
        CardModel news = CreateRandom(owner, owner.Creature.CombatState!);
        await CardPileCmd.AddGeneratedCardsToCombat(new[] { news }, PileType.Hand, owner);
        return news;
    }
}
