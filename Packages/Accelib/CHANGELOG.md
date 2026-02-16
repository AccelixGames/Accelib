# Changelog

이 문서는 Accelib 패키지의 주요 변경 내역을 기록한다.

형식은 [Keep a Changelog](https://keepachangelog.com/en/1.1.0/)를 기반으로 하며,
[Semantic Versioning](https://semver.org/spec/v2.0.0.html)을 따른다.

## [0.0.15] - 2026-02-16

### 추가
- `Accelib.InputState` 모듈 신규 생성 — 토큰 기반 입력 상태 관리
  - `SO_InputState`: GameObject 토큰으로 입력 잠금/해제, `OnStateChanged` 이벤트
- `Accelib.UI.Popup` 모듈 신규 생성 — 레이어 팝업 및 모달 시스템
  - `Accelib.Runtime`의 `Module.UI.Popup`에서 독립 모듈로 추출
  - `PopupSingleton`에 `SO_InputState` 연동 추가 (Optional)

### 제거
- `Accelib.Runtime`의 `Module.UI.Popup` 삭제 — `Accelib.UI.Popup` 모듈로 대체

### 변경
- `MODULE_CATALOG.md` InputState, UI.Popup 모듈 추가

## [0.0.14] - 2026-02-16

### 추가
- `Accelib.Pool` 모듈 신규 생성 — 오브젝트 풀링 시스템
  - `IPoolTarget`: 풀 대상 인터페이스
  - `ResourcePool<T>`: Stack 기반 제네릭 리소스 풀
  - `ComponentPool<T>`: 델리게이트 구동 컴포넌트 풀
  - `PrefabPool<T>`: MonoBehaviour 프리팹 전용 풀

### 제거
- 레거시 `Module.ObjectPool` 삭제 (`GameObjectPool`, `IPoolableGameObject`) — `Accelib.Pool` 모듈로 대체

### 변경
- `MODULE_CATALOG.md` Pool 모듈 추가

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
