using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    // Скорость
    private float speed = 7.0f;
    private float speedMax = 15.0f;

    // Смерть
    public bool die = false;
    // Разрешено движение
    private bool movie = false;
    // Текущий блок
    private GameObject currentBlock = null;

    private int countBlockLeft = 0;

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(!die & movie)
        {
            Vector3 _curPosition = transform.position;
            _curPosition.z += speed;
            transform.position = Vector3.Lerp(transform.position, _curPosition, Time.deltaTime);

            RaycastHit _Hit;
            Vector3 _pointTwo = transform.position;
            _pointTwo.z += 0.5f;

            if(!Physics.Raycast(transform.position, Vector3.down, out _Hit, 10.0f) && !Physics.Raycast(_pointTwo, Vector3.down, out _Hit, 10.0f))
            {
                //Debug.Log("RayCast Die");
                die = true;
            }
            else
            {

                if(_Hit.transform.parent != null && _Hit.transform.parent.CompareTag("Block") && currentBlock != _Hit.transform.parent.gameObject)
                {
                    countBlockLeft++;
                    if(speed < speedMax)
                    {
                        speed = speed + ((int)Mathf.Log(countBlockLeft) / 4);
                        speed = (speed > speedMax) ? speedMax : speed;
                    }

                    currentBlock = _Hit.transform.parent.gameObject;
                    BlockRotator _CurrentBlockRotator = currentBlock.GetComponent<BlockRotator>() as BlockRotator;
                    if(_CurrentBlockRotator != null)
                    {
                        _CurrentBlockRotator.rotate = false;
                        _CurrentBlockRotator.SetLocalRotationAngle();
                    }
                }
            }
        }

    }

    void OnCollisionEnter(Collision col)
    {
        //Debug.Log("Collision Die");
        die = true;
    }

    // Устанавливаем может передвигаться или нет
    public void SetMovie(bool val)
    {
        movie = (val == true) ? true : false;
    }

}
