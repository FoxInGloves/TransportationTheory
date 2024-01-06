using System.Threading;
using TransportTask.Models.TransportTaskSolver.Abstractions;

namespace TransportTask.Models.TransportTaskSolver.Implementations.Decorators;

public sealed class TransportTaskSolverWithOptimizations : BaseTransportTaskSolverDecorator
{
    public TransportTaskSolverWithOptimizations(ITransportTaskSolver transportTaskSolver) : base(transportTaskSolver)
    {
    }

    public override float[,] Calculate(in TransportTask.TransportTask matrix)
    {
        var previousResult = TransportTaskSolver.Calculate(matrix);

        return Optimize(matrix.TariffMatrix, previousResult);
    }

    //TODO the code was taken from github and needs to be rewritten, because it’s trash
    public float[,] Optimize(float[,] costs, float[,] basicPlan)
    {
        float[,] C; //адресмассива(двумерного) стоимости перевозки
        float[] u; //адрес массива потенциалов поставщиков
        float[] v; //адрес массива потенциалов потребителей
        float[,] D; //адрес массива(двумерного) оценок свободных ячеек таблицы
        bool stop; //признак достижения оптимального плана
        bool[,] T; //массив будет хранить коодинаты ячеек, в которые уже вписывались
        var ok = true; //нулевые поставки при попытках устранить вырожденность плана


        var row = costs.GetLength(0); //кол-во строк таблицы стоимости
        var column = costs.GetLength(1); //кол-во столбцов таблицы стоимости

        //----------------------Инициализация динамических массивов:


        //Двумерный массив для Стоимости:
        C = costs;


        var L = 0;
        for (var i = 0; i < row; i++)
        for (var j = 0; j < column; j++)
            if (basicPlan[i, j] >= 0)
                L++; //подсчёт заполненных ячеек
        var d = column + row - 1 - L; //если d>0,то задача - вырожденная,придётся добавлять d нулевых поставок

        var d1 = d;

        //--------------------------Метод потенциалов

        T = new bool[row, column];

        for (var i = 0; i < row; i++)
        for (var j = 0; j < column; j++)
            T[i, j] = false;

        do
        {
            //цикл поиска оптимального решения
            stop = true; //признак оптимальности плана(после проверки может стать false)
            u = new float[row]; //выделение массивов под значения потециалов
            v = new float[column];
            var ub = new bool[row]; //вспомогательные массвы
            var vb = new bool[column];
            for (var i = 0; i < row; i++)
                ub[i] = false;
            for (var i = 0; i < column; i++)
                vb[i] = false;


            //цикл расчёта потенциалов (несколько избыточен):
            u[0] = 0; //значение одного потенциала выбираем произвольно
            ub[0] = true;
            var count = 1;
            var tmp = 0;
            do
            {
                for (var i = 0; i < row; i++)
                    if (ub[i])
                        for (var j = 0; j < column; j++)
                            if (basicPlan[i, j] >= 0)
                                if (vb[j] == false)
                                {
                                    v[j] = C[i, j] - u[i];
                                    vb[j] = true;
                                    count++;
                                }

                for (var j = 0; j < column; j++)
                    if (vb[j])
                        for (var i = 0; i < row; i++)
                            if (basicPlan[i, j] >= 0)
                                if (ub[i] == false)
                                {
                                    u[i] = C[i, j] - v[j];
                                    ub[i] = true;
                                    count++;
                                }

                tmp++;
            } while (count < column + row - d * 2 && tmp < column * row);


            //цикл добавления нулевых поставок(в случае вырожденности):
            var t = false;

            if (d > 0 || ok == false) t = true; //цикл начинается, если d>0
            while (t) //цикл продолжается до тех пор, пока все потенциалы не будут найдены
            {
                for (var i = 0; i < row; i++) //просматриваем потенциалы поставщиков
                    if (ub[i] == false) //если среди них не заполненный потенциал
                        for (var j = 0; j < column; j++)
                            if (vb[j])
                            {
                                if (d > 0)
                                    if (T[i, j] == false) //если раньше не пытались использовать
                                    {
                                        basicPlan[i, j] = 0; //добавляем нулевую поставку
                                        d--; //уменьшаем кол-во требуемых добавлений нулевых поставок
                                        T[i, j] = true; //отмечаем, что эту ячейку уже использовали
                                    }

                                if (basicPlan[i, j] >= 0)
                                {
                                    u[i] = C[i, j] - v[j]; //дозаполняем потенциалы
                                    ub[i] = true;
                                }
                            }

                for (var j = 0; j < column; j++) //просматриваем потенциалы потребителей
                    if (vb[j] == false) //если среди них не заполненный потенциал
                        for (var i = 0; i < row; i++)
                            if (ub[i])
                            {
                                if (d > 0)
                                    if (T[i, j] == false) //если раньше не пытались использовать
                                    {
                                        basicPlan[i, j] = 0; //добавляем нулевую поставку
                                        d--; //уменьшаем кол-во требуемых добавлений нулевых поставок
                                        T[i, j] = true; //отмечаем, что эту ячейку уже использовали
                                    }

                                if (basicPlan[i, j] >= 0)
                                {
                                    v[j] = C[i, j] - u[i]; //дозаполняем потенциалы
                                    vb[j] = true;
                                }
                            }

                t = false; //проверяем, все ли потенциалы найдены
                for (var i = 0; i < row; i++)
                    if (ub[i] == false)
                        t = true;
                for (var j = 0; j < column; j++)
                    if (vb[j] == false)
                        t = true;
            }

            D = new float[row, column]; //выделяем память под массив оценок свободных ячеек

    
            for (var i = 0; i < row; i++)
            for (var j = 0; j < column; j++)
            {
                if (basicPlan[i, j] >= 0) //если ячейка не свободна
                    D[i, j] = 88888; //Заполняем любыми положительными числами
                else //если ячейка свободна
                    D[i, j] = C[i, j] - u[i] - v[j]; //находим оценку

                if (D[i, j] < 0) stop = false; //признак того, что план не оптимальный
            }

            if (stop == false) //если план не оптимальный
            {
                var Y = new float[row, column]; //массив для хранения цикла перераспределения поставок

                float find1, find2; //величина перераспределения поставки для цикла
                float best1 = 0; //наилучшая оценка улучшения среди всех допустимых перераспределений
                float best2 = 0;
                var ib1 = -1;
                var jb1 = -1;
                var ib2 = -1;
                var jb2 = -1;
                //Ищем наилучший цикл перераспределения поставок:
                for (var i = 0; i < row; i++)
                for (var j = 0; j < column; j++)
                    if (D[i, j] < 0) //Идём по ВСЕМ ячейкам с отрицательной оценкой
                    {
                        //и ищем допустимые циклы перераспределения ДЛЯ КАЖДОЙ такой ячейки
                        //Обнуляем матрицу Y:
                        for (var i1 = 0; i1 < row; i1++)
                        for (var j1 = 0; j1 < column; j1++)
                            Y[i1, j1] = 0;
                        //Ищем цикл для ячейки с оценкой D[i,j]:
                        find1 = FindHorizontal(i, j, i, j, row, column, basicPlan, Y, 0, -1); //Начинаем идти по горизонтали

                        //Обнуляем матрицу Y:
                        for (var i1 = 0; i1 < row; i1++)
                        for (var j1 = 0; j1 < column; j1++)
                            Y[i, j] = 0;

                        find2 = FindVertical(i, j, i, j, row, column, basicPlan, Y, 0, -1); //Начинаем по вертикали

                        if (find1 > 0)
                            if (best1 > D[i, j] * find1)
                            {
                                best1 = D[i, j] * find1; //наилучшая оценка
                                ib1 = i; //запомминаем координаты ячейки
                                jb1 = j; //цикл из которой даёт наибольшее улучшение
                            }

                        if (find2 > 0)
                            if (best2 > D[i, j] * find2)
                            {
                                best2 = D[i, j] * find2; //наилучшая оценка
                                ib2 = i; //запомминаем координаты ячейки
                                jb2 = j; //цикл из которой даёт наибольшее улучшение
                            }
                    }

                if (best1 == 0 && best2 == 0)
                {
                    ok = false;
                    d = d1; //откат назад
                    for (var i = 0; i < row; i++)
                    for (var j = 0; j < column; j++)
                        if (basicPlan[i, j] == 0)
                            basicPlan[i, j] = -1;
                    continue;
                }

                //Обнуляем матрицу Y:
                for (var i = 0; i < row; i++)
                for (var j = 0; j < column; j++)
                    Y[i, j] = 0;
                //возвращаемся к вычислению цикла с наилучшим перераспределением:
                int ib, jb;
                if (best1 < best2)
                {
                    FindHorizontal(ib1, jb1, ib1, jb1, row, column, basicPlan, Y, 0, -1);
                    ib = ib1;
                    jb = jb1;
                }
                else
                {
                    FindVertical(ib2, jb2, ib2, jb2, row, column, basicPlan, Y, 0, -1);
                    ib = ib2;
                    jb = jb2;
                }

                for (var i = 0; i < row; i++)
                for (var j = 0; j < column; j++)
                {
                    if (basicPlan[i, j] == 0 && Y[i, j] < 0)
                    {
                        stop = true;
                    }

                    basicPlan[i, j] += Y[i, j]; //перераспределяем поставки
                    if (i == ib && j == jb)
                        basicPlan[i, j] = basicPlan[i, j] + 1; //добавляем 1 (т.к. до этого было -1 )
                    if (Y[i, j] <= 0 && basicPlan[i, j] == 0)
                        basicPlan[i, j] = -1; //если ячейка обнулилась, то выбрасываем её из рассмотрения
                }

                ok = true;
                for (var i = 0; i < row; i++)
                for (var j = 0; j < column; j++)
                    T[i, j] = false;

                //проверка на вырожденность: (?)
                L = 0;
                for (var i = 0; i < row; i++)
                for (var j = 0; j < column; j++)
                    if (basicPlan[i, j] >= 0)
                        L++; //подсчёт заполненных ячеек
                d = column + row - 1 - L; //если d>0,то задача - вырожденная
                d1 = d;
                if (d > 0) ok = false;
            }
        } while (stop == false);

        return basicPlan;
    }

    //---------------------------------------------------------------------------
    //Функуция поиска ячеек,подлежащих циклу перераспределения (вдоль строки)
    private float FindHorizontal(int i_next, int j_next, int im, int jm, int n, int m, float[,] X, float[,] Y, int odd,
        float Xmin)
    {
        float rez = -1;
        for (var j = 0; j < m; j++) //идём вдоль строки, на которой стоим
            //ищем заполненную ячейку(кроме той,где стоим) или начальная ячейка(но уже в конце цикла:odd!=0 )
            if ((X[i_next, j] >= 0 && j != j_next) || (j == jm && i_next == im && odd != 0))
            {
                odd++; //номер ячейки в цикле перерасчёта(начало с нуля)
                float Xmin_old = -1;
                if (odd % 2 == 1) //если ячейка нечётная в цикле ( начальная- нулевая )
                {
                    Xmin_old = Xmin; //Запоминаем значение минимальной поставки в цикле (на случай отката назад)
                    if (Xmin < 0) Xmin = X[i_next, j]; //если это первая встреченная ненулевая ячейка
                    else if (X[i_next, j] < Xmin && X[i_next, j] >= 0) Xmin = X[i_next, j];
                }

                if (j == jm && i_next == im &&
                    odd % 2 == 0) //если замкнулся круг и цикл имеет чётное число ячеек
                {
                    Y[im, jm] = Xmin; //Значение минимальной поставки, на величину которой будем изменять
                    return Xmin;
                }
                //если круг еще не замкнулся - переходим к поиску по вертикали:

                rez = FindVertical(i_next, j, im, jm, n, m, X, Y, odd, Xmin); //рекурсивный вызов

                if (rez >= 0) //как бы обратный ход рекурсии(в случае если круг замкнулся)
                {
                    //для каждой ячейки цикла заполняем матрицу перерасчёта поставок:
                    if (odd % 2 == 0) Y[i_next, j] = Y[im, jm]; //в чётных узлах прибавляем
                    else Y[i_next, j] = -Y[im, jm]; //в нечётных узлах вычитаем
                    break;
                }

                //откат назад в случае неудачи(круг не замкнулся):
                odd--;
                if (Xmin_old >= 0) //если мы изменяли Xmin на этой итерации
                    Xmin = Xmin_old;
            }

        return rez; //если круг замкнулся (вернулись в исходную за чётное число шагов) -
        // возвращает найденное минимальное значение поставки в нечётных ячейках цикла,
        // если круг не замкнулся, то возвращает -1.
    }

    //-----------------------------------------------------------------------------
    //Функуция поиска ячеек,подлежащих циклу перераспределения (вдоль столбца)
    private float FindVertical(int i_next, int j_next, int im, int jm, int n, int m, float[,] X, float[,] Y, int odd,
        float Xmin)
    {
        float rez = -1;
        int i;
        for (i = 0; i < n; i++) //идём вдоль столбца, на котором стоим
            //ищем заполненную ячейку(кроме той,где стоим) или начальная ячейка(но уже в конце цикла:odd!=0 )
            if ((X[i, j_next] >= 0 && i != i_next) || (j_next == jm && i == im && odd != 0))
            {
                odd++; //номер ячейки в цикле перерасчёта(начало с нуля)
                float Xmin_old = -1;
                if (odd % 2 == 1) //если ячейка нечётная в цикле ( начальная- нулевая )
                {
                    Xmin_old = Xmin; //Запоминаем значение минимальной поставки в цикле (на случай отката назад)
                    if (Xmin < 0) Xmin = X[i, j_next]; //если это первая встреченная ненулевая ячейка
                    else if (X[i, j_next] < Xmin && X[i, j_next] >= 0)
                        Xmin = X[i, j_next];
                }

                if (i == im && j_next == jm &&
                    odd % 2 == 0) //если замкнулся круг и цикл имеет чётное число ячеек
                {
                    Y[im, jm] = Xmin; //Значение минимальной поставки, на величину которой будем изменять
                    return Xmin;
                }
                //если круг еще не замкнулся - переходим к поиску по горизонтали:

                rez = FindHorizontal(i, j_next, im, jm, n, m, X, Y, odd, Xmin); //- рекурсивный вызов

                if (rez >= 0) //как бы обратный ход (в случае если круг замкнулся)
                {
                    //для каждой ячейки цикла заполняем матрицу перерасчёта поставок:
                    if (odd % 2 == 0) Y[i, j_next] = Y[im, jm]; //эти прибавляются
                    else Y[i, j_next] = -Y[im, jm]; //эти вычитаются
                    break;
                }

                //откат назад в случае неудачи (круг не замкнулся):
                odd--;
                if (Xmin_old >= 0) //если мы изменяли Xmin на этой итерации
                    Xmin = Xmin_old;
            }

        return rez;
    }
}