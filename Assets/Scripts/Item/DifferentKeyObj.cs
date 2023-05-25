using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DifferentKeyObj : InteractableBase, IInteractRequest   
{
    public int KeyIDV1;
    public int KeyIDV2;
    bool opened;
    public UnityEvent itemOpenV1;
    public UnityEvent itemOpenV2;

    public void InteractRequest(int ItemID)
    {

        if (opened)
        {
            showWord = openWord;
        }
        else
        {
            if (ItemID == KeyIDV1)//��
            {
                opened = true;
                showWord = openWord;
                itemOpenV1.Invoke();
            }else if (ItemID == KeyIDV2)
            {
                opened = true;
                showWord = openWord;
                itemOpenV2.Invoke();
            }
            else if (ItemID == 0)//����
            {
                showWord = emptyHandWord; //Ӧ�ÿ��Լ����Ż��� ��ҪԿ�׵�һ���࣬����Կ�׶��ַ�����һ�����࣬pickupһ��
            }
            else // ���˴�Ķ�������
            {
                showWord = cannotOpenWord;
            }

        }

    }
}
