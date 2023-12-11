using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneSidedDoor : MonoBehaviour
{
    // Это можно считать копией Door.cs
    // С поправкой на то, что нет метода KeyCollected (и соответственно, ключа)
    // и на то, что сохраняется Transform игрока (одного!) для проверки по оси X (левее/правее)
    // Проверка проводится при открывании двери
    // При желании можно переделать так, чтобы копипасты не было, но пока можно и так))

    enum StateOfDoor { Wait = 0, Openning = 1, Closing = 2 };

    private StateOfDoor currentState = StateOfDoor.Wait;
    private float progress = 0.0f; // Это уровень открытости двери, по сути

    private Vector3 startPosition; // Позиция, когда дверь закрыта
    private Vector3 endPosition;   // Позиция, когда дверь открыта
    Transform transform_;          // Просто transform двери (именно той части, на которой спрайт висит)

    Transform playerTransform_;

    // Показывает: нужно ли нам реагировать на события с клавиатуры или игрок пока слишком далеко?
    bool isHerePlayer = false;

    // Дополнительные комментарии про двери:
    // У той части двери, на которой висит тригер стоит тэг 'Door'. Без этого тэга дверь не будет открываться.
    // Сами двери находятся на layout 'Door'
    // Закрывание двери происходит автоматически после покидания зоны взаимодействия игрока с дверью

    // ! Опирается в onTriggerEnter/Exit на то, что взаимодействовать с дверями могут только игроки! (матрица взаимодействия слоёв)

    // Если игроков будет два, то нужно будет немного модифицировать код: чтобы дверь не реагировала на игрока,
    // с которым она взаимодействовать не может (в Update настроить кнопки для каждого игрока)
    // и изменить onTriggerExit2D и onTriggerEnter2D

    private void Start()
    {
        transform_ = gameObject.gameObject.transform.GetChild(0);
        startPosition = transform_.position;
        endPosition = transform_.position + transform_.up;
    }

    private void Open()
    {
        // Debug.Log("Open");
        if (playerTransform_.position.x <= startPosition.x)
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
            if (progress >= 1.0f)
            {
                transform_.position = endPosition;
                currentState = StateOfDoor.Wait;
            }
            else
            {
                transform_.position = Vector3.Lerp(startPosition, endPosition, progress);
            }
        }
        else if (currentState == StateOfDoor.Closing)
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
        isHerePlayer = true;
        playerTransform_ = collision.transform;
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
