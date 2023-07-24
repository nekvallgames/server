namespace Plugin.Tools
{
    public struct OperationCode
    {
        /// <summary>
        /// Комната набрала всех необходимых игроков.
        /// Перед началом игры, сервер требует у игрока, что бы он выбрал юнитов, которыми будет играть
        /// </summary>
        public static byte selectUnitsForGame = 0;

        /// <summary>
        /// Клиент выбрал юнитов, которыми будет играть. И прислал операцию с текущим ключем
        /// </summary>
        public static byte choosedUnitsForGame = 1;

        /// <summary>
        /// Комната набрала всех необходимых игроков. Стартонуть игру.
        /// </summary>
        public static byte startGame = 2;

        /// <summary>
        /// Игрок сделал шаг. Имеется ввиду, что игрок переставил юнитов
        /// и сделал какие то действия (атака и т.д.)
        /// </summary>
        public static byte syncStep = 3;

        /// <summary>
        /// Сервер отправляет всем клиентам результат выполненого шага
        /// </summary>
        public static byte stepResult = 4;

        /// <summary>
        /// Локальный игрок сообщает серверу, что он изменил VIP какому то юниту
        /// </summary>
        public static byte changeVip = 5;
    }
}
