﻿using System.ComponentModel.DataAnnotations;

namespace ContextLibrary.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        
    }
}
