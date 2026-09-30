#!/usr/bin/env python3
"""
XENOASIS — Unified Tool Track Pipeline (Tripo AI + World Labs)
Syncs and generates all 3D assets and world environments for the Tripothon project.
"""

import os
import sys
import argparse
from pathlib import Path

# Load .env file if present
def load_env():
    env_file = Path(".env")
    if env_file.exists():
        with open(env_file, "r") as f:
            for line in f:
                line = line.strip()
                if line and not line.startswith("#") and "=" in line:
                    key, val = line.split("=", 1)
                    os.environ[key.strip()] = val.strip()

def main():
    parser = argparse.ArgumentParser(description="Unified Asset Pipeline for XENOASIS (Tripothon S1)")
    parser.add_argument("--dry-run", action="store_true", help="Run in dry-run mode without charging API credits")
    parser.add_argument("--skip-worldlabs", action="store_true", help="Skip World Labs environment generation")
    parser.add_argument("--skip-tripo", action="store_true", help="Skip Tripo AI asset generation")
    args = parser.parse_args()

    load_env()

    tripo_key = os.getenv("TRIPO_API_KEY")
    wl_key = os.getenv("WORLDLABS_API_KEY")

    print("================================================================")
    print("           XENOASIS — Tripothon Tool Pipeline Sync             ")
    print("================================================================")
    print(f"  Tripo API Key:      {'[CONFIGURED]' if tripo_key else '[NOT SET (use --dry-run or set TRIPO_API_KEY)]'}")
    print(f"  World Labs API Key: {'[CONFIGURED]' if wl_key else '[NOT SET (use --dry-run or set WORLDLABS_API_KEY)]'}")
    print(f"  Dry Run:            {args.dry_run}")
    print("----------------------------------------------------------------\n")

    # 1. World Labs
    if not args.skip_worldlabs:
        print("[STEP 1/2] World Labs: Generating Celestial Void Sanctuary...")
        from worldlabs_api import WorldLabsClient, DEFAULT_WORLD_PROMPT
        wl_client = WorldLabsClient(dry_run=args.dry_run)
        try:
            op = wl_client.generate_world(prompt=DEFAULT_WORLD_PROMPT)
            res = wl_client.poll_operation(op.get("operation_id"))
            world_id = res.get("world_id")
            if world_id:
                dl_url = wl_client.export_world(world_id, "panorama")
                if dl_url:
                    out_path = Path("Assets/Textures/Skybox/CosmicVoid_HDR.exr")
                    wl_client.download_file(dl_url, out_path)
        except Exception as e:
            print(f"[!] World Labs Generation Error: {e}")

    # 2. Tripo AI
    if not args.skip_tripo:
        print("\n[STEP 2/2] Tripo AI: Generating 9 Sacred Artifacts...")
        from tripo_api import TripoClient, ASSET_PROMPTS
        tripo_client = TripoClient(dry_run=args.dry_run)
        out_dir = Path("Assets/Models/Tripo")
        for name, prompt in ASSET_PROMPTS.items():
            print(f"\n  -> Artifact: {name}")
            out_file = out_dir / f"{name}.glb"
            try:
                task = tripo_client.create_text_to_model(prompt)
                task_id = task.get("data", {}).get("task_id")
                model_url = tripo_client.poll_until_complete(task_id)
                if model_url:
                    tripo_client.download_glb(model_url, out_file)
            except Exception as e:
                print(f"[!] Tripo Generation Error for {name}: {e}")

    print("\n================================================================")
    print("  Pipeline sync finished!")
    print("================================================================")

if __name__ == "__main__":
    main()
