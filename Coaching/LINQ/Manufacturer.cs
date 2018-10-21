using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Coaching.LINQ
{
    public class Manufacturer
    {
        public string Name { get; set; }
        public string HeadQuarters { get; set; }
        public int Year { get; set; }

        public IEnumerable<Manufacturer> ProcessManufacturers(string path)
        {
            var query = 
                File.ReadAllLines(path)
                .Where(line => line.Length > 1)
                .Select(line =>
                {
                    var columns = line.Split(",");
                    return new Manufacturer
                    {
                        Name = columns[0],
                        HeadQuarters = columns[1],
                        Year = int.Parse(columns[2])
                    };
                });

            return query.ToList();
        }
    }
}
