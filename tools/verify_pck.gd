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
	if failures == 0:
		print("PCK 本地化可解析；未验证纹理、游戏模型、场景或机制。")
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
