using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace TextGame
{
    // ======================== Интерфейсы ========================
    public interface ICommand
    {
        void Execute(Game game, string args);
    }

    public interface ICondition
    {
        bool IsTrue(IGameState state);
    }

    public interface IEffect
    {
        void Apply(IGameState state);
    }

    public interface IInteractable
    {
        string Id { get; }
        void Interact(Player player, IGameState state);
    }

    public interface IGameState
    {
        int Health { get; }
        List<string> Inventory { get; }
        Dictionary<string, bool> Flags { get; }
        int TurnCount { get; }
        List<string> Log { get; }
        int VillageReputation { get; }
    }

    // ======================== Абстрактные классы ========================
    public abstract class CommandBase : ICommand
    {
        protected string[] ParseArgs(string args)
        {
            return args?.Split(' ', StringSplitOptions.RemoveEmptyEntries) ?? new string[0];
        }
        public abstract void Execute(Game game, string args);
    }

    public abstract class ConditionBase : ICondition
    {
        public abstract bool IsTrue(IGameState state);
    }

    public abstract class EffectBase : IEffect
    {
        public abstract void Apply(IGameState state);
    }

    public abstract class GameEventBase
    {
        protected ICondition condition;
        protected List<IEffect> effects;
        protected bool isOneTime;
        protected bool triggered = false;

        public GameEventBase(ICondition condition, List<IEffect> effects, bool isOneTime = false)
        {
            this.condition = condition;
            this.effects = effects;
            this.isOneTime = isOneTime;
        }

        public void CheckAndApply(IGameState state)
        {
            if (isOneTime && triggered) return;
            if (condition.IsTrue(state))
            {
                foreach (var effect in effects)
                    effect.Apply(state);
                triggered = true;
            }
        }
    }

    // ======================== Основные классы состояния ========================
    public class GameState : IGameState
    {
        public int Health { get; set; } = 100;
        public List<string> Inventory { get; set; } = new List<string>();
        public Dictionary<string, bool> Flags { get; set; } = new Dictionary<string, bool>();
        public int TurnCount { get; set; } = 0;
        public List<string> Log { get; set; } = new List<string>();
        public int VillageReputation { get; set; } = 0; // от -3 до +3
    }

    public class Player
    {
        public string Name { get; set; } = "Герой";
    }

    public class Location
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public List<IInteractable> Objects { get; set; } = new List<IInteractable>();
        public List<GameEventBase> Events { get; set; } = new List<GameEventBase>();
        public Dictionary<string, string> Exits { get; set; } = new Dictionary<string, string>();
        public bool HasCombat { get; set; } = false;
        public Combat CombatData { get; set; } = new Combat(); // инициализация пустым объектом
    }

    // Класс для хранения данных боя
    public class Combat
    {
        public string EnemyName { get; set; } = "";
        public int RequiredCorrectAnswers { get; set; } = 3;
        public int Difficulty { get; set; } = 1; // 1-3
        public bool IsDefeated { get; set; } = false;
        public string VictoryMessage { get; set; } = "";
        public string RewardItem { get; set; } = ""; // предмет, который даётся после победы
    }

    // ======================== Система боя ========================
    public static class CombatSystem
    {
        private static Random rand = new Random();

        public static bool StartCombat(Combat combat, GameState state)
        {
            if (combat.IsDefeated)
            {
                Console.WriteLine($"Вы уже победили {combat.EnemyName}.");
                return true;
            }

            Console.WriteLine($"\n=== Бой с {combat.EnemyName}! ===");
            Console.WriteLine($"Вам нужно решить {combat.RequiredCorrectAnswers} математических примеров.");
            Console.WriteLine("Используйте зелье 'intellect' для упрощения (даст ответ на один пример).");
            Console.WriteLine("Команда: use intellect (если есть в инвентаре)");

            int correct = 0;
            int attempts = 0;
            int maxAttempts = combat.RequiredCorrectAnswers + 2; // небольшой запас

            while (correct < combat.RequiredCorrectAnswers && attempts < maxAttempts)
            {
                // Генерируем пример в зависимости от сложности
                int a, b, answer;
                string operation;
                switch (combat.Difficulty)
                {
                    case 1:
                        a = rand.Next(1, 10);
                        b = rand.Next(1, 10);
                        operation = "+";
                        answer = a + b;
                        break;
                    case 2:
                        a = rand.Next(1, 20);
                        b = rand.Next(1, 20);
                        if (rand.Next(2) == 0)
                        {
                            operation = "+";
                            answer = a + b;
                        }
                        else
                        {
                            operation = "-";
                            answer = a - b;
                            if (answer < 0) { answer = -answer; a = a + answer; } // корректировка
                        }
                        break;
                    default:
                        a = rand.Next(1, 30);
                        b = rand.Next(1, 10);
                        operation = "*";
                        answer = a * b;
                        break;
                }

                Console.Write($"Решите: {a} {operation} {b} = ");
                string input = Console.ReadLine();
                if (input.Equals("use intellect", StringComparison.OrdinalIgnoreCase))
                {
                    if (state.Inventory.Contains("intellect"))
                    {
                        state.Inventory.Remove("intellect");
                        Console.WriteLine($"Вы использовали зелье интеллекта. Ответ: {answer}");
                        correct++;
                        continue;
                    }
                    else
                    {
                        Console.WriteLine("У вас нет зелья интеллекта.");
                    }
                }

                if (int.TryParse(input, out int userAnswer) && userAnswer == answer)
                {
                    Console.WriteLine("Верно!");
                    correct++;
                }
                else
                {
                    Console.WriteLine("Неверно!");
                }
                attempts++;
            }

            if (correct >= combat.RequiredCorrectAnswers)
            {
                Console.WriteLine($"Победа! {combat.VictoryMessage}");
                combat.IsDefeated = true;
                if (!string.IsNullOrEmpty(combat.RewardItem))
                {
                    state.Inventory.Add(combat.RewardItem);
                    Console.WriteLine($"Вы получили: {combat.RewardItem}");
                }
                return true;
            }
            else
            {
                Console.WriteLine("Вы проиграли бой. Вы будете выброшены в предыдущую локацию.");
                return false;
            }
        }
    }

    // ======================== Конкретные команды ========================
    public class LookCommand : CommandBase
    {
        public override void Execute(Game game, string args)
        {
            var location = game.Locations[game.CurrentLocationId];
            Console.WriteLine(location.Name);
            Console.WriteLine(location.Description);
            if (location.Objects.Count > 0)
            {
                Console.WriteLine("Вы видите: " + string.Join(", ", location.Objects.Select(o => o.Id)));
            }
            if (location.Exits.Count > 0)
            {
                Console.WriteLine("Выходы: " + string.Join(", ", location.Exits.Keys));
            }
        }
    }

    public class GoCommand : CommandBase
    {
        public override void Execute(Game game, string args)
        {
            var parts = ParseArgs(args);
            if (parts.Length == 0)
            {
                // Выводим список доступных направлений
                var currentLoc = game.Locations[game.CurrentLocationId];
                if (currentLoc.Exits.Count == 0)
                    Console.WriteLine("Отсюда никуда не уйти.");
                else
                    Console.WriteLine("Доступные направления: " + string.Join(", ", currentLoc.Exits.Keys));
                return;
            }
            string direction = parts[0];
            var fromLoc = game.Locations[game.CurrentLocationId];
            if (fromLoc.Exits.ContainsKey(direction))
            {
                string nextId = fromLoc.Exits[direction];

                // Блокировка входа в деревню при плохой репутации
                if (nextId == "village" && game.State.Flags.ContainsKey("village_banned"))
                {
                    Console.WriteLine("Вас не пускают в деревню. Жители отвернулись от вас.");
                    return;
                }

                // Проверяем, открыта ли локация (для болота, гнезда, логова)
                if ((nextId == "swamp" && !game.State.Flags.ContainsKey("swamp_unlocked")) ||
                    (nextId == "nest" && !game.State.Flags.ContainsKey("nest_unlocked")) ||
                    (nextId == "lair" && !game.State.Flags.ContainsKey("lair_unlocked")))
                {
                    Console.WriteLine("Путь закрыт. Вам нужно выполнить предыдущие задания.");
                    return;
                }

                game.CurrentLocationId = nextId;
                Console.WriteLine($"Вы идёте {direction}.");
                // Если в локации есть бой и он ещё не пройден, запускаем бой
                var newLoc = game.Locations[nextId];
                if (newLoc.HasCombat && newLoc.CombatData != null && !newLoc.CombatData.IsDefeated)
                {
                    bool victory = CombatSystem.StartCombat(newLoc.CombatData, game.State);
                    if (!victory)
                    {
                        // Возвращаемся обратно
                        game.CurrentLocationId = fromLoc.Id;
                        Console.WriteLine("Вы отступили и вернулись назад.");
                    }
                    else
                    {
                        // После победы открываем следующую локацию
                        if (nextId == "swamp") game.State.Flags["nest_unlocked"] = true;
                        if (nextId == "nest") game.State.Flags["lair_unlocked"] = true;
                        if (nextId == "lair") 
                        {
                            // В логове дракона после боя даём ключ
                            game.State.Inventory.Add("ключ от церкви");
                            Console.WriteLine("Вы нашли ключ от церкви!");
                        }
                    }
                }
                // Выполняем look автоматически
                var lookCmd = new LookCommand();
                lookCmd.Execute(game, "");
            }
            else
            {
                Console.WriteLine("Туда нельзя пойти.");
            }
        }
    }

    public class InteractCommand : CommandBase
    {
        public override void Execute(Game game, string args)
        {
            var parts = ParseArgs(args);
            if (parts.Length == 0)
            {
                var location = game.Locations[game.CurrentLocationId];
                if (location.Objects.Count == 0)
                    Console.WriteLine("Здесь нет объектов для взаимодействия.");
                else
                    Console.WriteLine("Доступные объекты: " + string.Join(", ", location.Objects.Select(o => o.Id)));
                return;
            }
            string objId = parts[0];
            var currentLoc = game.Locations[game.CurrentLocationId];
            var obj = currentLoc.Objects.FirstOrDefault(o => string.Equals(o.Id, objId, StringComparison.OrdinalIgnoreCase));
            if (obj != null)
            {
                obj.Interact(new Player(), game.State);
            }
            else
            {
                Console.WriteLine($"Здесь нет объекта '{objId}'.");
            }
        }
    }

    public class InventoryCommand : CommandBase
    {
        public override void Execute(Game game, string args)
        {
            if (game.State.Inventory.Count == 0)
            {
                Console.WriteLine("Ваш инвентарь пуст.");
            }
            else
            {
                Console.WriteLine("В инвентаре: " + string.Join(", ", game.State.Inventory));
            }
        }
    }

    public class HelpCommand : CommandBase
    {
        public override void Execute(Game game, string args)
        {
            Console.WriteLine("Доступные команды:");
            Console.WriteLine("  look - осмотреться");
            Console.WriteLine("  go [направление] - пойти (без аргумента показывает доступные выходы)");
            Console.WriteLine("  interact [объект] - взаимодействовать (без аргумента показывает объекты)");
            Console.WriteLine("  inventory / inv - показать инвентарь");
            Console.WriteLine("  status - показать состояние");
            Console.WriteLine("  help - справка");
            Console.WriteLine("  use [предмет] - использовать предмет (без аргумента показывает что можно использовать)");
            Console.WriteLine("  buy [предмет] - купить на рынке (без аргумента показывает список товаров)");
            Console.WriteLine("  talk - поговорить (в деревне)");
            Console.WriteLine("  quest - взять/сдать задание (в подвале)");
        }
    }

    public class StatusCommand : CommandBase
    {
        public override void Execute(Game game, string args)
        {
            Console.WriteLine($"Здоровье: {game.State.Health}");
            Console.WriteLine($"Ход: {game.State.TurnCount}");
            Console.WriteLine($"Репутация в деревне: {game.State.VillageReputation}");
        }
    }

    public class UseCommand : CommandBase
    {
        public override void Execute(Game game, string args)
        {
            var parts = ParseArgs(args);
            if (parts.Length == 0)
            {
                var usable = game.State.Inventory.Where(i => i.StartsWith("зелье")).ToList();
                if (usable.Count == 0)
                    Console.WriteLine("У вас нет предметов, которые можно использовать.");
                else
                    Console.WriteLine("Можно использовать: " + string.Join(", ", usable));
                return;
            }
            string item = parts[0].ToLower();
            if (!game.State.Inventory.Contains(item))
            {
                Console.WriteLine($"У вас нет {item}.");
                return;
            }
            // Эффекты от зелий
            if (item == "зелье здоровья")
            {
                game.State.Health += 30;
                game.State.Inventory.Remove(item);
                Console.WriteLine("Вы выпили зелье здоровья. +30 здоровья.");
            }
            else if (item == "зелье силы")
            {
                game.State.Flags["strength_potion"] = true;
                game.State.Inventory.Remove(item);
                Console.WriteLine("Вы использовали зелье силы. Следующий бой будет легче.");
            }
            else if (item == "зелье интеллекта")
            {
                Console.WriteLine("Зелье интеллекта поможет вам в бою. Оно будет использовано автоматически при решении примера.");
                // Оставляем в инвентаре, бой сам его использует
            }
            else
            {
                Console.WriteLine("Этот предмет нельзя использовать.");
            }
        }
    }

    public class BuyCommand : CommandBase
    {
        public override void Execute(Game game, string args)
        {
            var parts = ParseArgs(args);
            if (parts.Length == 0)
            {
                Console.WriteLine("Доступные товары: зелье здоровья, зелье силы, зелье интеллекта");
                return;
            }
            string item = parts[0].ToLower();
            if (item == "зелье здоровья")
            {
                game.State.Inventory.Add("зелье здоровья");
                Console.WriteLine("Вы купили зелье здоровья.");
            }
            else if (item == "зелье силы")
            {
                game.State.Inventory.Add("зелье силы");
                Console.WriteLine("Вы купили зелье силы.");
            }
            else if (item == "зелье интеллекта")
            {
                game.State.Inventory.Add("зелье интеллекта");
                Console.WriteLine("Вы купили зелье интеллекта.");
            }
            else
            {
                Console.WriteLine("Такого товара нет.");
            }
        }
    }

    // ======================== Диалоги в деревне ========================
    public class DialogTopic
    {
        public string Question { get; set; }
        public string Option1 { get; set; }
        public string Option2 { get; set; }
        public int ReputationChange1 { get; set; }
        public int ReputationChange2 { get; set; }

        public DialogTopic(string question, string option1, string option2, int change1, int change2)
        {
            Question = question;
            Option1 = option1;
            Option2 = option2;
            ReputationChange1 = change1;
            ReputationChange2 = change2;
        }
    }

    public class TalkCommand : CommandBase
    {
        private static Random rand = new Random();
        private static List<DialogTopic> topics = new List<DialogTopic>
        {
            new DialogTopic(
                "Житель: 'Слышал, ты собираешься в опасное путешествие? Что тебя туда толкает?'",
                "1) Хочу защитить деревню и стать героем!",
                "2) Мне просто любопытно, что там за монстры.",
                1, 0),
            new DialogTopic(
                "Старушка: 'Дитя, ты веришь в добро?'",
                "1) Конечно, добро всегда побеждает зло!",
                "2) Не знаю... мир жесток.",
                1, -1),
            new DialogTopic(
                "Кузнец: 'Твои доспехи в плачевном состоянии. Ты сражался с кем-то?'",
                "1) Да, пришлось защищать слабых.",
                "2) Я ищу сильных врагов, чтобы доказать свою силу.",
                1, -1)
        };

        public override void Execute(Game game, string args)
        {
            if (game.CurrentLocationId != "village")
            {
                Console.WriteLine("Здесь не с кем поговорить.");
                return;
            }

            if (game.State.Flags.ContainsKey("village_banned"))
            {
                Console.WriteLine("Жители деревни отказываются с вами разговаривать.");
                return;
            }

            // Выбираем случайную тему
            var topic = topics[rand.Next(topics.Count)];

            Console.WriteLine(topic.Question);
            Console.WriteLine(topic.Option1);
            Console.WriteLine(topic.Option2);
            Console.Write("Ваш выбор (1 или 2): ");

            string input = Console.ReadLine();
            int change = 0;
            if (input == "1")
            {
                change = topic.ReputationChange1;
                Console.WriteLine("Житель: 'Хороший ответ, молодец!'");
            }
            else if (input == "2")
            {
                change = topic.ReputationChange2;
                Console.WriteLine("Житель: 'Как печально это слышать...'");
            }
            else
            {
                Console.WriteLine("Житель не понял вашего ответа и ушёл.");
                change = -1;
            }

            game.State.VillageReputation += change;
            Console.WriteLine($"Ваша репутация в деревне изменилась на {change}. Теперь она {game.State.VillageReputation}.");

            if (game.State.VillageReputation <= -3)
            {
                game.State.Flags["village_banned"] = true;
                Console.WriteLine("Жители деревни разгневаны вашими словами! Вас больше не пустят сюда.");
            }
        }
    }

    public class QuestCommand : CommandBase
    {
        public override void Execute(Game game, string args)
        {
            if (game.CurrentLocationId != "basement")
            {
                Console.WriteLine("Задания выдаются только в подвале.");
                return;
            }
            if (!game.State.Flags.ContainsKey("quest_taken"))
            {
                Console.WriteLine("Задание: победить орков в болоте, грифонов в гнезде и дракона в логове.");
                Console.WriteLine("После выполнения каждого задания возвращайтесь сюда за наградой.");
                game.State.Flags["quest_taken"] = true;
            }
            else
            {
                // Проверяем, какие задания выполнены
                if (game.State.Flags.ContainsKey("swamp_cleared") && !game.State.Flags.ContainsKey("swamp_rewarded"))
                {
                    game.State.Flags["swamp_rewarded"] = true;
                    game.State.Inventory.Add("зелье силы");
                    Console.WriteLine("Вы получили награду за болото: зелье силы.");
                }
                if (game.State.Flags.ContainsKey("nest_cleared") && !game.State.Flags.ContainsKey("nest_rewarded"))
                {
                    game.State.Flags["nest_rewarded"] = true;
                    game.State.Inventory.Add("зелье здоровья");
                    Console.WriteLine("Вы получили награду за гнездо: зелье здоровья.");
                }
                if (game.State.Flags.ContainsKey("lair_cleared") && !game.State.Flags.ContainsKey("lair_rewarded"))
                {
                    game.State.Flags["lair_rewarded"] = true;
                    game.State.Inventory.Add("зелье интеллекта");
                    Console.WriteLine("Вы получили награду за логово: зелье интеллекта.");
                }
                if (game.State.Flags.ContainsKey("swamp_rewarded") && game.State.Flags.ContainsKey("nest_rewarded") && game.State.Flags.ContainsKey("lair_rewarded"))
                {
                    Console.WriteLine("Все задания выполнены! Теперь вы можете открыть церковь.");
                    game.State.Flags["church_ready"] = true;
                }
            }
        }
    }

    // ======================== Конкретные объекты ========================
    public class Chest : IInteractable
    {
        public string Id { get; private set; }
        private List<string> items;
        private bool isOpen;
        private string requiredKey;
        private string message;

        public Chest(string id, List<string> items, string requiredKey = null, string message = null)
        {
            Id = id;
            this.items = items;
            this.requiredKey = requiredKey;
            this.message = message;
            isOpen = false;
        }

        public void Interact(Player player, IGameState state)
        {
            if (isOpen)
            {
                Console.WriteLine("Сундук уже открыт.");
                return;
            }
            if (!string.IsNullOrEmpty(requiredKey) && !state.Inventory.Contains(requiredKey))
            {
                Console.WriteLine($"Сундук заперт. Нужен {requiredKey}.");
                return;
            }
            if (!string.IsNullOrEmpty(message))
            {
                Console.WriteLine(message);
            }
            isOpen = true;
            foreach (var item in items)
                ((GameState)state).Inventory.Add(item);
            Console.WriteLine($"Вы нашли: {string.Join(", ", items)}.");
        }
    }

    public class NPC : IInteractable
    {
        public string Id { get; private set; }
        private string dialogue;
        private List<IEffect> onTalkEffects;

        public NPC(string id, string dialogue, List<IEffect> onTalkEffects = null)
        {
            Id = id;
            this.dialogue = dialogue;
            this.onTalkEffects = onTalkEffects ?? new List<IEffect>();
        }

        public void Interact(Player player, IGameState state)
        {
            Console.WriteLine(dialogue);
            foreach (var effect in onTalkEffects)
                effect.Apply(state);
        }
    }

    // ======================== Конкретные события ========================
    public class OneTimeEvent : GameEventBase
    {
        public OneTimeEvent(ICondition condition, List<IEffect> effects) : base(condition, effects, true) { }
    }

    // ======================== Игровой класс ========================
    public class Game
    {
        public GameState State { get; private set; }
        public Dictionary<string, Location> Locations { get; private set; }
        public string CurrentLocationId { get; set; } = "";
        public bool IsGameOver { get; set; }
        private Dictionary<string, ICommand> commands;
        private Random random = new Random();

        // Список случайных сообщений от жителей (если репутация низкая)
        private List<string> hostileMessages = new List<string>
        {
            "Вы слышите шёпот: 'Это он... тот, кто приносит беду.'",
            "Прохожий отворачивается, когда вы проходите мимо.",
            "Кто-то бросает вам вслед: 'Убирайся отсюда, злодей!'",
            "Вы замечаете, как люди закрывают двери при вашем приближении.",
            "Ребёнок указывает на вас пальцем и плачет. Мать быстро уводит его.",
            "Вам кажется, что все взгляды полны ненависти. Но вы не понимаете почему.",
            "Старик сплёвывает вам под ноги: 'Проклятый! Изыди!'",
            "Женщина шепчет соседке: 'Он убил моего мужа...'"
        };

        public Game()
        {
            State = new GameState();
            Locations = new Dictionary<string, Location>();
            commands = new Dictionary<string, ICommand>(StringComparer.OrdinalIgnoreCase);
            RegisterCommands();
        }

        private void RegisterCommands()
        {
            commands["look"] = new LookCommand();
            commands["go"] = new GoCommand();
            commands["inventory"] = new InventoryCommand();
            commands["inv"] = new InventoryCommand();
            commands["help"] = new HelpCommand();
            commands["status"] = new StatusCommand();
            commands["interact"] = new InteractCommand();
            commands["use"] = new UseCommand();
            commands["buy"] = new BuyCommand();
            commands["talk"] = new TalkCommand();
            commands["quest"] = new QuestCommand();
        }

        public void Run()
        {
            // Создаём локации
            CreateLocations();

            // Первая консцена
            RunIntroCutscene();

            while (!IsGameOver)
            {
                var currentLoc = Locations[CurrentLocationId];
                // Проверка событий (можно расширить)
                foreach (var evt in currentLoc.Events)
                    evt.CheckAndApply(State);

                // Случайные враждебные сообщения (если репутация плохая и игрок не в деревне)
                if (State.VillageReputation < 0 && random.Next(10) < 3 && CurrentLocationId != "village")
                {
                    string msg = hostileMessages[random.Next(hostileMessages.Count)];
                    Console.WriteLine($"\n[Случайное событие] {msg}\n");
                }

                Console.Write("\n> ");
                string input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input)) continue;

                ProcessCommand(input);

                State.TurnCount++;

                if (State.Health <= 0)
                {
                    Console.WriteLine("Вы погибли. Игра окончена.");
                    IsGameOver = true;
                }
            }
            Console.WriteLine("Нажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        private void ProcessCommand(string input)
        {
            var parts = input.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            string commandWord = parts[0].ToLower();
            string args = parts.Length > 1 ? parts[1] : "";

            if (commands.ContainsKey(commandWord))
            {
                commands[commandWord].Execute(this, args);
            }
            else
            {
                Console.WriteLine("Неизвестная команда. Введите help.");
            }
        }

        private void CreateLocations()
        {
            // 1. Зал начала (церковь)
            var church = new Location
            {
                Id = "church",
                Name = "Церковь",
                Description = "Мрачный зал. В центре стоит старый сундук.",
                Exits = new Dictionary<string, string> { { "наружу", "market" } }
            };
            church.Objects.Add(new Chest("сундук", new List<string>(), "ключ от церкви", "Сундук заперт."));

            // 2. Рынок
            var market = new Location
            {
                Id = "market",
                Name = "Рынок",
                Description = "Шумная торговая площадь. Здесь можно купить зелья (команда buy).",
                Exits = new Dictionary<string, string> { { "церковь", "church" }, { "деревня", "village" }, { "подвал", "basement" } }
            };

            // 3. Деревня магов (без NPC, диалог реализован в команде TalkCommand)
            var village = new Location
            {
                Id = "village",
                Name = "Деревня магов",
                Description = "Уютная деревня. Здесь можно поговорить с жителями (команда talk).",
                Exits = new Dictionary<string, string> { { "рынок", "market" } }
            };

            // 4. Подвал (черный рынок)
            var basement = new Location
            {
                Id = "basement",
                Name = "Подвал",
                Description = "Тёмное помещение. Здесь выдают задания (команда quest).",
                Exits = new Dictionary<string, string> { { "рынок", "market" } }
            };

            // 5. Болото орков
            var swamp = new Location
            {
                Id = "swamp",
                Name = "Болото орков",
                Description = "Грязное болото, кишащее орками.",
                HasCombat = true,
                CombatData = new Combat
                {
                    EnemyName = "Орки",
                    RequiredCorrectAnswers = 3,
                    Difficulty = 1,
                    VictoryMessage = "Орки разбежались!",
                    RewardItem = "трофей орков"
                },
                Exits = new Dictionary<string, string> { { "назад", "basement" }, { "гнездо", "nest" } }
            };
            swamp.Events.Add(new OneTimeEvent(
                new FlagCondition("swamp_cleared"),
                new List<IEffect> { new SetFlagEffect("swamp_cleared", true) }
            ));
            swamp.CombatData.VictoryMessage = "Вы победили орков! Теперь путь к гнезду открыт.";

            // 6. Гнездо грифонов
            var nest = new Location
            {
                Id = "nest",
                Name = "Гнездо грифонов",
                Description = "Высоко в скалах гнездо грифонов.",
                HasCombat = true,
                CombatData = new Combat
                {
                    EnemyName = "Грифоны",
                    RequiredCorrectAnswers = 4,
                    Difficulty = 2,
                    VictoryMessage = "Грифоны отступили!",
                    RewardItem = "перо грифона"
                },
                Exits = new Dictionary<string, string> { { "назад", "swamp" }, { "логово", "lair" } }
            };
            nest.CombatData.VictoryMessage = "Вы победили грифонов! Путь к логову открыт.";

            // 7. Логово дракона
            var lair = new Location
            {
                Id = "lair",
                Name = "Логово дракона",
                Description = "Пещера, где спит дракон.",
                HasCombat = true,
                CombatData = new Combat
                {
                    EnemyName = "Дракон",
                    RequiredCorrectAnswers = 5,
                    Difficulty = 3,
                    VictoryMessage = "Дракон повержен!",
                    RewardItem = "ключ от церкви"
                },
                Exits = new Dictionary<string, string> { { "назад", "nest" } }
            };

            // Добавляем локации
            Locations.Add(church.Id, church);
            Locations.Add(market.Id, market);
            Locations.Add(village.Id, village);
            Locations.Add(basement.Id, basement);
            Locations.Add(swamp.Id, swamp);
            Locations.Add(nest.Id, nest);
            Locations.Add(lair.Id, lair);

            // Начальная локация
            CurrentLocationId = "church";

            // Флаги: открытие последующих локаций
            State.Flags["swamp_unlocked"] = true;  // болото открыто с начала
            State.Flags["nest_unlocked"] = false;
            State.Flags["lair_unlocked"] = false;
            State.Flags["quest_taken"] = false;
        }

        private void RunIntroCutscene()
        {
            Console.Clear();
            Console.WriteLine("=== Пролог ===");
            PrintWithDelay("Вы просыпаетесь в полумраке старой церкви.", 2000);
            PrintWithDelay("Перед вами стоит массивный сундук.", 2000);
            PrintWithDelay("Вы подходите к нему...", 2000);
            PrintWithDelay("Из сундука доносится голос: 'Лишь себя и сам мир осознав, познаешь дары ты мои.'", 2000);
            PrintWithDelay("Вдруг пол поднимается и с силой выбрасывает вас наружу!", 2000);
            PrintWithDelay("Ворота церкви с грохотом закрываются.", 2000);
            PrintWithDelay("Вы слышите, как засов защёлкивается.", 2000);
            PrintWithDelay("Чтобы вернуться, нужен ключ...", 2000);
            Console.WriteLine("\nИгра началась. Введите help для списка команд.");
            CurrentLocationId = "market"; // теперь герой на рынке
        }

        private void PrintWithDelay(string text, int milliseconds)
        {
            Console.WriteLine(text);
            Thread.Sleep(milliseconds);
        }
    }

    // ======================== Простые эффекты и условия ========================
    public class SetFlagEffect : EffectBase
    {
        private string flag;
        private bool value;
        public SetFlagEffect(string flag, bool value)
        {
            this.flag = flag;
            this.value = value;
        }
        public override void Apply(IGameState state)
        {
            ((GameState)state).Flags[flag] = value;
        }
    }

    public class FlagCondition : ConditionBase
    {
        private string flag;
        private bool expected;
        public FlagCondition(string flag, bool expected = true)
        {
            this.flag = flag;
            this.expected = expected;
        }
        public override bool IsTrue(IGameState state)
        {
            return state.Flags.ContainsKey(flag) && state.Flags[flag] == expected;
        }
    }

    // ======================== Точка входа ========================
    class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game();
            game.Run();
        }
    }
}