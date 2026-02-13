# Accelib

게임 개발에 필요한 기능을 모아놓은 유틸리티 라이브러리.

## 모듈

| 모듈 | 요약 | 의존성 |
|------|------|--------|
| **Accelib.Preview** | 프리뷰 이름/아이콘/서브에셋 인터페이스 정의 | (없음) |
| **Accelib.Reflection** | 리플렉션 기반 멤버 접근 및 UI 바인딩 | Accelib.Preview |
| **Accelib.Conditional** | 규칙 기반 조건식 평가 시스템 | Accelib.Reflection, Accelib.Preview, ZLinq, Collections |
| **Accelib.OdinExtension** | R3 ReactiveProperty용 Odin Drawer | R3, Odin Inspector |
| **Accelib.R3Extension** | R3 Observable 확장 메서드 (Delta 등) | R3 |

상세 내용은 [MODULE_CATALOG.md](Module/MODULE_CATALOG.md) 참조.

## 설치

```
https://github.com/AccelixGames/Accelib.git?path=Packages/Accelib
```

Unity Package Manager → Add package from git URL 에 위 주소를 입력한다.

## 라이선스

GPL
