using UnityEngine;
using UnityEngine.SceneManagement;

public class BunnyInteractions : MonoBehaviour
{
    private bool seenIntro = false;
    private bool seenEnding = false;
    
    public void InteractionIntro()
    {
        if (!seenIntro)
        {
            seenIntro = true;
            DialogueManager.instance.TriggerDialogue("WelcomeLevel1Intro");
            return;
        }
        int carrots = 0;
        carrots += KeyManager.instance.hasNormalKey ? 1 : 0;
        carrots += KeyManager.instance.hasGoldenKey ? 1 : 0;
        carrots += KeyManager.instance.hasDiamondKey ? 1 : 0;
        if (carrots >= 3)
        {
            DialogueManager.instance.TriggerDialogue("YouFoundGoToEnd");
            return;
        }
        if (!seenEnding)
        {
            
            DialogueManager.instance.TriggerDialogue("Level1Intro");
            return;
        }
        DialogueManager.instance.TriggerDialogue("StillNothing");
    }
    
    public void InteractionEnding()
    {
        seenEnding = true;
        int carrots = 0;
        carrots += KeyManager.instance.hasNormalKey ? 1 : 0;
        carrots += KeyManager.instance.hasGoldenKey ? 1 : 0;
        carrots += KeyManager.instance.hasDiamondKey ? 1 : 0;

        switch (carrots)
        {
            case 0:
                DialogueManager.instance.TriggerDialogue("NoCarrots");
                break;
            case 1:
                DialogueManager.instance.TriggerDialogue("OneCarrot");
                if (!KeyManager.instance.hasNormalKey)
                    DialogueManager.instance.TriggerDialogue("NormalCarrotMissing");
                if (!KeyManager.instance.hasGoldenKey)
                    DialogueManager.instance.TriggerDialogue("GoldenCarrotMissing");
                if (!KeyManager.instance.hasDiamondKey)
                    DialogueManager.instance.TriggerDialogue("DiamondCarrotMissing");
                break;
            case 2:
                DialogueManager.instance.TriggerDialogue("TwoCarrots");
                if (!KeyManager.instance.hasNormalKey)
                    DialogueManager.instance.TriggerDialogue("NormalCarrotMissing");
                if (!KeyManager.instance.hasGoldenKey)
                    DialogueManager.instance.TriggerDialogue("GoldenCarrotMissing");
                if (!KeyManager.instance.hasDiamondKey)
                    DialogueManager.instance.TriggerDialogue("DiamondCarrotMissing");
                break;
            case 3:
                DialogueManager.instance.TriggerDialogue("AllCarrots", () => {GameManager.instance.LevelComplete();});
                break;
        }
    }
}