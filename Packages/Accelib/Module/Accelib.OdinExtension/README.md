# Accelib.OdinExtension

R3 ReactiveProperty 타입을 위한 Odin Inspector 커스텀 드로어 모듈.
`SerializableReactiveProperty<T>`를 순수 값처럼 편집한다.

## 디렉토리 구조

```
Accelib.OdinExtension/
├── Accelib.OdinExtension.asmdef    # 어셈블리 정의 (Editor 전용)
├── Editor/
│   └── Drawer/
│       ├── ReactivePropertyDrawerHelper.cs              # ForceNotify 리플렉션 유틸리티
│       ├── SerializableReactivePropertyIntDrawer.cs     # int 편집 드로어
│       ├── SerializableReactivePropertyBoolDrawer.cs    # bool 편집 드로어
│       └── SerializableReactivePropertyFloatDrawer.cs   # float 편집 드로어
└── OdinVisualDesigner~/            # Odin Visual Designer 캐시 (자동 생성)
```

## 주요 클래스

### SerializableReactiveProperty 드로어

R3의 `SerializableReactiveProperty<T>` 기본 `CustomPropertyDrawer`를 대체하여 Odin 레이아웃에 맞는 순수 값 필드로 편집한다.
`EditorGUILayout.IntField/Toggle/FloatField`로 직접 편집하고, `rp.Value` 대입 + `ForceMarkDirty()`로 변경을 저장한다.

- `SerializableReactivePropertyIntDrawer` — int 편집
- `SerializableReactivePropertyBoolDrawer` — bool 편집
- `SerializableReactivePropertyFloatDrawer` — float 편집

값 변경 시 `ForceNotify()`를 리플렉션으로 호출하여 reactive 구독자에게 알린다.

### ReactivePropertyDrawerHelper

`ForceNotify()` 리플렉션 호출 공용 유틸리티. `MethodInfo`를 타입별로 캐시하여 반복 리플렉션 비용을 제거한다.

## 사용 예시

```csharp
// SerializableReactiveProperty — 인스펙터에서 순수 값처럼 편집 가능
[SerializeField]
private SerializableReactiveProperty<int> health = new(100);

[SerializeField]
private SerializableReactiveProperty<bool> isActive = new(true);

[SerializeField]
private SerializableReactiveProperty<float> speed = new(5.0f);

```

## 의존성

| 패키지 | 용도 |
|--------|------|
| `R3` (Cysharp/R3) | `SerializableReactiveProperty<T>` |
| `Sirenix.OdinInspector` | `OdinValueDrawer<T>`, `SirenixEditorGUI` |

## 네임스페이스

```
Accelib.OdinExtension.Editor.Drawer    — 모든 드로어 및 유틸리티
```
