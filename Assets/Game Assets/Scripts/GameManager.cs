﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Message;
using Tendresse.Data;

public class GameManager : MonoBehaviour {

    static public GameManager instance;

    public bool isFirst; //TODO :The server chooses a first and second player in the date. THIS DOES NOT CHANGE DURING THE DATE !
    private bool finishedAGame = false;


    public AudioClip music_Intro;

    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    void Start() {
        LoadData();
    }

    void OnDestroy() {
        if (instance == this) {
            instance = null;
            SaveData();
           
        }
    }

    //-----------------------------------------------------------------------------------------------------------------------
    //////////////////////////////////////////////// SAVE AND LOAD ///////////////////////////////////////////////////////////


    public void LoadData() {
        SaveAndLoad.Load();
        Debug.Log("Length : " + SaveAndLoad.savedGame.lenght);
    }

    public void SaveData() {
        SaveAndLoad.Save();
        Debug.Log("Length : " + SaveAndLoad.savedGame.lenght);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.P)){
            SaveAndLoad.DeleteSaves();
        }

        if (Input.GetKeyDown(KeyCode.O)) {
            SaveAndLoad.savedGame.lenght++;
            Debug.Log("Length : " + SaveAndLoad.savedGame.lenght);
        }
    }

    //-----------------------------------------------------------------------------------------------------------------------
    //////////////////////////////////////////////// SWITCH SCENE ///////////////////////////////////////////////////////////

    //VARIABLES
    Scenes currentScene;

    public enum Scenes {
        Menu,
        LoadingGame,
        Main
    }

    //FUNCTIONS

    /// <summary>
    /// Switch between scenes
    /// </summary>
    /// <param name="scene"></param>
    public void SwitchScene(Scenes scene) {
        OnSceneEnd(currentScene);
        currentScene = scene;
        OnSceneStartup(currentScene);
    }

    /// <summary>
    /// Event when the scene end
    /// </summary>
    /// <param name="scene"></param>
    void OnSceneEnd(Scenes scene) {
        switch (scene) {
            case Scenes.Menu:
                GameObject.FindWithTag("MainMenuRef").GetComponent<MainMenuRefUI>().mainMenu.DOFade(0, 0.75f);
                AudioManager.instance.StopMusic();
                break;
            case Scenes.LoadingGame:
                CanvasGroup loading = GameObject.FindWithTag("MainMenuRef").GetComponent<MainMenuRefUI>().loading;
                loading.DOFade(0, 0.75f);
                loading.GetComponent<LoadingUI>().StopAllCoroutines();
                break;
            case Scenes.Main:
                finishedAGame = true;
                break;
        }
    }

    /// <summary>
    /// Event on scene startup
    /// </summary>
    /// <param name="scene"></param>
    void OnSceneStartup(Scenes scene) {
        switch (scene) {
            case Scenes.Menu:
                Application.LoadLevel("Menu");
                if (finishedAGame) GameObject.FindGameObjectWithTag("cucumber").GetComponent<TiltShakeMotor>().isInTendresse = true;
                break;
            case Scenes.LoadingGame:
                CanvasGroup loading = GameObject.FindWithTag("MainMenuRef").GetComponent<MainMenuRefUI>().loading;
                loading.DOFade(1, 0.75f);
                loading.GetComponent<LoadingUI>().StartAnim();
                message messa = new message("queueMatch");
                NetManager.instance.SendMessage(messa);
                break;
            case Scenes.Main:
                Application.LoadLevel("Main");
                message mes = new message("requestDateStart");
                NetManager.instance.SendMessage(mes);
                break;
        }
    }

//-----------------------------------------------------------------------------------------------------------------------
////////////////////////////////////////////////              ///////////////////////////////////////////////////////////

    /// <summary>
    /// Event when the player finds a partner online
    /// </summary>
    public void Event_OnFindPartner(bool _isFirst) {
        isFirst = _isFirst;
        SwitchScene(Scenes.Main);
    }

    public void Event_OnSendImage(TendresseData tData) {
        Debug.Log("Beginning Send Message");
        DateManager.instance.GetCurrentEvent().SetImage(tData);
        message mes = NetManager.instance.MakeMessageFromImage(tData);
        Debug.Log("Created Message" + conversionTools.convertMessageToString(mes));
        NetManager.instance.SendMessage(mes);
        Debug.Log("Sent Message");
    }

    public void Event_OnReceiveImage(TendresseData tData) {
        Debug.Log("draw 1");
        //DateManager.instance.DrawImageAt(tData, Vector3.zero, 1f);
        DateManager.instance.GetCurrentEvent().SetImage(tData);
        DateManager.instance.ExecuteDateEvent_TextPhase();
    }

    public void Event_OnReveiceSound(message messa) {
        DateManager.instance.GetCurrentEvent().SetSound(messa);
        DateManager.instance.ExecuteDateEvent_TextPhase();
        AudioManager.instance.PlaySoundFromMessage(messa);
    }

    public void Event_OnSendSound(message messa) {
        NetManager.instance.SendMessage(messa);
        Debug.Log("Sent Message");
    }
    
}
