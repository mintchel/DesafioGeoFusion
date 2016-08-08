using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DesafioGeoFusion.Models
{
    public class EmailModels
    {  
        [Required(ErrorMessage = "Please enter your email address.")]
        public string Email {get;set;}
    }
}