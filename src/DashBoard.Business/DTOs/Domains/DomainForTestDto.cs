using System;
using System.ComponentModel.DataAnnotations;
using DashBoard.Data.Enums;

namespace DashBoard.Business.DTOs.Domains
{
    public class DomainForTestDto
    {
        [Required]
        //[Url] uzkomentuoju, nes neaisku, dar kokius linkus pinginsim
        public string Url { get; set; }
        [Range(0, 1)]
        public ServiceType Service_Type { get; set; }
        [Range(0, 1)]
        public RequestMethod Method { get; set; }
        public bool Basic_Auth { get; set; }
        public string Auth_User { get; set; }
        public string Auth_Password { get; set; } 
        public string Parameters { get; set; } 
        public bool Active { get; set; } = true; 

    }
}
