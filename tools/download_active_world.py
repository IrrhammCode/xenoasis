#!/usr/bin/env python3
"""
Downloads the actively generating World Labs celestial sanctuary once completed.
Operation: 46ddb0a7-3601-4a32-8610-f526be985fb1
World ID: 5060bcee-5094-4022-b495-3cd1ad024003
"""

import os
import time
import requests
from pathlib import Path

def load_env():
    env_file = Path(".env")
    if env_file.exists():
        with open(env_file, "r") as f:
            for line in f:
                line = line.strip()
                if line and not line.startswith("#") and "=" in line:
                    k, v = line.split("=", 1)
                    os.environ.setdefault(k.strip(), v.strip())

load_env()
api_key = os.getenv("WORLDLABS_API_KEY")
headers = {"WLT-Api-Key": api_key}
op_id = "46ddb0a7-3601-4a32-8610-f526be985fb1"

print(f"[*] Checking active World Labs generation (Op: {op_id})...")
res = requests.get(f"https://api.worldlabs.ai/marble/v1/operations/{op_id}", headers=headers)
data = res.json()
print("Status:", data.get("metadata", {}).get("progress", {}))

if data.get("done", False):
    world_id = data.get("metadata", {}).get("world_id") or "5060bcee-5094-4022-b495-3cd1ad024003"
    print(f"[+] World is ready! World ID: {world_id}")
    world_res = requests.get(f"https://api.worldlabs.ai/marble/v1/worlds/{world_id}", headers=headers)
    world_info = world_res.json()
    assets = world_info.get("assets", {})
    pano_url = assets.get("imagery", {}).get("pano_url")

    if pano_url:
        print(f"[*] Downloading 360 Panorama -> Assets/Textures/Skybox/CosmicVoid_Pano.png")
        out_file = Path("Assets/Textures/Skybox/CosmicVoid_Pano.png")
        out_file.parent.mkdir(parents=True, exist_ok=True)
        img_res = requests.get(pano_url)
        with open(out_file, "wb") as f:
            f.write(img_res.content)
        print(f"[+] Saved {out_file} ({out_file.stat().st_size} bytes)")
    else:
        print("[!] No pano_url found in assets yet.")
else:
    print("[-] World generation is still processing on World Labs servers.")
