# 한국사 문제 풀이 보드 게임, TimeTravel
다른 사람과 경쟁하며 한국사 문제를 푸는 뱀사다리 게임

<img src="https://github.com/Junghs-cloud/TimeTravel/assets/77110178/a3dad494-1ca5-4746-96cf-eb98524cec58" width="200" height="200">

## 개발 기간 및 개발 환경
개발 기간: 2023.06.26~ 2023.11.06

개발 인원: 2인 개발

개발 환경
- C#
- MySQL
- Visual Studio
- Photon
- Unity 2021.3.15f1

## 게임 화면

<div align=center>
    
### 메인 화면
<img src="https://github.com/Junghs-cloud/TimeTravel/assets/77110178/b7ce3b18-e776-4515-8d92-054635ceb0d9" width="533" height="300">

### 룸 화면
<img src="https://github.com/Junghs-cloud/TimeTravel/assets/77110178/db26947b-aac1-4815-9d93-c4c4380f52ef" width="533" height="300">

### 문제 풀이 화면
<img src="https://github.com/Junghs-cloud/TimeTravel/assets/77110178/bfcda24d-4db9-45d4-afd2-c08f20a52430" width="533" height="300">

### 게임 종료 화면
<img src="https://github.com/Junghs-cloud/TimeTravel/assets/77110178/56bae07b-ab94-4627-971d-d6faf79e6aa8" width="533" height="300">

</div>

## 게임 설명
- 2인에서 4인까지 플레이 가능한 멀티플레이 보드 게임입니다. 플레이어들은 1부터 100까지 채워진 보드 판을 주사위를 굴리며 한국사 문제를 풀며 도착 지점까지 나아가야 합니다.

- 게임 시작 시 플레이어들에게 도움이 될 카드(아이템)들이 4장 지급됩니다.

- 문제를 5개 맞힐 때마다 주사위를 추가로 굴릴 수 있으며, 보유하고 있는 카드가 3장 이하일 경우 카드를 랜덤으로 한 장 더 받을 수 있습니다.

- 한 플레이어가 도착 지점에 먼저 도달하여 우승하거나, 모든 플레이어가 나가 한 사람만 남게 되면 게임 종료입니다. 게임이 종료가 되면 자신이 틀렸던 문제들을 보여주는 오답 노트가 제공됩니다.

- 다른 플레이어들과 매칭은 랜덤으로 가능하기도 하며, 비밀번호를 걸어 아는 사람들과 플레이하는 것도 가능합니다.

- 관리자가 게임의 문제를 추가하거나, 수정할 수 있는 관리자 모드가 제공됩니다. 비밀번호와 아이디를 사용해 관리자 모드로 진입할 수 있으며, 문제는 MySQL 데이터베이스에 저장됩니다.

### 보드 게임의 칸
보드 판의 칸은 아무것도 없는 칸, 문제 칸, 사다리 칸, 포탈 칸 총 4가지로 이루어집니다.
- 아무것도 없는 칸: 해당 칸으로 바로 이동합니다.
- 문제 칸: 해당 시대에 관한 랜덤 문제를 풀어서 맞혀야 주사위의 눈 수만큼 이동할 수 있습니다.
- 사다리 칸: 사다리의 연결된 칸으로 이동합니다.
- 포탈 칸: 같은 색상의 포탈이 그려진 칸으로 이동합니다.


### 아이템 카드
- 자신의 차례에서 사용 가능한 A세트 카드, 다른 사람의 차례 때 방해하는 B 세트 카드로 나누어집니다.

- A세트 카드: 종류에 상관없이 한 턴에 한 번만 사용할 수 있습니다.
<div align=center>
    <img src="https://github.com/Junghs-cloud/TimeTravel/assets/77110178/79fa5cbb-a46a-42fb-b1b9-8bdd95cb695e" width=115 height="159">
    <img src="https://github.com/Junghs-cloud/TimeTravel/assets/77110178/b50fdfe5-868f-4d65-9736-aafece71a73c" width=115 height="159">
    <img src="https://github.com/Junghs-cloud/TimeTravel/assets/77110178/a4e41243-7066-4b98-ada8-5e3db0b2372b" width=115 height="159">
</div>
    
      문제 패스: 해당 문제를 패스하며, 맞은 것으로 간주합니다.
    
      힌트: 문제에 대한 힌트가 지급됩니다. 일부 문제에서만 사용할 수 있습니다.
    
      선택지 제거: 틀린 선택지를 하나 제거합니다.

- B 세트 카드: 다른 플레이어가 문제 풀기 전에 사용할 수 있습니다.
<div align=center>
    <img src="https://github.com/Junghs-cloud/TimeTravel/assets/77110178/f6d58f25-d5f5-4466-b872-06ca4ab0a880" width=115 height="159">
    <img src="https://github.com/Junghs-cloud/TimeTravel/assets/77110178/e6e13585-d052-4f16-88ff-55dfef0090a2" width=115 height="159">
    <img src="https://github.com/Junghs-cloud/TimeTravel/assets/77110178/cf47d67e-c659-409c-a68b-9c93363e2609" width=115 height="159">
</div>

      운명공동체: 상대방이 문제를 맞히면 이 카드를 사용한 사람도 주사위의 눈 수만큼 이동합니다.
    
      시간 빼앗기: 상대방의 문제 푸는 시간을 줄입니다.

      카드 빼앗기: 상대방의 카드 중 하나를 랜덤으로 가져옵니다.

    
    
     

