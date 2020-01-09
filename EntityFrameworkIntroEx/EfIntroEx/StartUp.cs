using System;
using System.Globalization;
using System.Linq;
using System.Text;
using SoftUni.Data;
using SoftUni.Models;

namespace SoftUni
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            SoftUniContext context = new SoftUniContext();

            using (context)
            {
                string result = string.Empty;

                //Problem 3 result = GetEmployeesFullInformation(context);
                //Problem 4 result = GetEmployeesWithSalaryOver50000(context);
                //Problem 5 result = GetEmployeesFromResearchAndDevelopment(context);
                //Problem 6 result = AddNewAddressToEmployee(context);
                //Problem 7 result = GetEmployeesInPeriod(context);
                //Problem 8 result = GetAddressesByTown(context);
                //Problem9 result = GetEmployee147(context);
                //Problem 10 result = GetDepartmentsWithMoreThan5Employees(context);
                //Problem 11 result = GetLatestProjects(context);
                //Problem 12 result = IncreaseSalaries(context);
                //Problem 13 result = GetEmployeesByFirstNameStartingWithSa(context);
                //Problem 14 result = DeleteProjectById(context);
                //Problem 15 result = RemoveTown(context);

                Console.WriteLine(result);
            }
        }

        public static string RemoveTown(SoftUniContext context)
        {
            int countDeletedAddresses = 0;

            var seattle = context.Towns.FirstOrDefault(t => t.Name == "Seattle");
            var seattleAddresses = context.Addresses.Where(a => a.Town.Name == "Seattle").ToArray();
            int seattleId = context.Towns.Where(t => t.Name == "Seattle").Select(i => i.TownId).Single();
            var employees = context.Employees.ToArray();

            foreach (var employee in employees)
            {
                if (employee.Address.TownId == seattleId)
                {
                    employee.Address.TownId = null;
                }
            }

            foreach (var address in seattleAddresses)
            {
                context.Addresses.Remove(address);
                countDeletedAddresses++;
            }

            context.Remove(seattle);
            context.SaveChanges();

            string result = $"{countDeletedAddresses} addresses in Seattle were deleted";
            return result;
        }

        public static string DeleteProjectById(SoftUniContext context)
        {
            var project = context.Projects.FirstOrDefault(p => p.ProjectId == 2);

            var empProject = context.EmployeesProjects.Where(x => x.ProjectId == 2).ToArray();

            foreach (var proj in empProject)
            {
                context.Remove(proj);
            }
            
            context.Remove(project);
            context.SaveChanges();

            var projects = context.Projects.Select(p => new
            {
                p.Name
            })
            .Take(10)
            .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var proj in projects)
            {
                sb.AppendLine(proj.Name);
            }

            return sb.ToString().Trim();
        }

        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            var employees = context.Employees.Where(n => n.FirstName.ToString().StartsWith("Sa")).Select(e => new
            {
                e.FirstName,
                e.LastName,
                e.JobTitle,
                e.Salary
            }).OrderBy(e => e.FirstName).ThenBy(e => e.LastName).ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var emp in employees)
            {
                sb.AppendLine($"{emp.FirstName} {emp.LastName} - {emp.JobTitle} - (${emp.Salary:f2})");
            }

            return sb.ToString().Trim();
        }

        public static string IncreaseSalaries(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(e => e.Department.Name == "Engineering" || e.Department.Name == "Tool Design" || e.Department.Name == "Marketing" || e.Department.Name == "Information Services")
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToArray();

            foreach (var emp in employees)
            {
                emp.Salary = emp.Salary * 1.12m;
            }

            context.SaveChanges();

            StringBuilder sb = new StringBuilder();

            foreach (var emp in employees)
            {
                sb.AppendLine($"{emp.FirstName} {emp.LastName} (${emp.Salary:f2})");
            }

            return sb.ToString().Trim();
        }

        public static string GetLatestProjects(SoftUniContext context)
        {
            var projects = context.Projects.Select(p => new
            {
                p.Name,
                p.Description,
                p.StartDate
            })
            .OrderByDescending(sd => sd.StartDate)
            .Take(10)
            .OrderBy(p => p.Name)
            .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var project in projects)
            {
                sb.AppendLine(project.Name);
                sb.AppendLine(project.Description);
                sb.AppendLine(project.StartDate.ToString("M/d/yyyy h:mm:ss tt"));
            }

            return sb.ToString().Trim();
        }

        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            var departments = context.Departments
                .Where(d => d.Employees.Count > 5)
                .Select(d => new
                {
                    d.Name,
                    employeeCount = d.Employees.Count(),
                    managerFullName = d.Manager.FirstName + " " + d.Manager.LastName,
                    employees = d.Employees.Select(e => new
                    {
                        e.FirstName,
                        e.LastName,
                        employeeFullName = e.FirstName + " " + e.LastName,
                        e.JobTitle
                    }
                )
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToArray()
                })
            .OrderBy(e => e.employeeCount)
            .ThenBy(d => d.Name)
            .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var department in departments)
            {
                sb.AppendLine($"{department.Name} – {department.managerFullName}");
                foreach (var emp in department.employees)
                {
                    sb.AppendLine($"{emp.employeeFullName} - {emp.JobTitle}");
                }
            }

            return sb.ToString().Trim();
        }

        public static string GetEmployee147(SoftUniContext context)
        {
            var employee = context.Employees.Select(e => new
            {
                FullName = e.FirstName + " " + e.LastName,
                e.JobTitle,
                projectNames = e.EmployeesProjects.Select(p => p.Project.Name).ToArray(),
                e.EmployeeId
            })
            .Where(e => e.EmployeeId == 147)
            .ToArray();

            StringBuilder sb = new StringBuilder();


            foreach (var emp in employee)
            {
                sb.AppendLine($"{emp.FullName} - {emp.JobTitle}");
                foreach (var project in emp.projectNames.OrderBy(n => n))
                {
                    sb.AppendLine(project);
                }
            }

            return sb.ToString().Trim();
        }

        public static string GetAddressesByTown(SoftUniContext context)
        {
            var adresses = context.Addresses.Select(x => new
            {
                x.AddressText,
                townName = x.Town.Name,
                count = x.Employees.Count()
            })
            .OrderByDescending(x => x.count)
            .ThenBy(x => x.townName)
            .ThenBy(x => x.AddressText)
            .Take(10).ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var ad in adresses)
            {
                sb.AppendLine($"{ad.AddressText}, {ad.townName} - {ad.count} employees");
            }

            return sb.ToString().Trim();
        }

        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(ep => ep.EmployeesProjects.Any(p => p.Project.StartDate.Year >= 2001 && p.Project.StartDate.Year <= 2003))
                .Select(e => new
                {
                    FullName = e.FirstName + " " + e.LastName,
                    ManagerName = e.Manager.FirstName + " " + e.Manager.LastName,
                    Projects = e.EmployeesProjects.Select(p => new
                    {
                        ProjectName = p.Project.Name,
                        ProjectStartDate = p.Project.StartDate,
                        ProjectEndDate = p.Project.EndDate
                    }).ToArray()
                })
                .Take(10).ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var emp in employees)
            {
                sb.AppendLine($"{emp.FullName} - Manager: {emp.ManagerName}");

                foreach (var project in emp.Projects)
                {
                    if (project.ProjectEndDate == null)
                    {
                        sb.AppendLine($"--{project.ProjectName} - {project.ProjectStartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)} - not finished");
                    }
                    else
                    {
                        sb.AppendLine($"--{project.ProjectName} - {project.ProjectStartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)} - {project.ProjectEndDate.Value.ToString("M/d/yyyy h:mm:ss tt")}");
                    }
                }
            }

            return sb.ToString().Trim();
        }

        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            var vitoshka = new Address
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };

            var nakov = context.Employees.FirstOrDefault(ln => ln.LastName == "Nakov");
            nakov.Address = vitoshka;

            context.SaveChanges();

            var empAddresses = context.Employees.OrderByDescending(e => e.AddressId)
                .Take(10).Select(x => x.Address.AddressText).ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var addres in empAddresses)
            {
                sb.AppendLine(addres);
            }

            return sb.ToString().Trim();
        }

        //Problem5
        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(e => e.Department.Name == "Research and Development")
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.Department.Name,
                    e.Salary
                }
                )
                .OrderBy(s => s.Salary)
                .ThenByDescending(fm => fm.FirstName)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var emp in employees)
            {
                sb.AppendLine($"{emp.FirstName} {emp.LastName} from Research and Development - ${emp.Salary:f2}");
            }

            return sb.ToString().Trim();
        }


        //Problem 4
        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            var employees = context.Employees.Where(e => e.Salary > 50000m).OrderBy(e => e.FirstName).ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var emp in employees)
            {
                sb.AppendLine($"{emp.FirstName} - {emp.Salary:f2}");
            }

            return sb.ToString().Trim();
        }

        //Problem 3
        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            var employees = context.Employees.ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var employee in employees.OrderBy(e => e.EmployeeId))
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} {employee.MiddleName} {employee.JobTitle} {employee.Salary:f2}");
            }

            return sb.ToString().Trim();
        }
    }
}