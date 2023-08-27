using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAISurvival : EnemyAI
{
    enum EnemyAIState
    {
        None = 0,
        Move = 1,
        MeleeAttack = 2,
        Shoot = 3,
        Grenade = 4,
        Reload = 5,
    }

    const int maxBullet = 6;
    const int maxGrenade = 3;
    

    public bool hasMelee;
    public bool hasGun;
    public bool hasGrenade;

    float reactionTime = 0;
    float selectTime = 0;
    float reloadTime = 4;

    int currentBullet;
    int currentGrenade;

    int[] weightByState;

    EnemyAIState currentState;

    Dictionary<EnemyAIState, string> dictFunc;

    public override void Start()
    {
        weightByState = new int[Enum.GetNames(typeof(EnemyAIState)).Length];

        //First action is always move
        weightByState[(int)EnemyAIState.Move] = 1;

        if (hasGun)
        {
            currentBullet = maxBullet;
        }
            
        if (hasGrenade)
            currentGrenade = maxGrenade;

        reactionTime = UnityEngine.Random.Range(1.5f, 2.5f);
        targetPos = Singleton.survivalSM.movePoints[Singleton.random.Next(0, Singleton.survivalSM.movePoints.Length - 1)];
    }


    public override void Update()
    {
        if(currentTime >= selectTime)
        {
            SelectState();
        }

        Debug.Log($"Current State : {currentState.ToString()}");
        Invoke(currentState.ToString(), 0);
        currentTime += Time.deltaTime;
    }

    void SelectState()
    {
        int sum = 0;

        for (int i = 0; i < weightByState.Length; i++)
            sum += weightByState[i];

        int randomValue = Singleton.random.Next(0, sum);
        for (int i = 0; i < weightByState.Length; i++)
        {
            randomValue -= weightByState[i];
            if (randomValue <= 0)
            {
                currentState = (EnemyAIState)i;
                break;
            }
        }   
    }

    void Move()
    {
        Chase();
    }

    void MeleeAttack()
    {
        if (Vector2.Distance(transform.position, Singleton.player.transform.position) <= 0.5f)
            Singleton.survivalSM.SurvivalEnd();
    }

    void Shoot()
    {
        currentBullet--;
        if(currentBullet == 0)
        {
            weightByState[(int)EnemyAIState.Shoot] = 0;
            weightByState[(int)EnemyAIState.Reload] = 5;
        }
    }

    void Grenade()
    {
        currentGrenade--;

        if(currentGrenade == 0)
        {
            weightByState[(int)EnemyAIState.Grenade] = 0;
        }
    }

    void Reload()
    {
        selectTime = currentTime + reloadTime;
        currentBullet = maxBullet;
        weightByState[(int)EnemyAIState.Reload] = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "SoundWave")
        {
            selectTime = currentTime + reactionTime;
        }
    }
}
