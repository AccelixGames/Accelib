# Accelib 모듈 작성 규칙

이 문서는 `Module/` 아래에 새 모듈을 생성할 때 따라야 하는 규칙이다.

## 1. 디렉토리 구조 (UPM 표준)

```
Accelib.<ModuleName>/
├── Accelib.<ModuleName>.asmdef     # 어셈블리 정의
├── README.md                       # 한국어 문서
├── CHANGELOG.md                    # 한국어 변경 이력
├── Runtime/                        # 런타임 코드
│   ├── Data/                       # 데이터 구조
│   └── Utility/                    # 유틸리티
└── Editor/                         # 에디터 전용 코드
    ├── Drawer/                     # 커스텀 드로어
    └── Utility/                    # 에디터 유틸리티
```

- 런타임 코드가 없으면 `Runtime/` 폴더 생략 가능
- 에디터 코드가 없으면 `Editor/` 폴더 생략 가능

## 2. Assembly Definition (asmdef)

```json
{
  "name": "Accelib.<ModuleName>",
  "rootNamespace": "Accelib.<ModuleName>",
  "references": [],
  "includePlatforms": [],
  "excludePlatforms": [],
  "allowUnsafeCode": false,
  "overrideReferences": false,
  "precompiledReferences": [],
  "autoReferenced": true,
  "defineConstraints": [],
  "versionDefines": [],
  "noEngineReferences": false
}
```

### 규칙
- `name`과 `rootNamespace`는 `Accelib.<ModuleName>`으로 통일
- Editor 전용 모듈: `includePlatforms: ["Editor"]`
- Odin 의존: `defineConstraints: ["ODIN_INSPECTOR"]`, `overrideReferences: true`, `precompiledReferences`에 Sirenix DLL 추가
- R3 의존: `references`에 `"R3.Unity"`, `precompiledReferences`에 `"R3.dll"`

## 3. README.md 형식

한국어로 작성. 포함해야 할 항목:

1. **제목** — `# Accelib.<ModuleName>`
2. **개요** — 모듈의 목적과 기능 1~2줄 요약
3. **디렉토리 구조** — 코드 블록으로 파일 트리 표시, 각 파일에 주석
4. **주요 클래스** — 클래스별 설명, 주요 메서드/프로퍼티 나열
5. **사용 예시** — C# 코드 블록으로 실제 사용법 표시
6. **의존성** — 테이블 형태 (패키지 | 용도)
7. **네임스페이스** — 코드 블록으로 나열

### 참고 예시
`Accelib.Reflection/README.md` 형식을 따른다.

## 4. CHANGELOG.md 형식

한국어로 작성. [Keep a Changelog](https://keepachangelog.com/) 형식 기반.

```markdown
# Changelog

이 문서는 Accelib.<ModuleName> 모듈의 주요 변경 내역을 기록한다.

## [0.1.0] - YYYY-MM-DD

### 추가
- 새로운 기능 설명

### 변경
- 변경된 기능 설명

### 수정
- 버그 수정 설명

### 문서
- 문서 관련 변경
```

### 규칙
- 첫 버전: `[0.1.0]`
- 날짜 형식: `YYYY-MM-DD`
- 카테고리: 추가/변경/수정/제거/문서

## 5. C# 코드 규칙

### 전처리기 래핑
```csharp
#if UNITY_EDITOR && ODIN_INSPECTOR
// 에디터 + Odin 전용 코드
#endif
```

- 에디터 전용: `#if UNITY_EDITOR`
- Odin 전용: `#if ODIN_INSPECTOR`
- 둘 다: `#if UNITY_EDITOR && ODIN_INSPECTOR`

### 네임스페이스
- 물리 경로와 일치시킨다
- `Accelib.<ModuleName>` — 루트
- `Accelib.<ModuleName>.Data` — 데이터 구조
- `Accelib.<ModuleName>.Utility` — 유틸리티
- `Accelib.<ModuleName>.Editor.Drawer` — 에디터 드로어

### XML 문서 주석
- 모든 public/internal 클래스에 `<summary>` 작성
- 한국어 또는 영어 (기존 코드 스타일 따름)

## 6. 모듈 카탈로그 업데이트

모듈을 생성, 수정, 삭제할 때 반드시 `MODULE_CATALOG.md`도 업데이트한다.

### 업데이트 시점
- **모듈 생성:** 카탈로그 테이블에 새 행 추가, 상세 섹션 추가, 의존성 그래프 업데이트
- **모듈 수정:** 요약/의존성이 변경된 경우 카탈로그 반영
- **모듈 삭제:** 카탈로그에서 해당 모듈 제거, 의존성 그래프 업데이트

### 체크리스트
모듈 작업 완료 후 다음을 확인한다:
1. [ ] 모듈 README.md 업데이트
2. [ ] 모듈 CHANGELOG.md 업데이트
3. [ ] `MODULE_CATALOG.md` 업데이트

## 7. 기존 모듈 참고

| 모듈 | 특징 |
|------|------|
| `Accelib.Reflection` | Runtime + Editor 분리, 리플렉션 캐시, README/CHANGELOG 표준 |
| `Accelib.Preview` | 최소 모듈 (인터페이스 하나), 의존성 없음 |
| `Accelib.Conditional` | 다중 의존성, ZLinq/Collections 사용 |
| `Accelib.OdinExtension` | Editor 전용, Odin + R3 의존, Drawer 패턴 |
