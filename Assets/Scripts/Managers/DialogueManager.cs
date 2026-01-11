using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[System.Serializable]
public struct DialogueLine
{
    public Sprite portrait;
    [TextArea(3, 10)] public string text;
}

[System.Serializable]
public struct NamedDialogue
{
    public string dialogueID;
    public DialogueLine[] conversation;
}

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    [Header("UI References")] public GameObject dialoguePanel;
    public Image portraitImage;
    public TextMeshProUGUI dialogueText;

    [Header("Settings")] public float typingSpeed = 0.05f;
    private Queue<DialogueLine> lines;
    private Action onDialogueEndCallback;
    private bool isTyping = false;
    public bool IsActive { get; private set; } = false;
    private DialogueLine currentLine;

    [Header("Dialogue Data")] public List<NamedDialogue> dialogueLibrary;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Debug.LogError("Duplicated Dialogue Manager", gameObject);
        lines = new Queue<DialogueLine>();
    }

    void Start() => dialoguePanel.SetActive(false);

    void Update()
    {
        if (dialoguePanel.activeSelf && Input.GetKeyDown(KeyCode.E))
        {
            if (isTyping) CompleteSentence();
            else DisplayNextSentence();
        }
    }

    public void TriggerDialogue(string id, Action callback = null)
    {
        NamedDialogue found = dialogueLibrary.Find(d => d.dialogueID == id);
        if (found.conversation is { Length: > 0 })
            StartDialogue(found.conversation, callback);
        else
            Debug.LogError("Dialogue ID `" + id + "` not found");
    }

    public void StartDialogue(DialogueLine[] dialogueConversation, Action callback = null)
    {
        onDialogueEndCallback = callback;
        Time.timeScale = 0f;
        IsActive = true;
        dialoguePanel.SetActive(true);
        lines.Clear();
        
        foreach (DialogueLine line in dialogueConversation)
        {
            lines.Enqueue(line);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (lines.Count == 0)
        {
            EndDialogue();
            return;
        }

        currentLine = lines.Dequeue();
        portraitImage.sprite = currentLine.portrait;

        StopAllCoroutines();
        StartCoroutine(TypeSentence(currentLine.text));
    }

    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogueText.text = sentence;
        dialogueText.maxVisibleCharacters = 0;
        dialogueText.ForceMeshUpdate();
        int totalVisibleCharacters = dialogueText.textInfo.characterCount;
        for (int i = 0; i <= totalVisibleCharacters; i++)
        {
            dialogueText.maxVisibleCharacters = i;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }

        isTyping = false;
    }

    private void CompleteSentence()
    {
        StopAllCoroutines();
        dialogueText.ForceMeshUpdate();
        dialogueText.maxVisibleCharacters = dialogueText.textInfo.characterCount;
        isTyping = false;
    }

    public void EndDialogue()
    {
        StartCoroutine(ResetActiveFlag());
    }

    private IEnumerator ResetActiveFlag()
    {
        yield return new WaitForEndOfFrame();
        Time.timeScale = 1f;
        IsActive = false;
        dialoguePanel.SetActive(false);
        if (onDialogueEndCallback != null)
        {
            onDialogueEndCallback.Invoke();
            onDialogueEndCallback = null;
        }
    }
}