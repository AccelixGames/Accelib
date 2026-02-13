# Accelib 모듈 카탈로그

이 문서는 Accelib 패키지의 모든 모듈과 코어 어셈블리의 개요를 기록한다.
모듈 작업 시 이 파일을 먼저 읽고, 필요한 모듈의 README를 참조한다.

## 모듈 목록

| 모듈 | 요약 | 의존성 | README |
|------|------|--------|--------|
| **Accelib.Preview** | 프리뷰 이름/아이콘/서브에셋 인터페이스 정의 | (없음) | README 없음 (인터페이스 3개만 포함) |
| **Accelib.Reflection** | 리플렉션 기반 멤버 접근 및 UI 바인딩. SO의 중첩 필드 경로를 드롭다운으로 선택, 캐시된 리플렉션으로 런타임 읽기 | Accelib.Preview | [README](Accelib.Reflection/README.md) |
| **Accelib.Conditional** | 조건식 평가 시스템. 비교/논리 연산자로 규칙 기반 로직 구성. 인스펙터에서 조건 편집 및 텍스트 프리뷰 | Accelib.Reflection, Accelib.Preview, ZLinq, Collections | README 없음 |
| **Accelib.OdinExtension** | R3 ReactiveProperty용 Odin Drawer. SerializableReactiveProperty 순수 값 편집 | R3, Odin Inspector | [README](Accelib.OdinExtension/README.md) |

## 모듈 상세

### Accelib.Preview
- **경로:** `Accelib.Preview/`
- **인터페이스:** `IPreviewNameProvider` (이름), `IPreviewIconProvider` (Odin SdfIcon), `ISubAssetProvider` (서브에셋 목록)
- 의존성 없는 최소 모듈. 다른 모듈들이 공통으로 참조함

### Accelib.Reflection
- **경로:** `Accelib.Reflection/`
- **주요 클래스:** `MemberRef` (직렬화 멤버 참조), `CachedChain` (리플렉션 캐시), `CachedReflectionUtility` (런타임 유틸리티), `ReflectionUtility` (에디터 스캔)
- R3 ReactiveProperty 타입 자동 감지 (어셈블리 참조 없이 타입명 기반)

### Accelib.Conditional
- **경로:** `Accelib.Conditional/`
- **주요 클래스:** `Conditional` (조건 컨테이너, Evaluate()), `Condition` (좌/우 ValueProvider + 비교연산자), `ValueProvider` (리터럴/SO/MemberRef 값 소스), `SO_Conditional` (ScriptableObject 래퍼)
- **연산자:** `EComparisonOperator` (==, !=, >, >=, <, <=), `ELogicalOperator` (And, Or)

### Accelib.OdinExtension
- **경로:** `Accelib.OdinExtension/`
- **드로어:** `SerializableReactiveProperty<int/bool/float>` (편집)
- **유틸리티:** `ReactivePropertyDrawerHelper` (ForceNotify 리플렉션 캐시)

## 의존성 그래프

```
Accelib.Preview (의존성 없음)
    ↑
    ├── Accelib.Reflection
    │       ↑
    │       └── Accelib.Conditional
    │
    └── Accelib.OdinExtension (R3, Odin 외부 의존)
```

## 코어 어셈블리

| 어셈블리 | 경로 | 요약 |
|----------|------|------|
| **Accelib.Runtime** | `../Runtime/` | 메인 런타임. Collections, Core, Data, Effect, Extensions, Localization, UI 등 서브시스템 포함 |
| **Accelib.Editor** | `../Editor/` | 에디터 도구. AutoBuild, CustomWindow, Localization 에디터, 각종 Drawer/Editor 포함 |

---

*이 파일은 모듈 생성/수정/삭제 시 반드시 업데이트한다. 상세 규칙은 [MODULE_RULES.md](MODULE_RULES.md) 참조.*
