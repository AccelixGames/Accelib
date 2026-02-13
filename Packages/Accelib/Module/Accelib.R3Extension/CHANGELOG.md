# Changelog

이 문서는 Accelib.R3Extension 모듈의 주요 변경 내역을 기록한다.

## [0.1.0] - 2025-02-13

### 추가
- `ObservableExtension.Delta()` — `Observable<int>`의 연속 값 차이를 발행하는 확장 메서드
  - 내부 체인: `DistinctUntilChanged()` → `Pairwise()` → `Select(Current - Previous)`

### 문서
- `README.md` 생성
- `CHANGELOG.md` 생성
