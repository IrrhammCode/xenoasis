# XENOASIS: The Memory of Water — Tripothon S1 Official Submission Package

> *"Before they arrive, we built them a gift."*

---

## 1. Project Overview & Meta
* **Project Title:** XENOASIS: The Memory of Water
* **Tagline:** A poetic VR sanctuary on PICO 4 Ultra offering Earth's ephemeral wonders to an arriving alien intelligence.
* **Direction Track:** VR / XR / AR
* **Tool Tracks:** 
  * 🥇 Best Use of Tripo AI
  * 🥇 Best Use of World Labs
  * 🥇 Best Use of PICO
* **Engine:** Unity 2022.3 LTS (Universal Render Pipeline)
* **Target Hardware:** PICO 4 Ultra (Standalone Android APK, 90 FPS)
* **Interaction Mode:** Optical Bare-Hand Tracking (No controllers needed) + MR Video See-Through Passthrough
* **Experience Length:** 4 minutes (Linear emotional narrative arc)

---

## 2. Artist Statement (300 Words)

We often imagine first contact as war, diplomacy, or technological subjugation. *XENOASIS* proposes an alternative: **hospitality as an act of cosmic grief and grace.**

You awaken as the last human curator aboard an ancient orbital waystation. Long after Earth’s surface has frozen into silence, an alien intelligence is scheduled to dock in four minutes. Knowing they will never see liquid water, hear acoustic harmonies, or witness biological growth in person, humanity left behind a sanctuary—a tactile memorial of what it felt like to inhabit Earth.

Through the optical bare-hand tracking of the PICO 4 Ultra, the experience strips away plastic controllers. The interface is intimacy itself: your bare hands ripple zero-gravity water, cup harmonic crystals to tune celestial singing bowls, and channel solar warmth to coax dormant alien xenobiology into bloom. 

Visually, *XENOASIS* merges Tripo AI’s organic 3D geometry with World Labs’ infinite celestial panorama. Rather than generating random assets, Tripo AI was tasked with materializing non-Euclidean artifacts: translucent sapphire basins with golden coral veins, faceted acoustic crystals, and bioluminescent flora that react organically to physical proximity. Beneath you, a 12-meter circular platform of polished obsidian glass reflects pulsing bioluminescent veins, framed by skeletal pillars curving overhead into an open cathedral dome.

When all three gifts are sealed, the sanctuary does not explode—it illuminates. A vertical beacon of white-gold light erupts skyward, sending Earth’s memories into the cosmic deep. *XENOASIS* is not a game to be won; it is an emotional offering—a quiet, tactile testament that we were here, and that what we loved most was gentle.

---

## 3. Tool Track Synergy & Technical Implementation

### A. Tripo AI (Best Use of Tool Track)
* **Role:** Complete 3D artifact pipeline generating 9 procedural, game-ready PBR models.
* **Automation:** Automated via Python API script (`tools/tripo_api.py`) interfacing with `openapi.tripo3d.ai/v3`.
* **Asset Index & Prompts:**
  1. `central_basin.glb` (54.0 MB): *"A shallow ornate ceremonial basin carved from translucent dark sapphire stone, intertwined with bioluminescent golden coral veins, elegant sci-fi solarpunk design, clean geometry, PBR textures, game-ready 3D model."*
  2. `water_lotus.glb` (48.6 MB): *"An ethereal floating lotus flower made of liquid frosted glass with glowing blue fiber optic veins inside the petals, blooming state, symmetrical, game asset."*
  3. `resonant_crystal.glb` (54.1 MB): *"A complex sacred geometry crystal in a faceted teardrop shape, glowing cyan energy core visible inside, ancient alien runic engravings on the surface, sci-fi artifact, game-ready."*
  4. `obsidian_pillar.glb` (54.8 MB): *"A tall twisted column of polished obsidian stone with glowing cyan resin flowing through cracks, ancient alien temple architecture, dark elegant, PBR game asset."*
  5. `alien_flora_dormant.glb` (52.1 MB): *"A closed alien plant bud with curled dark grey-green tendrils, organic alien xenobiology, dormant sleeping state, detailed texture, game asset."*
  6. `alien_flora_bloomed.glb` (50.4 MB): *"A fully bloomed bioluminescent alien flower with translucent glass-like petals glowing cyan and gold, pollen particles rising, ethereal beautiful, game-ready."*
  7. `glass_chime.glb` (46.2 MB): *"Three delicate floating teardrop-shaped chimes made of frosted glass, each glowing softly with internal blue light, game asset."*
  8. `offering_pedestal.glb` (52.8 MB): *"A low hexagonal pedestal made of dark stone with gold inlay patterns, ancient alien altar, subtle glow from the edges, clean geometry, game-ready PBR model."*
  9. `floor_tile.glb` (50.6 MB): *"A single hexagonal floor tile made of polished black obsidian glass with faint glowing cyan circuit-like veins beneath the surface, game asset."*

### B. World Labs Marble API (Best Use of Tool Track)
* **Role:** Generative cosmic void skybox and world physics collider.
* **Automation:** Automated via Python script (`tools/worldlabs_api.py`) using `api.worldlabs.ai/marble/v1`.
* **Generated Assets:**
  * `CosmicVoid_Pano.png` (10.2 MB): 360° equirectangular skybox depicting a celestial sanctuary floating in a cosmic void with swirling cyan/violet nebulae.
  * `sanctuary_collider.glb` (4.0 MB): Physics collider mesh bounding the playable dais.

### C. PICO 4 Ultra (Best Use of Hardware Track)
* **Optical Bare-Hand Tracking:** Native `PXR_HandTracking` querying 25 skeletal joints per hand without controllers. Supports:
  * Proximity-based water deformation and droplet detachment.
  * Two-handed acoustic singing bowl pitch modulation.
  * Palm-down solar ray emission with vector alignment (`Vector3.Dot`).
* **MR Video See-Through Passthrough:** Opening sequence uses `PXR_MixedReality.EnableVideoSeeThroughManual(true)` to transition the player from their actual physical room into the deep cosmic void.
* **90 FPS Optimization:** Standalone ARM64 APK with IL2CPP scripting backend, Single Pass Instanced rendering, and custom lightweight URP HLSL shaders.

---

## 4. Judges' Video Walkthrough Script (3-5 Minutes)

| Timestamp | Visual Action (Recorded from PICO 4 Ultra Headset) | On-Screen Caption / Overlay | Audio Soundscape |
|---|---|---|---|
| **0:00 - 0:25** | Physical living room visible via MR Passthrough. Reality dissolves into space. Player stands on obsidian floor looking at stars. | *"XENOASIS: The Memory of Water"*<br>*(Tool Tracks: Tripo AI + World Labs + PICO)* | Ambient 40Hz sub-bass drone (`cosmic_drone.wav`) |
| **0:25 - 0:40** | Floor particle trail softly guides gaze forward to the central basin. Prompt overlay appears. | *Tripo Prompt: "central_basin" & "water_lotus"* | Water echo droplets (`water_drops.wav`) |
| **0:40 - 1:25** | Player reaches bare hands into water sphere. Surface ripples. Cupping hands fragments sphere into floating ice. Pedestal turns gold. | *"Offering 1 Sealed — The Gift of Water"* | Harmonic bell chime (`offering_complete.wav`) |
| **1:25 - 2:10** | Player walks to left altar. Places two hands around teardrop crystal. Pitch modulates. Sweet spot emits golden waves. Pedestal turns gold. | *Tripo Prompt: "resonant_crystal"*<br>*"Offering 2 Sealed — The Gift of Sound"* | Crystal singing bowl C tone (`crystal_tone_C.wav`) |
| **2:10 - 2:55** | Player moves to right altar. Open palm above dormant plant channels golden energy beam. Bud blooms into glowing flower. | *Tripo Prompt: "alien_flora_dormant/bloomed"*<br>*"Offering 3 Sealed — The Gift of Life"* | Warm F# Maj9 bloom chord (`bloom_chord.wav`) |
| **2:55 - 3:35** | 3 beams converge to center. Massive vertical beacon erupts skyward. Stardust accelerates inward. Crescendo peaks. Screen whites out. | *"The Climax — The Beacon Ignites"* | 35s epic cathedral crescendo (`beacon_crescendo.wav`) |
| **3:35 - 4:00** | Pure white screen fades into text: *"The gift was received."* $\rightarrow$ *"XENOASIS"* $\rightarrow$ Credits. | *"Tripothon S1 · Built with Tripo AI + World Labs + PICO 4 Ultra"* | Peaceful silence |

---

## 5. Social Media Teaser Copy (X / Twitter & TikTok)

### Post Draft:
```
"Before they arrive, we built them a gift." 🌌

Presenting XENOASIS: The Memory of Water — our entry for #Tripothon S1! 

A 4-minute VR sanctuary on @PICOXR 4 Ultra using bare-hand tracking to prepare Earth's sacred offerings for an arriving alien intelligence.

✨ All 9 3D artifacts generated via @TripoAI
🌌 360° celestial void world via @WorldLabsAI
🤲 100% controller-free bare-hand tracking + MR passthrough on PICO 4 Ultra

Built solo with Unity URP (90 FPS native APK). Watch the full experience below! 💧💎🌸

#Tripothon #TripoAI #WorldLabs #PICO4Ultra #WebXR #VR #IndieDev
```
