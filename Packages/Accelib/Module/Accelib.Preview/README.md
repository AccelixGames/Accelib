# Accelib.Preview

에디터 프리뷰용 이름, 아이콘, 서브에셋 인터페이스 정의 모듈.
의존성 없는 최소 모듈로, 다른 Accelib 모듈들이 공통으로 참조한다.

## 디렉토리 구조

```
Accelib.Preview/
├── Accelib.Preview.asmdef        # 어셈블리 정의
├── README.md                     # 이 문서
├── CHANGELOG.md                  # 변경 이력
└── Runtime/
    ├── IPreviewNameProvider.cs   # 프리뷰 이름 인터페이스
    ├── IPreviewIconProvider.cs   # 프리뷰 아이콘 인터페이스 (Odin 전용)
    └── ISubAssetProvider.cs      # 서브에셋 목록 인터페이스
```

## 주요 인터페이스

### IPreviewNameProvider

에디터 프리뷰에 표시할 이름을 제공한다.

- `EditorPreviewName` — 프리뷰에 표시될 문자열

### IPreviewIconProvider

에디터 프리뷰에 표시할 SdfIcon을 제공한다. `#if ODIN_INSPECTOR` 전처리기로 래핑되어 Odin이 없는 환경에서는 제외된다.

- `EditorPreviewIcon` — Odin `SdfIconType` 값

### ISubAssetProvider

하위 ScriptableObject 에셋 목록을 제공한다. 에디터 트리뷰 등에서 재귀적으로 서브에셋을 탐색할 때 사용한다.

- `SubAssets` — `IReadOnlyList<ScriptableObject>`

## 사용 예시

```csharp
using Accelib.Preview;
using UnityEngine;

public class MyConfig : ScriptableObject, IPreviewNameProvider
{
    [SerializeField] private string displayName;

    public string EditorPreviewName => displayName;
}
```

## 의존성

| 패키지 | 용도 | 비고 |
|--------|------|------|
| `Sirenix.OdinInspector` | `SdfIconType` 타입 | `IPreviewIconProvider`에서만 사용, `#if ODIN_INSPECTOR`로 조건부 |

## 네임스페이스

```
Accelib.Preview    — IPreviewNameProvider, IPreviewIconProvider, ISubAssetProvider
```
