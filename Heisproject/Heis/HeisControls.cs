using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Heis
{
    class HeisControls
    {
        private int numberOfFloors;
        private int currentFloor;
        private List<int> floorsSelected;
        private HeisState heisState = HeisState.Running;
        private Thread floorRunner;
        private object threadKey = new object();
        private bool goingUp;
        private int nextFloor;
        private TimeSpan betweenFloorTime;

        public int CurrentFloor { get{ return currentFloor;} }

        public HeisState state { get { return heisState; } }

        public HeisControls(int numberOfFloors, int initialFloor)
        {
            this.numberOfFloors = numberOfFloors;
            currentFloor = initialFloor;
            floorsSelected = new List<int>(numberOfFloors);
            betweenFloorTime = new TimeSpan(0, 0, 3);
            heisState = HeisState.Running;
            floorRunner = new Thread(() =>
            {
                while (true)
                {
                    List<int> directionList = new List<int>();
                    lock (threadKey)
                    {
                        if (floorsSelected.Count > 0 && heisState == HeisState.Running)
                        {
                            if (floorsSelected.Contains(currentFloor))
                            {
                                Console.WriteLine("At {0}", currentFloor);
                                floorsSelected.Remove(floorsSelected.Where(a => a == currentFloor).First());
                                Thread.Sleep(betweenFloorTime);
                            }
                            else
                            {
                                
                                if (goingUp)
                                {
                                    
                                    var sameWay = from f in floorsSelected
                                                  where f > currentFloor
                                                  select f;

                                    if (sameWay.Count() > 0)
                                    {
                                        nextFloor = sameWay.Min();
                                    }
                                    else
                                    {
                                        goingUp = false;
                                        nextFloor = floorsSelected.Max();
                                    }

                                }
                                else
                                {
                                    var sameWay = from f in floorsSelected
                                                  where f < currentFloor
                                                  select f;

                                    if (sameWay.Count() > 0)
                                    {
                                        nextFloor = sameWay.Max();
                                    }
                                    else
                                    {
                                        goingUp = true;
                                        nextFloor = floorsSelected.Min();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if(heisState == HeisState.Running)
                                Console.WriteLine("No floor selected current flor is {0}", currentFloor);
                            else
                                Console.WriteLine("Alarm at {0}", currentFloor); 
                               

                        }


                    }

                    Thread.Sleep(betweenFloorTime);
                    if (nextFloor > currentFloor && heisState == HeisState.Running)
                    {
                        Console.WriteLine("Moving up");
                        currentFloor++;
                    }
                    else if(nextFloor < currentFloor && heisState == HeisState.Running)
                    {
                        Console.WriteLine("Moving down");
                        currentFloor--;
                    }
                    Console.WriteLine("Current floor {0}", currentFloor);


                }
            });
        }

        public void Start()
        {
            floorRunner.Start();
        }
        public void Stop()
        {
            floorRunner.Abort();
        }

        public bool SelectFloor(int floorSelected)
        {

            lock (threadKey)
            {
                if (!floorsSelected.Contains(floorSelected))
                {
                    floorsSelected.Add(floorSelected);

                }

            }

            return heisState == HeisState.Running;
        }

        public bool GoingUp()
        {
            return goingUp;
        }

        public TimeSpan TimeToNextFloor(int selecedFloor)
        {
            int numberOfFloorsToNext = Math.Abs(currentFloor - selecedFloor);

            long ticks = betweenFloorTime.Ticks * numberOfFloorsToNext;

            return new TimeSpan(ticks);
        }

        public void Alarm()
        {
            heisState = HeisState.Alarm;
        }

        public void AlarmFixed()
        {
            heisState = HeisState.Running;
        }


    }
}
