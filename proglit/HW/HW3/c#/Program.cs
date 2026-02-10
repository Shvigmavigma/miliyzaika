using System.Threading;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Globalization;
using System.Text.RegularExpressions;
using System;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Security.Permissions;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.IO.Compression;
class Polynomia{
    private double[] coefs;
    private int degree;
    private double[] Coefs{
        get => (double[]) this.coefs.Clone();
    }
    public int Degree{
        get=> this.degree;
    }
    public Polynomia(){
        this.coefs = new double[]{0};
        this.degree= 1;
    }
    public Polynomia(double[] new_coefs){
        this.degree=new_coefs.Length -1;
        this.coefs= (double[])new_coefs.Clone();
    }
    public override string ToString()
    {
        string z= "";
        int l=this.Coefs.Length;
        for(int i=0; i<this.Coefs.Length; i++ ){
            if (coefs[i] != 0)
            {
                switch (i)
                {
                    case 0:
                        if(i!=coefs.Length && coefs[i+1]<0){z+= coefs[i] + " - ";}
                        else{z+=coefs[i];}
                        break;
                    case 1:                    
                        if (coefs[i-1]!=0){
                            if (coefs[i] <0 ){    
                                if(i!=coefs.Length && coefs[i+1]<0){z+= Math.Abs(coefs[i]) + "x"+" - ";}
                                else{z+=Math.Abs(coefs[i]) +"x";}
                                }
                            else{
                                if(i!=coefs.Length && coefs[i+1]<0){z+=" + " + coefs[i] + "x"+" - ";}
                                else{z+=" + " + coefs[i] +"x";}
                                }
                        }
                        else{
                            if (coefs[i] <0 ){    
                                if(i!=coefs.Length && coefs[i+1]<0){z+=" - "+ Math.Abs(coefs[i]) + "x"+" - ";}
                                else{z+=" - "+ Math.Abs(coefs[i]) +"x";}
                                }
                            else{
                                if(i!=coefs.Length && coefs[i+1]<0){z+=" + " + coefs[i] + "x"+" - ";}
                                else{z+=" + " + coefs[i] +"x";}
                                }
                        }
                        break;
                    default:
                        if (coefs[i-1]!=0){      
                            if(coefs[i]<0){           
                                if(i!=coefs.Length-1 && coefs[i+1]<0){z+=Math.Abs(coefs[i]) +"x^"+ i + " - ";}
                                else{z+= Math.Abs(coefs[i]) +"x^"+ i;}
                            }
                            else
                            {
                                if(i!=coefs.Length-1 && coefs[i+1]<0){z+=" + " + coefs[i] +"x^"+ i + " - ";}
                                else{z+= " + " + coefs[i] +"x^"+ i;}
                            }
                        }
                        else
                        {
                            if (coefs[i] < 0)
                            {
                                if(i!=coefs.Length-1 && coefs[i+1]<0){z+=" - "+ Math.Abs(coefs[i]) +"x^"+ i + " - ";}
                                else{z+=" - "+ Math.Abs(coefs[i]) +"x^"+ i;}
                            }


                            else
                            {
                                if(i!=coefs.Length-1 && coefs[i+1]<0){z+=" + " + coefs[i] +"x^"+ i + " - ";}
                                else{z+= " + " + coefs[i] +"x^"+ i;}
                            }
                        }
                        break;
                }
            }
        }
        return z;
    }
public static Polynomia operator+(Polynomia obj1, Polynomia obj2)
{
    int maxLength = Math.Max(obj1.coefs.Length, obj2.coefs.Length);
    double[] result = new double[maxLength];
    
    for(int i = 0; i < maxLength; i++)
    {
        double coef1 = (i < obj1.coefs.Length) ? obj1.coefs[i] : 0;
        double coef2 = (i < obj2.coefs.Length) ? obj2.coefs[i] : 0;
        result[i] = coef1 + coef2;
    }
    
    return new Polynomia(result);
}
    
    public static Polynomia operator+(Polynomia obj, double k){
        int g = obj.coefs.Length;
        double[] coefs= new double[3];
        Array.Resize(ref coefs, g);
        for(int i = 0 ; i<g; i++)
        {
            coefs[i]=obj.coefs[i]+k;
        }
        return new Polynomia(coefs);
    }
    public static Polynomia operator-(Polynomia obj1, Polynomia obj2){
        int k =  obj1.coefs.Length > obj2.coefs.Length ?  obj1.coefs.Length : obj2.coefs.Length;
        Polynomia mensh = obj1.coefs.Length < obj2.coefs.Length ?  obj1 : obj2;
        Polynomia bolsh = obj2.coefs.Length > obj1.coefs.Length ?  obj2 : obj1;
        double[] coefs= new double[3];
        Array.Resize(ref coefs, k);
        for(int i =0; i<k; i++){
            if (bolsh.coefs.Length>i){
                if (mensh.coefs.Length>i){
                    coefs[i]= bolsh.coefs[i] - mensh.coefs[i];


                }
                else{
                    coefs[i]=bolsh.coefs[i];
                }
            }
            else if(bolsh.coefs.Length>i){
                coefs[i]=bolsh.coefs[i];
            }

        }

        return new Polynomia(coefs);}
    public static Polynomia operator-(Polynomia obj, double k){
        int g = obj.coefs.Length;
        double[] coefs= new double[3];
        Array.Resize(ref coefs, g);
        for(int i = 0 ; i<g; i++)
        {
            coefs[i]=obj.coefs[i]-k;
        }
        return new Polynomia(coefs);
        }
    public static Polynomia operator / (Polynomia obj1,Polynomia obj2)
    {
        int k =  obj1.coefs.Length > obj2.coefs.Length ?  obj1.coefs.Length : obj2.coefs.Length;
        Polynomia mensh = obj1.coefs.Length < obj2.coefs.Length ?  obj1 : obj2;
        Polynomia bolsh = obj2.coefs.Length > obj1.coefs.Length ?  obj2 : obj1;
        double[] coefs= new double[3];
        Array.Resize(ref coefs, k);
        for(int i =0; i<k; i++){
            if (bolsh.coefs.Length>i){
                if (mensh.coefs.Length>i){
                    coefs[i]= bolsh.coefs[i] / mensh.coefs[i];


                }
                else{
                    coefs[i]=bolsh.coefs[i];
                }
            }
            else if(bolsh.coefs.Length>i){
                coefs[i]=bolsh.coefs[i];
            }

        }
        return new Polynomia(coefs);
    }

    public static Polynomia operator / (Polynomia obj, double k)
    {   
        int g = obj.coefs.Length;
        double[] coefs= new double[3];
        Array.Resize(ref coefs, g);
        for(int j = 0 ; j<g; j++)
        {
            coefs[j]=obj.coefs[j]/k;
        }
        return new Polynomia(coefs);
    }
    public static Polynomia CeloChislennoeDelenie(Polynomia obj, double k)
    {   
        int g = obj.coefs.Length;
        double[] coefs= new double[3];
        Array.Resize(ref coefs, g);
        for(int j = 0 ; j<g; j++)
        {
            coefs[j]=Convert.ToInt32(obj.coefs[j])/Convert.ToInt32(k);
        }
        return new Polynomia(coefs);
    }
    public static Polynomia CeloChislennoeDelenie (Polynomia obj1,Polynomia obj2)
    {
        int k =  obj1.coefs.Length > obj2.coefs.Length ?  obj1.coefs.Length : obj2.coefs.Length;
        Polynomia mensh = obj1.coefs.Length < obj2.coefs.Length ?  obj1 : obj2;
        Polynomia bolsh = obj2.coefs.Length > obj1.coefs.Length ?  obj2 : obj1;
        double[] coefs= new double[3];
        Array.Resize(ref coefs, k);
        for(int i =0; i<k; i++){
            if (bolsh.coefs.Length>i){
                if (mensh.coefs.Length>i){
                    coefs[i]= Convert.ToInt32(bolsh.coefs[i]) / Convert.ToInt32(mensh.coefs[i]);


                }
                else{
                    coefs[i]=bolsh.coefs[i];
                }
            }
            else if(bolsh.coefs.Length>i){
                coefs[i]=bolsh.coefs[i];
            }

        }
        return new Polynomia(coefs);
        }
    public static Polynomia operator * (Polynomia obj, double k)
    {   
        int g = obj.coefs.Length;
        double[] coefs= new double[3];
        Array.Resize(ref coefs, g);
        for(int i = 0 ; i<g; i++)
        {
            coefs[i]=obj.coefs[i]*k;
        }
        return new Polynomia(coefs);
    }
    public static Polynomia operator * (Polynomia obj1, Polynomia obj2)
    {
        int k =  obj1.coefs.Length > obj2.coefs.Length ?  obj1.coefs.Length : obj2.coefs.Length;
        Polynomia mensh = obj1.coefs.Length < obj2.coefs.Length ?  obj1 : obj2;
        Polynomia bolsh = obj2.coefs.Length > obj1.coefs.Length ?  obj2 : obj1;
        double[] coefs= new double[3];
        Array.Resize(ref coefs, k);
        for(int i =0; i<k; i++){
            if (bolsh.coefs.Length>i){
                if (mensh.coefs.Length>i){
                    coefs[i]= Convert.ToInt32(bolsh.coefs[i]) * Convert.ToInt32(mensh.coefs[i]);


                }
                else{
                    coefs[i]=bolsh.coefs[i];
                }
            }
            else if(bolsh.coefs.Length>i){
                coefs[i]=bolsh.coefs[i];
            }
        
    }
    return new Polynomia(coefs);
    
    }
    public static Polynomia Vstepen(Polynomia obj, double k)
    {
        int g = obj.coefs.Length;
        double[] coefs= new double[3];
        Array.Resize(ref coefs, g);
        for(int i = 0 ; i<g; i++)
        {
            coefs[i]=Math.Pow(obj.coefs[i], k);
        }
        return new Polynomia(coefs); 
    }
    public static Polynomia Koreniz(Polynomia obj, double k)
    {   
        int g = obj.coefs.Length;
        double[] coefs= new double[3];
        Array.Resize(ref coefs, g);
        for(int i = 0 ; i<g; i++)
        {
            coefs[i]=Math.Pow(obj.coefs[i], 1.0/k);
        }
        return new Polynomia(coefs);
    }
}
class Programm
{
    static void Main(string[] args)
    {   
        bool shouldexit = false;
        {Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;
        double[] coeffs = new double[] {1.0, 0.0, 2.0 };
        Polynomia p = new Polynomia(coeffs);
        Console.WriteLine("При дефолтных значениях [1.0, 0.0, 2.0]");
        Console.WriteLine(p);

        double[] coffs = new double[] {};
        Console.WriteLine("Введите сколько будет коэффицентов");
        int q= int.Parse(Console.ReadLine());
        if (q >0){
        for (int i=0; i!=q; i++)
        {
            Console.WriteLine("Введите элемент №" + (i+1));
            Array.Resize(ref coffs, coffs.Length + 1);
            coffs[coffs.Length - 1] = Convert.ToDouble(Console.ReadLine());
        }
        Polynomia z = new Polynomia(coffs);
        Console.WriteLine("Получился вот такой многочлен:");
        Console.WriteLine(z.ToString());
        while (!shouldexit){
        Console.WriteLine(@"Выберите опперацию которую хотите применить к вашим многочленам:
+ - Сложить все члены выбраного многочлена с коэффицентом
++ - Сложить коэффиценты многочленов
- - Вычесть из всех членов выбраного многочлена коэффицент
-- - Вычесть коэффицент(из большего по длине меньший)
/ - Поделить все члены выбраного многочлена на коэффицент
// - Поделить нацело все члены выбраного многочлена на коэффицент
/// - Поделить члены многочленов между собой
//// - Поделить нацело члены многочленов между собой
* - Умножить все челны выбранного многочлена на коэффицент
** - Умножить члены многочленов между собой
^ - Возвести в степень все челны выбранного многочлена на коэффицент
v - Вывести корень из всех членов выбранного многочлена на коэффицент
stop - прервать действие функции");
        switch(Console.ReadLine()){
        case "+":
        Console.WriteLine("Какой многочлен вы хотите изменить: 1 - дефолтный, 2 - свой");
        string n1=Console.ReadLine();
        Console.WriteLine("Введите коэффицент прибавления:");
        if (n1=="1"){
        Polynomia ob1 = p + Convert.ToDouble(Console.ReadLine());
        Console.WriteLine("Получился многочлен:");
        Console.WriteLine(ob1.ToString());}
        else{Polynomia ob1 = z + Convert.ToDouble(Console.ReadLine());
        Console.WriteLine("Получился многочлен:");
        Console.WriteLine(ob1.ToString());}
            break; 
        case "++":
        Polynomia  ob2 = p + z;
        Console.WriteLine("Получился многочлен:");
        Console.WriteLine(ob2.ToString());;
            break;
        case "-":
        Console.WriteLine("Какой многочлен вы хотите изменить: 1 - дефолтный, 2 - свой");
        string n2=Console.ReadLine();
        Console.WriteLine("Введите коэффицент вычитания:");
        if (n2=="1"){
        Polynomia ob1 = p - Convert.ToDouble(Console.ReadLine());
        Console.WriteLine("Получился многочлен:");
        Console.WriteLine(ob1.ToString());}
        else{Polynomia ob1 = z - Convert.ToDouble(Console.ReadLine());
        Console.WriteLine("Получился многочлен:");
        Console.WriteLine(ob1.ToString());}
            break; 
        case "--":
        Polynomia  ob4 = p - z;
        Console.WriteLine("Получился многочлен:");
        Console.WriteLine(ob4.ToString());
            break;
        case "/":
        Console.WriteLine("Какой многочлен вы хотите изменить: 1 - дефолтный, 2 - свой");
        string n3=Console.ReadLine();
        Console.WriteLine("Введите коэффицент деления:");
        if (n3=="1"){
        Polynomia ob1 = p / Convert.ToDouble(Console.ReadLine());
        Console.WriteLine("Получился многочлен:");
        Console.WriteLine(ob1.ToString());}
        else{Polynomia ob1 = z / Convert.ToDouble(Console.ReadLine());
        Console.WriteLine("Получился многочлен:");
        Console.WriteLine(ob1.ToString());}
            break; 
        case "//":
        Console.WriteLine("Какой многочлен вы хотите изменить: 1 - дефолтный, 2 - свой");
        string n4=Console.ReadLine();
        Console.WriteLine("Введите коэффицент целочисленного деления:");
        if (n4=="1"){
        Polynomia ob1 =Polynomia.CeloChislennoeDelenie( p, Convert.ToDouble(Console.ReadLine()));
        Console.WriteLine("Получился многочлен:");
        Console.WriteLine(ob1.ToString());}
        else{Polynomia ob1 =Polynomia.CeloChislennoeDelenie( z, Convert.ToDouble(Console.ReadLine()));
        Console.WriteLine("Получился многочлен:");
        Console.WriteLine(ob1.ToString());}
            break;
        case "///":
        Polynomia  ob5 = p / z;
        Console.WriteLine("Получился многочлен:");
        Console.WriteLine(ob5.ToString());;
            break; 
        case "////":
        Polynomia  ob6 = Polynomia.CeloChislennoeDelenie(p, z);
        Console.WriteLine("Получился многочлен:");
        Console.WriteLine(ob6.ToString());;
            break;
        case "*":
Console.WriteLine("Какой многочлен вы хотите изменить: 1 - дефолтный, 2 - свой");
        string n5=Console.ReadLine();
        Console.WriteLine("Введите коэффицент умножения:");
        if (n5=="1"){
        Polynomia ob1 = p * Convert.ToDouble(Console.ReadLine());
        Console.WriteLine("Получился многочлен:");
        Console.WriteLine(ob1.ToString());}
        else{Polynomia ob1 = z * Convert.ToDouble(Console.ReadLine());
        Console.WriteLine("Получился многочлен:");
        Console.WriteLine(ob1.ToString());}
            break; 
        case "**":
        Polynomia  ob7 = p * z;
        Console.WriteLine("Получился многочлен:");
        Console.WriteLine(ob7.ToString());;
            break;
        case "^":
Console.WriteLine("Какой многочлен вы хотите изменить: 1 - дефолтный, 2 - свой");
        string n6=Console.ReadLine();
        Console.WriteLine("Введите коэффицент степени:");
        if (n6=="1"){
        Polynomia ob1 = Polynomia.Vstepen(p,  Convert.ToDouble(Console.ReadLine()));;
        Console.WriteLine("Получился многочлен:");
        Console.WriteLine(ob1.ToString());}
        else{Polynomia ob1 = Polynomia.Vstepen(z,  Convert.ToDouble(Console.ReadLine()));;
        Console.WriteLine("Получился многочлен:");
        Console.WriteLine(ob1.ToString());}
            break;
        case "v":
Console.WriteLine("Какой многочлен вы хотите изменить: 1 - дефолтный, 2 - свой");
        string n7=Console.ReadLine();
        Console.WriteLine("Введите коэффицент корня:");
        if (n7=="1"){
        Polynomia ob1 = Polynomia.Koreniz(p,  Convert.ToDouble(Console.ReadLine()));
        Console.WriteLine("Получился многочлен:");
        Console.WriteLine(ob1.ToString());}
        else{Polynomia ob1 =Polynomia.Koreniz(z,  Convert.ToDouble(Console.ReadLine()));
        Console.WriteLine("Получился многочлен:");
        Console.WriteLine(ob1.ToString());}
            break;
        case "stop":
        shouldexit = true;
        Console.WriteLine("Хорошего дня!");
            break; 
}
        }

        }
        else
            {
                while (!shouldexit)
                {
                    Console.WriteLine(@"Выберите опперацию которую хотите применить к вашему многочлену(будет только дефолтный, так как длина введеного вами не больше 0):
+ - Прибавить к каждому элементу многочлена введеный коеффицент
- - Вычесть из каждого элемента многочлена введеный коеффицент
/ - Поделить каждый элемент многочлена на введеный коеффицент
// - Поделить нацело каждый элемент многочлена на введеный коеффицент
* - Умножить каждый элемент многочлена на введеный коеффицент
^ - Возвоедение в степень введеного коеффицента членов многочлена
v - Корень из элементов многочлена степени введеного коеффицента коеффицента
stop - прервать действие функции
");
        switch(Console.ReadLine()){
        case "+":
        Console.WriteLine("Введите коеффицент для прибавления:");
        Polynomia ob1= p + Convert.ToDouble(Console.ReadLine());
        Console.WriteLine("Получился вот такой многочлен");
        Console.WriteLine(ob1.ToString());;
            break; 

        case "-":
        Console.WriteLine("Введите коеффицент для вычитания:");
        Polynomia ob2= p - Convert.ToDouble(Console.ReadLine());
        Console.WriteLine("Получился вот такой многочлен");
        Console.WriteLine(ob2.ToString());;
            break; 

        case "/":
        Console.WriteLine("Введите коеффицент для деления:");
        Polynomia ob3= p /  Convert.ToDouble(Console.ReadLine());
        Console.WriteLine("Получился вот такой многочлен");
        Console.WriteLine(ob3.ToString());;
            break; 
        case "//":
        Console.WriteLine("Введите коеффицент для деления нацело:");
        Polynomia ob4= Polynomia.CeloChislennoeDelenie(p, Convert.ToDouble(Console.ReadLine()));
        Console.WriteLine("Получился вот такой многочлен");
        Console.WriteLine(ob4.ToString());;
            break;

        case "*":
        Console.WriteLine("Введите коеффицент для умножения:");
        Polynomia ob5= p * Convert.ToDouble(Console.ReadLine());
        Console.WriteLine("Получился вот такой многочлен");
        Console.WriteLine(ob5.ToString());;
            break;
        case "^":
        Console.WriteLine("Введите коеффицент для возведения в степень:");
        Polynomia ob6= Polynomia.Vstepen(p, Convert.ToDouble(Console.ReadLine()));
        Console.WriteLine("Получился вот такой многочлен");
        Console.WriteLine(ob6.ToString());;
            break;
        case "v":
        Console.WriteLine("Введите коеффицент для получения корня:");
        Polynomia ob7= Polynomia.Koreniz(p, Convert.ToDouble(Console.ReadLine()));
        Console.WriteLine("Получился вот такой многочлен");
        Console.WriteLine(ob7.ToString());;
            break;
        case "stop":
            shouldexit = true;
            Console.WriteLine("Хорошего дня!");
            break; 
}
                }
            }
        }}}
