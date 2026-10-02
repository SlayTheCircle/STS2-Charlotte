# 验证真实 PCK 的本地化可解析；不代替游戏运行时验收。
#
# ##############################################################################
# #                                                                            #
# #   派生仓扩展点(重要):本文件是模板的本地化核心,只守护全部派生 Mod 的         #
# #   公共底线——11 张必需本地化表在 zhs+eng 存在且可解析。                      #
# #                                                                            #
# #   衍生 Mod 必须在此追加自己的代表性纹理抽样清单与断言,否则坏素材会静默       #
# #   漏过完整构建:                                                            #
# #     - TEXTURES 样本路径列表 + ResourceLoader 存在性/可加载检查              #
# #     - DIMENSIONS 关键图的实际显示尺寸断言(竖卡裁框/缩略图/内联小图)         #
# #     - TRANSPARENT_TEXTURES 透明边缘断言(实底图标在游戏小图槽会显示方框)     #
# #     - 需要场景入包时追加 FileAccess 存在性检查                               #
# #   完整参考实现见源工程(母本 Mod)的 tools/verify_pck.gd。                    #
# #                                                                            #
# ##############################################################################
extends SceneTree

const TABLES := ["cards", "powers", "relics", "potions", "characters", "card_keywords",
	"card_selection", "events", "enchantments", "epochs", "ancients"]

# 衍生仓扩展点:角色场景(B10 正式管线)必须入包且可打开。
const SCENES := [
	"res://%s/scenes/characters/charlotte_character.tscn",
	"res://%s/scenes/characters/charlotte_merchant.tscn",
	"res://%s/scenes/characters/charlotte_rest_site.tscn",
	"res://%s/scenes/characters/charlotte_char_select_bg.tscn",
	"res://%s/scenes/characters/charlotte_icon.tscn",
	"res://%s/scenes/combat/charlotte_energy_counter.tscn",
]

# 关键纹理的尺寸断言(宽x高);0 高表示仅查可加载。
const TEXTURES := {
	"res://%s/images/events/BreathtakingView.png": Vector2i(1672, 941),
	"res://%s/images/events/MerchantsRequest.png": Vector2i(1672, 941),
	"res://%s/images/events/HeatedDebate.png": Vector2i(1672, 941),
	"res://images/timeline/epoch_portraits/sts2_charlotte_epoch_1.png": Vector2i(1672, 941),
	"res://%s/images/timeline/sts2_charlotte_epoch_1_thumb.png": Vector2i(272, 174),
	"res://images/timeline/epoch_portraits/sts2_charlotte_epoch_2.png": Vector2i(1672, 941),
	"res://%s/images/timeline/sts2_charlotte_epoch_2_thumb.png": Vector2i(272, 174),
	"res://images/timeline/epoch_portraits/sts2_charlotte_epoch_3.png": Vector2i(1672, 941),
	"res://%s/images/timeline/sts2_charlotte_epoch_3_thumb.png": Vector2i(272, 174),
	"res://images/timeline/epoch_portraits/sts2_charlotte_epoch_4.png": Vector2i(1672, 941),
	"res://%s/images/timeline/sts2_charlotte_epoch_4_thumb.png": Vector2i(272, 174),
	"res://%s/images/powers/OneTurnBlockPersistPower.png": Vector2i(256, 256),
	"res://%s/images/characters/charlotte_normal.png": Vector2i(1024, 1536),
	"res://%s/images/characters/charlotte_merchant.png": Vector2i(1024, 1536),
	"res://%s/images/characters/charlotte_rest_site.png": Vector2i(1024, 1536),
	"res://%s/images/characters/charlotte_char_select_bg.png": Vector2i(1672, 941),
	"res://%s/images/characters/charlotte_character_icon.png": Vector2i(128, 128),
	"res://%s/images/energy/charlotte_energy_big.png": Vector2i(256, 256),
	"res://%s/images/energy/charlotte_energy_text.png": Vector2i(24, 24),
}

# 必须带透明通道的纹理(实底图在透明槽会显示方框)。
const TRANSPARENT_TEXTURES := [
	"res://%s/images/powers/OneTurnBlockPersistPower.png",
	"res://%s/images/characters/charlotte_select.png",
	"res://%s/images/characters/charlotte_select_locked.png",
	"res://%s/images/characters/charlotte_normal.png",
	"res://%s/images/energy/charlotte_energy_big.png",
]

var _frame := 0
var _mod_id := ""

func _process(_delta: float) -> bool:
	_frame += 1
	if _frame != 1:
		return true
	_mod_id = OS.get_environment("MOD_ID")
	if _mod_id.is_empty():
		push_error("MOD_ID not set")
		quit(1)
		return true
	var pck := OS.get_environment("MOD_PCK")
	if pck.is_empty() or not ProjectSettings.load_resource_pack(pck, true):
		push_error("PCK 挂载失败")
		quit(1)
		return true
	var failures := 0
	for lang in ["zhs", "eng"]:
		for table in TABLES:
			failures += _verify_localization(lang, table)
	for tpl in SCENES:
		var p: String = tpl % _mod_id
		if not FileAccess.file_exists(p):
			push_error("PCK 缺场景: " + p)
			failures += 1
	for tpl in TEXTURES:
		var p: String = tpl % _mod_id if "%s" in tpl else tpl
		var tex := load(p) as Texture2D
		if tex == null:
			push_error("PCK 纹理不可加载: " + p)
			failures += 1
		elif TEXTURES[tpl] != Vector2i(0, 0) and Vector2i(tex.get_size()) != TEXTURES[tpl]:
			push_error("PCK 纹理尺寸不符: %s 期望 %s 实际 %s" % [p, TEXTURES[tpl], Vector2i(tex.get_size())])
			failures += 1
	for tpl in TRANSPARENT_TEXTURES:
		var p: String = tpl % _mod_id
		var img := (load(p) as Texture2D).get_image() if load(p) is Texture2D else null
		if img == null or not img.detect_alpha():
			push_error("PCK 纹理无透明通道: " + p)
			failures += 1
	if failures == 0:
		print("PCK 本地化可解析;角色场景、剧情/角色/能力纹理尺寸及透明通道断言通过。")
	quit(0 if failures == 0 else 1)
	return true

func _verify_localization(lang: String, table: String) -> int:
	var path := "res://%s/localization/%s/%s.json" % [_mod_id, lang, table]
	if not FileAccess.file_exists(path):
		push_error("PCK 缺本地化: " + path)
		return 1
	# 空对象合法:表存在且可解析即过(未用表零键覆盖,游戏合并无害)。
	var parsed = JSON.parse_string(FileAccess.get_file_as_string(path))
	if not parsed is Dictionary:
		push_error("PCK 本地化不可解析: " + path)
		return 1
	return 0
