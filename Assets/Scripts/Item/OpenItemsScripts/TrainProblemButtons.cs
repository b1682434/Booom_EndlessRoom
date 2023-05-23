using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainProblemButtons : MonoBehaviour
{
    public string ChoiceHasMadeText;

    public ButtonObject[] buttons;

    public void DecisionMade()
    {
        foreach(ButtonObject button in buttons)
        {
            button.showWord = ChoiceHasMadeText;
            button.canBeInteracted = false;
        }
    }
}
