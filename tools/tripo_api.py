#!/usr/bin/env python3
"""
XENOASIS — Tripo AI API Automation Tool
Connects to Tripo3D API v3 (https://openapi.tripo3d.ai/v3)
Automates Text-to-3D generation, polling, and downloading of GLB assets into Assets/Models/Tripo/.
"""

import os
import sys
import time
import json
import argparse
import requests
from pathlib import Path

BASE_URL = "https://openapi.tripo3d.ai/v3"

# Asset registry from XENOASIS_PROJECT_BIBLE.md
ASSET_PROMPTS = {
    "central_basin": "A shallow ornate ceremonial basin carved from translucent dark sapphire stone, intertwined with bioluminescent golden coral veins, elegant sci-fi solarpunk design, clean geometry, PBR textures, game-ready 3D model",
    "water_lotus": "An ethereal floating lotus flower made of liquid frosted glass with glowing blue fiber optic veins inside the petals, blooming state, symmetrical, isolated on black background, game asset",
    "resonant_crystal": "A complex sacred geometry crystal in a faceted teardrop shape, glowing cyan energy core visible inside, ancient alien runic engravings on the surface, sci-fi artifact, studio lighting, game-ready",
    "obsidian_pillar": "A tall twisted column of polished obsidian stone with glowing cyan resin flowing through cracks, ancient alien temple architecture, dark elegant, smooth surfaces, PBR game asset",
    "alien_flora_dormant": "A closed alien plant bud with curled dark grey-green tendrils, organic alien xenobiology, sitting on a mossy stone base, dormant sleeping state, detailed texture, game asset",
    "alien_flora_bloomed": "A fully bloomed bioluminescent alien flower with translucent glass-like petals glowing cyan and gold, pollen particles rising, ethereal beautiful, isolated on black, game-ready",
    "glass_chime": "Three delicate floating teardrop-shaped chimes made of frosted glass, each glowing softly with internal blue light, hanging from thin silver threads, isolated, game asset",
    "offering_pedestal": "A low hexagonal pedestal made of dark stone with gold inlay patterns, ancient alien altar, subtle glow from the edges, clean geometry, game-ready PBR model",
    "floor_tile": "A single hexagonal floor tile made of polished black obsidian glass with faint glowing cyan circuit-like veins beneath the surface, seamless tileable, game asset"
}

def load_env():
    env_file = Path(".env")
    if env_file.exists():
        with open(env_file, "r") as f:
            for line in f:
                line = line.strip()
                if line and not line.startswith("#") and "=" in line:
                    key, val = line.split("=", 1)
                    os.environ.setdefault(key.strip(), val.strip())

class TripoClient:
    def __init__(self, api_key: str = None, dry_run: bool = False):
        load_env()
        self.api_key = api_key or os.getenv("TRIPO_API_KEY", "")
        self.dry_run = dry_run
        self.headers = {
            "Authorization": f"Bearer {self.api_key}",
            "Content-Type": "application/json"
        }

    def create_text_to_model(self, prompt: str, model_version: str = "v3.1-20260211") -> dict:
        """Submits a text-to-model task to Tripo AI."""
        if self.dry_run:
            print(f"[DRY-RUN] POST {BASE_URL}/generation/text-to-model")
            print(f"[DRY-RUN] Prompt: {prompt[:60]}...")
            return {"code": 0, "data": {"task_id": "dry_run_task_12345"}}

        if not self.api_key:
            raise ValueError("TRIPO_API_KEY not found. Set it via env or --api-key argument.")

        url = f"{BASE_URL}/generation/text-to-model"
        payload = {
            "prompt": prompt,
            "model": model_version,
            "texture": True,
            "pbr": True,
            "texture_quality": "detailed"
        }
        res = requests.post(url, headers=self.headers, json=payload, timeout=30)
        res.raise_for_status()
        return res.json()

    def get_task_status(self, task_id: str) -> dict:
        """Polls task status until finished."""
        if self.dry_run:
            return {
                "code": 0,
                "data": {
                    "task_id": task_id,
                    "status": "success",
                    "output": {"model_url": "https://example.com/dry_run_model.glb"}
                }
            }

        url = f"{BASE_URL}/tasks/{task_id}"
        res = requests.get(url, headers=self.headers, timeout=30)
        res.raise_for_status()
        return res.json()

    def poll_until_complete(self, task_id: str, poll_interval: int = 5, max_wait: int = 300) -> str:
        """Polls task and returns downloaded model URL when ready."""
        print(f"[*] Polling task {task_id}...")
        start_time = time.time()
        while time.time() - start_time < max_wait:
            data = self.get_task_status(task_id)
            task_data = data.get("data", {})
            status = task_data.get("status")

            if status == "success":
                model_url = task_data.get("output", {}).get("model_url")
                print(f"[+] Task {task_id} completed successfully!")
                return model_url
            elif status in ("failed", "cancelled"):
                raise RuntimeError(f"Task {task_id} failed with status: {status}")

            progress = task_data.get("progress", 0)
            print(f"[-] Status: {status} ({progress}%)... waiting {poll_interval}s")
            time.sleep(poll_interval)

        raise TimeoutError(f"Task {task_id} timed out after {max_wait}s.")

    def download_glb(self, url: str, output_path: Path):
        """Downloads the GLB file to the target local path."""
        output_path.parent.mkdir(parents=True, exist_ok=True)
        if self.dry_run:
            print(f"[DRY-RUN] Saved model to {output_path}")
            return

        print(f"[*] Downloading {url} -> {output_path}...")
        res = requests.get(url, stream=True, timeout=60)
        res.raise_for_status()
        with open(output_path, "wb") as f:
            for chunk in res.iter_content(chunk_size=8192):
                f.write(chunk)
        print(f"[+] Successfully saved {output_path} ({output_path.stat().st_size} bytes)")


def main():
    parser = argparse.ArgumentParser(description="Tripo AI 3D Generator for XENOASIS")
    parser.add_argument("--api-key", help="Tripo API Key (or set TRIPO_API_KEY env)")
    parser.add_argument("--asset", choices=list(ASSET_PROMPTS.keys()) + ["all"], default="all",
                        help="Specific asset to generate or 'all'")
    parser.add_argument("--output-dir", default="Assets/Models/Tripo", help="Target output folder")
    parser.add_argument("--dry-run", action="store_true", help="Simulate generation without calling API")
    args = parser.parse_args()

    client = TripoClient(api_key=args.api_key, dry_run=args.dry_run)
    out_dir = Path(args.output_dir)

    targets = ASSET_PROMPTS if args.asset == "all" else {args.asset: ASSET_PROMPTS[args.asset]}

    print(f"==================================================")
    print(f"  XENOASIS — Tripo AI 3D Pipeline Generator")
    print(f"  Target Assets: {len(targets)} | Dry Run: {args.dry_run}")
    print(f"==================================================")

    for name, prompt in targets.items():
        print(f"\n>> Processing: {name}")
        out_file = out_dir / f"{name}.glb"
        try:
            task_res = client.create_text_to_model(prompt)
            task_id = task_res.get("data", {}).get("task_id")
            model_url = client.poll_until_complete(task_id)
            if model_url:
                client.download_glb(model_url, out_file)
        except Exception as e:
            print(f"[!] Error generating {name}: {e}")

    print("\n[+] Done.")


if __name__ == "__main__":
    main()
