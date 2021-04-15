using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;

namespace var7KR
{
    abstract class Trucking
    {
        public string ClientName;
        public string ClientSurname;
        public string ClientPatronymic;
        public string TarifName;
        public double PriceForKm;
        public double DiscontSize;
        public string DiscontType;
        public double OrderedDistance;

    }
    class Tarif : Trucking
    {
        public Tarif(string a)
        {
            string[] line = a.Split('|');
            this.TarifName = line[0];
            this.PriceForKm = double.Parse(line[1]);
            this.DiscontType = line[2];
            this.DiscontSize = double.Parse(line[3]);
        }
        public Tarif(string TarifName, double PriceForKm, string DiscontType, double DiscontSize)
        {
            this.TarifName = TarifName;
            this.PriceForKm = PriceForKm;
            this.DiscontType = DiscontType;
            this.DiscontSize = DiscontSize;
        }
        public override string ToString()
        {
            return (this.TarifName + "|" + this.PriceForKm + "|" + this.DiscontType + "|" + this.DiscontSize);
        }

    }
    class Client : Trucking
    {
        public Client(string a)
        {
            string[] line = a.Split('|');
            this.ClientSurname = line[0];
            this.ClientName = line[1];
            this.ClientPatronymic = line[2];
        }
        public override string ToString()
        {
            return (this.ClientSurname + "|" + this.ClientName + "|" + this.ClientPatronymic);
        }
    }
    class Order : Trucking
    {
        public Order(string a)
        {
            string[] line = a.Split('|');
            this.ClientSurname = line[0];
            this.ClientName = line[1];
            this.ClientPatronymic = line[2];
            this.TarifName = line[3];
            this.PriceForKm = double.Parse(line[4]);
            this.DiscontSize = double.Parse(line[6]);
            this.DiscontType = line[5];
            this.OrderedDistance = double.Parse(line[7]);
        }
        public double OrderPrice // свойство, возвращающее стоимость заказа(только для чтения)
        {
            get
            {
                double summ;
                if (this.DiscontType == "%")
                {
                    summ = this.PriceForKm * this.OrderedDistance;
                    summ = summ * (1 - this.DiscontSize / 100);
                }
                else
                {
                    summ = this.PriceForKm * this.OrderedDistance - this.DiscontSize;
                }
                return summ;
            }
        }
        
    }
    class OrderSum:IComparable
    {
        public string ClientName;
        public double OrderCost;
        public OrderSum(string a, double b)
        {
            this.ClientName = a;
            this.OrderCost = b;
        }
        public int CompareTo(Object obj)
        {
            OrderSum x = (OrderSum)obj;
            return this.OrderCost.CompareTo(x.OrderCost);
        }
        public static OrderSum operator +(OrderSum x, double y)
        {
            OrderSum a = new OrderSum(x.ClientName, x.OrderCost + y);
            return a;
        }
    }
    class Program
    {
        static string[] ShowTarifTable()
        {
            StreamReader sr = new StreamReader(@".\tarif.txt");
            Console.WriteLine("Текущие тарифы.");
            string[] tmp = File.ReadAllLines(@".\tarif.txt");
            sr.Close();
            Console.WriteLine("|{0,25}|{1,25}|{2,10}|{3,15}|", "Название тарифа", "Цена за километр", "Тип скидки", "Размер скидки");
            foreach (string a in tmp)
            {
                string[] line = a.Split('|');
                Console.WriteLine("|{0,25}|{1,25}|{2,10}|{3,15}|", line[0], line[1], line[2], line[3]);
            }
            return tmp;
        }
        static void EnterTarification()
        {
            for (; ; )
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                ShowTarifTable();
                Console.WriteLine("Выберите действие с тарифами");
                Console.WriteLine("1.Добавить новый тариф.");
                Console.WriteLine("2.Удалить существующий тариф.");
                Console.WriteLine("0.Отмена(возврат в главное меню)");
                int choice = int.Parse(Console.ReadLine());
                switch (choice)
                {
                    case 1:
                        Console.Clear();
                        NewTarifEnter();
                        break;
                    case 2:
                        Console.Clear();
                        DeleteTarif();
                        break;
                    case 0:
                        Console.Clear();
                        return;
                }
                Console.ResetColor();
            }

        }
        static ArrayList ShowTarif()
        {
            string[] line = File.ReadAllLines(@".\tarif.txt");
            int counter = 0;
            ArrayList TarifList = new ArrayList();
            foreach (string a in line)
            {
                Tarif NewTarif = new Tarif(a);
                TarifList.Add(NewTarif);
            }
            foreach (Tarif tar in TarifList)
            {

                counter++;
                Console.WriteLine("{0}.Тариф '{1}' - {2} $ за км скидка {3}{4}", counter, tar.TarifName, tar.PriceForKm, tar.DiscontSize, tar.DiscontType);
            }
            return TarifList;
        }
        static void DeleteTarif()
        {
            Console.WriteLine("Выберите номер тарифа для удаления");
            ArrayList TarifList = ShowTarif();
            int choice = int.Parse(Console.ReadLine());
            TarifList.RemoveAt(choice - 1);
            FileStream fs = new FileStream(@".\tarif.txt", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
            foreach (Tarif tar in TarifList)
            {
                sw.WriteLine(tar.ToString());
            }
            sw.Close();
            fs.Close();
        }
        static void NewTarifEnter()//метод для ввода нового тарифа
        {
            Console.WriteLine("Введите название тарифа");
            string TarifName = null;
            ArrayList TarifList = new ArrayList();
            string[] tmp = ShowTarifTable();
            foreach (string a in tmp)
            {
                Tarif NewTarif = new Tarif(a);
                TarifList.Add(NewTarif);
            }

            try
            {
                TarifName = Console.ReadLine();
                TarifName = TarifName.Trim();
                if (TarifName.Length < 1) throw new Exception("Недопустим ввод пустой строки вместо имени тарифа!!!");//проверка ввода названия тарифа
                foreach (Tarif tar in TarifList) //исключение, если введена пустая строка
                {
                    if (tar.TarifName == TarifName)
                    {
                        throw new Exception("Тариф с таким именем уже существует!");//исключение, если такой тариф уже есть
                    }
                }
            }
            catch (Exception err)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.BackgroundColor = ConsoleColor.White;
                Console.WriteLine(err.Message + "  Попробуйте ещё раз...");
                Console.ResetColor();
                return;
            }
            Console.WriteLine("Введите цену за 1 км: ");
            double PriceForKm = double.Parse(Console.ReadLine());
            Console.WriteLine("Скидка в процентах?(нажмите 'ESC', если нет. Для подтверждения нажмите любую клавишу.)");
            ConsoleKeyInfo key = new ConsoleKeyInfo();
            key = Console.ReadKey();
            double DiscontSize = 0;
            string DiscontType;
            if ((key.KeyChar) == 27)
            {
                DiscontType = "$";
                Console.WriteLine("Введите скидку в $");
                DiscontSize = double.Parse(Console.ReadLine());
            }
            else
            {
                DiscontType = "%";
                Console.WriteLine("Введите процент скидки");
                DiscontSize = double.Parse(Console.ReadLine());
                if (DiscontSize >= 50)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.WriteLine("ПРИ ТАКОЙ СКИДКЕ НЕЛЬЗЯ РАБОТАТЬ!!! ЭТО ОДНИ УБЫТКИ!!! \n Вернитесь в предыдущее меню и подумайте хорошенько...");
                    Console.ResetColor();
                    return;
                }
            }
            string data = TarifName + "|" + PriceForKm + "|" + DiscontType + "|" + DiscontSize;
            FileStream fs;
            if (File.Exists(@".\tarif.txt"))
            {
                fs = new FileStream(@".\tarif.txt", FileMode.Append);
            }
            else
            {
                fs = new FileStream(@".\tarif.txt", FileMode.Create);
            }
            StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
            sw.WriteLine(data);
            sw.Close();
            fs.Close();
            Console.WriteLine("Тариф успешно добавлен!");
            //Tarif AddingTarif = new Tarif();
            return;
        }
        static void ClientReg()
        {
            for (; ; )
            {
                Console.Clear();
                Console.WriteLine("Регистрация клиента.");
                Console.Write("Введите фамилию:");
                string ClientSurname = Console.ReadLine();
                Console.Write("Введите имя: ");
                string ClientName = Console.ReadLine();
                Console.Write("Введите отчество: ");
                string ClientPatronymic = Console.ReadLine();
                Console.WriteLine("Выберите тариф");
                ArrayList TarifList = ShowTarif();
                int choice = int.Parse(Console.ReadLine());
                TarifList.ToArray();
                Tarif tar = new Tarif(TarifList[choice - 1].ToString());
                Console.WriteLine("Введите расстояние для перевозки(км)");
                double distance = double.Parse(Console.ReadLine());
                try
                {
                    if (tar.DiscontType == "$" && (distance * tar.PriceForKm - tar.DiscontSize) < 0)//исключение с отрицательной стоимостью заказа
                    {
                        throw new Exception("У данного заказа отрицательная стоимость! Оформление отклонено!");

                    }
                }
                catch (Exception err)
                {
                    Console.WriteLine(err.Message);
                    return;
                }
                string CurrentOrder = ClientSurname + "|" + ClientName + "|" + ClientPatronymic + "|" + tar.ToString() + "|" + distance;
                FileStream fs;
                if (File.Exists(@".\database.txt"))
                {
                    fs = new FileStream(@".\database.txt", FileMode.Append);
                }
                else
                {
                    fs = new FileStream(@".\database.txt", FileMode.Create);
                }
                StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
                sw.WriteLine(CurrentOrder);
                sw.Close();
                fs.Close();
                Console.WriteLine("Ваш заказ успешно добавлен!");
                break;
            }
        }
        static double AllOrderPrice()
        {
            double summ = 0;
            string[] tmp = File.ReadAllLines(@".\database.txt");
            foreach (string a in tmp)
            {
                Order NewOrder = new Order(a);
                summ += NewOrder.OrderPrice;
            }
            return summ;
        }

        static void FindMaxOrder()
        {
            FileStream fs = new FileStream(@".\ordersumm.txt", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
            string[] tmp = File.ReadAllLines(@".\database.txt");
            string temp = null;
            ArrayList OrderList = new ArrayList();
            Queue<OrderSum> OrderSearch = new Queue<OrderSum>();
            foreach (string a in tmp)
            {
                Order NewOrder = new Order(a);
                OrderList.Add(NewOrder);
            }

            double summ = 0;
            foreach (Order x in OrderList)//выделяем заказы клиента среди всех для определения общей суммы заказов данного клиента
            {
                if (x.OrderedDistance != 0)
                {
                    temp = x.ClientSurname + " " + x.ClientName + " " + x.ClientPatronymic;
                    summ += x.OrderPrice;
                    x.OrderedDistance = 0;
                    x.DiscontSize = 0;
                    foreach (Order y in OrderList)
                    {
                        if (temp == y.ClientSurname + " " + y.ClientName + " " + y.ClientPatronymic)
                        {
                            summ += y.OrderPrice;
                            y.OrderedDistance = 0;  //обнуление заказанной перевозки-она не будет теперь учтена вторично
                            y.PriceForKm = 0;
                            y.DiscontSize = 0;
                        }
                    }
                    OrderSum a = new OrderSum(temp, summ);
                    OrderSearch.Enqueue(a);
                    sw.WriteLine(temp + "|" + summ);//запись в файл суммы заказов каждого клиента
                    summ = 0;
                }
            }
            sw.Close();
            fs.Close();
            OrderSum[] ord = OrderSearch.ToArray();
            Array.Sort(ord);
            int n = ord.Length - 1;//номер максимального элемента
            Console.WriteLine("Максимальный заказ у {0} он равен {1}", ord[n].ClientName, ord[n].OrderCost);
        }
        static void ShowAllOrders()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Вывести заказы клиентов");
            Console.WriteLine("1.Все заказы ");
            Console.WriteLine("2.Суммарный заказ каждого клиента");
            Console.WriteLine("0.Возврат в главное меню");
            int choice = int.Parse(Console.ReadLine());
            Console.ResetColor();
            switch (choice)
            {
                case 1:
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("ЗАКАЗЫ КЛИЕНТОВ");
                    Console.WriteLine("{0,50}|{1,15}|{2,10}|{3,6}","ФИО","Тариф","Дистанция","Сумма");
                    string[] line = File.ReadAllLines(@".\database.txt");
                    foreach (string a in line)
                    {
                        Order x = new Order(a);
                        double summ = x.OrderPrice;
                        string[] tmp = a.Split('|');
                        Console.WriteLine("{0,50}|{1,15}|{2,10}|{3,6}", tmp[0]+" "+tmp[1]+" "+tmp[2],tmp[3], tmp[7], summ);
                    }
                    Console.ResetColor();
                    break;
                case 2:
                     Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("СУММА  ЗАКАЗОВ ПО КЛИЕНТАМ");
                    Console.WriteLine("{0,50}|{1,8}|","ФИО","Сумма");
                    string[] ord = File.ReadAllLines(@".\ordersumm.txt");
                    foreach (string a in ord)
                    {
                        string[] tmp = a.Split('|');
                        Console.WriteLine("{0,50}|{1,8}|", tmp[0], tmp[1]);
                    }
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.WriteLine("Можно выбрать только 1,2 или 0!");
                    Console.ResetColor();
                    ShowAllOrders();
                    break;
            }
            
        }
        static void Main()
        {
            for (; ; )
            {
                try
                {
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine("Main Menu");
                    Console.WriteLine("1.Управление тарифами");
                    Console.WriteLine("2.Регистрация клиента");
                    Console.WriteLine("3.Суммарная стоимость всех заказов");
                    Console.WriteLine("4.Поиск клиента с максимальным заказом");
                    Console.WriteLine("5.Показать все заказы");
                    Console.WriteLine("0.Выход");
                    int choice = int.Parse(Console.ReadLine());
                    Console.ResetColor();
                    switch (choice)
                    {
                        case 1:
                            EnterTarification();
                            break;
                        case 2:
                            ClientReg();
                            break;
                        case 3:
                            double summ = AllOrderPrice();
                            Console.WriteLine("Всего заказано перевозок на {0}$", summ);
                            break;
                        case 4:
                            FindMaxOrder();
                            break;
                        case 5:
                            ShowAllOrders();
                            break;
                        case 0:
                            Console.WriteLine("Завершение работы...");
                            Environment.Exit(0);
                            break;
                    }
                }
                catch (SystemException err)
                {
                    Console.WriteLine("ОШИБКА!-->"+err.Message);
                }
            }

        }
    }
}
