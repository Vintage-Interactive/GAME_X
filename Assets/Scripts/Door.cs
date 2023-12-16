using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    enum StateOfDoor { Wait = 0, Openning = 1, Closing = 2 };

    private StateOfDoor currentState = StateOfDoor.Wait;
    private float progress = 0.0f; // Это уровень открытости двери, по сути

    private Vector3 startPosition; // Позиция, когда дверь закрыта
    private Vector3 endPosition;   // Позиция, когда дверь открыта
    Transform transform_;          // Просто transform двери (именно той части, на которой спрайт висит)

    // Индикатор того, есть ли у нас ключ.
    // У простых дверей он равен true (значение видно в инспекторе)
    // У дверей с ключом изначально равен false (т.е. значение в инспекторе здесь не трогаем)
    [SerializeField] private bool isKeyCollected = false;

    // Показывает: нужно ли нам реагировать на события с клавиатуры или игрок пока слишком далеко?
    bool isHerePlayer = false;

    // Дополнительные комментарии про двери:
    // У той части двери, на которой висит тригер стоит тэг 'Door'. Без этого тэга дверь не будет открываться.
    // Сами двери находятся на layout 'Door'
    // Закрывание двери происходит автоматически после покидания зоны взаимодействия игрока с дверью

    // Взаимодействие двери возможно только с носителем тэга 'Player'.

    // Если игроков будет два, то нужно будет немного модифицировать код: чтобы дверь не реагировала на игрока,
    // с которым она взаимодействовать не может (в Update настроить кнопки для каждого игрока)
    // и изменить onTriggerExit2D и onTriggerEnter2D

    public void KeyCollected()
    {
        Debug.Log("My key collected!");
        isKeyCollected = true;
    }

    private void Start()
    {
        transform_ = gameObject.transform.GetChild(0);
        startPosition = transform_.position;
        endPosition = transform_.position + transform_.up;
    }

    private void Open()
    {
        // Debug.Log("Open");
        if (isKeyCollected)
        {
            currentState = StateOfDoor.Openning;
        }
    }

    private void Close()
    {
        // Debug.Log("Close");
        currentState = StateOfDoor.Closing;
    }

    private void Update()
    {
        if (isHerePlayer && Input.GetButtonDown("Open_door"))
        {
            Open();
        }
    }

    void FixedUpdate()
    {
        // Проверяем состояние двери
        if (currentState == StateOfDoor.Openning)
        {
            // В "открывающемся" состоянии нужно увеличить счётчик прогресса (уровень открытости двери) и сдвинуть дверь
            // Если дальше сдвигать не нужно, то идём в состояние "ждать'
            progress += Time.deltaTime;
            if (progress >= 1.0f) {
                transform_.position = endPosition;
                currentState = StateOfDoor.Wait;
            } else
            {
                transform_.position = Vector3.Lerp(startPosition, endPosition, progress);
            }
        } else if (currentState == StateOfDoor.Closing)
        {
            // В "закрывающемся" состоянии нужно уменьшить счётчик прогресса (уровень открытости двери) и сдвинуть дверь
            // Если дальше сдвигать не нужно, то идём в состояние "ждать'
            progress -= Time.deltaTime;
            if (progress <= 0.0f)
            {
                transform_.position = startPosition;
                currentState = StateOfDoor.Wait;
            }
            else
            {
                transform_.position = Vector3.Lerp(startPosition, endPosition, progress);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Взаимодействовать с дверями могут только игроки, поэтому можно не делать проверки слоя/тэга
        if (collision.CompareTag("Player"))
        {
            isHerePlayer = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Игрок пока только один, поэтому не проверяем на наличие второго игрока

        // Выход - только при срабатывании потери контакта в связке "триггер двери - зона взаимодействия игрока"
        if (collision.CompareTag("Player"))
        {
            isHerePlayer = false;
            Close();
        }
    }
}
