#!/usr/bin/env python3
"""
XENOASIS — Procedural Spatial Audio Synthesizer
Generates 16-bit 44.1kHz PCM stereo/mono WAV soundscapes using pure Python (math + wave).
Produces ambient cosmic drone, water reverberations, crystal singing bowls, flora bloom chord,
offering bells, and the 35-second climax beacon crescendo.
"""

import math
import struct
import wave
from pathlib import Path

SAMPLE_RATE = 44100

def write_wav(filepath: Path, samples: list[float], channels: int = 1):
    """Writes a normalized float sample list [-1.0, 1.0] to a 16-bit PCM WAV."""
    filepath.parent.mkdir(parents=True, exist_ok=True)
    # Normalize
    max_val = max(abs(s) for s in samples) or 1.0
    norm_factor = 0.95 / max_val

    with wave.open(str(filepath), 'w') as wav:
        wav.setnchannels(channels)
        wav.setsampwidth(2) # 16-bit
        wav.setframerate(SAMPLE_RATE)
        frames = bytearray()
        for s in samples:
            val = int(max(-1.0, min(1.0, s * norm_factor)) * 32767)
            frames.extend(struct.pack('<h', val))
        wav.writeframes(frames)
    print(f"[+] Generated: {filepath} ({len(samples)/SAMPLE_RATE:.1f}s)")

def gen_cosmic_drone(duration: float = 12.0) -> list[float]:
    """Deep sub-bass hum (45Hz + 60Hz + 90Hz) with slow binaural breathing."""
    num_samples = int(duration * SAMPLE_RATE)
    samples = []
    for i in range(num_samples):
        t = i / SAMPLE_RATE
        # Slow 0.2Hz tidal modulation (like breathing in space)
        breath = 0.8 + 0.2 * math.sin(2 * math.pi * 0.15 * t)
        # Harmonics
        f1 = math.sin(2 * math.pi * 45.0 * t) * 0.6
        f2 = math.sin(2 * math.pi * 90.0 * t) * 0.3
        f3 = math.sin(2 * math.pi * 135.0 * t) * 0.15
        # Soft warmth
        sub = math.sin(2 * math.pi * 30.0 * t) * 0.4
        sample = (f1 + f2 + f3 + sub) * breath
        # Smooth loop fade
        fade = min(1.0, t / 0.5) * min(1.0, (duration - t) / 0.5)
        samples.append(sample * fade)
    return samples

def gen_water_drops(duration: float = 8.0) -> list[float]:
    """Scattered zero-g water droplets echoing in space."""
    num_samples = int(duration * SAMPLE_RATE)
    samples = [0.0] * num_samples
    drop_times = [0.3, 1.2, 2.5, 3.8, 5.0, 6.2, 7.1]
    drop_freqs = [820, 1100, 950, 1350, 780, 1220, 990]

    for start_t, base_freq in zip(drop_times, drop_freqs):
        start_idx = int(start_t * SAMPLE_RATE)
        drop_len = int(0.6 * SAMPLE_RATE)
        for i in range(drop_len):
            if start_idx + i >= num_samples:
                break
            t = i / SAMPLE_RATE
            # Chirp frequency downward slightly as droplet forms
            freq = base_freq * (1.0 - 0.3 * (t / 0.6))
            env = math.exp(-12.0 * t) # fast exponential decay
            reverb = 0.25 * math.exp(-3.0 * t) * math.sin(2 * math.pi * freq * 0.5 * t)
            sample = (math.sin(2 * math.pi * freq * t) * env) + reverb
            samples[start_idx + i] += sample * 0.7
    return samples

def gen_crystal_tone(freq: float, duration: float = 4.0) -> list[float]:
    """Crystalline singing bowl tone with pure harmonics and natural decay."""
    num_samples = int(duration * SAMPLE_RATE)
    samples = []
    for i in range(num_samples):
        t = i / SAMPLE_RATE
        env = math.exp(-1.2 * t)
        vibrato = 1.0 + 0.003 * math.sin(2 * math.pi * 5.0 * t)
        f1 = math.sin(2 * math.pi * freq * vibrato * t) * 0.7
        f2 = math.sin(2 * math.pi * (freq * 2.76) * t) * 0.25 # overtone
        f3 = math.sin(2 * math.pi * (freq * 5.4) * t) * 0.08  # glass sparkle
        samples.append((f1 + f2 + f3) * env)
    return samples

def gen_bloom_chord(duration: float = 6.0) -> list[float]:
    """Lush, warm celestial chord (F# Maj9 - mystical and uplifting)."""
    num_samples = int(duration * SAMPLE_RATE)
    samples = []
    # F#3, A#3, C#4, F4, G#4
    notes = [185.0, 233.08, 277.18, 349.23, 415.30]
    for i in range(num_samples):
        t = i / SAMPLE_RATE
        # Attack and release envelope (slow bloom swell)
        attack = min(1.0, t / 1.5)
        decay = math.exp(-0.45 * max(0.0, t - 1.5))
        env = attack * decay
        sample = sum(math.sin(2 * math.pi * n * t) for n in notes) / len(notes)
        # Add subtle shimmer
        shimmer = 0.15 * math.sin(2 * math.pi * (notes[-1] * 2) * t)
        samples.append((sample + shimmer) * env)
    return samples

def gen_offering_complete(duration: float = 3.5) -> list[float]:
    """Bright ascending celestial bell chime."""
    num_samples = int(duration * SAMPLE_RATE)
    samples = [0.0] * num_samples
    # C5, E5, G5, B5, D6 arpeggio
    notes = [523.25, 659.25, 783.99, 987.77, 1174.66]
    for idx, n in enumerate(notes):
        start_idx = int(idx * 0.12 * SAMPLE_RATE)
        for i in range(int(2.5 * SAMPLE_RATE)):
            if start_idx + i >= num_samples: break
            t = i / SAMPLE_RATE
            env = math.exp(-3.5 * t)
            s = math.sin(2 * math.pi * n * t) * env
            samples[start_idx + i] += s * 0.4
    return samples

def gen_beacon_crescendo(duration: float = 35.0) -> list[float]:
    """The 35-second epic climax crescendo building to white-light climax."""
    num_samples = int(duration * SAMPLE_RATE)
    samples = []
    base_freqs = [65.4, 98.0, 130.8, 196.0] # C2, G2, C3, G3 (cathedral root)

    for i in range(num_samples):
        t = i / SAMPLE_RATE
        progress = t / duration # 0 to 1

        # Exponential intensity growth
        vol = (progress ** 1.8) * 0.95 + 0.05

        # Cathedral drone base
        drone = sum(math.sin(2 * math.pi * f * t) for f in base_freqs) * 0.25

        # Ascending celestial harmonic sweep
        sweep_freq = 200.0 + 600.0 * (progress ** 2.2)
        sweep = math.sin(2 * math.pi * sweep_freq * t) * (progress * 0.35)

        # Pulsing energy heart (accelerating heartbeat/beacon throb)
        throb_rate = 1.0 + 5.0 * progress
        throb = 0.5 + 0.5 * math.sin(2 * math.pi * throb_rate * t)

        # Grand choir chord fading in during final 15 seconds
        choir = 0.0
        if t > 18.0:
            c_prog = (t - 18.0) / 17.0
            choir_notes = [261.63, 329.63, 392.00, 523.25, 659.25]
            choir = sum(math.sin(2 * math.pi * cn * t) for cn in choir_notes) * 0.12 * c_prog

        sample = (drone + sweep * throb + choir) * vol

        # Final second fade to pure silence before white screen
        if t > duration - 1.5:
            fade_out = (duration - t) / 1.5
            sample *= fade_out

        samples.append(sample)
    return samples

def main():
    print("==================================================")
    print("  XENOASIS — Procedural Soundscape Generator")
    print("==================================================")

    audio_dir = Path("Assets/Audio")

    # 1. Ambient
    write_wav(audio_dir / "Ambient/cosmic_drone.wav", gen_cosmic_drone(12.0))
    write_wav(audio_dir / "Ambient/water_drops.wav", gen_water_drops(8.0))

    # 2. SFX
    write_wav(audio_dir / "SFX/crystal_tone_C.wav", gen_crystal_tone(261.63, 4.0))
    write_wav(audio_dir / "SFX/crystal_tone_E.wav", gen_crystal_tone(329.63, 4.0))
    write_wav(audio_dir / "SFX/crystal_tone_G.wav", gen_crystal_tone(392.00, 4.0))
    write_wav(audio_dir / "SFX/bloom_chord.wav", gen_bloom_chord(6.0))
    write_wav(audio_dir / "SFX/offering_complete.wav", gen_offering_complete(3.5))

    # 3. Climax
    write_wav(audio_dir / "Climax/beacon_crescendo.wav", gen_beacon_crescendo(35.0))

    print("\n[+] All spatial audio assets successfully synthesized!")

if __name__ == "__main__":
    main()
