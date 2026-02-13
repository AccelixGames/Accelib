# Accelib — Claude Code 프로젝트 규칙

이 파일은 Claude Code가 자동으로 읽는 프로젝트 규칙이다.
Accelib 코드를 수정하는 모든 작업에 적용된다.

## 절대 규칙

Accelib 코드를 **1줄이라도** 변경했다면, 작업 종료 전에 반드시 아래 문서를 모두 업데이트한다.
**예외 없음.** 버그 수정, 리팩토링, 신규 기능 모두 해당.

### 모듈 레벨
1. 해당 모듈 `CHANGELOG.md` 업데이트
2. 해당 모듈 `README.md` 업데이트 (동작 설명이 바뀐 경우)
3. `Module/MODULE_CATALOG.md` 업데이트 (요약/의존성이 바뀐 경우)

### 패키지 루트
4. `Packages/Accelib/CHANGELOG.md` 업데이트 (Keep a Changelog 한국어 형식)
5. `Packages/Accelib/README.md` 업데이트 (모듈 목록 등이 변경된 경우)
6. `Packages/Accelib/package.json` 버전 업데이트

## 버전 관리

- **커밋 전까지 동일 버전 유지** — 여러 변경이 있어도 CHANGELOG·package.json 버전은 하나로 유지
- **커밋 시** — 해당 버전의 CHANGELOG 내용을 참고하여 커밋 메시지 작성
- **커밋 완료 후** — 다음 작업부터 새 버전 번호 사용

## CHANGELOG 카테고리 (한국어)

| 카테고리 | 원문 | 용도 |
|---------|------|------|
| **추가** | Added | 새로운 기능 |
| **변경** | Changed | 기존 기능의 변경 |
| **폐기 예정** | Deprecated | 곧 제거될 기능 |
| **제거** | Removed | 삭제된 기능 |
| **수정** | Fixed | 버그 수정 |
| **보안** | Security | 보안 취약점 |

## 참조 문서

- 모듈 작성 규칙: `Packages/Accelib/Module/MODULE_RULES.md`
- 모듈 카탈로그: `Packages/Accelib/Module/MODULE_CATALOG.md`
