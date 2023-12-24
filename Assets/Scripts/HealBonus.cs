using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HealBonus : MonoBehaviour
{
    // Это ссылка на дверь, которую открывает данный ключ
    // Ставится в инспекторе (в префабе уже стоит)
    [SerializeField] private GameObject mainCharacter_;
    [SerializeField] private int value = 1;

    // Дополнительные комменттарии:
    // Ключ на слое 'Collectable'
    // Собирается автоматически
    // После сбора деактивируется (удаления не происходит)

    // Если нужно сделать по нажатию кнопки, то это не проблема:
    // можно сделать схожим с дверьми образом: переменная IsPlayerHere и Update для сбора кнопкой

    void Start()
    {
        if (mainCharacter_ == null)
        {
            Debug.Log("Ammo bonus: I don't have hero!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            mainCharacter_.GetComponent<Player>().HealHero(value);
            this.gameObject.SetActive(false);
        }
    }
}
