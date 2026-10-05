#!/usr/bin/env python3
"""
XENOASIS — Generate Tripo AI Museum Architecture Assets
Dispatches 4 key museum architectural assets to Tripo API v3.1 concurrently,
polls their status, and downloads the resulting GLBs to Assets/Models/Tripo/.
"""

import os
import sys
import time
import requests
from pathlib import Path

BASE_URL = "https://openapi.tripo3d.ai/v3"

def load_api_key():
    env_file = Path(".env")
    if env_file.exists():
        with open(env_file, "r") as f:
            for line in f:
                if line.startswith("TRIPO_API_KEY="):
                    return line.split("=", 1)[1].strip()
    return os.environ.get("TRIPO_API_KEY", "")

PROMPTS = {
    "museum_ceiling_chandelier": "A massive futuristic sci-fi museum ceiling chandelier and celestial ring light sculpture, concentric rings of dark titanium and warm gold luminescence, luxury solarpunk architectural lighting, clean symmetrical 3D model, PBR textures",
    "museum_curatorial_portal": "A grand architectural futuristic museum entrance portal archway, dark obsidian stone frame, geometric gold inlays, subtle cyan illuminated lines, luxury sci-fi pavilion doorway, clean geometry, PBR textures",
    "museum_sci_fi_column": "A tall luxury futuristic architectural column with dark titanium fluted shaft, gold accented capital and base rings, glowing subtle vertical cyan conduits, neoclassical sci-fi museum pillar, clean symmetrical 3D model, PBR textures",
    "museum_artifact_vitrine": "A luxury futuristic museum display vitrine showcase, dark obsidian plinth base with gold rim trim, transparent glass casing, internal soft warm gallery downlight, clean PBR game asset"
}

def main():
    api_key = load_api_key()
    if not api_key:
        print("[!] Error: TRIPO_API_KEY not found in .env")
        sys.exit(1)

    headers = {
        "Authorization": f"Bearer {api_key}",
        "Content-Type": "application/json"
    }

    out_dir = Path("Assets/Models/Tripo")
    out_dir.mkdir(parents=True, exist_ok=True)

    tasks = {}
    print(f"[*] Submitting {len(PROMPTS)} architectural asset tasks to Tripo v3.1...")

    for name, prompt in PROMPTS.items():
        payload = {
            "prompt": prompt,
            "model": "v3.1-20260211",
            "texture": True,
            "pbr": True,
            "texture_quality": "detailed"
        }
        try:
            r = requests.post(f"{BASE_URL}/generation/text-to-model", headers=headers, json=payload, timeout=30)
            data = r.json()
            task_id = data.get("data", {}).get("task_id")
            if task_id:
                tasks[name] = {"task_id": task_id, "prompt": prompt, "status": "queued"}
                print(f"[+] Task dispatched: {name} -> {task_id}")
            else:
                print(f"[-] Failed to submit {name}: {data}")
        except Exception as e:
            print(f"[-] Exception submitting {name}: {e}")

    # Poll tasks until all complete
    start_time = time.time()
    pending = set(tasks.keys())

    while pending and (time.time() - start_time < 360):
        print(f"[*] Polling {len(pending)} pending tasks ({int(time.time() - start_time)}s elapsed)...")
        time.sleep(12)

        for name in list(pending):
            tid = tasks[name]["task_id"]
            try:
                r = requests.get(f"{BASE_URL}/tasks/{tid}", headers=headers, timeout=30)
                res = r.json()
                data = res.get("data", {})
                status = data.get("status")
                progress = data.get("progress", 0)

                if status == "success":
                    output = data.get("output", {})
                    # In Tripo v3, model url can be in model_url or pbr_model
                    model_url = output.get("pbr_model") or output.get("model_url") or output.get("model")
                    if model_url:
                        target_file = out_dir / f"{name}.glb"
                        print(f"[✓] {name} SUCCESS! Downloading GLB from {model_url} -> {target_file}")
                        glb_res = requests.get(model_url, timeout=120)
                        target_file.write_bytes(glb_res.content)
                        print(f"[✓] Saved {target_file} ({target_file.stat().st_size // 1024} KB)")
                        pending.remove(name)
                    else:
                        print(f"[!] {name} success but no model_url: {output}")
                        pending.remove(name)
                elif status == "failed":
                    print(f"[X] {name} FAILED: {data}")
                    pending.remove(name)
                else:
                    print(f"[-] {name} in progress: {status} ({progress}%)")
            except Exception as e:
                print(f"[!] Error polling {name}: {e}")

    if not pending:
        print("[★] ALL TRIPO ASSETS SUCCESSFULLY GENERATED AND DOWNLOADED!")
    else:
        print(f"[!] Timeout reached. Still pending: {pending}")

if __name__ == "__main__":
    main()
