using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Lever : MonoBehaviour
{
    // Это ссылка на дверь, которую открывает данный ключ
    // Ставится в инспекторе (в префабе уже стоит)
    [SerializeField] private GameObject Door;

    // Дополнительные комменттарии:
    // Ключ на слое 'Collectable'
    // Собирается автоматически
    // После сбора деактивируется (удаления не происходит)

    // Если нужно сделать по нажатию кнопки, то это не проблема:
    // можно сделать схожим с дверьми образом: переменная IsPlayerHere и Update для сбора кнопкой

    void Start()
    {
        if (Door == null)
        {
            Debug.Log("Key: I don't have door!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Door.GetComponent<ExitDoor>().KeyCollected();
            this.gameObject.SetActive(false);
        }
    }
}
