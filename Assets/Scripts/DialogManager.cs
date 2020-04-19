using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


public enum DialogKey : int
{
    None,
    WhereAmI,
    CountedSteps,
    TeachConsumeSouls,
    MoveCrates,
    MoreSoulCapacity,
    MoreTime,
    NowCanDash,
    EvenMoreTime,
    CantKillMe,
    CanDestroyTheDemon,
    Ending
}

public class DialogContainer
{
    private Dictionary<DialogKey, string> dialogs = new Dictionary<DialogKey, string>();    

    public DialogContainer()
    {
        dialogs[DialogKey.WhereAmI] = "What is this place? Did I die?";
        dialogs[DialogKey.CountedSteps] = "Every step I take feels like it weakens me a bit.";        
        dialogs[DialogKey.TeachConsumeSouls] = "Demon: You are now linked to me. You shall collect souls to feed me. " +
                                               "In exchange I will prolong your life. Prove your worth to me and I will " +
                                               "help you with powers to speed your soul collection.";
        dialogs[DialogKey.MoveCrates] = "Demon: Now that you have demonstrated basic competency I will give you enough strength " +
                                       "to move those crates over there so you may continue your hunt for souls.";
        dialogs[DialogKey.MoreSoulCapacity] = "Demon: You have proved yourself a loyal servant. " +
                                               "You now have the ability to carry twice the souls back to me.";
        dialogs[DialogKey.MoreTime] = "Demon: Back so soon? How about I give you a bit more time to find souls.";
        dialogs[DialogKey.NowCanDash] = "Demon: Yes! Yes! More souls.  Now you may cross gaps to reach more souls!";
        dialogs[DialogKey.EvenMoreTime] = "Demon: Back so soon again? How about I give you even more time to find souls.";
        dialogs[DialogKey.CantKillMe] = "Demon: You think you can kill me?  Fool... Try again.";
        dialogs[DialogKey.CanDestroyTheDemon] = "Spirit: The demon is not the only one who gives powers. I have granted "+
                                                "you the ability to slay the demon if you touch him, but know that it comes at a horrible price.";
        dialogs[DialogKey.Ending] = "You have become the demon and have lost your powers. You are trapped on this island." +
                                    "Now you can only give powers away. It is now your turn to keep your self alive by having others do your bidding.";
    }

    public string TextForKey(DialogKey key)
    {
        string text = dialogs[key];
        Assert.IsNotNull(text);
        return text;
    }
}
