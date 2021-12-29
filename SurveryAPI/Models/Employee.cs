namespace SurveyAPI.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Employee
    {
        [Key]
        public Guid Id { get; set; }
        public string Email { get; set; }
    }
}
