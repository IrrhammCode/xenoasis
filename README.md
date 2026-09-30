# XENOASIS: The Memory of Water

> *"Before they arrive, we built them a gift."*

**XENOASIS** is a 4-minute VR sanctuary experience for the **PICO 4 Ultra** where you — the last human curator — prepare three sacred offerings of Earth's ephemeral wonders for an alien intelligence about to dock.

## Tripothon S1 Entry
- **Direction Track:** VR / XR / AR
- **Tool Tracks:** Tripo AI + World Labs + PICO
- **Engine:** Unity 2022.3 LTS (URP)
- **Deadline:** Oct 5, 2026 23:59 AoE

## Quick Start

### 1. Unity Project
Open this folder in **Unity 2022.3 LTS**. Install:
- Universal Render Pipeline (URP)
- PICO Unity Integration SDK
- TextMeshPro

### 2. Generate 3D Assets (API Pipeline)
```bash
# Install Python dependencies
pip install requests

# Copy and fill in your API keys
cp .env.example .env

# Generate all Tripo AI assets + World Labs environment
python tools/pipeline_sync.py

# Or dry-run to test without spending credits
python tools/pipeline_sync.py --dry-run
```

### 3. Scene Setup
In Unity: **XENOASIS > Setup Scene — Welcome Chamber** (menu bar)

## Project Structure
```
Assets/
├── Scripts/
│   ├── Core/          GameManager, OfferingManager
│   ├── Interaction/   HandProximityDetector, WaterSphere, Crystal, Flora
│   ├── VFX/           Beacon, Stardust, EnergyBeam, SoundWave, GuidingTrail
│   ├── Audio/         AudioManager
│   ├── Camera/        FadeController, PassthroughTransition
│   ├── UI/            EndScreenUI
│   └── Editor/        SceneSetupEditor
├── Shaders/           EmissivePulse, WaterSphere, ObsidianFloor
├── Models/Tripo/      (generated .glb files)
├── Textures/Skybox/   (World Labs panorama)
└── ...
tools/
├── tripo_api.py       Tripo AI text-to-3D automation
├── worldlabs_api.py   World Labs Marble world generation
└── pipeline_sync.py   Unified pipeline runner
```

## License
Hackathon submission — all rights reserved.
