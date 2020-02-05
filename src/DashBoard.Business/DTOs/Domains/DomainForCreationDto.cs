using System.ComponentModel.DataAnnotations;
using DashBoard.Data.Enums;

namespace DashBoard.Business.DTOs.Domains
{
    public class DomainForCreationDto
    {
        [Required]
        public string Service_Name { get; set; }
        [Required]
        //[Url] uzkomentuoju, nes neaisku, dar kokius linkus pinginsim
        public string Url { get; set; }
        [Range(0, 2)]
        public ServiceType Service_Type { get; set; }
        [Range(0, 1)]
        public RequestMethod Method { get; set; }
        public bool Basic_Auth { get; set; }
        public string Auth_User { get; set; }
        public string Auth_Password { get; set; } //pagalvoti kaip saugoti PW
        public string Parameters { get; set; } //kaip cia parametrus, jei JSON arba XML ?
        [Required]
        [EmailAddress]
        public string Notification_Email { get; set; }
        [Range(3000, int.MaxValue, ErrorMessage = "Interval ms value must be more than 3000")]
        public int Interval_Ms { get; set; } = 600000; //default, jei nieko neiveda is front-end
        public bool Active { get; set; } = true; //Default reiksme
    }
}