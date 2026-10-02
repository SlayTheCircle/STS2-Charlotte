#!/usr/bin/env bash
# 安全部署工坊布局到本机 mods/:核心防呆是不预删——直接覆盖拷贝。
# 理由:rm-then-cp 在游戏恰好启动的竞态下会留下残缺目录(2026-10-02 实测事故:
# rm 删了一半被 dll 锁挡住,运行中的游戏随后读不到 pck 直接闪退);
# 覆盖式 cp 遇锁只会失败,不会破坏现有可用安装。删除多余旧文件用逐条 rm+容错。
set -euo pipefail
cd "$(dirname "$0")/.."
source scripts/dev-env.sh
SRC="$MOD_ROOT/mods-dist/workshop/$MOD_ID"
DST="${GAME_DIR:?需要 GAME_DIR}/mods/$MOD_ID"

if [ ! -f "$SRC/$MOD_ID.pck" ]; then
    echo "错误: $SRC 缺构建产物,先运行 scripts/package.sh" >&2
    exit 1
fi
if ! tasklist_out=$("$TASKLIST_EXE" 2>&1); then
    echo "错误: 无法调用 tasklist.exe,拒绝部署。" >&2
    exit 1
fi
if printf '%s' "$tasklist_out" | grep -qiE 'spire|sts2'; then
    echo "错误: 游戏正在运行。运行中部署会导致卡图丢失/闪退,请先关闭游戏。" >&2
    exit 1
fi

mkdir -p "$DST"
# 覆盖拷贝:被锁文件 cp 失败即中止(set -e),现状不被破坏。
if ! cp -rf "$SRC/." "$DST/"; then
    echo "错误: 拷贝失败(文件被占用?)。现有安装未被破坏,请确认游戏已完全退出后重试。" >&2
    exit 1
fi
# 清掉源目录中不存在的历史文件(逐条容错,失败只警告)。
( cd "$DST" && find . -type f ! -exec test -e "$SRC/{}" \; -print ) 2>/dev/null | while read -r stale; do
    rm -f "$DST/$stale" 2>/dev/null || echo "警告: 旧文件未清理 $stale"
done
echo "部署完成: $DST ($(find "$DST" -type f | wc -l) 个文件)"
