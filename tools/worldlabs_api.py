#!/usr/bin/env python3
"""
XENOASIS — World Labs (Marble API) Automation Tool
Connects to World Labs Marble API v1 (https://api.worldlabs.ai/marble/v1/)
Generates 3D Worlds / Environments / Panoramas / Gaussian Splats for the Cosmic Sanctuary.
"""

import os
import sys
import time
import json
import argparse
import requests
from pathlib import Path

BASE_URL = "https://api.worldlabs.ai/marble/v1"

# Environment World Prompt for XENOASIS
DEFAULT_WORLD_PROMPT = (
    "A vast celestial sanctuary floating in deep cosmic void. Circular obsidian dark glass platform "
    "suspended in infinite space surrounded by bioluminescent cyan stardust and swirling violet nebula. "
    "Ancient arched obsidian ribs forming an open cathedral dome overhead. Ethereal, awe-inspiring, "
    "non-euclidean alien architecture, solarpunk cosmic hospitality, hyper-detailed 3D environment."
)

class WorldLabsClient:
    def __init__(self, api_key: str = None, dry_run: bool = False):
        self.api_key = api_key or os.getenv("WORLDLABS_API_KEY", "")
        self.dry_run = dry_run
        self.headers = {
            "WLT-Api-Key": self.api_key,
            "Content-Type": "application/json"
        }

    def generate_world(self, prompt: str = DEFAULT_WORLD_PROMPT, model: str = "marble-1.1") -> dict:
        """Sends a world generation request to World Labs."""
        if self.dry_run:
            print(f"[DRY-RUN] POST {BASE_URL}/worlds:generate")
            print(f"[DRY-RUN] Prompt: {prompt[:80]}...")
            return {"operation_id": "dry_run_op_wl_998877"}

        if not self.api_key:
            raise ValueError("WORLDLABS_API_KEY not found. Set it via env or --api-key argument.")

        url = f"{BASE_URL}/worlds:generate"
        payload = {
            "prompt": prompt,
            "model": model
        }
        res = requests.post(url, headers=self.headers, json=payload, timeout=30)
        res.raise_for_status()
        return res.json()

    def poll_operation(self, operation_id: str, poll_interval: int = 10, max_wait: int = 600) -> dict:
        """Polls long-running operation until complete."""
        if self.dry_run:
            return {
                "done": True,
                "response": {
                    "world_id": "world_xenoasis_sanctuary_01",
                    "preview_url": "https://example.com/dry_run_sanctuary_preview.png"
                }
            }

        url = f"{BASE_URL}/operations/{operation_id}"
        print(f"[*] Polling operation {operation_id}...")
        start_time = time.time()
        while time.time() - start_time < max_wait:
            res = requests.get(url, headers=self.headers, timeout=30)
            res.raise_for_status()
            data = res.json()

            if data.get("done", False):
                print(f"[+] Operation {operation_id} completed!")
                return data.get("response", {})

            print(f"[-] Generating world... waiting {poll_interval}s")
            time.sleep(poll_interval)

        raise TimeoutError(f"Operation {operation_id} timed out after {max_wait}s.")

    def export_world(self, world_id: str, export_format: str = "panorama") -> str:
        """Requests export of the world (e.g. panorama/skybox, mesh, gaussian_splat)."""
        if self.dry_run:
            print(f"[DRY-RUN] POST {BASE_URL}/worlds/{world_id}:export format={export_format}")
            return f"https://example.com/export_{export_format}.exr"

        url = f"{BASE_URL}/worlds/{world_id}:export"
        payload = {"format": export_format}
        res = requests.post(url, headers=self.headers, json=payload, timeout=30)
        res.raise_for_status()
        data = res.json()
        return data.get("download_url", "")

    def download_file(self, url: str, output_path: Path):
        """Downloads exported environment asset to local path."""
        output_path.parent.mkdir(parents=True, exist_ok=True)
        if self.dry_run:
            print(f"[DRY-RUN] Saved environment file to {output_path}")
            return

        print(f"[*] Downloading environment asset -> {output_path}...")
        res = requests.get(url, stream=True, timeout=120)
        res.raise_for_status()
        with open(output_path, "wb") as f:
            for chunk in res.iter_content(chunk_size=8192):
                f.write(chunk)
        print(f"[+] Successfully saved {output_path} ({output_path.stat().st_size} bytes)")


def main():
    parser = argparse.ArgumentParser(description="World Labs Marble API Generator for XENOASIS")
    parser.add_argument("--api-key", help="World Labs API Key (or set WORLDLABS_API_KEY env)")
    parser.add_argument("--prompt", default=DEFAULT_WORLD_PROMPT, help="Prompt for 3D world")
    parser.add_argument("--format", choices=["panorama", "mesh", "gaussian_splat"], default="panorama",
                        help="Export format for Unity environment")
    parser.add_argument("--output-dir", default="Assets/Textures/Skybox", help="Destination folder")
    parser.add_argument("--dry-run", action="store_true", help="Simulate generation without calling API")
    args = parser.parse_args()

    client = WorldLabsClient(api_key=args.api_key, dry_run=args.dry_run)
    out_dir = Path(args.output_dir)

    print(f"==================================================")
    print(f"  XENOASIS — World Labs Marble API Generator")
    print(f"  Target Format: {args.format} | Dry Run: {args.dry_run}")
    print(f"==================================================")

    try:
        op = client.generate_world(prompt=args.prompt)
        op_id = op.get("operation_id")
        result = client.poll_operation(op_id)
        world_id = result.get("world_id")

        if world_id:
            ext = ".exr" if args.format == "panorama" else (".glb" if args.format == "mesh" else ".ply")
            out_file = out_dir / f"CosmicVoid_{args.format}{ext}"
            download_url = client.export_world(world_id, export_format=args.format)
            if download_url:
                client.download_file(download_url, out_file)
    except Exception as e:
        print(f"[!] Error generating world: {e}")

    print("\n[+] Done.")


if __name__ == "__main__":
    main()
