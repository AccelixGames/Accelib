# Accelib.Localization

로컬라이제이션 시스템. 다국어 텍스트 관리, 언어별 폰트 교체, Google Sheets 연동 다운로드를 제공한다.

## 디렉토리 구조

```
Accelib.Localization/
├── Accelib.Localization.asmdef          # 런타임 어셈블리 정의
├── README.md                            # 이 문서
├── CHANGELOG.md                         # 변경 이력
├── Runtime/
│   ├── LocalizationSingleton.cs         # 싱글톤 로컬라이제이션 매니저
│   ├── Architecture/
│   │   ├── ILocaleChangedEventListener.cs  # 언어 변경 이벤트 인터페이스
│   │   ├── LocaleSO.cs                  # 로케일 ScriptableObject
│   │   └── LocaleFontData.cs            # 언어별 폰트 데이터
│   ├── Model/
│   │   ├── LocaleKey.cs                 # 로케일 키 구조체
│   │   └── LocaleKeyList.cs             # 로케일 키 리스트 래퍼
│   └── Helper/
│       ├── LocalizedMonoBehaviour.cs    # 로컬라이즈 컴포넌트 추상 베이스
│       ├── LocalizedTMP.cs              # TMP 텍스트 + 폰트 교체
│       ├── LocalizedFont.cs             # TMP 폰트만 교체
│       ├── LocalizedImage.cs            # 언어별 이미지 교체
│       ├── LocalizedEvent.cs            # UnityEvent로 문자열 전달
│       ├── LocalizedTypewriter.cs       # (주석 처리) Febucci 타이프라이터
│       ├── SimpleLocaleChanger.cs       # 버튼 트리거 언어 변경
│       ├── SimpleLocaleToggle.cs        # 두 언어 간 토글
│       └── Formatter/
│           ├── ILocalizedFormatter.cs   # 포맷 인자 인터페이스
│           └── LocalizedFormatter_String.cs  # 문자열 배열 포맷터
└── Editor/
    ├── Accelib.Localization.Editor.asmdef  # 에디터 어셈블리 정의
    ├── Drawer/
    │   └── LocaleKeyDrawer.cs           # LocaleKey Odin 커스텀 드로어
    ├── Utility/
    │   ├── LocaleUtility.cs             # 에디터 로케일 유틸리티
    │   ├── EditorObjectField.cs         # EditorPrefs SO 참조 필드
    │   └── EditorPrefsField.cs          # EditorPrefs 에셋 참조 필드
    └── Window/
        └── DownloadLocaleWindow.cs      # Google Sheets 다운로드 창
```

## 주요 클래스

### LocalizationSingleton

`MonoSingleton<LocalizationSingleton>` 기반 로컬라이제이션 매니저.

- `ChangeLanguage(SystemLanguage)` — 언어 변경, 모든 리스너에 브로드캐스트
- `GetLocalizedString(string key)` — 키에 해당하는 로컬라이즈된 문자열 조회
- `GetFontData(int id)` — 현재 언어의 폰트 데이터 조회
- `CurrLang` — 현재 설정된 언어

### LocaleSO

로케일 ScriptableObject. `SerializedDictionary<string, string>`로 텍스트, `List<LocaleFontData>`로 폰트 관리.

### ILocaleChangedEventListener

언어 변경 이벤트를 수신하는 인터페이스. `LocalizationSingleton`이 씬 내 모든 구현체를 자동 탐색.

### LocalizedTMP

TMP 텍스트 + 폰트를 언어에 맞게 교체하는 컴포넌트.

- `Reload()` — 로케일 다시 로드
- `ChangeKey(string key)` — 키 변경
- `SetFormat(params object[] args)` — 포맷 인자 설정

### DownloadLocaleWindow (Editor)

Google Sheets에서 5개 언어(한/영/일/중간/중번) 데이터를 다운로드하여 LocaleSO에 저장.

## 사용 예시

```csharp
// 로컬라이즈된 문자열 가져오기
var text = LocalizationSingleton.GetLocalizedString("ui_title");

// 언어 변경
LocalizationSingleton.ChangeLanguage(SystemLanguage.Japanese);

// LocalizedTMP 키 변경
localizedTMP.ChangeKey("new_key", "format_arg1", "format_arg2");
```

## 의존성

| 패키지 | 용도 |
|--------|------|
| `Accelib.Runtime` | MonoSingleton, Deb 로깅, Extensions |
| `TextMeshPro` | TMP 폰트/텍스트 |
| `NaughtyAttributes` | 인스펙터 어트리뷰트 |
| `SerializedCollections` | SerializedDictionary |
| `Odin Inspector` | 커스텀 드로어, 어트리뷰트 (precompiled) |
| `UniTask` | 비동기 다운로드 (Editor) |

## 네임스페이스

```
Accelib.Module.Localization                     — LocalizationSingleton
Accelib.Module.Localization.Architecture        — ILocaleChangedEventListener, LocaleSO, LocaleFontData
Accelib.Module.Localization.Model               — LocaleKey, LocaleKeyList
Accelib.Module.Localization.Helper              — LocalizedMonoBehaviour, LocalizedTMP 등
Accelib.Module.Localization.Helper.Formatter    — ILocalizedFormatter, LocalizedFormatter_String
Accelib.Editor.Module.Localization              — DownloadLocaleWindow
Accelib.Editor.Module.Localization.Drawer       — LocaleKeyDrawer
Accelib.Editor.Module.Localization.Utility      — LocaleUtility, EditorObjectField, EditorPrefsField
```
