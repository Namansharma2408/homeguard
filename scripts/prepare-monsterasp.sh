#!/usr/bin/env bash
set -Eeuo pipefail

repo_root="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")/.." && pwd)"
client_dir="$repo_root/netforge.client"
dist_dir="$client_dir/dist"
wwwroot_dir="$repo_root/NetForge.Server/wwwroot"
manifest="$repo_root/scripts/.monsterasp-vite-manifest"
manifest_tmp=""

cleanup() {
    if [[ -n "$manifest_tmp" && -f "$manifest_tmp" ]]; then
        unlink -- "$manifest_tmp"
    fi
}
trap cleanup EXIT

(
    cd "$client_dir"
    npm ci
    npm run build
)

if [[ ! -f "$dist_dir/index.html" ]]; then
    printf 'Frontend build did not produce %s\n' "$dist_dir/index.html" >&2
    exit 1
fi

mkdir -p "$wwwroot_dir"

# Refuse to overwrite server-owned files. Previous frontend output is identified only by this
# manifest, so cleanup never removes unrelated static resources from wwwroot.
while IFS= read -r -d '' source_file; do
    relative_path="${source_file#"$dist_dir"/}"
    target_file="$wwwroot_dir/$relative_path"
    if [[ ( -e "$target_file" || -L "$target_file" ) ]] && ! grep -Fqx -- "$relative_path" "$manifest" 2>/dev/null; then
        printf 'Refusing to overwrite server-owned static file: %s\n' "$target_file" >&2
        exit 1
    fi
done < <(find "$dist_dir" \( -type f -o -type l \) -print0)

if [[ -f "$manifest" ]]; then
    while IFS= read -r relative_path || [[ -n "$relative_path" ]]; do
        case "$relative_path" in
            ''|/*|..|../*|*/../*|*/..)
                printf 'Invalid path in frontend manifest: %s\n' "$relative_path" >&2
                exit 1
                ;;
        esac

        target_file="$wwwroot_dir/$relative_path"
        if [[ -L "$target_file" || -f "$target_file" ]]; then
            unlink -- "$target_file"
        elif [[ -e "$target_file" ]]; then
            printf 'Expected a generated file but found a directory: %s\n' "$target_file" >&2
            exit 1
        fi

        parent_dir="$(dirname -- "$target_file")"
        while [[ "$parent_dir" == "$wwwroot_dir/"* ]]; do
            rmdir -- "$parent_dir" 2>/dev/null || true
            parent_dir="$(dirname -- "$parent_dir")"
        done
    done < "$manifest"
fi

cp -a "$dist_dir/." "$wwwroot_dir/"

manifest_tmp="$(mktemp "$manifest.tmp.XXXXXX")"
(
    cd "$dist_dir"
    find . \( -type f -o -type l \) -print | sed 's#^\./##' | LC_ALL=C sort
) > "$manifest_tmp"
mv -f -- "$manifest_tmp" "$manifest"
manifest_tmp=""

if [[ ! -f "$wwwroot_dir/index.html" ]]; then
    printf 'Frontend copy did not produce %s\n' "$wwwroot_dir/index.html" >&2
    exit 1
fi

printf 'Prepared %s for ASP.NET publish.\n' "$wwwroot_dir"
