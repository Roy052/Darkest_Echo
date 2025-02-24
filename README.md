# DE (Darkest_Echo)
 <div>
    <h2> 게임 정보 </h2>
    <img src = "https://img.itch.zone/aW1nLzE5OTMyMDI0LnBuZw==/315x250%23c/LM7onR.png"><br>
    <img src="https://img.shields.io/badge/Unity-yellow?style=flat-square&logo=Unity&logoColor=FFFFFF"/>
    <img src="https://img.shields.io/badge/Adventure-blue"/>
    <h4> 개발 일자 : 2025.02 <br><br>
    <팀원> <br><br>
      <table border="1">
        <tr>
          <th>
            Roy052 
          </th>
          <th>
            맵 배치 및 생성, 메뉴 맵 선택 스테이지 씬 제작, 적 AI
          </th>
          <th>
          </th>
        </tr>
        <tr> 
            <th> Peppertunacan </th>
          <th> 캐릭터 이동, 음파 기능, 박수, 사운드 </th>
          <th> https://github.com/Peppertunacan </th>
        </tr>
      </table>
    게임 플레이 : https://goodstarter.itch.io/de
  </div>
  <div>
    <h2> 게임 설명 </h2>
    <h3> 스토리 </h3>
     어두운 곳에서 깨어난 당신.<br><br>
     자신의 음파를 통해 이곳을 파악하고 탈출해야 합니다.<br><br>
     Dark Echo에서 영감을 받았습니다.
    <h3> 게임 플레이 </h3>
    WASD로 이동, Space로 박수, Esc로 메뉴
     </div>
     
  <div>
    <h2> 게임 스크린샷 </h2>
      <table>
        <td><img src = "https://img.itch.zone/aW1hZ2UvMzMxNjQwNi8xOTkzMjAyNS5wbmc=/250x600/yPSEIc.png"></td>
      </table>
  </div>
  <div>
    <h2> 게임 플레이 영상 </h2>
    https://youtu.be/pC8ZTxhjmpc
  </div>
  <div>
  </div>

   <div>
       <h2> 주요 코드 </h2>
       <h4> SoundWave Object Pooling </h4>
    </div>
    
```csharp
private Queue<GameObject> objectPool = new();
    public static SoundWaveGenerator instance = null;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            for (var i = 0; i < 600; i++) objectPool.Enqueue(CreateNewSoundWave());
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private GameObject CreateNewSoundWave()
    {
        var soundWave = Instantiate(soundWavePrefab, this.transform);
        soundWave.name = $"SoundWave {objectPool.Count}";
        soundWave.gameObject.SetActive(false);
        return soundWave;
    }
    
    public void ThrowSoundWave(Vector3 direction, Vector3 position)
    {
        if (objectPool.Count < 1)
            for (var i = 0; i < 100; i++)
                objectPool.Enqueue(CreateNewSoundWave());

        var soundWave = objectPool.Dequeue();
        ...
    }

    public void RemoveSoundWave(GameObject soundWave)
    {
        ...
        instance.objectPool.Enqueue(soundWave);
    }

    public void ClearAllSoundWave()
    {
        while (objectPool.Count > 0)
            objectPool.Dequeue();

        var soundWaves = FindObjectsOfType<SoundWave>();
        foreach (SoundWave sw in soundWaves)
        {
            if(sw.isTemp == false)
                RemoveSoundWave(sw.gameObject);
            else
                DestroyImmediate(sw.gameObject);
        }
    }
```

<h4> SoundWave 반사 효과 </h4>
    </div>
    
```csharp
        if (circleCollider.isTrigger)
        {
            int count = rigid.Cast(moveDir, wallFilter, hitInfo);
            if (count > 0)
            {
                if (hitInfo[0].collider != null)
                    reflectDir = Vector2.Reflect(moveDir, hitInfo[0].normal).normalized;
                Debug.DrawRay(transform.position, moveDir);
            }
        }

        if (hitInfo[0].distance < 0.1f && Mathf.Approximately(moveSpeed, 12f))
        {
            moveSpeed = 0f;
            moveDir = Vector3.zero;
            trailRenderer.startColor = normalEndColor;
            SoundWaveGenerator.instance.SpawnSoundWave(SoundWaveGenerator.WaveType.Clapping, transform.position);
        }

        // Fading sound wave and destroy it
        if (trailStartTime < fadeDuration)
        {
            var t = trailStartTime / fadeDuration;
            Color fadedColor =
                new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(1f, 0f, t));
            trailRenderer.startColor = fadedColor;
            spriteRenderer.color = fadedColor;
        }
```
