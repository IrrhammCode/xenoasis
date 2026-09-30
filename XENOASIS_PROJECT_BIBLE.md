# XENOASIS — Project Bible
### *Xeno (Alien) + Oasis (Sanctuary) = XENOASIS*
> **"Build a world as a Gift — For the aliens about to arrive."**

---

## 0. PROJECT IDENTITY

| Field | Value |
|---|---|
| **Project Name** | **XENOASIS** |
| **Full Title** | XENOASIS: The Memory of Water |
| **Tagline** | *"Before they arrive, we built them a gift."* |
| **Hackathon** | Tripothon S1 — The 1st World-Building Hackathon (by Tripo AI) |
| **Direction Track** | VR / XR / AR |
| **Tool Tracks** | Tripo AI + World Labs + PICO |
| **Target Device** | PICO 4 Ultra (Standalone Android APK) |
| **Engine** | Unity 2022.3 LTS — Universal Render Pipeline (URP) |
| **Team Size** | Solo (1 person) |
| **Experience Duration** | 3–5 minutes (vertical slice, not open world) |

---

## 1. TIMELINE & DEADLINES

| Date | Milestone |
|---|---|
| **30 Sep 2026 (Day 1)** | Ideation Lock + Tripo AI Asset Generation + Unity Project Setup |
| **1 Oct 2026 (Day 2)** | Environment Assembly + Lighting + Post-Processing + Skybox |
| **2 Oct 2026 (Day 3)** | Hand-Tracking Interactions + 3 Offerings Mechanics |
| **3 Oct 2026 (Day 4)** | Audio + Climax Sequence + Passthrough MR Transition |
| **4 Oct 2026 (Day 5)** | QA, Optimization, APK Build, Video Recording, Submission |
| **5 Oct 2026 23:59 AoE** | **SUBMISSION DEADLINE** (= 6 Oct 2026, 19:00 WIB) |

---

## 2. CORE CONCEPT & PHILOSOPHY

### 2.1 The Hook (Elevator Pitch)
> Humanity detects an extraterrestrial signal. Instead of building weapons, we build a gift.
> XENOASIS is a 4-minute VR sanctuary built on the PICO 4 Ultra where you — the last human curator — prepare three sacred offerings of Earth's most ephemeral wonders (water, sound, life) for an alien intelligence about to dock.
> Using bare-hand tracking, you sculpt floating water, awaken resonant crystals, and bloom alien-earth hybrid flora — then witness the cosmos respond.

### 2.2 Philosophical Subversion
- **NOT:** Alien invasion, combat, fear, military defense.
- **YES:** Cosmic hospitality, diplomatic empathy, sensory translation across species.
- The aliens don't understand language. The gift must be felt — through water, vibration, and organic life.

### 2.3 Core Metaphor
Water is the universal solvent, the origin molecule of life across the cosmos. It is the one substance aliens would recognize regardless of biology. We offer them our most precious memory: *the feeling of rain on skin, the sound of a river, the refraction of light through a droplet.*

### 2.4 Theme Fit Justification
The Tripothon theme says *"Build a world as a Gift — For the aliens about to arrive."*
XENOASIS is literally a world (a celestial sanctuary) built as a gift (three offerings) for aliens (extraterrestrial intelligence about to dock). Every interaction IS the act of gift-giving. The player doesn't fight; the player *offers*.

---

## 3. VISUAL AESTHETICS & ART DIRECTION

### 3.1 Style Keywords
`Bioluminescent Deep Void` · `Ethereal Solarpunk` · `Sacred Cosmic Architecture` · `Non-Euclidean Organic Geometry` · `Underwater-Space Hybrid`

### 3.2 Color Palette

| Role | Color | Hex | Usage |
|---|---|---|---|
| **Void Base** | Abyssal Navy Black | `#06070B` | Background, skybox, negative space |
| **Primary Glow** | Bioluminescent Cyan | `#00FFD1` | Interactive elements, flora glow, particles |
| **Accent Warm** | Liquid Gold / Amber | `#FFD166` | Offering pedestals, completion feedback |
| **Accent Cool** | Ethereal Lavender | `#B8A9E8` | Atmospheric haze, distant nebula |
| **Highlight** | Pearl White | `#F0EDE5` | Water droplets, crystal refraction |
| **Alert / Climax** | Solar Flare Yellow | `#F9CF00` (Tripo Acid) | Final beacon, climax explosion |

### 3.3 Lighting Setup (Unity URP)
- **Ambient:** Very dark, near-black. Scene is lit primarily by emissive objects.
- **Directional Light:** Extremely dim, cold blue (#1A1A3E), angled 45° from above. Simulates distant starlight.
- **Point Lights:** Placed inside every interactive object (cyan emission), intensity animated on interaction.
- **Post-Processing Stack:**
  - `Bloom` — Threshold 0.8, Intensity 2.5, Scatter 0.7 (makes every glow object radiate halos).
  - `Vignette` — Intensity 0.35 (frames the view, cinematic darkness at edges).
  - `Color Grading` — Lift shadows toward deep blue, Gain highlights toward warm gold.
  - `Chromatic Aberration` — Subtle 0.15 (adds lens realism).
  - `Film Grain` — Light grain for organic texture.
  - `Motion Blur` — Disabled (VR requirement to prevent nausea).

### 3.4 Spatial Audio Design
- **Ambient Layer:** Deep cosmic drone (sub-bass 40Hz hum), like being inside a whale's ribcage in space.
- **Water Layer:** Gentle droplet echoes, underwater resonance, tidal breathing rhythm.
- **Interaction SFX:** Crystalline chimes when touching objects, harmonic overtones that stack as offerings are completed.
- **Climax Audio:** All layers crescendo + a massive low-frequency vibration + ascending harmonic choir (synthesized).
- **Implementation:** Unity Audio Source with 3D Spatial Blend = 1.0 for positional sound. Reverb Zone for chamber echo.

### 3.5 Particle Systems
1. **Cosmic Stardust:** Thousands of tiny white/cyan emissive particles floating slowly in all directions (simulates being inside a nebula). Shader: Additive Blend.
2. **Water Droplets:** Spherical transparent particles with refraction, floating in zero gravity around the central basin.
3. **Completion Sparks:** Gold/amber burst particles triggered when each offering is activated.
4. **Climax Beacon:** Vertical column of intense yellow-white particles shooting upward from the chamber center.

---

## 4. SPATIAL ENVIRONMENT DESIGN

### 4.1 The Setting: "The Welcome Chamber"
A single, self-contained circular chamber suspended in deep space.

```
         ┌─────────────────────────────────┐
         │        COSMIC VOID / SKYBOX      │
         │     (stars, distant nebula)       │
         │                                   │
         │   ┌───────────────────────────┐   │
         │   │    THE WELCOME CHAMBER    │   │
         │   │                           │   │
         │   │  [Pillar]    [Pillar]     │   │
         │   │                           │   │
         │   │     ┌─── Offering 1 ───┐  │   │
         │   │     │  WATER BASIN     │  │   │
         │   │     └──────────────────┘  │   │
         │   │                           │   │
         │   │  [Offering 2]  [Offering 3]│  │
         │   │  CRYSTAL        FLORA     │   │
         │   │                           │   │
         │   │     ┌─── BEACON ───┐      │   │
         │   │     │  (Climax)    │      │   │
         │   │     └──────────────┘      │   │
         │   │                           │   │
         │   └───────────────────────────┘   │
         │                                   │
         └───────────────────────────────────┘
```

### 4.2 Chamber Architecture
- **Floor:** Semi-transparent dark obsidian glass with faint cyan vein patterns pulsing beneath the surface. Circular, ~12m diameter.
- **Walls:** None. Open to the void. The chamber is a floating platform/island in deep space.
- **Ceiling:** Implied by 4–6 tall twisted obsidian pillars that arc overhead, forming a skeletal dome/ribcage structure.
- **Edge:** The platform edge fades into cosmic mist. Looking down = infinite void with distant galaxies below.

### 4.3 Player Spawn & Orientation
- Player spawns at the edge of the chamber, facing inward toward the central basin.
- A gentle particle trail on the floor guides the player forward (subtle wayfinding without UI).
- No teleportation needed — the chamber is small enough (~12m) to walk around physically or with thumbstick locomotion.

---

## 5. THE THREE OFFERINGS (Core Interactions)

### 5.1 Offering 1: THE GIFT OF WATER (Central, Primary)

**Location:** Center of the chamber — a shallow ornate basin on a low pedestal.

**Visual:** A sapphire-and-gold basin filled with a hovering sphere of water (~60cm diameter) that defies gravity. The water sphere slowly rotates, refracting light and casting dancing caustics on the surrounding floor.

**Interaction (Hand-Tracking):**
1. Player approaches the basin. Water sphere pulses gently, inviting touch.
2. Player reaches bare hands INTO the water sphere (PICO hand-tracking detects palm entry).
3. The water responds: ripples emanate from the hand, droplets detach and orbit the player's fingers.
4. Player CUPS both hands beneath the sphere and slowly LIFTS — the water sphere rises and fragments into hundreds of individual droplets that hover around the chamber like a constellation of tiny liquid planets.
5. **Completion Feedback:** The droplets freeze into microscopic ice crystals that emit soft cyan light. A harmonic tone rings. The basin pedestal glows gold — Offering 1 is sealed.

**Emotional Beat:** Wonder, tenderness, the fragility of water as Earth's most precious element.

**Key Script Components:**
- `WaterSphereController.cs` — Manages the water sphere mesh deformation and particle emission on hand proximity.
- `HandProximityDetector.cs` — Uses PICO XR Hand Tracking API to detect palm position relative to the sphere collider.
- `OfferingManager.cs` — Global state manager tracking which of the 3 offerings are completed.

---

### 5.2 Offering 2: THE GIFT OF SOUND (Left Side)

**Location:** Left of center — a tall, faceted crystal (~1.2m) hovering above a dark plinth.

**Visual:** A complex teardrop-shaped crystal with fractal internal geometry. Dormant state: dim, translucent grey with faint internal facets visible. Active state: glowing cyan core with visible sound wave rings emanating outward.

**Interaction (Hand-Tracking):**
1. Player approaches the crystal. It hums at a barely audible low frequency.
2. Player places BOTH palms on either side of the crystal (not gripping — hovering ~5cm away, like warming hands by a fire).
3. The crystal responds to hand proximity: it begins vibrating, emitting audible tones.
4. Moving hands CLOSER = higher pitch, brighter glow. Moving hands FARTHER = lower pitch, dimmer.
5. Player "finds" the resonant sweet spot (a specific distance where the tone harmonizes perfectly). When found: the crystal SINGS — a cascade of overtones, the internal geometry lights up in a chain reaction.
6. **Completion Feedback:** The crystal emits a pulse of sound visible as a golden ripple wave. The plinth glows gold — Offering 2 is sealed.

**Emotional Beat:** Listening, patience, the intimacy of music as a language beyond words.

**Key Script Components:**
- `CrystalResonance.cs` — Maps hand distance (from PICO hand joint positions) to audio pitch (AudioSource.pitch) and emission intensity.
- `SoundWaveVisualizer.cs` — Spawns expanding ring meshes (torus) synchronized with audio peaks.

---

### 5.3 Offering 3: THE GIFT OF LIFE (Right Side)

**Location:** Right of center — a dormant organic form (~80cm) on a moss-covered stone pedestal.

**Visual:** An alien-earth hybrid flora. Dormant state: closed bud, dark grey-green, tendrils curled inward, no light. Active state: slowly unfurling petals of translucent glass-like tissue emitting bioluminescent cyan and gold light, with pollen-like particles rising from the stamen.

**Interaction (Hand-Tracking):**
1. Player approaches the dormant flora. Nothing happens unless the player initiates.
2. Player holds ONE OPEN PALM above the flora (like offering sunlight/energy downward).
3. A stream of warm gold light particles flows from the player's palm INTO the flora (visual feedback that the player is "giving energy").
4. The flora slowly responds: tendrils uncurl, a central bud swells, petals begin to separate.
5. Player must sustain the gesture for ~8 seconds. If hand is withdrawn too early, the flora pauses (but doesn't reset — forgiving design).
6. **Full Bloom:** The flora erupts into full bioluminescent bloom — petals spread wide, glowing filaments extend upward, pollen particles dance. A warm chord plays.
7. **Completion Feedback:** The pedestal glows gold — Offering 3 is sealed.

**Emotional Beat:** Nurturing, patience, the miracle of biological life awakening.

**Key Script Components:**
- `FloraBloom.cs` — Animated blend shapes or bone-based animation driving the bloom sequence, triggered by sustained hand presence above the collider.
- `EnergyBeamVFX.cs` — Particle system emitting from tracked palm position downward into the flora.

---

### 5.4 THE CLIMAX: The Welcoming Signal

**Trigger:** All 3 offering pedestals are glowing gold (OfferingManager detects all 3 complete).

**Sequence (30–45 seconds):**
1. **Beat 1 (0–10s):** The chamber rumbles. All ambient audio dips to silence. The three golden pedestals pulse in sync.
2. **Beat 2 (10–20s):** Golden light streams from each pedestal converge at the chamber's center — forming a vertical beam of pure white-gold light that shoots upward into the void.
3. **Beat 3 (20–35s):** The beacon intensifies. The cosmic stardust particles accelerate toward the beam. The skybox subtly shifts — a massive shape (shadow / silhouette) becomes visible in the distant stars above, slowly descending. The spatial audio swells: a deep harmonic chord (like a cathedral organ + whale song).
4. **Beat 4 (35–45s):** The light reaches maximum intensity. The player looks up. The alien presence is NOT shown explicitly — only suggested by a massive, warm, golden glow filling the sky above, and a feeling of reciprocal acknowledgment. The screen slowly whites out to a gentle, warm fade.
5. **End Screen:** Simple text on black: *"The gift was received."* — then the project title "XENOASIS" and credits.

**Emotional Beat:** Awe, catharsis, the profound relief of peaceful first contact.

---

## 6. PICO 4 ULTRA — TECHNICAL INTEGRATION

### 6.1 Key SDK Features Used

| Feature | PICO SDK Class/Method | Usage in XENOASIS |
|---|---|---|
| **Hand Tracking** | `PXR_HandTracking.GetJointLocations()` | Detect palm position, finger curl, hand proximity to interactive objects |
| **MR Passthrough** | `PXR_MixedReality.EnableVideoSeeThroughManual()` | Opening sequence: start in real room, dissolve into VR chamber |
| **Spatial Audio** | Unity AudioSource + PICO Spatial Audio SDK | 360° positional soundscapes |
| **Eye Tracking** (optional) | `PXR_EyeTracking` | Foveated rendering for performance optimization |

### 6.2 MR Passthrough Opening Sequence (Optional but High-Impact)
**If implemented (Day 4 stretch goal):**
1. Experience starts in **Passthrough mode** — user sees their real physical room.
2. Small cyan particle anomalies begin appearing in the real room (rendered on top of passthrough feed).
3. A crack of light appears on the floor. Water (VR particles) seeps up through the "real" floor.
4. Over 15 seconds, the passthrough feed fades to black, and the VR Welcome Chamber materializes around the player.

**If NOT implemented (fallback):**
- Experience starts directly in the VR chamber with a slow fade-in from black. Still impressive, saves 1 day of development.

### 6.3 Performance Targets (PICO 4 Ultra)
- **Target FPS:** 90 FPS (PICO 4 Ultra native refresh rate).
- **Triangle Budget:** < 500K triangles total scene.
- **Draw Calls:** < 100 (use GPU instancing, texture atlasing on Tripo assets).
- **Texture Resolution:** Max 2048x2048 per asset, prefer 1024x1024.
- **Shader Complexity:** URP/Lit or URP/Simple Lit only. Custom shaders for water (grab pass refraction) and emissive pulse only if performance allows.

---

## 7. TRIPO AI — ASSET GENERATION PIPELINE

### 7.1 Asset List & Prompts

| # | Asset Name | Category | Tripo AI Prompt |
|---|---|---|---|
| 1 | **Central Basin** | Architecture | `"A shallow ornate ceremonial basin carved from translucent dark sapphire stone, intertwined with bioluminescent golden coral veins, elegant sci-fi solarpunk design, clean geometry, PBR textures, game-ready 3D model"` |
| 2 | **Water Lotus** | Interactive Prop | `"An ethereal floating lotus flower made of liquid frosted glass with glowing blue fiber optic veins inside the petals, blooming state, symmetrical, isolated on black background, game asset"` |
| 3 | **Resonant Crystal** | Interactive Prop | `"A complex sacred geometry crystal in a faceted teardrop shape, glowing cyan energy core visible inside, ancient alien runic engravings on the surface, sci-fi artifact, studio lighting, game-ready"` |
| 4 | **Obsidian Pillars (x4)** | Architecture | `"A tall twisted column of polished obsidian stone with glowing cyan resin flowing through cracks, ancient alien temple architecture, dark elegant, smooth surfaces, PBR game asset"` |
| 5 | **Alien Flora (dormant)** | Interactive Prop | `"A closed alien plant bud with curled dark grey-green tendrils, organic alien xenobiology, sitting on a mossy stone base, dormant sleeping state, detailed texture, game asset"` |
| 6 | **Alien Flora (bloomed)** | Interactive Prop | `"A fully bloomed bioluminescent alien flower with translucent glass-like petals glowing cyan and gold, pollen particles rising, ethereal beautiful, isolated on black, game-ready"` |
| 7 | **Glass Chimes (x3)** | Decorative | `"Three delicate floating teardrop-shaped chimes made of frosted glass, each glowing softly with internal blue light, hanging from thin silver threads, isolated, game asset"` |
| 8 | **Offering Pedestal (x3)** | Architecture | `"A low hexagonal pedestal made of dark stone with gold inlay patterns, ancient alien altar, subtle glow from the edges, clean geometry, game-ready PBR model"` |
| 9 | **Floor Tile (modular)** | Architecture | `"A single hexagonal floor tile made of polished black obsidian glass with faint glowing cyan circuit-like veins beneath the surface, seamless tileable, game asset"` |

### 7.2 Post-Generation Workflow
1. Download all assets as `.glb` from Tripo AI.
2. Import into Unity (drag-and-drop into `Assets/Models/Tripo/`).
3. Verify materials: Re-assign URP/Lit shader if needed. Add emissive channel for glowing elements.
4. Scale and orient: Normalize all models to Unity metric scale (1 unit = 1 meter).
5. Add colliders: Box/Sphere colliders for interactive objects, Mesh collider for floor/basin.

### 7.3 World Labs (Marble API) — Spatial Environment Generation
World Labs' Marble API generates persistent, photorealistic 3D spatial worlds and radiance fields/panoramas.

- **Base Endpoint:** `https://api.worldlabs.ai/marble/v1/worlds:generate`
- **Authentication:** `WLT-Api-Key: <key>`
- **Export Targets:** 360 Panorama HDR for Skybox (`Assets/Textures/Skybox/CosmicVoid_HDR.exr`) + optional 3D Gaussian Splat (.ply/.spz).
- **Environment Prompt:**
  `"A vast celestial sanctuary floating in deep cosmic void. Circular obsidian dark glass platform suspended in infinite space surrounded by bioluminescent cyan stardust and swirling violet nebula. Ancient arched obsidian ribs forming an open cathedral dome overhead. Ethereal, awe-inspiring, non-euclidean alien architecture, solarpunk cosmic hospitality, hyper-detailed 3D environment."`

---

## 8. UNITY PROJECT STRUCTURE

```
Assets/
├── Scenes/
│   └── XenoasisMain.unity
├── Scripts/
│   ├── Core/
│   │   ├── GameManager.cs            // Overall game state, sequence control
│   │   └── OfferingManager.cs        // Tracks 3 offerings completion state
│   ├── Interaction/
│   │   ├── HandProximityDetector.cs   // PICO hand tracking proximity detection
│   │   ├── WaterSphereController.cs  // Offering 1: water sphere interaction
│   │   ├── CrystalResonance.cs       // Offering 2: crystal sound interaction
│   │   └── FloraBloom.cs             // Offering 3: flora bloom interaction
│   ├── VFX/
│   │   ├── EnergyBeamVFX.cs          // Palm-to-flora energy particle
│   │   ├── SoundWaveVisualizer.cs    // Crystal sound rings
│   │   ├── BeaconClimaxVFX.cs        // Final beacon light column
│   │   └── StardustController.cs     // Ambient floating particles
│   ├── Audio/
│   │   └── AudioManager.cs           // Ambient layers, interaction SFX, climax
│   └── Camera/
│       ├── PassthroughTransition.cs   // MR to VR fade (PICO passthrough)
│       └── FadeController.cs          // Screen fade in/out utility
├── Models/
│   └── Tripo/                         // All .glb files from Tripo AI
├── Materials/
│   ├── EmissiveCyan.mat
│   ├── EmissiveGold.mat
│   ├── ObsidianGlass.mat
│   └── WaterRefraction.mat
├── Audio/
│   ├── Ambient/
│   │   ├── cosmic_drone.wav
│   │   └── water_drops.wav
│   ├── SFX/
│   │   ├── crystal_tone_C.wav
│   │   ├── crystal_tone_E.wav
│   │   ├── crystal_tone_G.wav
│   │   ├── bloom_chord.wav
│   │   └── offering_complete.wav
│   └── Climax/
│       └── beacon_crescendo.wav
├── VFX/
│   ├── PS_Stardust.prefab
│   ├── PS_WaterDroplets.prefab
│   ├── PS_GoldSparks.prefab
│   ├── PS_EnergyBeam.prefab
│   └── PS_BeaconColumn.prefab
├── Textures/
│   └── Skybox/
│       └── CosmicVoid_HDR.exr
├── Prefabs/
│   ├── OfferingPedestal.prefab
│   ├── ObsidianPillar.prefab
│   └── GlassChime.prefab
└── Plugins/
    └── PICO/                          // PICO Unity Integration SDK
```

---

## 9. CORE SCRIPT ARCHITECTURE (C#)

### 9.1 OfferingManager.cs — Global State
```csharp
// Central state manager — singleton
// Tracks which offerings are complete
// When all 3 are done, triggers the Climax Beacon sequence

public enum OfferingState { Locked, Active, Complete }

// Events:
// OnOfferingComplete(int offeringIndex)
// OnAllOfferingsComplete() -> triggers BeaconClimaxVFX
```

### 9.2 HandProximityDetector.cs — PICO Hand Tracking
```csharp
// Attach to each interactive object
// Uses PICO PXR_HandTracking to get joint positions
// Calculates distance from palm center to this object's collider
// Exposes: float NormalizedProximity (0 = far, 1 = touching)
// Exposes: bool IsPalmFacingObject (dot product of palm normal vs object direction)
// Exposes: bool IsBothHandsPresent
```

### 9.3 Interaction Flow Pattern
```
Player Approaches -> HandProximityDetector fires ->
  -> Object-specific controller reads proximity/gesture ->
    -> Visual + Audio feedback scales with proximity ->
      -> Threshold reached -> Offering sealed ->
        -> OfferingManager.CompleteOffering(index) ->
          -> If all 3 complete -> Climax Sequence
```

---

## 10. MINUTE-BY-MINUTE JUDGES' WALKTHROUGH

### Minute 0:00 - 0:30 | THE INCEPTION
- **[0:00]** Black screen. A single low drone hum begins.
- **[0:05]** Fade in. The player stands at the edge of the Welcome Chamber. Stars surround them in every direction — above, below, to the sides. They are floating in deep space on a platform of dark glass.
- **[0:10]** Cosmic stardust particles drift slowly past. The obsidian pillars arc overhead like a skeletal cathedral. The ambient hum settles into a meditative rhythm.
- **[0:20]** A soft particle trail on the floor leads the player's gaze toward the center, where a glowing water sphere hovers above a basin. Two other objects glow dimly on either side.
- **[0:30]** The player begins walking forward, drawn by the water.

### Minute 0:30 - 1:30 | OFFERING 1: WATER
- Player reaches the basin. Reaches hands into the water sphere. Water ripples, droplets orbit fingers. Player cups and lifts — the sphere fragments into a constellation of hovering droplets. Ice crystals form. Pedestal glows gold. A chime rings.

### Minute 1:30 - 2:15 | OFFERING 2: CRYSTAL
- Player moves to the crystal. Holds palms on either side. Pitch and glow respond to distance. Player finds the resonant sweet spot — the crystal erupts in harmonic overtones and light. Pedestal glows gold.

### Minute 2:15 - 3:00 | OFFERING 3: FLORA
- Player moves to the dormant flora. Holds open palm above it, channeling energy. The bud slowly unfurls, petals spread, bioluminescence ignites. Full bloom. Pedestal glows gold.

### Minute 3:00 - 3:45 | THE CLIMAX
- Three golden beams converge at center. Vertical beacon of white-gold light erupts skyward. Stardust accelerates. Something vast and warm fills the sky above. Audio crescendo. Fade to white.
- **End card:** *"The gift was received."* — XENOASIS.

---

## 11. SUBMISSION DELIVERABLES CHECKLIST

### Required by Tripothon S1:
- [ ] **Playable Demo:** Android APK built for PICO 4 Ultra (signed, installable via SideQuest or adb).
- [ ] **Screen Recording / Walkthrough Video (1-2 min):** First-person VR recording showing all 3 offerings + climax. NOT a cinematic trailer — a genuine walkthrough.
- [ ] **Visual Asset Board:** 1-page poster showing key renders, color palette, Tripo AI prompts used, and PICO 4 Ultra screenshots.

### For Social Media Prize Pool:
- [ ] **X (Twitter) Post:** 30-60 second teaser clip. Tag `@TripoAI`. Hashtag `#Tripothon`.
- [ ] **Build Log Posts:** At least 2-3 progress posts during the 5-day build showing WIP screenshots and Tripo AI asset generation process.

### For "Best Use of PICO" Tool Track:
- [ ] Document/video showing PICO 4 Ultra hand-tracking in action (bare hands interacting with offerings).
- [ ] Mention PICO XR SDK integration in submission notes.

### For "Best Use of Tripo" Tool Track:
- [ ] Screenshot of Tripo AI dashboard showing generated assets.
- [ ] List all Tripo prompts used (already documented in Section 7).
- [ ] Show before/after: Tripo raw output vs Final in-engine render.

---

## 12. JUDGING CRITERIA OPTIMIZATION

### Direction Track Scoring:

| Criterion | Weight | How XENOASIS Maximizes It |
|---|---|---|
| **Creativity** | 30% | Subverts alien tropes entirely. No combat, no fear — pure cosmic empathy. Hand-tracking bare-hand interactions with water, crystal, and living flora feel unprecedented. |
| **Completeness** | 25% | 1 chamber, 3 interactions, 1 climax. Tight scope = zero bugs, 90 FPS, high polish. Every asset has post-processing glow. No placeholder textures. |
| **Theme Fit** | 20% | The entire experience IS the act of building a gift. Each interaction = one offering. The climax = the gift being received. 100% theme alignment. |
| **Viral Potential** | 15% | The water sphere fragmentation moment (hands reaching into floating water in VR) is a guaranteed viral clip. Bioluminescent aesthetics photograph beautifully. |
| **Commercial Value** | 10% | Repackageable as a standalone VR meditation/art experience. Demonstrates Tripo AI + PICO 4 Ultra commercial pipeline. |

### Tool Track Scoring (Tripo):

| Criterion | Weight | Strategy |
|---|---|---|
| Inventive use of tool | 35% | All 9+ environmental assets generated via Tripo AI text-to-3D. Zero hand-modeled geometry. |
| Tool synergy | 25% | Tripo .glb to Unity URP pipeline is seamless. Emissive materials enhance Tripo output. |
| Tool contribution | 20% | Without Tripo, this project would take 3 weeks of Blender work. Tripo made it possible in 5 days. |
| Theme fit | 10% | Every Tripo-generated asset serves the "gift" narrative directly. |
| Breakout potential | 10% | Demonstrates that Tripo AI can power full VR productions, not just static renders. |

### Tool Track Scoring (World Labs):

| Criterion | Weight | Strategy |
|---|---|---|
| Inventive use of tool | 35% | Generates the entire deep space cosmic void & Welcome Chamber environment via Marble Large World Model. |
| Tool synergy | 25% | Direct export to HDR 360 panorama & radiance fields seamlessly integrated into Unity URP skybox. |
| Tool contribution | 20% | Provides expansive cosmic scale, astronomical lighting, and spatial atmosphere impossible with standard skyboxes. |
| Theme fit | 10% | The sanctuary itself is the grand spatial world built as the foundation for the gift. |
| Breakout potential | 10% | Showcases World Labs' Marble platform powering immersive VR headset worlds on PICO 4 Ultra. |

### Tool Track Scoring (PICO):

| Criterion | Weight | Strategy |
|---|---|---|
| Inventive use of tool | 35% | Bare-hand tracking as the ONLY input method. No controllers. Hands ARE the gift-giving instrument. |
| Tool synergy | 25% | PICO 4 Ultra optical hand tracking + passthrough MR are core to the experience, not bolted on. |
| Tool contribution | 20% | This experience is impossible on any non-PICO headset without hand tracking + passthrough. |
| Theme fit | 10% | Using bare hands to offer gifts = thematically perfect for "build a world as a gift." |
| Breakout potential | 10% | Showcases PICO 4 Ultra as a premium creative/art platform, not just a gaming device. |

---

## 13. VIRAL MOMENT STRATEGY

### The 15-Second Clip (for X / TikTok / Instagram):
**Shot:** Close-up of bare hands reaching into a floating sphere of shimmering water in deep space. Water ripples. Droplets orbit the fingers. Hands cup and lift. The sphere fragments into a hundred tiny floating ice crystals that glow cyan. Camera pulls back to reveal the cosmic chamber.

**Caption:** *"We didn't build weapons. We built a gift. #Tripothon @TripoAI"*

**Why it works:** Water-in-VR with bare hands is visually stunning, immediately understandable without context, and emotionally resonant. The bioluminescent aesthetic is inherently shareable.

---

## 14. AUDIO ASSET LIST (Free / CC0 Sources)

| Asset | Description | Suggested Source |
|---|---|---|
| `cosmic_drone.wav` | Deep sub-bass ambient hum, 40-60Hz | Freesound.org / Generate with Audacity |
| `water_drops.wav` | Gentle water droplet echoes, reverbed | Freesound.org CC0 |
| `crystal_tone_C.wav` | Singing bowl / glass harmonica, note C | Freesound.org CC0 |
| `crystal_tone_E.wav` | Singing bowl / glass harmonica, note E | Freesound.org CC0 |
| `crystal_tone_G.wav` | Singing bowl / glass harmonica, note G | Freesound.org CC0 |
| `bloom_chord.wav` | Warm organic chord, strings + synth pad | Freesound.org CC0 |
| `offering_complete.wav` | Bright chime + golden bell | Freesound.org CC0 |
| `beacon_crescendo.wav` | 30s building crescendo, organ + choir + sub | Freesound.org CC0 / Custom mix |

---

## 15. RISK MITIGATION & FALLBACKS

| Risk | Mitigation |
|---|---|
| Tripo AI assets have bad topology / too many tris | Use Blender Decimate modifier (quick) or Unity LOD. Budget: <50K tris per hero asset. |
| PICO hand tracking jittery / unreliable | Add generous collider zones (30cm radius). Use smoothed joint positions (lerp). Don't require precise finger poses — palm proximity is enough. |
| MR Passthrough transition too complex for Day 4 | Cut it. Start directly in VR chamber with fade-from-black. Still impressive. |
| Performance drops below 90 FPS | Reduce particle counts first. Then reduce texture resolution. Then bake lighting (Lightmapper). |
| Audio assets not ready in time | Use a single ambient loop + Unity built-in AudioSource pitch shifting for crystal interaction. |
| Submission upload fails near deadline | Build APK on Day 4 evening. Record video on Day 4 evening. Submit early on Day 5 morning. Buffer = 24+ hours. |

---

## 16. CONTACT & SUBMISSION

| Field | Value |
|---|---|
| **Registration / Start Building** | https://tripo-ambassador.typeform.com/to/ScfyutRF?typeform-lang=en |
| **Submission Portal** | https://developers.tripo3d.ai/en/events/tripothon-s1 (Login -> Submit / View Project) |
| **Community Discord** | https://discord.com/invite/NEdkyQQ3VP |
| **Organizer Email** | tripothon01@vastai3d.com |
| **Deadline** | Oct 5, 2026, 23:59 AoE (UTC-12) = **Oct 6, 2026, 19:00 WIB** |

---

*"Your world is the next gift."*
**XENOASIS — Build a world as a Gift, for the aliens about to arrive.**
