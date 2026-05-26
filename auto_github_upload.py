#!/usr/bin/env python3
"""
auto_github_upload.py

Run this script inside (or point it at) a folder to:
  - init git (if needed)
  - create a GitHub repo (if needed)
  - add/commit all files
  - push to GitHub (origin -> main)

Requirements:
  - git installed and on PATH
  - either:
      A) GitHub CLI installed (`gh auth login` done), OR
      B) set env var GITHUB_TOKEN with a PAT that can create/push repos

Examples:
  python auto_github_upload.py
  python auto_github_upload.py --path . --repo my-new-repo --private
  python auto_github_upload.py --path C:\path\to\project --commit "update"
  python auto_github_upload.py --remote https://github.com/you/repo.git
"""

from __future__ import annotations

import argparse
import json
import os
import shutil
import subprocess
import sys
import urllib.error
import urllib.request
from pathlib import Path


def _run(cmd: list[str], cwd: Path, check: bool = True) -> subprocess.CompletedProcess:
    return subprocess.run(cmd, cwd=str(cwd), text=True, capture_output=True, check=check)


def _have(cmd: str) -> bool:
    return shutil.which(cmd) is not None


def _git(cwd: Path, *args: str, check: bool = True) -> subprocess.CompletedProcess:
    return _run(["git", *args], cwd=cwd, check=check)


def _git_output(cwd: Path, *args: str) -> str:
    return _git(cwd, *args).stdout.strip()


def _is_git_repo(cwd: Path) -> bool:
    return (cwd / ".git").exists()


def _ensure_git_repo(cwd: Path) -> None:
    if not _is_git_repo(cwd):
        _git(cwd, "init")

    # Ensure branch is main
    head = _git_output(cwd, "rev-parse", "--abbrev-ref", "HEAD")
    if head in ("HEAD", ""):
        # No commits yet; create/rename branch to main
        _git(cwd, "checkout", "-B", "main")
    elif head != "main":
        # If 'main' already exists, switch to it; otherwise rename current to main
        cp = _git(cwd, "show-ref", "--verify", "--quiet", "refs/heads/main", check=False)
        if cp.returncode == 0:
            _git(cwd, "checkout", "main")
        else:
            _git(cwd, "branch", "-M", "main")


def _ensure_user_config(cwd: Path) -> None:
    # If the user hasn't set global config, git commit will fail. Use local config as fallback.
    name = _git_output(cwd, "config", "--get", "user.name") if _is_git_repo(cwd) else ""
    email = _git_output(cwd, "config", "--get", "user.email") if _is_git_repo(cwd) else ""
    if not name:
        name = os.environ.get("GIT_AUTHOR_NAME") or os.environ.get("GIT_COMMITTER_NAME") or ""
        if name:
            _git(cwd, "config", "user.name", name)
    if not email:
        email = os.environ.get("GIT_AUTHOR_EMAIL") or os.environ.get("GIT_COMMITTER_EMAIL") or ""
        if email:
            _git(cwd, "config", "user.email", email)


def _remote_url(cwd: Path, remote: str = "origin") -> str | None:
    cp = _git(cwd, "remote", "get-url", remote, check=False)
    if cp.returncode != 0:
        return None
    url = (cp.stdout or "").strip()
    return url or None


def _gh_authed() -> bool:
    if not _have("gh"):
        return False
    cp = subprocess.run(["gh", "auth", "status"], text=True, capture_output=True)
    return cp.returncode == 0


def _gh_owner() -> str | None:
    # Reads the authenticated user login from GitHub via gh
    if not _have("gh"):
        return None
    cp = subprocess.run(["gh", "api", "user", "--jq", ".login"], text=True, capture_output=True)
    if cp.returncode != 0:
        return None
    login = (cp.stdout or "").strip()
    return login or None


def _create_repo_with_gh(repo: str, private: bool) -> str:
    # Returns https remote URL (which will work with gh credential helper)
    vis = "--private" if private else "--public"
    # --source . allows running inside folder; --remote origin sets origin
    cp = subprocess.run(
        ["gh", "repo", "create", repo, vis, "--source", ".", "--remote", "origin", "--push=false"],
        text=True,
        capture_output=True,
    )
    if cp.returncode != 0:
        raise RuntimeError((cp.stderr or cp.stdout or "").strip() or "gh repo create failed")

    # Grab the remote URL from git config after creation
    return _remote_url(Path.cwd()) or f"https://github.com/{_gh_owner()}/{repo}.git"


def _github_api_request(token: str, method: str, url: str, body: dict | None = None) -> dict:
    data = None
    if body is not None:
        data = json.dumps(body).encode("utf-8")
    req = urllib.request.Request(url, data=data, method=method)
    req.add_header("Accept", "application/vnd.github+json")
    req.add_header("X-GitHub-Api-Version", "2022-11-28")
    req.add_header("Authorization", f"Bearer {token}")
    if data is not None:
        req.add_header("Content-Type", "application/json")
    try:
        with urllib.request.urlopen(req, timeout=30) as resp:
            raw = resp.read().decode("utf-8")
            return json.loads(raw) if raw else {}
    except urllib.error.HTTPError as e:
        raw = e.read().decode("utf-8", errors="replace")
        raise RuntimeError(f"GitHub API error {e.code}: {raw}") from e


def _github_login_from_token(token: str) -> str:
    me = _github_api_request(token, "GET", "https://api.github.com/user")
    login = me.get("login")
    if not login:
        raise RuntimeError("Could not determine GitHub user from token")
    return str(login)


def _create_repo_with_token(token: str, repo: str, private: bool) -> str:
    body = {"name": repo, "private": private}
    created = _github_api_request(token, "POST", "https://api.github.com/user/repos", body=body)
    html_url = created.get("html_url")
    if not html_url:
        raise RuntimeError("Repo created but API did not return html_url")
    # Use HTTPS remote; user must have a credential helper or embed token (we avoid embedding)
    return str(created.get("clone_url") or f"{html_url}.git")


def _ensure_origin_remote(cwd: Path, remote_url: str) -> None:
    if _remote_url(cwd, "origin"):
        return
    _git(cwd, "remote", "add", "origin", remote_url)


def _has_commits(cwd: Path) -> bool:
    cp = _git(cwd, "rev-parse", "--verify", "HEAD", check=False)
    return cp.returncode == 0


def _commit_all(cwd: Path, message: str) -> None:
    _git(cwd, "add", "-A")
    # Only commit if there is something to commit
    cp = _git(cwd, "status", "--porcelain")
    if (cp.stdout or "").strip() == "":
        return
    if not _has_commits(cwd):
        _git(cwd, "commit", "-m", message)
        return
    # Avoid failing when nothing changed (we already checked porcelain, but keep safe)
    _git(cwd, "commit", "-m", message, check=False)


def _push(cwd: Path) -> None:
    # Set upstream on first push
    cp = _git(cwd, "rev-parse", "--abbrev-ref", "--symbolic-full-name", "@{u}", check=False)
    if cp.returncode != 0:
        _git(cwd, "push", "-u", "origin", "main")
    else:
        _git(cwd, "push", "origin", "main")


def main() -> int:
    p = argparse.ArgumentParser(description="Initialize + upload a folder to GitHub.")
    p.add_argument("--path", default=".", help="Folder to upload (default: current folder).")
    p.add_argument("--repo", default=None, help="GitHub repo name (default: folder name).")
    p.add_argument(
        "--remote",
        default=None,
        help="Existing remote URL to use for origin (e.g. https://github.com/user/repo.git).",
    )
    p.add_argument("--private", action="store_true", help="Create a private GitHub repo.")
    p.add_argument("--commit", default="Initial commit", help="Commit message.")
    p.add_argument("--no-create", action="store_true", help="Do not create the GitHub repo; only push if origin exists.")
    args = p.parse_args()

    if not _have("git"):
        print("ERROR: git is not installed or not on PATH.", file=sys.stderr)
        return 2

    cwd = Path(args.path).expanduser().resolve()
    if not cwd.exists() or not cwd.is_dir():
        print(f"ERROR: path is not a folder: {cwd}", file=sys.stderr)
        return 2

    def _prompt(msg: str) -> str:
        try:
            return input(msg)
        except EOFError:
            return ""

    remote_arg = (args.remote or "").strip()
    repo = args.repo or cwd.name
    if (args.remote is None and args.repo is None) and sys.stdin.isatty():
        print("Repo setup:")
        remote_arg = _prompt("  Paste GitHub repo URL (or press Enter to create one): ").strip()
        if not remote_arg:
            typed = _prompt(f"  Repo name (Enter for '{repo}'): ").strip()
            if typed:
                repo = typed
        print()

    try:
        _ensure_git_repo(cwd)
        _ensure_user_config(cwd)

        origin = _remote_url(cwd, "origin")
        if origin is None and remote_arg:
            _ensure_origin_remote(cwd, remote_arg)
            origin = remote_arg

        if origin is None and not args.no_create:
            if _gh_authed():
                # Run gh in the target folder so it writes origin there
                cp = subprocess.run(
                    ["gh", "repo", "create", repo, "--private" if args.private else "--public", "--source", str(cwd), "--remote", "origin", "--push=false"],
                    text=True,
                    capture_output=True,
                )
                if cp.returncode != 0:
                    raise RuntimeError((cp.stderr or cp.stdout or "").strip() or "gh repo create failed")
                origin = _remote_url(cwd, "origin")
            else:
                token = os.environ.get("GITHUB_TOKEN")
                if not token:
                    raise RuntimeError(
                        "No origin remote, and no GitHub auth found.\n"
                        "Either install/login to GitHub CLI (gh auth login), or set GITHUB_TOKEN to a GitHub PAT."
                    )
                remote_url = _create_repo_with_token(token, repo, args.private)
                _ensure_origin_remote(cwd, remote_url)
                origin = remote_url

        if origin is None:
            raise RuntimeError("No git remote 'origin' is set. Re-run without --no-create, or add origin manually.")

        _commit_all(cwd, args.commit)
        _push(cwd)

        # Friendly output
        print(f"Pushed to: {origin}")
        return 0
    except subprocess.CalledProcessError as e:
        msg = (e.stderr or e.stdout or str(e)).strip()
        print(f"ERROR: command failed: {' '.join(e.cmd) if isinstance(e.cmd, list) else e.cmd}\n{msg}", file=sys.stderr)
        return e.returncode or 1
    except Exception as e:
        print(f"ERROR: {e}", file=sys.stderr)
        return 1


if __name__ == "__main__":
    raise SystemExit(main())
