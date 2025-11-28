# Building RoR2 DevTool

## Prerequisites

1. **Node.js** (v18 or higher)
2. **pnpm** - Install with: `npm install -g pnpm`
3. **Rust** - Install from https://rustup.rs/
4. **Tauri CLI** - Will be installed automatically via pnpm

## Build Instructions

### Development Build
```bash
cd WebUI-Svelte
pnpm install
pnpm tauri dev
```

### Production Build (Portable Executable)

#### Windows
```bash
cd WebUI-Svelte
pnpm install
pnpm tauri build
```

This will create:
- **NSIS Installer**: `src-tauri/target/release/bundle/nsis/RoR2 DevTool_1.0.0_x64-setup.exe`
- **MSI Installer**: `src-tauri/target/release/bundle/msi/RoR2 DevTool_1.0.0_x64_en-US.msi`
- **Portable EXE**: `src-tauri/target/release/RoR2 DevTool.exe`

#### Linux
```bash
cd WebUI-Svelte
pnpm install
pnpm tauri build
```

Creates: `.deb`, `.AppImage`, and binary in `src-tauri/target/release/`

#### macOS
```bash
cd WebUI-Svelte
pnpm install
pnpm tauri build
```

Creates: `.dmg` and `.app` bundle in `src-tauri/target/release/bundle/`

## Build Outputs

### Windows
- **NSIS Installer** (Recommended): User-friendly installer with shortcuts
- **MSI Installer**: Enterprise deployment option
- **Portable EXE**: Standalone executable (no installation required)
  - Location: `src-tauri/target/release/RoR2 DevTool.exe`
  - Can be copied and run anywhere

### Portable Executable Usage
The portable `.exe` file can be:
- Copied to any location
- Run without installation
- Distributed as a single file
- No admin rights required

## Build Configuration

The build is configured in `src-tauri/tauri.conf.json`:

- **NSIS Installer Options**:
  - Per-user installation (no admin required)
  - Desktop shortcut creation
  - Start menu shortcut
  - Custom installation directory
  - Uninstaller included

- **Bundle Targets**: NSIS and MSI for Windows
- **Compression**: LZMA (best compression)
- **One-Click Install**: Disabled (allows customization)

## Troubleshooting

### Build Fails
1. Ensure Rust is installed: `rustc --version`
2. Update Rust: `rustup update`
3. Clear cache: `pnpm tauri build --clean`

### Missing Dependencies
```bash
# Install all dependencies
pnpm install

# Update Tauri
pnpm update @tauri-apps/cli @tauri-apps/api
```

### Reduce Build Size
The release build is optimized automatically. To further reduce:
1. Edit `src-tauri/Cargo.toml`
2. Add under `[profile.release]`:
```toml
strip = true
lto = true
codegen-units = 1
```

## Distribution

### For End Users
Distribute the **NSIS installer** (`RoR2 DevTool_1.0.0_x64-setup.exe`):
- Easy installation
- Automatic shortcuts
- Clean uninstallation

### For Portable Use
Distribute the **portable EXE** (`RoR2 DevTool.exe`):
- No installation needed
- Single file
- Can run from USB drive

## File Locations After Build

```
WebUI-Svelte/
└── src-tauri/
    └── target/
        └── release/
            ├── RoR2 DevTool.exe          # Portable executable
            └── bundle/
                ├── nsis/
                │   └── RoR2 DevTool_1.0.0_x64-setup.exe  # Installer
                └── msi/
                    └── RoR2 DevTool_1.0.0_x64_en-US.msi  # MSI installer
```

## Quick Build Commands

```bash
# Full production build
pnpm tauri build

# Build with debug info
pnpm tauri build --debug

# Build for specific target
pnpm tauri build --target x86_64-pc-windows-msvc

# Clean build
rm -rf src-tauri/target
pnpm tauri build
```
