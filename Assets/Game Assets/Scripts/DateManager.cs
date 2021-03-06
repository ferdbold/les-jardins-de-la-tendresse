﻿using UnityEngine;
using System.Collections.Generic;
using Tendresse.Data;
using Tendresse.Date;
using Message;

namespace Tendresse.Date {
    //Date Structure
    public class DateStructure {
        public string theme;
        public string intro;
        public string dateName;
        public string yourName;
        public int ID;
        public int relationLevel;
        public List<DateEvent> DateEvents;

        public DateStructure(string t, string i, int id, int rl) {
            theme = t;
            intro = i;
            ID = id;
            relationLevel = rl;
            DateEvents = new List<DateEvent>();
            dateName = "";
            yourName = "";
        }
    }

    //Date Event
    public class DateEvent {
        public string question; //Question to be asked
        public string answer; //Stored answer written for the question
        public TendresseData image; //Store image drawn for the question
        public bool mediaIsDrawing;
        public message sound;

        public void SetAnswer(string var) { answer = var; }
        public void SetSound(message var) { sound = var; }
        public void SetImage(TendresseData var) { image = var; }

        public DateEvent(string q, bool d) {
            question = q;
            answer = "";
            image = new TendresseData();
            mediaIsDrawing = d;
            sound = new message("sendSound");
        }
    }
}

public class DateManager : MonoBehaviour {

    static public DateManager instance;

    [Header("Date Management")]
    public DateStructure Date; //List of date events to execute
    private int _currentDateEvent = -1; //Current Date event
    private int _currentStepInDate = 0; //Current Step in the event ( 0 = first player draws , 1 = second player writes, 2 = text resolution, then start next )
    private bool _canUseConfirmButton = false;

    [Header("Draw Zones")]
    public GameObject DrawingObjectPrefab;
    public TouchDraw mainTouchDraw; //Main space to draw. This is where the player draws.
    private List<TouchDraw> tempDrawingList = new List<TouchDraw>(); //Temporary image shown, used to show drawings but not to draw !

    public List<message> listSound = new List<message>();

    [Header("Components")]
    [SerializeField]
    private HUD HUD;

    void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    void OnDestroy() {
        if (instance = this) {
            instance = null;
        }
    }


    void Update() {
        DebugMethods();
    }

    /////////////////////////// MAKE DRAWINGS ////////////////////////
    #region Make Drawing
    /// <summary>
    /// Draw a temporary drawing into a newly created gameobject
    /// </summary>
    /// <param name="tData"></param>
    /// <param name="imagePosition"></param>
    /// <param name="imageScale"></param>
    public void DrawTempImageAt(TendresseData tData, Vector3 imagePosition, float imageScale) {
        GameObject go = (GameObject)Instantiate(DrawingObjectPrefab, imagePosition, Quaternion.identity);
        TouchDraw touchDraw = go.GetComponent<TouchDraw>();
        touchDraw.canDraw = false; //Cant draw in temporaty touch  <--- Yes, there is a typo, too lazy to fix. Yes Am Tired. KthxBye.
        touchDraw.LoadTendresseData(tData, imagePosition, imageScale);

        tempDrawingList.Add(touchDraw);
    }

    /// <summary>
    /// Delete all temporary drawings 
    /// </summary>
    public void DeleteTempImage() {
        for (int i = 0; i < tempDrawingList.Count; i++) {
            Destroy(tempDrawingList[i].gameObject);
        }
        tempDrawingList = new List<TouchDraw>();
    }

    /// <summary>
    /// Draw into the main image Draw
    /// </summary>
    /// <param name="tData"></param>
    /// <param name="imagePosition"></param>
    /// <param name="imageScale"></param>
    public void DrawImageAt(TendresseData tData, Vector3 imagePosition, float imageScale) {
        if (mainTouchDraw != null) {
            mainTouchDraw.LoadTendresseData(tData, imagePosition, imageScale);
        }
    }



    private void DebugMethods() {

        if (Input.GetKeyDown(KeyCode.Z)) {
            GameManager.instance.Event_OnSendImage(mainTouchDraw.SaveCurrentData());
        }
        if (Input.GetKeyDown(KeyCode.X)) {

        }
    }
    #endregion

    /////////////////////////////////////////////// Date Management ///////////////////////////////
    #region Date Managment

    /// <summary>
    /// Make new Date and starts the intro stage of the date
    /// </summary>
    /// <param name="theme"></param>
    /// <param name="intro"></param>
    /// <param name="id"></param>
    public void OnStartNewDate(string theme, string intro, int id, int relationLevel) {
        Date = new DateStructure(theme, intro, id, relationLevel);
        _currentDateEvent = -1;
        //DateManager.instance.SendMessage_OnConfirm("Roger Toupin");
    }

    public void OnGetOwnName(string name){
        Date.yourName = name;
        GameObject.Find("UI").GetComponent<HUD>().Event_OnBeginDate();
       
    }

    /// <summary>
    /// Make new Event and starts it
    /// </summary>
    /// <param name="eventText"></param>
    public void OnStartNewEvent(string eventText, bool mediaIsDrawing) {
        Date.DateEvents.Add(new DateEvent(eventText, mediaIsDrawing));
        _currentDateEvent++;
        //listSound.Add(new message(""));
        GameObject.Find("UI").GetComponent<HUD>().Event_OnBeginEvent();
    }

    /// <summary>
    /// Player has clicked the submit button.
    /// </summary>
    public void OnConfirmEntry() {
        //If first player presses confirm when he draws, start text phase
        //ExecuteDateEvent_TextPhase();
        if (_currentStepInDate == 0) 
        {
            _currentStepInDate++;
            SendMessage_OnConfirmMedia();
        } 
        else if (_currentStepInDate == 1) { //If second player presses confirm when he writes, start end phase
            //ExecuteDateEvent_EndPhase(Date.DateEvents[_currentDateEvent]);
        }
    }

    /*/// <summary>
    /// Execute a complete Date Event
    /// </summary>
    /// <param name="dateEvent"></param>
    private void ExecuteDateEvent() {
        if (_currentDateEvent < Dates[_currentDate].DateEvents.Count) { //If there is still events to do in list
            Debug.Log("Executing Date Event ");
            
        } else {
            Debug.Log("Executed last date event in list. Date over.");
        }
    }*/

    /// <summary>
    /// Start the draw phase of the event. First player draws while second player wait.
    /// </summary>
    /// <param name="dateEvent"></param>
    private void ExecuteDateEvent_DrawPhase(DateEvent dateEvent) {
        //GameObject.Find("UI").GetComponent<HUD>().
        //.confirmButton.SetActive(IAmFirst());
    }

    /// <summary>
    /// Starts the Text Phase of the Event where the second player write about the drawing while the first player waits.
    /// </summary>
    /// <param name="dateEvent"></param>
    public void ExecuteDateEvent_TextPhase() {
        HUD.Event_OnReceivedMedia();
        //mainPage.confirmButton.SetActive(!IAmFirst());
    }

    public void ExecuteDateEvent_OnReceiveText(string text) {
        GetCurrentEvent().SetAnswer(text);
        HUD.Event_OnReceivedText();
        SendMessage_OnConfirm(Date.dateName);
    }

    /// <summary>
    /// Starts the last part of the event where the resolution is shown
    /// </summary>
    /// <param name="dateEvent"></param>
    public void ExecuteDateEvent_EndPhase() {
        GameManager.instance.SwitchScene(GameManager.Scenes.Menu);
    }

    /// <summary>
    /// Returns where you are the first player for this date event.
    /// Player 1 is first for even numbered events
    /// Player 2 is first for un-even numbered events
    /// </summary>
    /// <returns></returns>
    public bool IAmFirst() {
        Debug.Log(_currentDateEvent + " "+GameManager.instance.isFirst);
        if (_currentDateEvent % 2 == 0) {
            if (GameManager.instance.isFirst == true) return true;
            else return false;
        } else {
            if (GameManager.instance.isFirst == true) return false;
            else return true;
        }
    }

    /// <summary>
    /// Returns the current event
    /// </summary>
    /// <returns> current event </returns>
    public DateEvent GetCurrentEvent() {
        return Date.DateEvents[_currentDateEvent];
    }

    public void SetCurrentEventSound(message messa) {
        GetCurrentEvent().SetSound(messa);
    }

    public DateStructure GetCurrentDate() {
        return Date;
    }

    #endregion

    /////////////////////////////////////////////// Send Date Message ///////////////////////////////

    /// <summary>
    /// Send a message to confirm that the game starts
    /// </summary>
    /// <param name="name"> Name of the partner's name </param>
    public void SendMessage_OnConfirm(string name) {
        //DateManager.instance.OnConfirmEntry();
        message messa = new message("dateReady");
        messa.addNetObject(new NetObject(""));
        messa.getNetObject(0).addString("", name);
        Date.dateName = name;
        NetManager.instance.SendMessage(messa);
    }

    public void SendMessage_OnConfirmHandle(string text) {
        if (IAmFirst()) {
            SendMessage_OnConfirmMedia();
        }
        else {
            SendMessage_OnConfirmText(text);
        }
    }

    /// <summary>
    /// When sending media to server
    /// </summary>
    public void SendMessage_OnConfirmMedia() {
        GameObject.Find("UI").GetComponent<HUD>().Event_OnReceivedMedia();
        if (GetCurrentEvent().mediaIsDrawing)
        {
            GetCurrentEvent().SetImage(mainTouchDraw.SaveCurrentData());
            GameManager.instance.Event_OnSendImage(GetCurrentEvent().image);
        }
        else {
             SetCurrentEventSound(AudioManager.instance.getRecordMessage());
             GameManager.instance.Event_OnSendSound(GetCurrentEvent().sound);
        }
    }

    /// <summary>
    /// Send a message when you press the button to confirm text
    /// </summary>
    /// <param name="text"></param>
    public void SendMessage_OnConfirmText(string text) {
        GetCurrentEvent().SetAnswer(text);
        message messa = new message("sendText");
        messa.addNetObject(new NetObject(""));
        messa.getNetObject(0).addString("", text);
        NetManager.instance.SendMessage(messa);
    }

    public void SendMessage_DisconnectFromServer() {
        message messa = new message("disconnect");
        NetManager.instance.SendMessage(messa);
    }
}
