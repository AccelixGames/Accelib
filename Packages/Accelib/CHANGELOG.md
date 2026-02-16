# Changelog

이 문서는 Accelib 패키지의 주요 변경 내역을 기록한다.

형식은 [Keep a Changelog](https://keepachangelog.com/en/1.1.0/)를 기반으로 하며,
[Semantic Versioning](https://semver.org/spec/v2.0.0.html)을 따른다.

## [0.1.0] - 2026-02-16

### 추가
- `Accelib.Core` 모듈 신규 생성 — 핵심 런타임 (MonoSingleton, Logging)
  - `Accelib.Runtime`의 `Core/MonoSingleton*`을 독립 모듈로 추출
  - `Accelib.Runtime`의 `Logging/Deb`을 독립 모듈로 이동
- `Accelib.Extensions` 모듈 신규 생성 — 확장 메서드 (List, IReadonlyList)
  - `Accelib.Runtime`의 `Extensions/`를 독립 모듈로 추출
- `Accelib.Pool` 모듈 신규 생성 — 오브젝트 풀링 시스템
  - `IPoolTarget`, `ResourcePool<T>`, `ComponentPool<T>`, `PrefabPool<T>`
- `Accelib.Flag` 모듈 신규 생성 — 토큰 기반 플래그 관리
  - `SO_TokenFlag`: MonoBehaviour 토큰으로 플래그 활성화/비활성화
- `Accelib.UI.Popup` 모듈 신규 생성 — 레이어 팝업 및 모달 시스템
  - `Accelib.Runtime`의 `Module.UI.Popup`에서 독립 모듈로 추출
  - `LayerPopup_Modal`을 abstract 베이스 클래스로 변환
  - `LayerPopup_PlainModal` (일반 텍스트 모달) 추가
  - `LayerPopup_LocalizedModal` (로컬라이제이션 모달) 추가
  - `SO_TokenFlag showCursor` 연동으로 팝업 시 커서 표시 자동 처리
- `Accelib.UI.Transition` 모듈 신규 생성 — 화면 전환 이펙트 시스템
  - `Accelib.Runtime`의 `Module.Transition`에서 독립 모듈로 추출
  - `SO_TokenFlag showCursor` 연동 추가
- `Accelib.Localization` 모듈 신규 생성 — 로컬라이제이션 시스템
  - `Accelib.Runtime`의 `Module.Localization`에서 독립 모듈로 추출
  - Editor 코드 별도 asmdef(`Accelib.Localization.Editor`)로 분리
  - `LocaleKeyDrawer`, `LocaleUtility` Runtime → Editor 이동
  - `DownloadLocaleWindow`, `EditorObjectField`, `EditorPrefsField` Accelib.Editor → 모듈 내 Editor 이동

### 변경
- `Accelib.Preview` 모듈 MODULE_RULES 표준 준수 리팩토링
  - 폴더 구조 UPM 표준 재구성, asmdef 표준화, `#if ODIN_INSPECTOR` 전처리기 래핑
- `Accelib.Conditional` 모듈 MODULE_RULES 표준 준수 개선
  - 폴더 구조·네임스페이스 표준화, XML 주석 추가
- `PopupSingleton`의 `isPaused`/`SO_InputState` → `SO_TokenFlag showCursor`로 변경
- `ModalOpenOption`에서 `useLocale` 필드 제거
- `LocalizedTypewriter` 주석 처리 (Febucci TextAnimator 의존성 제거)
- `EditorObjectField` 네임스페이스 통일
- `Accelib.Runtime` asmdef에 신규 모듈 참조 추가

### 수정
- `Accelib.Localization` — `LocaleSO.TryGetValue()` null 초기화 버그 수정
- `Accelib.Localization` — `LocaleFontData.GetMaterial()` null 안전성 추가

### 제거
- 레거시 `Module.ObjectPool` 삭제 — `Accelib.Pool` 모듈로 대체
- `Accelib.InputState` 모듈 삭제 — `Accelib.Flag` 모듈로 대체 (이름·API 전면 변경)
- `Accelib.Runtime`의 `Module.Localization` 삭제 — `Accelib.Localization` 모듈로 대체
- `Accelib.Runtime`의 `Module.Transition` 삭제 — `Accelib.UI.Transition` 모듈로 대체
- `Accelib.Runtime`의 `Module.UI.Popup` 삭제 — `Accelib.UI.Popup` 모듈로 대체
- `Accelib.Runtime`의 `Core/MonoSingleton*` 삭제 — `Accelib.Core` 모듈로 대체
- `Accelib.Runtime`의 `Extensions/` 삭제 — `Accelib.Extensions` 모듈로 대체
- `Accelib.Runtime`의 `Logging/SimpleLog.cs` 삭제
- `Accelib.Editor`의 `Module.Localization` 삭제 — `Accelib.Localization.Editor`로 대체
- `Test_TMPFontAsset.cs` 삭제

## [0.0.13] - 2025-02-13

### 추가
- `Accelib.R3Extension` 모듈 신규 생성 — R3 Observable 확장 메서드
  - `ObservableExtension.Delta()`: `Observable<int>`의 연속 값 차이(델타) 발행

### 변경
- `CHANGELOG.md` Keep a Changelog 형식으로 정비
- `README.md` 패키지 소개 및 모듈 목록으로 재작성
- `MODULE_CATALOG.md` R3Extension 모듈 추가
- `MODULE_RULES.md` 패키지 루트 문서 업데이트 규칙 및 버전 관리 규칙 추가 (§7, §8)

## [0.0.3] - 2024-04-16

### 추가
- CHANGELOG.md 추가

### 변경
- README.md 파일 내용 수정

## [0.0.2] - 2024-04-16

### 추가
- A lot of tools Updated.

## [0.0.1] - 2024-02-29

### 추가
- Initial commit
