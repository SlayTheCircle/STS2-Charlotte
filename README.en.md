# STS2-Template · Slay the Spire 2 mod template

[简体中文](README.md)

A Slay the Spire 2 mod template for the SlayTheCircle organization, distilled from a complete playable-character mod project. It ships the dual-branch (0.107.1 stable / 0.111.0 beta) compile, packaging and release pipeline together with the variant loader, the three-part documentation system, and the source audit suite. This repository is not a playable mod — it is the starting point for your next mod.

## Usage

Click **Use this template** on GitHub (or `gh repo create SlayTheCircle/<YourMod> --template SlayTheCircle/STS2-Template`), then:

```bash
git clone <your new repo> && cd <your new repo>
git config core.hookspath .githooks    # activate pre-commit audits (after every clone)
scripts/init-mod.sh STS2-<Mod> <PascalName> --cn-name "<Chinese name>"
```

`init-mod.sh` performs the full rename (manifest, source directories, namespaces, base family, localization keys, script and workflow references), self-verifies with the source checks, and scaffolds the private side. Repository setup and credentials are covered in [onboarding](docs/dev/onboarding.md).

## What you get

- **A compiling skeleton**: one character plus one example card / relic / power / potion / enchantment, dual-target conditional compilation (0.107.1 shims built in); `build.sh --dll-only` passes out of the box.
- **Distribution infrastructure**: the variant loader (workshop items pick content per running game version, including the game-side assembly registration fix), dual-target packaging, and the tag-triggered draft release workflow that extracts release notes from CHANGELOG sections.
- **An audit suite**: repository boundary, doc links, localization coverage, placeholders, roster, and card-table generation — enforced on pre-commit and CI.
- **The documentation system**: architecture / engineering style / verification discipline / workflow contracts, plus incident and decision log skeletons.

## What you do not get

Artwork and audio masters (media never enters public repositories; full builds need your private organization art repo configured as `ART_SOURCE_DIR`), game and RitsuLib binaries (compile references come from your own game installation), and the code for deep character-mod modules such as worldlines or ancient dialogues (shipped as optional-module docs with wiring guides instead).

## Development

Requirements: Bash, Python 3.11+, and the .NET SDK selected by `global.json`. Full asset builds additionally require the Godot 4.5.1 standard editor and local artwork.

```bash
cp .local-dev.env.example .local-dev.env   # configure local dependency and tool locations
./scripts/check.sh --source-only           # source checks; no game DLLs or artwork required
./scripts/restore-refs.sh                  # copy references from GAME_DIR
./scripts/build.sh --dll-only              # compile the DLL only
```

See the [contributor guide](CONTRIBUTING.md) and [build pipeline](docs/dev/pipeline.md). Read the private entry `local_dev/README.md` first when a local workspace exists.

## Documentation

- [Documentation index](docs/README.md) · [Developer docs](docs/dev/README.md) · [Design docs](docs/design/README.md) · [Technical history](docs/history/README.md)
- [Onboarding](docs/dev/onboarding.md): the complete checklist for deriving a new mod from this template.
- [Content SOP](docs/dev/content-sop.md): the recommended path from design documents to a playable mod (guidance, not gospel).
- [Changelog](CHANGELOG.md) · Community: [Code of conduct](CODE_OF_CONDUCT.md), [Security policy](SECURITY.md), [Support](SUPPORT.md), [Credits](CREDITS.md).

## License

Original software is licensed under [MIT](LICENSE); see the [licensing scope](LICENSING.md) and [third-party notices](THIRD_PARTY_NOTICES.md). The variant loader implementation derives from the loading approach of the [RitsuLib](https://github.com/BAKAOLC/STS2-RitsuLib) ecosystem; see the third-party notes.
