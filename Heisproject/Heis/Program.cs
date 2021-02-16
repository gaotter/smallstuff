using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Heis
{
    class Program
    {
        static void Main(string[] args)
        {
            int numberOfFloors = 90;
            HeisControls heis = new HeisControls(numberOfFloors, 0);
            heis.Start();
            int select = 0;
            while (select != -1)
            {
                Console.WriteLine("Current floor is {0} \n Select floor", heis.CurrentFloor);

                string floor = Console.ReadLine();
                int selected;
                if (int.TryParse(floor, out selected) && selected > 0 && selected < numberOfFloors)
                {
                    heis.SelectFloor(selected);
                    Console.WriteLine("Tid til {0} etasje er {1} sekunder.", selected, heis.TimeToNextFloor(selected).TotalMinutes);
                }
                else
                {
                    
                    if(floor.Equals("alarm"))
                    {
                        heis.Alarm();
                    }
                    if (floor.Equals("stop"))
                    {
                        select = -1;
                    }
                    if (floor.Equals("start"))
                    {
                        heis.AlarmFixed();
                    }
                    Console.WriteLine("Please enter valid floor");

                }
            }

            heis.Stop();


        }
    }
}
