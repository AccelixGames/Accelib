# Changelog

이 문서는 Accelib.Preview 모듈의 주요 변경 내역을 기록한다.

## [0.2.0] - 2026-02-16

### 변경
- 폴더 구조를 UPM 표준에 맞게 재구성 (`Runtime/` 하위로 이동)
- asmdef 표준화 — Odin 의존성 명시 (`precompiledReferences`)
- `IPreviewIconProvider`를 `#if ODIN_INSPECTOR` 전처리기로 래핑

### 추가
- 모든 인터페이스에 `<summary>` 주석 추가
- `README.md` 생성
- `CHANGELOG.md` 생성

## [0.1.0] - 초기 버전

### 추가
- `IPreviewNameProvider` — 에디터 프리뷰 이름 인터페이스
- `IPreviewIconProvider` — 에디터 프리뷰 아이콘 인터페이스
- `ISubAssetProvider` — 서브에셋 목록 인터페이스
