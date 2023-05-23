using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//���������õ� ����Կ����
public class DuoKeyObject : InteractableBase, IInteractRequest
{
    public int KeyIDV1;
    public int KeyIDV2;
    bool opened;
    bool key1Opened;
    public UnityEvent itemOpenV1;
    public UnityEvent itemOpenV2;
    public string key1RequireWord;
    public string key2RequireWord;
    public void InteractRequest(int ItemID)
    {

        if (opened)
        {
            showWord = openWord;
        }
        else
        {
            if (key1Opened)
            {
                if (ItemID == KeyIDV1)//��
                {
                    opened = true;
                    showWord = openWord;
                    itemOpenV1.Invoke();
                }
                else if (ItemID == 0)//����
                {
                    showWord = key1RequireWord; //Ӧ�ÿ��Լ����Ż��� ��ҪԿ�׵�һ���࣬����Կ�׶��ַ�����һ�����࣬pickupһ��
                }
                else // ���˴�Ķ�������
                {
                    showWord = cannotOpenWord;
                }
            }
            else
            {
                if (ItemID == KeyIDV2)//��
                {
                    opened = true;
                    showWord = openWord;
                    itemOpenV2.Invoke();
                }
                else if (ItemID == 0)//����
                {
                    showWord = key2RequireWord; ; //Ӧ�ÿ��Լ����Ż��� ��ҪԿ�׵�һ���࣬����Կ�׶��ַ�����һ�����࣬pickupһ��
                }
                else // ���˴�Ķ�������
                {
                    showWord = cannotOpenWord;
                }
            }


        }

    }
}
