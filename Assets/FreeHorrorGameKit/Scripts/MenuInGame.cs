using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;
using System.IO;

public class MenuInGame : MonoBehaviour
{
    [Header("UI Settings")]
    public GameObject MainCanvas;
    public GameObject OptionsUI;
    public Toggle fullscreenToggle;
    public Dropdown resolutionDropdown;
    public Dropdown vSyncDropdown;
    public Dropdown antialiasingDropdown;
    public Dropdown textureQualityDropdown;
    public Slider musicVolumeSlider;
    public AudioSource MapAudio;
    public Button applyButton;
    public Resolution[] resolutions;
    public GameSettings gameSettings;

    // Scene
    Scene m_Scene;

    void OnEnable()
    {
        // load configs
        gameSettings = new GameSettings();

        // set all UI functions
        fullscreenToggle.onValueChanged.AddListener(delegate { OnFullscreenToggle(); });
        resolutionDropdown.onValueChanged.AddListener(delegate { OnResolutionChange(); });
        textureQualityDropdown.onValueChanged.AddListener(delegate { OnTextureQualityChange(); });
        antialiasingDropdown.onValueChanged.AddListener(delegate { OnAntialiasingChange(); });
        vSyncDropdown.onValueChanged.AddListener(delegate { OnVSyncChange(); });
        musicVolumeSlider.onValueChanged.AddListener(delegate { OnMusicVolumeChange(); });
        applyButton.onClick.AddListener(delegate { OnApplyButtonClick(); });

        // get resolutions
        resolutions = Screen.resolutions;

        // set all avaliable resolutions to dropdown
        foreach(Resolution resolution in resolutions)
        {
            resolutionDropdown.options.Add(new Dropdown.OptionData(resolution.ToString()));
        }

        // load saved settings
        LoadSettings();
    }

    void Start()
    {
        // get current scene
        m_Scene = SceneManager.GetActiveScene();

        // check if is main menu scene
        if (m_Scene.buildIndex == 0)
            return;

        // add options button listener
        Button btnOptions = MainCanvas.gameObject.transform.Find("InGameMenuPanel").transform.Find("SettingsBtn").gameObject.GetComponent<Button>();
        btnOptions.onClick.AddListener(Options);

        // add close options button listener
        Button btnCloseOptions = MainCanvas.gameObject.transform.Find("OptionsUI").transform.Find("CloseOptionsBtn").gameObject.GetComponent<Button>();
        btnCloseOptions.onClick.AddListener(CloseOptions);

        // add play again button listener
        Button btnPlayAgain = MainCanvas.gameObject.transform.Find("InGameMenuPanel").transform.Find("PlayAgainBtn").gameObject.GetComponent<Button>();
        btnPlayAgain.onClick.AddListener(PlayAgain);

        // add quit button listener
        Button btnQuitGame = MainCanvas.gameObject.transform.Find("InGameMenuPanel").transform.Find("QuitBtn").gameObject.GetComponent<Button>();
        btnQuitGame.onClick.AddListener(QuitGame);

        // add continue button listener
        Button btnContinue = MainCanvas.gameObject.transform.Find("InGameMenuPanel").transform.Find("ContinueBtn").gameObject.GetComponent<Button>();
        btnContinue.onClick.AddListener(Continue);
    }

    private void Update()
    {
        // get current scene
        m_Scene = SceneManager.GetActiveScene();

        // check if is main menu scene
        if (m_Scene.buildIndex == 0)
            return;

        // if press escape on keyboard and your health is > 0 pause game and show in game menu
        if (Input.GetKeyDown(KeyCode.Escape) && this.gameObject.GetComponent<PlayerBehaviour>().health > 0)
        {
            MainCanvas.gameObject.transform.Find("InGameMenuPanel").gameObject.active = !MainCanvas.gameObject.transform.Find("InGameMenuPanel").gameObject.active;
            this.gameObject.GetComponent<PlayerBehaviour>().paused = !this.gameObject.GetComponent<PlayerBehaviour>().paused;
        }

        // if game is paused block first person controller and enable mouse cursor
        if (this.gameObject.GetComponent<PlayerBehaviour>().paused)
        {
            this.gameObject.GetComponent<FirstPersonController>().enabled = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        // active again first person controller and disable mouse cursor
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
            this.gameObject.GetComponent<FirstPersonController>().enabled = true;
        }         
    }

    public void Options()
    {
        // check if is main menu scene
        if (m_Scene.buildIndex == 0)
        {
            // show options UI    
            MainCanvas.gameObject.transform.Find("InGameMenuPanel").gameObject.SetActive(false);
            OptionsUI.SetActive(true);
        }
        else
        {
            // show options UI   and disable player behaviour  
            this.gameObject.GetComponent<PlayerBehaviour>().paused = true;
            MainCanvas.gameObject.transform.Find("InGameMenuPanel").gameObject.SetActive(false);
            OptionsUI.SetActive(true);
        }   
    }

    public void CloseOptions()
    {
        // close options UI
        MainCanvas.gameObject.transform.Find("InGameMenuPanel").gameObject.SetActive(true);
        OptionsUI.SetActive(false);
    }

    public void StartGame()
    {
        // load main game scene
        SceneManager.LoadScene(1);
    }

    public void PlayAgain()
    {
        // reload menu scene
        SceneManager.LoadScene(0);
    }

    public void Continue()
    {
        // continue game and disable all panels
        this.gameObject.GetComponent<PlayerBehaviour>().paused = false;
        MainCanvas.gameObject.transform.Find("InGameMenuPanel").gameObject.SetActive(false);
    }

    public void QuitGame()
    {
        // quit game
        Application.Quit();
    }

    // UI event
    void OnFullscreenToggle()
    {
       gameSettings.fullscren = Screen.fullScreen = fullscreenToggle.isOn;
    }

    // UI event
    void OnResolutionChange()
    {
        Screen.SetResolution(resolutions[resolutionDropdown.value].width, resolutions[resolutionDropdown.value].height, Screen.fullScreen);
        gameSettings.resolutionsIndex = resolutionDropdown.value;
    }

    // UI event
    void OnTextureQualityChange()
    {
        QualitySettings.masterTextureLimit  = gameSettings.textureQuality = textureQualityDropdown.value;        
    }

    // UI event
    void OnAntialiasingChange()
    {
        QualitySettings.antiAliasing =  gameSettings.antialiasing = (int)Mathf.Pow(2f, antialiasingDropdown.value);
    }

    // UI event
    void OnVSyncChange()
    {
        QualitySettings.vSyncCount = gameSettings.vSync = vSyncDropdown.value;
    }

    // UI event
    void OnMusicVolumeChange()
    {
        MapAudio.volume = musicVolumeSlider.value;
        gameSettings.musicVolume = MapAudio.volume;
    }

    void SaveSettings()
    {
        // save game settings to json file
        string jsonData = JsonUtility.ToJson(gameSettings, true);
        File.WriteAllText("Assets/FreeHorrorGameKit/gamesettings.json", jsonData);
    }

    void LoadSettings()
    {
        // load game settings from json file
        gameSettings = JsonUtility.FromJson<GameSettings>(File.ReadAllText("Assets/FreeHorrorGameKit/gamesettings.json"));
        musicVolumeSlider.value = gameSettings.musicVolume;
        antialiasingDropdown.value = gameSettings.antialiasing;
        vSyncDropdown.value = gameSettings.vSync;
        textureQualityDropdown.value = gameSettings.textureQuality;
        resolutionDropdown.value = gameSettings.resolutionsIndex;
        fullscreenToggle.isOn = gameSettings.fullscren;
        Screen.fullScreen = gameSettings.fullscren;
        resolutionDropdown.RefreshShownValue();
    }

    void OnApplyButtonClick()
    {
        // apply settings
        SaveSettings();
    }
}


