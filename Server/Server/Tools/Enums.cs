﻿namespace Plugin.Tools
{
    public struct Enums
    {
        /// <summary>
        /// Двуичная маска для ячеек
        /// </summary>
        public enum CellMask
        {
            backWall = 000001,       // задняя стена                              1
            floor = 000010,          // пол                                       2
            backFloorWall = 000100,  // задняя стенка, но по которой можно ходить 4
            cellWalkLock = 001000,   // селл запрещен для перемещения             8
            border = 010000,         // края игровой сетки                        16
        }

        /// <summary>
        /// Обозначение соседних ячеек игровой сетки
        /// </summary>
        public enum Direction
        {
            left,
            right,
            up,
            down
        }

        /// <summary>
        /// Часть тела
        /// </summary>
        public enum PartBody
        {
            empty = 0,    // пусто. Текущая часть тела не должна получать урон. Выстрел в текущею часть тела должен считатся промахом
            head = 1,    // голова
            body = 2,    // тело
            bottom = 3,    // ноги
        }

        public enum PVPMode
        {
            FightToFirstDead = 0,
            FightWithVip = 1,
            Duel = 2,
            Result = 3
        }

        /// <summary>
        /// Тип перемещения юнита. Именно сетка, рисунок,
        /// как будет перемещатся юнит по игровой сетке
        /// </summary>
        public enum WalkNavigation
        {
            body_width_1,              // перемещение юнита при перестрелке. Ширина юнита 1
            body_width_2,              // перемещение юнита при перестрелке. Ширина юнита 2
            body_width_3,              // перемещение юнита при перестрелке. Ширина юнита 3

            body_width_1_for_duel,    // перемещение юнита, когда он попадет в дуэль. Ширина юнита 1
            body_width_2_for_duel,    // перемещение юнита, когда он попадет в дуэль. Ширина юнита 2
            body_width_3_for_duel,    // перемещение юнита, когда он попадет в дуэль. Ширина юнита 3

            static_body_width_1,      // это путь в 1 клеточку, для позиционирования каких статических обьектов: например турель, преграда и т.д.
            static_body_width_2,      // это путь в 1 клеточку, для позиционирования каких статических обьектов: например турель, преграда и т.д.
            static_body_width_3,      // это путь в 1 клеточку, для позиционирования каких статических обьектов: например турель, преграда и т.д.
            static_body_width_4,      // это путь в 1 клеточку, для позиционирования каких статических обьектов: например турель, преграда и т.д.

            body_width_1_horizontal_width_13,        // перемещение юнита при перестрелке. Ширина юнита 1. Ширина по горизонтали 13
        }

        /// <summary>
        /// Пассивный навык юнита. То, чем он может помочь поманде во время перемещения юнитов
        /// </summary>
        public enum Additional
        {
            Healing,
            // Armor
            // Teleport ect
        }

        public enum SyncGroup
        {
            vip,
            action,
            additionalByPos,
            additionalByUnit,
            positionOnGrid
        }
    }
}
