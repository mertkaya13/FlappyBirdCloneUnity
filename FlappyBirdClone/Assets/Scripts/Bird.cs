using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    private const float JUMP_AMNT = 100;
    private Rigidbody2D birdrigidbody2D;


    private static Bird instance;

    public static Bird getInstance()
    {
        return instance;
    }

    public event EventHandler OnDied;
    public event EventHandler OnStartedPlaying;
    private State state;

    private enum State
    {
        WaitingToStart,
        Playing,
        Dead
    }
    private void Awake()
    {

        instance = this;
        birdrigidbody2D = GetComponent<Rigidbody2D>();
        birdrigidbody2D.bodyType = RigidbodyType2D.Static;
        state = State.WaitingToStart;

    }
    // Update is called once per frame
    private void Update()
    {

        if(state == State.WaitingToStart)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButton(0))
            {
                Jump();
                SoundManager.playSound();
                state = State.Playing;
                birdrigidbody2D.bodyType = RigidbodyType2D.Dynamic;
                if(OnStartedPlaying != null) { OnStartedPlaying(this, EventArgs.Empty); }
            }
        }else if (state == State.Playing)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButton(0))
            {
                birdrigidbody2D.bodyType = RigidbodyType2D.Dynamic;
                Jump();
                SoundManager.playSound();
            }
        }


    }

    private void Jump()
    {
        birdrigidbody2D.velocity = Vector2.up * JUMP_AMNT;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (OnDied != null) { OnDied(this, EventArgs.Empty); }
        state = State.Dead;
        birdrigidbody2D.bodyType = RigidbodyType2D.Static;

    }
}
