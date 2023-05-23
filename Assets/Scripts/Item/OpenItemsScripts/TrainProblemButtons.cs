using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using UnityEngine.Events;

public class TrainProblemButtons : MonoBehaviour
{
    public string ChoiceHasMadeText;

    public ButtonObject[] buttons;

    public PathCreator[] pcreators;

    public Transform trainModelTrans;

    public UnityEvent[] choiceEvents;
    
    int currentSplineNumber;

    private void Start()
    {
        //trainModelTrans.position = pcreators[currentSplineNumber].path.GetPointAtDistance(pcreators[currentSplineNumber].path.length);
        //trainModelTrans.rotation = AdjustRot(pcreators[currentSplineNumber].path.GetRotationAtDistance(pcreators[currentSplineNumber].path.length));
    }
    public void DecisionMade(int ChoiceNumber)
    {
        currentSplineNumber = ChoiceNumber;
        StartCoroutine(MoveAlongSpline());
        
        foreach(ButtonObject button in buttons)
        {
            button.mouseOVerWord = ChoiceHasMadeText;
            
            button.canBeInteracted = false;
        }
        print("exed");
    }

    

    IEnumerator MoveAlongSpline()
    {
        int i = 0;
        float length = pcreators[currentSplineNumber].path.length;
        float lerp;
        for(; ; )
        {
            i += 1;
            lerp = Mathf.Lerp(0, 1, i / 50f);
            trainModelTrans.position = pcreators[currentSplineNumber].path.GetPointAtDistance(length-lerp * length);
           // trainModelTrans.rotation = AdjustRot( pcreators[currentSplineNumber].path.GetRotationAtDistance(length - lerp * length));
            trainModelTrans.rotation = AdjustRot(pcreators[currentSplineNumber].path.GetDirectionAtDistance(length - lerp * length));
            //trainModelTrans.rotation = pcreators[currentSplineNumber].path.GetRotationAtDistance(length - lerp * length);
            if (i >= 49)
            {
                StopAllCoroutines();
                choiceEvents[currentSplineNumber].Invoke();
            }
            Debug.DrawLine(trainModelTrans.position, trainModelTrans.position + pcreators[1].path.GetDirectionAtDistance(length - lerp * length) * 5);
            yield return new WaitForSeconds(0.02f);
        }
    }

    Quaternion AdjustRot(Vector3 direction)
    {

        return Quaternion.FromToRotation(Vector3.right, direction);
        
    }
}
