using UnityEngine;
using System.Collections;

public class PlayerData : MonoBehaviour
{
    // Собранные монетки
    private int _score = 0;
    // Баланс
    private int _balans = 300;

    public void SetScore(int val = 1)
    {
        if(val > 0)
            _score += val;
    }

    public int GetScore()
    {
        return _score;
    }

    // Баланс
    public int Balans
    {
        get
        {
            return _balans;
        }
        set
        {
            if(value >= 0)
                _balans = value;
        }
    }

}
