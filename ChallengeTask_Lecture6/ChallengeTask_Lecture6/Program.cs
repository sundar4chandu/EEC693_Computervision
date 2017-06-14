using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallengeTask_Lecture6
{
    class Program
    {
        static void Main(string[] args)
 
            {
                Console.WriteLine("Enter Numbers :");
                int[] number = new int[5];
                string number1 = "";
                for (int i = 0; i < 5; i++)
                {
                    number1 = Console.ReadLine();
                    number[i] = Convert.ToInt32(number1);
                }

                int a, b, temp = 0;
                Console.WriteLine("Sorted Numbers are");
                for (a = 0; a < number.Length; a++)
                {
                    for (b = a + 1; b < number.Length; b++)
                    {
                        if (number[a] > number[b])
                        {
                            temp = number[a];
                        number[a] = number[b];
                        number[b] = temp;
                        }
                    }
                    Console.WriteLine(number[a]);
                }
                Console.ReadLine();
            }
        }

    }
