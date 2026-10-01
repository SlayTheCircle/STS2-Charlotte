#!/usr/bin/env bash
# 验证真实产物的本地化可解析；不能替代游戏内模型、场景和机制验收。
set -euo pipefail
source "$(dirname "$0")/dev-env.sh"
mod_require_godot
export MOD_PCK="${MOD_PCK:-$MOD_ROOT/mods-dist/$MOD_ID/$MOD_ID.pck}"
[[ -f "$MOD_PCK" ]] || { echo '错误: PCK 不存在；先完整构建。' >&2; exit 1; }
"$GODOT_EXE" --headless --script "$MOD_ROOT/tools/verify_pck.gd"
