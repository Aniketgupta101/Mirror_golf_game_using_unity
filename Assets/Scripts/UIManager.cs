using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Script to control game UI
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] private Image powerBar;        //ref to powerBar image
    [SerializeField] private Text shotText;
    private double shotCount = 10;
    private double showshotCount;         //ref to shot info text
    [SerializeField] private GameObject mainMenu, gameMenu, gameOverPanel, retryBtn, nextBtn;   //important gameobjects
    [SerializeField] private GameObject container, lvlBtnPrefab;    //important gameobjects

    public Text ShotText { get { return shotText; } }   //getter for shotText
    public Image PowerBar { get => powerBar; }          //getter for powerBar

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        powerBar.fillAmount = 0;
        shotText.text = shotCount.ToString();                       //set the fill amount to zero
    }

    public void ShotTaken()
    {   
        if (shotCount > 0)                                          //if shotcount is more than 0
        {
            shotCount-=0.5;
                                                    //reduce it by 1
            shotText.text = shotCount.ToString();    //set the text

            if (shotCount <= 0)                                     //if shotCount is less than 0
            {
                SceneManager.LoadScene("MainMenu");                                          //Level failed
            }
        }
    }

    /// <summary>
    /// Method which spawn levels button
    /// </summary>
    // void CreateLevelButtons()
    // {
    //     //total count is number of level datas
    //     for (int i = 0; i < LevelManager.instance.levelDatas.Length; i++)
    //     {
    //         GameObject buttonObj = Instantiate(lvlBtnPrefab, container.transform);   //spawn the button prefab
    //         buttonObj.transform.GetChild(0).GetComponent<Text>().text = "" + (i + 1);   //set the text child
    //         Button button = buttonObj.GetComponent<Button>();                           //get the button componenet
    //         button.onClick.AddListener(() => OnClick(button));                          //add listner to button
    //     }
    // }

    /// <summary>
    /// Method called when we click on button
    /// </summary>
    void OnClick(Button btn)
    {
        mainMenu.SetActive(false);                                                      //deactivate main menu
        gameMenu.SetActive(true);                                                       //activate game manu
        GameManager.singleton.currentLevelIndex = btn.transform.GetSiblingIndex(); ;    //set current level equal to sibling index on button
        LevelManager.instance.SpawnLevel(GameManager.singleton.currentLevelIndex);      //spawn level
    }

    /// <summary>
    /// Method call after level fail or win
    /// </summary>
    public void GameResult()
    {
        switch (GameManager.singleton.gameStatus)
        {
            case GameStatus.Complete:                       //if completed
                gameOverPanel.SetActive(true);              //activate game finish panel
                nextBtn.SetActive(true);                    //activate next button
                SoundManager.instance.PlayFx(FxTypes.GAMECOMPLETEFX);
                break;
            case GameStatus.Failed:                         //if failed
                gameOverPanel.SetActive(true);              //activate game finish panel
                retryBtn.SetActive(true);                   //activate retry button
                SoundManager.instance.PlayFx(FxTypes.GAMEOVERFX);
                break;
        }
    }

    //method to go to main menu
    public void HomeBtn()
    {
        GameManager.singleton.gameStatus = GameStatus.None;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    //method to reload scene
    public void NextRetryBtn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


}