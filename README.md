# GAME_X

Я старалась оставлять максимально подробные комментарии в коде, если это понадобится.
Если какие-то файлы загружать было не нужно, то прошу прощения за это. (Я работала в 2d шаблоне, если нужно, переделаю на 3d шаблон.)
Теперь комментарии к тому, что было выгружено.

Про управление в сцене и ориентирование в ассетах проекта:
1. Передвижение игрока - либо WASD, либо стрелки. Разбивания на два набора кнопок для мультиплеера в коде нет. Игрок движется вверх-вниз, влево-вправо. Движение не связано с поворотом спрайта. (Если нужно, исправлю это.)
2. Переход в стелс-режим по кнопке **shift**. Режим бега - **ctrl**.
3. Поворот игрока происходит по нажатию кнопок **q**,**e** ("Rotate" в Input менеджере). (Делала для того, чтобы потом стрельбу по прямой прикрутить можно было. Если нужно, то можно запретить поворот во время движения, а во время движения поворачиваться по ходу движения.)
4. Двери открываются по нажатию кнопки "Open_door" (либо **x**, либо пробел). Закрываются автоматически при выходе из зоны коллайдера-триггера. (Т.е. если игрок отошёл не такое расстояние, что нажатие кнопки больше дверь не откроет, то дверь закроется сама.)
5. Ключи подбираются автоматически. После сбора становятся Disabled. Но явного удаления нет. Если нужен сбор по кнопке, то там править придётся не очень много. (Можно сделать аналогично скрипту на двери, например.)
6. Немного очевидные комментарии, но пусть будут:
    - Ярко рыжие блоки - стены, единственные статичные префабы.
    - Зелёные двери - обычные двери.
    - Синевато-голубоватый префаб - дверь с ключом. После сбора ключа открывается как обычная дверь.
    - Самая светлая, жёлто-зелёная дверь - это односторонняя. (Открыть можно только находясь левее по оси X, чем сама дверь. Но успеть вернуться, пока она не закрылась можно))

Про сам проект/ориентирование в нём:
1. Матрица Layer Collision Matrix была изменена. Осталось только взаимодействие слоя *Player* со слоями *Default*, *Collectable*, *Walls* и *Player*. Взаимодействие с дверьми я поправила, поэтому при необходимости меняйте)
2. Я старалась разносить логику и физику в скриптах, возможно, неоптимальным способом. Это буквально значит, что на *каждой* двери висит и Update, и FixedUpdate.
3. Что находится на целых значениях оси **Z** (чтобы каждый раз сравнивать не приходилось):
    - Z = 1. Тут только пол.
    - Z = 0. Все двери и ключи: префабы *Simple Door*, *Key And Door*, *One-sided Door*.
    - Z = -1. Здесь игрок и стены (*Player*, *Wall*)
4. Про использованные слои:
    - Player. Тут, очевидно, только игрок.
    - Collectable. Объекты, которые можно собрать. В данный момент это исключительно ключ в префабе *Key And Door*.
    - Walls. Это то, что ведёт себя схожим со стенами образом: сами стены и внутренние части префабов дверей, на которых висят спрайты.
    - Default. Всё остальное.
5. Тэги:
    - Сейчас используется тэг *Door* на объекте с триггером зоны двери (теоретическое расположение закрытой двери.)
    - Тэг *Player* для проверки, что взаимодействие было именно с тригерной частью префаба игрока.

Я допушил самого начального монстра. Теперь он обходит препятствия по следу игрока, а если видит игрока ЦЕЛИКОМ, то идет к нему навстречу. Проблем с движением монстра быть не должно, оно теперь финальное(если не толкать монстра игроком), если они возникают, просьба сообщить. Монстр ClassicZombie если не финален, то близок к этому состоянию. Добавлены еще 2 типа зомби, не сильно отличающихся от классического: быстрые и толстые. Добавлены ловушки:
1) Песок - пока мобов под него нет, но будут.
2) Колючая проволока - тонкая полоска на карте - при прохождении сквозь нее у героя отнимается жизнь. 
3) Болото / вода - замедляет, пока герой находится в нем. 
4) Скрипучие полы - отменяют стелс(рядом с зомби расположены)
5) Капканы - мгновенный стан героя на 3 секунды.
   Все ловушки отмечены каждая своим тегом и все вместе слоем Trap.

Поправила взаимодействие с дверями: теперь мобы двери открывать не могут.

Я добавила режимы *стелс* и *бег*. На световой радиус они пока не влияют, как и на мобов. Т.е. пока это влияет только на скорость персонажа.
Если понадобится, то для отладки значение выносливости можно увидеть в испекторе (SerializeField). Также текущий режим обозначается цветом:
- Белый: обычная ходьба
- Жёлтый: стелс-режим
- Зелёный: бег
