using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

public class Level : MonoBehaviour
{
    private const float PIPE_WDT = 7.8f;
    private const float PIPE_HEAD_HEIGHT = 3.75f;
    private const float CAM_ORTHO_SIZE = 50f;
    private const float PIPE_MOVE_SPD = 30f;
    private const float LEFT_LIMIT = -110f;
    private const float PIPE_SPWN_X = +100f;
    private const float BIRD_X_POS = 0;

    //To get properties from Score Window
    private static Level instance;

    private List<Pipe> pipeList;
    private int score = 0;
    private int allPipeCount;
    private float pipeSpawnTimer;
    private float pipeSpawnTimerMax;

    //Gets lower by time to make it more difficult
    private float gapSize;

    private State state;

    public enum Difficulty
    {
        Easy,
        Medium,
        Hard,
        Impossible,
    }

    private enum State
    {
        WaitingToStart,
        Playing,
        BirdDead,
    }

    public static Level getInstance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = this;
        pipeList = new List<Pipe>();
        pipeSpawnTimerMax = 1f;
        setDifficulty(Difficulty.Easy);
        state = State.WaitingToStart;
    }
    private void CreatePipe(float height, float xPosition,bool pipeOnBottom)
    {

        //Pipe Head setup
        Transform pipeHead = Instantiate(GameAssets.GetInstance().pfPipeHead);
        float pipeHeadYPosition;
        if (pipeOnBottom)
        {
            pipeHeadYPosition = -CAM_ORTHO_SIZE + height - PIPE_HEAD_HEIGHT * 0.5f;
        }
        else
        {
            pipeHeadYPosition = +CAM_ORTHO_SIZE - height - PIPE_HEAD_HEIGHT * 0.5f;
        }
        pipeHead.position = new Vector3(xPosition, pipeHeadYPosition);



        //Pipe Body setup

        Transform pipeBody = Instantiate(GameAssets.GetInstance().pfPipeBody);
        float pipeBodyYPosition;
        if (pipeOnBottom)
        {
            pipeBodyYPosition = -CAM_ORTHO_SIZE;
        }
        else
        {
            pipeBodyYPosition = +CAM_ORTHO_SIZE;
            pipeBody.localScale = new Vector3(1, -1, 1);
        }

        pipeBody.position = new Vector3(xPosition, pipeBodyYPosition);

        SpriteRenderer pipeBodySpriteRenderer = pipeBody.GetComponent<SpriteRenderer>();
        pipeBodySpriteRenderer.size = new Vector2(PIPE_WDT, height);

        BoxCollider2D pipeBodyBoxCollider = pipeBody.GetComponent<BoxCollider2D>();
        pipeBodyBoxCollider.size = new Vector2(PIPE_WDT, height);
        pipeBodyBoxCollider.offset = new Vector2(0f, height * 0.5f);

        Pipe aPipe = new Pipe(pipeHead, pipeBody,pipeOnBottom);
        pipeList.Add(aPipe);
    }
    private void Start()
    {
        Bird.getInstance().OnDied += Bird_OnDied;
        Bird.getInstance().OnStartedPlaying += Bird_OnStartedPlaying;
    }

    private void Bird_OnStartedPlaying(object sender, System.EventArgs e)
    {
        state = State.Playing;
    }

    private void Bird_OnDied(object sender, System.EventArgs e)
    {
        state = State.BirdDead;


    }

    private void Update()
    {
        if(state == State.Playing) { 
            handleMoveOfPipes();
            handleSpawnOfPipes();
        }
    }


    private void setDifficulty(Difficulty difficulty)
    {
        if(difficulty == Difficulty.Impossible)
        {
            gapSize = 25f;
            pipeSpawnTimerMax = 0.9f;
        }
        else if (difficulty == Difficulty.Hard)
        {
            gapSize = 35f;
            pipeSpawnTimerMax = 1.0f;
        }
        else if (difficulty == Difficulty.Medium)
        {
            gapSize = 43f;
            pipeSpawnTimerMax = 1.1f;
        }
        else
        {
            gapSize = 55f;
            pipeSpawnTimerMax = 1.2f;
        }
    }

    private Difficulty GetCurrentDifficulty()
    {
        if (allPipeCount >= 25) {return Difficulty.Impossible;}
        else if (allPipeCount >= 15) { return Difficulty.Hard; }
        else if (allPipeCount >= 10) { return Difficulty.Medium; }
        else { return Difficulty.Easy; }
    }

    private void handleSpawnOfPipes()
    {
        pipeSpawnTimer -= Time.deltaTime;
        if(pipeSpawnTimer < 0)
        {

            //Time to spawn another Pipe
            pipeSpawnTimer += pipeSpawnTimerMax;

            float heightEdgeLimit = 20f;
            float minHeight = gapSize * 0.5f + heightEdgeLimit;

            float totalHeight = CAM_ORTHO_SIZE * 2f;
            float maxHeight = totalHeight - gapSize * 0.5f - heightEdgeLimit;


            float height = Random.Range(minHeight, maxHeight);
            CreateGapPipes(height, gapSize, PIPE_SPWN_X);
        }
    }

    private void handleMoveOfPipes()
    {
        for (int i = 0; i < pipeList.Count; i++)
        {
            Pipe pipe = pipeList[i];

            bool isToTheRightOfBird = pipe.getXPos() > BIRD_X_POS;

            pipe.move();
            if(isToTheRightOfBird && (pipe.getXPos() <= BIRD_X_POS))
            {
                //Pipe was right of the bird and passed it.
                if(pipe.isBottomPipe)
                    score++;
            }
            if (pipe.getXPos() < LEFT_LIMIT)
            {
                //Destroy Pipe
                pipe.destroySelf();
                pipeList.Remove(pipe);
                i--;
            }
        }
    }


    public int returnScore()
    {
        return score;
    }


    private void CreateGapPipes(float gapY,float gapSize,float xPosition)
    {

        CreatePipe(gapY - gapSize * 0.5f, xPosition, true);
        CreatePipe(CAM_ORTHO_SIZE*2f - gapY - gapSize* 0.5f, xPosition, false);
        allPipeCount++;
        setDifficulty(GetCurrentDifficulty());

    }



    //Single Pipe
    private class Pipe
    {
        public bool isBottomPipe;
        private Transform pipeHeadTransform;
        private Transform pipeBodyTransform;

        public Pipe(Transform pipeHeadTransform, Transform pipeBodyTransform, bool isBottomPipe)
        {
            this.isBottomPipe = isBottomPipe;
            this.pipeHeadTransform = pipeHeadTransform;
            this.pipeBodyTransform = pipeBodyTransform;
        }

        public void move()
        {
            pipeHeadTransform.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPD * Time.deltaTime;
            pipeBodyTransform.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPD * Time.deltaTime;
        }

        public float getXPos()
        {
            return pipeHeadTransform.position.x;
        }

        public void destroySelf()
        {
            Destroy(pipeHeadTransform.gameObject);
            Destroy(pipeBodyTransform.gameObject);
        }
    }

}
