---
name: font-sync
description: >
  TMP 폰트 머테리얼 동기화 — 언어별 폰트 머테리얼을 기준 언어에서 대상 언어로 복제·동기화.
  Use this skill when the user wants to sync TextMeshPro font materials
  between language folders (e.g., EN → KR), create missing material variants,
  or update locale assets with new font material references.
  Korean triggers: "폰트 동기화", "폰트 머테리얼", "KR 폰트", "EN 폰트 맞춰",
  "머테리얼 복사", "로케일 폰트", "TMP 머테리얼", "폰트 세팅 맞춰"
  English triggers: "font sync", "font material", "TMP material sync",
  "locale font", "sync font materials"
---

# TMP Font Material Sync

TextMeshPro 폰트 머테리얼을 기준 언어(EN)에서 대상 언어(KR 등)로 동기화하는 스킬.

## 언제 사용하는가

- 기준 언어 폴더에 머테리얼 변형(Outline_White, Outline_Blue 등)이 추가되었을 때
- 대상 언어 폴더에 대응하는 머테리얼이 없을 때
- Locale SO 에셋의 fontMaterials 리스트를 업데이트해야 할 때

## 경로 규약

| 항목 | 경로 |
|------|------|
| EN 폰트 폴더 | `Assets/TextMesh Pro/Resources/Fonts & Materials/EN/` |
| KR 폰트 폴더 | `Assets/TextMesh Pro/Resources/Fonts & Materials/KR/` |
| EN Locale SO | `Assets/Accelix/Data/System.Locale/(Locale) EN.asset` |
| KO Locale SO | `Assets/Accelix/Data/System.Locale/(Locale) KO.asset` |

## 워크플로우

### Phase 1 — 분석

1. **EN 폴더 스캔**: `.mat` 파일 목록을 수집한다.
2. **KR 폴더 스캔**: `.mat` 파일 목록을 수집한다.
3. **누락 머테리얼 식별**: EN에 존재하지만 KR에 없는 머테리얼을 명칭 규칙 기반으로 매핑한다.
   - 명칭 규칙: `EN_` 접두사 → `KR_` 접두사 (나머지 동일)
   - 예: `EN_SCDream6 Bold SDF Material_Outline_Blue.mat` → `KR_SCDream6 Bold SDF Material_Outline_Blue.mat`

### Phase 2 — 폰트별 아틀라스 데이터 수집

각 KR 폰트 에셋(`.asset`)에서 다음 값을 읽는다:

| 값 | 위치 |
|----|------|
| `_MainTex` fileID | `.asset` 파일의 `m_SavedProperties > m_TexEnvs > _MainTex > m_Texture` |
| `_MainTex` guid | `.asset.meta` 파일의 `guid` |
| `_GradientScale` | `.asset` 파일의 `m_Floats > _GradientScale` |
| `_ScaleRatioA` | `.asset` 파일의 `m_Floats > _ScaleRatioA` |
| `_ScaleRatioB` | `.asset` 파일의 `m_Floats > _ScaleRatioB` |
| `_ScaleRatioC` | `.asset` 파일의 `m_Floats > _ScaleRatioC` |

**중요:** `_GradientScale`과 `_ScaleRatio*` 값은 폰트 아틀라스에 종속적이다. EN 값을 그대로 복사하면 안 된다.

### Phase 3 — KR 머테리얼 생성

각 누락 머테리얼에 대해:

1. **대응하는 EN 머테리얼 읽기** — 전체 YAML 내용 로드
2. **KR 머테리얼 파일 생성** — 다음 값만 변경하고 나머지는 EN과 동일:

| 필드 | 변경 내용 |
|------|----------|
| `m_Name` | `EN_` → `KR_` |
| `_MainTex` | KR 폰트의 fileID + guid |
| `_GradientScale` | KR 폰트의 값 |
| `_ScaleRatioA/B/C` | KR 폰트의 기본값 (Unity가 에디터에서 자동 재계산) |

3. **시각 속성은 EN과 동일하게 유지**:
   - `_FaceDilate`, `_OutlineWidth`
   - `_FaceColor`, `_OutlineColor`
   - `_OutlineSoftness`, `_UnderlayDilate/Offset/Softness`
   - 기타 모든 `m_Floats`, `m_Colors`

4. **`.meta` 파일은 생성하지 않는다** — Unity가 자동 생성

### Phase 4 — Locale SO 업데이트

1. **Unity가 `.meta` 파일 생성 확인** — `Glob`으로 새 `.mat.meta` 존재 확인
2. **각 `.meta`에서 GUID 수집**
3. **KO Locale SO 편집** — `fontDataList` 내 해당 폰트의 `fontMaterials` 리스트에 새 머테리얼 GUID 추가:
   ```yaml
   - {fileID: 2100000, guid: <새 머테리얼 GUID>, type: 2}
   ```
4. **기존 항목은 유지** — 추가만 한다

### Phase 5 — 검증

1. `Glob`으로 `**/nul` 파일 확인 → 있으면 삭제
2. KR 폴더의 `.mat` 파일 수가 EN과 동일한지 확인 (KR 전용 머테리얼 제외)
3. KO Locale SO의 fontMaterials 항목 수가 EN Locale SO와 동일한지 확인

## 주의사항

- **폰트 에셋 간 _MainTex fileID가 다르다** — 반드시 각 KR 폰트 에셋에서 개별 확인
- **KR 전용 머테리얼** (EN에 없는 것)은 건드리지 않는다
- **EN 전용 폰트** (KR에 대응 폰트가 없는 경우)는 건너뛴다
- Locale SO의 `textDict`는 절대 수정하지 않는다 — `fontDataList`만 수정
