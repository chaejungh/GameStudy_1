using System.Timers;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum  Choice{ None, Scissors, Rock, Paper, Restart, GameOver,WinPlayer, WinCom}
    
    [Header("Result Text")]
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI gameOverText;
    
    
    [Header("UI Buttons")]
    public Button buttonScissors;
    public Button buttonRock;
    public Button buttonPaper;
    public Button buttonRestart;
    public Button buttonGameOver;
    public Button buttonWinPlayer;
    public Button buttonWinCom;

    [Header("Display Images")]
    public Image imagePlayer;
    public Image imageComputer;
    
    [Header("Sprites")]
    public Sprite spriteScissors;
    public Sprite spriteRock;
    public Sprite spritePaper;
    public Sprite spriteQuestion;

    [Header("Score Board")] 
    public TextMeshProUGUI textScorePlayer;
    public TextMeshProUGUI textScoreComputer;
    
    [Header("Panel")] 
    public GameObject panel;

    private int scorePlayer = 0;
    private int scoreComputer = 0;

    
    
    //스프라이트 순환용 변수
    private bool isAnimating = true;
    private float animationInterval = 0.1f;
    private float animationTimer = 0f;
    private int currentSpriteIndex = 0;
    private Sprite[] sprites;
    //게임이 진행되었는지 야부
    private bool isGaming=false;
    private bool GameOver=false;
    
    //플레이어가 선택한 값 저장
    private Choice playerChoice = Choice.None;
    
    
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sprites = new Sprite[] {spriteScissors, spriteRock, spritePaper,spriteQuestion};
        
        buttonScissors.onClick.AddListener(() => OnPlayerChoice(Choice.Scissors));
        buttonRock.onClick.AddListener(() => OnPlayerChoice(Choice.Rock));
        buttonPaper.onClick.AddListener(() => OnPlayerChoice(Choice.Paper));
        buttonRestart.onClick.AddListener(() => OnPlayerChoice(Choice.Restart));
        buttonGameOver.onClick.AddListener(() => OnPlayerChoice(Choice.GameOver));
        buttonWinPlayer.onClick.AddListener(() => OnPlayerChoice(Choice.WinPlayer));
        buttonWinCom.onClick.AddListener(() => OnPlayerChoice(Choice.WinCom));
        
        //결과 텍스트 초기화
        panel.SetActive(false);
        resultText.text = "Please choose Rock, Scissors, or Paper!";
    }

    void OnPlayerChoice(Choice choice)
    {
        if (choice==Choice.WinCom)
        {
            scoreComputer+=5;
            DetermineWinner(Choice.Rock, Choice.Paper);
        }else if(choice == Choice.WinPlayer)
        {
            scorePlayer += 5;
            DetermineWinner(Choice.Paper, Choice.Rock);
        }
        if (isGaming)
        {
            ResetGame();
            isGaming=!isGaming;
            return;
        }

        if (GameOver)
        {
            DeterMineGameOver(choice);
            isGaming = false;
            return;
        }
        isAnimating = false;
        
        playerChoice = choice;
        Debug.Log("플레이어 선택: " + choice.ToString());
        //컴퓨터 선택 및 승부 판정
        Choice computerChoice = GetComputerChoice();
        Debug.Log("컴퓨터 선택: "+ computerChoice.ToString());

        imagePlayer.sprite = GetSpriteFromChoice(playerChoice);
        imageComputer.sprite = GetSpriteFromChoice(computerChoice);
        string result = DetermineWinner(playerChoice, computerChoice);
        textScorePlayer.text = scorePlayer.ToString();
        textScoreComputer.text = scoreComputer.ToString();
        resultText.text = result;
        isGaming = !isGaming;
        Debug.Log("결과: "+ result);
    }

    void ResetGame()
    {
        resultText.text = "Please choose Rock, Scissors, or Paper!";
        isAnimating = true;
        playerChoice = Choice.None;
        imagePlayer.sprite = spriteQuestion;
        Debug.Log("Reset Game");
        
        
    }

    void OnExit()
    {
        #if UNITY_EDITOR
                EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }

    Sprite GetSpriteFromChoice(Choice choice)
    {
        switch (choice)
        {
            case Choice.Scissors: return spriteScissors;
            case Choice.Rock: return spriteRock;
            case Choice.Paper: return spritePaper;
            default: return spriteRock;
        }
    }
    Choice GetComputerChoice()//컴퓨터가 랜덤으로 가위/바위/보 중에 선택
    {
        int random = Random.Range(0, 3);
        switch (random)
        {
            case 0: return Choice.Scissors;
            case 1: return Choice.Rock;
            case 2: return Choice.Paper;
            default: return Choice.Rock;
        }
    }
    
    string DetermineWinner(Choice player, Choice computer)
    {
        if (player == computer)
            return "무승부!";
        
        //플레이어 승리 조건: 가위->보, 바위->가위 -> 보->바위
        bool playWins = (player == Choice.Scissors && computer == Choice.Paper) ||
                        (player == Choice.Rock && computer == Choice.Scissors) ||
                        (player == Choice.Paper && computer == Choice.Rock);

        if (playWins)
        {
            scorePlayer++;
            if (scorePlayer >= 5)
            {
                //게임종료
                panel.SetActive(true);
                gameOverText.text = "게임 오버!!!\n 플레이어 승리!!!(5점) ";
                GameOver = true;
            }
        }
        else
        {
            
            scoreComputer++;
            if (scoreComputer >= 5)
            {
                //게임종료
                panel.SetActive(true);
                gameOverText.text = "게임 오버!!!\n 컴퓨터 승리!!! (5점) ";
                GameOver = true;
            }
        }
        return playWins ? "플레이어 승리!" : "컴퓨터 승리";
    }

    void DeterMineGameOver(Choice choice)
    {
        if (choice == Choice.Restart)
        {
            ResetGame();
            scorePlayer = 0;
            scoreComputer = 0;
            gameOverText.text = "";
            panel.SetActive(false);
            return;
        }
        else if (choice == Choice.GameOver)
        {
            OnExit();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isAnimating)
        {
            animationTimer += Time.deltaTime;
            if (animationTimer >= animationInterval)
            {
                animationTimer = 0f;
                currentSpriteIndex = (currentSpriteIndex + 1) % 3;
                
                //두 이미지가 서로 다른 스프라이트를 보여주도록
                imageComputer.sprite = sprites[(currentSpriteIndex + 1) % 3];
            }
        }
    }
}
