using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CSProject
{
    class Staff
    {
        //Fields
        private float hourlyRate;
        private int hWorked;

        //Properties
        public float TotalPay
        {
            get;
            protected set;
        }
        public float BasicPay
        {
            get; private set;
        }
        public string NameOfStaff
        {
            get; private set;
        }
        public int HoursWorked
        {
            get { return hWorked; }
            set
            {
                if (value > 0)
                {
                    hWorked = value;
                }
                else
                {
                    hWorked = 0;
                }
            }
        }
        

        //Constructor
        public Staff(string name, float rate) {
            NameOfStaff = name;
            hourlyRate = rate;
        }

        //Methods
        public virtual void CalculatePay()
        {
            Console.WriteLine("Calculating pay ....");
            BasicPay = hWorked * hourlyRate;
            TotalPay = BasicPay;
        }
        public override string ToString()
        {
            return "Name of Staff = " + NameOfStaff + ", hourlyRate = " + hourlyRate + ", hWorked = " + hWorked + ", Basic pay = " + BasicPay + ", total pay = " + TotalPay;
        }

    }

    class Manager : Staff
    {
        //field
        private const float managerHourlyRate = 50;

        //property
        public int Allowance
        {
            get;
            private set;
        }
        
        //constructor
        public Manager(string name) : base(name,managerHourlyRate)
        {
            
        }

        public override void CalculatePay()
        {
            base.CalculatePay();
            if (HoursWorked > 160)
            {
                Allowance = 1000;
                TotalPay = TotalPay + Allowance;
            }
            else
            {
                Allowance = 0;
            }
        }
        public override string ToString()
        {
            return "Name of Staff = " + NameOfStaff + ", Allowance = " + Allowance + ", Basic pay = " + BasicPay + ", total pay = " + TotalPay;
        }
    }

    class Admin : Staff
    {
        //fields
        private const float overtimeRate = 15.5f;
        private const float adminHourlyRate = 30;

        //property
        public float Overtime
        {
            get;
            private set;
        }

        //constructor
        public Admin(string name) : base(name,adminHourlyRate)
        {
            
        }

        //methods
        public override void CalculatePay()
        {
            base.CalculatePay();
            if (HoursWorked > 160)
            {
                Overtime = overtimeRate * (HoursWorked - 160);
                TotalPay = TotalPay + Overtime;
            }
            else 
            {
                Overtime = 0;
            }
        }
        public override string ToString()
        {
            return "Name of Staff = " + NameOfStaff + ", Overtime = " + Overtime + ", Basic pay = " + BasicPay + ", total pay = " + TotalPay;
        }
    }

    class FileReader
    {
        //method
        public List<Staff> ReadFile()
        { 
            List<Staff> myStaff = new List<Staff>();
            string[] result = new string[2];
            string path = "staff.​txt";
            string[] separator = {", "};
            if (File.Exists(@"staff.txt"))
            {
                using (StreamReader sr = new StreamReader(@"staff.txt"))
                {
                    string s;
                    while (!sr.EndOfStream)
                    {
                        s = sr.ReadLine();
                        result = s.Split(separator,StringSplitOptions.RemoveEmptyEntries);
                        if (result[1].Equals("Manager"))
                        {
                            Manager mng = new Manager(result[0]);
                            myStaff.Add(mng);
                        }
                        else if (result[1].Equals("Admin"))
                        {
                            Admin adm = new Admin(result[0]);
                            myStaff.Add(adm);
                        }
                    }
                    sr.Close();
                }
            }
            else
            {
                Console.WriteLine("Error : File can't be found.");
            }
            return myStaff;
        }
    }

    class PaySlip
    {
        //fields
        private int month;
        private int year;
        enum MonthsOfYear {JAN = 1, FEB, MAR, APR, MAY, JUN, JUL, AUG, SEP, OCT, NOV, DEC};

        //constructor
        public PaySlip(int payMonth, int payYear)
        {
            month = payMonth;
            year = payYear;
        }

        //method
        public void GeneratePaySlip(List<Staff> myStaff)
        {
            string path;
            foreach(Staff s in myStaff)
            {
                path = s.NameOfStaff+".txt";
                StreamWriter sw = new StreamWriter(path);
                sw.WriteLine("PAYSLIP FOR {0} {1}", (MonthsOfYear)month,year);
                sw.WriteLine("==========================");
                sw.WriteLine("Name of Staff : "+s);
                sw.WriteLine("Hours Worked : "+s.HoursWorked);
                sw.WriteLine();
                sw.WriteLine("Basic Pay : "+s.BasicPay);
                if (s.GetType().ToString().Equals("Manager"))
                { 
                    sw.WriteLine("Allowance : "+((Manager)s).Allowance);
                }
                else if (s.GetType().ToString().Equals("Admin"))
                {
                    sw.WriteLine("Overtime : " + ((Admin)s).Overtime);
                }
                sw.WriteLine();
                sw.WriteLine("==========================");
                sw.WriteLine("Total Pay : " + s.TotalPay);
                sw.WriteLine("==========================");
                sw.Close();
            }
        }

        public void GenerateSummary(List<Staff> myStaff)
        {
            var result = from s in myStaff
                         where s.HoursWorked < 10
                         orderby s.NameOfStaff ascending
                         select new
                         {
                             s.NameOfStaff,
                             s.HoursWorked
                         };
                         
            string path = "summary.txt";
            StreamWriter sw = new StreamWriter(path);
            sw.WriteLine("Staff with less than 10 working hours");
            sw.WriteLine();
            foreach(var r in result)
            {
                sw.WriteLine("Name of Staff: {0}, Hours Worked: {1}",r.NameOfStaff, r.HoursWorked);
            }
            sw.Close();
        }
        public override string ToString()
        {
            return "finish";
        }
    }
    
    class Program
    {
        static void Main(string[] args)
        {
            List<Staff> myStaff = new List<Staff>();
            FileReader fr = new FileReader();
            int month=0;
            int year=0;

            while(year==0)
            {
                Console.Write("\nPlease enter the year: ");
                try
                {
                    year = Convert.ToInt32(Console.ReadLine());
                }
                catch (FormatException)
                {
                    Console.WriteLine("Wrong input");
                    year=0;
                }
            }
            while(month==0)
            {
                Console.Write("\nPlease enter the month: ");
                try
                {
                    month = Convert.ToInt32(Console.ReadLine());
                    if(month<1 || month>12)
                    {
                        Console.WriteLine("Invalid month");
                        month=0;
                    }
                }
                catch (FormatException)
                {
                    Console.WriteLine("Wrong input");
                    month=0;
                }
            }

            myStaff = fr.ReadFile();

            for (int i = 0; i < myStaff.Count; i++)
            {
                try
                {
                    Console.WriteLine("Enter hours worked for {0}",myStaff[i].NameOfStaff);
                    myStaff[i].HoursWorked = Convert.ToInt32(Console.ReadLine());
                    myStaff[i].CalculatePay();
                    Console.WriteLine(myStaff[i].ToString());

                }
                catch (Exception e)
                {
                    Console.WriteLine("Error");
                    i--;
                }
            }

            PaySlip ps = new PaySlip(month,year);
            ps.GeneratePaySlip(myStaff);
            ps.GenerateSummary(myStaff);

            Console.ReadLine();



        }
    }
}
