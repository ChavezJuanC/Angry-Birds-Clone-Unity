using UnityEngine;

public class SlingShotArea : MonoBehaviour
{

    [SerializeField] private LayerMask _slingShotAreaMask;

    public bool IsWithinSlingShotArea()
    {
        Vector2 worldPositon = Camera.main.ScreenToWorldPoint(InputManager.MousePositon);


        if (Physics2D.OverlapPoint(worldPositon, _slingShotAreaMask))

        {

            return true;
        }
        else
        {
            return false;
        }
    }
}
