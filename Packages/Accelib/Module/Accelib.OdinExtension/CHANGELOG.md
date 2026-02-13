# Changelog

이 문서는 Accelib.OdinExtension 모듈의 주요 변경 내역을 기록한다.

## [0.2.1] - 2026-02-13

### 수정
- `SerializableReactiveProperty` 드로어를 `EditorGUILayout` 직접 편집 방식으로 변경
  - `EditorGUILayout.IntField/Toggle/FloatField`로 순수 값 편집
  - `rp.Value` 직접 대입 + `ForceMarkDirty()` + `ForceNotify()`
  - null 체크 시 `SirenixEditorGUI.ErrorMessageBox()` 표시

## [0.2.0] - 2026-02-13

### 제거
- `ReadOnlyReactiveProperty<int/bool/float>` 드로어 3개 삭제

## [0.1.1] - 2026-02-13

### 수정
- `SerializableReactiveProperty` 드로어가 인스펙터에서 읽기 전용으로 표시되던 문제 수정
  - Odin의 자식 프로퍼티(`value` 필드)를 직접 그리는 방식으로 변경하여 Unity 직렬화 파이프라인을 정상 사용하도록 수정
  - Undo/Redo 자동 지원

## [0.1.0] - 2026-02-13

### 추가
- `SerializableReactiveProperty<int>` Odin 드로어 — IntField로 순수 값 편집
- `SerializableReactiveProperty<bool>` Odin 드로어 — Toggle로 순수 값 편집
- `SerializableReactiveProperty<float>` Odin 드로어 — FloatField로 순수 값 편집
- `ReadOnlyReactiveProperty<int>` Odin 드로어 — 읽기 전용 IntField 표시
- `ReadOnlyReactiveProperty<bool>` Odin 드로어 — 읽기 전용 Toggle 표시
- `ReadOnlyReactiveProperty<float>` Odin 드로어 — 읽기 전용 FloatField 표시
- `ReactivePropertyDrawerHelper` — ForceNotify 리플렉션 호출 유틸리티

### 문서
- `README.md` 생성 (모듈 개요, 디렉토리 구조, 클래스 설명, 사용 예시, 의존성)
- `CHANGELOG.md` 생성
