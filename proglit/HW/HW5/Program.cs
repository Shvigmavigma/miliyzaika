public interface IDamageble
{
    void Takedavage(int damage);
}
abstract class Character : IDamageble
{
    public string name;
    public int health;
    public abstract void Attack();
    public void Takedavage(int damage)
    {
        Console.WriteLine(damage);
        health=health-damage;
    }
    public void Characterinfo()
    {
        Console.WriteLine($"Имя = {name}, Здоровье = {health}");
    }
    public void Move()
    {
        Console.WriteLine("Персонаж переместился на 2 клетки");
    }
}
class Warrior : Character
{
    public Warrior()
    {
        health = 100;
        name = "Zumba";
    }
    public override void Attack()
    {
        Console.WriteLine("Удар мечом");
    }
}
class Mage : Character
{
    public override void Attack()
    {
        Console.WriteLine("Удар посохом");
    }
}
class Archer : Character
{
    public override void Attack()
    {
        Console.WriteLine("Удар стрелой");
    }
}
class Programm
{
    static void Main()
    {
        Character[] characters = new Character[3];

        characters[0] = new Warrior{name = "Goat"};
        characters[1] = new Mage{name= "tiger"};
        characters[2] = new Archer{name= "zumer"};

        foreach (Character character in characters)
        {
            character.Attack();
        }


        Warrior ob1 = new Warrior();
        Console.WriteLine("Был создан персонаж");
        ob1.Characterinfo();
        Console.WriteLine("Напишите сколько домага получит персонаж: ");
        ob1.Takedavage(int.Parse(Console.ReadLine()));
        Console.WriteLine("После изменений: ");
        ob1.Characterinfo();


        
    }
}
// 1 вопрос: почему данный класс должен быть абстрактным?
// Потому что у нас есть метод Attack и поля health и name которые принимаются классами наследниками и переопределяются, не предназначен для создания обьектов напрямую.
// 2 вопрос: почему при одинаковом типе переменной вызываются разные методы?
// потому что у нас в списке типа чарактер объекты других наследуемых классов и за счёт полиморфизма у нас выводятся разные сообщения.
