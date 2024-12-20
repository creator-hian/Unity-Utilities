# Changelog

All notable changes to this project will be documented in this file.

## 버전 관리 정책

이 프로젝트는 Semantic Versioning을 따릅니다:

- **Major.Minor.Patch** 형식
  - **Major**: 호환성이 깨지는 변경
  - **Minor**: 하위 호환성 있는 기능 추가
  - **Patch**: 하위 호환성 있는 버그 수정
- **최신 버전이 상단에, 이전 버전이 하단에 기록됩니다.**

## [0.2.0] - 2024-12-20

### Added


## [0.1.0] - 2024-12-19

### Added

- `EventUtils` 클래스 추가
  - `ClearEventInvocations` 메서드 추가 (이벤트 구독 해제 기능)
- `WaitForHelper` 클래스 추가
  - `WaitForSeconds` 및 `WaitForSecondsRealtime` 객체 캐싱 기능 제공
  - 정적 생성자를 사용하여 백그라운드 Task 자동 시작
  - `ConcurrentDictionary`를 사용하여 동시성 문제 해결
  - `Application.quitting` 이벤트를 사용하여 애플리케이션 종료 시 Task 취소 및 리소스 해제

### Changed

- `WaitForHelper`의 `CleanupUnusedCache` 메서드에서 `TryRemove` 대신 `ContainsKey`와 `Remove`를 사용하도록 수정
- `WaitForHelper`의 `GetWaitForSecondsRealtime` 메서드에서 `Time.timeScale == 0` 조건 검사를 `if` 문 내부로 이동

### Removed

## [0.0.1] - 2024-12-19

### Added

- Initialize Package

### Changed

### Fixed
