# Unity-Utilities

Unity 개발에 유용한 다양한 유틸리티 함수들을 제공하는 패키지입니다.

## 주요 기능

- `WaitForHelper`: `WaitForSeconds` 및 `WaitForSecondsRealtime` 객체 캐싱 제공
  - 객체 생성 오버헤드 감소 및 가비지 컬렉션 부담 감소
  - 일정 시간 사용되지 않은 객체 자동 제거 (백그라운드 Task 사용)
  - `ConcurrentDictionary`를 사용한 동시성 문제 해결
- `MonoBehaviour` 확장 메서드 제공 (제거됨)
  - `GetWaitForSeconds`: `WaitForSeconds` 객체 캐싱 제공 (제거됨)
  - `GetWaitForSecondsRealtime`: `WaitForSecondsRealtime` 객체 캐싱 제공 (제거됨)
  - `CleanupWaitForSecondsCache`: 일정 시간 동안 사용되지 않은 `WaitForSeconds` 캐시를 정리 (제거됨)
- 일시 정지 상태를 고려한 `WaitForSecondsRealtime` 캐싱 메커니즘
- `Unity-Extensions` 패키지와 함께 사용 가능 (의존성 추가 필요)

## 사용 예제

```
# Unity-Utilities

Unity 개발에 유용한 다양한 유틸리티 함수들을 제공하는 패키지입니다.

## 주요 기능

- `WaitForHelper`: `WaitForSeconds` 및 `WaitForSecondsRealtime` 객체 캐싱 제공
    - 객체 생성 오버헤드 감소 및 가비지 컬렉션 부담 감소
    - 일정 시간 사용되지 않은 객체 자동 제거 (백그라운드 Task 사용)
    - `ConcurrentDictionary`를 사용한 동시성 문제 해결
- `MonoBehaviour` 확장 메서드 제공 (제거됨)
    - `GetWaitForSeconds`: `WaitForSeconds` 객체 캐싱 제공 (제거됨)
    - `GetWaitForSecondsRealtime`: `WaitForSecondsRealtime` 객체 캐싱 제공 (제거됨)
    - `CleanupWaitForSecondsCache`: 일정 시간 동안 사용되지 않은 `WaitForSeconds` 캐시를 정리 (제거됨)
- 일시 정지 상태를 고려한 `WaitForSecondsRealtime` 캐싱 메커니즘
- `Unity-Extensions` 패키지와 함께 사용 가능 (의존성 추가 필요)

## 설치 방법

[설치 방법에 대한 상세 설명 추가]

## 사용 예제

```csharp
using UnityEngine;
using Hian.Utilities;

public class MyMonoBehaviour : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(MyCoroutine());
    }

    private System.Collections.IEnumerator MyCoroutine()
    {
        Debug.Log("Waiting for 1 second...");
        yield return WaitForHelper.GetWaitForSeconds(1f);
        Debug.Log("1 second passed!");
        Debug.Log("Waiting for 2 seconds (realtime)...");
        yield return WaitForHelper.GetWaitForSecondsRealtime(2f);
        Debug.Log("2 seconds passed (realtime)!");
    }
}
```

## 설치 방법

### UPM을 통한 설치 (Git URL 사용)

#### 선행 조건

- Git 클라이언트(최소 버전 2.14.0)가 설치되어 있어야 합니다.
- Windows 사용자의 경우 `PATH` 시스템 환경 변수에 Git 실행 파일 경로가 추가되어 있어야 합니다.

#### 설치 방법 1: Package Manager UI 사용

1. Unity 에디터에서 Window > Package Manager를 엽니다.
2. 좌측 상단의 + 버튼을 클릭하고 "Add package from git URL"을 선택합니다.

   ![Package Manager Add Git URL](Document/upm-ui-giturl.png)
3. 다음 URL을 입력합니다:

```text
https://github.com/creator-hian/Unity-Common.git
```

4. 'Add' 버튼을 클릭합니다.

   ![Package Manager Add Button](Document/upm-ui-giturl-add.png)

#### 설치 방법 2: manifest.json 직접 수정

1. Unity 프로젝트의 `Packages/manifest.json` 파일을 열어 다음과 같이 dependencies 블록에 패키지를 추가하세요:

```json
{
  "dependencies": {
    "com.creator-hian.unity.utilities": "https://github.com/creator-hian/Unity-Utilities.git",
    ...
  }
}
```

#### 특정 버전 설치

특정 버전을 설치하려면 URL 끝에 #{version} 을 추가하세요:

```json
{
  "dependencies": {
    "com.creator-hian.unity.utilities": "https://github.com/creator-hian/Unity-Utilities.git#0.0.1",
    ...
  }
}
```

#### 참조 문서

- [Unity 공식 매뉴얼 - Git URL을 통한 패키지 설치](https://docs.unity3d.com/kr/2023.2/Manual/upm-ui-giturl.html)

## 문서

각 기능에 대한 자세한 설명은 해당 기능의 README를 참조하세요:

## 원작성자

- [Hian](https://github.com/creator-hian)

## 기여자

## 라이센스

[라이센스 정보 추가 필요]
