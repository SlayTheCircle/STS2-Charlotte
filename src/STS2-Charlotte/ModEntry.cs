using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
using STS2RitsuLib.Content;
using STS2RitsuLib.Interop;
using STS2RitsuLib.Keywords;
using STS2RitsuLib.Scaffolding.Characters;

namespace CharlotteMod;

/// <summary>
/// Mod 入口。加载清单见仓库根 STS2-Charlotte.json;本文件只做注册接线,
/// 游戏内容逻辑全部位于 Content/ 下的纯模型类中。
/// </summary>
[ModInitializer("Init")]
public static class ModEntry
{
    public const string ModId = "STS2-Charlotte";

    public static void Init()
    {
        // 归属声明:让本程序集内的 RitsuLib 自动注册特性([RegisterCard]/[RegisterCharacter]/…)正确归属到本 mod。
        // 必须第一条;内容注册全部由各模型类上的特性完成,本入口不再手工登记内容清单。
        ModTypeDiscoveryHub.RegisterModAssembly(ModId, typeof(ModEntry).Assembly);

        // 机制关键词注册(stem 必须与 CharlotteKeywords 常量的 KEYWORD_ 后缀一致;
        // 文本在 localization/{lang}/card_keywords.json)。
        ModKeywordRegistry keywords = ModKeywordRegistry.For(ModId);
        keywords.RegisterCardKeywordOwnedByLocNamespace("SNAPSHOT"); // → 留影
        keywords.RegisterCardKeywordOwnedByLocNamespace("FOCUS");    // → 聚焦
        keywords.RegisterCardKeywordOwnedByLocNamespace("NEWS");     // → 新闻

        // 角色资产档案(B10 正式管线,无 Spine 路线,场景与图全部自备,与 Navia 同构):
        // 战斗形象/商店/休息点/选人背景为自建场景(单 Sprite2D + 原版节点契约脚本);
        // 立绘母版 1024×1536(倒下为横图);选人半身图由站立立绘人工校准裁剪(characters.sh)。
        string charlotteEntry = ModContentRegistry.GetCompoundId(ModId, "character", nameof(Content.Characters.Charlotte)).ToLowerInvariant();
        ModContentRegistry.For(ModId).RegisterCharacterAssetReplacement(charlotteEntry, new CharacterAssetProfile(
            new CharacterSceneAssetSet(
                "res://STS2-Charlotte/scenes/characters/charlotte_character.tscn",
                "res://STS2-Charlotte/scenes/combat/charlotte_energy_counter.tscn",
                "res://STS2-Charlotte/scenes/characters/charlotte_merchant.tscn",
                "res://STS2-Charlotte/scenes/characters/charlotte_rest_site.tscn"),
            new CharacterUiAssetSet(
                "res://STS2-Charlotte/images/characters/charlotte_character_icon.png",
                "res://STS2-Charlotte/images/characters/charlotte_character_icon_outline.png",
                "res://STS2-Charlotte/scenes/characters/charlotte_icon.tscn",
                "res://STS2-Charlotte/scenes/characters/charlotte_char_select_bg.tscn",
                "res://STS2-Charlotte/images/characters/charlotte_select.png",
                "res://STS2-Charlotte/images/characters/charlotte_select_locked.png",
                // 转场材质留空会按条目名推导 mod 条目下不存在的 .tres → AssetLoadException 炸开局;先借通用淡入淡出。
                "res://materials/transitions/fade_transition_mat.tres",
                "res://STS2-Charlotte/images/characters/charlotte_map_marker.png"),
            // 出牌轨迹留空会推导 vfx/card_trail_<entry> → NCardFlyVfx._Ready 空引用,牌堆动画卡死;先借铁甲轨迹。
            new CharacterVfxAssetSet("res://scenes/vfx/card_trail_ironclad.tscn"),
            null,
            null,
            null));

        // 自带 Harmony 补丁:ModInitializer 通道与游戏自动 PatchAll 是官方二选一语义——
        // 本类有 [ModInitializer] 则游戏只调 Init() 不再自动 PatchAll;有补丁类时必须在此手动补。
        new Harmony(ModId + ".patches").PatchAll(typeof(ModEntry).Assembly);
    }
}
