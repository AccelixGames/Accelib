# Changelog

이 문서는 Accelib.Reflection 모듈의 주요 변경 내역을 기록한다.

## [0.3.0] - 2026-02-13

### 변경
- 폴더 구조를 UPM 표준에 맞게 재구성
  - `Runtime/` — 런타임 코드 (`MemberRef`, `Data/`, `Utility/`)
  - `Editor/` — 에디터 전용 코드 (`Utility/ReflectionUtility`)
- `Model/` → `Runtime/Data/`로 폴더명 변경
- 네임스페이스 변경: `Accelib.Reflection.Model` → `Accelib.Reflection.Data`

## [0.2.0] - 2026-02-13

### 추가
- `ReflectionUtility`에 R3 `ReactiveProperty<T>` 계열 타입 지원 추가
  - `TryGetReactivePropertyValueType()` 메서드 추가
  - `ReactiveProperty<T>`, `SerializableReactiveProperty<T>`, `ReadOnlyReactiveProperty<T>`, `BindableReactiveProperty<T>` 감지
  - 내부 `T`가 숫자형일 때 `fieldName.Value` 경로를 드롭다운에 자동 노출
  - R3 어셈블리 참조 없이 타입 이름 기반 판별 (asmdef 수정 불필요)

### 문서
- `README.md` 생성 (모듈 개요, 디렉토리 구조, 클래스 설명, 사용 예시, 의존성)
- `CHANGELOG.md` 생성

## [0.1.0] - 초기 버전

### 추가
- `MemberRef` — ScriptableObject의 중첩 멤버 경로를 인스펙터에서 선택·읽기
- `CachedChain` — 리플렉션 체인 캐시 데이터 구조
- `ENumericType` — 지원하는 숫자 타입 열거형
- `CachedReflectionUtility` — 런타임 캐시 기반 멤버 접근 유틸리티 (`BuildChain`, `BuildDoubleGetter`, `ConvertToDoubleFast`)
- `ReflectionUtility` — 에디터 전용 멤버 스캔 유틸리티 (`GetMemberList`, `ScanType`, `TryMapNumeric`)
