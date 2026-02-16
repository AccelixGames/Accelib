# Changelog

이 문서는 Accelib.Localization 모듈의 주요 변경 내역을 기록한다.

## [0.1.1] - 2026-02-16

### 수정
- `LocaleSO.TryGetValue()` — `textDict` null 초기화 시 자기 자신을 생성자에 전달하던 버그 수정
- `LocaleFontData.GetMaterial()` — `FontAsset`이 null일 때 NullReferenceException 방지

## [0.1.0] - 2026-02-16

### 추가
- `Accelib.Runtime`의 `Module.Localization`에서 독립 모듈로 추출
- Editor 코드 분리 — 별도 asmdef(`Accelib.Localization.Editor`) 생성
  - `LocaleKeyDrawer` (Runtime → Editor 이동)
  - `LocaleUtility` (Runtime → Editor 이동)
  - `DownloadLocaleWindow`, `EditorObjectField`, `EditorPrefsField` (Accelib.Editor → 모듈 내 Editor 이동)

### 변경
- `LocalizedTypewriter` 주석 처리 (Febucci TextAnimator 의존성 제거)
- `EditorObjectField` 네임스페이스 `Accelix.Editor` → `Accelib.Editor.Module.Localization.Utility` 수정
- `EditorPrefsField` 네임스페이스 통일

### 제거
- `Test_TMPFontAsset.cs` 삭제 (디버깅용 테스트 코드)
- `LocaleKey.EditorPreview` 프로퍼티 제거 (Drawer가 프리뷰 담당)
