﻿using UnityEngine;
using UnityEngine.UI;
using Tendresse.Date;

public class HUD : MonoBehaviour {

    [Header("Components")]
    [SerializeField]
    private IntroView introView;
    [SerializeField]
    private StoryView storyView;
    [SerializeField]
    private MessageView messageView;

    public void Awake() {
        this.AdaptToScreenSize();
    }

    public void Start() {
        //this.PushTestDateEvents();
    }

    /// <summary>
    /// Push test date events into story view. Do not use in release.
    /// </summary>
    private void PushTestDateEvents() {
        DateEvent testDateEvent = new DateEvent("Test", true);

        testDateEvent.question = "À ce moment, il fut clair que Joséphine ne pouvait résister au __________ de Roger.";
        testDateEvent.answer = "gros criss de tracteur trois vitesses de Canadian Tire";
        testDateEvent.mediaIsDrawing = false;

        this.storyView.OnNewDateEvent(testDateEvent);
        //this.storyView.OnNewDateEvent(testDateEvent);

        //this.messageView.OnNewDateEvent(testDateEvent, false);

        //this.messageView.OnNewDateEvent(testDateEvent, true);
    }

    /// <summary>
    /// Adjusts the size of elements so it fits all phone screen sizes.
    /// </summary>
    private void AdaptToScreenSize() {
        RectTransform introViewRectTransform = this.introView.GetComponent<RectTransform>(),
                      storyViewRectTransform = this.storyView.GetComponent<RectTransform>(),
                      messageViewRectTransform = this.messageView.GetComponent<RectTransform>();
        Vector2 introViewSize = introViewRectTransform.sizeDelta,
                storyViewSize = storyViewRectTransform.sizeDelta,
                messageViewSize = messageViewRectTransform.sizeDelta;

        introViewSize.y = Screen.height;
        storyViewSize.y = Screen.height;
        messageViewSize.y = Screen.height;

        introViewRectTransform.sizeDelta = introViewSize;
        storyViewRectTransform.sizeDelta = storyViewSize;
        messageViewRectTransform.sizeDelta = messageViewSize;
    }

    /// <summary>
    /// Called the date begins
    /// </summary>
    public void Event_OnBeginDate() {
        this.introView.OnBeginDate();
        this.storyView.OnBeginDate();
    }

    /// <summary>
    /// Called when a new event is started
    /// </summary>
    public void Event_OnBeginEvent() {
        this.storyView.OnNewDateEvent(DateManager.instance.GetCurrentEvent());
        this.messageView.OnNewDateEvent(DateManager.instance.GetCurrentEvent(), DateManager.instance.IAmFirst());
    }

    /// <summary>
    /// Called when your partner end his drawing
    /// </summary>
    public void Event_OnReceivedMedia() {
        this.storyView.OnReceivedMediaEvent(DateManager.instance.GetCurrentEvent());
        this.messageView.OnReceivedMediaEvent(DateManager.instance.GetCurrentEvent(), DateManager.instance.IAmFirst());
    }

    /// <summary>
    /// Called when your partner submitted a new answer
    /// </summary>
    public void Event_OnReceivedText() {
        this.storyView.OnNewDateEventTextAnswer(DateManager.instance.GetCurrentEvent());
    }
}
