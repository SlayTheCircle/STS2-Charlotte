# 全量再生成前检查母版；缺输入时失败，不能靠已有成品伪装交付完成。
# 映射为空时各项检查自然通过；衍生仓登记映射后此脚本自动开始强制对应母版。
# 立绘/头像/能量等非映射母版的需求，衍生仓在此按自身清单追加 require_art_source 行。
require_art_source() {
    [[ -f "$SRC/$1" ]] || { echo "错误: 美术母版缺失: $1" >&2; return 1; }
}

for zh in "${!CARDS[@]}"; do require_art_source "卡图/$zh.png"; done
for zh in "${!RELICS[@]}"; do require_art_source "遗物/$zh.png"; done
for zh in "${!POTIONS[@]}"; do require_art_source "药水/$zh.png"; done
for zh in "${!POWERS[@]}"; do require_art_source "buff图标/$zh.png"; done
