# Changelog

이 문서는 Accelib.Conditional 모듈의 주요 변경 내역을 기록한다.

## [0.2.0] - 2026-02-16

### 변경
- 폴더 구조를 UPM 표준에 맞게 재구성
  - `Runtime/` — 런타임 코드 (Conditional, Condition, ValueProvider, SO_*)
  - `Runtime/Data/` — 열거형 (EComparisonOperator, ELogicalOperator, EValueSourceType)
  - `Runtime/Utility/` — 유틸리티 (OperatorStringUtility)
- 네임스페이스 변경
  - `Accelib.Conditional.Definition` → `Accelib.Conditional.Data`
  - `Accelib.Conditional.Model` → `Accelib.Conditional`
  - `Accelib.Conditional.Scriptable` → `Accelib.Conditional`
- asmdef에 표준 필드 추가 (Odin 의존성: `overrideReferences`, `precompiledReferences`, `defineConstraints`)

### 추가
- 모든 public 클래스/구조체/열거형에 XML `<summary>` 주석 추가
- 모든 public 메서드 및 abstract 프로퍼티에 XML 주석 추가

### 문서
- `README.md` 생성 (모듈 개요, 디렉토리 구조, 클래스 설명, 사용 예시, 의존성)
- `CHANGELOG.md` 생성

## [0.1.0] - 초기 버전

### 추가
- `Conditional` — 다중 조건식 컨테이너 (Evaluate, Preview)
- `Condition` — 단일 조건 (좌변 비교연산자 우변, 논리연산자)
- `ValueProvider` — 값 제공자 (Integer/Double/Boolean 리터럴, ScriptableObject, MemberRef)
- `SO_Conditional` — 조건식 ScriptableObject 래퍼
- `SO_ValueProviderBase` — 값 제공자 추상 베이스
- `SO_PresetValue` — MemberRef 프리셋 값 제공자
- `EComparisonOperator` — 비교 연산자 열거형 (==, !=, >, >=, <, <=)
- `ELogicalOperator` — 논리 연산자 열거형 (And, Or)
- `EValueSourceType` — 값 소스 타입 열거형
- `OperatorStringUtility` — 연산자 문자열 변환 유틸리티
