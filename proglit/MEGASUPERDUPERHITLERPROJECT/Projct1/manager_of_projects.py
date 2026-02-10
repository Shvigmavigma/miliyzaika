def create():
    print("Добавьте заметку")
    l=input()
    z.append(l)
    pass
def delete():
    print(("найдите нужную заметку"))
    z.pop(search())
    print("Она удалена")
    pass
def search():
    print("Введите заметку которая вас интресует!")
    l=input()
    if l in z:
        print(f"Она находится под этим индексом {z.index(l)+1}!")
        return (z.index(l))
    else:
        print("Такой заметки не существует, перепроверь")
    pass
def close():
    with open("BD.txt", "w") as newfile:
        for i in z:
            if not i.endswith('\n'):
                file.write(i + '\n')
            else:
                file.write(i)
    print("Досвидания" "\n список ваших заметок обновлён")
    exit()
    pass
def show(z):
    print(("найдите нужную заметку!"))
    print("Вам нужна заметка по индексу(если это то введите 1), или индекс от интересующий заметки(если это то введите 2)")
    match input():
        case "1":
            print(f"Введите ваш индекс. Всего их {len(z)}")
            q=int(input())
            if q > len(z):
                print("Заметки с таким индексом не существует")
            else:
                print(z[(int(input())-1)])
        case "2":
            print(search())
        case _:
            print("Вы не ввели, ни одной из команд")
    
    pass
def interface():
    print("Здравствуй пользователь, это я менедежер твоих заметок")
    while True:
        phrase="""
команды:
1 - создать заметку
2 -  удалить заметку
3 - найти заметку
4 - закрыть ваш менеджер заметок
5 - показать заметку


Введите номер выбраной команды:
        """
        print(phrase)
        answer=input()


        # if answer == "1":
        #         create()
        # if answer == "2":
        #         delete()
        # if answer == "3":
        #         search()
        # if answer == "4":
        #         close()
        # if answer == "5":
        #         show()
        # else :
        #         print("Введеной вами команды не существует!!!, пожалуйста введите ещё раз.")
        #         continue
        match answer:
            case  "1":
                create()
            case "2":
                delete()
            case "3":
                search()
            case "4":
                close()
            case "5":
                show(z)
            case _:
                    print("Введеной вами команды не существует!!!, пожалуйста введите ещё раз.")
                    continue
if __name__ == "__main__":
    z=[]
    with open("BD.txt", "r+") as file:
        z=file.readlines()

        interface()