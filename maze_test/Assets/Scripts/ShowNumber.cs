using UnityEngine;

public class ShowNumber : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text cellNumberTxt;
    [SerializeField] TMPro.TMP_Text cell;
    [SerializeField] int cellNumber = 0;
    [SerializeField] int cellOrder = 0;
    [SerializeField] GameObject UP;
    [SerializeField] GameObject Right;

    public void AllOnOff(bool v)
    {
        UP.SetActive(v);
        Right.SetActive(v);
        cellNumberTxt.gameObject.SetActive(v);
        cell.gameObject.SetActive(v);
    }

    public void SetNumber(int num)
    {
        cellNumberTxt.gameObject.SetActive(true);
        cellNumber = num;
        cellNumberTxt.text = cellNumber.ToString();
    }

    public void SetOrder(int num)
    {
        cell.gameObject.SetActive(true);
        cellOrder = num;
        cell.text = cellOrder.ToString();
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
