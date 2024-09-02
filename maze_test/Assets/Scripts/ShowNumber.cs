using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShowNumber : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text number;
    [SerializeField] TMPro.TMP_Text cell;
    [SerializeField] TMPro.TMP_Text element;
    [SerializeField] int cellNumber = 0;
    [SerializeField] int cellOrder = 0;
    [SerializeField] int elementNum = 0;
    [SerializeField] GameObject UP;
    [SerializeField] GameObject Right;

    public void AllOnOff(bool v)
    {
        UP.SetActive(v);
        Right.SetActive(v);
        number.gameObject.SetActive(v);
        cell.gameObject.SetActive(v);
        element.gameObject.SetActive(v);
    }

    public void SetNumber(int num)
    {
        number.gameObject.SetActive(true);
        cellNumber = num;
        number.text = cellNumber.ToString();
    }

    public void SetOrder(int num)
    {
        cell.gameObject.SetActive(true);
        cellOrder = num;
        cell.text = cellOrder.ToString();
    }

    public void SetElement(int num)
    {
        element.gameObject.SetActive(true);
        elementNum = num;
        element.text = elementNum.ToString();
    }

    public void SetUP(bool v)
    {
        UP.SetActive(v);
    }

    public void SetRight(bool v)
    {
        Right.SetActive(v);
    }
}
