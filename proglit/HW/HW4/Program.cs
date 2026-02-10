using System;
using System.Diagnostics.Contracts;
using System.Security.Cryptography.X509Certificates;

class Student
{
    private double age;
    private double clas;
    private string name;

    public double Age
    {
        get =>age;
        set{if (value < 5 || value > 100)
            {
                throw new ArgumentException("Возраст не можешет превышать сто и быть меньше 5");
            }
            else
            {
                age=value;
            }
        }
    }
    public double Clas
    {
        get =>clas;
        set{if (value < 1 || value > 12)
            {
                throw new ArgumentException("Всего может быть только 12 классов с 1 - 12");
            }
            else
            {
                clas=value;
            }
        }
    }
    public string Name
    {
        get => name;
        set
        {
            if (value.Length < 2)
            {
                throw new ArgumentException("Имя должно быть реальным");
            }
            name= value;
        }
    }
    public Student(){
        Name = "Ivan";
        Clas = 10;
        age= 16;  
    }
    public Student(double age, double clas, string name)
    {   
    Age = age;    
    Clas = clas;  
    Name = name;  
    }
    public virtual void Studying()
    {
        Console.WriteLine($"Студент по имени {name}, которому {age} лет, учится в группе {clas}");
    }
}
class  Bakalavr : Student
{
    private bool istestended;
    public bool Istestended
    {
        get => istestended; 
        set => istestended = value;
    }
    public void ZdalTest()
    {
        Console.WriteLine("Сдал ли ученик тесты? 1 - да, 2 - нет");
        if (Console.ReadLine() == "1")
        {
            istestended = true;
        }
        else
        {
            istestended = false;
        }
    }
    public Bakalavr(): base()
    {
        istestended = false;
    }
    public Bakalavr(bool istestended) : base()
    {
        Istestended= istestended;
    }
    public override void Studying()
    {
        base.Studying();
        Console.Write(". Бакалавр тест: ");
        if (istestended == true)
        {
            Console.Write("сдал");
        }
        else
        {
            Console.Write("не сдал");
        }
    }
    
}
class  Magistr : Student
{
    private bool isdiplomdeefed;
    public bool Isdiplomdeefed
    {
        get => isdiplomdeefed; 
        set => isdiplomdeefed = value;
    }
    public void ZdalDiplome()
    {
        Console.WriteLine("Сдал ли ученик диплом? 1 - да, 2 - нет");
        if (Console.ReadLine() == "1")
        {
            isdiplomdeefed = true;
        }
        else
        {
            isdiplomdeefed = false;
        }
    }
    public Magistr(): base()
    {
        isdiplomdeefed = false;
    }
    public Magistr(bool isdiplomdeefed) : this()
    {
        Isdiplomdeefed = isdiplomdeefed;
    }
    public override void Studying()
    {
        base.Studying();
        Console.Write(". Магистр тест: ");
        if (isdiplomdeefed == true)
        {
            Console.Write("сдал");
        }
        else
        {
            Console.Write("не сдал");
        }
    }
    
}
