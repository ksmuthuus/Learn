using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Coaching.LINQ
{
    //Model Year,Division,Carline,Eng Displ,# Cyl,City FE,Hwy FE,Comb FE
    public class Car
    {
        public int YearModel { get; set; }
        public string Manufacturer { get; set; }
        public string Name { get; set; }
        public double City { get; set; }
        public double Highway { get; set; }
        public double Combined { get; set; }

        public IEnumerable<Car> ProcessCars(string path)
        {
            var query =
                File.ReadAllLines(path)
                .Where(line => line.Length > 1)
                .Skip(1)
                .Select(line =>
                {
                    var columns = line.Split(",");
                    return new Car
                    {
                        YearModel = int.Parse(columns[0]),
                        Manufacturer = columns[1],
                        Name = columns[2],
                        City = int.Parse(columns[5]),
                        Highway = int.Parse(columns[6]),
                        Combined = int.Parse(columns[7])

                    };
                });

            return query.ToList();
        }
    }
}
